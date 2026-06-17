# Cierre del esqueleto local del portal administrativo

## Estado

Unidad local propuesta para continuar el Bloque 11 despues de las tres unidades
publicadas. No cierra el Bloque 11 completo; solo puede cerrar el esqueleto
local de navegacion y superficies protegidas si no introduce integraciones,
mutaciones ni gobierno productivo.

## Alcance exacto

La unidad puede completar el esqueleto local del portal agregando superficies
administrativas protegidas, sinteticas y de solo lectura para los modulos
objetivo que aun no tienen ruta propia:

- identidad y acceso, sin crear usuarios, roles, owners, grupos ni scopes
  corporativos;
- aprobaciones, con estados vacios y politica descriptiva, sin aprobar ni
  rechazar solicitudes;
- tickets y soporte remoto, mostrando solo enlaces o estados no disponibles
  sinteticos, sin conectar OpenText ni Rescue;
- reportes y configuracion, con resumen local o estado vacio, sin exportaciones
  productivas ni retencion real.

Las rutas exactas deben elegirse dentro del namespace `/admin/*`, reutilizando
el shell existente y manteniendo `/admin`, `/admin/catalog`,
`/admin/operations` y `/admin/audit` operativas. La pagina `/` permanece como
superficie tecnica del Bloque 8.

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

Si se agregan capabilities sinteticas para separar vistas del esqueleto, deben
permanecer locales, documentadas, denegadas por defecto y cubiertas por pruebas
permitidas/denegadas. No representan roles productivos ni asignaciones reales.

## Estrategia de pruebas

La unidad debe ampliar la cobertura existente en lugar de reemplazarla:

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
