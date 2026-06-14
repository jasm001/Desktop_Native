import { createHash } from "node:crypto";
import type {
  BotCaseEscalationRequestedEventV1,
} from "@it-support-native/control-plane-contracts";
import type {
  ExternalTicketDraft,
  ITicketingProvider,
} from "../application/ticketing-provider.js";

export class FakeTicketingProvider implements ITicketingProvider {
  public createTicket(
    escalation: BotCaseEscalationRequestedEventV1,
  ): ExternalTicketDraft {
    const digest = hash(`external-ticket:${escalation.caseId}`);
    const target = (
      `${escalation.productId} ${escalation.productVersion}`
    ).slice(0, 120);

    return {
      id: deterministicUuid(digest),
      caseId: escalation.caseId,
      provider: "fake",
      externalReference: `FAKE-${digest.slice(0, 20).toUpperCase()}`,
      category: escalation.category,
      status: "open",
      correlationId: escalation.correlationId,
      reasonCode: escalation.reasonCode,
      description:
        `Synthetic escalation for ${target}; ` +
        "human support review required.",
    };
  }
}

function hash(value: string): string {
  return createHash("sha256").update(value, "utf8").digest("hex");
}

function deterministicUuid(digest: string): string {
  const characters = digest.slice(0, 32).split("");
  characters[12] = "5";
  characters[16] = (
    (Number.parseInt(characters[16] ?? "0", 16) & 0x3) |
    0x8
  ).toString(16);
  const value = characters.join("");

  return [
    value.slice(0, 8),
    value.slice(8, 12),
    value.slice(12, 16),
    value.slice(16, 20),
    value.slice(20, 32),
  ].join("-");
}
