export const portalRoles = ["DeveloperAllAccess"] as const;
export type PortalRole = (typeof portalRoles)[number];

export const portalCapabilities = [
  "portal.dashboard.read",
  "portal.catalog.read",
  "portal.operations.read",
  "portal.audit.read",
  "portal.identity.read",
  "portal.approvals.read",
  "portal.support.read",
  "portal.reporting.read",
] as const;
export type PortalCapability = (typeof portalCapabilities)[number];

export interface PortalPrincipal {
  readonly subject: "local-portal-operator-001";
  readonly displayName: "Operador de desarrollo";
  readonly role: PortalRole;
}
