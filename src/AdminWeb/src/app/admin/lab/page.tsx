import { getAdminLabReadModel } from "@/modules/administration/application/get-admin-lab-read-model";
import { PrismaAdminReadRepository } from "@/modules/administration/infrastructure/prisma-admin-read-repository";
import { AdminAccessDenied } from "@/modules/administration/ui/admin-access-denied";
import { AdminLabContent } from "@/modules/administration/ui/admin-lab-content";
import { AdminShell } from "@/modules/administration/ui/admin-shell";
import { resolveDevelopmentPortalAccess } from "@/modules/identity/application/portal-authorization";
import { getPrisma } from "@/platform/db/prisma";

export const dynamic = "force-dynamic";

export default async function AdminLabPage() {
  const principal = resolveDevelopmentPortalAccess("portal.lab.read");

  if (principal === null) {
    return <AdminAccessDenied />;
  }

  const lab = await getAdminLabReadModel(
    new PrismaAdminReadRepository(getPrisma()),
  );

  return (
    <AdminShell
      activeItem="lab"
      description="Estado local y lecturas reales de laboratorio saneadas. Esta vista no inicia servicios, no administra VM y no conecta proveedores corporativos."
      kicker="Laboratorio"
      principal={principal}
      title="Laboratorio local"
    >
      <AdminLabContent lab={lab} />
    </AdminShell>
  );
}
