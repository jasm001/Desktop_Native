import { stat } from "node:fs/promises";
import path from "node:path";
import type { AdminLabHealthProvider } from "../application/admin-lab-health-provider";
import type { AdminLabComponentStatus } from "../application/admin-read-repository";

interface StatLike {
  isDirectory(): boolean;
}

interface LabFileSystem {
  stat(path: string): Promise<StatLike>;
}

type LabFetch = (
  input: string,
  init?: {
    readonly headers?: Readonly<Record<string, string>>;
    readonly signal?: AbortSignal;
  },
) => Promise<{ readonly ok: boolean; readonly status: number }>;

const probeTimeoutMs = 1500;

const defaultFileSystem: LabFileSystem = {
  stat,
};

const defaultFetch: LabFetch = (input, init) => fetch(input, init);

export class LocalLabHealthProvider implements AdminLabHealthProvider {
  public constructor(
    private readonly environment: Readonly<Record<string, string | undefined>> =
      process.env,
    private readonly fetcher: LabFetch = defaultFetch,
    private readonly fileSystem: LabFileSystem = defaultFileSystem,
    private readonly workspaceRoot: string = process.cwd(),
    private readonly now: () => Date = () => new Date(),
  ) {}

  public async listComponentStatuses(): Promise<
    readonly AdminLabComponentStatus[]
  > {
    if (this.environment.IT_SUPPORT_ENVIRONMENT !== "development") {
      return [
        denied("hermes", "Hermes local"),
        denied("artifact-mirror", "Mirror de artefactos"),
        denied("lab-bridge", "Bridge de laboratorio"),
      ];
    }

    const checkedAt = this.now();
    return Promise.all([
      this.getHermesStatus(checkedAt),
      this.getMirrorStatus(checkedAt),
      this.getBridgeStatus(checkedAt),
    ]);
  }

  private async getHermesStatus(
    checkedAt: Date,
  ): Promise<AdminLabComponentStatus> {
    const enabled = this.environment.IT_SUPPORT_HERMES_CHAT_ENABLED === "true";
    const baseUrl = this.environment.IT_SUPPORT_HERMES_BASE_URL;
    const apiKey = this.environment.IT_SUPPORT_HERMES_API_KEY;

    if (!enabled) {
      return component({
        id: "hermes",
        name: "Hermes local",
        status: "offline",
        mode: "not-configured",
        detail: "Proveedor opcional apagado para esta sesion.",
        lastCheckedAt: null,
      });
    }

    const modelsUrl = toLoopbackUrl(baseUrl, "/models");
    if (modelsUrl === null || apiKey === undefined || apiKey.trim() === "") {
      return component({
        id: "hermes",
        name: "Hermes local",
        status: "misconfigured",
        mode: "health-check",
        detail: "Configuracion local incompleta o no loopback.",
        lastCheckedAt: checkedAt,
      });
    }

    const reachable = await probeHttp(modelsUrl, this.fetcher, {
      Authorization: `Bearer ${apiKey}`,
    });

    return component({
      id: "hermes",
      name: "Hermes local",
      status: reachable,
      mode: "health-check",
      detail:
        reachable === "available"
          ? "Endpoint compatible con OpenAI respondio en loopback."
          : "Endpoint local no respondio dentro del limite acotado.",
      lastCheckedAt: checkedAt,
    });
  }

  private async getMirrorStatus(
    checkedAt: Date,
  ): Promise<AdminLabComponentStatus> {
    const mirrorPath = this.environment.IT_SUPPORT_LAB_MIRROR_PATH;

    if (mirrorPath === undefined || mirrorPath.trim() === "") {
      return component({
        id: "artifact-mirror",
        name: "Mirror de artefactos",
        status: "offline",
        mode: "not-configured",
        detail: "Ruta local del mirror no configurada en esta sesion.",
        lastCheckedAt: null,
      });
    }

    const resolvedMirror = path.resolve(mirrorPath);
    const resolvedWorkspace = path.resolve(this.workspaceRoot);
    if (isInsideWorkspace(resolvedMirror, resolvedWorkspace)) {
      return component({
        id: "artifact-mirror",
        name: "Mirror de artefactos",
        status: "misconfigured",
        mode: "health-check",
        detail: "El mirror debe vivir fuera del repositorio Git.",
        lastCheckedAt: checkedAt,
      });
    }

    try {
      const mirror = await this.fileSystem.stat(resolvedMirror);
      if (!mirror.isDirectory()) {
        return component({
          id: "artifact-mirror",
          name: "Mirror de artefactos",
          status: "misconfigured",
          mode: "health-check",
          detail: "La ruta configurada no es un directorio de laboratorio.",
          lastCheckedAt: checkedAt,
        });
      }
      return component({
        id: "artifact-mirror",
        name: "Mirror de artefactos",
        status: "available",
        mode: "health-check",
        detail: "Directorio local de mirror disponible fuera de Git.",
        lastCheckedAt: checkedAt,
      });
    } catch (error) {
      return component({
        id: "artifact-mirror",
        name: "Mirror de artefactos",
        status: isNotFound(error) ? "offline" : "unavailable",
        mode: "health-check",
        detail: "Mirror local no disponible para lectura acotada.",
        lastCheckedAt: checkedAt,
      });
    }
  }

  private async getBridgeStatus(
    checkedAt: Date,
  ): Promise<AdminLabComponentStatus> {
    const mode = this.environment.IT_SUPPORT_LAB_BRIDGE_MODE;
    const baseUrl = this.environment.IT_SUPPORT_LAB_BRIDGE_BASE_URL;

    if (mode !== undefined && mode !== "validate-only") {
      return bridgeComponent({
        status: "misconfigured",
        mode: "validate-only",
        detail: "El bridge de laboratorio solo acepta modo validate-only.",
        lastCheckedAt: checkedAt,
      });
    }

    if (baseUrl === undefined || baseUrl.trim() === "") {
      return bridgeComponent({
        status: "validate-only",
        mode: "not-configured",
        detail: "Patron validate-only documentado; sin endpoint local activo.",
        lastCheckedAt: null,
      });
    }

    const healthUrl = toLoopbackUrl(baseUrl, "/health");
    if (healthUrl === null) {
      return bridgeComponent({
        status: "misconfigured",
        mode: "validate-only",
        detail: "El health del bridge debe apuntar a loopback.",
        lastCheckedAt: checkedAt,
      });
    }

    const reachable = await probeHttp(healthUrl, this.fetcher);
    return bridgeComponent({
      status: reachable === "available" ? "validate-only" : reachable,
      mode: "validate-only",
      detail:
        reachable === "available"
          ? "Health local respondio; validacion sin despliegue enviado."
          : "Health local del bridge no respondio sin prometer despliegue.",
      lastCheckedAt: checkedAt,
    });
  }
}

export function createFakeTicketingHealth(
  ticketCount: number,
  checkedAt: Date,
): AdminLabComponentStatus {
  return component({
    id: "ticketing-fake",
    name: "Ticketing fake",
    status: "available",
    source: "fake",
    scope: "local-demo",
    mode: "read-only",
    detail: `${ticketCount} registros fake persistidos localmente; no son OpenText.`,
    lastCheckedAt: checkedAt,
  });
}

function denied(id: string, name: string): AdminLabComponentStatus {
  return component({
    id,
    name,
    status: "unavailable",
    mode: "not-configured",
    detail: "Health local deshabilitado fuera de Development.",
    lastCheckedAt: null,
  });
}

function component(
  input: Omit<
    AdminLabComponentStatus,
    "source" | "scope" | "mode"
  > &
    Partial<Pick<AdminLabComponentStatus, "source" | "scope" | "mode">>,
): AdminLabComponentStatus {
  return {
    source: "local",
    scope: "local-demo",
    mode: "health-check",
    ...input,
  };
}

function bridgeComponent(
  input: Pick<
    AdminLabComponentStatus,
    "status" | "mode" | "detail" | "lastCheckedAt"
  >,
): AdminLabComponentStatus {
  return component({
    id: "lab-bridge",
    name: "Bridge de laboratorio",
    source: "validate-only",
    scope: "validate-only",
    ...input,
  });
}

async function probeHttp(
  url: string,
  fetcher: LabFetch,
  headers: Readonly<Record<string, string>> = {},
): Promise<"available" | "offline" | "unavailable"> {
  try {
    const response = await fetcher(url, {
      headers,
      signal: AbortSignal.timeout(probeTimeoutMs),
    });
    return response.ok ? "available" : "unavailable";
  } catch {
    return "offline";
  }
}

function toLoopbackUrl(
  baseUrl: string | undefined,
  suffix: string,
): string | null {
  if (baseUrl === undefined || baseUrl.trim() === "") {
    return null;
  }

  try {
    const parsed = new URL(baseUrl);
    if (parsed.protocol !== "http:" && parsed.protocol !== "https:") {
      return null;
    }
    if (!isLoopbackHost(parsed.hostname)) {
      return null;
    }
    return new URL(`.${suffix}`, parsed.href.endsWith("/") ? parsed : `${parsed.href}/`)
      .href;
  } catch {
    return null;
  }
}

function isLoopbackHost(hostname: string): boolean {
  return hostname === "localhost" || hostname === "127.0.0.1" || hostname === "::1";
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
