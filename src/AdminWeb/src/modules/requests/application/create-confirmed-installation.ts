import type { CreateSoftwareInstallationRequest } from "@it-support-native/control-plane-contracts";
import { requireAllowedCatalogAction } from "../../catalog/application/list-catalog-products";
import type { DevelopmentPrincipal } from "../../identity/domain/development-principal";
import type { CreatedSupportRequest } from "../domain/support-request";
import { createRequestPayloadHash } from "./request-payload-hash";
import type { SupportRequestRepository } from "./support-request-repository";

export class CreateConfirmedInstallation {
  public constructor(private readonly repository: SupportRequestRepository) {}

  public async execute(
    input: CreateSoftwareInstallationRequest,
    idempotencyKey: string,
    correlationId: string,
    principal: DevelopmentPrincipal,
  ): Promise<CreatedSupportRequest> {
    requireAllowedCatalogAction(
      input.productId,
      input.productVersion,
      input.actionId,
    );

    const command = {
      idempotencyKey,
      correlationId,
      actorSubject: principal.subject,
      deviceId: input.deviceId,
      productId: input.productId,
      productVersion: input.productVersion,
      actionId: input.actionId,
    };

    return this.repository.createConfirmedInstallation(
      command,
      createRequestPayloadHash(command),
    );
  }
}
