# Workflow de desarrollo

## Estado

- Fase actual: capacidades locales controladas para Windows 11.
- Bloque activo: 6. Primer adaptador en VM (`in_progress`).
- Ultimo resultado: adaptador cerrado para 7-Zip 26.01 x64 MSI publicado en
  `f808425`, con manifiesto, integridad, autorizacion y pruebas automatizadas.
- Resultado en curso: renovar el token de Windows despues de agregar al usuario
  a `Administradores de Hyper-V`, verificar VM/checkpoint y completar la matriz
  real del adaptador.
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
- `scripts/Test-Secrets.ps1`: correcto, sin hallazgos.
- `scripts/Validate.ps1`: correcto.
- Lockfiles sin cambios; Desktop/WindowsUi conservan unicamente `win-x64`.
- Matriz VM: pendiente. El usuario agrego `DESKTOP-LDK3DDJ\ruruu` al grupo
  localizado `Administradores de Hyper-V`; falta reiniciar o cerrar sesion,
  abrir una sesion nueva y verificar que el SID `S-1-5-32-578` este habilitado.
- El MSI no se ha ejecutado en la PC principal y el Bloque 6 sigue
  `in_progress`.

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
| 6. Primer adaptador en VM | in_progress | Adaptador 7-Zip 26.01 x64, manifiesto y 110 pruebas publicados en `f808425`; acceso Hyper-V agregado, pendiente renovar token y completar matriz VM. |
| 7. API compartida y persistencia | pending | |
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
politicas. No cambian el estado `in_progress` del Bloque 6 ni bloquean la matriz
local del adaptador.

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

## Prerrequisito operativo pendiente del Bloque 6

```text
Fecha: 2026-06-13
Modulo: DeviceAgent / primer adaptador real
Accion pendiente: Reiniciar el host o cerrar completamente la sesion de Windows,
  volver a iniciar sesion y abrir un chat nuevo para renovar el token del
  usuario.
Evidencia: `Get-VM` y `Get-VM | Get-VMSnapshot` devuelven que el usuario no
  dispone del permiso necesario sobre la directiva de autorizacion Hyper-V del
  host `DESKTOP-LDK3DDJ` en la sesion anterior. El usuario confirmo despues que
  agrego `DESKTOP-LDK3DDJ\ruruu` al grupo local
  `Administradores de Hyper-V` (`S-1-5-32-578`).
Verificacion al reanudar: Confirmar que `whoami /groups` muestra
  `S-1-5-32-578` habilitado; consultar `Get-VM`; consultar
  `Get-VM | Get-VMSnapshot`; identificar una VM Windows 11 desechable y un
  checkpoint inicial antes de copiar o ejecutar el MSI.
Impacto: Se puede implementar y validar el adaptador con dobles de proceso y
  artefacto, pero el Bloque 6 no puede marcarse `completed` ni el MSI puede
  ejecutarse hasta completar instalacion, repeticion, verificacion,
  desinstalacion, fallos de mirror/hash y restauracion de checkpoint en la VM.
Recomendacion: Reiniciar el equipo, verificar acceso en una sesion nueva y
  mantener el bloque `in_progress` hasta cerrar la matriz. No desactivar UAC,
  no ejecutar Codex permanentemente elevado y no ejecutar el paquete en el
  host.
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
