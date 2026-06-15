# Lecturas locales del portal administrativo

## Estado

Segunda unidad local del Bloque 11 implementada. El bloque permanece
`in_progress`; esta unidad no representa autenticacion productiva, RBAC
corporativo ni una superficie administrativa mutable.

## Alcance

La unidad agrega:

- capabilities `portal.catalog.read`, `portal.operations.read` y
  `portal.audit.read`, ademas de `portal.dashboard.read`;
- rutas server-rendered `/admin/catalog`, `/admin/operations` y
  `/admin/audit`;
- navegacion real con estado activo y enlace para saltar contenido;
- catalogo sintetico reutilizado desde el dominio existente;
- repositorio Prisma server-side para solicitudes y eventos recientes;
- limite fijo de 25 registros por consulta;
- selecciones explicitas que omiten payloads de auditoria;
- tablas semanticas, estados vacios y layout adaptable.

El unico rol local sigue siendo `DeveloperAllAccess`. No se agregan nombres,
asignaciones o scopes corporativos.

## Frontera de datos

La autorizacion se resuelve antes de crear el cliente Prisma. Las proyecciones
operativas incluyen solo referencia, producto, dispositivo, estados, ticket y
fecha. La proyeccion de auditoria incluye metadata de trazabilidad y excluye
`payload`.

No se agregan Server Actions, Route Handlers, formularios, mutaciones,
migraciones, tablas, outbox ni llamadas al DeviceAgent.

## Evidencia

- 22 pruebas unitarias de AdminWeb;
- 12 pruebas de integracion AdminWeb sobre PostgreSQL efimero;
- lectura paralela de operaciones y auditoria sin cambios en conteos;
- cuatro migraciones existentes aplicadas sin cambios;
- build Next.js con cuatro rutas `/admin/*` dinamicas;
- las cuatro rutas responden `200` en el perfil local;
- QA `1440x1000` y emulacion `390x844`;
- en movil `innerWidth`, `scrollWidth` y `bodyScrollWidth` son `390`;
- cero elementos fuera del viewport en dashboard y operaciones.

El gate de integracion completo conserva 4 pruebas del worker y el E2E
WinUI/control plane/DeviceAgent.

## Riesgos residuales

- identidad y rol siguen siendo sinteticos;
- no existen sesiones, expiracion, logout, CSRF ni OIDC/Entra;
- no hay roles, scopes, segregacion o asignaciones productivas;
- faltan Fluent UI, Testing Library y Playwright;
- no existen filtros, paginacion por cursor ni exportaciones;
- no existen mutaciones administrativas ni su auditoria;
- faltan hosting, observabilidad, retencion y revision Security;
- OpenText y Rescue reales permanecen desconectados.

Estos riesgos impiden cerrar el Bloque 11.
