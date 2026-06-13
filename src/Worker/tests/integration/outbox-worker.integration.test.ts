import { afterAll, beforeAll, describe, expect, it } from "vitest";
import {
  supportRequestConfirmedEventType,
  type SupportRequestConfirmedEventV1,
} from "@it-support-native/control-plane-contracts";
import { closeWorkerPool, getWorkerPool } from "../../src/platform/database";
import { processNextOutboxEvent } from "../../src/outbox/process-next-event";

describe("outbox worker", () => {
  const pool = getWorkerPool();

  beforeAll(async () => {
    await pool.query("DELETE FROM synthetic_outbox_effects");
    await pool.query("DELETE FROM outbox_events");
    await pool.query("DELETE FROM execution_jobs");
    await pool.query("DELETE FROM support_requests");

    const requestId = crypto.randomUUID();
    const jobId = crypto.randomUUID();
    const event: SupportRequestConfirmedEventV1 = {
      version: 1,
      requestId,
      jobId,
      correlationId: "worker-integration-correlation",
      actorSubject: "development-user-001",
      deviceId: "local-device-001",
      actionId: "software.install.simulated.v1",
      productId: "secure-transfer",
      productVersion: "6.5",
    };

    const client = await pool.connect();
    try {
      await client.query("BEGIN");
      await client.query(
        `
            INSERT INTO support_requests (
              id,
              reference,
              idempotency_key,
              idempotency_hash,
              correlation_id,
              requester_subject,
              device_id,
              product_id,
              product_version,
              action_id,
              status,
              created_at,
              updated_at
            )
            VALUES (
              $1,
              $2,
              $3,
              $4,
              $5,
              $6,
              $7,
              $8,
              $9,
              $10,
              'CONFIRMED',
              clock_timestamp(),
              clock_timestamp()
            )
        `,
        [
          requestId,
          `REQ-${requestId}`,
          `worker-integration-${requestId}`,
          "0".repeat(64),
          event.correlationId,
          event.actorSubject,
          event.deviceId,
          event.productId,
          event.productVersion,
          event.actionId,
        ],
      );
      await client.query(
        `
            INSERT INTO execution_jobs (
              id,
              request_id,
              action_id,
              target_id,
              target_version,
              status,
              created_at,
              updated_at
            )
            VALUES (
              $1,
              $2,
              $3,
              $4,
              $5,
              'QUEUED',
              clock_timestamp(),
              clock_timestamp()
            )
        `,
        [
          jobId,
          requestId,
          event.actionId,
          event.productId,
          event.productVersion,
        ],
      );
      await client.query(
        `
          INSERT INTO outbox_events (
            id,
            aggregate_type,
            aggregate_id,
            event_type,
            payload,
            status,
            attempt_count,
            available_at,
            created_at
          )
          VALUES ($1, $2, $3, $4, $5::jsonb, 'PENDING', 0, now(), now())
        `,
        [
          crypto.randomUUID(),
          "support-request",
          requestId,
          supportRequestConfirmedEventType,
          JSON.stringify(event),
        ],
      );
      await client.query("COMMIT");
    } catch (error) {
      await client.query("ROLLBACK");
      throw error;
    } finally {
      client.release();
    }
  });

  afterAll(async () => {
    await closeWorkerPool();
  });

  it("claims and completes one event without duplicating its effect", async () => {
    const ready = await pool.query<{
      status: string;
      attempt_count: number;
      ready: boolean;
    }>(
      `
        SELECT
          status::text,
          attempt_count,
          available_at <= clock_timestamp() AS ready
        FROM outbox_events
        ORDER BY created_at
      `,
    );

    expect(ready.rows).toEqual([
      {
        status: "PENDING",
        attempt_count: 0,
        ready: true,
      },
    ]);

    const first = await processNextOutboxEvent(
      pool,
      "worker-integration-test",
    );
    const second = await processNextOutboxEvent(
      pool,
      "worker-integration-test",
    );
    const effects = await pool.query<{ count: string }>(
      `
        SELECT count(*)::text AS count
        FROM synthetic_outbox_effects
      `,
    );
    const completed = await pool.query<{ count: string }>(
      `
        SELECT count(*)::text AS count
        FROM outbox_events
        WHERE status = 'COMPLETED'
      `,
    );
    const states = await pool.query<{
      request_status: string;
      job_status: string;
    }>(
      `
        SELECT
          request.status::text AS request_status,
          job.status::text AS job_status
        FROM support_requests AS request
        JOIN execution_jobs AS job ON job.request_id = request.id
      `,
    );

    expect(first.kind).toBe("completed");
    expect(second.kind).toBe("idle");
    expect(effects.rows[0]?.count).toBe("1");
    expect(completed.rows[0]?.count).toBe("1");
    expect(states.rows).toEqual([
      {
        request_status: "COMPLETED",
        job_status: "COMPLETED",
      },
    ]);
  });

  it("moves an invalid event to failed after three bounded attempts", async () => {
    const invalidEventId = crypto.randomUUID();
    await pool.query(
      `
        INSERT INTO outbox_events (
          id,
          aggregate_type,
          aggregate_id,
          event_type,
          payload,
          status,
          attempt_count,
          available_at,
          created_at
        )
        VALUES ($1, 'support-request', $2, $3, '{}'::jsonb, 'PENDING', 0, now(), now())
      `,
      [
        invalidEventId,
        crypto.randomUUID(),
        supportRequestConfirmedEventType,
      ],
    );

    const first = await processNextOutboxEvent(pool, "worker-retry-test");
    await makeAvailable(invalidEventId);
    const second = await processNextOutboxEvent(pool, "worker-retry-test");
    await makeAvailable(invalidEventId);
    const third = await processNextOutboxEvent(pool, "worker-retry-test");
    const state = await pool.query<{
      status: string;
      attempt_count: number;
      last_error_code: string;
    }>(
      `
        SELECT status::text, attempt_count, last_error_code
        FROM outbox_events
        WHERE id = $1
      `,
      [invalidEventId],
    );

    expect(first.kind).toBe("retry_scheduled");
    expect(second.kind).toBe("retry_scheduled");
    expect(third.kind).toBe("failed");
    expect(state.rows).toEqual([
      {
        status: "FAILED",
        attempt_count: 3,
        last_error_code: "invalid_payload",
      },
    ]);
  });

  it("rolls back the effect when a valid event references missing records", async () => {
    const orphanEventId = crypto.randomUUID();
    const orphanEvent: SupportRequestConfirmedEventV1 = {
      version: 1,
      requestId: crypto.randomUUID(),
      jobId: crypto.randomUUID(),
      correlationId: "worker-orphan-correlation",
      actorSubject: "development-user-001",
      deviceId: "local-device-001",
      actionId: "software.install.simulated.v1",
      productId: "secure-transfer",
      productVersion: "6.5",
    };
    await pool.query(
      `
        INSERT INTO outbox_events (
          id,
          aggregate_type,
          aggregate_id,
          event_type,
          payload,
          status,
          attempt_count,
          available_at,
          created_at
        )
        VALUES ($1, 'support-request', $2, $3, $4::jsonb, 'PENDING', 0, now(), now())
      `,
      [
        orphanEventId,
        orphanEvent.requestId,
        supportRequestConfirmedEventType,
        JSON.stringify(orphanEvent),
      ],
    );

    const result = await processNextOutboxEvent(pool, "worker-orphan-test");
    const outbox = await pool.query<{
      status: string;
      attempt_count: number;
      last_error_code: string;
    }>(
      `
        SELECT status::text, attempt_count, last_error_code
        FROM outbox_events
        WHERE id = $1
      `,
      [orphanEventId],
    );
    const effects = await pool.query<{ count: string }>(
      `
        SELECT count(*)::text AS count
        FROM synthetic_outbox_effects
        WHERE outbox_event_id = $1
      `,
      [orphanEventId],
    );

    expect(result.kind).toBe("retry_scheduled");
    expect(outbox.rows).toEqual([
      {
        status: "PENDING",
        attempt_count: 1,
        last_error_code: "processing_failed",
      },
    ]);
    expect(effects.rows[0]?.count).toBe("0");
  });

  async function makeAvailable(eventId: string): Promise<void> {
    await pool.query(
      `
        UPDATE outbox_events
        SET available_at = clock_timestamp()
        WHERE id = $1
      `,
      [eventId],
    );
  }
});
