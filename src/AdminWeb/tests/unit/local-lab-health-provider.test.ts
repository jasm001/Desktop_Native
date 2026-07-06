import { describe, expect, it, vi } from "vitest";
import { LocalLabHealthProvider } from "@/modules/administration/infrastructure/local-lab-health-provider";

const developmentEnvironment = {
  IT_SUPPORT_ENVIRONMENT: "development",
} as const;

describe("local lab health provider", () => {
  it("reports available local health without exposing configured endpoints", async () => {
    const fetcher = vi.fn().mockResolvedValue({ ok: true, status: 200 });
    const fileSystem = {
      stat: vi.fn().mockResolvedValue({ isDirectory: () => true }),
    };
    const provider = new LocalLabHealthProvider(
      {
        ...developmentEnvironment,
        IT_SUPPORT_HERMES_CHAT_ENABLED: "true",
        IT_SUPPORT_HERMES_BASE_URL: "http://127.0.0.1:8765/v1",
        IT_SUPPORT_HERMES_API_KEY: "local-test-key",
        IT_SUPPORT_LAB_MIRROR_PATH: "C:\\lab-mirror",
        IT_SUPPORT_LAB_BRIDGE_MODE: "validate-only",
        IT_SUPPORT_LAB_BRIDGE_BASE_URL: "http://127.0.0.1:9077",
      },
      fetcher,
      fileSystem,
      "C:\\repo",
      fixedNow,
    );

    const statuses = await provider.listComponentStatuses();

    expect(statuses).toEqual(
      expect.arrayContaining([
        expect.objectContaining({
          id: "hermes",
          status: "available",
          source: "local",
          scope: "local-demo",
          mode: "health-check",
        }),
        expect.objectContaining({
          id: "artifact-mirror",
          status: "available",
          mode: "health-check",
        }),
        expect.objectContaining({
          id: "lab-bridge",
          status: "validate-only",
          source: "validate-only",
          scope: "validate-only",
          mode: "validate-only",
        }),
      ]),
    );
    expect(JSON.stringify(statuses)).not.toContain("127.0.0.1");
    expect(JSON.stringify(statuses)).not.toContain("local-test-key");
    expect(fetcher).toHaveBeenCalledTimes(2);
  });

  it("reports offline dependencies without throwing", async () => {
    const fetcher = vi.fn().mockRejectedValue(new Error("ECONNREFUSED"));
    const fileSystem = {
      stat: vi.fn().mockRejectedValue(Object.assign(new Error("missing"), { code: "ENOENT" })),
    };
    const provider = new LocalLabHealthProvider(
      {
        ...developmentEnvironment,
        IT_SUPPORT_HERMES_CHAT_ENABLED: "true",
        IT_SUPPORT_HERMES_BASE_URL: "http://127.0.0.1:8765/v1",
        IT_SUPPORT_HERMES_API_KEY: "local-test-key",
        IT_SUPPORT_LAB_MIRROR_PATH: "C:\\lab-mirror",
        IT_SUPPORT_LAB_BRIDGE_MODE: "validate-only",
        IT_SUPPORT_LAB_BRIDGE_BASE_URL: "http://127.0.0.1:9077",
      },
      fetcher,
      fileSystem,
      "C:\\repo",
      fixedNow,
    );

    await expect(provider.listComponentStatuses()).resolves.toEqual(
      expect.arrayContaining([
        expect.objectContaining({ id: "hermes", status: "offline" }),
        expect.objectContaining({ id: "artifact-mirror", status: "offline" }),
        expect.objectContaining({ id: "lab-bridge", status: "offline" }),
      ]),
    );
  });

  it("reports misconfiguration for non-loopback or non-validate-only settings", async () => {
    const fetcher = vi.fn();
    const fileSystem = {
      stat: vi.fn().mockResolvedValue({ isDirectory: () => true }),
    };
    const provider = new LocalLabHealthProvider(
      {
        ...developmentEnvironment,
        IT_SUPPORT_HERMES_CHAT_ENABLED: "true",
        IT_SUPPORT_HERMES_BASE_URL: "https://example.test/v1",
        IT_SUPPORT_HERMES_API_KEY: "local-test-key",
        IT_SUPPORT_LAB_MIRROR_PATH: "C:\\repo\\mirror",
        IT_SUPPORT_LAB_BRIDGE_MODE: "deploy",
      },
      fetcher,
      fileSystem,
      "C:\\repo",
      fixedNow,
    );

    await expect(provider.listComponentStatuses()).resolves.toEqual(
      expect.arrayContaining([
        expect.objectContaining({ id: "hermes", status: "misconfigured" }),
        expect.objectContaining({
          id: "artifact-mirror",
          status: "misconfigured",
        }),
        expect.objectContaining({ id: "lab-bridge", status: "misconfigured" }),
      ]),
    );
    expect(fetcher).not.toHaveBeenCalled();
  });

  it("fails closed outside Development without probing dependencies", async () => {
    const fetcher = vi.fn();
    const fileSystem = {
      stat: vi.fn(),
    };
    const provider = new LocalLabHealthProvider(
      {
        IT_SUPPORT_ENVIRONMENT: "production",
        IT_SUPPORT_HERMES_CHAT_ENABLED: "true",
        IT_SUPPORT_HERMES_BASE_URL: "http://127.0.0.1:8765/v1",
        IT_SUPPORT_HERMES_API_KEY: "local-test-key",
        IT_SUPPORT_LAB_MIRROR_PATH: "C:\\lab-mirror",
        IT_SUPPORT_LAB_BRIDGE_MODE: "validate-only",
        IT_SUPPORT_LAB_BRIDGE_BASE_URL: "http://127.0.0.1:9077",
      },
      fetcher,
      fileSystem,
      "C:\\repo",
      fixedNow,
    );

    await expect(provider.listComponentStatuses()).resolves.toEqual(
      expect.arrayContaining([
        expect.objectContaining({ id: "hermes", status: "unavailable" }),
        expect.objectContaining({
          id: "artifact-mirror",
          status: "unavailable",
        }),
        expect.objectContaining({ id: "lab-bridge", status: "unavailable" }),
      ]),
    );
    expect(fetcher).not.toHaveBeenCalled();
    expect(fileSystem.stat).not.toHaveBeenCalled();
  });
});

function fixedNow() {
  return new Date("2026-07-05T12:00:00.000Z");
}
