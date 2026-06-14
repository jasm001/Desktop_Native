# Contexto actual

Fecha de ultima actualizacion: 2026-06-14.

## Objetivo inmediato

Los Bloques 0 a 8 estan `completed`. El Bloque 9 queda `blocked` por la
integracion corporativa del bot existente. El trabajo local acotado del Bloque
10 termino y el bloque queda `blocked` por gates externos. No hay un bloque
principal `in_progress`; el Bloque 11 permanece `pending`.

El cierre tecnico del Bloque 8 vive en
`docs/modules/case-foundation.md` y su gobierno en `modules/TICKETING.md`.
El Bloque 9 conserva su gobierno en `modules/TEAMS.md` y la evidencia local en
`docs/modules/teams-channel-local-increment.md`. El documento propietario del
Bloque 10 es `modules/PILOT_HARDENING.md` y su threat model de trabajo vive en
`docs/threat-model/README.md`.

## Estado del repositorio

- Rama principal `main`; remoto
  `https://github.com/jasm001/Desktop_Native.git`.
- El Bloque 9 local esta publicado en `0448a42`; su integracion corporativa no
  esta completada.
- `src/AdminWeb` es el control plane Next.js modular; no es el portal del
  Bloque 11.
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
- OpenText, Teams, Entra, UEMS, Hermes/RAG y portal productivos siguen
  deshabilitados.
- El gate completo mantiene 136 pruebas .NET; Node tiene 20 pruebas
  unitarias/de contrato, 11 integraciones AdminWeb y 4 del Worker, mas el E2E
  WinUI/DeviceAgent sobre PostgreSQL efimero.

## Siguiente reanudacion

1. Mantener los Bloques 9 y 10 `blocked` y el Bloque 11 `pending`.
2. Reanudar el Bloque 10 solo cuando exista evidencia saneada de UEMS, cuenta
   restringida, identidad, Security/Sophos, owner del kill switch, logs y
   retencion, paquete/publicador y dos endpoints autorizados.
3. Definir un perfil empresarial nuevo despues de decidir si los proveedores
   seran corporativos o propios; no promover ni renombrar `local-demo`.
4. Mantener por separado el stopper de Teams; reanudar el Bloque 9 solo cuando
   exista evidencia saneada del bot corporativo.

## Limites vigentes

- No usar datos, credenciales, endpoints ni identificadores corporativos.
- No conectar OpenText real.
- No permitir que Teams, WinUI, IA o portal ejecuten comandos.
- No presentar el trabajo local del Bloque 10 como piloto corporativo ni
  declarar su gate cerrado sin revision externa y ensayo en dos endpoints.
- No adelantar el Bloque 11.
- Registrar stopper en `WORKFLOW.md` si una decision cambia seguridad,
  persistencia, contratos publicos, stack o alcance.
