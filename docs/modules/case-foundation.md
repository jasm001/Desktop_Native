# Casos internos y ticketing fake

## Estado

Bloque 8 `completed` el 2026-06-14.

## Alcance implementado

- un `BotCase` por `SupportRequest` confirmada, creado en la misma transaccion;
- transiciones idempotentes de exito y fallo;
- politica pura de elegibilidad de cierre a las 72 horas;
- evento `bot-case.escalation-requested.v1` para resultados fallidos;
- interfaz de aplicacion `ITicketingProvider`;
- provider fake local, determinista y sin red;
- un `ExternalTicket` sintetico por caso escalado;
- procesamiento mediante claim, lease y retry del worker existente;
- consulta HTTP v1 del caso y su ticket nullable;
- auditoria append-only y payloads acotados.

## Relacion de entidades

- `SupportRequest` conserva la solicitud confirmada.
- `ExecutionJob` conserva la ejecucion tecnica.
- `BotCase` conserva el seguimiento operativo interno.
- `ExternalTicket` conserva la representacion sintetica del escalamiento.

Los identificadores son independientes. Solicitud, trabajo, caso y ticket
comparten correlacion explicita, pero no se sustituyen entre si.

## Transiciones

| Resultado tecnico | Estado del caso | Resultado | Ticket |
| --- | --- | --- | --- |
| Pendiente | `open` | `pending` | ninguno |
| Exito | `attended_waiting_user` | `succeeded` | ninguno |
| Fallo | `escalated` | `failed` | uno, despues del worker |

El fallo y la solicitud de escalamiento se escriben en la misma transaccion. El
worker genera una referencia `FAKE-*`, una descripcion saneada y un ID
determinista. La unicidad por `case_id` y la validacion del registro existente
evitan duplicados ante reintentos.

## Contratos

El evento de escalamiento incluye solo:

- IDs de caso, solicitud y trabajo;
- correlacion;
- categoria cerrada;
- motivo tipado;
- producto y version acotados.

No contiene comandos, scripts, rutas, logs, hostname, IP, credenciales, secretos
ni texto operativo libre.

`GET /api/v1/requests/{requestId}/case` es de solo lectura. Devuelve
`externalTicket: null` antes del procesamiento y el ticket fake despues, sin
crear auditoria, outbox ni otras mutaciones.

## Persistencia

La migracion `20260614013000_bot_case_foundation` agrega `bot_cases`, estados,
backfill y restricciones de consistencia.

La migracion `20260614090000_fake_ticketing` agrega:

- enums `TicketingProvider` y `ExternalTicketStatus`;
- tabla `external_tickets`;
- unicidad por caso y referencia externa;
- restriccion de motivos permitidos;
- indice parcial que permite una sola publicacion de escalamiento por caso.

## Politica de 72 horas

La politica solo calcula elegibilidad para `attended_waiting_user`. No agrega
scheduler, cron, recordatorios, notificaciones ni cierre automatico.

## Limites

No hay conexion OpenText, red externa, configuracion corporativa, Teams, portal
administrativo, Entra, UEMS, Hermes/RAG ni cambios del DeviceAgent.
