import Link from "next/link";

export function AdminAccessDenied() {
  return (
    <main className="access-denied" lang="es">
      <section className="access-denied__panel" aria-labelledby="access-title">
        <div className="access-denied__mark" aria-hidden="true">
          <svg viewBox="0 0 24 24" focusable="false">
            <path d="M12 2.75 4.5 6v5.2c0 4.7 3.2 8.95 7.5 10.05 4.3-1.1 7.5-5.35 7.5-10.05V6L12 2.75Zm0 2.18 5.5 2.38v3.89c0 3.57-2.26 6.9-5.5 7.93-3.24-1.03-5.5-4.36-5.5-7.93V7.31L12 4.93Zm-1 4.07v4h2V9h-2Zm0 5.5v2h2v-2h-2Z" />
          </svg>
        </div>
        <p className="access-denied__brand">IT Support Native</p>
        <h1 id="access-title">Acceso administrativo no disponible</h1>
        <p>
          Este portal local falla de forma cerrada si no están configurados el
          ambiente exacto de desarrollo, la identidad explícita del portal y
          un rol conocido.
        </p>
        <Link href="/" className="text-link">
          Volver al estado del plano de control
        </Link>
      </section>
    </main>
  );
}
