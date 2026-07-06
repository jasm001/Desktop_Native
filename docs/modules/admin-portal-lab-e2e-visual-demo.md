# Recorrido end-to-end visual de laboratorio

## Alcance

La Unidad 5 compone una demostracion visual local sobre datos reales de
laboratorio ya persistidos. No conecta servicios productivos, no usa datos
corporativos y no convierte `local-demo` en piloto.

La vista administrativa `/admin/lab` muestra trazas por `correlationId` para el
recorrido:

1. WinUI o cliente local confirma explicitamente una solicitud.
2. La API local crea `SupportRequest`, `ExecutionJob`, `BotCase`, auditoria y
   outbox dentro de PostgreSQL local.
3. El worker procesa outbox y registra efectos saneados.
4. El agente simulado o la VM personal, cuando exista, reporta evidencia
   acotada.
5. El caso queda atendido o escalado.
6. Si aplica, se crea un ticket fake persistido.

## Gobierno

- Una consulta administrativa no crea solicitudes.
- Solo `POST` confirmado con identidad de desarrollo e idempotency key crea
  solicitud, trabajo y caso.
- La vista usa `select` explicitos y no renderiza payloads completos, prompts,
  secretos, headers ni texto operativo.
- No ejecuta comandos, no llama al DeviceAgent, no arranca servicios y no
  administra VM.
- Los tickets mostrados siguen siendo fake aunque el registro sea real en
  laboratorio.

## Degradacion

La traza muestra estados honestos:

- worker pendiente o no comprobado cuando el outbox aun no produjo efecto;
- agente o VM no comprobados cuando no existe resultado;
- bridge, Hermes, mirror y VM degradados desde el health local existente;
- ticket fake ausente cuando el caso no requiere escalacion o el outbox sigue
  pendiente.

## Idempotencia

La vista muestra evidencia de proteccion contra duplicados sin exponer las keys:

- solicitud protegida por `idempotencyKey` unica;
- resultado protegido por `resultIdempotencyKey` cuando el agente ya reporto;
- conteo de duplicados detectados como diferencia entre filas existentes y la
  unica fila esperada.

## Cierre

Este recorrido es una demostracion local con datos `lab-real-sanitized`.
No acredita despliegue productivo, piloto, integracion corporativa ni ejecucion
administrativa desde el portal.
