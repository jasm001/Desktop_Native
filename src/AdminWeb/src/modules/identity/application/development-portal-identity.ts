import type {
  PortalPrincipal,
  PortalRole,
} from "../domain/portal-principal";
import { portalRoles } from "../domain/portal-principal";
import { PortalAccessError } from "./portal-access-error";

const roleSet = new Set<string>(portalRoles);

export function getDevelopmentPortalIdentity(
  environment: Readonly<Record<string, string | undefined>> = process.env,
): PortalPrincipal {
  const enabled =
    environment.IT_SUPPORT_ENVIRONMENT === "development" &&
    environment.LOCAL_DEVELOPMENT_PORTAL_IDENTITY_ENABLED === "true";

  if (!enabled) {
    throw new PortalAccessError(
      "portal_identity_unavailable",
      "The local portal identity is unavailable.",
    );
  }

  const configuredRole = environment.LOCAL_DEVELOPMENT_PORTAL_ROLE;
  if (configuredRole === undefined || !roleSet.has(configuredRole)) {
    throw new PortalAccessError(
      "portal_role_unknown",
      "The local portal role is unavailable.",
    );
  }

  return {
    subject: "local-portal-operator-001",
    displayName: "Operador de desarrollo",
    role: configuredRole as PortalRole,
  };
}
