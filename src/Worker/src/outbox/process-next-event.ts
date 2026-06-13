import {
  executionJobResultRecordedEventType,
  executionJobResultRecordedEventV1Schema,
  supportRequestConfirmedEventType,
  supportRequestConfirmedEventV1Schema,
} from "@it-support-native/control-plane-contracts";
import type { Pool } from "pg";
import {
  claimNextOutboxEvent,
  completeOutboxEvent,
  recordOutboxFailure,
  type ClaimedOutboxEvent,
} from "./outbox-store.js";

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

  const effect = parseEffect(event);
  if (effect === null) {
    return handleFailure(pool, event, "invalid_payload");
  }

  const client = await pool.connect();
  try {
    await client.query("BEGIN");
    await completeOutboxEvent(
      client,
      event,
      effect.effectType,
      effect.payload,
    );
    await client.query("COMMIT");
    return { kind: "completed", eventId: event.id };
  } catch {
    await client.query("ROLLBACK");
    return await handleFailure(pool, event, "processing_failed");
  } finally {
    client.release();
  }
}

function parseEffect(
  event: ClaimedOutboxEvent,
): { readonly effectType: string; readonly payload: unknown } | null {
  if (event.eventType === supportRequestConfirmedEventType) {
    const parsed = supportRequestConfirmedEventV1Schema.safeParse(event.payload);
    return parsed.success
      ? {
          effectType: "support-request.dispatch-ready.v1",
          payload: {
            requestId: parsed.data.requestId,
            jobId: parsed.data.jobId,
            result: "dispatch_ready",
          },
        }
      : null;
  }

  if (event.eventType === executionJobResultRecordedEventType) {
    const parsed = executionJobResultRecordedEventV1Schema.safeParse(
      event.payload,
    );
    return parsed.success
      ? {
          effectType: "execution-job.result-recorded.v1",
          payload: parsed.data,
        }
      : null;
  }

  return null;
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
