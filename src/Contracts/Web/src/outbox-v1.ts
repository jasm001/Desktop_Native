import { z } from "zod";
import {
  boundedIdentifierSchema,
  correlationIdSchema,
  requestIdSchema,
} from "./http-v1";

export const supportRequestConfirmedEventType =
  "support-request.confirmed.v1" as const;
export const executionJobResultRecordedEventType =
  "execution-job.result-recorded.v1" as const;
export const botCaseEscalationRequestedEventType =
  "bot-case.escalation-requested.v1" as const;

export const supportRequestConfirmedEventV1Schema = z
  .object({
    version: z.literal(1),
    requestId: requestIdSchema,
    jobId: requestIdSchema,
    correlationId: correlationIdSchema,
    actorSubject: boundedIdentifierSchema,
    deviceId: boundedIdentifierSchema,
    actionId: boundedIdentifierSchema,
    productId: boundedIdentifierSchema,
    productVersion: boundedIdentifierSchema,
  })
  .strict();

export type SupportRequestConfirmedEventV1 = z.infer<
  typeof supportRequestConfirmedEventV1Schema
>;

export const executionJobResultRecordedEventV1Schema = z
  .object({
    version: z.literal(1),
    requestId: requestIdSchema,
    jobId: requestIdSchema,
    correlationId: correlationIdSchema,
    deviceId: boundedIdentifierSchema,
    result: z.enum(["succeeded", "failed"]),
  })
  .strict();

export type ExecutionJobResultRecordedEventV1 = z.infer<
  typeof executionJobResultRecordedEventV1Schema
>;

export const botCaseEscalationReasonSchema = z.enum([
  "execution_failed",
  "claim_lease_exhausted",
]);

export const botCaseEscalationRequestedEventV1Schema = z
  .object({
    version: z.literal(1),
    caseId: requestIdSchema,
    requestId: requestIdSchema,
    jobId: requestIdSchema,
    correlationId: correlationIdSchema,
    category: z.literal("software_installation"),
    reasonCode: botCaseEscalationReasonSchema,
    productId: boundedIdentifierSchema,
    productVersion: boundedIdentifierSchema,
  })
  .strict();

export type BotCaseEscalationRequestedEventV1 = z.infer<
  typeof botCaseEscalationRequestedEventV1Schema
>;
