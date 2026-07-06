import type { AdminLabComponentStatus } from "./admin-read-repository";

export interface AdminLabHealthProvider {
  listComponentStatuses(): Promise<readonly AdminLabComponentStatus[]>;
}
