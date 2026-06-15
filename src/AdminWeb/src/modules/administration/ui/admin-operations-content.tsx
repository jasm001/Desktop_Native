import type { AdminOperationSummary } from "../application/admin-read-repository";
import { adminReadPageLimit } from "../application/get-admin-read-model";

export function AdminOperationsContent({
  operations,
}: {
  readonly operations: readonly AdminOperationSummary[];
}) {
  return (
    <section className="admin-panel" aria-labelledby="operations-table-title">
      <div className="panel-heading">
        <div>
          <p className="section-kicker">PostgreSQL local</p>
          <h2 id="operations-table-title">Solicitudes recientes</h2>
        </div>
        <span className="panel-note">
          Máximo {adminReadPageLimit} registros
        </span>
      </div>

      {operations.length === 0 ? (
        <AdminEmptyState message="Todavía no existen solicitudes persistidas en el ambiente local." />
      ) : (
        <div className="admin-table-wrapper">
          <table className="admin-table">
            <thead>
              <tr>
                <th scope="col">Solicitud</th>
                <th scope="col">Producto</th>
                <th scope="col">Dispositivo</th>
                <th scope="col">Estados</th>
                <th scope="col">Ticket</th>
                <th scope="col">Creada</th>
              </tr>
            </thead>
            <tbody>
              {operations.map((operation) => (
                <tr key={operation.id}>
                  <td>
                    <strong>{operation.reference}</strong>
                    <small>{operation.id}</small>
                  </td>
                  <td>{operation.productName}</td>
                  <td>{operation.deviceName}</td>
                  <td>
                    <span className="table-status">
                      Solicitud: {formatStatus(operation.requestStatus)}
                    </span>
                    <span className="table-status">
                      Trabajo: {formatStatus(operation.jobStatus)}
                    </span>
                    <span className="table-status">
                      Caso: {formatStatus(operation.caseStatus)}
                    </span>
                  </td>
                  <td>{operation.ticketReference ?? "Sin ticket"}</td>
                  <td>{formatDate(operation.createdAt)}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </section>
  );
}

function AdminEmptyState({ message }: { readonly message: string }) {
  return (
    <div className="admin-empty-state">
      <strong>Sin actividad</strong>
      <p>{message}</p>
    </div>
  );
}

function formatStatus(status: string | null): string {
  return status?.replaceAll("_", " ").toLocaleLowerCase("es-MX") ?? "no creado";
}

function formatDate(date: Date): string {
  return new Intl.DateTimeFormat("es-MX", {
    dateStyle: "medium",
    timeStyle: "short",
    timeZone: "UTC",
  }).format(date);
}
