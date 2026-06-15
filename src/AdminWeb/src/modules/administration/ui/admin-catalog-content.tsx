import type { CatalogProduct } from "@it-support-native/control-plane-contracts";

const productStatusLabels = {
  approved: "Aprobado",
  unlisted: "No listado",
  end_of_life: "Fin de vida",
  prohibited: "Prohibido",
} as const;

export function AdminCatalogContent({
  products,
}: {
  readonly products: readonly CatalogProduct[];
}) {
  return (
    <section className="admin-panel" aria-labelledby="catalog-table-title">
      <div className="panel-heading">
        <div>
          <p className="section-kicker">Datos sintéticos</p>
          <h2 id="catalog-table-title">Productos versionados</h2>
        </div>
        <span className="panel-note">{products.length} registros</span>
      </div>

      <div className="admin-table-wrapper">
        <table className="admin-table">
          <thead>
            <tr>
              <th scope="col">Producto</th>
              <th scope="col">Versión</th>
              <th scope="col">Estado</th>
              <th scope="col">Acción permitida</th>
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
                <td>{product.actionId ?? "Sin acción"}</td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </section>
  );
}
