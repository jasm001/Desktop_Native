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
- La segunda unidad local del Bloque 11 esta publicada en `17e7581`.
- La tercera unidad local del Bloque 11 esta publicada en `7a65f3e`; agrego
  Testing Library/jsdom, Playwright y el gate local de calidad del portal.
- La cuarta unidad local del Bloque 11 esta publicada en `212f274`; cerro el
  esqueleto local `/admin/*` con rutas protegidas y sinteticas
  `/admin/access`, `/admin/approvals`, `/admin/support` y `/admin/reporting`.
- La quinta unidad local del Bloque 11 esta publicada en `dc7ad98`; agrego
  `/admin/lab` de solo lectura con categoria `lab-real-sanitized`, datos reales
  de laboratorio saneados y persistidos, sin secretos ni mutaciones.
- La sexta unidad local del Bloque 11 esta publicada en `3d6b7bd`; implemento
  la Unidad 3 del roadmap con `AdminLabHealthProvider`, health local para
  Hermes, mirror de artefactos, bridge `validate-only` y ticketing fake.
- Las Unidades 4 y 5 del roadmap local de laboratorio estan implementadas como
  incrementos locales pendientes de publicacion: catalogo local curado y
  recorrido end-to-end visual por correlacion. No agregan migraciones,
  mutaciones administrativas, instaladores en Git ni integraciones
  corporativas.
- La unidad local del asistente WinUI esta publicada en `cfa3342`; agrego chat
  visual en memoria para Hermes local, indicador de respuesta, autoscroll y
  envio con Enter, sin persistencia ni acciones.
- El ajuste local de Hermes esta publicado en `eb99434`; elevo a 120 segundos
  el timeout cliente de WinUI para permitir skills locales lentas.
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
- `/admin`, `/admin/catalog`, `/admin/operations`, `/admin/audit`,
  `/admin/access`, `/admin/approvals`, `/admin/support`, `/admin/reporting` y
  `/admin/lab` contienen identidad de portal sintetica separada, autorizacion
  server-side fail-closed, navegacion real, capabilities de lectura separadas,
  lecturas limitadas, estados sinteticos en memoria, datos reales de laboratorio
  saneados, health local de conectores, catalogo local curado y trazas
  end-to-end por correlacion con evidencia saneada e idempotencia.
- Testing Library/jsdom cubre componentes del portal local. Playwright cubre
  escritorio, movil, teclado, estado activo, acceso denegado, solo lectura y
  ausencia de overflow horizontal para la superficie administrativa actual; el
  reintento focalizado de Playwright administrativo de la Unidad 3 quedo
  pendiente por restricciones de sandbox/red tras un fallo HMR de ejecucion
  paralela.
- No existen aun OIDC/Entra, sesiones productivas, RBAC productivo, mutaciones,
  Fluent UI, roles productivos, owners ni scopes corporativos.
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
  RAG productivo y portal administrativo productivo siguen deshabilitados.
- `docs/modules/local-mvp-lab.md` documenta el laboratorio personal/local:
  VM Windows 11, datos publicos o sinteticos, proveedores fake/locales, mirror
  local, Hermes local opcional, RAG local curado pendiente y perfil
  `local-demo`. No acredita piloto ni produccion.
- `docs/modules/local-lab-real-data-roadmap.md` documenta cinco unidades para
  pasar de muestras a datos reales de laboratorio saneados; las cinco unidades
  estan implementadas localmente como superficies de solo lectura. El siguiente
  roadmap recomendado debe ser separado para WinUI, consumiendo estos datos por
  APIs/contratos de solo lectura sin mezclar el gobierno del portal.
- `docs/modules/admin-portal-lab-real-read-model.md` documenta las Unidades 1 y
  2 del portal de laboratorio real saneado.
- `docs/modules/admin-portal-lab-health-connectors.md` documenta la Unidad 3:
  health local reemplazable, fail-closed y sin exposicion de secretos.
- `docs/modules/admin-portal-local-lab-catalog.md` documenta la Unidad 4:
  catalogo local curado con manifiestos development-only, licencia
  redistribuible, origen publico, version fija, SHA-256 y estados de artefacto.
- `docs/modules/admin-portal-lab-e2e-visual-demo.md` documenta la Unidad 5:
  trazas por `correlationId`, etapas WinUI/API/worker/agente/evidencia/caso/
  ticket fake, outbox/effects e idempotencia, sin payloads completos ni piloto.
- El laboratorio externo Bunny Bridge queda documentado en
  `docs/modules/lab-bridge-reuse-notes.md` solo como patron reutilizable:
  bridge local `validate-only`, requester/hostname/software allowlisted,
  gateway y separacion de secretos. Sus dominios, hostnames, usuarios y UEMS
  trial simulan una operacion realista, pero no son supuestos productivos ni
  desbloquean UEMS, Teams, Entra, Power Automate productivo o el Bloque 10.
- WinUI puede habilitar Hermes local temporalmente mediante variables de
  entorno para texto libre informativo, sin acciones, sin datos corporativos,
  sin solicitudes, sin tickets, sin auditoria, sin outbox, sin llamadas al
  DeviceAgent y con historial visual solo en memoria. El procedimiento vive en
  `docs/runbooks/local-hermes-chat.md`.
- Hermes local usa un endpoint compatible con OpenAI limitado a loopback. La
  app envia `Authorization: Bearer <key>` a `/chat/completions`, modelo
  `it-support`, timeout cliente de 120 segundos. No guardes API keys en archivos
  del repositorio, `.env`, notas, capturas ni logs compartidos;
  `scripts/Test-Secrets.ps1` escanea tambien archivos ignorados.
- `SQLitePCLRaw.bundle_e_sqlite3` esta fijado en `3.0.3` mediante Central
  Package Management para evitar la vulnerabilidad alta reportada por NuGet
  Audit en la transitiva `SQLitePCLRaw.lib_e_sqlite3` 2.1.11.
- Ultimo gate completo publicado: 140 pruebas .NET, 53 pruebas Node
  unitarias/de contrato/componente, 13 integraciones AdminWeb, 4 del Worker,
  cuatro migraciones PostgreSQL, E2E local y escaneo de secretos correctos. El
  portal conserva recorridos Playwright locales para nueve rutas administrativas;
  la auditoria `pnpm audit --prod --audit-level high` de la Unidad 3 no pudo
  repetirse por red restringida del sandbox y no hubo cambios de dependencias.
- Ultima validacion focalizada: 2026-07-05, build Release, 140 pruebas .NET,
  `dotnet format` y `scripts/Test-Secrets.ps1` correctos para la unidad del
  asistente WinUI y el timeout cliente de Hermes.
- Ultima validacion local del portal: 2026-07-06, `AdminWeb` lint/typecheck,
  45 pruebas unitarias/componente del paquete y HTTP `GET /admin/lab` 200 con
  traza visible. La integracion directa `control-plane.integration.test.ts`
  queda pendiente porque el setup choco con el trigger append-only de
  `audit_events`. El reintento focalizado `.NET` de `SevenZipAdapterTests`
  queda pendiente porque `dotnet test` salio con codigo 1 despues de restore
  sin diagnostico util; no forzar ni deshabilitar controles para hacerlo pasar.
- `scripts/Validate.ps1`, auditoria de dependencias y escaneo de secretos pasan.
- `context/` y `reference/` conservan historia y no definen el estado actual.

Precedencia documental:
- Aplica la precedencia de `README.md`.
- `MASTER_PROMPT.md` es handoff, no fuente normativa.
- Si una peticion cambia stack, alcance, arquitectura, persistencia, seguridad,
  contratos publicos o estados de bloque, registra un stopper en `WORKFLOW.md`
  antes de implementar.

Antes de editar:
1. ejecuta `git status --branch --short`, `git log -5 --oneline` y verifica el
   tracking de `main`;
2. lee completos `README.md`, `CURRENT_CONTEXT.md`, `WORKFLOW.md`,
   `DEVELOPMENT_PLAN.md` y `CONSISTENCY_AUDIT.md`;
3. aplica la precedencia documental de `README.md`;
4. lee los documentos normativos y modulares relevantes para la unidad:
   `core/SECURITY.md`, `core/DECISIONS.md`, `core/SCOPE.md`,
   `core/STACK.md`, `core/ARCHITECTURE.md`, `standards/`,
   el documento propietario bajo `modules/` y los documentos en `docs/modules/`
   citados por la unidad;
5. si la unidad toca AdminWeb, inspecciona `src/AdminWeb/package.json`,
   `src/AdminWeb/src/app`, `src/AdminWeb/src/modules/identity`,
   `src/AdminWeb/src/platform`, `src/AdminWeb/tests`, `src/AdminWeb/prisma`,
   `src/Contracts/Web` y `src/Worker`;
6. confirma que pagina actual, dependencias, identidad, capabilities y pruebas
   coinciden con la linea base documentada antes de elegir implementacion.

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
- identidades, roles, scopes, capabilities, payloads o versiones desconocidos
  se rechazan;
- una identidad de desarrollo solo puede habilitarse con ambiente local exacto
  y configuracion explicita;
- `DeveloperAllAccess` nunca se acepta fuera de desarrollo;
- toda mutacion futura requiere identidad, autorizacion, confirmacion,
  correlacion, idempotencia y auditoria;
- no uses datos, credenciales, endpoints, tenant, certificados ni
  identificadores corporativos;
- no escribas API keys ni tokens locales en archivos del repo; usa variables de
  entorno de sesion y placeholders como `<api-key-local>`;
- no conectes Entra, Microsoft 365, Teams, OpenText, Rescue, UEMS, Sophos, PKI
  ni servicios productivos;
- no heredes rutas internas `/dcapi`, cookies, CSRF, headers de sesion,
  credenciales humanas, IDs concretos de UEMS ni afirmaciones de instalacion
  real desde el laboratorio Bunny Bridge;
- no inventes owners, asignaciones de rol, scopes corporativos, cuentas de
  servicio, permisos, retencion, excepciones ni mecanismos de despliegue;
- los roles productivos de `modules/ADMIN_PORTAL.md` son un modelo objetivo, no
  asignaciones confirmadas;
- no conviertas la identidad sintetica en autenticacion productiva;
- no reabras ni declares completados los Bloques 9 o 10;
- no declares completado el Bloque 11 mientras falte cualquier gate de su
  documento propietario;
- no agregues dependencias salvo las aprobadas por `core/STACK.md` y necesarias
  para la unidad; conserva versiones fijadas y lockfile reproducible;
- no declares terminada una unidad sin ejecutar los gates aplicables.

Direccion para los siguientes chats:
- Continuar con capacidades de laboratorio personal/local y reemplazables, no
  conexiones productivas.
- Usar `docs/modules/local-mvp-lab.md` y
  `docs/modules/lab-bridge-reuse-notes.md` como base para cualquier puente,
  gateway, adaptador, RAG, Hermes, UEMS trial, Power Automate/Copilot Studio de
  laboratorio o recorrido end-to-end personal.
- Toda integracion de laboratorio debe ser explicita, `Development`/`local-demo`
  only, fail-closed, sin secretos en repo, sin datos corporativos, con
  allowlists de requester/hostname/software, confirmacion explicita,
  correlacion, idempotencia y auditoria antes de cualquier accion.
- El modo `validate-only` es aceptable y preferible cuando no hay endpoint
  soportado. No afirmar despliegue o instalacion real si el proveedor no acepto
  la orden.
- Mantener proveedores detras de interfaces/adaptadores reemplazables. No
  acoplar dominio, portal, Teams, IA o Power Automate directamente a UEMS,
  DeviceAgent o servicios corporativos.
- Si se usa el laboratorio Bunny Bridge externo, tratarlo como referencia de
  patron y evidencia saneada. No copiar `.env`, cookies, CSRF, headers de
  sesion, claves, hostnames productivos, tenant ni rutas internas.
- El objetivo inmediato no es cerrar Bloques 9, 10 u 11 completos; es preparar
  ciclos de laboratorio seguros y documentados que luego puedan sustituirse por
  proveedores corporativos aprobados.

Configuracion local opcional de Hermes para demo WinUI:
- Usa solo variables de entorno de sesion:
  `$env:IT_SUPPORT_HERMES_CHAT_ENABLED='true'`,
  `$env:IT_SUPPORT_HERMES_BASE_URL='http://127.0.0.1:8765/v1'`,
  `$env:IT_SUPPORT_HERMES_MODEL='it-support'`,
  `$env:IT_SUPPORT_HERMES_API_KEY='<api-key-local>'`.
- La app llama internamente a `/chat/completions` bajo la base URL configurada.
- La app envia `Authorization: Bearer <key>`, no `x-api-key`.
- El modelo esperado de prueba es `it-support`.
- El endpoint debe ser `http` o `https` de loopback; endpoints externos se
  rechazan.
- Timeout actual de la app: 120 segundos; este limite es del lado de WinUI, no
  del gateway Hermes.
- Con Hermes apagado o mal configurado, el campo de texto libre queda
  deshabilitado y el asistente conserva las opciones deterministas.
- Hermes solo puede responder orientacion informativa; no autoriza ni ejecuta
  acciones y no alimenta comandos del agente.

Tareas sugeridas para el siguiente chat:
1. Si se quiere cerrar la validacion pendiente, diagnosticar sin forzar:
   - `dotnet test tests\Integration\ITSupportNative.IntegrationTests.csproj
     --filter FullyQualifiedName~SevenZipAdapterTests --verbosity minimal`;
   - la integracion AdminWeb directa
     `corepack pnpm@11.5.3 --filter @it-support-native/admin-web exec vitest
     run tests/integration/control-plane.integration.test.ts`, que requiere un
     harness limpio porque el trigger append-only de `audit_events` puede
     bloquear la limpieza.
2. No deshabilitar triggers append-only, no relajar idempotencia, no modificar
   DeviceAgent ni cambiar controles solo para hacer pasar esas pruebas.
3. Si se sigue con producto, crear un roadmap separado para WinUI y datos reales
   de laboratorio, como recomienda
   `docs/modules/local-lab-real-data-roadmap.md`: la app nativa debe consumir
   APIs/contratos de solo lectura y no mezclar gobierno del portal con la
   experiencia nativa.
4. Mantener Bloques 9 y 10 `blocked`; Bloque 11 sigue `in_progress`.
5. No declarar completo un bloque por evidencia de laboratorio personal.

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

Gate base:
.\scripts\Validate.ps1

Comandos adicionales:
corepack pnpm@11.5.3 audit --prod --audit-level high
.\scripts\Test-Secrets.ps1

Al terminar una unidad:
- informa alcance implementado, archivos, autorizacion y pruebas;
- lista riesgos y pendientes externos;
- actualiza documentos propietarios y `WORKFLOW.md`, `CURRENT_CONTEXT.md`,
  `README.md` y threat model si cambia una superficie;
- actualiza `CONSISTENCY_AUDIT.md` si cambia el estado o una frontera;
- recomienda un Conventional Commit;
- no hagas commit ni push automaticamente.
