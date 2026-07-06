// @vitest-environment jsdom

import "@testing-library/jest-dom/vitest";
import { cleanup, render, screen, within } from "@testing-library/react";
import { afterEach, describe, expect, it } from "vitest";
import type { CatalogProduct } from "@it-support-native/control-plane-contracts";
import type {
  AdminAuditSummary,
  AdminLabReadModel,
  AdminOperationSummary,
} from "@/modules/administration/application/admin-read-repository";
import { getAdminApprovalsSkeleton } from "@/modules/administration/application/get-admin-skeleton";
import { AdminAccessDenied } from "@/modules/administration/ui/admin-access-denied";
import { AdminAuditContent } from "@/modules/administration/ui/admin-audit-content";
import { AdminCatalogContent } from "@/modules/administration/ui/admin-catalog-content";
import { AdminLabContent } from "@/modules/administration/ui/admin-lab-content";
import { AdminOperationsContent } from "@/modules/administration/ui/admin-operations-content";
import { AdminShell } from "@/modules/administration/ui/admin-shell";
import { AdminSkeletonContent } from "@/modules/administration/ui/admin-skeleton-content";
import type { PortalPrincipal } from "@/modules/identity/domain/portal-principal";

const principal: PortalPrincipal = {
  subject: "local-portal-operator-001",
  displayName: "Operador de desarrollo",
  role: "DeveloperAllAccess",
};

afterEach(() => {
  cleanup();
});

describe("admin portal components", () => {
  it("renders the shell with semantic navigation, active state, and keyboard skip target", () => {
    render(
      <AdminShell
        activeItem="operations"
        description="Vista local de solo lectura."
        kicker="Operaciones"
        principal={principal}
        title="Actividad reciente"
      >
        <p>Contenido protegido</p>
      </AdminShell>,
    );

    const navigation = screen.getByRole("navigation");
    expect(
      within(navigation).getByRole("link", { name: "Operaciones" }),
    ).toHaveAttribute("aria-current", "page");
    expect(within(navigation).getByRole("link", { name: "Acceso" })).toHaveAttribute(
      "href",
      "/admin/access",
    );
    expect(
      within(navigation).getByRole("link", { name: "Aprobaciones" }),
    ).toHaveAttribute("href", "/admin/approvals");
    expect(
      within(navigation).getByRole("link", { name: "Laboratorio" }),
    ).toHaveAttribute("href", "/admin/lab");
    expect(screen.getByRole("main")).toHaveAttribute("id", "admin-content");
    expect(
      screen.getByRole("link", {
        name: "Saltar al contenido principal",
      }),
    ).toHaveAttribute("href", "#admin-content");
    expect(screen.getByLabelText("Identidad actual del portal")).toHaveTextContent(
      "DeveloperAllAccess",
    );
    expect(screen.getByText("Solo lectura")).toBeVisible();
    expect(document.querySelector("form")).toBeNull();
    expect(screen.queryByRole("button")).not.toBeInTheDocument();
  });

  it("renders the catalog as a bounded read-only table", () => {
    const products: CatalogProduct[] = [
      {
        id: "secure-transfer",
        name: "Secure Transfer",
        version: "6.5",
        status: "approved",
        actionId: "software.install.simulated.v1",
      },
      {
        id: "legacy-tool",
        name: "Legacy Tool",
        version: "1.0",
        status: "end_of_life",
        actionId: null,
      },
    ];

    render(<AdminCatalogContent products={products} />);

    const table = screen.getByRole("table");
    expect(within(table).getByRole("columnheader", { name: "Producto" })).toBeVisible();
    expect(within(table).getByRole("columnheader", { name: /Versi/u })).toBeVisible();
    expect(screen.getByText("2 registros")).toBeVisible();
    expect(screen.getByText("Secure Transfer")).toBeVisible();
    expect(screen.getByText(/Sin acci/u)).toBeVisible();
  });

  it("renders operation rows with table semantics and the empty state without controls", () => {
    const operations: AdminOperationSummary[] = [
      {
        id: "7d35a43a-cdb2-4fe1-941a-2234fb749652",
        reference: "REQ-LOCAL-001",
        deviceName: "Synthetic Windows 11 device",
        productName: "secure-transfer 6.5",
        requestStatus: "CONFIRMED",
        jobStatus: "QUEUED",
        caseStatus: "OPEN",
        ticketReference: null,
        createdAt: new Date("2026-06-15T12:00:00.000Z"),
      },
    ];

    const { rerender } = render(<AdminOperationsContent operations={operations} />);

    expect(screen.getByRole("table")).toBeVisible();
    expect(screen.getByText("REQ-LOCAL-001")).toBeVisible();
    expect(screen.getByText("Sin ticket")).toBeVisible();

    rerender(<AdminOperationsContent operations={[]} />);

    expect(screen.getByText("Sin actividad")).toBeVisible();
    expect(screen.queryByRole("table")).not.toBeInTheDocument();
    expect(screen.queryByRole("button")).not.toBeInTheDocument();
  });

  it("renders audit rows without payload details and keeps the empty state accessible", () => {
    const events: AdminAuditSummary[] = [
      {
        id: "1a1d7471-52d5-44cd-9af5-98ec2d78139a",
        eventType: "support-request.created",
        entityType: "support-request",
        entityId: "REQ-LOCAL-001",
        actorSubject: "development-user-001",
        correlationId: "correlation-1",
        createdAt: new Date("2026-06-15T12:00:00.000Z"),
      },
    ];

    const { rerender } = render(<AdminAuditContent events={events} />);

    expect(screen.getByRole("table")).toBeVisible();
    expect(screen.getByText("support-request.created")).toBeVisible();
    expect(screen.queryByText("payload")).not.toBeInTheDocument();

    rerender(<AdminAuditContent events={[]} />);

    expect(screen.getByText("Sin evidencia")).toBeVisible();
    expect(screen.queryByRole("table")).not.toBeInTheDocument();
  });

  it("renders local skeleton surfaces as read-only empty states", () => {
    render(<AdminSkeletonContent view={getAdminApprovalsSkeleton()} />);

    expect(
      screen.getByRole("heading", { name: "Superficie protegida" }),
    ).toBeVisible();
    expect(screen.getByText("Sin acciones configuradas")).toBeVisible();
    expect(screen.getAllByText("Diferido")).toHaveLength(2);
    expect(screen.getByText("Bloqueado")).toBeVisible();
    expect(screen.queryByRole("button")).not.toBeInTheDocument();
    expect(document.querySelector("form")).toBeNull();
  });

  it("renders lab-real status and persisted local summaries without actions", () => {
    const lab: AdminLabReadModel = {
      components: [
        {
          id: "postgresql",
          name: "PostgreSQL local",
          status: "available",
          source: "lab-real-sanitized",
          scope: "development",
          mode: "read-only",
          detail: "Lectura bounded completada.",
          lastCheckedAt: new Date("2026-07-05T12:00:00.000Z"),
        },
        {
          id: "windows-vm",
          name: "VM Windows 11",
          status: "not_checked",
          source: "local",
          scope: "local-demo",
          mode: "not-configured",
          detail: "La VM se inicia manualmente.",
          lastCheckedAt: null,
        },
      ],
      metrics: [
        {
          label: "Solicitudes",
          value: 2,
          source: "lab-real-sanitized",
          detail: "Filas reales de support_requests.",
        },
        {
          label: "Tickets fake",
          value: 1,
          source: "fake",
          detail: "Registros fake persistidos localmente.",
        },
      ],
      recentOperations: [],
      recentAuditEvents: [],
      recentOutboxEvents: [
        {
          id: "632c99fb-b56a-49d6-873f-4315482fdab8",
          eventType: "support-request.confirmed.v1",
          aggregateType: "support-request",
          status: "PENDING",
          attemptCount: 0,
          createdAt: new Date("2026-07-05T12:00:00.000Z"),
        },
      ],
      recentExternalTickets: [
        {
          id: "cc794622-391f-4ed2-bf78-ded50ed7e902",
          reference: "FAKE-1234",
          provider: "FAKE",
          status: "OPEN",
          reasonCode: "execution_failed",
          correlationId: "lab-correlation",
          createdAt: new Date("2026-07-05T12:00:00.000Z"),
        },
      ],
      boundaries: ["Sin llamadas directas al DeviceAgent."],
    };

    render(<AdminLabContent lab={lab} />);

    expect(
      screen.getByRole("heading", { name: "Estado del laboratorio" }),
    ).toBeVisible();
    expect(screen.getByText("PostgreSQL local")).toBeVisible();
    expect(screen.getByText("No comprobado")).toBeVisible();
    expect(screen.getByText("support-request.confirmed.v1")).toBeVisible();
    expect(screen.getByText("FAKE-1234")).toBeVisible();
    expect(screen.getAllByText("lab-real-sanitized").length).toBeGreaterThan(0);
    expect(screen.getByText(/development - read-only/u)).toBeVisible();
    expect(screen.getByText(/local-demo - not-configured/u)).toBeVisible();
    expect(screen.queryByRole("button")).not.toBeInTheDocument();
    expect(document.querySelector("form")).toBeNull();
    expect(screen.queryByText("payload")).not.toBeInTheDocument();
  });

  it("renders access denied without protected portal content", () => {
    render(<AdminAccessDenied />);

    expect(
      screen.getByRole("heading", {
        name: "Acceso administrativo no disponible",
      }),
    ).toBeVisible();
    expect(screen.getByRole("link", { name: "Volver al estado del plano de control" })).toHaveAttribute(
      "href",
      "/",
    );
    expect(screen.queryByText("Operador de desarrollo")).not.toBeInTheDocument();
    expect(screen.queryByText("Centro de operaciones")).not.toBeInTheDocument();
  });
});
