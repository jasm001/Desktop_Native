import { getAdminApprovalsSkeleton } from "@/modules/administration/application/get-admin-skeleton";
import { AdminAccessDenied } from "@/modules/administration/ui/admin-access-denied";
import { AdminShell } from "@/modules/administration/ui/admin-shell";
import { AdminSkeletonContent } from "@/modules/administration/ui/admin-skeleton-content";
import { resolveDevelopmentPortalAccess } from "@/modules/identity/application/portal-authorization";

export const dynamic = "force-dynamic";

export default function AdminApprovalsPage() {
  const principal = resolveDevelopmentPortalAccess("portal.approvals.read");

  if (principal === null) {
    return <AdminAccessDenied />;
  }

  return (
    <AdminShell
      activeItem="approvals"
      description="Colas objetivo de seguridad, licencias y compras representadas como estados vacios de solo lectura."
      kicker="Aprobaciones"
      principal={principal}
      title="Aprobaciones pendientes"
    >
      <AdminSkeletonContent view={getAdminApprovalsSkeleton()} />
    </AdminShell>
  );
}
