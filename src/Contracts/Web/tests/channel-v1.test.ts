import { readFile } from "node:fs/promises";
import { describe, expect, it } from "vitest";
import {
  conversationChannelInputSchema,
  conversationChannelOutputSchema,
} from "../src";

interface FixtureCase {
  readonly name: string;
  readonly input: unknown;
}

interface ChannelFixture {
  readonly validInputs: readonly FixtureCase[];
  readonly invalidInputs: readonly FixtureCase[];
}

const fixtureUrl = new URL(
  "../../../../tests/Fixtures/conversation-channel-v1.json",
  import.meta.url,
);

describe("conversation channel v1 contract", () => {
  it("accepts all shared valid fixtures", async () => {
    const fixture = await loadFixture();

    for (const testCase of fixture.validInputs) {
      expect(
        conversationChannelInputSchema.safeParse(testCase.input).success,
        testCase.name,
      ).toBe(true);
    }
  });

  it("rejects unsupported, unknown, and executable fixtures", async () => {
    const fixture = await loadFixture();

    for (const testCase of fixture.invalidInputs) {
      expect(
        conversationChannelInputSchema.safeParse(testCase.input).success,
        testCase.name,
      ).toBe(false);
    }
  });

  it("accepts only bounded normalized output", () => {
    const output = {
      version: 1,
      messageId: "message-query-1",
      correlationId: "correlation-query-1",
      sessionId: "parity-session-1",
      state: "query",
      resultCode: "query_answered",
      decision: {
        kind: "inform",
        effectiveStatus: "approved",
        productId: "secure-transfer",
        productName: "Secure Transfer",
        productVersion: "6.5",
        alternatives: [],
      },
      request: null,
      status: null,
      error: null,
    };

    expect(conversationChannelOutputSchema.safeParse(output).success).toBe(true);
    expect(
      conversationChannelOutputSchema.safeParse({
        ...output,
        command: "powershell.exe",
      }).success,
    ).toBe(false);
  });
});

async function loadFixture(): Promise<ChannelFixture> {
  return JSON.parse(await readFile(fixtureUrl, "utf8")) as ChannelFixture;
}
