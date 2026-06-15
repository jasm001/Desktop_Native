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
- Bloque 9, canal Teams existente, esta `blocked`; su incremento local esta
  publicado en `0448a42`.
- Bloque 10, endurecimiento para piloto, esta `blocked`; su cierre local esta
  publicado en `be6c4fc`.
- Bloque 11, portal administrativo web, esta `in_progress` y es el unico bloque
  principal activo.
- `modules/TEAMS.md`, `modules/PILOT_HARDENING.md` y
  `modules/ADMIN_PORTAL.md` son los documentos propietarios de los Bloques 9,
  10 y 11 respectivamente.
- `src/AdminWeb` es el control plane Next.js modular sobre el que se construye
  el Bloque 11.
- La pagina `/` actual de AdminWeb sigue siendo una superficie tecnica del
  Bloque 8; no es el portal administrativo terminado.
- AdminWeb ya contiene APIs v1, identidad sintetica de desarrollo,
  Prisma/PostgreSQL, auditoria append-only, outbox, solicitudes, trabajos,
  `BotCase` y ticketing fake.
- No existen aun autenticacion de portal, OIDC/Entra, RBAC server-side, rutas
  administrativas, Fluent UI, Testing Library ni Playwright.
- `src/Worker` conserva el proceso Node durable separado.
- Prisma/PostgreSQL tiene cuatro migraciones versionadas.
- Cada confirmacion crea una `SupportRequest`, un `ExecutionJob` y un
  `BotCase`.
- Los fallos publican `bot-case.escalation-requested.v1`; el worker crea un solo
  `ExternalTicket` fake por caso.
- Teams local usa `conversation-channel.v1`, `IConversationChannel` y un
  adaptador recorded sin red.
- DeviceAgent conserva IPC tipado, allowlist cerrada, idempotencia, evidencia
  saneada, kill switch y confinamiento de `local-demo` a `Development`.
- OpenText real, Teams corporativo, Entra, UEMS, Sophos, PKI, Rescue,
  Hermes/RAG productivo y portal administrativo productivo siguen
  deshabilitados.
- Ultimo gate completo: 136 pruebas .NET, 20 pruebas Node unitarias/de contrato,
  11 integraciones AdminWeb, 4 del Worker, cuatro migraciones PostgreSQL y E2E.
- `scripts/Validate.ps1`, auditoria de dependencias y escaneo de secretos pasan.
- La documentacion vigente fue auditada y alineada para iniciar el Bloque 11;
  `context/` y `reference/` conservan historia y no definen el estado actual.

Antes de editar:
1. ejecuta `git status --branch --short`, `git log -5 --oneline` y verifica el
   tracking de `main`;
2. lee completos `README.md`, `CURRENT_CONTEXT.md`, `WORKFLOW.md`,
   `DEVELOPMENT_PLAN.md` y `CONSISTENCY_AUDIT.md`;
3. aplica la precedencia documental de `README.md`;
4. lee completos `core/SECURITY.md`, `core/DECISIONS.md`, `core/SCOPE.md`,
   `core/STACK.md` y `core/ARCHITECTURE.md`;
5. lee completos `standards/CODING_STANDARDS.md` y `standards/DELIVERY.md`;
6. lee completos `modules/ADMIN_PORTAL.md`, `modules/WEB_DELIVERY.md`,
   `modules/OPERATIONS.md`, `modules/PILOT_HARDENING.md` y `modules/TEAMS.md`;
7. lee completos `docs/modules/control-plane-foundation.md`,
   `docs/modules/control-plane-local-flow.md`,
   `docs/modules/local-mvp-lab.md`, `docs/threat-model/README.md` y
   `project-management/INFORMATION_REQUESTS.md`;
8. inspecciona completos `src/AdminWeb/package.json`, `src/AdminWeb/src/app`,
   `src/AdminWeb/src/modules/identity`, `src/AdminWeb/src/platform`,
   `src/AdminWeb/tests`, `src/AdminWeb/prisma`, `src/Contracts/Web` y
   `src/Worker`;
9. confirma que la pagina actual, dependencias, identidad y pruebas coinciden
   con la linea base documentada antes de elegir la implementacion.

Reglas no negociables:
- no cambies stack, alcance, arquitectura, persistencia, seguridad o contratos
  publicos sin registrar un stopper en `WORKFLOW.md`;
- conserva Next.js App Router, TypeScript estricto, Prisma y PostgreSQL como
  control plane;
- conserva el worker Node como proceso durable separado;
- no agregues un backend ASP.NET ni microservicios;
- el portal no ejecuta comandos, scripts, rutas, argumentos ni texto operativo;
- el portal nunca llama directamente al DeviceAgent;
- toda autorizacion se aplica server-side y falla cerrada;
- identidades, roles, scopes, payloads o versiones desconocidos se rechazan;
- una identidad de desarrollo solo puede habilitarse con ambiente local exacto
  y configuracion explicita;
- `DeveloperAllAccess` nunca se acepta fuera de desarrollo;
- toda mutacion futura requiere identidad, autorizacion, confirmacion,
  correlacion, idempotencia y auditoria;
- no uses datos, credenciales, endpoints, tenant, certificados ni
  identificadores corporativos;
- no conectes Entra, Microsoft 365, Teams, OpenText, Rescue, UEMS, Sophos, PKI
  ni servicios productivos;
- no inventes owners, asignaciones de rol, scopes corporativos, cuentas de
  servicio, permisos, retencion, excepciones ni mecanismos de despliegue;
- los roles productivos de `modules/ADMIN_PORTAL.md` son un modelo objetivo, no
  asignaciones confirmadas;
- no conviertas la identidad sintetica en autenticacion productiva;
- no reabras ni declares completados los Bloques 9 o 10;
- no declares completado el Bloque 11 con una sola unidad local;
- no agregues dependencias salvo las aprobadas por `core/STACK.md` y necesarias
  para la unidad; conserva versiones fijadas y lockfile reproducible;
- no declares terminada una unidad sin ejecutar los gates aplicables.

Tarea:
Inicia el Bloque 11 con una primera unidad local pequena y completa sobre
`src/AdminWeb`: frontera de identidad/autorizacion server-side fail-closed y
shell administrativo minimo, accesible y de solo lectura.

Antes de implementar:
- contrasta la identidad sintetica existente con una identidad de portal;
- evita reutilizar la identidad del agente como identidad de usuario;
- define en `docs/modules/admin-portal-foundation.md` alcance exacto,
  alternativas, contratos internos, evidencia y criterio de aceptacion;
- decide, siguiendo los patrones actuales, la ruta y abstraccion minimas para
  el portal sin mover reglas de dominio a componentes React;
- identifica si Fluent UI React y Playwright son necesarios en esta unidad;
  si se agregan, usa solo las dependencias aprobadas y actualiza el lockfile;
- registra un stopper solo si la solucion exige alterar una frontera normativa
  o depende de una decision externa no documentada.

Alcance minimo esperado:
- identidad de portal local, sintetica y separada de la identidad del agente;
- habilitacion solo con `IT_SUPPORT_ENVIRONMENT=development` y configuracion
  explicita equivalente;
- rechazo server-side fuera del perfil permitido, sin render parcial;
- politica de autorizacion deny-by-default para rol o capability conocida;
- shell administrativo minimo con navegacion accesible y datos sinteticos o de
  solo lectura;
- ninguna mutacion administrativa en esta primera unidad;
- sin cambios a contratos IPC, acciones del DeviceAgent o worker durable;
- pruebas unitarias y/o integracion de acceso permitido, acceso denegado,
  ambiente invalido y ausencia de efectos laterales;
- verificacion de teclado, foco, layout adaptable y semantica basica;
- punto de sustitucion documentado para OIDC/Entra futuro;
- riesgos residuales y gates externos claramente separados.

Criterios de aceptacion:
- Bloques 0 a 8 permanecen `completed`;
- Bloques 9 y 10 permanecen `blocked`;
- Bloque 11 permanece como unico bloque `in_progress`;
- no se crea ninguna conexion o identidad corporativa;
- la identidad local falla cerrada fuera de desarrollo;
- la autorizacion ocurre en servidor antes de mostrar contenido protegido;
- roles o capabilities desconocidos se rechazan;
- el shell no ejecuta comandos ni llama al DeviceAgent;
- no se agregan mutaciones, secretos, PII o datos corporativos;
- `src/AdminWeb` conserva sus APIs y modulos existentes;
- el worker Node sigue separado;
- las cuatro migraciones existentes no se modifican retroactivamente;
- los gates de los Bloques 7 a 10 siguen pasando;
- `scripts/Validate.ps1`, auditoria de dependencias y escaneo de secretos pasan;
- no se simula Entra, aprobacion Security ni un portal productivo completo.

Stopper externo de Teams que debe conservarse:
Fecha: 2026-06-14
Modulo: Canal Teams
Decision requerida: Confirmar owner, plataforma, repositorio, autenticacion,
  permisos, tenant, ambientes, DLP, despliegue y mecanismo de acciones del bot
  corporativo existente.
Impacto: Sin esta evidencia no puede validarse ni declararse completa la
  integracion corporativa del Bloque 9.

Stopper externo de hardening que debe conservarse:
Fecha: 2026-06-14
Modulo: Endurecimiento para piloto
Decision requerida: Confirmar UEMS, cuenta restringida, identidad de usuario y
  dispositivo, revision Security/Sophos, owner del kill switch, logs y
  retencion, respuesta ante compromiso, paquete/confianza de publicador y dos
  endpoints autorizados con criterio de restauracion.
Impacto: Sin esta evidencia no puede declararse `completed` el Bloque 10 ni
  iniciarse un piloto corporativo.

Gates externos futuros del Bloque 11:
- App Registration, tenant y contrato OIDC/Entra;
- owners y asignaciones reales de roles/scopes;
- matriz de segregacion de funciones;
- hosting, secretos, observabilidad y retencion;
- revision Security del portal y sesiones;
- OpenText/Rescue y sus mecanismos de enlace o adaptador;
- datos, ambientes y proceso de despliegue aprobados.

Estos gates no bloquean la primera unidad local sintetica.

Gate base:
.\scripts\Validate.ps1

Comandos adicionales:
corepack pnpm@11.5.3 audit --prod --audit-level high
.\scripts\Test-Secrets.ps1

Al terminar:
- informa alcance implementado, archivos, autorizacion y pruebas;
- lista riesgos y pendientes externos;
- actualiza `modules/ADMIN_PORTAL.md`,
  `docs/modules/admin-portal-foundation.md`, `WORKFLOW.md`,
  `CURRENT_CONTEXT.md`, `README.md` y el threat model si cambia la superficie;
- actualiza `CONSISTENCY_AUDIT.md` si cambia el estado o una frontera;
- recomienda un Conventional Commit;
- no hagas commit ni push automaticamente.
