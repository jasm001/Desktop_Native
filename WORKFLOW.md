# Workflow de desarrollo

## Estado

- Fase actual: capacidades locales controladas para Windows 11.
- Bloque activo: Bloque 7 `in_progress`. Bloque 6 permanece `completed`.
- Ultimo resultado funcional: primer incremento del control plane del Bloque 7
  validado localmente con Next.js, Prisma/PostgreSQL y worker outbox separado.
- Ultimo resultado publicado: cierre documental del Bloque 6 en `bfb4a35`. El
  incremento del Bloque 7 permanece solo en el working tree y esta documentado
  en `docs/modules/control-plane-foundation.md`.
- Ruta local aprobada para desarrollo: mirror local de software libre, servicios
  locales/fake, Hermes con API externa opcional, RAG local y continuidad
  degradada; no equivale a piloto corporativo.

## Ultima validacion

- Fecha: 2026-06-13.
- SDK global `10.0.301`: correcto.
- El gate .NET local usa `-m:1`, deshabilita build servers y escribe build/test
  en `.artifacts/validate` para no competir con el build host del IDE.
- `dotnet restore ITSupportNative.slnx --locked-mode`: correcto.
- `dotnet format ITSupportNative.slnx --verify-no-changes --no-restore`:
  correcto.
- `dotnet build ITSupportNative.slnx --configuration Release --no-restore`:
  correcto, 0 warnings y 0 errores.
- `dotnet test ITSupportNative.slnx --configuration Release --no-build`:
  correcto, 110 pruebas.
- Pruebas nuevas del Bloque 6: manifiesto versionado, mirror, longitud,
  SHA-256, perfil `local-demo`, plataforma, argumentos fijos, idempotencia,
  timeout, codigos MSI, fallo de inicio, verificacion, desinstalacion,
  seleccion exacta, autorizacion deny-by-default y cancelacion segura/no segura.
- Las pruebas del adaptador usan dobles y no ejecutan el MSI en el host.
- Named Pipe real: correcto fuera del sandbox con ACL del usuario actual.
- `corepack pnpm@11.5.3 run check`: correcto.
- Contratos/Node: 8 pruebas unitarias y de contrato; lint, TypeScript estricto y
  builds de Contracts, AdminWeb y Worker correctos.
- Next.js `16.2.9`: build standalone correcto con webpack; rutas HTTP v1 de
  catalogo, creacion confirmada y estado incluidas.
- PostgreSQL 18 real efimero: migracion
  `20260613074457_control_plane_foundation` aplicada con `migrate deploy`.
- Integracion del Bloque 7: 7 pruebas AdminWeb y 3 pruebas Worker correctas;
  idempotencia, transaccion, auditoria append-only, timestamps de base, consultas
  sin mutacion, claim/retry y efecto sintetico cubiertos.
- `corepack pnpm@11.5.3 audit --prod --audit-level high`: correcto, sin
  vulnerabilidades conocidas.
- `scripts/Test-Secrets.ps1`: correcto, sin hallazgos.
- `scripts/Validate.ps1`: correcto.
- Lockfiles sin cambios; Desktop/WindowsUi conservan unicamente `win-x64`.
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
| 7. API compartida y persistencia | in_progress | Primer incremento validado localmente: contratos HTTP v1, identidad de desarrollo, migracion PostgreSQL/Prisma, mutacion transaccional idempotente, auditoria append-only, outbox y worker separado; aun no publicado. |
| 8. Casos, tickets y OpenText fake | pending | |
| 9. Canal Teams existente | pending | |
| 10. Endurecimiento para piloto | pending | |
| 11. Portal administrativo web | pending | |

## Alcance del MVP local

El desarrollo puede continuar por los Bloques 6-11 con proveedores locales o
fake, respetando el orden y los gates de cada unidad. La referencia tecnica es
`docs/modules/local-mvp-lab.md`.

Permitido antes de la decision corporativa:

- VM Windows 11 personal sin datos ni credenciales corporativas;
- Windows Service de laboratorio y Salud real por IPC;
- software libre redistribuible desde mirror local con SHA-256;
- backend, persistencia, portal e identidad de desarrollo;
- Hermes local con API externa usando contenido publico/sintetico;
- RAG e indice locales;
- operacion offline cerrada con politica y artefactos ya disponibles.

Pendiente para piloto:

- UEMS, Entra, OpenText y bot de Teams reales;
- Security/Sophos, PKI, firma y confianza de publicador;
- identidad restringida y despliegue corporativo del servicio;
- hosting, retencion, conectividad y paquetes corporativos aprobados.

## Stoppers futuros no bloqueantes

Estos stoppers condicionan solo la mejora posterior de refresco administrado de
politicas. No cambian el estado `completed` del Bloque 6 ni el gate local ya
validado.

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
Recomendacion: Mantener el adaptador y su politica sin cambios hasta una tarea
  separada; iniciar el Bloque 7 solo como nueva unidad.
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
