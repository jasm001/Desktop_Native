import { z } from "zod";
import {
  boundedIdentifierSchema,
  correlationIdSchema,
  requestIdSchema,
} from "./http-v1";

export const supportRequestConfirmedEventType =
  "support-request.confirmed.v1" as const;

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
