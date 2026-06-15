import { getAdminOverview } from "@/modules/administration/application/get-admin-overview";
import { AdminAccessDenied } from "@/modules/administration/ui/admin-access-denied";
import { AdminShell } from "@/modules/administration/ui/admin-shell";
import { PortalAccessError } from "@/modules/identity/application/portal-access-error";
import { requireDevelopmentPortalAccess } from "@/modules/identity/application/portal-authorization";

export const dynamic = "force-dynamic";

export default function AdminPage() {
  const access = resolveAdminPageAccess();

  if (!access.allowed) {
    return <AdminAccessDenied />;
  }

  return <AdminShell overview={access.overview} principal={access.principal} />;
}

function resolveAdminPageAccess() {
  try {
    const principal = requireDevelopmentPortalAccess("portal.dashboard.read");
    const overview = getAdminOverview();

    return { allowed: true, overview, principal } as const;
  } catch (error) {
    if (error instanceof PortalAccessError) {
      return { allowed: false } as const;
    }

    throw error;
  }
}
