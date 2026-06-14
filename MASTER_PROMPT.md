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
- El incremento local del Bloque 9 esta publicado en `0448a42`.
- Bloque 9, canal Teams existente, esta `blocked` hasta disponer de evidencia
  para integrar y validar el bot corporativo real.
- `modules/TEAMS.md` conserva el gobierno del Bloque 9.
- Bloque 10, endurecimiento para piloto, esta `in_progress` y es el unico bloque
  principal activo.
- `modules/PILOT_HARDENING.md` es el documento propietario del Bloque 10.
- `docs/threat-model/README.md` es el threat model de trabajo y aun no tiene
  revision ni aprobacion de Security.
- Bloque 11, portal administrativo web, permanece `pending`.
- `src/AdminWeb` es el control plane Next.js modular, no el portal del
  Bloque 11.
- `src/Worker` es un proceso Node durable separado.
- Prisma/PostgreSQL tiene cuatro migraciones versionadas.
- WinUI y DeviceAgent conservan IPC tipado, allowlist cerrada, idempotencia,
  evidencia saneada y recuperacion local.
- Cada confirmacion crea una `SupportRequest`, un `ExecutionJob` y un `BotCase`.
- Los fallos publican `bot-case.escalation-requested.v1`; el worker crea un solo
  `ExternalTicket` fake por caso.
- Teams local usa `conversation-channel.v1`,
  `IConversationChannel` y un adaptador recorded sin red.
- OpenText real, Teams corporativo, Entra, UEMS, Sophos, PKI, Hermes/RAG
  productivo y portal administrativo siguen deshabilitados.
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
6. lee completos `modules/PILOT_HARDENING.md`, `modules/OPERATIONS.md`,
   `modules/TEAMS.md` y `modules/ADMIN_PORTAL.md`;
7. lee completos `docs/threat-model/README.md`,
   `docs/modules/local-mvp-lab.md`,
   `project-management/PILOT_ASSESSMENT.md` y
   `project-management/INFORMATION_REQUESTS.md`;
8. inspecciona `src/Desktop`, `src/DeviceAgent`, `src/AdminWeb`, `src/Worker`,
   `src/Contracts`, `deploy`, `scripts` y sus pruebas;
9. identifica controles implementados y brechas reales antes de elegir la
   primera unidad tecnica del Bloque 10.

Reglas no negociables:
- no cambies stack, alcance, arquitectura, persistencia, seguridad o contratos
  publicos sin registrar un stopper en `WORKFLOW.md`;
- conserva Next.js/TypeScript/Prisma/PostgreSQL como control plane;
- conserva el worker Node como proceso durable separado;
- conserva WinUI sin privilegios y DeviceAgent como frontera privilegiada;
- no aceptes comandos, scripts, rutas, argumentos o texto operativo libre;
- toda mutacion requiere identidad, autorizacion, confirmacion, correlacion e
  idempotencia;
- payloads desconocidos, versiones no soportadas y acciones no allowlisted
  fallan cerrados;
- no uses datos, credenciales, endpoints, tenant, certificados ni
  identificadores corporativos;
- no conectes OpenText, Teams, Microsoft 365, Entra, UEMS, Sophos o PKI reales;
- no inventes owners, cuentas de servicio, permisos, politicas, excepciones,
  firma, publicador, retencion ni mecanismos de despliegue;
- no solicites ni implementes una exclusion antivirus general;
- no presentes `local-demo` como piloto corporativo;
- no reabras ni declares completado el Bloque 9 sin integrar el bot real;
- no adelantes el portal administrativo del Bloque 11;
- no agregues dependencias si las APIs y herramientas actuales son suficientes;
- no declares completado el Bloque 10 sin threat model revisado y ensayo
  autorizado de despliegue/retiro en dos endpoints;
- no declares terminada una unidad sin ejecutar los gates aplicables.

Tarea:
Inicia el Bloque 10 con una primera unidad local de hardening coherente con el
codigo existente. El objetivo es convertir el threat model de trabajo en una
auditoria verificable y cerrar una brecha local acotada sin depender de
infraestructura corporativa.

Antes de implementar:
- mapea las amenazas de `docs/threat-model/README.md` a codigo, configuracion y
  pruebas existentes;
- contrasta identidad del agente, IPC, replay, artefactos, logs, evidencia,
  perdida de red, deshabilitacion y retiro;
- elige una sola brecha local de mayor prioridad que pueda cerrarse sin UEMS,
  Entra, Sophos, PKI ni endpoints reales;
- define en `docs/modules/` el alcance tecnico exacto, alternativas, evidencia y
  criterio de aceptacion;
- registra un stopper nuevo solo si la solucion exige alterar una frontera
  normativa o depende de una decision externa no documentada.

Alcance minimo esperado:
- inventario trazable de controles existentes y brechas del threat model;
- una unidad tecnica local, pequena y completa, elegida despues de inspeccionar
  el codigo;
- fallo cerrado y configuracion segura por defecto;
- pruebas unitarias, de contrato o integracion proporcionales al cambio;
- evidencia de que no se amplian comandos, privilegios ni datos recolectados;
- documentacion del punto de sustitucion o configuracion para piloto;
- runbook actualizado de deshabilitacion, rollback o retiro relacionado;
- riesgos residuales y evidencia externa requerida para dos endpoints.

Criterios de aceptacion:
- Bloques 0 a 8 permanecen `completed`;
- Bloque 9 permanece `blocked`;
- Bloque 10 permanece como unico bloque `in_progress`;
- Bloque 11 permanece `pending`;
- no se crea ninguna conexion o identidad corporativa;
- el control agregado falla cerrado y es verificable localmente;
- WinUI, Teams e IA siguen sin ejecutar contenido privilegiado;
- DeviceAgent conserva acciones tipadas y allowlisted;
- no aparecen secretos, PII innecesaria ni logs operativos completos;
- los gates existentes de Bloques 7 a 9 siguen pasando;
- `scripts/Validate.ps1`, auditoria de dependencias y escaneo de secretos pasan;
- no se declara cerrado el gate de piloto ni se simula aprobacion externa.

Stopper externo de Teams que debe conservarse:
Fecha: 2026-06-14
Modulo: Canal Teams
Decision requerida: Confirmar owner, plataforma, repositorio, autenticacion,
  permisos, tenant, ambientes, DLP, despliegue y mecanismo de acciones del bot
  corporativo existente.
Impacto: Sin esta evidencia no puede validarse ni declararse completa la
  integracion corporativa del Bloque 9.

Gates externos del Bloque 10:
- UEMS y procedimiento de despliegue/retiro;
- cuenta restringida del agente;
- revision Security/Sophos;
- identidad de usuario y dispositivo;
- kill switch y owner operativo;
- logs, retencion y respuesta ante compromiso;
- paquete y confianza de publicador;
- dos endpoints autorizados y criterio de restauracion.

Gate base:
.\scripts\Validate.ps1

Comandos adicionales:
corepack pnpm@11.5.3 audit --prod --audit-level high
.\scripts\Test-Secrets.ps1

Al terminar:
- informa alcance implementado, archivos, controles y pruebas;
- lista riesgos y pendientes externos;
- actualiza documento propietario, evidencia, `WORKFLOW.md`,
  `CURRENT_CONTEXT.md` y el threat model;
- recomienda un Conventional Commit;
- no hagas commit ni push automaticamente.
