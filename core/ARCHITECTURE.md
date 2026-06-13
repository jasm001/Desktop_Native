# Arquitectura

## Componentes

```text
WinUI 3 Client      Teams Bot       Admin Web Portal
  | user token          | user token       | scoped admin token
  +--------------------- HTTPS ------------+
  | HTTPS
  v
Next.js Control Plane --------- OpenText / Device Mgmt / Rescue
  |
PostgreSQL + Node Worker ----- AI Gateway / Artifact Repository
  |                                  adapters
  | authorized policy and jobs
  v
Device Agent <---- authenticated Named Pipe ----> WinUI 3 Client
  |
  +---- inventory collectors
  +---- diagnostic probes
  +---- approved software executors
  +---- durable local queue
```

El agente siempre inicia comunicaciones salientes por HTTPS. No abre puertos de
administracion entrantes en el equipo.

Teams y WinUI son canales del mismo asistente. Comparten backend, catalogo,
politicas, conocimiento, solicitudes y tickets. Teams no envia comandos al
equipo: solicita una accion al backend y este solo crea un trabajo si la
identidad, dispositivo y politica son validos.

### Perfil local de demostracion

El perfil `local-demo` conserva las mismas fronteras mediante proveedores
reemplazables:

```text
WinUI -> API local/fake -> DeviceAgent en VM
  |          |                 |
  |          +-- SQLite/PostgreSQL local
  +-- Hermes provider -> API externa opcional
  +-- RAG e indice local
DeviceAgent -> origen local de artefactos con manifiesto y SHA-256
```

La politica de desarrollo se instala explicitamente en la VM y solo contiene
acciones allowlisted. No se deriva una autorizacion desde el texto del chat. El
perfil no contiene endpoints, identidades, software ni credenciales
corporativas.

## Fronteras

### Cliente WinUI

- Presenta dashboard, catalogo, asistente, salud, tickets y soporte.
- Mantiene estado de interfaz, no la verdad operativa.
- Solicita acciones al backend y muestra progreso del agente.
- Puede leer diagnosticos de bajo privilegio por IPC.
- Nunca contiene secretos de integraciones ni credenciales administrativas.
- Permite consultas de software autorizado, EOL, prohibido o con licencia.
- Puede mostrar avisos y guias de identidad cuando el usuario ya inicio sesion.
- No puede ejecutarse sobre la pantalla de inicio de sesion de Windows.

### Canal Teams

- Atiende consultas, catalogo, solicitudes, seguimiento y notificaciones.
- Permite continuar desde otro dispositivo si el usuario conserva acceso.
- Usa la misma API y reglas que WinUI; no mantiene un catalogo independiente.
- Puede iniciar una solicitud para un equipo registrado, pero no llama
  directamente al agente ni transmite comandos.
- Requiere Internet, Microsoft 365 y acceso vigente a la cuenta corporativa.
- No es un canal de recuperacion cuando la misma cuenta de Teams esta bloqueada.

### Agente Windows

- Es el unico componente que ejecuta acciones locales privilegiadas.
- Valida autorizacion, vigencia, equipo, usuario, accion e idempotency key.
- Reporta progreso y evidencia, incluso tras reinicios o perdida de red.
- Separa recopilacion de inventario, diagnostico y ejecucion de software.
- Rechaza cualquier operacion fuera del catalogo/politica instalada.
- Puede terminar un trabajo ya descargado y autorizado durante una caida del
  backend; conserva evidencia local y sincroniza al reconectarse.

### Portal administrativo

- Gestiona invitaciones, usuarios, roles y alcance por sede/area.
- Importa y normaliza el catalogo corporativo.
- Implementa colas de aprobacion para seguridad, licencias y compras.
- Permite a soporte ver tickets y diagnostico autorizado.
- Expone auditoria y metricas sin otorgar acceso directo al agente.
- En desarrollo existe un perfil local con todos los roles; no se promueve a
  produccion.

### Backend

Modulos iniciales:

- Identity and Access.
- Device Registry.
- Software Catalog.
- Requests and Tickets.
- Job Orchestration.
- Audit and Evidence.
- Integrations.
- Assistant.
- Administration and Reporting.
- Approvals and Entitlements.
- Artifact Repository.
- Bot Metrics and Ticketing.
- Conversation Channels.
- Identity Recovery Guidance.

Cada modulo posee sus reglas y tablas. La comunicacion entre modulos usa
interfaces internas y eventos de aplicacion; no acceso directo indiscriminado.

El plano web se implementa en Next.js, pero los modulos de dominio no dependen de
componentes React, cookies ni Prisma directamente. Route Handlers y Server
Actions llaman casos de uso; repositorios Prisma quedan en infraestructura.

## Flujo de instalacion

1. Usuario elige una aplicacion autorizada.
2. Backend valida identidad, dispositivo, licencia y politica.
3. Se crea solicitud e idempotency key.
4. Backend emite un trabajo firmado para el agente.
5. Agente valida y ejecuta un adaptador conocido.
6. Agente transmite estados: queued, downloading, executing, verifying,
   succeeded/failed.
7. Backend registra evidencia y sincroniza el ticket cuando aplique.
8. Usuario confirma cierre y satisfaccion.

Los programas libres aprobados pueden avanzar automaticamente segun politica.
Los comerciales crean una aprobacion para compras/licencias. Los no listados o
prohibidos se rechazan o envian a seguridad, nunca se instalan por coincidencia
de texto.

## Flujo del asistente

1. WinUI o Teams envia texto y contexto minimo autorizado.
2. El backend elimina o seudonimiza datos no necesarios.
3. El proveedor de IA devuelve una intencion estructurada.
4. El backend valida la intencion contra catalogo y politicas.
5. Si la intencion es `query`, devuelve informacion sin crear trabajo ni ticket.
6. Si existe una accion posible, devuelve una propuesta y espera confirmacion.
7. Solo una confirmacion explicita crea solicitud, trabajo o escalamiento.
8. La ejecucion sigue el mismo flujo tipado; el modelo nunca llama al agente.

Si la IA no esta disponible, el catalogo, busqueda, opciones fijas y clasificador
determinista siguen funcionando. La IA es una mejora de lenguaje, no una
dependencia operativa.

### Estados de conversacion

- `query`: consulta informativa, sin efectos.
- `proposal`: respuesta con opciones posibles.
- `confirmation_required`: espera una eleccion explicita del usuario.
- `request_created`: solicitud persistida.
- `job_created`: accion local autorizada.
- `ticket_escalated`: ticket externo creado.
- `cancelled`: el usuario decide no continuar.

Una consulta nunca se convierte por inferencia en `request_created`. La
confirmacion incluye accion, software/version, equipo y resultado esperado. La
idempotency key evita que repetir "si" o reintentar el mensaje duplique una
instalacion o ticket.

El backend compartido es la fuente de verdad para catalogo, alternativas,
licencias y estados. WinUI y el bot existente de Teams no implementan reglas
separadas.

## Orquestacion con Power Automate

Power Automate puede integrarse como adaptador para mensajes de Teams,
aprobaciones, recordatorios y procesos Microsoft 365. No es la frontera que
autoriza o ejecuta acciones locales.

Un flujo puede crear una solicitud en el backend. El backend vuelve a validar
usuario, equipo, catalogo y politica. Solo despues emite un trabajo tipado al
agente. Las credenciales de Power Automate nunca se entregan al agente.

Power Automate Desktop/RPA no sustituye el agente para el MVP: depende de
sesiones de Windows, conectividad y licencias; la elevacion desatendida vigente
o planeada debe validarse por tenant y aun requeriria los controles de catalogo,
idempotencia, rollback y evidencia.

## Recuperacion de identidad

La recuperacion de contrasena no la implementa la IA ni el agente.

- El autoservicio actual se abre en `recovery.softtek.com`, usando un correo
  personal previamente registrado, codigo de verificacion y CAPTCHA.
- Con sesion iniciada, WinUI orienta, abre el portal oficial y comprueba
  prerrequisitos de red sin capturar la contrasena.
- El proveedor oficial solicita contrasena/MFA cuando corresponda. WinUI no
  renderiza ni intercepta esos campos.
- Una sesion Windows o token Teams vigente identifica contexto, pero no basta
  como autorizacion para restablecer credenciales.
- Sin acceso a Windows, se evalua el portal corporativo y si puede exponerse
  desde la pantalla de inicio mediante una capacidad soportada.
- Para identidades sincronizadas desde AD local, se confirma password writeback.
- Un equipo hybrid joined necesita conectividad con un controlador de dominio
  para usar la nueva contrasena y actualizar credenciales almacenadas.
- Si no existe conectividad, correo personal enrolado o configuracion necesaria,
  el caso se escala por un canal alterno.

No se desarrolla un Credential Provider propio en el MVP. Tampoco se almacenan
contrasenas nuevas, anteriores, de VPN o de dominio.

El nombre SSPR se mantiene como posibilidad tecnica, no como hecho confirmado,
hasta que Identity identifique la plataforma de `recovery.softtek.com`.

Las cuentas `softtek.com` y `softtekprojects.com` siguen flujos diferentes. La
segunda usa credencial de dominio independiente y, tras reset administrativo,
Windows obliga a reemplazar la credencial temporal al iniciar sesion.

### Restablecimiento asistido

Cuando el autoservicio no aplica:

1. WinUI crea un `IdentityRecoveryCase`;
2. backend registra usuario, dispositivo y evidencia no sensible;
3. el usuario realiza step-up/MFA en el proveedor de identidad;
4. el caso se escala al grupo autorizado;
5. un administrador aprueba y ejecuta el procedimiento vigente;
6. el producto registra estado y orienta sobre red/VPN/cambio posterior.

La automatizacion de la accion administrativa mediante Microsoft Graph o una API
interna queda fuera del primer MVP. Requiere permisos de autenticacion de alto
impacto, separacion entre solicitante y aprobador, credencial de workload,
auditoria reforzada y entrega segura. Ninguna contrasena temporal se guarda en
PostgreSQL, logs, ticket, Power Automate o historial conversacional.

El plazo de 48-72 horas se registra como procedimiento operativo: se recomienda
al usuario cambiar la credencial temporal dentro de esa ventana y evitar cambios
consecutivos durante las primeras 24 horas. No se modela como expiracion
automatica. Si Identity confirma `forceChangePasswordNextSignIn` u otra regla
tecnica, se documentara como una politica independiente.

El producto registra que mostro la recomendacion, pero no crea recordatorios,
seguimiento, bloqueo ni remediacion si el usuario conserva la credencial
temporal. El gobierno de esa practica queda fuera del alcance.

## Politicas de dominio y conectividad

- El agente puede detectar interfaces, DNS, dominio y controladores alcanzables,
  pero esa señal no demuestra por si sola que todas las politicas esten sanas.
- Acciones como refrescar politicas declaran `CorporateNetworkRequired`.
- `gpupdate` es una accion tipada, con target, timeout y manejo de logoff/reboot;
  no un comando libre.
- Si no hay LAN/VPN, el MVP no intenta autenticar la VPN: informa y escala el
  caso. Un estado `waiting_for_corporate_network` solo se habilitara despues si
  Network aprueba un flujo confiable.
- El agente no guarda contrasenas VPN ni intenta mantener una VPN personal
  conectada permanentemente.
- Playwright no controla FortiClient: automatiza navegadores, no aplicaciones
  Win32. UI Automation para encender/apagar VPN seria fragil y no se adopta.
- Solo se automatiza VPN si existe CLI/API corporativa documentada y aprobada.
  De lo contrario se abre el cliente, se guia al usuario y se escala.
- Cambios de contrasena, expiracion o politicas de dominio siguen siendo
  autoridad de Active Directory y de la VPN corporativa.
- Windows puede permitir inicio offline con credenciales de dominio almacenadas.
  Un cambio de contrasena remoto no actualiza ese verificador local hasta que el
  equipo recupere comunicacion con la autoridad correspondiente.
- La expiracion puede no manifestarse mientras el equipo trabaja solo con cache.
  Al recuperar LAN/VPN, el dominio puede exigir el cambio o rechazar acceso.

### Campana futura de refresco

Una mejora posterior al MVP puede usar el control plane como fuente explicita de
campanas de refresco de politicas. El agente consulta por HTTPS saliente con baja
frecuencia y jitter; no intenta descubrir cambios con ping ni replica cambios
desde una VM, sandbox o usuario alterno.

El backend entrega identidad de campana, alcance, expiracion y el identificador
de una accion conocida. El endpoint valida politica instalada, conectividad de
dominio y consentimiento local. `gpupdate`, logoff y reboot son pasos separados,
tipados y auditables.

La conectividad automatica queda condicionada a un perfil corporativo de maquina
o pre-logon aprobado. No se almacenan credenciales humanas y el rechazo de esa
capacidad no bloquea el producto: se mantiene la orientacion/escalamiento del
MVP.

## Autoservicio y mantenimiento posterior

El DeviceAgent puede crecer mediante capacidades independientes y tipadas:

- diagnostico de solo lectura;
- exportacion de evidencia saneada;
- reparaciones locales firmadas y allowlisted;
- limpieza segura de almacenamiento;
- diagnostico guiado de perifericos;
- coordinacion de mantenimiento y reinicios;
- perfiles declarativos de ambientacion;
- operacion local limitada cuando backend o IA no estan disponibles.

Hermes, RAG u otro proveedor puede explicar una deteccion o proponer una accion
conocida. Nunca produce scripts, argumentos o comandos ejecutables. El agente
solo resuelve identificadores instalados y firmados.

El coordinador agrupa trabajos compatibles para reducir reinicios. Una
actualizacion critica puede tener fecha limite no posponible, pero siempre
muestra motivo, tiempo estimado, cuenta regresiva y necesidad de guardar
archivos. Si BitLocker exige PIN al arrancar, informa que se requiere presencia
del usuario; no conoce ni deriva ese PIN.

La responsabilidad detallada vive en
`../docs/modules/endpoint-self-service.md`.

## Borrado y reprovisionamiento posterior

El agente no sobrevive fisicamente a una reinstalacion de Windows. La
continuidad se obtiene porque una autoridad externa vuelve a instalar un
bootstrap firmado y el backend reconoce un registro previo del dispositivo.

Proveedores previstos:

- `IEnrollmentProvider`;
- `IOperatingSystemDeploymentProvider`;
- `IDeviceIdentityProvider`;
- `ISecurityAgentProvider`;
- `INetworkAccessProvider`;
- `IDomainJoinProvider`;
- `ISoftwareProvisioningProvider`.

La rama empresarial usa la ISO/UEMS aprobada para instalar Windows, UEMS,
Sophos y el bootstrap. La rama independiente usa Windows limpio y un portal
autenticado para descargar un bootstrap de un solo uso que recupera el perfil
despues de validar usuario y dispositivo.

Antes del borrado solo se conserva fuera del endpoint la configuracion
permitida: identidad del dispositivo, hostname, asset tag, perfil, aplicaciones
seleccionadas y correlacion. El usuario confirma que completo el respaldo en el
canal aprobado; el producto no copia ni inspecciona archivos personales.

BitLocker no se automatiza con datos derivados del asset tag. Si el proceso
corporativo requiere enrolar o recuperar BitLocker, se crea o escala un ticket
para intervencion humana. El PIN y la recovery password nunca se almacenan,
calculan ni muestran.

El flujo completo vive en `../docs/modules/device-reprovisioning.md` y se
implementa despues de las capacidades de autoservicio.

## Persistencia

Entidades centrales:

- Tenant, User, Role, Device, DeviceAssignment.
- SoftwareProduct, SoftwarePackage, SoftwarePolicy.
- SupportRequest, TicketLink, ExecutionJob, JobAttempt.
- DiagnosticSnapshot, InventorySnapshot.
- AuditEvent, EvidenceArtifact, UserFeedback.
- IntegrationConnection, SyncCheckpoint.
- Site, Area, RoleAssignment, ApprovalRequest.
- Artifact, ArtifactVersion, CatalogImport, NetworkPrerequisite.
- BotCase, ExternalTicket, TicketSyncAttempt, SatisfactionResponse.

Los eventos de auditoria son append-only a nivel de aplicacion. Cambios y
correcciones se registran con nuevos eventos, no sobrescribiendo evidencia.

## Disponibilidad

- Cliente util con backend temporalmente no disponible: consulta cache reciente y
  cola solicitudes sin prometer ejecucion.
- Agente durable ante reinicios.
- Backend idempotente para reintentos.
- Integraciones externas desacopladas mediante outbox y workers.
- Instalacion descargada y autorizada puede continuar sin IA y sin backend.
- Un trabajo que aun no tenga artefacto o autorizacion no empieza offline.
- El conocimiento local y los flujos deterministas permanecen disponibles sin
  el proveedor de IA.
- En `local-demo`, una politica de desarrollo instalada puede ser la autoridad
  local para acciones cerradas dentro de la VM. En piloto, esa autoridad se
  reemplaza por politica corporativa firmada y autorizacion del backend.

## Aislamiento y rollback

No existe un sandbox general que permita cambiar el host y garantizar rollback.
Windows Sandbox sirve para analizar o probar un instalador en un entorno
desechable, pero una instalacion alli no instala el programa en el equipo real.

Se adopta:

- validacion previa de paquetes en VM/Windows Sandbox;
- ejecucion en host mediante adaptadores cerrados;
- proceso hijo con token, ACL, timeout y Job Object cuando aplique;
- MSIX transaccional cuando el software lo permita;
- rollback propio de MSI y desinstalacion/repair definida por paquete;
- captura de estado antes/despues y punto de recuperacion solo como defensa
  adicional, no como garantia.
