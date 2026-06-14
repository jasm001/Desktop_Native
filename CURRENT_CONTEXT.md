# Contexto actual

Fecha de ultima actualizacion: 2026-06-14.

## Objetivo inmediato

Los Bloques 0 a 8 estan `completed`. El Bloque 9 queda `blocked` por la
integracion corporativa del bot existente. El Bloque 10 es el unico bloque
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
- OpenText, Teams, Entra, UEMS, Hermes/RAG y portal productivos siguen
  deshabilitados.
- El gate completo mantiene 125 pruebas .NET; Node tiene 20 pruebas
  unitarias/de contrato, 11 integraciones AdminWeb y 4 del Worker, mas el E2E
  WinUI/DeviceAgent sobre PostgreSQL efimero.

## Siguiente reanudacion

1. Mantener el Bloque 9 `blocked`, el Bloque 10 `in_progress` y el Bloque 11
   `pending`.
2. Auditar el hardening real del repositorio contra `core/SECURITY.md` y el
   threat model antes de elegir una primera unidad tecnica.
3. Implementar una unidad local acotada con fakes o configuracion de desarrollo,
   sin UEMS, Entra, Sophos, PKI ni endpoints corporativos.
4. Preparar evidencia y runbooks de despliegue, deshabilitacion y retiro para
   una futura prueba en dos endpoints.
5. Mantener por separado el stopper de Teams; reanudar el Bloque 9 solo cuando
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
