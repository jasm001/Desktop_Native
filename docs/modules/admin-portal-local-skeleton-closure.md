# Cierre del esqueleto local del portal administrativo

## Estado

Cuarta unidad local del Bloque 11 implementada. Cierra el esqueleto local de
navegacion y superficies protegidas, pero no cierra el Bloque 11 completo. No
introduce integraciones, mutaciones ni gobierno productivo.

## Alcance exacto

La unidad completa el esqueleto local del portal agregando superficies
administrativas protegidas, sinteticas y de solo lectura para los modulos
objetivo que no tenian ruta propia:

- identidad y acceso, sin crear usuarios, roles, owners, grupos ni scopes
  corporativos;
- aprobaciones, con estados vacios y politica descriptiva, sin aprobar ni
  rechazar solicitudes;
- tickets y soporte remoto, mostrando solo enlaces o estados no disponibles
  sinteticos, sin conectar OpenText ni Rescue;
- reportes y configuracion, con resumen local o estado vacio, sin exportaciones
  productivas ni retencion real.

Las rutas elegidas son `/admin/access`, `/admin/approvals`, `/admin/support` y
`/admin/reporting`. Reutilizan el shell existente y mantienen `/admin`,
`/admin/catalog`, `/admin/operations` y `/admin/audit` operativas. La pagina
`/` permanece como superficie tecnica del Bloque 8.

## Limites

La unidad no puede:

- conectar Entra, Microsoft 365, Teams, OpenText, Rescue, UEMS, Sophos, PKI ni
  servicios productivos;
- convertir `DeveloperAllAccess` en autenticacion productiva;
- inventar owners, asignaciones reales, scopes corporativos, tenants,
  ambientes, retencion o despliegue;
- agregar formularios, Server Actions, Route Handlers mutantes, comandos,
  scripts, rutas operativas, argumentos ni llamadas directas al DeviceAgent;
- crear migraciones o tablas solo para representar placeholders;
- declarar completo el Bloque 11.

Si la implementacion necesita cambiar contratos publicos, persistencia,
seguridad, stack o alcance, debe registrarse un stopper en `WORKFLOW.md` antes
de continuar.

## Estrategia de autorizacion

Toda ruta nueva debe resolver identidad y capabilities en servidor antes de
renderizar contenido protegido. Los perfiles, roles, capabilities, payloads o
versiones desconocidos deben rechazarse sin filtrar datos de la ruta.

Se agregan capabilities sinteticas `portal.identity.read`,
`portal.approvals.read`, `portal.support.read` y `portal.reporting.read`. Son
locales, documentadas, denegadas por defecto y cubiertas por pruebas
permitidas/denegadas. No representan roles productivos ni asignaciones reales.

## Estrategia de pruebas

La unidad amplia la cobertura existente en lugar de reemplazarla:

- pruebas unitarias/componentes para navegacion, shell, estados vacios,
  tablas/listas locales y acceso denegado;
- pruebas de identidad/capability para rutas nuevas permitidas y denegadas;
- Playwright para rutas nuevas y regresion de las cuatro rutas existentes,
  cubriendo desktop, movil, teclado, foco, estado activo, solo lectura y
  ausencia de overflow horizontal;
- verificacion de que no aparecen formularios ni controles mutantes dentro de
  `main`;
- gate completo del repositorio, auditoria de dependencias y escaneo de
  secretos antes de proponer cierre de la unidad.

## Evidencia local

- `corepack pnpm@11.5.3 --filter @it-support-native/admin-web run test`:
  33 pruebas unitarias/componentes.
- `corepack pnpm@11.5.3 --filter @it-support-native/admin-web run lint`:
  ESLint y TypeScript estricto.
- `corepack pnpm@11.5.3 --filter @it-support-native/admin-web run test:e2e`:
  6 recorridos de acceso denegado y 18 recorridos protegidos en desktop y
  movil.
- `corepack pnpm@11.5.3 --filter @it-support-native/admin-web run build`:
  build standalone con ocho rutas `/admin/*` dinamicas.

No se modifican contratos publicos, Worker, DeviceAgent, Prisma schema ni las
cuatro migraciones historicas.

## Criterio de aceptacion

La unidad puede considerarse cerrada solo si:

- Bloques 0 a 8 permanecen `completed`;
- Bloques 9 y 10 permanecen `blocked`;
- Bloque 11 permanece como unico bloque principal `in_progress`;
- las superficies nuevas son de solo lectura, sinteticas y protegidas en
  servidor;
- la identidad invalida muestra solo acceso no disponible;
- las rutas existentes del portal siguen protegidas, operativas y cubiertas por
  pruebas;
- no hay mutaciones administrativas, secretos, PII, datos corporativos ni
  conexiones externas;
- `src/AdminWeb` conserva Next.js App Router, TypeScript estricto, Prisma y
  PostgreSQL como control plane;
- `src/Worker`, contratos IPC, DeviceAgent y migraciones historicas no se
  modifican para esta unidad;
- los riesgos externos quedan separados del esqueleto local.

## Riesgos residuales

Aunque el esqueleto local quede completo, seguiran pendientes:

- App Registration, tenant y contrato OIDC/Entra;
- owners y asignaciones reales de roles/scopes;
- matriz de segregacion de funciones;
- sesiones, expiracion, logout, CSRF, MFA y revision Security;
- hosting, secretos, observabilidad, retencion y proceso de despliegue;
- OpenText/Rescue reales y sus mecanismos aprobados;
- datos, ambientes y operacion corporativa aprobados.

Estos pendientes bloquean el cierre del Bloque 11 completo, pero no bloquean un
esqueleto local no mutante y reemplazable.
