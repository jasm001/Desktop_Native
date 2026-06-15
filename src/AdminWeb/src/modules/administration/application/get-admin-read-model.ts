import type {
  AdminAuditSummary,
  AdminOperationSummary,
  AdminReadRepository,
} from "./admin-read-repository";

export const adminReadPageLimit = 25;

export function getRecentAdminOperations(
  repository: AdminReadRepository,
): Promise<readonly AdminOperationSummary[]> {
  return repository.listRecentOperations(adminReadPageLimit);
}

export function getRecentAdminAuditEvents(
  repository: AdminReadRepository,
): Promise<readonly AdminAuditSummary[]> {
  return repository.listRecentAuditEvents(adminReadPageLimit);
}
