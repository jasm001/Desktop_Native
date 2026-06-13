# Fundacion del control plane

## Estado

Primer incremento del Bloque 7 implementado, validado y publicado en `2b89a6b`.
La fundacion se complementa con el recorrido local documentado en
`control-plane-local-flow.md`, que cierra el Bloque 7.

## Objetivo acotado

Establecer la frontera ejecutable del control plane compartido sin construir el
portal administrativo ni conectar canales o procesos privilegiados.

Este incremento incluye:

- `src/AdminWeb` como aplicacion Next.js App Router sobre Node.js, con
  TypeScript estricto y sin interfaz administrativa;
- contratos HTTP v1 y eventos de outbox compartidos, tipados y validados;
- identidad local sintetica, determinista y habilitable solo en desarrollo;
- modulos iniciales de identidad, dispositivos, catalogo, solicitudes/trabajos,
  auditoria y outbox;
- Prisma sobre PostgreSQL con una primera migracion versionada;
- una mutacion sintetica confirmada que crea solicitud, trabajo, auditoria y
  outbox en una sola transaccion;
- idempotencia por clave y hash canonico del payload;
- consultas de catalogo y estado sin efectos laterales;
- `src/Worker` como proceso Node durable separado que reclama un outbox con
  `FOR UPDATE SKIP LOCKED`, retry acotado y efecto sintetico idempotente;
- pruebas unitarias, de contrato e integracion contra una base PostgreSQL
  efimera real.

## Superficie HTTP v1

- `GET /api/v1/catalog/products`: catalogo sintetico paginado y de solo lectura.
- `POST /api/v1/requests/software-installations`: crea una solicitud sintetica
  solo con confirmacion explicita, identidad de desarrollo valida e
  `Idempotency-Key`.
- `GET /api/v1/requests/{requestId}`: consulta el estado persistido de la
  solicitud y su trabajo, sin crear auditoria, outbox ni otras mutaciones.
- `POST /api/v1/agent/jobs/claim`: entrega un trabajo listo solo a la identidad
  sintetica exacta del agente de desarrollo.
- `POST /api/v1/agent/jobs/{jobId}/result`: registra resultado y evidencia
  allowlisted con lease e idempotencia.

Los contratos no exponen entidades Prisma. Los errores usan codigos fijos,
mensajes saneados y correlacion. Los identificadores de accion, producto,
version y dispositivo son valores tipados y acotados; no existen campos para
comandos, scripts, rutas o argumentos.

## Modelo minimo

- `Device`: registro sintetico acotado para asociar la solicitud.
- `SupportRequest`: estado, correlacion, identidad solicitante, payload
  canonico e idempotencia.
- `ExecutionJob`: trabajo tipado en cola; no contiene contenido ejecutable.
- `AuditEvent`: registro append-only protegido tambien por la base de datos.
- `OutboxEvent`: evento transaccional con claim, lease, intentos y estado.
- `SyntheticOutboxEffect`: recibo unico que demuestra procesamiento
  idempotente sin llamar servicios externos.

La migracion `20260613074457_control_plane_foundation` crea el esquema base. La
migracion aditiva `20260613183000_control_plane_local_flow` agrega leases,
resultado idempotente y evidencia de ejecucion. PostgreSQL gobierna los
timestamps mediante triggers para evitar que el huso horario del proceso Node
altere fechas persistidas. La auditoria rechaza `UPDATE` y `DELETE` en la base.

## Limites

Este incremento no incluye:

- portal administrativo, RBAC productivo, Entra, MFA o login corporativo;
- backend conectado directamente al DeviceAgent o a procesos privilegiados;
- tickets, `BotCase`, OpenText, Teams, UEMS, Rescue o telemetria productiva;
- Windows Service, Hermes, RAG, artefactos, descargas o ejecucion local;
- comandos, PowerShell, shell, rutas arbitrarias o parametros generados;
- SQLite, `prisma db push`, mocks de base como evidencia de integracion o datos
  corporativos.

## Gate de integracion

El repositorio usa `DATABASE_URL` solo desde el entorno local o CI. El gate crea
una base PostgreSQL temporal con un nombre aleatorio, aplica
`prisma migrate deploy`, ejecuta las pruebas de integracion y elimina la base al
final. La cuenta debe poder crear bases de datos.

En el entorno inspeccionado el 2026-06-13 no existe Docker en `PATH`, pero hay
PostgreSQL 18 local, `DATABASE_URL` conecta correctamente y el rol tiene
`CREATEDB`. No se requiere stopper ni degradacion.

## Resultado del incremento

- Next.js `16.2.9`, React `19.2.7`, Prisma `7.8.0`, TypeScript `5.9.3`,
  Zod `4.4.3` y Vitest `4.1.8`, con versiones fijadas en el lockfile.
- El build de produccion valida y limpia solo `.next` con reintentos, usa el
  builder webpack soportado por Next.js y fija una configuracion PostCSS local.
  Esto evita herencia ambiental y bloqueos reproducibles de handles sobre
  salidas anteriores en Windows.
- La repeticion de una clave con el mismo payload reutiliza la solicitud; un
  payload distinto devuelve `idempotency_conflict`.
- Las consultas HTTP de catalogo y estado no crean mutaciones.
- El worker valida que conserva el claim y publica un efecto de despacho
  sintetico sin ejecutar el trabajo; el resultado del agente produce un segundo
  efecto idempotente y los eventos invalidos fallan tras tres intentos.
- La integracion cubre 10 pruebas de AdminWeb y 3 del worker sobre PostgreSQL
  efimero real, ademas del recorrido ejecutable de extremo a extremo en Windows.
- CI dispone de un servicio PostgreSQL 18 separado para este gate.

## Riesgos residuales

- La identidad sigue siendo exclusivamente sintetica y de desarrollo.
- Solo existe un evento y efecto sinteticos; no hay adaptadores externos.
- No se han definido todavia retencion, observabilidad productiva, despliegue,
  autenticacion corporativa ni escalado horizontal de produccion.
- El portal administrativo continua reservado para el Bloque 11.

## Siguiente bloque

El Bloque 8 esta desbloqueado y permanece `pending`. Debe agregar casos,
escalamiento y `ITicketingProvider` fake sobre estas fronteras sin conectar
OpenText real.
