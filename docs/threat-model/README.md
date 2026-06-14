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

## Amenazas y controles actuales

| Amenaza | Control existente | Evidencia pendiente |
| --- | --- | --- |
| Suplantacion de agente o dispositivo | Contratos tipados y perfil local; sin identidad corporativa | Bootstrap, rotacion, revocacion y vinculacion reales |
| Elevacion por Named Pipe | ACL del usuario actual, version y allowlist exacta | Validacion con cuenta restringida del servicio |
| Replay o duplicacion de trabajos | Correlacion, idempotencia y restricciones unicas | Prueba de recuperacion y replay en dos endpoints |
| Paquete sustituido | Version, longitud y SHA-256 fijados en `local-demo` | Firma/confianza de publicador y proceso UEMS |
| Downgrade del agente o contrato | Versiones desconocidas fallan cerradas | Politica de version minima y rollback aprobado |
| Abuso de contenido conversacional | Esquemas estrictos; sin comandos, scripts ni rutas | Validacion del bot corporativo real |
| Fuga por logs o evidencia | Resultados tipados y saneados; secretos prohibidos | Inventario de eventos, retencion y revision de Security |
| Perdida de red durante una operacion | Estados durables, SQLite local y sincronizacion idempotente | Ensayo de interrupcion y recuperacion |
| Manipulacion de auditoria u outbox | Auditoria append-only y escritura transaccional | Acceso operativo, alertas y retencion |
| Integracion de tickets comprometida | Provider fake separado y payload tipado | Autenticacion, rate limits y ambiente OpenText |
| Administrador interno abusivo | Portal administrativo aun no implementado | RBAC, segregacion, alertas y revision del Bloque 11 |
| Exclusion antivirus excesiva | Se prohibe exclusion general | Resultado Sophos y excepcion acotada si fuera necesaria |
| Promocion accidental de `local-demo` | Providers y politica de desarrollo separados | Gate de configuracion y paquete por ambiente |
| Deshabilitacion o retiro incompletos | Cancelacion y desinstalacion acotadas | Kill switch, revocacion y runbook ensayado |

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
