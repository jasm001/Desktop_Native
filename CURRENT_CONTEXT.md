# Contexto actual

Fecha de ultima actualizacion: 2026-06-16.

## Objetivo inmediato

Los Bloques 0 a 8 estan `completed`. El Bloque 9 queda `blocked` por la
integracion corporativa del bot existente. El trabajo local acotado del Bloque
10 termino y el bloque queda `blocked` por gates externos. El Bloque 11,
portal administrativo web, es el unico bloque principal `in_progress`.

El cierre tecnico del Bloque 8 vive en
`docs/modules/case-foundation.md` y su gobierno en `modules/TICKETING.md`.
El Bloque 9 conserva su gobierno en `modules/TEAMS.md` y la evidencia local en
`docs/modules/teams-channel-local-increment.md`. El documento propietario del
Bloque 10 es `modules/PILOT_HARDENING.md` y su threat model de trabajo vive en
`docs/threat-model/README.md`. El documento propietario del Bloque 11 es
`modules/ADMIN_PORTAL.md`.

## Estado del repositorio

- Rama principal `main`; remoto
  `https://github.com/jasm001/Desktop_Native.git`.
- La segunda unidad local del Bloque 11 esta publicada en `17e7581`; la tercera
  unidad local de calidad del portal esta implementada localmente y pendiente de
  publicacion.
- El cierre local del Bloque 10 esta publicado en `be6c4fc`.
- El Bloque 9 local esta publicado en `0448a42`; su integracion corporativa no
  esta completada.
- `src/AdminWeb` es el control plane Next.js modular sobre el que se construye
  el Bloque 11. `/` sigue siendo la superficie tecnica del Bloque 8. Las rutas
  `/admin`, `/admin/catalog`, `/admin/operations` y `/admin/audit` usan una
  identidad sintetica separada, capabilities server-side fail-closed y lecturas
  limitadas. Testing Library/jsdom cubre componentes locales y Playwright cubre
  recorridos desktop/movil de las cuatro rutas y acceso denegado. No existen
  OIDC/Entra, RBAC productivo, Fluent UI ni mutaciones.
- Prisma/PostgreSQL tiene cuatro migraciones versionadas.
- Cada confirmacion crea una sola `SupportRequest`, `ExecutionJob` y `BotCase`.
- Exito deja el caso en `attended_waiting_user` sin ticket.
- Fallo deja el caso en `escalated` y publica
  `bot-case.escalation-requested.v1` en la misma transaccion.
- `src/Worker` valida el evento y usa `ITicketingProvider`.
- `FakeTicketingProvider` es determinista, local, sin red y sin configuracion
  corporativa.
- `external_tickets` permite un solo ticket sintetico por caso.
- La consulta HTTP del caso devuelve el ticket nullable sin efectos laterales.
- `conversation-channel.v1` define entrada/salida normalizada estricta en C# y
  TypeScript.
- `ConversationChannelService` reutiliza `ConversationService` y
  `CatalogDecisionService`; no duplica reglas en Teams.
- `RecordedTeamsConversationChannel` es local, determinista, sin red y no
  supone payloads de Microsoft.
- WinUI usa la misma aplicacion normalizada y pasa fixtures de paridad con el
  canal recorded.
- Correlacion, dispositivo e idempotency key llegan al cliente HTTP compartido;
  solicitud, estado y caso usan endpoints existentes.
- La revision humana durable no tiene endpoint actual y falla como capacidad no
  disponible despues de confirmacion, sin efectos laterales.
- La politica de 72 horas es pura; no existe scheduler ni cierre automatico.
- Auditoria append-only, outbox, leases y reintentos acotados permanecen activos.
- WinUI y DeviceAgent conservan el recorrido local del Bloque 7 y no recibieron
  nuevas capacidades privilegiadas.
- La primera unidad local del Bloque 10 agrega
  `DeviceAgent:JobExecutionEnabled=false` por defecto. El gate impide admision
  IPC, claims remotos e inicio/reanudacion de trabajos pendientes, sin ampliar
  allowlist, contratos, privilegios ni evidencia.
- El runbook local de deshabilitacion, rollback y retiro vive en
  `docs/runbooks/device-agent-disable-rollback-retirement.md`.
- La unidad local final valida al iniciar que solo existan los perfiles
  `disabled` y `local-demo`, confina `local-demo` a `Development` y rechaza
  capacidades habilitadas con perfil `disabled`.
- Los fallos de los workers IPC, trabajos y sincronizacion usan eventos fijos
  sin objetos `Exception`, payloads ni salida operativa completa.
- El inventario de cierre y los puntos de sustitucion viven en
  `docs/modules/pilot-hardening-local-closure.md`.
- La primera unidad del Bloque 11 vive en
  `docs/modules/admin-portal-foundation.md`; no modifica APIs, Prisma,
  migraciones, worker ni DeviceAgent.
- La segunda unidad vive en `docs/modules/admin-portal-read-model.md`; agrega
  navegacion real, catalogo sintetico y proyecciones Prisma de solicitudes y
  auditoria limitadas a 25 registros, sin payloads ni efectos laterales.
- La tercera unidad vive en `docs/modules/admin-portal-local-quality-gate.md`;
  agrega pruebas de componentes y recorridos Playwright sobre PostgreSQL
  efimero, sin ampliar roles, contratos, migraciones o mutaciones.
- OpenText, Teams, Entra, UEMS, Hermes/RAG y portal productivos siguen
  deshabilitados.
- El gate completo mantiene 136 pruebas .NET; Node tiene 40 pruebas
  unitarias/de contrato/componente, 12 integraciones AdminWeb y 4 del Worker,
  mas el E2E WinUI/DeviceAgent sobre PostgreSQL efimero. El portal agrega 12
  recorridos Playwright locales.

## Siguiente reanudacion

1. Mantener los Bloques 9 y 10 `blocked` y el Bloque 11 `in_progress`.
2. Mantener validadas las tres unidades locales del Bloque 11 y elegir una
   siguiente unidad pequena sin agregar integraciones o mutaciones prematuras.
3. Conservar la identidad de portal solo en entorno local; Entra, MFA,
   grupos y usuarios corporativos permanecen deshabilitados.
4. Reanudar el Bloque 10 solo cuando exista evidencia saneada de UEMS, cuenta
   restringida, identidad, Security/Sophos, owner del kill switch, logs y
   retencion, paquete/publicador y dos endpoints autorizados.
5. Definir un perfil empresarial nuevo despues de decidir si los proveedores
   seran corporativos o propios; no promover ni renombrar `local-demo`.
6. Mantener por separado el stopper de Teams; reanudar el Bloque 9 solo cuando
   exista evidencia saneada del bot corporativo.

## Limites vigentes

- No usar datos, credenciales, endpoints ni identificadores corporativos.
- No conectar OpenText real.
- No permitir que Teams, WinUI, IA o portal ejecuten comandos.
- No presentar el trabajo local del Bloque 10 como piloto corporativo ni
  declarar su gate cerrado sin revision externa y ensayo en dos endpoints.
- No convertir la identidad de desarrollo en autenticacion productiva ni
  inventar roles, scopes, owners o permisos corporativos.
- El portal no ejecuta comandos, no llama directamente al DeviceAgent y no
  conecta Entra, OpenText, Teams, Rescue o UEMS reales.
- Registrar stopper en `WORKFLOW.md` si una decision cambia seguridad,
  persistencia, contratos publicos, stack o alcance.
