import type { AdminAuditSummary } from "../application/admin-read-repository";
import { adminReadPageLimit } from "../application/get-admin-read-model";

export function AdminAuditContent({
  events,
}: {
  readonly events: readonly AdminAuditSummary[];
}) {
  return (
    <section className="admin-panel" aria-labelledby="audit-table-title">
      <div className="panel-heading">
        <div>
          <p className="section-kicker">Append-only</p>
          <h2 id="audit-table-title">Eventos recientes</h2>
        </div>
        <span className="panel-note">
          Máximo {adminReadPageLimit} registros
        </span>
      </div>

      {events.length === 0 ? (
        <div className="admin-empty-state">
          <strong>Sin evidencia</strong>
          <p>Todavía no existen eventos de auditoría en el ambiente local.</p>
        </div>
      ) : (
        <div className="admin-table-wrapper">
          <table className="admin-table">
            <thead>
              <tr>
                <th scope="col">Evento</th>
                <th scope="col">Entidad</th>
                <th scope="col">Actor</th>
                <th scope="col">Correlación</th>
                <th scope="col">Fecha</th>
              </tr>
            </thead>
            <tbody>
              {events.map((event) => (
                <tr key={event.id}>
                  <td>
                    <strong>{event.eventType}</strong>
                    <small>{event.id}</small>
                  </td>
                  <td>
                    {event.entityType}
                    <small>{event.entityId}</small>
                  </td>
                  <td>{event.actorSubject}</td>
                  <td>{event.correlationId}</td>
                  <td>{formatDate(event.createdAt)}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </section>
  );
}

function formatDate(date: Date): string {
  return new Intl.DateTimeFormat("es-MX", {
    dateStyle: "medium",
    timeStyle: "short",
    timeZone: "UTC",
  }).format(date);
}
