import type { SupportRequestView } from "@it-support-native/control-plane-contracts";
import { ApplicationError } from "../../../platform/errors/application-error";
import type { SupportRequestRepository } from "./support-request-repository";

export class GetSupportRequest {
  public constructor(private readonly repository: SupportRequestRepository) {}

  public async execute(requestId: string): Promise<SupportRequestView> {
    const request = await this.repository.findById(requestId);

    if (request === null) {
      throw new ApplicationError(
        "request_not_found",
        404,
        "The support request was not found.",
      );
    }

    return request;
  }
}
