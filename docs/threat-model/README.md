# Threat model

## Estado

Artefacto de trabajo del Bloque 10 `in_progress`. Se basa en las capacidades
implementadas hasta el incremento local del Bloque 9 y no representa revision
ni aprobacion de Security.

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
- portal administrativo del Bloque 11.

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
| Downgrade del agente o contrato | alta | IPC y canal conversacional rechazan versiones desconocidas; adaptador y allowlist fijan versiones exactas; pruebas de contrato | Version minima del agente, politica firmada y rollback aprobado |
| Abuso de contenido conversacional | alta | Contratos C#/Zod estrictos, acciones enumeradas y rechazo de campos desconocidos; Teams recorded y WinUI solo llaman aplicacion/control plane | Validacion del bot corporativo real y su identidad |
| Fuga por logs o evidencia | alta | Evidencia usa codigos y resumenes fijos; excepciones de diagnostico y ejecucion se sanean; logs del agente omiten payloads; pruebas buscan datos sensibles | Inventario completo de eventos, retencion, acceso y revision Security |
| Perdida de red durante una operacion | alta | SQLite conserva trabajos; estados `running` vuelven a `queued`; control plane usa leases, retries e idempotencia; pruebas de recuperacion y claims agotados | Ensayo de interrupcion antes/durante/despues en dos endpoints |
| Manipulacion de auditoria u outbox | alta | Trigger PostgreSQL impide update/delete de auditoria; request/job/case/outbox son transaccionales; worker usa claims y retries acotados; integraciones verifican append-only | Acceso operativo, alertas, backup y retencion |
| Integracion de tickets comprometida | alta | `ITicketingProvider` fake separado, evento v1 estricto, ticket unico por caso y descripcion acotada; pruebas de retry | OpenText real: autenticacion, rate limits, sandbox, soporte y revocacion |
| Administrador interno abusivo | alta | Portal del Bloque 11 no implementado; control plane actual solo expone flujos locales acotados | RBAC, segregacion, alertas y revision del Bloque 11 |
| Exclusion antivirus excesiva | alta | `core/SECURITY.md` prohibe exclusion general; no existe configuracion de exclusion en codigo o deploy | Resultado Sophos; excepcion especifica y expirable solo si fuera necesaria |
| Promocion accidental de `local-demo` | alta | Perfil `disabled` por defecto; URL de agente limitada a loopback; manifiesto `development-only`; 7-Zip falla fuera de `local-demo`; pruebas de perfil | Configuracion administrada, paquete por ambiente y bloqueo de promocion |
| Deshabilitacion o retiro incompletos | alta | `JobExecutionEnabled=false` por defecto; `AgentJobExecutionGate` bloquea admision, claim e inicio/reanudacion; pruebas focalizadas; runbook local | Owner, configuracion protegida, revocacion, retencion y ensayo UEMS en dos endpoints |

## Primera mitigacion del Bloque 10

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
