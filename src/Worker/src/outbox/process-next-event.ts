import {
  botCaseEscalationRequestedEventType,
  botCaseEscalationRequestedEventV1Schema,
  executionJobResultRecordedEventType,
  executionJobResultRecordedEventV1Schema,
  supportRequestConfirmedEventType,
  supportRequestConfirmedEventV1Schema,
} from "@it-support-native/control-plane-contracts";
import type { Pool } from "pg";
import type { ITicketingProvider } from "../ticketing/application/ticketing-provider.js";
import { persistExternalTicket } from "../ticketing/infrastructure/external-ticket-store.js";
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
  ticketingProvider: ITicketingProvider,
): Promise<ProcessNextEventResult> {
  const event = await claimNextOutboxEvent(pool, workerId);
  if (event === null) {
    return { kind: "idle" };
  }

  const parsedEvent = parseEvent(event);
  if (parsedEvent === null) {
    return handleFailure(pool, event, "invalid_payload");
  }

  const client = await pool.connect();
  try {
    await client.query("BEGIN");
    const effect =
      parsedEvent.kind === "ticket"
        ? await createExternalTicketEffect(
            client,
            event,
            ticketingProvider,
            parsedEvent.escalation,
          )
        : parsedEvent;
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

type ParsedEvent =
  | {
      readonly kind: "effect";
      readonly effectType: string;
      readonly payload: unknown;
    }
  | {
      readonly kind: "ticket";
      readonly escalation: ReturnType<
        typeof botCaseEscalationRequestedEventV1Schema.parse
      >;
    };

function parseEvent(
  event: ClaimedOutboxEvent,
): ParsedEvent | null {
  if (event.eventType === supportRequestConfirmedEventType) {
    const parsed = supportRequestConfirmedEventV1Schema.safeParse(event.payload);
    return parsed.success
      ? {
          kind: "effect",
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
          kind: "effect",
          effectType: "execution-job.result-recorded.v1",
          payload: parsed.data,
        }
      : null;
  }

  if (event.eventType === botCaseEscalationRequestedEventType) {
    const parsed = botCaseEscalationRequestedEventV1Schema.safeParse(
      event.payload,
    );
    return parsed.success
      ? {
          kind: "ticket",
          escalation: parsed.data,
        }
      : null;
  }

  return null;
}

async function createExternalTicketEffect(
  client: Parameters<typeof persistExternalTicket>[0],
  event: ClaimedOutboxEvent,
  ticketingProvider: ITicketingProvider,
  escalation: ReturnType<
    typeof botCaseEscalationRequestedEventV1Schema.parse
  >,
): Promise<{
  readonly effectType: string;
  readonly payload: unknown;
}> {
  const ticket = await persistExternalTicket(
    client,
    event,
    ticketingProvider.createTicket(escalation),
  );

  return {
    effectType: "bot-case.external-ticket-created.v1",
    payload: {
      ticketId: ticket.id,
      caseId: ticket.caseId,
      externalReference: ticket.externalReference,
      result: "ticket_created",
    },
  };
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
