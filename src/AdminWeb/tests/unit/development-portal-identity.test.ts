import { describe, expect, it } from "vitest";
import { getAdminOverview } from "@/modules/administration/application/get-admin-overview";
import { getDevelopmentPortalIdentity } from "@/modules/identity/application/development-portal-identity";
import type { PortalPrincipal } from "@/modules/identity/domain/portal-principal";
import {
  authorizePortalCapability,
  requireDevelopmentPortalAccess,
  resolveDevelopmentPortalAccess,
} from "@/modules/identity/application/portal-authorization";
import { portalCapabilities } from "@/modules/identity/domain/portal-principal";

const validEnvironment = {
  IT_SUPPORT_ENVIRONMENT: "development",
  LOCAL_DEVELOPMENT_PORTAL_IDENTITY_ENABLED: "true",
  LOCAL_DEVELOPMENT_PORTAL_ROLE: "DeveloperAllAccess",
} as const;

describe("development portal identity", () => {
  it("creates a portal principal that is separate from user and agent identities", () => {
    const principal = getDevelopmentPortalIdentity(validEnvironment);

    expect(principal).toEqual({
      subject: "local-portal-operator-001",
      displayName: "Operador de desarrollo",
      role: "DeveloperAllAccess",
    });
    expect(principal.subject).not.toBe("development-user-001");
    expect(principal.subject).not.toBe("local-agent-001");
  });

  it.each([
    [{ ...validEnvironment, IT_SUPPORT_ENVIRONMENT: "test" }],
    [{ ...validEnvironment, IT_SUPPORT_ENVIRONMENT: "production" }],
    [{ ...validEnvironment, LOCAL_DEVELOPMENT_PORTAL_IDENTITY_ENABLED: "false" }],
    [
      {
        ...validEnvironment,
        LOCAL_DEVELOPMENT_PORTAL_IDENTITY_ENABLED: undefined,
      },
    ],
  ])("fails closed outside the exact enabled development profile", (environment) => {
    expect(() => getDevelopmentPortalIdentity(environment)).toThrowError(
      expect.objectContaining({ code: "portal_identity_unavailable" }),
    );
  });

  it("rejects unknown roles", () => {
    expect(() =>
      getDevelopmentPortalIdentity({
        ...validEnvironment,
        LOCAL_DEVELOPMENT_PORTAL_ROLE: "PlatformSuperAdmin",
      }),
    ).toThrowError(expect.objectContaining({ code: "portal_role_unknown" }));
  });
});

describe("portal authorization", () => {
  it.each(portalCapabilities)(
    "allows the known read-only capability %s",
    (capability) => {
      expect(
        requireDevelopmentPortalAccess(capability, validEnvironment),
      ).toMatchObject({ role: "DeveloperAllAccess" });
    },
  );

  it("returns no principal when route access fails closed", () => {
    expect(
      resolveDevelopmentPortalAccess("portal.dashboard.read", {
        ...validEnvironment,
        IT_SUPPORT_ENVIRONMENT: "production",
      }),
    ).toBeNull();
  });

  it("rejects unknown capabilities and roles", () => {
    const principal = getDevelopmentPortalIdentity(validEnvironment);
    const unknownRolePrincipal = {
      ...principal,
      role: "UnknownRole",
    } as unknown as PortalPrincipal;

    expect(() =>
      authorizePortalCapability(principal, "portal.jobs.execute"),
    ).toThrowError(
      expect.objectContaining({ code: "portal_capability_unknown" }),
    );
    expect(() =>
      authorizePortalCapability(
        unknownRolePrincipal,
        "portal.dashboard.read",
      ),
    ).toThrowError(expect.objectContaining({ code: "portal_role_unknown" }));
  });

  it("builds the read-only overview without performing I/O", () => {
    const first = getAdminOverview();
    const second = getAdminOverview();

    expect(second).toBe(first);
    expect(first.capabilities).toEqual(
      expect.arrayContaining([
        expect.objectContaining({
          name: "Mutaciones administrativas",
          status: "Blocked",
        }),
      ]),
    );
  });
});
