import { createSoftwareInstallationRequestSchema } from "@it-support-native/control-plane-contracts";
import { randomUUID } from "node:crypto";
import { CreateConfirmedInstallation } from "@/modules/requests/application/create-confirmed-installation";
import { PrismaSupportRequestRepository } from "@/modules/requests/infrastructure/prisma-support-request-repository";
import { getDevelopmentIdentity } from "@/modules/identity/application/development-identity";
import { ApplicationError } from "@/platform/errors/application-error";
import { getPrisma } from "@/platform/db/prisma";
import { errorResponse, successResponse } from "@/platform/http/api-response";
import {
  requireIdempotencyKey,
  resolveCorrelationId,
} from "@/platform/http/request-metadata";

export async function POST(request: Request): Promise<Response> {
  let correlationId: string = randomUUID();

  try {
    correlationId = resolveCorrelationId(request);
    const idempotencyKey = requireIdempotencyKey(request);
    const payload = createSoftwareInstallationRequestSchema.safeParse(
      await readJson(request),
    );

    if (!payload.success) {
      throw new ApplicationError(
        "invalid_request",
        400,
        "The request payload is invalid.",
      );
    }

    const principal = getDevelopmentIdentity();
    const useCase = new CreateConfirmedInstallation(
      new PrismaSupportRequestRepository(getPrisma()),
    );
    const result = await useCase.execute(
      payload.data,
      idempotencyKey,
      correlationId,
      principal,
    );

    return successResponse(result, correlationId, result.replayed ? 200 : 201);
  } catch (error) {
    return errorResponse(error, correlationId);
  }
}

async function readJson(request: Request): Promise<unknown> {
  try {
    return await request.json();
  } catch {
    throw new ApplicationError(
      "invalid_request",
      400,
      "The request body must contain valid JSON.",
    );
  }
}
