import type { SupportRequestView } from "@it-support-native/control-plane-contracts";

export interface CreateConfirmedInstallationCommand {
  readonly idempotencyKey: string;
  readonly correlationId: string;
  readonly actorSubject: string;
  readonly deviceId: string;
  readonly productId: string;
  readonly productVersion: string;
  readonly actionId: string;
}

export interface CreatedSupportRequest {
  readonly request: SupportRequestView;
  readonly replayed: boolean;
}
