import type {
  PortalCapability,
  PortalPrincipal,
  PortalRole,
} from "../domain/portal-principal";
import {
  portalCapabilities,
  portalRoles,
} from "../domain/portal-principal";
import { getDevelopmentPortalIdentity } from "./development-portal-identity";
import { PortalAccessError } from "./portal-access-error";

const knownRoles = new Set<string>(portalRoles);
const knownCapabilities = new Set<string>(portalCapabilities);
const roleCapabilities: Readonly<Record<PortalRole, ReadonlySet<PortalCapability>>> =
  {
    DeveloperAllAccess: new Set<PortalCapability>(["portal.dashboard.read"]),
  };

export function authorizePortalCapability(
  principal: PortalPrincipal,
  capability: string,
): void {
  if (!knownRoles.has(principal.role)) {
    throw new PortalAccessError(
      "portal_role_unknown",
      "The portal role is not recognized.",
    );
  }

  if (!knownCapabilities.has(capability)) {
    throw new PortalAccessError(
      "portal_capability_unknown",
      "The portal capability is not recognized.",
    );
  }

  const allowed = roleCapabilities[principal.role].has(
    capability as PortalCapability,
  );
  if (!allowed) {
    throw new PortalAccessError(
      "portal_access_denied",
      "The portal capability is not authorized.",
    );
  }
}

export function requireDevelopmentPortalAccess(
  capability: PortalCapability,
  environment: Readonly<Record<string, string | undefined>> = process.env,
): PortalPrincipal {
  const principal = getDevelopmentPortalIdentity(environment);
  authorizePortalCapability(principal, capability);
  return principal;
}
