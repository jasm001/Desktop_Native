import type { ClaimedAgentJob } from "@it-support-native/control-plane-contracts";
import type { AgentJobRepository } from "./agent-job-repository";

export class ClaimAgentJob {
  public constructor(private readonly repository: AgentJobRepository) {}

  public execute(
    deviceId: string,
    agentSubject: string,
    correlationId: string,
  ): Promise<ClaimedAgentJob | null> {
    return this.repository.claimNext(deviceId, agentSubject, correlationId);
  }
}
