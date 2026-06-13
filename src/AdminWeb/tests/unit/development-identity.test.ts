import { describe, expect, it } from "vitest";
import { getDevelopmentIdentity } from "@/modules/identity/application/development-identity";

describe("development identity", () => {
  it("returns the deterministic synthetic principal only when explicitly enabled", () => {
    const principal = getDevelopmentIdentity({
      IT_SUPPORT_ENVIRONMENT: "development",
      LOCAL_DEVELOPMENT_IDENTITY_ENABLED: "true",
    });

    expect(principal.subject).toBe("development-user-001");
    expect(principal.role).toBe("DeveloperAllAccess");
  });

  it("is disabled outside the exact development profile", () => {
    expect(() =>
      getDevelopmentIdentity({
        IT_SUPPORT_ENVIRONMENT: "production",
        LOCAL_DEVELOPMENT_IDENTITY_ENABLED: "true",
      }),
    ).toThrowError("Development identity is unavailable.");
  });
});
