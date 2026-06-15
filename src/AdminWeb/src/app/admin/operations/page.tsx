import {
  getRecentAdminOperations,
} from "@/modules/administration/application/get-admin-read-model";
import { PrismaAdminReadRepository } from "@/modules/administration/infrastructure/prisma-admin-read-repository";
import { AdminAccessDenied } from "@/modules/administration/ui/admin-access-denied";
import { AdminOperationsContent } from "@/modules/administration/ui/admin-operations-content";
import { AdminShell } from "@/modules/administration/ui/admin-shell";
import { resolveDevelopmentPortalAccess } from "@/modules/identity/application/portal-authorization";
import { getPrisma } from "@/platform/db/prisma";

export const dynamic = "force-dynamic";

export default async function AdminOperationsPage() {
  const principal = resolveDevelopmentPortalAccess("portal.operations.read");

  if (principal === null) {
    return <AdminAccessDenied />;
  }

  const operations = await getRecentAdminOperations(
    new PrismaAdminReadRepository(getPrisma()),
  );

  return (
    <AdminShell
      activeItem="operations"
      description="Últimas solicitudes, trabajos y casos persistidos. La consulta está limitada y no modifica el plano de control."
      kicker="Operaciones"
      principal={principal}
      title="Actividad reciente"
    >
      <AdminOperationsContent operations={operations} />
    </AdminShell>
  );
}
