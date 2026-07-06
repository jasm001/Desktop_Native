import type { CatalogProduct } from "@it-support-native/control-plane-contracts";
import type { LocalLabCatalogEntry } from "@/modules/catalog/application/list-local-lab-catalog";

const productStatusLabels = {
  approved: "Aprobado",
  unlisted: "No listado",
  end_of_life: "Fin de vida",
  prohibited: "Prohibido",
} as const;

const artifactStatusLabels = {
  available: "Disponible",
  absent: "Ausente",
  hash_mismatch: "Hash no coincidente",
} as const;

export function AdminCatalogContent({
  labEntries = [],
  products,
}: {
  readonly labEntries?: readonly LocalLabCatalogEntry[];
  readonly products: readonly CatalogProduct[];
}) {
  return (
    <>
      <section className="admin-panel" aria-labelledby="catalog-table-title">
        <div className="panel-heading">
          <div>
            <p className="section-kicker">Catalogo sintetico</p>
            <h2 id="catalog-table-title">Productos versionados</h2>
          </div>
          <span className="panel-note">{products.length} registros</span>
        </div>

        <div className="admin-table-wrapper">
          <table className="admin-table">
            <thead>
              <tr>
                <th scope="col">Producto</th>
                <th scope="col">Version</th>
                <th scope="col">Estado</th>
                <th scope="col">Accion permitida</th>
              </tr>
            </thead>
            <tbody>
              {products.map((product) => (
                <tr key={product.id}>
                  <td>
                    <strong>{product.name}</strong>
                    <small>{product.id}</small>
                  </td>
                  <td>{product.version}</td>
                  <td>
                    <span
                      className={
                        product.status === "approved"
                          ? "status-label status-label--available"
                          : "status-label status-label--blocked"
                      }
                    >
                      {productStatusLabels[product.status]}
                    </span>
                  </td>
                  <td>{product.actionId ?? "Sin accion"}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </section>

      <section className="admin-panel" aria-labelledby="lab-catalog-table-title">
        <div className="panel-heading">
          <div>
            <p className="section-kicker">lab-real-sanitized</p>
            <h2 id="lab-catalog-table-title">Catalogo local curado</h2>
          </div>
          <span className="panel-note">{labEntries.length} registros</span>
        </div>

        <div className="catalog-boundary-grid" aria-label="Fronteras de catalogo">
          <div>
            <span>Sintetico</span>
            <strong>Decisiones demo</strong>
          </div>
          <div>
            <span>Laboratorio</span>
            <strong>Artefactos libres con SHA-256</strong>
          </div>
          <div>
            <span>Corporativo futuro</span>
            <strong>No conectado</strong>
          </div>
        </div>

        {labEntries.length === 0 ? (
          <div className="admin-empty-state">
            <strong>Catalogo de laboratorio no habilitado</strong>
            <p>Solo Development con perfil local-demo puede mostrar entradas.</p>
          </div>
        ) : (
          <div className="admin-table-wrapper">
            <table className="admin-table admin-table--lab-catalog">
              <thead>
                <tr>
                  <th scope="col">Producto</th>
                  <th scope="col">Artefacto</th>
                  <th scope="col">Licencia y origen</th>
                  <th scope="col">Hash</th>
                  <th scope="col">Estado</th>
                </tr>
              </thead>
              <tbody>
                {labEntries.map((entry) => (
                  <tr key={entry.artifactId}>
                    <td>
                      <strong>{entry.product}</strong>
                      <small>
                        {entry.version} / {entry.architecture} / {entry.scope}
                      </small>
                    </td>
                    <td>
                      <strong>{entry.artifactId}</strong>
                      <small>{entry.adapterId}</small>
                    </td>
                    <td>
                      <strong>{entry.license}</strong>
                      <small>{entry.originUrl}</small>
                    </td>
                    <td>
                      <code>{entry.sha256}</code>
                    </td>
                    <td>
                      <span
                        className={
                          entry.artifactStatus === "available" &&
                          entry.validationStatus === "valid"
                            ? "status-label status-label--available"
                            : "status-label status-label--blocked"
                        }
                      >
                        {artifactStatusLabels[entry.artifactStatus]}
                      </span>
                      <small>{entry.artifactStatusDetail}</small>
                      {entry.validationMessages.length > 0 ? (
                        <small>{entry.validationMessages.join(" ")}</small>
                      ) : (
                        <small>Manifiesto validado para laboratorio.</small>
                      )}
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}
      </section>
    </>
  );
}
