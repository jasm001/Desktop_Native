import { getAdminSupportSkeleton } from "@/modules/administration/application/get-admin-skeleton";
import { AdminAccessDenied } from "@/modules/administration/ui/admin-access-denied";
import { AdminShell } from "@/modules/administration/ui/admin-shell";
import { AdminSkeletonContent } from "@/modules/administration/ui/admin-skeleton-content";
import { resolveDevelopmentPortalAccess } from "@/modules/identity/application/portal-authorization";

export const dynamic = "force-dynamic";

export default function AdminSupportPage() {
  const principal = resolveDevelopmentPortalAccess("portal.support.read");

  if (principal === null) {
    return <AdminAccessDenied />;
  }

  return (
    <AdminShell
      activeItem="support"
      description="Tickets y soporte remoto permanecen representados por adaptadores fake o estados no disponibles."
      kicker="Soporte"
      principal={principal}
      title="Tickets y soporte remoto"
    >
      <AdminSkeletonContent view={getAdminSupportSkeleton()} />
    </AdminShell>
  );
}
