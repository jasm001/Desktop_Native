# Incremento local del canal Teams

## Estado

Primer incremento del Bloque 9. El bloque pasa a `in_progress` y no puede
cerrarse hasta integrar y validar el bot corporativo existente.

## Objetivo

Fijar una frontera neutral y comprobable para un canal conversacional remoto sin
conectar Microsoft 365, crear otro bot ni asumir plataforma, tenant,
autenticacion, permisos, Adaptive Cards o despliegue.

## Auditoria de la API compartida

La superficie HTTP v1 existente cubre:

- catalogo sintetico paginado y de solo lectura;
- creacion confirmada de una instalacion allowlisted con correlacion e
  idempotencia;
- consulta de solicitud y trabajo sin efectos laterales;
- consulta de `BotCase` y escalamiento/ticket fake sin efectos laterales.

No expone:

- una API conversacional;
- calculo de propuesta o decision de catalogo;
- creacion durable de una revision humana para software comercial, no listado
  u otro escalamiento solicitado por el usuario.

No se agrega un endpoint conversacional. La maquina de estados y
`CatalogDecisionService` ya son la fuente de verdad para consulta, propuesta,
confirmacion y rechazo. El adaptador solo usa la API compartida para las
capacidades durables que ya existen.

## Decision tecnica

El incremento agrega:

- contrato JSON normalizado `conversation-channel.v1`, estricto y versionado;
- `IConversationChannel` como frontera de traduccion de payloads;
- `RecordedTeamsConversationChannel`, local, determinista y sin red, que usa
  exclusivamente el contrato normalizado del repositorio;
- un caso de uso de canal sobre `ConversationService`;
- propagacion de correlacion, dispositivo e idempotency key hacia el cliente del
  control plane;
- consulta tipada de solicitud y caso;
- fixtures compartidos entre contratos C# y TypeScript;
- pruebas de paridad que ejecutan la misma entrada por la fabrica WinUI y por el
  adaptador recorded.

El nombre del adaptador indica el canal objetivo, no un payload real de
Microsoft. El futuro adaptador corporativo sustituira solo la lectura/escritura
de plataforma y entregara el mismo contrato normalizado.

## Reglas del contrato

Entradas permitidas:

- `catalog.query`;
- `software.request`;
- `proposal.continue`;
- `request.confirm`;
- `conversation.cancel`;
- `request.status`;
- `case.status`.

Cada entrada incluye version, mensaje, correlacion, sesion, actor y dispositivo.
`request.confirm` exige idempotency key. Las consultas de estado exigen un UUID
de solicitud.

El contrato rechaza:

- campos desconocidos;
- versiones no soportadas;
- acciones fuera de allowlist;
- comandos, scripts, argumentos, rutas, logs o texto operativo libre;
- identificadores fuera de limites;
- combinaciones de campos no validas para la accion.

Las salidas contienen codigos y vistas tipadas; no incluyen contenido
ejecutable, secretos ni errores internos.

## Mutaciones

- `catalog.query`, `request.status` y `case.status` no mutan la conversacion ni
  el control plane.
- `software.request` solo produce propuesta o rechazo.
- `request.confirm` solo crea una solicitud durable para
  `SoftwareAcquisition`.
- La misma idempotency key y payload reutilizan la solicitud existente.
- Una confirmacion de `HumanReview` no crea una instalacion ni un escalamiento
  durable porque la API actual no ofrece ese caso de uso. Devuelve una
  capacidad no disponible despues de comprobar la confirmacion explicita.

## Sustitucion futura

Cuando se conozca la plataforma del bot existente se implementara otro
`IConversationChannel` que:

1. valide el payload nativo de la plataforma aprobada;
2. normalice identidad y contexto autorizados;
3. produzca `conversation-channel.v1`;
4. presente la respuesta tipada mediante el mecanismo aprobado;
5. conserve correlacion e idempotencia.

La sustitucion no movera reglas de catalogo, autorizacion, ticketing o ejecucion
al bot. Teams nunca llamara al DeviceAgent.

## Fuera de alcance

- Microsoft 365, Teams SDK, Copilot Studio o Power Automate;
- tenant, App Registration, secretos, permisos o datos corporativos;
- un segundo bot;
- OpenText real, Entra, UEMS, Rescue, Hermes/RAG o portal administrativo;
- un nuevo endpoint conversacional;
- una nueva mutacion durable para revision humana;
- cambios al DeviceAgent.

## Gate local

- contratos C# y TypeScript aceptan los mismos fixtures validos;
- payloads desconocidos y ejecutables fallan cerrados;
- WinUI y el canal recorded producen la misma decision observable;
- consultas no mutan;
- confirmacion aprobada es idempotente;
- estado y caso usan la API compartida;
- gates completos de los Bloques 7 y 8 permanecen correctos.

## Evidencia de validacion

Validado el 2026-06-14:

- `scripts/Validate.ps1`: correcto;
- build Release: 0 warnings y 0 errores;
- .NET: 125 pruebas;
- Node unitario/contrato: 20 pruebas;
- AdminWeb sobre PostgreSQL efimero: 11 pruebas;
- Worker sobre PostgreSQL efimero: 4 pruebas;
- cuatro migraciones con `prisma migrate deploy`;
- E2E WinUI/API/worker/DeviceAgent: correcto;
- `corepack pnpm@11.5.3 audit --prod --audit-level high`: sin
  vulnerabilidades conocidas;
- `scripts/Test-Secrets.ps1`: sin hallazgos.

## Riesgos pendientes

- plataforma, owner, repositorio, autenticacion, permisos, tenant, ambientes,
  DLP, despliegue y rollback del bot real;
- contrato de identidad y vinculacion de dispositivo;
- formato y capacidades reales de acciones del canal;
- caso de uso durable para revision humana solicitado desde un canal;
- validacion corporativa de conectividad, auditoria y soporte.

El stopper de `WORKFLOW.md` permanece vigente y bloquea el cierre, no este
incremento local.
