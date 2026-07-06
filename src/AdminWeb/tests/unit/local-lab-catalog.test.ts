import { createHash } from "node:crypto";
import path from "node:path";
import { describe, expect, it } from "vitest";
import {
  listLocalLabCatalogEntries,
  type ListLocalLabCatalogOptions,
} from "@/modules/catalog/application/list-local-lab-catalog";

const repositoryRoot = path.join("C:", "repo", "development");
const manifestPath = path.join(
  repositoryRoot,
  "deploy",
  "local-demo",
  "manifests",
  "seven-zip-26.01-x64.json",
);
const artifactBytes = Buffer.from("fixed-lab-artifact");
const artifactHash = createHash("sha256")
  .update(artifactBytes)
  .digest("hex")
  .toUpperCase();

const validManifest = {
  schemaVersion: 1,
  artifactId: "seven-zip-26.01-x64-msi",
  product: "7-Zip",
  version: "26.01",
  packageVersion: "26.01.00.0",
  architecture: "x64",
  fileName: "7z2601-x64.msi",
  length: artifactBytes.length,
  sha256: artifactHash,
  officialUrl: "https://github.com/ip7z/7zip/releases/download/26.01/7z2601-x64.msi",
  officialRelease: "https://github.com/ip7z/7zip/releases/tag/26.01",
  license: "GNU LGPL with BSD components and unRAR restriction",
  licenseUrl: "https://www.7-zip.org/license.txt",
  redistributionNotice: "Binary redistribution must reproduce the license.",
  signature: "not-signed",
  productCode: "{23170F69-40C1-2702-2601-000001000000}",
  upgradeCode: "{23170F69-40C1-2702-0000-000004000000}",
  adapterId: "seven-zip.msi.v1",
  environment: "development-only",
} as const;

describe("local lab catalog", () => {
  it("lists curated manifests as lab-real-sanitized with absent artifact when no mirror is configured", async () => {
    const entries = await listLocalLabCatalogEntries(
      options({ artifacts: new Map() }),
    );

    expect(entries).toHaveLength(1);
    expect(entries[0]).toMatchObject({
      artifactId: "seven-zip-26.01-x64-msi",
      product: "7-Zip",
      architecture: "x64",
      adapterId: "seven-zip.msi.v1",
      source: "lab-real-sanitized",
      scope: "local-demo",
      catalogKind: "laboratory",
      validationStatus: "valid",
      artifactStatus: "absent",
    });
  });

  it("fails closed outside Development or outside local-demo", async () => {
    await expect(
      listLocalLabCatalogEntries(
        options({
          environment: {
            IT_SUPPORT_ENVIRONMENT: "production",
            IT_SUPPORT_EXECUTION_PROFILE: "local-demo",
          },
        }),
      ),
    ).resolves.toEqual([]);

    await expect(
      listLocalLabCatalogEntries(
        options({
          environment: {
            IT_SUPPORT_ENVIRONMENT: "development",
            IT_SUPPORT_EXECUTION_PROFILE: "pilot",
          },
        }),
      ),
    ).resolves.toEqual([]);
  });

  it("reports available only when the mirror artifact has exact length and SHA-256", async () => {
    const mirror = path.join("C:", "lab-mirror");
    const entries = await listLocalLabCatalogEntries(
      options({
        environment: {
          IT_SUPPORT_ENVIRONMENT: "development",
          IT_SUPPORT_EXECUTION_PROFILE: "local-demo",
          IT_SUPPORT_LAB_MIRROR_PATH: mirror,
        },
        artifacts: new Map([[path.join(mirror, "7z2601-x64.msi"), artifactBytes]]),
      }),
    );

    expect(entries[0]?.artifactStatus).toBe("available");
    expect(entries[0]?.artifactStatusDetail).toContain("SHA-256 esperado");
  });

  it("reports hash mismatch before an artifact can be treated as executable", async () => {
    const mirror = path.join("C:", "lab-mirror");
    const entries = await listLocalLabCatalogEntries(
      options({
        environment: {
          IT_SUPPORT_ENVIRONMENT: "development",
          IT_SUPPORT_EXECUTION_PROFILE: "local-demo",
          IT_SUPPORT_LAB_MIRROR_PATH: mirror,
        },
        artifacts: new Map([
          [path.join(mirror, "7z2601-x64.msi"), Buffer.from("changed-artifact!")],
        ]),
      }),
    );

    expect(entries[0]?.artifactStatus).toBe("hash_mismatch");
  });

  it("marks manifests invalid when license, version, or public origin are not allowed", async () => {
    const entries = await listLocalLabCatalogEntries(
      options({
        manifest: {
          ...validManifest,
          version: "latest",
          license: "Commercial Trial",
          officialUrl: "https://downloads.example.test/package.msi",
        },
      }),
    );

    expect(entries[0]?.validationStatus).toBe("invalid");
    expect(entries[0]?.validationMessages).toEqual([
      "La licencia no esta en la allowlist redistribuible.",
      "La version debe estar fijada; no se aceptan latest o rangos.",
      "El origen publico del artefacto no esta permitido.",
    ]);
  });
});

function options({
  artifacts = new Map<string, Buffer>(),
  environment = {
    IT_SUPPORT_ENVIRONMENT: "development",
    IT_SUPPORT_EXECUTION_PROFILE: "local-demo",
  },
  manifest = validManifest,
}: {
  readonly artifacts?: ReadonlyMap<string, Buffer>;
  readonly environment?: Readonly<Record<string, string | undefined>>;
  readonly manifest?: Readonly<Record<string, unknown>>;
} = {}): ListLocalLabCatalogOptions {
  return {
    environment,
    repositoryRoot,
    fileSystem: {
      readFile(filePath) {
        const normalized = path.normalize(filePath);
        if (normalized === path.normalize(manifestPath)) {
          return Promise.resolve(Buffer.from(JSON.stringify(manifest)));
        }

        const artifact = artifacts.get(normalized);
        if (artifact !== undefined) {
          return Promise.resolve(artifact);
        }

        return Promise.reject(notFound());
      },
      stat(filePath) {
        const artifact = artifacts.get(path.normalize(filePath));
        if (artifact === undefined) {
          return Promise.reject(notFound());
        }

        return Promise.resolve({
          size: artifact.length,
          isFile: () => true,
        });
      },
    },
  };
}

function notFound(): NodeJS.ErrnoException {
  const error = new Error("not found") as NodeJS.ErrnoException;
  error.code = "ENOENT";
  return error;
}
