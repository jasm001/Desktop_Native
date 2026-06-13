# Prompt maestro de continuidad

Usa el siguiente texto para iniciar una sesion limpia en la raiz de este
repositorio.

```text
Actua como ingeniero senior responsable de IT Support Native.

Repositorio:
- raiz: C:\Users\Ruruu\Documents\IT Support\_native-w11-product\development
- rama principal: main
- remoto: https://github.com/jasm001/Desktop_Native.git
- no hagas commit ni push automaticamente; recomienda un Conventional Commit y
  deja que el usuario publique los cambios.
- inspecciona el working tree antes de editar y conserva cualquier cambio
  existente.

Estado confirmado:
- Bloques 0 a 7 completados.
- Bloque 3 publicado en `b09f07a`.
- Bloque 4 publicado en `b56bfcb`.
- Bloque 5 publicado en `e3a0b8d`.
- Incremento automatizado del Bloque 6 publicado en `f808425`.
- Cierre documental y evidencia VM del Bloque 6 publicados en `bfb4a35`.
- Alineacion modular posterior del Bloque 6 publicada en `7fd18b6`.
- Fundacion del control plane del Bloque 7 publicada en `2b89a6b`.
- Cierre local del Bloque 7 publicado en `ed4789e`.
- La alineacion documental que declara al Bloque 8 como siguiente unidad esta
  en el commit local `cd9f91c`; verifica si ya fue publicado antes de editar.
- No hay otro bloque principal activo. El Bloque 8 esta `pending` y es el
  siguiente bloque desbloqueado.
- `src/AdminWeb` es un monolito modular Next.js App Router con TypeScript
  estricto, Prisma y PostgreSQL; no contiene portal administrativo.
- `src/Worker` es un proceso Node durable separado y ejecutable.
- El control plane tiene contratos HTTP v1 tipados para catalogo, solicitudes,
  estado, claim de agente y resultado.
- Prisma tiene dos migraciones versionadas:
  `20260613074457_control_plane_foundation` y
  `20260613183000_control_plane_local_flow`.
- La solicitud confirmada crea solicitud, trabajo, auditoria y outbox en una
  sola transaccion con idempotencia y correlacion.
- La auditoria es append-only y el worker procesa outbox con claim, lease,
  retry acotado y efectos sinteticos idempotentes.
- WinUI dispone de un cliente HTTP local, tipado y deshabilitado fuera del
  perfil exacto de desarrollo.
- El DeviceAgent inicia HTTP saliente hacia loopback, reclama solo la accion
  sintetica allowlisted y reporta evidencia saneada.
- El recorrido WinUI -> API -> worker/outbox -> agente simulado -> evidencia ->
  API -> WinUI pasa sobre PostgreSQL efimero real.
- La shell no ejecuta comandos privilegiados ni se conecta directamente al
  DeviceAgent.
- El DeviceAgent conserva Named Pipe IPC v1 deny-by-default, SQLite local,
  diagnosticos de solo lectura y el adaptador 7-Zip cerrado de `local-demo`.
- La matriz real del Bloque 6 paso en una VM Windows 11 desechable; el MSI no se
  ejecuto en el host.
- `modules/TICKETING.md` es el documento propietario del Bloque 8.
- Durante el Bloque 8, `ExternalTicket` es una representacion sintetica creada
  por un provider fake. No existe conexion OpenText real.
- Teams permanece reservado para el Bloque 9 y el portal administrativo para el
  Bloque 11.
- SDK fijado: .NET 10.0.301.
- `scripts/Validate.ps1` pasa completo: build Release sin warnings, 113 pruebas
  .NET, checks Node, migraciones/integracion PostgreSQL, E2E y secretos.
- Los lockfiles Desktop/WindowsUi estan limitados al RID `win-x64`.
- Visual Studio 2022 no admite net10.0; para depuracion en IDE se requiere
  Visual Studio 2026 version 18.0 o posterior.

Antes de editar:
1. inspecciona `git status`, `git log -5 --oneline`, tracking de `main` y
   conserva cambios existentes;
2. lee `README.md` y aplica su precedencia documental;
3. lee `CURRENT_CONTEXT.md`, `WORKFLOW.md` y `DEVELOPMENT_PLAN.md`;
4. lee `core/SCOPE.md`, `core/STACK.md`, `core/ARCHITECTURE.md`,
   `core/SECURITY.md` y `core/DECISIONS.md`;
5. lee `standards/CODING_STANDARDS.md` y `standards/DELIVERY.md`;
6. para el Bloque 8, lee completo `modules/TICKETING.md`,
   `modules/ASSISTANT.md`, `modules/CATALOG.md` y `modules/OPERATIONS.md`;
7. lee `docs/modules/control-plane-foundation.md`,
   `docs/modules/control-plane-local-flow.md`,
   `docs/modules/deterministic-conversation.md` y
   `docs/modules/local-mvp-lab.md`;
8. inspecciona `src/AdminWeb`, `src/Worker`, `src/Contracts/Web`,
   `src/Catalog`, `src/Conversation`, Prisma, migraciones y sus pruebas antes de
   crear contratos, dominios o infraestructura;
9. consulta `modules/ADMIN_PORTAL.md`, `modules/WEB_DELIVERY.md`,
   `modules/DEVICE_AGENT.md` y `docs/modules/desktop-shell.md` solo para
   proteger fronteras; no implementes los Bloques 9, 10 u 11;
10. revisa las rutas, repositorios, outbox y pruebas existentes del Bloque 7
    antes de decidir como enlazar `BotCase` con `SupportRequest`.

Reglas no negociables:
- no cambies stack, alcance, arquitectura, persistencia o fronteras de seguridad
  sin registrar un stopper en `WORKFLOW.md`;
- conserva Next.js/TypeScript/Prisma/PostgreSQL como control plane; no agregues
  otro backend ASP.NET;
- conserva el worker Node como proceso durable separado;
- Prisma pertenece solo a infraestructura server-side; HTTP y outbox no exponen
  entidades de persistencia;
- usa una migracion Prisma aditiva y versionada, `migrate deploy` y PostgreSQL
  real efimero; no uses SQLite ni `db push` como evidencia final;
- la auditoria sigue append-only y el outbox se escribe en la misma transaccion
  que el cambio de negocio;
- `BotCase` es el caso operativo interno; `SupportRequest`, `ExecutionJob` y
  `ExternalTicket` son conceptos distintos y deben conservar correlacion
  explicita;
- una consulta informativa nunca crea `BotCase`, solicitud, trabajo, ticket ni
  outbox;
- toda mutacion requiere contrato tipado, confirmacion explicita, identidad de
  desarrollo, correlacion e idempotencia;
- repetir la misma idempotency key con el mismo payload reutiliza el resultado;
  usarla con otro payload falla con error tipado;
- `ITicketingProvider` debe ser una interfaz de aplicacion y su primera
  implementacion debe ser fake, determinista, local e idempotente;
- el fake no llama OpenText, no usa red, no lee configuracion corporativa y no
  inventa IDs, usuarios, sedes, grupos o endpoints reales;
- no implementes login corporativo, Entra, MFA, roles productivos ni secretos;
- no copies payloads libres, excepciones, logs completos, comandos, rutas,
  hostnames, IP, credenciales, recovery keys ni PII innecesaria a casos,
  auditoria o tickets;
- los errores, descripciones y evidencia usan codigos y textos saneados con
  limites estrictos;
- el backend no ejecuta comandos ni se conecta directamente a procesos
  privilegiados;
- WinUI, Teams, IA y portal nunca ejecutan comandos privilegiados;
- no cambies IPC, allowlists, adaptadores reales ni comportamiento funcional del
  Bloque 6 salvo que una prueba de regresion exija una correccion separada;
- no adelantes Teams, portal administrativo, Hermes/RAG, Windows Service, UEMS
  real, OpenText real, Rescue, telemetria productiva ni endurecimiento de piloto;
- manten el nucleo de casos y politica de cierre comprobable sin dependencia de
  WinUI, Prisma o providers concretos;
- fija dependencias y lockfile; no agregues librerias si las APIs existentes son
  suficientes;
- corrige causas, no parches aislados, y actualiza pruebas y documentacion;
- no declares terminado un cambio sin build y pruebas aplicables.

Tarea:
Inicia el Bloque 8 con un primer incremento coherente de fundacion de casos y
ticketing fake. No intentes completar todo el bloque en un solo cambio.

Antes de implementar:
- audita el flujo confirmado y el evento de resultado del Bloque 7;
- define y documenta el alcance exacto del primer incremento en un documento
  propietario bajo `docs/modules/`;
- confirma la relacion entre `SupportRequest`, `ExecutionJob`, `BotCase` y
  `ExternalTicket`;
- registra un stopper si la implementacion exige cambiar las fronteras o
  semantica establecidas.

Implementa como alcance minimo esperado:
- dominio tipado inicial de `BotCase` con identidad, correlacion, solicitud
  relacionada, categoria, estado, resultado y timestamps;
- estados minimos para distinguir caso abierto, atendido esperando usuario y
  escalado; no inventes todo el workflow productivo de OpenText;
- politica pura y comprobable de elegibilidad para cierre por falta de respuesta
  a las 72 horas, sin agregar todavia scheduler, cron ni notificaciones;
- creacion de exactamente un `BotCase` para una mutacion operativa confirmada,
  en la misma transaccion que la solicitud/auditoria/outbox correspondiente;
- actualizacion idempotente del caso cuando llega un resultado sintetico:
  exito -> `attended_waiting_user`; fallo -> `escalated`;
- contrato/evento tipado para solicitar escalamiento por outbox;
- `ITicketingProvider` y un provider fake determinista que produzca un
  `ExternalTicket` sintetico solo para el escalamiento;
- persistencia Prisma/PostgreSQL mediante una nueva migracion aditiva;
- consultas HTTP v1 acotadas y sin efectos laterales para obtener el caso y su
  ticket fake cuando exista;
- worker capaz de procesar el escalamiento fake con claim/retry existente y sin
  duplicar tickets;
- auditoria y payloads saneados, sin texto libre operativo ni datos
  corporativos;
- pruebas unitarias, de contrato e integracion sobre PostgreSQL efimero real;
- documentacion propietaria y actualizacion de `WORKFLOW.md`,
  `CURRENT_CONTEXT.md`, `DEVELOPMENT_PLAN.md` y `modules/TICKETING.md`.

Decisiones que debes validar contra el codigo antes de fijarlas:
- si el `BotCase` se crea dentro de la mutacion confirmada existente o mediante
  una nueva mutacion confirmada, el resultado debe seguir siendo una sola
  solicitud y un solo caso por idempotency key;
- el provider fake debe ejecutarse desde el worker/outbox, no desde una consulta
  HTTP ni desde WinUI;
- el primer incremento no debe implementar cierre automatico; solo el modelo y
  la politica determinista de 72 horas.

Criterio de aceptacion del primer incremento:
- Bloques 6 y 7 permanecen `completed` y sin regresiones funcionales;
- Bloque 8 pasa de `pending` a `in_progress`; ningun otro bloque principal queda
  activo;
- una consulta de catalogo, solicitud o caso no crea `BotCase`,
  `ExternalTicket`, auditoria nueva ni outbox;
- una confirmacion operativa crea una sola solicitud y un solo `BotCase`
  correlacionado;
- repetir la misma idempotency key no duplica solicitud, trabajo, caso,
  auditoria, outbox ni ticket fake;
- reutilizar la clave con otro payload falla con `idempotency_conflict`;
- un resultado exitoso deja el caso en `attended_waiting_user` sin crear ticket
  de escalamiento;
- un resultado fallido deja el caso en `escalated`, escribe auditoria y outbox
  en la misma transaccion y el worker crea exactamente un ticket fake;
- reintentos del worker o del evento no duplican el ticket fake;
- el ticket fake conserva referencia sintetica, categoria, correlacion y
  descripcion saneada, sin secretos ni datos corporativos;
- la politica de 72 horas se prueba en limites antes/despues del umbral usando
  tiempo inyectable o timestamps gobernados por PostgreSQL;
- los contratos rechazan comandos, scripts, rutas, logs, descripciones libres
  no acotadas y campos desconocidos;
- el E2E y los contratos del Bloque 7 siguen pasando;
- no hay conexion OpenText real, Teams, portal, Entra, UEMS, Hermes/RAG,
  Windows Service nuevo ni cambios del DeviceAgent;
- build, lint, typecheck, pruebas, migracion de integracion, workspace,
  auditoria de dependencias y escaneo de secretos pasan;
- se informa el incremento completado, archivos, migracion, contratos, pruebas,
  riesgos residuales, estado documental y Conventional Commit sugerido;
- el Bloque 8 permanece `in_progress` al terminar este primer incremento; no se
  declara `completed` hasta cerrar el resto de su gate en unidades posteriores.

Gate base:
.\scripts\Validate.ps1

Comandos equivalentes relevantes:
dotnet restore ITSupportNative.slnx --locked-mode -m:1 --disable-build-servers
dotnet format ITSupportNative.slnx --verify-no-changes --no-restore
dotnet restore ITSupportNative.slnx --locked-mode -m:1 `
  --artifacts-path .artifacts/validate --disable-build-servers
dotnet build ITSupportNative.slnx --configuration Release --no-restore -m:1 `
  --artifacts-path .artifacts/validate --disable-build-servers
dotnet test ITSupportNative.slnx --configuration Release --no-build -m:1 `
  --artifacts-path .artifacts/validate --disable-build-servers
corepack pnpm@11.5.3 install --frozen-lockfile
corepack pnpm@11.5.3 run check
corepack pnpm@11.5.3 run test:integration
corepack pnpm@11.5.3 audit --prod --audit-level high
.\scripts\Test-Secrets.ps1

Agrega a los gates existentes solo los comandos reproducibles que necesite el
incremento. Si el entorno no dispone de PostgreSQL utilizable, investiga las
opciones locales y registra un stopper; no degrada silenciosamente a SQLite o
mocks como evidencia final.

Si detectas una contradiccion, aplica la precedencia de `README.md`, registra la
evidencia y no decidas silenciosamente.
```
