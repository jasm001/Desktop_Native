import {
  reportAgentJobResultRequestSchema,
  requestIdSchema,
} from "@it-support-native/control-plane-contracts";
import { getDevelopmentAgentIdentity } from "@/modules/identity/application/development-agent-identity";
import { ReportAgentJobResult } from "@/modules/jobs/application/report-agent-job-result";
import { PrismaAgentJobRepository } from "@/modules/jobs/infrastructure/prisma-agent-job-repository";
import { getPrisma } from "@/platform/db/prisma";
import { ApplicationError } from "@/platform/errors/application-error";
import { errorResponse, successResponse } from "@/platform/http/api-response";
import {
  requireIdempotencyKey,
  resolveCorrelationId,
} from "@/platform/http/request-metadata";
import { randomUUID } from "node:crypto";

interface RouteContext {
  readonly params: Promise<{ readonly jobId: string }>;
}

export async function POST(
  request: Request,
  context: RouteContext,
): Promise<Response> {
  let correlationId: string = randomUUID();

  try {
    correlationId = resolveCorrelationId(request);
    const principal = getDevelopmentAgentIdentity(request);
    const idempotencyKey = requireIdempotencyKey(request);
    const { jobId } = await context.params;
    const parsedJobId = requestIdSchema.safeParse(jobId);
    const payload = reportAgentJobResultRequestSchema.safeParse(
      await readJson(request),
    );
    if (!parsedJobId.success || !payload.success) {
      throw new ApplicationError(
        "invalid_request",
        400,
        "The request payload is invalid.",
      );
    }

    const useCase = new ReportAgentJobResult(
      new PrismaAgentJobRepository(getPrisma()),
    );
    const result = await useCase.execute(
      parsedJobId.data,
      idempotencyKey,
      correlationId,
      principal.subject,
      payload.data,
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
