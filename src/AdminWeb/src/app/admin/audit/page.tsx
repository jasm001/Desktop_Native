import {
  getRecentAdminAuditEvents,
} from "@/modules/administration/application/get-admin-read-model";
import { PrismaAdminReadRepository } from "@/modules/administration/infrastructure/prisma-admin-read-repository";
import { AdminAccessDenied } from "@/modules/administration/ui/admin-access-denied";
import { AdminAuditContent } from "@/modules/administration/ui/admin-audit-content";
import { AdminShell } from "@/modules/administration/ui/admin-shell";
import { resolveDevelopmentPortalAccess } from "@/modules/identity/application/portal-authorization";
import { getPrisma } from "@/platform/db/prisma";

export const dynamic = "force-dynamic";

export default async function AdminAuditPage() {
  const principal = resolveDevelopmentPortalAccess("portal.audit.read");

  if (principal === null) {
    return <AdminAccessDenied />;
  }

  const events = await getRecentAdminAuditEvents(
    new PrismaAdminReadRepository(getPrisma()),
  );

  return (
    <AdminShell
      activeItem="audit"
      description="Evidencia append-only con selección mínima. Los payloads permanecen fuera de la interfaz."
      kicker="Auditoría"
      principal={principal}
      title="Registro de evidencia"
    >
      <AdminAuditContent events={events} />
    </AdminShell>
  );
}
