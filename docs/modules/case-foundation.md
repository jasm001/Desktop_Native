# Fundacion de casos internos

## Estado

Primer incremento del Bloque 8 implementado localmente. El bloque queda
`in_progress`; ticketing fake y `ExternalTicket` permanecen para una unidad
posterior.

## Objetivo acotado

Introducir el caso operativo interno sin conectar OpenText ni publicar eventos
que el worker actual no pueda consumir.

Este incremento incluye:

- dominio tipado de `BotCase`;
- relacion uno a uno con `SupportRequest`;
- creacion del caso dentro de la transaccion confirmada existente;
- transicion idempotente por resultado del agente;
- politica pura de elegibilidad de cierre por falta de respuesta a las 72 horas;
- persistencia PostgreSQL mediante migracion Prisma aditiva;
- consulta HTTP v1 de solo lectura por solicitud;
- auditoria append-only y payloads acotados.

## Relacion de entidades

- `SupportRequest` representa la solicitud confirmada del usuario.
- `ExecutionJob` representa la ejecucion tecnica asociada a esa solicitud.
- `BotCase` representa el seguimiento operativo interno y existe exactamente
  una vez por solicitud confirmada.
- `ExternalTicket` representara una escalacion sintetica creada por el provider
  fake; todavia no existe en persistencia.

Los identificadores son independientes y la correlacion se conserva
explicitamente. Repetir la misma idempotency key reutiliza la solicitud y el
caso existentes.

## Estados iniciales

| Resultado tecnico | Estado del caso | Resultado del caso |
| --- | --- | --- |
| Pendiente | `open` | `pending` |
| Exito | `attended_waiting_user` | `succeeded` |
| Fallo | `escalated` | `failed` |

PostgreSQL exige combinaciones validas mediante un `CHECK`. Los timestamps de
espera y escalamiento se asignan con `clock_timestamp()` dentro de la misma
transaccion que actualiza solicitud, trabajo, auditoria y outbox de resultado.

## Politica de 72 horas

La politica es una funcion pura y no cierra casos:

- solo aplica a `attended_waiting_user`;
- requiere `waitingForUserSince`;
- antes de 72 horas devuelve no elegible;
- exactamente a las 72 horas y despues devuelve elegible.

No se agregaron scheduler, cron, recordatorios, notificaciones ni mutaciones de
cierre.

## Superficie HTTP

`GET /api/v1/requests/{requestId}/case` devuelve el caso relacionado sin crear
auditoria, outbox, tickets ni otras mutaciones.

El contrato usa enums y timestamps acotados. No acepta descripcion libre,
comandos, scripts, rutas, logs ni campos desconocidos.

## Persistencia

La migracion `20260614013000_bot_case_foundation` agrega:

- enums `BotCaseCategory`, `BotCaseStatus` y `BotCaseResult`;
- tabla `bot_cases`;
- unicidad por `request_id`;
- backfill de solicitudes existentes con estado derivado;
- indices de correlacion y elegibilidad;
- trigger de timestamps gobernados por PostgreSQL;
- restriccion de estado consistente.

## Pendiente para la segunda mitad

- evento tipado de solicitud de escalamiento;
- `ITicketingProvider`;
- provider fake determinista e idempotente;
- persistencia y consulta de `ExternalTicket`;
- procesamiento de escalamiento mediante el worker y retry existente;
- pruebas de no duplicacion del ticket fake.

No se emite todavia un evento de ticket. Agregarlo sin consumidor haria que el
worker actual agotara reintentos y marcara el outbox como fallido.

## Limites

No hay conexion OpenText, red externa, configuracion corporativa, Teams, portal
administrativo, Entra, UEMS, Hermes/RAG ni cambios del DeviceAgent.
