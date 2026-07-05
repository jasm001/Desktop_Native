# Workflow de desarrollo

## Estado

- Fase actual: capacidades locales controladas para Windows 11.
- Bloque activo: Bloque 11, portal administrativo web, `in_progress`. Los
  Bloques 9 y 10 permanecen `blocked`.
- Los Bloques 0 a 8 estan `completed`.
- Ultimo resultado publicado: ajuste local de Hermes en `eb99434`, que eleva a
  120 segundos el timeout cliente de WinUI. La unidad previa del asistente
  WinUI esta publicada en `cfa3342` y agrega historial visual en memoria,
  indicador de respuesta, autoscroll y envio con Enter. La cuarta unidad local
  del Bloque 11 esta publicada en `212f274`; el bloque completo sigue abierto.
- Unidades locales del Bloque 11: identidad sintetica separada, autorizacion
  server-side fail-closed, shell accesible, proyecciones Prisma sin mutacion,
  pruebas de componentes y recorridos de navegador. No hay OIDC/Entra, RBAC
  productivo, Fluent UI ni mutaciones.
- La unidad de esqueleto protegido `/admin/*`, documentada en
  `docs/modules/admin-portal-local-skeleton-closure.md`, esta publicada en
  `212f274` sin mutaciones, integraciones corporativas ni gobierno productivo
  inventado.
- Nota modular nueva: `docs/modules/lab-bridge-reuse-notes.md` registra que el
  laboratorio externo Bunny Bridge puede reutilizarse como patron de bridge y
  validacion `validate-only`, no como dependencia productiva ni como cierre de
  UEMS, Teams, Entra o piloto.
- Ruta local aprobada para desarrollo: mirror local de software libre, servicios
  locales/fake, Hermes local opcional para texto libre informativo en WinUI,
  historial visual en memoria, RAG local pendiente, datos reales de laboratorio
  persistidos bajo `lab-real-sanitized` y continuidad degradada; no equivale a
  piloto corporativo.
- Ruta documental nueva: `docs/modules/local-lab-real-data-roadmap.md` define
  cinco unidades independientes para sustituir muestras por evidencia real de
  laboratorio saneada sin cerrar los Bloques 9, 10 u 11.

## Ultima validacion

- Fecha base: 2026-06-28.
- SDK global `10.0.301`: correcto.
- El gate .NET local usa `-m:1`, deshabilita build servers y escribe build/test
  en `.artifacts/validate` para no competir con el build host del IDE.
- `dotnet restore ITSupportNative.slnx --locked-mode`: correcto.
- `dotnet format ITSupportNative.slnx --verify-no-changes --no-restore`:
  correcto.
- `dotnet build ITSupportNative.slnx --configuration Release --no-restore`:
  correcto, 0 warnings y 0 errores.
- `dotnet test ITSupportNative.slnx --configuration Release --no-build`:
  correcto, 140 pruebas.
- Hermes local de WinUI: proveedor `IAssistantProvider` opcional compatible con
  OpenAI, deshabilitado por defecto, limitado a loopback y texto informativo;
  4 pruebas nuevas cubren proveedor ausente, envio sin solicitud, configuracion
  local explicita y rechazo de endpoint no loopback.
- Dependencias .NET: `SQLitePCLRaw.bundle_e_sqlite3` fijado a `3.0.3` mediante
  Central Package Management para evitar la vulnerabilidad alta reportada por
  NuGet Audit en `SQLitePCLRaw.lib.e_sqlite3` 2.1.11.
- Pruebas del Bloque 10: kill switch sin persistencia, pendientes sin avance,
  rechazo IPC y ausencia de claim; politica de configuracion predeterminada,
  perfiles/ambientes/combinaciones invalidos; eventos de runtime con IDs,
  mensajes fijos y sin excepciones.
- Smoke del DeviceAgent con perfil no soportado: salida fija sin stack trace y
  codigo `78` antes de construir el host.
- Pruebas nuevas del Bloque 6: manifiesto versionado, mirror, longitud,
  SHA-256, perfil `local-demo`, plataforma, argumentos fijos, idempotencia,
  timeout, codigos MSI, fallo de inicio, verificacion, desinstalacion,
  seleccion exacta, autorizacion deny-by-default y cancelacion segura/no segura.
- Las pruebas del adaptador usan dobles y no ejecutan el MSI en el host.
- Named Pipe real: correcto fuera del sandbox con ACL del usuario actual.
- `corepack pnpm@11.5.3 run check`: correcto.
- Contratos/Node: 40 pruebas unitarias, de contrato y de componentes; lint,
  TypeScript estricto y builds de Contracts, AdminWeb y Worker correctos.
- Portal local: 33 pruebas unitarias/componentes cubren identidad separada,
  ambiente o flag invalido, rol/capability desconocidos, ocho capabilities
  permitidas, limites fijos para lecturas administrativas, shell, tablas,
  estados vacios, esqueleto local y acceso denegado.
- QA de navegador: 24 recorridos Playwright cubren acceso denegado, `/admin`,
  `/admin/catalog`, `/admin/operations`, `/admin/audit`, `/admin/access`,
  `/admin/approvals`, `/admin/support`, `/admin/reporting`, escritorio
  `1440x1000`, movil `390x844`, teclado, estados activos, solo lectura,
  ausencia de formularios/botones mutantes en `main` y cero overflow horizontal
  de pagina.
- Canal Teams local: contrato estricto C#/Zod, acciones allowlisted, rechazo de
  campos ejecutables, adaptador recorded, correlacion, idempotencia, estado,
  caso y paridad con WinUI cubiertos.
- Next.js `16.2.9`: build standalone correcto con webpack; rutas HTTP v1 de
  catalogo, creacion confirmada, estado y caso interno incluidas.
- PostgreSQL 18 real efimero: cuatro migraciones aplicadas con `migrate deploy`,
  incluidas `20260614013000_bot_case_foundation` y
  `20260614090000_fake_ticketing`.
- Integracion PostgreSQL: 12 pruebas AdminWeb y 4 pruebas Worker correctas;
  idempotencia, caso unico, conflicto de payload, transaccion, auditoria
  append-only, consultas sin mutacion, resultados de exito/fallo, leases
  agotados, escalamiento tipado, ticket fake idempotente y efectos sinteticos
  cubiertos.
- E2E del Bloque 7: build standalone de Next.js, bundle ejecutable del worker,
  cliente HTTP de WinUI y DeviceAgent saliente completaron una solicitud con
  tres evidencias saneadas sin ejecutar instalaciones reales.
- `corepack pnpm@11.5.3 audit --prod --audit-level high`: correcto, sin
  vulnerabilidades conocidas.
- `scripts/Test-Secrets.ps1`: correcto, sin hallazgos.
- `scripts/Validate.ps1`: correcto.
- Lockfiles NuGet actualizados solo por el pin de `SQLitePCLRaw.bundle_e_sqlite3`
  `3.0.3`; Desktop/WindowsUi conservan unicamente `win-x64`.
- Token Hyper-V: `S-1-5-32-578` habilitado; grupo local y acceso de lectura a
  VM/checkpoint confirmados.
- Matriz VM: correcta en Windows 11 Pro Education build 26200 x64, generacion 2,
  con checkpoint estandar y credencial local introducida interactivamente.
- Integridad: MSI de 2,002,432 bytes y licencia conservaron sus SHA-256 fijados
  en host y VM.
- Instalacion: `Succeeded`, codigo MSI `0`, 5.850 s; `Verify` `Detected`.
- Repeticion de instalacion: `AlreadyInDesiredState`, sin nuevo proceso.
- Desinstalacion: `Succeeded`, codigo MSI `0`, 1.738 s; deteccion `Absent`.
- Repeticion de desinstalacion: `AlreadyInDesiredState`, sin nuevo proceso.
- Fallos controlados: mirror ausente `ArtifactUnavailable`; hash corrupto
  `ArtifactHashMismatch`; ambos fallaron cerrados antes de iniciar MSI.
- Restauracion: correcta; producto y artefactos de laboratorio ausentes.
- Timeout, `1618`, `1641` y `3010` no se indujeron en la VM; permanecen cubiertos
  por pruebas automatizadas.
- El MSI no se ejecuto en la PC principal y el Bloque 6 esta `completed`.

Validacion focalizada posterior:

- Fecha: 2026-07-05.
- Unidad local del asistente WinUI: historial visual de chat en memoria,
  indicador de respuesta de Hermes, autoscroll, envio con Enter y timeout
  cliente de 120 segundos. No agrega persistencia, mutaciones, comandos,
  llamadas al DeviceAgent ni archivos `.env` con secretos.
- `dotnet build ITSupportNative.slnx --configuration Release --no-restore -m:1
  --artifacts-path .artifacts\validate --disable-build-servers`: correcto, 0
  warnings y 0 errores.
- `dotnet test ITSupportNative.slnx --configuration Release --no-build -m:1
  --artifacts-path .artifacts\validate --disable-build-servers`: correcto, 140
  pruebas.
- `dotnet format ITSupportNative.slnx --verify-no-changes --no-restore`:
  correcto.
- `scripts/Test-Secrets.ps1`: correcto, sin hallazgos.

Validacion anterior de fundacion:

- Remoto `Desktop_Native.git` verificado sin refs antes de inicializar.
- `dotnet restore ITSupportNative.slnx --force-evaluate`: correcto.
- `dotnet format ITSupportNative.slnx --verify-no-changes --no-restore`:
  correcto.
- `dotnet build ITSupportNative.slnx --configuration Release --no-restore`:
  correcto, 0 warnings y 0 errores.
- `dotnet test ITSupportNative.slnx --configuration Release --no-build`:
  correcto, 4 pruebas.
- `corepack pnpm@11.5.3 run check`: correcto.
- `gitleaks dir . --redact`: correcto, sin hallazgos.

## Regla por tarea

1. Elegir un solo modulo y criterio de aceptacion.
2. Leer contexto base y documentos del modulo.
3. Inspeccionar el codigo existente antes de proponer cambios.
4. Implementar el cambio minimo completo.
5. Ejecutar formato, build, pruebas y escaneos aplicables.
6. Revisar seguridad, arquitectura, accesibilidad y regresiones.
7. Actualizar documento del modulo y este workflow.
8. Proponer un mensaje de commit para la unidad de cambio validada.
9. Reportar archivos, validaciones, riesgos, documentos actualizados, commit
   sugerido y siguiente bloque desbloqueado.

## Regla de commits

- Proponer un commit al terminar una seccion coherente y validada; no esperar
  necesariamente al cierre de un bloque grande.
- Usar Conventional Commits, por ejemplo:
  `feat(desktop): add initial WinUI navigation shell`.
- No mezclar cambios no relacionados en el mismo commit sugerido.
- No sugerir commit como terminado si faltan build o pruebas aplicables.
- Al completar una seccion o modulo, actualizar su documentacion y la tabla de
  bloques antes de proponer el commit.
- El push no sustituye la validacion. Registrar en evidencia el commit o PR
  cuando realmente exista.

## Estados permitidos

- `pending`
- `in_progress`
- `blocked`
- `completed`

Solo un bloque principal puede estar `in_progress`.

## Registro de bloques

| Bloque | Estado | Evidencia |
| --- | --- | --- |
| 0. Fundacion del repositorio | completed | Solucion/workspace, lockfiles, CI y gates locales validados; `e42fe2c`. |
| 1. Shell nativo WinUI | completed | Cinco vistas, tema, teclado, accesibilidad base, layout amplio/compacto y smoke test validados; `f18e8cf`, toolchain `5f6dae7` y lockfiles x64 `531faf6`. |
| 2. Dominio y catalogo sintetico | completed | Modelos puros, siete fixtures, busqueda/filtros, cuatro decisiones tipadas, integracion fina con Desktop y 26 pruebas unitarias de catalogo; `0d1e315`. |
| 3. Conversacion determinista | completed | Cinco estados, intenciones fijas, solicitud sintetica idempotente, 13 pruebas unitarias y 3 pruebas del adaptador WinUI; `b09f07a`. |
| 4. Agente simulado e IPC | completed | Contrato v1, Named Pipe con ACL de usuario actual, allowlist exacta, maquina de estados, cancelacion, evidencia saneada, SQLite, recuperacion e IPC real cubiertos por 18 pruebas nuevas; `b56bfcb`. |
| 5. Diagnostico de solo lectura | completed | Snapshot IPC efimero, colectores Windows de solo lectura, prerrequisitos tipados, fallos parciales saneados y pruebas de frontera; `e3a0b8d`. |
| 6. Primer adaptador en VM | completed | Adaptador 7-Zip 26.01 x64 y 110 pruebas publicados en `f808425`; matriz real de instalacion, idempotencia, desinstalacion, fallos de mirror/hash y restauracion de checkpoint validada el 2026-06-13. |
| 7. API compartida y persistencia | completed | Fundacion publicada en `2b89a6b`; cierre local validado con segunda migracion, WinUI HTTP, worker separado, DeviceAgent saliente y E2E sobre PostgreSQL efimero. |
| 8. Casos, tickets y OpenText fake | completed | `BotCase`, politica de 72 horas, evento de escalamiento, `ITicketingProvider` fake, `ExternalTicket`, worker idempotente y consulta HTTP validados sobre PostgreSQL real efimero; `cf262b4`. |
| 9. Canal Teams existente | blocked | Incremento local publicado en `0448a42`: contrato v1 estricto, `IConversationChannel`, adaptador recorded, API compartida y paridad Teams/WinUI. Integracion corporativa bloqueada por evidencia externa. |
| 10. Endurecimiento para piloto | blocked | Trabajo local acotado: threat model trazable, kill switch apagado por defecto, perfil `local-demo` confinado a `Development`, fallos del host saneados y runbook de retiro. Revision externa y ensayo en dos endpoints pendientes. |
| 11. Portal administrativo web | in_progress | Cuatro unidades locales validadas: identidad separada, ocho capabilities server-side, navegacion `/admin/*`, lecturas Prisma limitadas sin payload de auditoria, estados sinteticos en memoria, pruebas de componentes y recorridos Playwright locales; sin Entra, RBAC productivo, Fluent UI o integraciones corporativas. |

## Alcance del MVP local

El MVP local permanece utilizable con proveedores locales o fake aunque los
Bloques 9 y 10 esten bloqueados. D-072 permite conservar unidades locales
reemplazables sin cerrar gates externos y permite continuar el Bloque 11 con
identidad de desarrollo, datos sinteticos y datos reales de laboratorio
persistidos bajo `lab-real-sanitized`. No autoriza promover `local-demo` ni
declarar cerrado un gate corporativo. La referencia tecnica es
`docs/modules/local-mvp-lab.md`.

Permitido antes de la decision corporativa:

- VM Windows 11 personal sin datos ni credenciales corporativas;
- Windows Service de laboratorio y Salud real por IPC;
- software libre redistribuible desde mirror local con SHA-256;
- backend, persistencia, portal e identidad de desarrollo;
- datos reales de laboratorio generados localmente, saneados y etiquetados como
  `lab-real-sanitized`;
- Hermes local con API externa usando contenido publico/sintetico para texto
  libre informativo;
- RAG e indice locales pendientes;
- operacion offline cerrada con politica y artefactos ya disponibles.

Pendiente para piloto:

- UEMS, Entra, OpenText y bot de Teams reales;
- Security/Sophos, PKI, firma y confianza de publicador;
- identidad restringida y despliegue corporativo del servicio;
- hosting, retencion, conectividad y paquetes corporativos aprobados.

## Decisiones locales registradas

La siguiente decision local ajusta el alcance del laboratorio sin cerrar gates
externos ni cambiar el estado de bloques.

```text
Fecha: 2026-07-05
Modulo: Laboratorio local / Portal administrativo
Decision requerida: Confirmar si el perfil local puede pasar de muestras a
  datos reales de laboratorio persistidos y saneados.
Evidencia: Solicitud y aprobacion del owner del repositorio para documentar la
  categoria `lab-real-sanitized` y cinco unidades independientes antes de
  implementar.
Alternativas: Mantener solo muestras sinteticas; exponer datos puramente en
  memoria; esperar datos corporativos aprobados.
Impacto: Se permite usar evidencia generada por laboratorio local controlado,
  etiquetada como `lab-real-sanitized`, sin secretos ni datos corporativos. No
  cierra Bloques 9, 10 u 11 y no habilita integraciones productivas.
Recomendacion: Empezar por las Unidades 1 y 2 de
  `docs/modules/local-lab-real-data-roadmap.md`; tratar VM y APIs apagadas como
  `not_checked`, `offline` o `unavailable`.
Owner: Desarrollo / operador local del laboratorio.
```

## Stoppers externos

Estos stoppers condicionan integraciones o mejoras externas. No revierten
bloques ya completados ni impiden unidades locales que respeten sus limites.

El siguiente stopper bloquea la conexion corporativa y el cierre del Bloque 9.
No revierte los contratos, el adaptador recorded o las pruebas de paridad ya
validadas.

```text
Fecha: 2026-06-14
Modulo: Canal Teams
Decision requerida: Confirmar owner, plataforma, repositorio, autenticacion,
  permisos, tenant, ambientes, DLP, despliegue y mecanismo de acciones del bot
  corporativo existente.
Evidencia: Contacto tecnico; configuracion o diagrama saneado; contrato de
  autenticacion; ambiente de prueba; payloads de ejemplo sin datos reales;
  proceso de despliegue y rollback.
Alternativas: Completar contratos, adaptador fake/recorded y pruebas de paridad;
  integrar despues mediante Teams SDK, Copilot Studio u otra plataforma
  aprobada.
Impacto: Sin esta evidencia no puede validarse ni declararse completa la
  integracion corporativa del Bloque 9.
Recomendacion: Mantener neutral `IConversationChannel` y no acoplar el dominio a
  una plataforma no confirmada.
Owner: Equipo actual del bot de Teams / Collaboration / Automation.
```

El siguiente stopper bloquea el cierre del Bloque 10. No afecta el MVP local ni
revierte sus controles validados.

```text
Fecha: 2026-06-14
Modulo: Endurecimiento para piloto
Decision requerida: Confirmar procedimiento UEMS de despliegue y retiro, cuenta
  restringida del agente, identidad de usuario y dispositivo, revision
  Security/Sophos, owner operativo del kill switch, logs y retencion, respuesta
  ante compromiso, paquete y confianza de publicador, y dos endpoints
  autorizados con criterio de restauracion.
Evidencia: Procedimiento y configuracion saneados; matriz de permisos; revision
  de Security; mecanismo de paquete/firma/confianza; resultados de despliegue,
  actualizacion, deshabilitacion, rollback y retiro en dos endpoints; revision
  de logs y evidencia.
Alternativas: Mantener el MVP `local-demo` en `Development`; decidir despues
  entre proveedores corporativos o propios mediante puntos de sustitucion
  explicitos, sin promover el perfil local.
Impacto: Sin esta evidencia no puede declararse `completed` el Bloque 10 ni
  iniciar un piloto corporativo.
Recomendacion: Mantener el Bloque 10 `blocked` y reanudarlo solo con el paquete
  minimo de evidencia externa. El trabajo local del Bloque 11 puede continuar
  bajo D-072 sin presentarse como evidencia de piloto.
Owner: Por confirmar mediante las solicitudes de informacion vigentes.
```

Los siguientes stoppers condicionan solo la mejora posterior de refresco
administrado de politicas. No cambian el estado `completed` del Bloque 6 ni el
gate local ya validado.

```text
Fecha: 2026-06-12
Modulo: DeviceAgent / conectividad corporativa
Decision requerida: Confirmar si Endpoint Management puede crear para el equipo
  de prueba un perfil FortiClient EMS de maquina, pre-logon o equivalente, sin
  credenciales humanas persistidas.
Evidencia: Export o captura saneada del perfil; version de FortiClient/EMS;
  metodo de autenticacion; alcance/rutas; comportamiento de auto-connect,
  desconexion y recuperacion tras reinicio.
Alternativas: Mantener VPN iniciada manualmente por el usuario; usar Axis Atmos
  si publica todos los servicios de dominio requeridos; escalar como en el MVP.
Impacto: Sin perfil aprobado no existe autoconexion segura para ejecutar una
  campana fuera de LAN.
Recomendacion: Solicitar un perfil limitado al equipo/OU de prueba con
  certificado de maquina o mecanismo corporativo equivalente.
Owner: Network / Endpoint Management / Security.
```

```text
Fecha: 2026-06-12
Modulo: DeviceAgent / conectividad corporativa
Decision requerida: Validar en el equipo fisico de prueba si Axis Atmos permite
  localizar y alcanzar DNS corporativo, controlador de dominio, SYSVOL y
  NETLOGON sin FortiClient.
Evidencia: Fecha y red de la prueba; estado de Axis; `nltest /dsgetdc:<dominio>`
  con nombres saneados; resolucion DNS; acceso a rutas requeridas; resultado de
  una GPO inocua y reversible; necesidad de logoff/reboot.
Alternativas: Perfil FortiClient EMS de maquina/pre-logon; conexion manual;
  conservar orientacion/escalamiento del MVP.
Impacto: Si Axis no publica esos servicios, no puede ser la conectividad para
  el refresco automatico de politicas.
Recomendacion: Probar primero solo lectura y despues una GPO de laboratorio
  aprobada, sin compartir nombres, IP, credenciales ni salida sensible.
Owner: Desarrollo / Network / Active Directory / Security.
```

```text
Fecha: 2026-06-12
Modulo: Producto / despliegue y reprovisionamiento
Decision requerida: Elegir despues del MVP si la evolucion continua como
  iniciativa corporativa, producto independiente o ambas variantes mediante
  proveedores distintos.
Evidencia: Resultado de aprobacion corporativa; capacidad de ISO/UEMS para
  incluir bootstrap; prueba del portal/bootstrap independiente; demanda de los
  niveles local, personal/pro y equipos pequenos.
Alternativas: Rama empresarial ISO/UEMS; rama independiente Windows limpio +
  portal; mantener solo autoservicio local sin borrado remoto.
Impacto: Define proveedores, empaquetado y modelo comercial, pero no cambia el
  dominio, el Bloque 5 ni los Bloques 6-11.
Recomendacion: Mantener contratos neutrales y posponer el borrado hasta validar
  autoservicio, seguridad, enrolamiento y retiro.
Owner: Product Owner / Security / Endpoint Management.
```

## Gate operativo cerrado del Bloque 6

```text
Fecha: 2026-06-13
Modulo: DeviceAgent / primer adaptador real
Accion completada: Renovar el token, verificar Hyper-V, ejecutar la matriz real
  y restaurar el checkpoint.
Evidencia: `whoami /groups` mostro `S-1-5-32-578` habilitado. Fuera del sandbox,
  Hyper-V expuso una sola VM Windows 11 x64 de generacion 2 y un checkpoint
  estandar. PowerShell Direct uso una credencial local introducida
  interactivamente y no persistida.
Resultado: `Detect` inicial/final `Absent`; `Preflight` `Ready`; instalacion y
  desinstalacion `Succeeded` con codigo `0`; `Verify` `Detected`; repeticiones
  `AlreadyInDesiredState`; mirror ausente `ArtifactUnavailable`; hash corrupto
  `ArtifactHashMismatch`.
Restauracion: Checkpoint restaurado; producto y artefactos de laboratorio
  ausentes. El MSI no se ejecuto en el host.
Impacto: El gate del Bloque 6 esta cerrado. Esta evidencia no autoriza promocion
  fuera de `local-demo` ni sustituye los gates de piloto.
Recomendacion al cierre, ya cumplida: Mantener el adaptador y su politica sin
  cambios. La unidad separada posterior del Bloque 7 tambien fue completada.
Owner: Desarrollo / administrador local de Hyper-V.
```

## Formato de stopper

```text
Fecha:
Modulo:
Decision requerida:
Evidencia:
Alternativas:
Impacto:
Recomendacion:
Owner:
```

## Regla de documentacion

No usar este archivo como diario detallado. Conservar solo estado actual,
resultados verificables, bloqueos y siguiente paso. El conocimiento estable vive
en el documento propietario del modulo o en un ADR del repositorio definitivo.
