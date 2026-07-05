import Link from "next/link";
import type { ReactNode } from "react";
import type { PortalPrincipal } from "../../identity/domain/portal-principal";

export type AdminNavigationKey =
  | "overview"
  | "catalog"
  | "operations"
  | "audit"
  | "access"
  | "approvals"
  | "support"
  | "reporting";

interface AdminShellProps {
  readonly activeItem: AdminNavigationKey;
  readonly children: ReactNode;
  readonly description: string;
  readonly kicker: string;
  readonly principal: PortalPrincipal;
  readonly title: string;
}

const navigation = [
  { key: "overview", label: "Resumen", href: "/admin", icon: HomeIcon },
  {
    key: "catalog",
    label: "Catálogo",
    href: "/admin/catalog",
    icon: ServicesIcon,
  },
  {
    key: "operations",
    label: "Operaciones",
    href: "/admin/operations",
    icon: OperationsIcon,
  },
  {
    key: "audit",
    label: "Auditoría",
    href: "/admin/audit",
    icon: EvidenceIcon,
  },
  {
    key: "access",
    label: "Acceso",
    href: "/admin/access",
    icon: AccessIcon,
  },
  {
    key: "approvals",
    label: "Aprobaciones",
    href: "/admin/approvals",
    icon: ApprovalsIcon,
  },
  {
    key: "support",
    label: "Soporte",
    href: "/admin/support",
    icon: SupportIcon,
  },
  {
    key: "reporting",
    label: "Reportes",
    href: "/admin/reporting",
    icon: ReportingIcon,
  },
] as const;

export function AdminShell({
  activeItem,
  children,
  description,
  kicker,
  principal,
  title,
}: AdminShellProps) {
  return (
    <div className="admin-shell" lang="es">
      <a className="skip-link" href="#admin-content">
        Saltar al contenido principal
      </a>

      <aside className="admin-sidebar" aria-label="Navegación administrativa">
        <Link
          className="admin-brand"
          href="/admin"
          aria-label="Inicio administrativo de IT Support Native"
        >
          <span className="admin-brand__mark" aria-hidden="true">
            IT
          </span>
          <span>
            <strong>IT Support</strong>
            <small>Native Admin</small>
          </span>
        </Link>

        <nav className="admin-navigation">
          {navigation.map((item) => {
            const Icon = item.icon;
            const isActive = item.key === activeItem;

            return (
              <Link
                className={
                  isActive ? "admin-nav-link is-active" : "admin-nav-link"
                }
                href={item.href}
                key={item.href}
                aria-current={isActive ? "page" : undefined}
              >
                <Icon />
                <span>{item.label}</span>
              </Link>
            );
          })}
        </nav>

        <div className="admin-sidebar__footer">
          <span className="availability-dot" aria-hidden="true" />
          <span>
            <strong>Ambiente local</strong>
            <small>Servicios corporativos desactivados</small>
          </span>
        </div>
      </aside>

      <div className="admin-workspace">
        <header className="admin-topbar">
          <div>
            <p>Portal administrativo</p>
            <span>Lecturas protegidas y limitadas</span>
          </div>
          <div className="admin-identity" aria-label="Identidad actual del portal">
            <span className="admin-identity__avatar" aria-hidden="true">
              SO
            </span>
            <span>
              <strong>{principal.displayName}</strong>
              <small>{principal.role}</small>
            </span>
          </div>
        </header>

        <main id="admin-content" className="admin-content">
          <section className="admin-heading" aria-labelledby="page-title">
            <div>
              <p className="section-kicker">{kicker}</p>
              <h1 id="page-title">{title}</h1>
              <p>{description}</p>
            </div>
            <div className="read-only-state">
              <LockIcon />
              <span>Solo lectura</span>
            </div>
          </section>

          {children}

          <footer className="admin-footer">
            <span>IT Support Native</span>
            <span>Datos sintéticos · Sin conexiones corporativas</span>
          </footer>
        </main>
      </div>
    </div>
  );
}

function HomeIcon() {
  return (
    <svg viewBox="0 0 24 24" aria-hidden="true" focusable="false">
      <path d="M3.5 10.8 12 3.7l8.5 7.1v9.5h-6v-6h-5v6h-6v-9.5Z" />
    </svg>
  );
}

function ServicesIcon() {
  return (
    <svg viewBox="0 0 24 24" aria-hidden="true" focusable="false">
      <path d="M4 4h7v7H4V4Zm9 0h7v7h-7V4ZM4 13h7v7H4v-7Zm9 0h7v7h-7v-7Z" />
    </svg>
  );
}

function OperationsIcon() {
  return (
    <svg viewBox="0 0 24 24" aria-hidden="true" focusable="false">
      <path d="M4 5h16v4H4V5Zm0 5.5h16v4H4v-4ZM4 16h16v3H4v-3Zm2-9v1h6V7H6Zm0 5.5v1h9v-1H6Z" />
    </svg>
  );
}

function EvidenceIcon() {
  return (
    <svg viewBox="0 0 24 24" aria-hidden="true" focusable="false">
      <path d="M6 3h9l4 4v14H6V3Zm8 2v3h3l-3-3ZM9 12h7v-2H9v2Zm0 4h7v-2H9v2Zm0 4h5v-2H9v2Z" />
    </svg>
  );
}

function AccessIcon() {
  return (
    <svg viewBox="0 0 24 24" aria-hidden="true" focusable="false">
      <path d="M12 3a4 4 0 0 1 4 4c0 1.3-.62 2.45-1.58 3.18A7 7 0 0 1 19 16.75V20H5v-3.25a7 7 0 0 1 4.58-6.57A4 4 0 0 1 12 3Zm0 2a2 2 0 1 0 0 4 2 2 0 0 0 0-4Zm0 7a5 5 0 0 0-5 5v1h10v-1a5 5 0 0 0-5-5Z" />
    </svg>
  );
}

function ApprovalsIcon() {
  return (
    <svg viewBox="0 0 24 24" aria-hidden="true" focusable="false">
      <path d="M5 3h10l4 4v14H5V3Zm9 2v3h3l-3-3ZM8 12h8v-2H8v2Zm0 4h5v-2H8v2Zm1.8 4.2 7.4-7.4-1.4-1.4-6 6-2.6-2.6-1.4 1.4 4 4Z" />
    </svg>
  );
}

function SupportIcon() {
  return (
    <svg viewBox="0 0 24 24" aria-hidden="true" focusable="false">
      <path d="M12 3a8 8 0 0 0-8 8v4a3 3 0 0 0 3 3h1v-7H6a6 6 0 0 1 12 0h-2v7h1.6A5.6 5.6 0 0 1 12 21h-2v-2h2a3.6 3.6 0 0 0 3.45-2.55L15.6 16H18v-5a6 6 0 0 0-6-6Z" />
    </svg>
  );
}

function ReportingIcon() {
  return (
    <svg viewBox="0 0 24 24" aria-hidden="true" focusable="false">
      <path d="M4 19h16v2H4v-2Zm2-9h3v7H6v-7Zm5-5h3v12h-3V5Zm5 8h3v4h-3v-4Z" />
    </svg>
  );
}

export function CheckIcon() {
  return (
    <svg viewBox="0 0 24 24" aria-hidden="true" focusable="false">
      <path d="m9.2 16.6-4.1-4.1 1.4-1.4 2.7 2.7 8.3-8.3 1.4 1.4-9.7 9.7Z" />
    </svg>
  );
}

export function ShieldIcon() {
  return (
    <svg viewBox="0 0 24 24" aria-hidden="true" focusable="false">
      <path d="M12 2.7 4.5 6v5.2c0 4.7 3.2 8.95 7.5 10.1 4.3-1.15 7.5-5.4 7.5-10.1V6L12 2.7Zm0 2.2 5.5 2.4v3.9c0 3.55-2.3 6.85-5.5 7.9-3.2-1.05-5.5-4.35-5.5-7.9V7.3L12 4.9Z" />
    </svg>
  );
}

export function LockIcon() {
  return (
    <svg viewBox="0 0 24 24" aria-hidden="true" focusable="false">
      <path d="M7 10V8a5 5 0 0 1 10 0v2h2v11H5V10h2Zm2 0h6V8a3 3 0 0 0-6 0v2Zm3 4a2 2 0 0 0-1 3.73V19h2v-1.27A2 2 0 0 0 12 14Z" />
    </svg>
  );
}
