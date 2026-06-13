import {
  supportRequestConfirmedEventType,
  supportRequestConfirmedEventV1Schema,
} from "@it-support-native/control-plane-contracts";
import type { Pool } from "pg";
import {
  claimNextOutboxEvent,
  completeSyntheticEvent,
  recordOutboxFailure,
  type ClaimedOutboxEvent,
} from "./outbox-store";

export type ProcessNextEventResult =
  | { readonly kind: "idle" }
  | { readonly kind: "completed"; readonly eventId: string }
  | { readonly kind: "retry_scheduled"; readonly eventId: string }
  | { readonly kind: "failed"; readonly eventId: string };

export async function processNextOutboxEvent(
  pool: Pool,
  workerId: string,
): Promise<ProcessNextEventResult> {
  const event = await claimNextOutboxEvent(pool, workerId);
  if (event === null) {
    return { kind: "idle" };
  }

  if (event.eventType !== supportRequestConfirmedEventType) {
    return handleFailure(pool, event, "unsupported_event");
  }

  const parsed = supportRequestConfirmedEventV1Schema.safeParse(event.payload);
  if (!parsed.success) {
    return handleFailure(pool, event, "invalid_payload");
  }

  const client = await pool.connect();
  try {
    await client.query("BEGIN");
    await completeSyntheticEvent(client, event, {
      requestId: parsed.data.requestId,
      jobId: parsed.data.jobId,
      result: "synthetic_completed",
    });
    await client.query("COMMIT");
    return { kind: "completed", eventId: event.id };
  } catch {
    await client.query("ROLLBACK");
    return await handleFailure(pool, event, "processing_failed");
  } finally {
    client.release();
  }
}

async function handleFailure(
  pool: Pool,
  event: ClaimedOutboxEvent,
  errorCode: string,
): Promise<ProcessNextEventResult> {
  await recordOutboxFailure(pool, event, errorCode);
  return {
    kind: event.attemptCount >= 3 ? "failed" : "retry_scheduled",
    eventId: event.id,
  };
}
