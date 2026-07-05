export interface AdminSkeletonSection {
  readonly title: string;
  readonly status: "Available" | "Deferred" | "Blocked";
  readonly detail: string;
}

export interface AdminSkeletonView {
  readonly source: string;
  readonly sections: readonly AdminSkeletonSection[];
  readonly boundaries: readonly string[];
}

const commonBoundaries = [
  "Proteccion server-side antes de renderizar contenido administrativo.",
  "Vista sintetica de solo lectura, sin formularios ni Server Actions.",
  "Sin Entra, OpenText, Rescue, UEMS, Teams ni DeviceAgent directo.",
] as const;

const accessSkeleton: AdminSkeletonView = {
  source: "Identidad local sintetica",
  sections: [
    {
      title: "Identidad del portal",
      status: "Available",
      detail:
        "Operador local separado del usuario de API y de la identidad del agente.",
    },
    {
      title: "Roles y scopes productivos",
      status: "Deferred",
      detail:
        "Pendientes de App Registration, owners, matriz de segregacion y asignaciones reales.",
    },
    {
      title: "DeveloperAllAccess",
      status: "Blocked",
      detail:
        "Aceptado solo en Development con flag explicito; se rechaza fuera del perfil local.",
    },
  ],
  boundaries: commonBoundaries,
};

const approvalsSkeleton: AdminSkeletonView = {
  source: "Politica descriptiva local",
  sections: [
    {
      title: "Software no listado",
      status: "Deferred",
      detail:
        "Requiere aprobacion de seguridad antes de cualquier flujo mutable.",
    },
    {
      title: "Licencias y compras",
      status: "Deferred",
      detail:
        "Pendiente de owner, centro/proyecto y proceso corporativo confirmado.",
    },
    {
      title: "Publicacion de catalogo",
      status: "Blocked",
      detail:
        "Nadie puede crear, aprobar y publicar una entrada productiva por si solo.",
    },
  ],
  boundaries: commonBoundaries,
};

const supportSkeleton: AdminSkeletonView = {
  source: "Adaptadores fake desconectados",
  sections: [
    {
      title: "Tickets sinteticos",
      status: "Available",
      detail:
        "Las escalaciones locales usan ITicketingProvider fake y referencias FAKE.",
    },
    {
      title: "OpenText Service Manager",
      status: "Deferred",
      detail:
        "Pendiente de API, autenticacion, ambiente de prueba y campos aprobados.",
    },
    {
      title: "Rescue remoto",
      status: "Deferred",
      detail:
        "El MVP conserva el flujo humano actual; no hay API ni iframe habilitado.",
    },
  ],
  boundaries: commonBoundaries,
};

const reportingSkeleton: AdminSkeletonView = {
  source: "Resumen local no exportable",
  sections: [
    {
      title: "Auditoria visible",
      status: "Available",
      detail:
        "La ruta de auditoria existente muestra eventos recientes sin payload.",
    },
    {
      title: "Reportes agregados",
      status: "Deferred",
      detail:
        "Pendientes de retencion, datos aprobados, observabilidad y revision Security.",
    },
    {
      title: "Configuracion productiva",
      status: "Blocked",
      detail:
        "No existen secretos, tenants, ambientes, owners ni despliegue corporativo.",
    },
  ],
  boundaries: commonBoundaries,
};

export function getAdminAccessSkeleton(): AdminSkeletonView {
  return accessSkeleton;
}

export function getAdminApprovalsSkeleton(): AdminSkeletonView {
  return approvalsSkeleton;
}

export function getAdminSupportSkeleton(): AdminSkeletonView {
  return supportSkeleton;
}

export function getAdminReportingSkeleton(): AdminSkeletonView {
  return reportingSkeleton;
}
