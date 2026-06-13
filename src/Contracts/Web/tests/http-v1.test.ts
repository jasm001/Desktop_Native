import { randomUUID } from "node:crypto";
import { describe, expect, it } from "vitest";
import {
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
});
