import { describe, expect, it } from "vitest";
import { supportRequestConfirmedEventV1Schema } from "@it-support-native/control-plane-contracts";

describe("worker event contract", () => {
  it("rejects payloads that include executable content", () => {
    const result = supportRequestConfirmedEventV1Schema.safeParse({
      version: 1,
      requestId: "afae27b0-8bb6-4c4b-97d7-035fa9bddf48",
      jobId: "84bf6dda-1e73-4ea9-a17f-12523048785b",
      correlationId: "correlation-1",
      actorSubject: "development-user-001",
      deviceId: "local-device-001",
      actionId: "software.install.simulated.v1",
      productId: "secure-transfer",
      productVersion: "6.5",
      command: "powershell.exe",
    });

    expect(result.success).toBe(false);
  });
});
