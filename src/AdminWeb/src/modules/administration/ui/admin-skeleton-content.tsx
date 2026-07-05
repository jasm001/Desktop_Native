import type { AdminSkeletonView } from "../application/get-admin-skeleton";

const statusLabels = {
  Available: "Disponible",
  Deferred: "Diferido",
  Blocked: "Bloqueado",
} as const;

export function AdminSkeletonContent({
  view,
}: {
  readonly view: AdminSkeletonView;
}) {
  return (
    <div className="admin-grid">
      <section className="admin-panel" aria-labelledby="skeleton-title">
        <div className="panel-heading">
          <div>
            <p className="section-kicker">Esqueleto local</p>
            <h2 id="skeleton-title">Superficie protegida</h2>
          </div>
          <span className="panel-note">{view.source}</span>
        </div>

        <div className="capability-list" role="list">
          {view.sections.map((section) => (
            <article
              className="capability-row"
              role="listitem"
              key={section.title}
            >
              <span
                className={
                  section.status === "Available"
                    ? "service-icon service-icon--available"
                    : "service-icon service-icon--blocked"
                }
                aria-hidden="true"
              >
                <span className="skeleton-status-dot" />
              </span>
              <div>
                <h3>{section.title}</h3>
                <p>{section.detail}</p>
              </div>
              <span
                className={
                  section.status === "Available"
                    ? "status-label status-label--available"
                    : "status-label status-label--blocked"
                }
              >
                {statusLabels[section.status]}
              </span>
            </article>
          ))}
        </div>
      </section>

      <section className="admin-panel" aria-labelledby="skeleton-boundaries-title">
        <div className="panel-heading">
          <div>
            <p className="section-kicker">Limites</p>
            <h2 id="skeleton-boundaries-title">Sin efectos laterales</h2>
          </div>
        </div>
        <div className="admin-empty-state">
          <strong>Sin acciones configuradas</strong>
          <p>
            Esta ruta conserva solo lectura y no expone controles operativos.
          </p>
        </div>
        <ul className="boundary-list">
          {view.boundaries.map((boundary) => (
            <li key={boundary}>{boundary}</li>
          ))}
        </ul>
      </section>
    </div>
  );
}
