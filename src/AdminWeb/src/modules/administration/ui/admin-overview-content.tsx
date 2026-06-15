import type { AdminOverview } from "../application/get-admin-overview";
import { CheckIcon, LockIcon, ShieldIcon } from "./admin-shell";

const capabilityStatusLabels = {
  Available: "Disponible",
  Blocked: "Bloqueado",
} as const;

const activityStateLabels = {
  Completed: "Completado",
  "In progress": "En curso",
  Blocked: "Bloqueado",
} as const;

export function AdminOverviewContent({
  overview,
}: {
  readonly overview: AdminOverview;
}) {
  return (
    <>
      <section className="environment-band" aria-labelledby="environment-title">
        <div>
          <p className="section-kicker">Estado del entorno</p>
          <h2 id="environment-title">Los servicios locales están disponibles</h2>
          <p>
            El portal usa una identidad sintética de operador separada. Todos
            los proveedores corporativos permanecen desconectados.
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
        <section
          className="admin-panel admin-panel--wide"
          aria-labelledby="services-title"
        >
          <div className="panel-heading">
            <div>
              <p className="section-kicker">Capacidades</p>
              <h2 id="services-title">Servicios disponibles</h2>
            </div>
            <span className="panel-note">Datos sintéticos</span>
          </div>
          <div className="capability-list" role="list">
            {overview.capabilities.map((capability) => (
              <article
                className="capability-row"
                role="listitem"
                key={capability.name}
              >
                <span
                  className={
                    capability.status === "Available"
                      ? "service-icon service-icon--available"
                      : "service-icon service-icon--blocked"
                  }
                  aria-hidden="true"
                >
                  {capability.status === "Available" ? (
                    <CheckIcon />
                  ) : (
                    <LockIcon />
                  )}
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

        <section className="admin-panel" aria-labelledby="activity-title">
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

      <section className="security-panel" aria-labelledby="boundaries-title">
        <div className="security-panel__intro">
          <span className="security-panel__icon" aria-hidden="true">
            <ShieldIcon />
          </span>
          <div>
            <p className="section-kicker">Límites activos</p>
            <h2 id="boundaries-title">Protegido por defecto</h2>
            <p>
              Esta unidad expone consultas acotadas. Las acciones operativas y
              administrativas permanecen fuera del portal.
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
    </>
  );
}
