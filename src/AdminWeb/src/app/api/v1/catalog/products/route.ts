import {
  catalogQuerySchema,
  correlationIdSchema,
} from "@it-support-native/control-plane-contracts";
import { randomUUID } from "node:crypto";
import { listCatalogProducts } from "@/modules/catalog/application/list-catalog-products";
import { getDevelopmentIdentity } from "@/modules/identity/application/development-identity";
import { errorResponse, successResponse } from "@/platform/http/api-response";

export function GET(request: Request): Response {
  let correlationId: string = randomUUID();

  try {
    const providedCorrelationId = request.headers.get("x-correlation-id");
    if (providedCorrelationId !== null) {
      correlationId = correlationIdSchema.parse(providedCorrelationId);
    }

    getDevelopmentIdentity();
    const url = new URL(request.url);
    const query = catalogQuerySchema.parse({
      limit: url.searchParams.get("limit") ?? undefined,
      cursor: url.searchParams.get("cursor") ?? undefined,
    });

    return successResponse(
      listCatalogProducts(query.limit, query.cursor),
      correlationId,
    );
  } catch (error) {
    return errorResponse(error, correlationId);
  }
}
