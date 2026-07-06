import { AdminAccessDenied } from "@/modules/administration/ui/admin-access-denied";
import { AdminCatalogContent } from "@/modules/administration/ui/admin-catalog-content";
import { AdminShell } from "@/modules/administration/ui/admin-shell";
import { listCatalogProducts } from "@/modules/catalog/application/list-catalog-products";
import { listLocalLabCatalogEntries } from "@/modules/catalog/application/list-local-lab-catalog";
import { resolveDevelopmentPortalAccess } from "@/modules/identity/application/portal-authorization";

export const dynamic = "force-dynamic";

export default async function AdminCatalogPage() {
  const principal = resolveDevelopmentPortalAccess("portal.catalog.read");

  if (principal === null) {
    return <AdminAccessDenied />;
  }

  const labEntries = await listLocalLabCatalogEntries();

  return (
    <AdminShell
      activeItem="catalog"
      description="Catalogo sintetico y catalogo local curado para laboratorio, sin publicar paquetes ni ejecutar acciones desde el portal."
      kicker="Catalogo"
      principal={principal}
      title="Catalogo controlado"
    >
      <AdminCatalogContent
        labEntries={labEntries}
        products={listCatalogProducts(25).items}
      />
    </AdminShell>
  );
}
