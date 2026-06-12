# DeviceAgent simulado e IPC

## Responsabilidad

El Bloque 4 demuestra que un proceso separado puede recibir mensajes locales
tipados, rechazarlos por defecto, conservar estado durable y reportar un
resultado sin ejecutar acciones sobre el dispositivo.

## Protocolo

Cada frame usa:

1. longitud little-endian de 4 bytes;
2. JSON UTF-8 de hasta 64 KiB.

El envelope de solicitud contiene `version`, `messageType`, `correlationId` y un
`payload` tipado. La respuesta conserva la correlacion y devuelve un snapshot de
trabajo o un `AgentErrorCode`.

Errores tipados:

- `InvalidMessage`;
- `UnsupportedVersion`;
- `UnknownMessage`;
- `UnauthorizedAction`;
- `IdempotencyConflict`;
- `JobNotFound`;
- `JobNotCancellable`;
- `InternalError`.

## Maquina de estados

```text
Queued -> Running -> Succeeded
   |         |
   +---------+-> Cancelled
```

`Failed` forma parte del contrato para fallos futuros, pero la simulacion actual
no invoca ningun componente que pueda producir un fallo operativo real.

El progreso avanza en pasos deterministas. La evidencia usa codigos y textos
fijos del agente; no copia payloads, salida de procesos ni valores enviados por
el usuario.

## Persistencia

`SqliteAgentJobStore` crea una tabla local `agent_jobs` y persiste el snapshot
completo dentro de una transaccion. El store usa conexiones cortas sin pooling
para permitir cierre limpio, pruebas aisladas y recuperacion tras reinicio.

La persistencia es una cola tecnica local. PostgreSQL, outbox y auditoria central
siguen reservados para el Bloque 7.

## Named Pipe

`NamedPipeAgentWorker` atiende una solicitud por conexion en el pipe de
desarrollo `ITSupportNative.DeviceAgent.dev.v1`.

El servidor usa:

- modo byte con framing propio;
- I/O asincrono y `CancellationToken`;
- una instancia concurrente en esta primera version;
- `PipeOptions.CurrentUserOnly`;
- respuestas saneadas ante frames invalidos.

## Pruebas

`tests/Contract` verifica serializacion, versiones, errores tipados y ausencia de
campos de ejecucion libre.

`tests/Integration` verifica:

- autorizacion exacta por accion, target y version;
- rechazo de tipos y versiones desconocidas;
- idempotencia y conflictos;
- progreso y evidencia saneada;
- cancelacion segura;
- recuperacion SQLite tras recrear el servicio;
- intercambio real por Named Pipe con ACL de usuario actual;
- ausencia de dependencias del nucleo hacia Desktop o WinUI.

La prueba de Named Pipe debe ejecutarse fuera del sandbox de Codex porque el
sandbox bloquea la conexion local aun cuando servidor y cliente comparten
usuario.
