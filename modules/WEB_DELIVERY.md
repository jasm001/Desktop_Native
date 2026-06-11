# Delivery del portal web

## Objetivo

Portal administrativo y plano de control con Next.js. Mantiene experiencia Fluent
inspirada en el mockup, sin simular Windows ni copiar el frontend Vite existente.

## Stack

- Next.js App Router y TypeScript estricto.
- React Server Components por defecto.
- Fluent UI React.
- Prisma ORM.
- PostgreSQL.
- Zod o esquema equivalente en limites.
- Vitest, Testing Library y Playwright.
- OpenTelemetry.

## Estructura

```text
src/AdminWeb/
  app/
    (auth)/
    (portal)/
    api/
  modules/
    identity/
    devices/
    catalog/
    approvals/
    jobs/
    bot-cases/
    opentext/
    remote-support/
    audit/
  platform/
    auth/
    db/
    observability/
    security/
  components/
    fluent/
    shared/
  prisma/
  tests/
```

Cada modulo contiene dominio, casos de uso, infraestructura y UI propia cuando
aplique. `app/` compone rutas; no concentra reglas de negocio.

## Perfiles de despliegue

### Desarrollo financiado por el equipo

- Vercel para Next.js.
- Supabase PostgreSQL.
- Datos sinteticos.
- OpenText, agentes, Rescue y correos reales desactivados.
- Adaptadores fake/recorded fixtures.

### Piloto corporativo

- Mismo build Next.js con `output: "standalone"`.
- Azure App Service Linux o Azure Container Apps.
- Azure Database for PostgreSQL.
- Key Vault y almacenamiento privado.
- Worker Node durable separado.
- Integraciones y endpoints de agentes habilitados.

Prisma funciona con ambos PostgreSQL. Mantener URL pooled para runtime y URL
directa para migraciones cuando el proveedor lo requiera. Una sola herramienta
es propietaria de migraciones: Prisma Migrate.

## Vercel no es produccion por defecto

Vercel permite iterar UI, rendimiento y flujos rapidamente. Antes de usar datos
reales se deben aprobar:

- residencia y procesamiento de datos;
- conectividad a OpenText/Azure;
- identidad corporativa;
- logs y secretos;
- costos y soporte;
- salida/migracion.

No colocar credenciales corporativas en un proyecto personal.

## Pipeline GitHub

Pull request:

1. install reproducible;
2. format/lint/typecheck;
3. unit tests;
4. Prisma schema validation;
5. build Next.js;
6. CodeQL o Semgrep;
7. secret/SCA/IaC scans;
8. preview Vercel solo con configuracion fake.

Main/release:

1. integration tests con PostgreSQL efimero;
2. Playwright;
3. migracion validada;
4. build standalone;
5. SBOM/Trivy;
6. deploy a development/test;
7. ZAP baseline;
8. aprobacion manual para piloto.

## Migraciones

- Nunca ejecutar `db push` en produccion.
- Revisar SQL generado.
- Aplicar `prisma migrate deploy` desde job controlado.
- Backup y plan de rollback antes de migraciones destructivas.
- Cambios expand/contract cuando portal y worker desplieguen por separado.

## Iframe OpenText

Crear un spike temprano:

1. comprobar `Content-Security-Policy frame-ancestors`;
2. comprobar `X-Frame-Options`;
3. validar SSO/cookies en iframe;
4. validar expiracion y navegacion;
5. confirmar aprobacion de seguridad.

Si falla cualquiera, usar enlace profundo en nueva pestana. El iframe es mejora
de experiencia, no dependencia del MVP.
