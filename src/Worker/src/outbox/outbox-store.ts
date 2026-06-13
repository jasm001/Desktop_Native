import type { Pool, PoolClient } from "pg";

const maximumAttempts = 3;

export interface ClaimedOutboxEvent {
  readonly id: string;
  readonly eventType: string;
  readonly payload: unknown;
  readonly attemptCount: number;
  readonly claimedBy: string;
}

export async function claimNextOutboxEvent(
  pool: Pool,
  workerId: string,
): Promise<ClaimedOutboxEvent | null> {
  const result = await pool.query<{
    id: string;
    event_type: string;
    payload: unknown;
    attempt_count: number;
    claimed_by: string;
  }>(
    `
      WITH candidate AS (
        SELECT id
        FROM outbox_events
        WHERE attempt_count < $2
          AND (
            (status = 'PENDING' AND available_at <= now())
            OR (
              status = 'PROCESSING'
              AND claimed_at < now() - interval '30 seconds'
            )
          )
        ORDER BY available_at, created_at, id
        FOR UPDATE SKIP LOCKED
        LIMIT 1
      )
      UPDATE outbox_events AS event
      SET
        status = 'PROCESSING',
        attempt_count = event.attempt_count + 1,
        claimed_at = now(),
        claimed_by = $1,
        last_error_code = NULL
      FROM candidate
      WHERE event.id = candidate.id
      RETURNING
        event.id,
        event.event_type,
        event.payload,
        event.attempt_count,
        event.claimed_by
    `,
    [workerId, maximumAttempts],
  );

  const row = result.rows[0];
  return row === undefined
    ? null
    : {
        id: row.id,
        eventType: row.event_type,
        payload: row.payload,
        attemptCount: row.attempt_count,
        claimedBy: row.claimed_by,
      };
}

export async function completeOutboxEvent(
  client: PoolClient,
  event: ClaimedOutboxEvent,
  effectType: string,
  effectPayload: unknown,
): Promise<void> {
  await client.query(
    `
      INSERT INTO synthetic_outbox_effects (
        id,
        outbox_event_id,
        effect_type,
        payload,
        created_at
      )
      VALUES ($1, $2, $3, $4::jsonb, now())
      ON CONFLICT (outbox_event_id) DO NOTHING
    `,
    [
      crypto.randomUUID(),
      event.id,
      effectType,
      JSON.stringify(effectPayload),
    ],
  );

  const result = await client.query(
    `
      UPDATE outbox_events
      SET
        status = 'COMPLETED',
        completed_at = now(),
        claimed_at = NULL,
        claimed_by = NULL,
        last_error_code = NULL
      WHERE id = $1
        AND status = 'PROCESSING'
        AND claimed_by = $2
    `,
    [event.id, event.claimedBy],
  );

  if (result.rowCount !== 1) {
    throw new Error("outbox_claim_lost");
  }
}

export async function recordOutboxFailure(
  pool: Pool,
  event: ClaimedOutboxEvent,
  errorCode: string,
): Promise<void> {
  const terminal = event.attemptCount >= maximumAttempts;
  const retrySeconds = Math.min(event.attemptCount * 2, 10);

  if (terminal) {
    const result = await pool.query(
      `
        UPDATE outbox_events
        SET
          status = 'FAILED',
          claimed_at = NULL,
          claimed_by = NULL,
          last_error_code = $3
        WHERE id = $1
          AND status = 'PROCESSING'
          AND claimed_by = $2
      `,
      [event.id, event.claimedBy, errorCode],
    );
    assertClaimUpdated(result.rowCount);
    return;
  }

  const result = await pool.query(
    `
      UPDATE outbox_events
      SET
        status = 'PENDING',
        available_at = clock_timestamp() + ($3 * interval '1 second'),
        claimed_at = NULL,
        claimed_by = NULL,
        last_error_code = $4
      WHERE id = $1
        AND status = 'PROCESSING'
        AND claimed_by = $2
    `,
    [event.id, event.claimedBy, retrySeconds, errorCode],
  );
  assertClaimUpdated(result.rowCount);
}

function assertClaimUpdated(rowCount: number | null): void {
  if (rowCount !== 1) {
    throw new Error("outbox_claim_lost");
  }
}
