# Calidad local del portal administrativo

## Estado

Tercera unidad local del Bloque 11 implementada. El bloque permanece
`in_progress`; esta unidad no representa portal productivo, OIDC/Entra, RBAC
real, sesiones ni mutaciones administrativas.

## Alcance exacto

La unidad cierra deuda local no bloqueante de calidad sobre la superficie ya
existente:

- pruebas de componentes con Vitest, Testing Library y jsdom para shell,
  navegacion, acceso denegado, tablas y estados vacios;
- recorridos Playwright para `/admin`, `/admin/catalog`, `/admin/operations`,
  `/admin/audit` y acceso denegado;
- ejecucion Playwright en Edge/Chromium con viewport desktop `1440x1000` y
  movil `390x844`;
- verificacion de navegacion por teclado, skip link, `aria-current`, lectura
  solo lectura, ausencia de formularios/botones mutantes y ausencia de overflow
  horizontal de pagina;
- harness e2e con PostgreSQL efimero, `prisma migrate deploy`, seed sintetico,
  servidor Next local y limpieza de base al finalizar;
- atributo `data-scroll-behavior="smooth"` en el layout para satisfacer la
  advertencia de Next.js asociada al `scroll-behavior` existente.

No se agregan rutas, Server Actions, Route Handlers, migraciones, tablas,
mutaciones, roles productivos, owners, scopes corporativos, secretos ni
integraciones externas.

## Estrategia de pruebas

Testing Library se adopta solo para componentes renderizables sin navegador:

- `AdminShell` conserva landmark `main`, skip link, identidad sintetica,
  navegacion semantica y estado activo;
- `AdminCatalogContent` conserva tabla semantica y accion nullable;
- `AdminOperationsContent` y `AdminAuditContent` cubren filas y estados vacios;
- `AdminAccessDenied` no filtra identidad ni contenido protegido.

Playwright se adopta para el comportamiento que requiere navegador real:

- perfil invalido: `/admin` muestra solo acceso no disponible;
- perfil valido: las cuatro rutas renderizan como protegidas y de solo lectura;
- teclado: primer `Tab` llega al skip link y `Enter` sobre navegacion cambia de
  ruta;
- responsive: desktop y movil no producen overflow horizontal de pagina;
- consola: errores y advertencias relevantes deben estar vacios.

El harness levanta servidores separados para el perfil invalido y valido. Esto
evita simular cambios de identidad por query string o cabeceras de prueba y
mantiene la autorizacion server-side fail-closed.

## Roles y capabilities

La matriz local no cambia:

| Rol local | Capabilities |
| --- | --- |
| `DeveloperAllAccess` | `portal.dashboard.read`, `portal.catalog.read`, `portal.operations.read`, `portal.audit.read` |

Los roles y capabilities desconocidos siguen rechazados por las pruebas
unitarias existentes. Playwright valida el caso de identidad invalida sin
renderizar contenido protegido. No se crean roles productivos, owners,
asignaciones ni scopes corporativos.

## Fluent UI React

Fluent UI React permanece diferido en esta unidad.

Motivo:

- el objetivo inmediato era cerrar calidad verificable sobre rutas existentes;
- el shell actual ya tiene semantica, foco visible y layout adaptable;
- introducir Fluent UI ahora obligaria a reescribir estructura visual antes de
  tener sesiones, mutaciones, formularios y reglas de diseno productivas;
- la dependencia aumentaria superficie y deuda sin mejorar la cobertura local
  requerida por esta unidad.

Fluent UI sigue siendo el stack objetivo aprobado para el portal. Su adopcion
debe hacerse en una unidad posterior con criterio visual, componentes
reutilizables y pruebas de regresion, no como cambio cosmetico para marcar una
casilla.

## Evidencia

Validaciones ejecutadas para esta unidad:

- `corepack pnpm@11.5.3 --filter @it-support-native/admin-web run test`:
  27 pruebas unitarias/componentes;
- `corepack pnpm@11.5.3 --filter @it-support-native/admin-web run lint`:
  ESLint y TypeScript estricto;
- `corepack pnpm@11.5.3 --filter @it-support-native/admin-web run test:e2e`:
  2 pruebas de acceso denegado y 10 recorridos Playwright de portal sobre
  PostgreSQL efimero.

Los gates completos del repositorio se registran en `WORKFLOW.md`.

## Riesgos residuales

- no existe OIDC/Entra, sesion, expiracion, logout, CSRF ni MFA;
- `DeveloperAllAccess` sigue siendo sintetico y exclusivo de desarrollo;
- no existen roles/scopes productivos, segregacion, owners ni asignaciones
  reales;
- no hay mutaciones administrativas ni auditoria de mutaciones;
- Fluent UI React, observabilidad productiva, hosting, retencion y revision
  Security siguen pendientes;
- OpenText, Rescue, Teams, UEMS y Entra reales permanecen desconectados.

Estos riesgos impiden cerrar el Bloque 11, pero no bloquean esta unidad local.
