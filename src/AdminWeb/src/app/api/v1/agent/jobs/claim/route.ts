import { claimAgentJobRequestSchema } from "@it-support-native/control-plane-contracts";
import { ClaimAgentJob } from "@/modules/jobs/application/claim-agent-job";
import { PrismaAgentJobRepository } from "@/modules/jobs/infrastructure/prisma-agent-job-repository";
import { getDevelopmentAgentIdentity } from "@/modules/identity/application/development-agent-identity";
import { getPrisma } from "@/platform/db/prisma";
import { ApplicationError } from "@/platform/errors/application-error";
import { errorResponse, successResponse } from "@/platform/http/api-response";
import { resolveCorrelationId } from "@/platform/http/request-metadata";
import { randomUUID } from "node:crypto";

export async function POST(request: Request): Promise<Response> {
  let correlationId: string = randomUUID();

  try {
    correlationId = resolveCorrelationId(request);
    const principal = getDevelopmentAgentIdentity(request);
    const payload = claimAgentJobRequestSchema.safeParse(
      await readJson(request),
    );
    if (!payload.success) {
      throw new ApplicationError(
        "invalid_request",
        400,
        "The request payload is invalid.",
      );
    }

    const useCase = new ClaimAgentJob(
      new PrismaAgentJobRepository(getPrisma()),
    );
    const job = await useCase.execute(
      payload.data.deviceId,
      principal.subject,
      correlationId,
    );

    return successResponse({ job }, correlationId);
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
