// @vitest-environment jsdom

import "@testing-library/jest-dom/vitest";
import { cleanup, render, screen, within } from "@testing-library/react";
import { afterEach, describe, expect, it } from "vitest";
import type { CatalogProduct } from "@it-support-native/control-plane-contracts";
import type {
  AdminAuditSummary,
  AdminOperationSummary,
} from "@/modules/administration/application/admin-read-repository";
import { getAdminApprovalsSkeleton } from "@/modules/administration/application/get-admin-skeleton";
import { AdminAccessDenied } from "@/modules/administration/ui/admin-access-denied";
import { AdminAuditContent } from "@/modules/administration/ui/admin-audit-content";
import { AdminCatalogContent } from "@/modules/administration/ui/admin-catalog-content";
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
