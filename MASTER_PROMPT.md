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
- Bloque 9, canal Teams existente, esta `in_progress` y es el unico bloque
  principal activo.
- El primer incremento local del Bloque 9 esta implementado y validado.
- `modules/TEAMS.md` es el documento propietario.
- La evidencia tecnica vive en
  `docs/modules/teams-channel-local-increment.md`.
- Existe `conversation-channel.v1` estricto en C# y TypeScript.
- `IConversationChannel` es una frontera de traduccion.
- `RecordedTeamsConversationChannel` es local, determinista, sin red y no
  representa un payload real de Microsoft.
- `ConversationChannelService` reutiliza `ConversationService` y
  `CatalogDecisionService`.
- WinUI usa la misma aplicacion normalizada y pasa fixtures de paridad con el
  canal recorded.
- Correlacion, dispositivo e idempotency key se propagan al control plane.
- Los endpoints existentes cubren catalogo, instalacion confirmada, estado de
  solicitud y consulta de caso/escalamiento.
- No existe una mutacion HTTP durable para `HumanReview`; el canal exige
  confirmacion y devuelve `capability_unavailable` sin efectos laterales.
- No se creo un segundo bot ni una conexion Microsoft 365.
- OpenText real, Teams corporativo, Entra, UEMS, Hermes/RAG productivo y portal
  administrativo siguen deshabilitados.
- Ultimo gate completo: 125 pruebas .NET, 20 pruebas Node unitarias/de contrato,
  11 integraciones AdminWeb, 4 del Worker, cuatro migraciones PostgreSQL y E2E.
- `scripts/Validate.ps1`, auditoria de dependencias y escaneo de secretos pasan.

Antes de editar:
1. ejecuta `git status --branch --short`, `git log -5 --oneline` y verifica el
   tracking de `main`;
2. lee completos `README.md`, `CURRENT_CONTEXT.md`, `WORKFLOW.md`,
   `DEVELOPMENT_PLAN.md` y `CONSISTENCY_AUDIT.md`;
3. aplica la precedencia documental de `README.md`;
4. lee completos `core/SECURITY.md`, `core/DECISIONS.md`, `core/SCOPE.md`,
   `core/STACK.md` y `core/ARCHITECTURE.md`;
5. lee completos `standards/CODING_STANDARDS.md` y `standards/DELIVERY.md`;
6. lee completos `modules/TEAMS.md`, `modules/ASSISTANT.md`,
   `modules/CATALOG.md`, `modules/TICKETING.md` y `modules/OPERATIONS.md`;
7. lee `docs/modules/teams-channel-local-increment.md`,
   `context/IDENTITY_AND_TEAMS_RESPONSES.md` y la seccion Teams de
   `project-management/INFORMATION_REQUESTS.md`;
8. inspecciona `src/Conversation`, `src/Desktop`, `src/AdminWeb`,
   `src/Contracts`, `src/Contracts/Web` y sus pruebas;
9. consulta `modules/ADMIN_PORTAL.md` solo para proteger el Bloque 11.

Reglas no negociables:
- no cambies stack, alcance, arquitectura, persistencia, seguridad o contratos
  publicos sin registrar un stopper en `WORKFLOW.md`;
- conserva Next.js/TypeScript/Prisma/PostgreSQL como control plane;
- conserva el worker Node como proceso durable separado;
- integra el bot existente; no crees otro bot, tenant o aplicacion corporativa;
- no inventes owner, plataforma Teams, App Registration, IDs, permisos,
  endpoints, Adaptive Cards, secretos ni capacidades del bot;
- Teams y WinUI son canales del mismo asistente y no mantienen reglas separadas;
- `IConversationChannel` es traduccion, no motor de negocio;
- Teams nunca ejecuta comandos, construye argumentos ni llama al DeviceAgent;
- una consulta no crea solicitud, caso, ticket, trabajo, auditoria ni outbox;
- toda mutacion requiere confirmacion explicita, identidad, dispositivo,
  correlacion e idempotencia;
- payloads desconocidos, versiones no soportadas y acciones no allowlisted
  fallan cerrados;
- no uses datos, credenciales, endpoints ni identificadores corporativos;
- una sesion abierta de Teams no autoriza recuperacion de identidad ni acciones
  sensibles;
- no conectes OpenText real, Entra, UEMS, Power Automate o Microsoft 365 sin
  evidencia y aprobacion;
- no adelantes endurecimiento del Bloque 10 ni portal del Bloque 11;
- no agregues dependencias si las APIs actuales son suficientes;
- no declares completado el Bloque 9 sin integrar y validar el bot real;
- no declares terminada una unidad sin ejecutar los gates aplicables.

Tarea de reanudacion:
Continuar el Bloque 9 solo cuando se disponga de evidencia saneada del bot
corporativo existente. Confirmar:

- owner y contacto tecnico;
- plataforma y runtime;
- repositorio y proceso de despliegue/rollback;
- autenticacion del bot frente al control plane;
- permisos, tenant, App Registration o workload identity;
- ambientes, DLP, limites, soporte y auditoria;
- mecanismo real de acciones o tarjetas;
- payloads de ejemplo sin datos reales;
- capacidad de consumir la API compartida.

Con esa evidencia:
1. implementar el adaptador corporativo como sustituto de
   `RecordedTeamsConversationChannel`;
2. conservar `conversation-channel.v1` y las reglas en
   `ConversationChannelService`;
3. validar identidad, vinculacion de dispositivo, correlacion e idempotencia;
4. probar catalogo, propuesta, confirmacion, solicitud, estado y caso;
5. definir primero un caso de uso durable del control plane si se requiere
   escalamiento manual `HumanReview`; no lo simules dentro del canal;
6. mantener Teams sin acceso al DeviceAgent;
7. actualizar pruebas, documento propietario, evidencia, `WORKFLOW.md`,
   `CURRENT_CONTEXT.md` y esta ayuda de handoff.

Stopper externo vigente:
Fecha: 2026-06-14
Modulo: Canal Teams
Decision requerida: Confirmar owner, plataforma, repositorio, autenticacion,
  permisos, tenant, ambientes, DLP, despliegue y mecanismo de acciones del bot
  corporativo existente.
Evidencia: Contacto tecnico; configuracion o diagrama saneado; contrato de
  autenticacion; ambiente de prueba; payloads de ejemplo sin datos reales;
  proceso de despliegue y rollback.
Impacto: Sin esta evidencia no puede validarse ni declararse completa la
  integracion corporativa del Bloque 9.
Recomendacion: Mantener neutral `IConversationChannel` y no acoplar el dominio a
  una plataforma no confirmada.
Owner: Equipo actual del bot de Teams / Collaboration / Automation.

Gate base:
.\scripts\Validate.ps1

Comandos adicionales:
corepack pnpm@11.5.3 audit --prod --audit-level high
.\scripts\Test-Secrets.ps1

Al terminar:
- informa alcance implementado, archivos, contratos y pruebas;
- lista riesgos y pendientes externos;
- actualiza estado documental;
- recomienda un Conventional Commit;
- no hagas commit ni push automaticamente.
