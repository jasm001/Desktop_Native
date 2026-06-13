import { describe, expect, it } from "vitest";
import { createRequestPayloadHash } from "@/modules/requests/application/request-payload-hash";

const command = {
  idempotencyKey: "key-1",
  correlationId: "correlation-1",
  actorSubject: "development-user-001",
  deviceId: "local-device-001",
  productId: "secure-transfer",
  productVersion: "6.5",
  actionId: "software.install.simulated.v1",
};

describe("request payload hash", () => {
  it("is stable across transport metadata changes", () => {
    const first = createRequestPayloadHash(command);
    const second = createRequestPayloadHash({
      ...command,
      correlationId: "correlation-retry",
      idempotencyKey: "key-retry",
    });

    expect(first).toBe(second);
    expect(first).toHaveLength(64);
  });

  it("changes when the business payload changes", () => {
    expect(
      createRequestPayloadHash({
        ...command,
        productVersion: "6.6",
      }),
    ).not.toBe(createRequestPayloadHash(command));
  });
});
