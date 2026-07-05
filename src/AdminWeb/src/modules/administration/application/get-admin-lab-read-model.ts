import type {
  AdminLabReadModel,
  AdminReadRepository,
} from "./admin-read-repository";
import { adminReadPageLimit } from "./get-admin-read-model";

export function getAdminLabReadModel(
  repository: AdminReadRepository,
): Promise<AdminLabReadModel> {
  return repository.getLabReadModel(adminReadPageLimit);
}
