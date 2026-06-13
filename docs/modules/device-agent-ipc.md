# DeviceAgent e IPC local

## Responsabilidad

Los Bloques 4 y 5 demostraron que un proceso separado puede recibir mensajes
locales tipados, rechazarlos por defecto, conservar trabajos durables y devolver
diagnosticos efimeros. El Bloque 6 mantuvo el mismo contrato IPC v1 y agrego dos
acciones reales cerradas de 7-Zip, autorizadas solo en `local-demo` y validadas
en una VM Windows 11 desechable.

## Protocolo

Cada frame usa:

1. longitud little-endian de 4 bytes;
2. JSON UTF-8 de hasta 64 KiB.

El envelope de solicitud contiene `version`, `messageType`, `correlationId` y un
`payload` tipado. La respuesta conserva la correlacion y devuelve un snapshot de
trabajo o un `AgentErrorCode`.

Mensajes allowlisted en v1:

- `agent.job.start`;
- `agent.job.get`;
- `agent.job.cancel`;
- `agent.diagnostics.get`.

`agent.diagnostics.get` solo acepta una referencia tipada
`actionId`/`targetId`/`targetVersion`. No contiene comando, argumentos, ruta,
script ni opciones libres. La respuesta usa `agent.diagnostics.snapshot`.

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

`Failed` ahora se usa para resultados controlados del adaptador real, por
ejemplo perfil deshabilitado, mirror ausente, integridad incorrecta, timeout,
exit code rechazado o verificacion fallida. La accion sintetica conserva su
flujo determinista anterior.

El progreso avanza en pasos deterministas. La evidencia usa codigos y textos
fijos del agente; no copia payloads, salida de procesos ni valores enviados por
el usuario.

## Persistencia

`SqliteAgentJobStore` crea una tabla local `agent_jobs` y persiste el snapshot
completo dentro de una transaccion. El store usa conexiones cortas sin pooling
para permitir cierre limpio, pruebas aisladas y recuperacion tras reinicio.

La persistencia SQLite sigue siendo una cola tecnica local del agente. El
Bloque 7 agrego PostgreSQL, outbox y auditoria central sin reemplazar este store
ni abrir una conexion entrante desde el backend.

El snapshot diagnostico nunca se persiste en esta base.

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
campos de ejecucion libre en trabajos y diagnosticos.

`tests/Integration` verifica:

- autorizacion exacta por accion, target y version;
- rechazo de tipos y versiones desconocidas;
- idempotencia y conflictos;
- progreso y evidencia saneada;
- cancelacion segura;
- recuperacion SQLite tras recrear el servicio;
- seleccion exacta del adaptador real;
- rechazo deny-by-default de accion, target y version;
- cancelacion en cola y rechazo tipado durante MSI;
- finalizacion real simulada mediante dobles sin ejecutar instaladores;
- intercambio real por Named Pipe con ACL de usuario actual;
- ausencia de dependencias del nucleo hacia Desktop o WinUI.
- snapshot diagnostico por dispatcher y Named Pipe;
- fallos parciales saneados, cancelacion y limites;
- colectores reales de Windows con datos agregados;
- ausencia de dependencia del diagnostico hacia la persistencia de trabajos.

La prueba de Named Pipe debe ejecutarse fuera del sandbox de Codex porque el
sandbox bloquea la conexion local aun cuando servidor y cliente comparten
usuario.

El detalle del snapshot vive en `read-only-device-diagnostics.md`.
El adaptador real y su matriz VM viven en `seven-zip-adapter.md`.
