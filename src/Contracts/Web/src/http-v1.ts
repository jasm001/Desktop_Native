import { z } from "zod";

export const apiVersion = "v1" as const;

export const boundedIdentifierSchema = z
  .string()
  .trim()
  .min(1)
  .max(128)
  .regex(/^[A-Za-z0-9][A-Za-z0-9._:-]*$/u);

export const correlationIdSchema = boundedIdentifierSchema.max(100);
export const idempotencyKeySchema = boundedIdentifierSchema.max(100);
export const requestIdSchema = z.uuid();

export const apiMetaSchema = z
  .object({
    apiVersion: z.literal(apiVersion),
    correlationId: correlationIdSchema,
  })
  .strict();

export const apiErrorCodeSchema = z.enum([
  "invalid_request",
  "identity_unavailable",
  "device_not_found",
  "request_not_found",
  "case_not_found",
  "idempotency_conflict",
  "agent_identity_unavailable",
  "agent_claim_invalid",
  "internal_error",
]);

export const apiErrorEnvelopeSchema = z
  .object({
    error: z
      .object({
        code: apiErrorCodeSchema,
        message: z.string().min(1).max(200),
      })
      .strict(),
    meta: apiMetaSchema,
  })
  .strict();

export const createSoftwareInstallationRequestSchema = z
  .object({
    confirmed: z.literal(true),
    deviceId: boundedIdentifierSchema,
    productId: boundedIdentifierSchema,
    productVersion: boundedIdentifierSchema,
    actionId: boundedIdentifierSchema,
  })
  .strict();

export const supportRequestStatusSchema = z.enum([
  "confirmed",
  "completed",
  "failed",
]);

export const executionJobStatusSchema = z.enum([
  "queued",
  "running",
  "completed",
  "failed",
]);

export const executionEvidenceCodeSchema = z.enum([
  "job.accepted",
  "job.simulation.started",
  "job.simulation.verified",
  "job.simulation.failed",
]);

export const executionEvidenceSchema = z
  .object({
    code: executionEvidenceCodeSchema,
    summary: z.string().min(1).max(200),
    recordedAt: z.iso.datetime(),
  })
  .strict();

export const supportRequestViewSchema = z
  .object({
    id: requestIdSchema,
    reference: boundedIdentifierSchema,
    correlationId: correlationIdSchema,
    status: supportRequestStatusSchema,
    deviceId: boundedIdentifierSchema,
    productId: boundedIdentifierSchema,
    productVersion: boundedIdentifierSchema,
    actionId: boundedIdentifierSchema,
    createdAt: z.iso.datetime(),
    job: z
      .object({
        id: requestIdSchema,
        status: executionJobStatusSchema,
        evidence: z.array(executionEvidenceSchema).max(20),
      })
      .strict(),
  })
  .strict();

export const createSoftwareInstallationResponseSchema = z
  .object({
    data: z
      .object({
        request: supportRequestViewSchema,
        replayed: z.boolean(),
      })
      .strict(),
    meta: apiMetaSchema,
  })
  .strict();

export const getSupportRequestResponseSchema = z
  .object({
    data: z
      .object({
        request: supportRequestViewSchema,
      })
      .strict(),
    meta: apiMetaSchema,
  })
  .strict();

export const botCaseCategorySchema = z.enum(["software_installation"]);

export const botCaseStatusSchema = z.enum([
  "open",
  "attended_waiting_user",
  "escalated",
]);

export const botCaseResultSchema = z.enum([
  "pending",
  "succeeded",
  "failed",
]);

export const externalTicketViewSchema = z
  .object({
    id: requestIdSchema,
    provider: z.literal("fake"),
    reference: boundedIdentifierSchema,
    category: botCaseCategorySchema,
    status: z.literal("open"),
    correlationId: correlationIdSchema,
    reasonCode: z.enum(["execution_failed", "claim_lease_exhausted"]),
    description: z.string().min(1).max(200),
    createdAt: z.iso.datetime(),
  })
  .strict();

export const botCaseViewSchema = z
  .object({
    id: requestIdSchema,
    requestId: requestIdSchema,
    correlationId: correlationIdSchema,
    category: botCaseCategorySchema,
    status: botCaseStatusSchema,
    result: botCaseResultSchema,
    waitingForUserSince: z.iso.datetime().nullable(),
    escalatedAt: z.iso.datetime().nullable(),
    createdAt: z.iso.datetime(),
    updatedAt: z.iso.datetime(),
    externalTicket: externalTicketViewSchema.nullable(),
  })
  .strict();

export const getBotCaseResponseSchema = z
  .object({
    data: z
      .object({
        case: botCaseViewSchema,
      })
      .strict(),
    meta: apiMetaSchema,
  })
  .strict();

export const catalogProductSchema = z
  .object({
    id: boundedIdentifierSchema,
    name: z.string().min(1).max(120),
    version: boundedIdentifierSchema,
    status: z.enum(["approved", "unlisted", "end_of_life", "prohibited"]),
    actionId: boundedIdentifierSchema.nullable(),
  })
  .strict();

export const catalogQuerySchema = z
  .object({
    limit: z.coerce.number().int().min(1).max(50).default(20),
    cursor: boundedIdentifierSchema.optional(),
  })
  .strict();

export const catalogResponseSchema = z
  .object({
    data: z
      .object({
        items: z.array(catalogProductSchema).max(50),
        nextCursor: boundedIdentifierSchema.nullable(),
      })
      .strict(),
    meta: apiMetaSchema,
  })
  .strict();

export const claimAgentJobRequestSchema = z
  .object({
    deviceId: boundedIdentifierSchema,
  })
  .strict();

export const claimedAgentJobSchema = z
  .object({
    jobId: requestIdSchema,
    requestId: requestIdSchema,
    idempotencyKey: idempotencyKeySchema,
    actionId: boundedIdentifierSchema,
    targetId: boundedIdentifierSchema,
    targetVersion: boundedIdentifierSchema,
    claimToken: z.uuid(),
    leaseExpiresAt: z.iso.datetime(),
  })
  .strict();

export const claimAgentJobResponseSchema = z
  .object({
    data: z
      .object({
        job: claimedAgentJobSchema.nullable(),
      })
      .strict(),
    meta: apiMetaSchema,
  })
  .strict();

export const reportAgentJobResultRequestSchema = z
  .object({
    claimToken: z.uuid(),
    result: z.enum(["succeeded", "failed"]),
    evidence: z
      .array(
        z
          .object({
            code: executionEvidenceCodeSchema,
            recordedAt: z.iso.datetime(),
          })
          .strict(),
      )
      .min(1)
      .max(20),
  })
  .strict();

export const reportAgentJobResultResponseSchema = z
  .object({
    data: z
      .object({
        request: supportRequestViewSchema,
        replayed: z.boolean(),
      })
      .strict(),
    meta: apiMetaSchema,
  })
  .strict();

export type ApiErrorCode = z.infer<typeof apiErrorCodeSchema>;
export type CatalogProduct = z.infer<typeof catalogProductSchema>;
export type CreateSoftwareInstallationRequest = z.infer<
  typeof createSoftwareInstallationRequestSchema
>;
export type ClaimAgentJobRequest = z.infer<typeof claimAgentJobRequestSchema>;
export type ClaimedAgentJob = z.infer<typeof claimedAgentJobSchema>;
export type ReportAgentJobResultRequest = z.infer<
  typeof reportAgentJobResultRequestSchema
>;
export type BotCaseView = z.infer<typeof botCaseViewSchema>;
export type SupportRequestView = z.infer<typeof supportRequestViewSchema>;
