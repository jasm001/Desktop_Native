import { AdminAccessDenied } from "@/modules/administration/ui/admin-access-denied";
import { AdminCatalogContent } from "@/modules/administration/ui/admin-catalog-content";
import { AdminShell } from "@/modules/administration/ui/admin-shell";
import { listCatalogProducts } from "@/modules/catalog/application/list-catalog-products";
import { resolveDevelopmentPortalAccess } from "@/modules/identity/application/portal-authorization";

export const dynamic = "force-dynamic";

export default function AdminCatalogPage() {
  const principal = resolveDevelopmentPortalAccess("portal.catalog.read");

  if (principal === null) {
    return <AdminAccessDenied />;
  }

  return (
    <AdminShell
      activeItem="catalog"
      description="Productos sintéticos disponibles para validar decisiones de catálogo sin publicar paquetes ni ejecutar acciones."
      kicker="Catálogo"
      principal={principal}
      title="Catálogo controlado"
    >
      <AdminCatalogContent products={listCatalogProducts(25).items} />
    </AdminShell>
  );
}
