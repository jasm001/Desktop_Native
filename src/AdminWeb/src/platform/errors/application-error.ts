import type { ApiErrorCode } from "@it-support-native/control-plane-contracts";

export class ApplicationError extends Error {
  public constructor(
    public readonly code: ApiErrorCode,
    public readonly httpStatus: number,
    message: string,
  ) {
    super(message);
    this.name = "ApplicationError";
  }
}
