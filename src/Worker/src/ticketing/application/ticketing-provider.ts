import type {
  BotCaseEscalationRequestedEventV1,
} from "@it-support-native/control-plane-contracts";

export interface ExternalTicketDraft {
  readonly id: string;
  readonly caseId: string;
  readonly provider: "fake";
  readonly externalReference: string;
  readonly category: "software_installation";
  readonly status: "open";
  readonly correlationId: string;
  readonly reasonCode:
    | "execution_failed"
    | "claim_lease_exhausted";
  readonly description: string;
}

export interface ITicketingProvider {
  createTicket(
    escalation: BotCaseEscalationRequestedEventV1,
  ): ExternalTicketDraft;
}
