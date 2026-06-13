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
  "idempotency_conflict",
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
  "completed",
  "failed",
]);

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

export type ApiErrorCode = z.infer<typeof apiErrorCodeSchema>;
export type CatalogProduct = z.infer<typeof catalogProductSchema>;
export type CreateSoftwareInstallationRequest = z.infer<
  typeof createSoftwareInstallationRequestSchema
>;
export type SupportRequestView = z.infer<typeof supportRequestViewSchema>;
