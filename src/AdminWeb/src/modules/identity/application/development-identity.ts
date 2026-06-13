import { ApplicationError } from "../../../platform/errors/application-error";
import type { DevelopmentPrincipal } from "../domain/development-principal";

const developmentPrincipal: DevelopmentPrincipal = {
  subject: "development-user-001",
  displayName: "Synthetic Development User",
  role: "DeveloperAllAccess",
};

export function getDevelopmentIdentity(
  environment: Readonly<Record<string, string | undefined>> = process.env,
): DevelopmentPrincipal {
  const isDevelopment = environment.IT_SUPPORT_ENVIRONMENT === "development";
  const isEnabled =
    environment.LOCAL_DEVELOPMENT_IDENTITY_ENABLED === "true";

  if (!isDevelopment || !isEnabled) {
    throw new ApplicationError(
      "identity_unavailable",
      401,
      "Development identity is unavailable.",
    );
  }

  return developmentPrincipal;
}
