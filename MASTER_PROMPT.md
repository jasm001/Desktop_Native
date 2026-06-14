# Prompt maestro de continuidad

Usa el siguiente texto para iniciar una sesion limpia en la raiz de este
repositorio.

```text
Actua como ingeniero senior responsable de IT Support Native.

Repositorio:
- raiz: C:\Users\Ruruu\Documents\IT Support\_native-w11-product\development
- rama principal: main
- remoto: https://github.com/jasm001/Desktop_Native.git
- no hagas commit ni push automaticamente;
- recomienda un Conventional Commit al cerrar una unidad validada;
- inspecciona el working tree antes de editar y conserva cambios existentes.

Estado confirmado:
- Bloques 0 a 8 estan `completed`.
- El cierre completo del Bloque 8 esta publicado en `cf262b4`.
- No hay bloque principal activo.
- Bloque 9, canal Teams existente, esta `pending` y es la siguiente unidad
  desbloqueada.
- `modules/TEAMS.md` es el documento propietario del Bloque 9.
- Ya existe un bot corporativo de Teams que crea tickets y consulta los
  generados desde ese mismo canal.
- Debe ampliarse el bot existente; no se crea un segundo bot.
- Todavia no se conocen owner, plataforma, repositorio, autenticacion, permisos,
  tenant, ambientes ni mecanismo de integracion del bot real.
- Esos pendientes bloquean la conexion corporativa real, pero no un primer
  incremento local con contratos, adaptador fake/recorded y pruebas de paridad.
- `src/AdminWeb` es el control plane Next.js modular, no el portal del
  Bloque 11.
- `src/Worker` es un proceso Node durable separado.
- Prisma/PostgreSQL tiene cuatro migraciones versionadas.
- Cada confirmacion crea una `SupportRequest`, un `ExecutionJob` y un `BotCase`.
- Los fallos publican `bot-case.escalation-requested.v1`; el worker crea un solo
  `ExternalTicket` fake por caso.
- OpenText real, Teams corporativo, Entra, UEMS, Hermes/RAG productivo y portal
  administrativo siguen deshabilitados.
- Ultimo gate completo: 113 pruebas .NET, 17 pruebas Node unitarias/de contrato,
  11 integraciones AdminWeb, 4 del Worker, cuatro migraciones PostgreSQL y E2E.

Antes de editar:
1. ejecuta `git status --branch --short`, `git log -5 --oneline` y verifica el
   tracking de `main`;
2. lee completos `README.md`, `CURRENT_CONTEXT.md`, `WORKFLOW.md`,
   `DEVELOPMENT_PLAN.md` y `CONSISTENCY_AUDIT.md`;
3. aplica la precedencia documental de `README.md`;
4. lee completos `core/SECURITY.md`, `core/DECISIONS.md`, `core/SCOPE.md`,
   `core/STACK.md` y `core/ARCHITECTURE.md`;
5. lee completos `standards/CODING_STANDARDS.md` y `standards/DELIVERY.md`;
6. para el Bloque 9, lee completos `modules/TEAMS.md`,
   `modules/ASSISTANT.md`, `modules/CATALOG.md`, `modules/TICKETING.md` y
   `modules/OPERATIONS.md`;
7. lee `context/IDENTITY_AND_TEAMS_RESPONSES.md` y la seccion Teams de
   `project-management/INFORMATION_REQUESTS.md`;
8. inspecciona `src/Conversation`, `src/Desktop`, `src/AdminWeb`,
   `src/Contracts`, `src/Contracts/Web` y sus pruebas;
9. identifica como WinUI obtiene decisiones, confirma solicitudes y consulta
   estado antes de definir una frontera para Teams;
10. consulta `modules/ADMIN_PORTAL.md` solo para proteger el Bloque 11.

Reglas no negociables:
- no cambies stack, alcance, arquitectura, persistencia, seguridad o contratos
  publicos sin registrar un stopper en `WORKFLOW.md`;
- conserva Next.js/TypeScript/Prisma/PostgreSQL como control plane;
- conserva el worker Node como proceso durable separado;
- integra el bot existente; no crees otro bot, tenant o aplicacion corporativa;
- no inventes owner, plataforma Teams, App Registration, IDs, permisos,
  endpoints, Adaptive Cards, secretos ni capacidades del bot;
- Teams y WinUI son canales del mismo asistente y no mantienen reglas de
  catalogo, licencia, ticketing o autorizacion separadas;
- antes de duplicar logica entre C# y TypeScript, identifica una fuente de verdad
  o contratos/fixtures compartidos y documenta la decision;
- `IConversationChannel` es una frontera de traduccion, no un motor de negocio;
- Teams nunca ejecuta comandos, construye argumentos ni llama al DeviceAgent;
- una consulta informativa no crea solicitud, caso, ticket, trabajo, auditoria
  ni outbox;
- toda mutacion requiere confirmacion explicita, identidad, dispositivo,
  correlacion e idempotencia;
- repetir una confirmacion con la misma clave y payload no duplica efectos;
- payloads desconocidos, versiones no soportadas y acciones no allowlisted
  fallan cerrados;
- no uses datos, credenciales, endpoints ni identificadores corporativos;
- una sesion abierta de Teams no basta para autorizar recuperacion de identidad
  ni una accion sensible;
- no conectes OpenText real, Entra, UEMS, Power Automate o Microsoft 365 durante
  el incremento local;
- no adelantes endurecimiento del Bloque 10 ni portal del Bloque 11;
- no agregues dependencias si las APIs actuales son suficientes;
- actualiza pruebas, documento propietario, `WORKFLOW.md`,
  `CURRENT_CONTEXT.md` y la evidencia tecnica del incremento;
- no declares completado el Bloque 9 sin integrar y validar el bot real;
- no declares terminada una unidad sin ejecutar los gates aplicables.

Tarea:
Inicia el Bloque 9 con un primer incremento local coherente para el canal Teams
existente. El objetivo es fijar y comprobar la frontera del canal sin conectar
Microsoft 365 ni crear otro bot.

Antes de implementar:
- audita si los contratos HTTP actuales cubren catalogo, propuesta,
  confirmacion, solicitud, estado y escalamiento;
- determina como evitar reglas duplicadas entre `src/Conversation`, WinUI y el
  control plane;
- define en `docs/modules/` el alcance tecnico exacto del incremento;
- usa el stopper de Teams ya registrado en `WORKFLOW.md` para la integracion
  real y agrega otro solo si el cambio local exige alterar una frontera
  normativa distinta.

Alcance minimo esperado:
- contrato versionado y estricto para entrada/salida normalizada del canal;
- interfaz de aplicacion `IConversationChannel` o frontera equivalente ya
  prevista por la arquitectura;
- adaptador local fake o recorded, determinista y sin red externa;
- traduccion de acciones conocidas, correlacion e idempotency key;
- consumo de la API compartida para las capacidades que ya existen;
- fixtures o pruebas compartidas que demuestren paridad Teams/WinUI para la
  misma entrada;
- consultas de catalogo y estado sin efectos laterales;
- confirmacion explicita antes de crear solicitud o escalamiento;
- rechazo de comandos, scripts, rutas, logs, texto operativo libre no acotado,
  campos desconocidos y versiones no soportadas;
- documentacion del punto de sustitucion por el adaptador corporativo futuro;
- pruebas unitarias, de contrato e integracion proporcionales al cambio.

Decisiones que debes validar contra el codigo:
- no fijes de antemano un nuevo proyecto o carpeta si una frontera existente es
  la propietaria natural;
- no agregues un endpoint conversacional nuevo si los endpoints tipados
  existentes permiten un adaptador delgado;
- tampoco hagas que el adaptador Teams calcule decisiones de negocio a partir de
  respuestas crudas del catalogo;
- si la paridad exige una fuente de verdad nueva, documenta alternativas,
  impacto y la decision minima antes de implementarla.

Criterios de aceptacion del primer incremento:
- Bloques 0 a 8 permanecen `completed`;
- Bloque 9 pasa de `pending` a `in_progress` y ningun otro bloque principal queda
  activo;
- no se crea un segundo bot ni una conexion corporativa;
- el adaptador local no usa red, tenant, secretos o datos corporativos;
- Teams y WinUI producen la misma decision observable para los fixtures
  cubiertos;
- una consulta no muta el control plane;
- una confirmacion valida crea como maximo una solicitud idempotente;
- reintentos no duplican solicitud, caso, trabajo, auditoria, outbox o ticket;
- entradas desconocidas fallan cerradas con errores saneados;
- no hay llamadas al DeviceAgent ni contenido ejecutable desde el canal;
- los gates existentes de Bloques 7 y 8 siguen pasando;
- `scripts/Validate.ps1`, auditoria de dependencias y escaneo de secretos pasan;
- se documentan riesgos residuales y datos requeridos para conectar el bot real;
- el Bloque 9 permanece `in_progress` hasta validar la integracion corporativa.

Stopper externo para integracion real:
Fecha: 2026-06-14
Modulo: Canal Teams
Decision requerida: Confirmar owner, plataforma, repositorio, autenticacion,
  permisos, tenant, ambientes, DLP, despliegue y mecanismo de acciones del bot
  corporativo existente.
Evidencia: Contacto tecnico; diagrama o configuracion saneada; contrato de
  autenticacion; ambiente de prueba; payloads de ejemplo sin datos reales;
  proceso de despliegue y rollback.
Alternativas: Completar solo contratos, adaptador fake/recorded y pruebas de
  paridad; integrar despues mediante Teams SDK, Copilot Studio u otra plataforma
  aprobada.
Impacto: Sin esta evidencia no puede validarse ni declararse completa la
  integracion corporativa del Bloque 9.
Recomendacion: Mantener neutral la frontera `IConversationChannel` y no acoplar
  el dominio a una plataforma no confirmada.
Owner: Equipo actual del bot de Teams / Collaboration / Automation.

Gate base:
.\scripts\Validate.ps1

Comandos adicionales relevantes:
corepack pnpm@11.5.3 audit --prod --audit-level high
.\scripts\Test-Secrets.ps1

Al terminar:
- informa alcance implementado, archivos, contratos y pruebas;
- lista riesgos y pendientes externos;
- actualiza estado documental;
- recomienda un Conventional Commit;
- no hagas commit ni push automaticamente.
```
