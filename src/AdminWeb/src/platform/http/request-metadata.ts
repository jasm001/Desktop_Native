import {
  correlationIdSchema,
  idempotencyKeySchema,
} from "@it-support-native/control-plane-contracts";
import { randomUUID } from "node:crypto";
import { ApplicationError } from "../errors/application-error";

export function resolveCorrelationId(request: Request): string {
  const provided = request.headers.get("x-correlation-id");
  if (provided === null) {
    return randomUUID();
  }

  const parsed = correlationIdSchema.safeParse(provided);
  if (!parsed.success) {
    throw new ApplicationError(
      "invalid_request",
      400,
      "The correlation identifier is invalid.",
    );
  }

  return parsed.data;
}

export function requireIdempotencyKey(request: Request): string {
  const parsed = idempotencyKeySchema.safeParse(
    request.headers.get("idempotency-key"),
  );
  if (!parsed.success) {
    throw new ApplicationError(
      "invalid_request",
      400,
      "A valid Idempotency-Key header is required.",
    );
  }

  return parsed.data;
}
