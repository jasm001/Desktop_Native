import { getAdminReportingSkeleton } from "@/modules/administration/application/get-admin-skeleton";
import { AdminAccessDenied } from "@/modules/administration/ui/admin-access-denied";
import { AdminShell } from "@/modules/administration/ui/admin-shell";
import { AdminSkeletonContent } from "@/modules/administration/ui/admin-skeleton-content";
import { resolveDevelopmentPortalAccess } from "@/modules/identity/application/portal-authorization";

export const dynamic = "force-dynamic";

export default function AdminReportingPage() {
  const principal = resolveDevelopmentPortalAccess("portal.reporting.read");

  if (principal === null) {
    return <AdminAccessDenied />;
  }

  return (
    <AdminShell
      activeItem="reporting"
      description="Reportes y configuracion se mantienen como resumen local no exportable hasta definir retencion y hosting."
      kicker="Reportes"
      principal={principal}
      title="Reportes y configuracion"
    >
      <AdminSkeletonContent view={getAdminReportingSkeleton()} />
    </AdminShell>
  );
}
