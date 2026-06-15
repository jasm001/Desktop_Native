import { getAdminOverview } from "@/modules/administration/application/get-admin-overview";
import { AdminAccessDenied } from "@/modules/administration/ui/admin-access-denied";
import { AdminOverviewContent } from "@/modules/administration/ui/admin-overview-content";
import { AdminShell } from "@/modules/administration/ui/admin-shell";
import { resolveDevelopmentPortalAccess } from "@/modules/identity/application/portal-authorization";

export const dynamic = "force-dynamic";

export default function AdminPage() {
  const principal = resolveDevelopmentPortalAccess("portal.dashboard.read");

  if (principal === null) {
    return <AdminAccessDenied />;
  }

  return (
    <AdminShell
      activeItem="overview"
      description="Vista local para supervisar capacidades habilitadas, evidencia y límites protegidos."
      kicker="Resumen"
      principal={principal}
      title="Centro de operaciones"
    >
      <AdminOverviewContent overview={getAdminOverview()} />
    </AdminShell>
  );
}
