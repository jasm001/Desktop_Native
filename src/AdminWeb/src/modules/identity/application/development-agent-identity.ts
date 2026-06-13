import { ApplicationError } from "../../../platform/errors/application-error";

export interface DevelopmentAgentPrincipal {
  readonly subject: "local-agent-001";
}

export function getDevelopmentAgentIdentity(
  request: Request,
  environment: Readonly<Record<string, string | undefined>> = process.env,
): DevelopmentAgentPrincipal {
  const enabled =
    environment.IT_SUPPORT_ENVIRONMENT === "development" &&
    environment.LOCAL_DEVELOPMENT_AGENT_ENABLED === "true";
  const subject = request.headers.get("x-development-agent-id");

  if (!enabled || subject !== "local-agent-001") {
    throw new ApplicationError(
      "agent_identity_unavailable",
      401,
      "Development agent identity is unavailable.",
    );
  }

  return { subject };
}
