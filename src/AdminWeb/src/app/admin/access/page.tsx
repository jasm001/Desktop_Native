import { getAdminAccessSkeleton } from "@/modules/administration/application/get-admin-skeleton";
import { AdminAccessDenied } from "@/modules/administration/ui/admin-access-denied";
import { AdminShell } from "@/modules/administration/ui/admin-shell";
import { AdminSkeletonContent } from "@/modules/administration/ui/admin-skeleton-content";
import { resolveDevelopmentPortalAccess } from "@/modules/identity/application/portal-authorization";

export const dynamic = "force-dynamic";

export default function AdminAccessPage() {
  const principal = resolveDevelopmentPortalAccess("portal.identity.read");

  if (principal === null) {
    return <AdminAccessDenied />;
  }

  return (
    <AdminShell
      activeItem="access"
      description="Identidad y acceso del portal local sin usuarios, grupos, owners ni scopes corporativos."
      kicker="Acceso"
      principal={principal}
      title="Identidad y acceso"
    >
      <AdminSkeletonContent view={getAdminAccessSkeleton()} />
    </AdminShell>
  );
}
