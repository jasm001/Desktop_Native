# Threat model

## Estado

Artefacto de trabajo del Bloque 10 `blocked`. Fue contrastado localmente con
codigo, configuracion y pruebas, pero no representa revision ni aprobacion de
Security.

## Alcance

Componentes incluidos:

- WinUI 3 sin privilegios;
- `DeviceAgent` y su IPC Named Pipe versionado;
- control plane Next.js, PostgreSQL y Prisma;
- worker Node, leases y outbox;
- contratos HTTP e IPC;
- artefactos y adaptador cerrado de laboratorio;
- canales WinUI y Teams recorded;
- auditoria, evidencia y datos sinteticos.

Fuera del alcance conectado actual:

- UEMS, Entra, Sophos, PKI y confianza corporativa;
- OpenText, Teams y Microsoft 365 reales;
- IA productiva y RAG con datos corporativos;
- autenticacion, sesiones, mutaciones e integraciones productivas del portal
  administrativo.

Las rutas locales `/admin/*` del Bloque 11 se incorporan solo para registrar su
frontera de identidad, autorizacion y lectura limitada. No se usan como
evidencia para cerrar el Bloque 10.

## Activos

- identidad de usuario, dispositivo y servicio;
- autorizaciones, confirmaciones e idempotency keys;
- catalogo, solicitudes, trabajos y casos;
- manifiestos, hashes, paquetes y politica allowlisted;
- auditoria append-only, outbox y evidencia;
- configuracion, tokens y secretos futuros;
- capacidad de deshabilitar y retirar el agente.

## Fronteras de confianza

1. Usuario y WinUI.
2. WinUI y control plane.
3. WinUI y DeviceAgent mediante Named Pipe.
4. Control plane, PostgreSQL y worker.
5. DeviceAgent y proveedor de artefactos.
6. Canales conversacionales y aplicacion compartida.
7. Entorno `local-demo` y un futuro entorno piloto.
8. Owners corporativos y mecanismos externos todavia no conectados.

## Inventario trazable

| Amenaza | Prioridad | Codigo, configuracion y pruebas existentes | Brecha o evidencia pendiente |
| --- | --- | --- | --- |
| Suplantacion de agente o dispositivo | critica | Identidad local cerrada en `development-agent-identity.ts`; claim vinculado a `deviceId`, `agentSubject`, token hasheado y lease en `prisma-agent-job-repository.ts`; integracion PostgreSQL valida claims | Bootstrap, identidad unica, rotacion, revocacion y vinculacion corporativa |
| Elevacion por Named Pipe | critica | `NamedPipeAgentWorker` usa `CurrentUserOnly`; `AgentPipeFraming` limita 64 KiB; `AgentRequestDispatcher` valida version y message type; pruebas de IPC real y contratos sin command/argument/path | ACL e identidad cliente/servicio con cuenta restringida en dos endpoints |
| Replay o duplicacion de trabajos | alta | `AgentJobService`, restricciones unicas PostgreSQL, advisory locks, idempotency hash, claim lease y resultado idempotente; pruebas locales, HTTP e integracion | Replay y recuperacion ensayados bajo identidad y red del piloto |
| Paquete sustituido o repositorio comprometido | critica | `LocalDevelopmentArtifactSource` restringe raiz, longitud y SHA-256; manifiesto versionado; adaptador fija producto, version y argumentos; pruebas de hash, longitud y mirror | Firma/confianza de publicador, origen UEMS y respuesta ante compromiso |
| Downgrade del agente o contrato | alta | IPC y canal conversacional rechazan versiones desconocidas; adaptador y allowlist fijan versiones exactas; `DeviceAgentConfigurationPolicy` rechaza perfiles y combinaciones no soportados al iniciar | Version minima del agente, politica firmada y rollback aprobado |
| Abuso de contenido conversacional | alta | Contratos C#/Zod estrictos, acciones enumeradas y rechazo de campos desconocidos; Teams recorded y WinUI solo llaman aplicacion/control plane | Validacion del bot corporativo real y su identidad |
| Fuga por logs o evidencia | alta | Evidencia usa codigos y resumenes fijos; excepciones de diagnostico y ejecucion se sanean; eventos de runtime 1001, 2001 y 3001 usan mensajes fijos sin objeto `Exception` ni payload; pruebas verifican el contenido exacto | Inventario operativo, retencion, acceso, alertas y revision Security |
| Perdida de red durante una operacion | alta | SQLite conserva trabajos; estados `running` vuelven a `queued`; control plane usa leases, retries e idempotencia; pruebas de recuperacion y claims agotados | Ensayo de interrupcion antes/durante/despues en dos endpoints |
| Manipulacion de auditoria u outbox | alta | Trigger PostgreSQL impide update/delete de auditoria; request/job/case/outbox son transaccionales; worker usa claims y retries acotados; integraciones verifican append-only | Acceso operativo, alertas, backup y retencion |
| Integracion de tickets comprometida | alta | `ITicketingProvider` fake separado, evento v1 estricto, ticket unico por caso y descripcion acotada; pruebas de retry | OpenText real: autenticacion, rate limits, sandbox, soporte y revocacion |
| Administrador interno abusivo | alta | Las rutas `/admin/*` exigen ambiente `development`, flag explicito, rol local conocido y capability de lectura antes de resolver datos; principal separado del usuario de API y del agente; consultas limitadas a 25 registros, selecciones explicitas sin payload de auditoria y sin mutaciones; pruebas rechazan ambientes, roles y capabilities desconocidos y verifican ausencia de efectos laterales | OIDC/Entra, sesiones, RBAC/scopes reales, segregacion, alertas, auditoria de mutaciones y Playwright por rol |
| Exclusion antivirus excesiva | alta | `core/SECURITY.md` prohibe exclusion general; no existe configuracion de exclusion en codigo o deploy | Resultado Sophos; excepcion especifica y expirable solo si fuera necesaria |
| Promocion accidental de `local-demo` | alta | Perfil `disabled` por defecto; politica de inicio acepta `local-demo` solo en `Development` y rechaza capacidades habilitadas con perfil `disabled`; URL limitada a loopback; manifiesto `development-only`; pruebas focalizadas | Configuracion administrada, paquete por ambiente y perfil empresarial explicito |
| Deshabilitacion o retiro incompletos | alta | `JobExecutionEnabled=false` por defecto; `AgentJobExecutionGate` bloquea admision, claim e inicio/reanudacion; pruebas focalizadas; runbook local | Owner, configuracion protegida, revocacion, retencion y ensayo UEMS en dos endpoints |

## Mitigaciones locales del Bloque 10

La unidad `docs/modules/pilot-hardening-local-kill-switch.md` cierra la brecha
local de admision durante una deshabilitacion:

- configuracion fail-closed;
- ningun trabajo nuevo por IPC;
- ningun claim del control plane;
- ningun inicio o reanudacion de trabajos en cola;
- diagnostico, consulta y cancelacion conservados;
- sin cambios a contratos, allowlist, privilegios o datos.

Riesgo residual: el switch se carga al iniciar, no interrumpe un adaptador que
ya entro en ejecucion y aun no tiene owner, distribucion o proteccion
corporativa.

La unidad
`docs/modules/pilot-hardening-local-profile-confinement.md` cierra el margen
local de configuracion contradictoria y exposicion generica de excepciones:

- perfiles desconocidos fallan al iniciar;
- `local-demo` queda confinado a `Development`;
- las capacidades no pueden habilitarse con perfil `disabled`;
- los fallos de workers usan eventos y mensajes fijos sin detalles internos.

El inventario de cierre y los puntos de sustitucion viven en
`docs/modules/pilot-hardening-local-closure.md`.

## Criterios de revision

Antes de un piloto en dos endpoints se debe:

1. contrastar cada control con codigo, configuracion o prueba reproducible;
2. asignar owner y severidad a cada brecha;
3. registrar un stopper si la mitigacion cambia una frontera normativa;
4. validar cuenta restringida, despliegue, actualizacion, rollback y retiro;
5. verificar kill switch, revocacion y recuperacion tras perdida de red;
6. revisar logs, evidencia, PII, secretos y retencion;
7. confirmar paquete, firma o confianza del publicador y respuesta de Sophos;
8. obtener revision de Security y de los owners operativos aplicables.

El gate y las restricciones del bloque viven en
`../../modules/PILOT_HARDENING.md`.

El contraste local esta completo. La revision Security, los owners, la
configuracion administrada, la identidad restringida, UEMS, la confianza del
publicador, la retencion y el ensayo en dos endpoints son evidencia externa; por
eso el Bloque 10 esta `blocked` y no `completed`.
