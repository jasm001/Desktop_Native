import type { ReportAgentJobResultRequest } from "@it-support-native/control-plane-contracts";
import type {
  AgentJobRepository,
  ReportedAgentJobResult,
} from "./agent-job-repository";

export class ReportAgentJobResult {
  public constructor(private readonly repository: AgentJobRepository) {}

  public execute(
    jobId: string,
    idempotencyKey: string,
    correlationId: string,
    agentSubject: string,
    result: ReportAgentJobResultRequest,
  ): Promise<ReportedAgentJobResult> {
    return this.repository.reportResult(
      jobId,
      idempotencyKey,
      correlationId,
      agentSubject,
      result,
    );
  }
}
