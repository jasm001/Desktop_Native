import { requestIdSchema } from "@it-support-native/control-plane-contracts";
import { randomUUID } from "node:crypto";
import { GetBotCase } from "@/modules/cases/application/get-bot-case";
import { PrismaBotCaseRepository } from "@/modules/cases/infrastructure/prisma-bot-case-repository";
import { getDevelopmentIdentity } from "@/modules/identity/application/development-identity";
import { getPrisma } from "@/platform/db/prisma";
import { errorResponse, successResponse } from "@/platform/http/api-response";
import { resolveCorrelationId } from "@/platform/http/request-metadata";

interface RouteContext {
  readonly params: Promise<{ requestId: string }>;
}

export async function GET(
  request: Request,
  context: RouteContext,
): Promise<Response> {
  let correlationId: string = randomUUID();

  try {
    correlationId = resolveCorrelationId(request);
    getDevelopmentIdentity();
    const { requestId } = await context.params;
    const parsedRequestId = requestIdSchema.parse(requestId);
    const useCase = new GetBotCase(
      new PrismaBotCaseRepository(getPrisma()),
    );

    return successResponse(
      { case: await useCase.execute(parsedRequestId) },
      correlationId,
    );
  } catch (error) {
    return errorResponse(error, correlationId);
  }
}
