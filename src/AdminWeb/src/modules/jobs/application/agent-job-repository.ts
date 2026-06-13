import type {
  ClaimedAgentJob,
  ReportAgentJobResultRequest,
  SupportRequestView,
} from "@it-support-native/control-plane-contracts";

export interface ReportedAgentJobResult {
  readonly request: SupportRequestView;
  readonly replayed: boolean;
}

export interface AgentJobRepository {
  claimNext(
    deviceId: string,
    agentSubject: string,
    correlationId: string,
  ): Promise<ClaimedAgentJob | null>;

  reportResult(
    jobId: string,
    idempotencyKey: string,
    correlationId: string,
    agentSubject: string,
    result: ReportAgentJobResultRequest,
  ): Promise<ReportedAgentJobResult>;
}
