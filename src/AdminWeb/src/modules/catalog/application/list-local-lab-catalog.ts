import { createHash } from "node:crypto";
import { readFile, stat } from "node:fs/promises";
import path from "node:path";
import { z } from "zod";

export type LocalLabArtifactStatus = "available" | "absent" | "hash_mismatch";

export interface LocalLabCatalogEntry {
  readonly artifactId: string;
  readonly product: string;
  readonly version: string;
  readonly packageVersion: string;
  readonly architecture: string;
  readonly license: string;
  readonly licenseUrl: string;
  readonly originUrl: string;
  readonly publicReleaseUrl: string;
  readonly sha256: string;
  readonly adapterId: string;
  readonly source: "lab-real-sanitized";
  readonly scope: "local-demo";
  readonly catalogKind: "laboratory";
  readonly validationStatus: "valid" | "invalid";
  readonly validationMessages: readonly string[];
  readonly artifactStatus: LocalLabArtifactStatus;
  readonly artifactStatusDetail: string;
}

interface FileInfo {
  readonly size: number;
  isFile(): boolean;
}

interface LabCatalogFileSystem {
  readFile(path: string): Promise<Buffer>;
  stat(path: string): Promise<FileInfo>;
}

export interface ListLocalLabCatalogOptions {
  readonly environment?: Readonly<Record<string, string | undefined>>;
  readonly repositoryRoot?: string;
  readonly fileSystem?: LabCatalogFileSystem;
}

const manifestRelativePaths = [
  path.join("deploy", "local-demo", "manifests", "seven-zip-26.01-x64.json"),
] as const;

const publicArtifactHosts = new Set(["github.com"]);
const redistributableLicenses = new Set([
  "GNU LGPL with BSD components and unRAR restriction",
]);
const fixedVersionPattern = /^\d+(?:\.\d+){1,3}$/u;
const sha256Pattern = /^[A-F0-9]{64}$/u;

const manifestSchema = z
  .object({
    schemaVersion: z.literal(1),
    artifactId: z.string().min(1),
    product: z.string().min(1),
    version: z.string().min(1),
    packageVersion: z.string().min(1),
    architecture: z.enum(["x64", "x86", "arm64"]),
    fileName: z.string().min(1),
    length: z.number().int().positive(),
    sha256: z.string().regex(sha256Pattern),
    officialUrl: z.url(),
    officialRelease: z.url(),
    license: z.string().min(1),
    licenseUrl: z.url(),
    redistributionNotice: z.string().min(1),
    signature: z.string().min(1),
    productCode: z.string().min(1),
    upgradeCode: z.string().min(1),
    adapterId: z.string().min(1),
    environment: z.literal("development-only"),
  })
  .strict();

type LocalLabManifest = z.infer<typeof manifestSchema>;

const defaultFileSystem: LabCatalogFileSystem = {
  readFile,
  stat,
};

export async function listLocalLabCatalogEntries({
  environment = process.env,
  repositoryRoot = resolveRepositoryRoot(process.cwd()),
  fileSystem = defaultFileSystem,
}: ListLocalLabCatalogOptions = {}): Promise<readonly LocalLabCatalogEntry[]> {
  if (!isLocalDemoCatalogEnabled(environment)) {
    return [];
  }

  return Promise.all(
    manifestRelativePaths.map(async (relativePath) => {
      const manifestPath = path.join(repositoryRoot, relativePath);
      const manifest = manifestSchema.parse(
        JSON.parse((await fileSystem.readFile(manifestPath)).toString("utf8")),
      );
      const validationMessages = validateManifest(manifest);
      const artifact = await resolveArtifactStatus(
        manifest,
        environment,
        repositoryRoot,
        fileSystem,
      );

      return {
        artifactId: manifest.artifactId,
        product: manifest.product,
        version: manifest.version,
        packageVersion: manifest.packageVersion,
        architecture: manifest.architecture,
        license: manifest.license,
        licenseUrl: manifest.licenseUrl,
        originUrl: manifest.officialUrl,
        publicReleaseUrl: manifest.officialRelease,
        sha256: manifest.sha256,
        adapterId: manifest.adapterId,
        source: "lab-real-sanitized",
        scope: "local-demo",
        catalogKind: "laboratory",
        validationStatus: validationMessages.length === 0 ? "valid" : "invalid",
        validationMessages,
        artifactStatus: artifact.status,
        artifactStatusDetail: artifact.detail,
      } satisfies LocalLabCatalogEntry;
    }),
  );
}

function isLocalDemoCatalogEnabled(
  environment: Readonly<Record<string, string | undefined>>,
): boolean {
  const profile = environment.IT_SUPPORT_EXECUTION_PROFILE ?? "local-demo";
  return (
    environment.IT_SUPPORT_ENVIRONMENT === "development" &&
    profile === "local-demo"
  );
}

function validateManifest(manifest: LocalLabManifest): readonly string[] {
  const messages: string[] = [];

  if (!redistributableLicenses.has(manifest.license)) {
    messages.push("La licencia no esta en la allowlist redistribuible.");
  }

  if (
    !fixedVersionPattern.test(manifest.version) ||
    !fixedVersionPattern.test(manifest.packageVersion)
  ) {
    messages.push("La version debe estar fijada; no se aceptan latest o rangos.");
  }

  const origin = new URL(manifest.officialUrl);
  if (origin.protocol !== "https:" || !publicArtifactHosts.has(origin.hostname)) {
    messages.push("El origen publico del artefacto no esta permitido.");
  }

  if (!sha256Pattern.test(manifest.sha256)) {
    messages.push("El hash SHA-256 debe tener 64 caracteres hexadecimales.");
  }

  return messages;
}

async function resolveArtifactStatus(
  manifest: LocalLabManifest,
  environment: Readonly<Record<string, string | undefined>>,
  repositoryRoot: string,
  fileSystem: LabCatalogFileSystem,
): Promise<{
  readonly status: LocalLabArtifactStatus;
  readonly detail: string;
}> {
  const mirrorPath = environment.IT_SUPPORT_LAB_MIRROR_PATH;
  if (mirrorPath === undefined || mirrorPath.trim() === "") {
    return {
      status: "absent",
      detail: "Mirror local no configurado; no se versiona instalador en Git.",
    };
  }

  const resolvedMirror = path.resolve(mirrorPath);
  if (isInsideWorkspace(resolvedMirror, path.resolve(repositoryRoot))) {
    return {
      status: "absent",
      detail: "Mirror rechazado porque apunta dentro del repositorio Git.",
    };
  }

  const artifactPath = path.join(resolvedMirror, manifest.fileName);
  try {
    const artifactInfo = await fileSystem.stat(artifactPath);
    if (!artifactInfo.isFile()) {
      return {
        status: "absent",
        detail: "La entrada del mirror no es un archivo regular.",
      };
    }

    if (artifactInfo.size !== manifest.length) {
      return {
        status: "hash_mismatch",
        detail: "La longitud del artefacto no coincide con el manifiesto.",
      };
    }

    const bytes = await fileSystem.readFile(artifactPath);
    const actualHash = createHash("sha256").update(bytes).digest("hex").toUpperCase();
    return actualHash === manifest.sha256
      ? {
          status: "available",
          detail: "Artefacto disponible en mirror local con SHA-256 esperado.",
        }
      : {
          status: "hash_mismatch",
          detail: "El SHA-256 del artefacto no coincide con el manifiesto.",
        };
  } catch (error) {
    if (isNotFound(error)) {
      return {
        status: "absent",
        detail: "Artefacto ausente en el mirror local configurado.",
      };
    }

    return {
      status: "absent",
      detail: "No fue posible leer el artefacto de laboratorio.",
    };
  }
}

function resolveRepositoryRoot(startPath: string): string {
  let current = path.resolve(startPath);
  for (;;) {
    if (path.basename(current) === "development") {
      return current;
    }

    const parent = path.dirname(current);
    if (parent === current) {
      return path.resolve(startPath);
    }

    current = parent;
  }
}

function isInsideWorkspace(candidate: string, workspaceRoot: string): boolean {
  const relative = path.relative(workspaceRoot, candidate);
  return relative === "" || (!relative.startsWith("..") && !path.isAbsolute(relative));
}

function isNotFound(error: unknown): boolean {
  return (
    typeof error === "object" &&
    error !== null &&
    "code" in error &&
    error.code === "ENOENT"
  );
}
