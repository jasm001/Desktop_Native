import type {
  AdminLabComponentStatus,
  AdminLabReadModel,
  AdminLabStatus,
} from "../application/admin-read-repository";
import { adminReadPageLimit } from "../application/get-admin-read-model";

const statusLabels = {
  available: "Disponible",
  offline: "Apagado",
  unavailable: "No disponible",
  misconfigured: "Mal configurado",
  "validate-only": "Validate-only",
  not_checked: "No comprobado",
} as const satisfies Record<AdminLabStatus, string>;

const statusClasses = {
  available: "status-label status-label--available",
  offline: "status-label status-label--blocked",
  unavailable: "status-label status-label--blocked",
  misconfigured: "status-label status-label--blocked",
  "validate-only": "status-label status-label--available",
  not_checked: "status-label status-label--blocked",
} as const satisfies Record<AdminLabStatus, string>;

export function AdminLabContent({ lab }: { readonly lab: AdminLabReadModel }) {
  return (
    <>
      <section className="admin-panel" aria-labelledby="lab-status-title">
        <div className="panel-heading">
          <div>
            <p className="section-kicker">local-demo</p>
            <h2 id="lab-status-title">Estado del laboratorio</h2>
          </div>
          <span className="panel-note">Sin acciones operativas</span>
        </div>

        <div className="lab-status-grid" role="list">
          {lab.components.map((component) => (
            <LabStatusCard component={component} key={component.id} />
          ))}
        </div>
      </section>

      <section className="admin-panel" aria-labelledby="lab-metrics-title">
        <div className="panel-heading">
          <div>
            <p className="section-kicker">lab-real-sanitized</p>
            <h2 id="lab-metrics-title">Lecturas operativas locales</h2>
          </div>
          <span className="panel-note">
            Maximo {adminReadPageLimit} registros por lista
          </span>
        </div>

        <div className="lab-metric-grid" role="list">
          {lab.metrics.map((metric) => (
            <article className="lab-metric" role="listitem" key={metric.label}>
              <span>{metric.label}</span>
              <strong>{metric.value}</strong>
              <small>{metric.detail}</small>
              <em>{metric.source}</em>
            </article>
          ))}
        </div>
      </section>

      <div className="admin-grid">
        <section className="admin-panel" aria-labelledby="lab-outbox-title">
          <div className="panel-heading">
            <div>
              <p className="section-kicker">Outbox</p>
              <h2 id="lab-outbox-title">Eventos locales recientes</h2>
            </div>
          </div>

          {lab.recentOutboxEvents.length === 0 ? (
            <AdminEmptyState message="Todavia no existen eventos de outbox en PostgreSQL local." />
          ) : (
            <div className="admin-table-wrapper">
              <table className="admin-table">
                <thead>
                  <tr>
                    <th scope="col">Evento</th>
                    <th scope="col">Agregado</th>
                    <th scope="col">Estado</th>
                    <th scope="col">Intentos</th>
                    <th scope="col">Creado</th>
                  </tr>
                </thead>
                <tbody>
                  {lab.recentOutboxEvents.map((event) => (
                    <tr key={event.id}>
                      <td>
                        <strong>{event.eventType}</strong>
                        <small>{event.id}</small>
                      </td>
                      <td>{event.aggregateType}</td>
                      <td>{formatStatus(event.status)}</td>
                      <td>{event.attemptCount}</td>
                      <td>{formatDate(event.createdAt)}</td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          )}
        </section>

        <section className="admin-panel" aria-labelledby="lab-ticket-title">
          <div className="panel-heading">
            <div>
              <p className="section-kicker">Ticketing fake</p>
              <h2 id="lab-ticket-title">Tickets persistidos</h2>
            </div>
          </div>

          {lab.recentExternalTickets.length === 0 ? (
            <AdminEmptyState message="Todavia no existen tickets fake persistidos." />
          ) : (
            <div className="admin-table-wrapper">
              <table className="admin-table admin-table--compact">
                <thead>
                  <tr>
                    <th scope="col">Referencia</th>
                    <th scope="col">Proveedor</th>
                    <th scope="col">Motivo</th>
                    <th scope="col">Creado</th>
                  </tr>
                </thead>
                <tbody>
                  {lab.recentExternalTickets.map((ticket) => (
                    <tr key={ticket.id}>
                      <td>
                        <strong>{ticket.reference}</strong>
                        <small>{ticket.status}</small>
                      </td>
                      <td>{formatStatus(ticket.provider)}</td>
                      <td>{formatStatus(ticket.reasonCode)}</td>
                      <td>{formatDate(ticket.createdAt)}</td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          )}
        </section>
      </div>

      <section className="admin-panel" aria-labelledby="lab-boundaries-title">
        <div className="panel-heading">
          <div>
            <p className="section-kicker">Limites</p>
            <h2 id="lab-boundaries-title">Fronteras activas</h2>
          </div>
        </div>
        <ul className="boundary-list">
          {lab.boundaries.map((boundary) => (
            <li key={boundary}>{boundary}</li>
          ))}
        </ul>
      </section>
    </>
  );
}

function LabStatusCard({
  component,
}: {
  readonly component: AdminLabComponentStatus;
}) {
  return (
    <article className="lab-status-card" role="listitem">
      <div>
        <h3>{component.name}</h3>
        <p>{component.detail}</p>
      </div>
      <span className={statusClasses[component.status]}>
        {statusLabels[component.status]}
      </span>
      <small>
        {component.source}
        {component.lastCheckedAt === null
          ? ""
          : ` - ${formatDate(component.lastCheckedAt)}`}
      </small>
    </article>
  );
}

function AdminEmptyState({ message }: { readonly message: string }) {
  return (
    <div className="admin-empty-state">
      <strong>Sin actividad local</strong>
      <p>{message}</p>
    </div>
  );
}

function formatStatus(status: string): string {
  return status.replaceAll("_", " ").toLocaleLowerCase("es-MX");
}

function formatDate(date: Date): string {
  return new Intl.DateTimeFormat("es-MX", {
    dateStyle: "medium",
    timeStyle: "short",
    timeZone: "UTC",
  }).format(date);
}
