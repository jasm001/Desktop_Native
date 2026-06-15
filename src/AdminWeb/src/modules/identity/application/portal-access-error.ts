export type PortalAccessErrorCode =
  | "portal_identity_unavailable"
  | "portal_role_unknown"
  | "portal_capability_unknown"
  | "portal_access_denied";

export class PortalAccessError extends Error {
  public constructor(
    public readonly code: PortalAccessErrorCode,
    message: string,
  ) {
    super(message);
    this.name = "PortalAccessError";
  }
}
