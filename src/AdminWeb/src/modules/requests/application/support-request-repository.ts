import type {
  CreateConfirmedInstallationCommand,
  CreatedSupportRequest,
} from "../domain/support-request";

export interface SupportRequestRepository {
  createConfirmedInstallation(
    command: CreateConfirmedInstallationCommand,
    payloadHash: string,
  ): Promise<CreatedSupportRequest>;

  findById(requestId: string): Promise<CreatedSupportRequest["request"] | null>;
}
