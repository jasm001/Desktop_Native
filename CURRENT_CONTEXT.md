# Contexto actual

Fecha de ultima actualizacion: 2026-06-14.

## Objetivo inmediato

Los Bloques 0 a 8 estan `completed`. El Bloque 9 es el unico bloque principal
`in_progress`. Su primer incremento local fija la frontera del canal sin
Microsoft 365, sin crear otro bot y sin adelantar endurecimiento o portal.

El cierre tecnico del Bloque 8 vive en
`docs/modules/case-foundation.md` y su gobierno en `modules/TICKETING.md`.
El documento propietario es `modules/TEAMS.md` y la evidencia tecnica vive en
`docs/modules/teams-channel-local-increment.md`.

## Estado del repositorio

- Rama principal `main`; remoto
  `https://github.com/jasm001/Desktop_Native.git`.
- El Bloque 8 esta publicado y cerrado en `cf262b4`.
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
- OpenText, Teams, Entra, UEMS, Hermes/RAG y portal productivos siguen
  deshabilitados.
- El gate completo mantiene 125 pruebas .NET; Node tiene 20 pruebas
  unitarias/de contrato, 11 integraciones AdminWeb y 4 del Worker, mas el E2E
  WinUI/DeviceAgent sobre PostgreSQL efimero.

## Siguiente reanudacion

1. Mantener el Bloque 9 `in_progress` y los Bloques 10-11 `pending`.
2. Obtener owner, plataforma, repositorio, autenticacion, permisos, tenant,
   ambientes, DLP, despliegue y payloads saneados del bot existente.
3. Sustituir el adaptador recorded solo cuando exista esa evidencia.
4. Definir en el control plane la mutacion durable de revision humana antes de
   ofrecer escalamiento manual desde Teams.
5. No declarar `completed` el Bloque 9 hasta validar el bot corporativo real.

## Limites vigentes

- No usar datos, credenciales, endpoints ni identificadores corporativos.
- No conectar OpenText real.
- No permitir que Teams, WinUI, IA o portal ejecuten comandos.
- No adelantar Bloques 10 u 11.
- Registrar stopper en `WORKFLOW.md` si una decision cambia seguridad,
  persistencia, contratos publicos, stack o alcance.
