import { z } from "zod";
import {
  boundedIdentifierSchema,
  correlationIdSchema,
  idempotencyKeySchema,
  requestIdSchema,
} from "./http-v1";

export const conversationChannelVersion = 1 as const;

export const conversationChannelActionSchema = z.enum([
  "catalog.query",
  "software.request",
  "proposal.continue",
  "request.confirm",
  "conversation.cancel",
  "request.status",
  "case.status",
]);

const channelBaseInputSchema = z
  .object({
    version: z.literal(conversationChannelVersion),
    messageId: boundedIdentifierSchema.max(100),
    correlationId: correlationIdSchema,
    sessionId: boundedIdentifierSchema.max(100),
    actorSubject: boundedIdentifierSchema,
    deviceId: boundedIdentifierSchema,
  })
  .strict();

export const conversationChannelInputSchema = z.discriminatedUnion("action", [
  channelBaseInputSchema.extend({
    action: z.literal("catalog.query"),
    productReference: boundedIdentifierSchema,
    requestId: z.null(),
    idempotencyKey: z.null(),
  }),
  channelBaseInputSchema.extend({
    action: z.literal("software.request"),
    productReference: boundedIdentifierSchema,
    requestId: z.null(),
    idempotencyKey: z.null(),
  }),
  channelBaseInputSchema.extend({
    action: z.literal("proposal.continue"),
    productReference: z.null(),
    requestId: z.null(),
    idempotencyKey: z.null(),
  }),
  channelBaseInputSchema.extend({
    action: z.literal("request.confirm"),
    productReference: z.null(),
    requestId: z.null(),
    idempotencyKey: idempotencyKeySchema,
  }),
  channelBaseInputSchema.extend({
    action: z.literal("conversation.cancel"),
    productReference: z.null(),
    requestId: z.null(),
    idempotencyKey: z.null(),
  }),
  channelBaseInputSchema.extend({
    action: z.literal("request.status"),
    productReference: z.null(),
    requestId: requestIdSchema,
    idempotencyKey: z.null(),
  }),
  channelBaseInputSchema.extend({
    action: z.literal("case.status"),
    productReference: z.null(),
    requestId: requestIdSchema,
    idempotencyKey: z.null(),
  }),
]);

export const conversationChannelDecisionSchema = z
  .object({
    kind: z.enum(["inform", "propose", "escalate", "reject"]),
    effectiveStatus: z.enum([
      "approved",
      "unlisted",
      "end_of_life",
      "prohibited",
    ]),
    productId: boundedIdentifierSchema.nullable(),
    productName: z.string().min(1).max(120).nullable(),
    productVersion: boundedIdentifierSchema.nullable(),
    alternatives: z
      .array(
        z
          .object({
            productId: boundedIdentifierSchema,
            productName: z.string().min(1).max(120),
            productVersion: boundedIdentifierSchema,
          })
          .strict(),
      )
      .max(20),
  })
  .strict();

export const conversationChannelRequestViewSchema = z
  .object({
    kind: z.enum(["software_acquisition", "human_review"]),
    productId: boundedIdentifierSchema,
    productVersion: boundedIdentifierSchema,
    idempotencyKey: idempotencyKeySchema,
    syntheticReference: boundedIdentifierSchema,
    persistedRequestId: requestIdSchema.nullable(),
    persistedReference: boundedIdentifierSchema.nullable(),
    replayed: z.boolean().nullable(),
  })
  .strict();

export const conversationChannelStatusViewSchema = z
  .object({
    requestId: requestIdSchema,
    requestStatus: z
      .enum(["confirmed", "completed", "failed"])
      .nullable(),
    jobStatus: z.enum(["queued", "running", "completed", "failed"]).nullable(),
    caseStatus: z
      .enum(["open", "attended_waiting_user", "escalated"])
      .nullable(),
    caseResult: z.enum(["pending", "succeeded", "failed"]).nullable(),
    externalTicketReference: boundedIdentifierSchema.nullable(),
  })
  .strict();

export const conversationChannelOutputSchema = z
  .object({
    version: z.literal(conversationChannelVersion),
    messageId: boundedIdentifierSchema.max(100),
    correlationId: correlationIdSchema,
    sessionId: boundedIdentifierSchema.max(100),
    state: z.enum([
      "query",
      "proposal",
      "confirmation_required",
      "request_created",
      "cancelled",
    ]),
    resultCode: z.enum([
      "query_answered",
      "proposal_ready",
      "confirmation_required",
      "request_created",
      "cancelled",
      "rejected",
      "invalid_transition",
      "duplicate_command",
      "request_status_returned",
      "case_status_returned",
      "capability_unavailable",
      "control_plane_unavailable",
    ]),
    decision: conversationChannelDecisionSchema.nullable(),
    request: conversationChannelRequestViewSchema.nullable(),
    status: conversationChannelStatusViewSchema.nullable(),
    error: z
      .object({
        code: boundedIdentifierSchema,
        message: z.string().min(1).max(200),
      })
      .strict()
      .nullable(),
  })
  .strict();

export type ConversationChannelInput = z.infer<
  typeof conversationChannelInputSchema
>;
export type ConversationChannelOutput = z.infer<
  typeof conversationChannelOutputSchema
>;
