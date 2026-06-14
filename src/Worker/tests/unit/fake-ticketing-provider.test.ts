import { describe, expect, it } from "vitest";
import type {
  BotCaseEscalationRequestedEventV1,
} from "@it-support-native/control-plane-contracts";
import { FakeTicketingProvider } from "../../src/ticketing/infrastructure/fake-ticketing-provider";

describe("FakeTicketingProvider", () => {
  it("creates the same sanitized ticket for the same case", () => {
    const provider = new FakeTicketingProvider();
    const escalation: BotCaseEscalationRequestedEventV1 = {
      version: 1,
      caseId: crypto.randomUUID(),
      requestId: crypto.randomUUID(),
      jobId: crypto.randomUUID(),
      correlationId: "fake-ticket-correlation",
      category: "software_installation",
      reasonCode: "execution_failed",
      productId: "secure-transfer",
      productVersion: "6.5",
    };

    const first = provider.createTicket(escalation);
    const second = provider.createTicket(escalation);

    expect(second).toEqual(first);
    expect(first.externalReference).toMatch(/^FAKE-[A-F0-9]{20}$/u);
    expect(first.description).toBe(
      "Synthetic escalation for secure-transfer 6.5; human support review required.",
    );
    expect(JSON.stringify(first)).not.toContain("token");
    expect(JSON.stringify(first)).not.toContain("password");
  });

  it("bounds the generated description for maximum contract identifiers", () => {
    const provider = new FakeTicketingProvider();
    const ticket = provider.createTicket({
      version: 1,
      caseId: crypto.randomUUID(),
      requestId: crypto.randomUUID(),
      jobId: crypto.randomUUID(),
      correlationId: "bounded-description-correlation",
      category: "software_installation",
      reasonCode: "claim_lease_exhausted",
      productId: `p${"a".repeat(127)}`,
      productVersion: `v${"1".repeat(127)}`,
    });

    expect(ticket.description.length).toBeLessThanOrEqual(200);
  });
});
