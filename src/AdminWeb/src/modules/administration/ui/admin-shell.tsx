import type { AdminOverview } from "../application/get-admin-overview";
import type { PortalPrincipal } from "../../identity/domain/portal-principal";

interface AdminShellProps {
  readonly overview: AdminOverview;
  readonly principal: PortalPrincipal;
}

const navigation = [
  { label: "Resumen", href: "#overview", icon: HomeIcon },
  { label: "Servicios", href: "#services", icon: ServicesIcon },
  { label: "Límites de seguridad", href: "#boundaries", icon: ShieldIcon },
  { label: "Evidencia", href: "#evidence", icon: EvidenceIcon },
] as const;

const capabilityStatusLabels = {
  Available: "Disponible",
  Blocked: "Bloqueado",
} as const;

const activityStateLabels = {
  Completed: "Completado",
  "In progress": "En curso",
  Blocked: "Bloqueado",
} as const;

export function AdminShell({ overview, principal }: AdminShellProps) {
  return (
    <div className="admin-shell" lang="es">
      <a className="skip-link" href="#admin-content">
        Saltar al contenido principal
      </a>

      <aside className="admin-sidebar" aria-label="Navegación administrativa">
        <a className="admin-brand" href="#overview" aria-label="Inicio administrativo de IT Support Native">
          <span className="admin-brand__mark" aria-hidden="true">
            IT
          </span>
          <span>
            <strong>IT Support</strong>
            <small>Native Admin</small>
          </span>
        </a>

        <nav className="admin-navigation">
          {navigation.map((item, index) => {
            const Icon = item.icon;
            return (
              <a
                className={index === 0 ? "admin-nav-link is-active" : "admin-nav-link"}
                href={item.href}
                key={item.href}
                aria-current={index === 0 ? "page" : undefined}
              >
                <Icon />
                <span>{item.label}</span>
              </a>
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
            <span>Base de solo lectura</span>
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
          <section id="overview" className="admin-heading" aria-labelledby="page-title">
            <div>
              <h1 id="page-title">Centro de operaciones</h1>
              <p>
                Vista local de solo lectura para supervisar las capacidades
                habilitadas y los límites protegidos.
              </p>
            </div>
            <div className="read-only-state">
              <LockIcon />
              <span>Solo lectura</span>
            </div>
          </section>

          <section className="environment-band" aria-labelledby="environment-title">
            <div>
              <p className="section-kicker">Estado del entorno</p>
              <h2 id="environment-title">Los servicios locales están disponibles</h2>
              <p>
                El portal usa una identidad sintética de operador separada.
                Todos los proveedores corporativos permanecen desconectados.
              </p>
            </div>
            <dl className="environment-facts">
              <div>
                <dt>Perfil</dt>
                <dd>
                  <span className="availability-dot" aria-hidden="true" />
                  {overview.environment.name}
                </dd>
              </div>
              <div>
                <dt>Acceso</dt>
                <dd>{overview.environment.access}</dd>
              </div>
              <div>
                <dt>Integraciones</dt>
                <dd>{overview.environment.integrations}</dd>
              </div>
            </dl>
          </section>

          <div className="admin-grid">
            <section id="services" className="admin-panel admin-panel--wide" aria-labelledby="services-title">
              <div className="panel-heading">
                <div>
                  <p className="section-kicker">Capacidades</p>
                  <h2 id="services-title">Servicios disponibles</h2>
                </div>
                <span className="panel-note">Datos sintéticos</span>
              </div>
              <div className="capability-list" role="list">
                {overview.capabilities.map((capability) => (
                  <article className="capability-row" role="listitem" key={capability.name}>
                    <span
                      className={
                        capability.status === "Available"
                          ? "service-icon service-icon--available"
                          : "service-icon service-icon--blocked"
                      }
                      aria-hidden="true"
                    >
                      {capability.status === "Available" ? <CheckIcon /> : <LockIcon />}
                    </span>
                    <div>
                      <h3>{capability.name}</h3>
                      <p>{capability.detail}</p>
                    </div>
                    <span
                      className={
                        capability.status === "Available"
                          ? "status-label status-label--available"
                          : "status-label status-label--blocked"
                      }
                    >
                      {capabilityStatusLabels[capability.status]}
                    </span>
                  </article>
                ))}
              </div>
            </section>

            <section id="evidence" className="admin-panel" aria-labelledby="activity-title">
              <div className="panel-heading">
                <div>
                  <p className="section-kicker">Evidencia</p>
                  <h2 id="activity-title">Estado actual de bloques</h2>
                </div>
              </div>
              <ol className="activity-list">
                {overview.activity.map((item) => (
                  <li key={item.title}>
                    <span
                      className={`activity-marker activity-marker--${item.state.replaceAll(" ", "-").toLowerCase()}`}
                      aria-hidden="true"
                    />
                    <div>
                      <div className="activity-title">
                        <h3>{item.title}</h3>
                        <span>{activityStateLabels[item.state]}</span>
                      </div>
                      <p>{item.detail}</p>
                    </div>
                  </li>
                ))}
              </ol>
            </section>
          </div>

          <section id="boundaries" className="security-panel" aria-labelledby="boundaries-title">
            <div className="security-panel__intro">
              <span className="security-panel__icon" aria-hidden="true">
                <ShieldIcon />
              </span>
              <div>
                <p className="section-kicker">Límites activos</p>
                <h2 id="boundaries-title">Protegido por defecto</h2>
                <p>
                  Esta primera unidad solo expone estado. Las acciones
                  operativas permanecen fuera del portal.
                </p>
              </div>
            </div>
            <ul>
              {overview.boundaries.map((boundary) => (
                <li key={boundary}>
                  <CheckIcon />
                  <span>{boundary}</span>
                </li>
              ))}
            </ul>
          </section>

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

function ShieldIcon() {
  return (
    <svg viewBox="0 0 24 24" aria-hidden="true" focusable="false">
      <path d="M12 2.7 4.5 6v5.2c0 4.7 3.2 8.95 7.5 10.1 4.3-1.15 7.5-5.4 7.5-10.1V6L12 2.7Zm0 2.2 5.5 2.4v3.9c0 3.55-2.3 6.85-5.5 7.9-3.2-1.05-5.5-4.35-5.5-7.9V7.3L12 4.9Z" />
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

function CheckIcon() {
  return (
    <svg viewBox="0 0 24 24" aria-hidden="true" focusable="false">
      <path d="m9.2 16.6-4.1-4.1 1.4-1.4 2.7 2.7 8.3-8.3 1.4 1.4-9.7 9.7Z" />
    </svg>
  );
}

function LockIcon() {
  return (
    <svg viewBox="0 0 24 24" aria-hidden="true" focusable="false">
      <path d="M7 10V8a5 5 0 0 1 10 0v2h2v11H5V10h2Zm2 0h6V8a3 3 0 0 0-6 0v2Zm3 4a2 2 0 0 0-1 3.73V19h2v-1.27A2 2 0 0 0 12 14Z" />
    </svg>
  );
}
