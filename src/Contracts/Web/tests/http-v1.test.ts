import { randomUUID } from "node:crypto";
import { describe, expect, it } from "vitest";
import {
  botCaseEscalationRequestedEventV1Schema,
  botCaseViewSchema,
  claimAgentJobRequestSchema,
  createSoftwareInstallationRequestSchema,
  reportAgentJobResultRequestSchema,
  supportRequestConfirmedEventV1Schema,
} from "../src";

describe("HTTP v1 contracts", () => {
  it("accepts only an explicitly confirmed typed installation request", () => {
    const result = createSoftwareInstallationRequestSchema.safeParse({
      confirmed: true,
      deviceId: "local-device-001",
      productId: "secure-transfer",
      productVersion: "6.5",
      actionId: "software.install.simulated.v1",
    });

    expect(result.success).toBe(true);
  });

  it("rejects execution-shaped fields and missing confirmation", () => {
    const result = createSoftwareInstallationRequestSchema.safeParse({
      confirmed: false,
      deviceId: "local-device-001",
      productId: "secure-transfer",
      productVersion: "6.5",
      actionId: "software.install.simulated.v1",
      command: "powershell.exe",
    });

    expect(result.success).toBe(false);
  });

  it("requires the fixed synthetic outbox payload", () => {
    const result = supportRequestConfirmedEventV1Schema.safeParse({
      version: 1,
      requestId: randomUUID(),
      jobId: randomUUID(),
      correlationId: "correlation-1",
      actorSubject: "development-user-001",
      deviceId: "local-device-001",
      actionId: "software.install.simulated.v1",
      productId: "secure-transfer",
      productVersion: "6.5",
    });

    expect(result.success).toBe(true);
  });

  it("rejects executable fields in agent claim and result contracts", () => {
    expect(
      claimAgentJobRequestSchema.safeParse({
        deviceId: "local-device-001",
        command: "powershell.exe",
      }).success,
    ).toBe(false);
    expect(
      reportAgentJobResultRequestSchema.safeParse({
        claimToken: randomUUID(),
        result: "succeeded",
        evidence: [
          {
            code: "job.simulation.verified",
            recordedAt: "2026-06-13T18:30:00.000Z",
            summary: "untrusted free text",
          },
        ],
      }).success,
    ).toBe(false);
  });

  it("accepts only bounded case views without operational free text", () => {
    const result = botCaseViewSchema.safeParse({
      id: randomUUID(),
      requestId: randomUUID(),
      correlationId: "correlation-1",
      category: "software_installation",
      status: "attended_waiting_user",
      result: "succeeded",
      waitingForUserSince: "2026-06-13T18:30:00.000Z",
      escalatedAt: null,
      createdAt: "2026-06-13T18:00:00.000Z",
      updatedAt: "2026-06-13T18:30:00.000Z",
      externalTicket: null,
    });

    expect(result.success).toBe(true);
    expect(
      botCaseViewSchema.safeParse({
        ...result.data,
        description: "unbounded operational detail",
      }).success,
    ).toBe(false);
  });

  it("accepts only a sanitized typed escalation event", () => {
    const event = {
      version: 1,
      caseId: randomUUID(),
      requestId: randomUUID(),
      jobId: randomUUID(),
      correlationId: "correlation-1",
      category: "software_installation",
      reasonCode: "execution_failed",
      productId: "secure-transfer",
      productVersion: "6.5",
    };

    expect(
      botCaseEscalationRequestedEventV1Schema.safeParse(event).success,
    ).toBe(true);
    expect(
      botCaseEscalationRequestedEventV1Schema.safeParse({
        ...event,
        logs: "untrusted diagnostic output",
      }).success,
    ).toBe(false);
  });

  it("accepts a bounded synthetic external ticket view", () => {
    const result = botCaseViewSchema.safeParse({
      id: randomUUID(),
      requestId: randomUUID(),
      correlationId: "correlation-1",
      category: "software_installation",
      status: "escalated",
      result: "failed",
      waitingForUserSince: null,
      escalatedAt: "2026-06-14T08:00:00.000Z",
      createdAt: "2026-06-14T07:55:00.000Z",
      updatedAt: "2026-06-14T08:00:00.000Z",
      externalTicket: {
        id: randomUUID(),
        provider: "fake",
        reference: "FAKE-1234567890ABCDEF",
        category: "software_installation",
        status: "open",
        correlationId: "correlation-1",
        reasonCode: "execution_failed",
        description:
          "Synthetic escalation for secure-transfer 6.5; human support review required.",
        createdAt: "2026-06-14T08:00:01.000Z",
      },
    });

    expect(result.success).toBe(true);
  });
});
