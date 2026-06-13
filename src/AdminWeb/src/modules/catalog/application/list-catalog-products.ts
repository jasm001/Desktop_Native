import type { CatalogProduct } from "@it-support-native/control-plane-contracts";
import { ApplicationError } from "../../../platform/errors/application-error";
import { syntheticCatalog } from "../domain/catalog-product";

export interface CatalogPage {
  readonly items: readonly CatalogProduct[];
  readonly nextCursor: string | null;
}

export function listCatalogProducts(
  limit: number,
  cursor?: string,
): CatalogPage {
  const startIndex =
    cursor === undefined
      ? 0
      : syntheticCatalog.findIndex((product) => product.id === cursor) + 1;

  if (cursor !== undefined && startIndex === 0) {
    throw new ApplicationError(
      "invalid_request",
      400,
      "The catalog cursor is invalid.",
    );
  }

  const items = syntheticCatalog.slice(startIndex, startIndex + limit);
  const hasMore = startIndex + items.length < syntheticCatalog.length;

  return {
    items,
    nextCursor: hasMore ? (items.at(-1)?.id ?? null) : null,
  };
}

export function requireAllowedCatalogAction(
  productId: string,
  productVersion: string,
  actionId: string,
): void {
  const product = syntheticCatalog.find((candidate) => candidate.id === productId);

  if (
    product === undefined ||
    product.status !== "approved" ||
    product.version !== productVersion ||
    product.actionId !== actionId
  ) {
    throw new ApplicationError(
      "invalid_request",
      400,
      "The requested catalog action is not available.",
    );
  }
}
