export interface AdminOverview {
  readonly environment: {
    readonly name: "Desarrollo local";
    readonly access: "Solo lectura";
    readonly integrations: "Conexiones corporativas deshabilitadas";
  };
  readonly capabilities: readonly {
    readonly name: string;
    readonly detail: string;
    readonly status: "Available" | "Blocked";
  }[];
  readonly activity: readonly {
    readonly title: string;
    readonly detail: string;
    readonly state: "Completed" | "In progress" | "Blocked";
  }[];
  readonly boundaries: readonly string[];
}

const adminOverview: AdminOverview = {
  environment: {
    name: "Desarrollo local",
    access: "Solo lectura",
    integrations: "Conexiones corporativas deshabilitadas",
  },
  capabilities: [
    {
      name: "Plano de control",
      detail: "Las APIs versionadas, PostgreSQL, auditoría y outbox permanecen disponibles.",
      status: "Available",
    },
    {
      name: "Casos internos",
      detail: "Los BotCase sintéticos y tickets fake pueden consultarse.",
      status: "Available",
    },
    {
      name: "Mutaciones administrativas",
      detail: "Cambios de usuarios, roles, catálogo, aprobaciones y configuración.",
      status: "Blocked",
    },
    {
      name: "Integraciones corporativas",
      detail: "Entra, OpenText, Teams, Rescue, UEMS y proveedores productivos.",
      status: "Blocked",
    },
  ],
  activity: [
    {
      title: "Bloques 0-8",
      detail: "Las bases locales permanecen validadas y completadas.",
      state: "Completed",
    },
    {
      title: "Bloque 11",
      detail:
        "Identidad, navegacion protegida y lecturas administrativas acotadas.",
      state: "In progress",
    },
    {
      title: "Bloques 9 y 10",
      detail: "La evidencia externa de Teams y piloto sigue pendiente.",
      state: "Blocked",
    },
  ],
  boundaries: [
    "La autorización se evalúa en servidor antes de renderizar contenido protegido.",
    "El portal no ejecuta comandos ni llama al DeviceAgent.",
    "No se carga identidad, endpoint, credencial o dato corporativo.",
    "La ruta no expone formularios, Server Actions ni mutaciones administrativas.",
  ],
};

export function getAdminOverview(): AdminOverview {
  return adminOverview;
}
