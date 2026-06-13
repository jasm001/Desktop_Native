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
- Bloques 0 a 5 completados y publicados en `main`.
- Bloque 3 publicado en `b09f07a`.
- Bloque 4 publicado en `b56bfcb`.
- Bloque 5 publicado en `e3a0b8d`.
- Bloque 6 esta `in_progress`; no hay otro bloque principal activo.
- El incremento automatizado del Bloque 6 fue publicado en `f808425`.
- La shell WinUI usa catalogo y conversacion sinteticos; no crea tickets,
  solicitudes corporativas ni instalaciones reales.
- El DeviceAgent expone un protocolo IPC v1 tipado mediante Named Pipe y usa
  autorizacion deny-by-default. Conserva el trabajo sintetico allowlisted y
  agrega acciones reales de 7-Zip solo en el perfil exacto `local-demo`.
- El estado de trabajos vive en SQLite local y soporta idempotencia,
  cancelacion, progreso y recuperacion simulada.
- El DeviceAgent obtiene snapshots diagnosticos efimeros de solo lectura con
  fallos parciales tipados y prerrequisitos allowlisted.
- Existe `seven-zip.msi.v1` para 7-Zip 26.01 x64 con `Detect`, `Preflight`,
  `Install`, `Verify` y `Uninstall`, argumentos internos fijos, timeout,
  idempotencia, cancelacion segura y evidencia saneada.
- El manifiesto versionado fija el MSI oficial, longitud `2,002,432`, ProductCode
  `{23170F69-40C1-2702-2601-000001000000}` y SHA-256
  `A47EA8DCF8BC08E6DE474CAE77C828E031FA22CB528F6095DEFFFEBF11CD02F2`.
- La shell todavia no invoca el DeviceAgent.
- Existe una VM Windows 11 personal sin datos ni credenciales corporativas para
  pruebas de laboratorio.
- En la sesion anterior `Get-VM` fallo por permisos. El usuario ya agrego
  `DESKTOP-LDK3DDJ\ruruu` al grupo localizado
  `Administradores de Hyper-V` (`S-1-5-32-578`) y reiniciara el equipo.
- Al reanudar hay que verificar el token nuevo, la VM Windows 11 x64 y su
  checkpoint antes de copiar o ejecutar el MSI. No asumir que el acceso quedo
  activo solo por la pertenencia al grupo.
- El perfil `local-demo` permite un mirror local simulado solo para software
  libre redistribuible, con manifiesto y SHA-256; no es storage productivo.
- El roadmap local posterior contempla Windows Service, Salud real por IPC,
  API/persistencia local, Hermes con API externa opcional, RAG local y modo
  degradado sin conexion.
- Hermes con API externa no es IA offline. Sin conexion permanecen disponibles
  conocimiento local, flujos deterministas y acciones ya autorizadas con
  artefactos locales.
- SDK fijado: .NET 10.0.301.
- `scripts/Validate.ps1` pasa completo: build Release sin warnings, 110 pruebas,
  checks del workspace y escaneo de secretos.
- Los lockfiles Desktop/WindowsUi estan limitados al RID `win-x64`.
- Visual Studio 2022 no admite net10.0; para depuracion en IDE se requiere
  Visual Studio 2026 version 18.0 o posterior.

Antes de editar:
1. inspecciona `git status`, `git log -5 --oneline` y conserva cambios
   existentes;
2. lee `README.md` y aplica su precedencia documental;
3. lee `CURRENT_CONTEXT.md`, `WORKFLOW.md` y `DEVELOPMENT_PLAN.md`;
4. lee `core/SCOPE.md`, `core/STACK.md`, `core/ARCHITECTURE.md`,
   `core/SECURITY.md` y `core/DECISIONS.md`;
5. lee `standards/CODING_STANDARDS.md` y `standards/DELIVERY.md`;
6. para el Bloque 6, lee `modules/DEVICE_AGENT.md`,
   `modules/EXECUTION_ADAPTERS.md`, `modules/CATALOG.md`,
   `docs/modules/device-agent-ipc.md`,
   `docs/modules/read-only-device-diagnostics.md` y
   `docs/modules/local-mvp-lab.md`;
7. lee `docs/modules/seven-zip-adapter.md` y el manifiesto
   `deploy/local-demo/manifests/seven-zip-26.01-x64.json`;
8. inspecciona `src/Contracts/Agent`, `src/DeviceAgent/Core`,
   `src/DeviceAgent` y sus pruebas antes de crear contratos o abstracciones;
9. consulta `docs/modules/desktop-shell.md` solo si conectas estados del
   adaptador con WinUI.

Reglas no negociables:
- no cambies stack, alcance, arquitectura, persistencia o fronteras de seguridad
  sin registrar un stopper en `WORKFLOW.md`;
- WinUI, Teams y la IA nunca ejecutan comandos privilegiados;
- el DeviceAgent nunca acepta shell, PowerShell libre, rutas arbitrarias ni
  argumentos generados;
- los diagnosticos son de solo lectura, con privilegio minimo y deny-by-default;
- no explores archivos personales ni construyas inventario corporativo general;
- consultar nunca crea ticket, solicitud, instalacion o accion correctiva;
- no uses secretos, credenciales, datos corporativos ni endpoints productivos;
- usa contratos tipados, DI, nullable, async, `CancellationToken`, limites y
  resultados saneados;
- no ejecutes el primer adaptador real en la PC principal; usa una VM Windows 11
  desechable con snapshot/checkpoint;
- no descargues ni ejecutes un paquete hasta documentar origen oficial, version,
  arquitectura, licencia, hash o firma disponible y modo unattended soportado;
- para el mirror local, confirma que la licencia permite redistribucion y
  reempaquetado cuando aplique; no guardes instaladores en Git;
- no uses datos corporativos con Hermes o su API externa; claves y configuracion
  sensible viven fuera del repositorio;
- sin conexion, no inventes autorizaciones ni descargues artefactos ausentes;
  solo usa politica, acciones y artefactos ya disponibles;
- el adaptador fija comandos y parametros internamente; IPC, UI, backend e IA
  solo envian identificadores tipados y allowlisted;
- manten el nucleo comprobable sin dependencia de WinUI;
- corrige causas, no parches aislados, y actualiza pruebas y documentacion;
- no declares terminado un cambio sin build y pruebas aplicables.

Tarea:
Termina la seccion pendiente del Bloque 6 de `DEVELOPMENT_PLAN.md`: verificar
acceso Hyper-V despues del reinicio y ejecutar la matriz del adaptador
`seven-zip.msi.v1` en una VM Windows 11 desechable con checkpoint.

No redisenes ni reimplementes el adaptador salvo que una prueba real revele una
causa tecnica. La implementacion y las pruebas automatizadas ya estan
publicadas. No avances al Bloque 7.

Criterio de aceptacion:
- ya cubierto en `f808425`: paquete y version fijados, evidencia oficial,
  licencia, manifiesto y SHA-256;
- ya cubierto: adaptador cerrado con `Detect`, `Preflight`, `Install`, `Verify`
  y `Uninstall`;
- ya cubierto: timeout, codigos MSI, retry declarado, reinicio, argumentos
  fijos, saneamiento, autorizacion deny-by-default, idempotencia y cancelacion
  segura;
- ya cubierto: 110 pruebas automatizadas y `scripts/Validate.ps1` completo;
- pendiente: confirmar acceso Hyper-V con el token renovado;
- validar en una VM Windows 11 con snapshot inicial, instalacion, repeticion,
  verificacion, desinstalacion, fallo del mirror/hash y restauracion del
  snapshot;
- pendiente: registrar evidencia VM saneada y actualizar
  `docs/modules/seven-zip-adapter.md`, `modules/EXECUTION_ADAPTERS.md`,
  `modules/DEVICE_AGENT.md`, `CURRENT_CONTEXT.md` y `WORKFLOW.md`;
- mantener la accion sintetica existente y no cambiar el contrato IPC;
- no marcar el Bloque 6 `completed` antes de terminar la matriz y restaurar el
  checkpoint;
- no implementar todavia backend, tickets, portal, UEMS real, telemetria
  productiva, inventario general, remediaciones generales ni el Bloque 7;
- no mezclar Hermes/RAG, conexion WinUI-DeviceAgent o instalacion del Windows
  Service dentro del Bloque 6 salvo que se definan y validen como incrementos
  separados despues del gate del adaptador.

Forma de trabajo:
1. confirma que el Bloque 6 sigue `in_progress` y que `f808425` esta presente;
2. ejecuta `whoami /groups` y confirma que `S-1-5-32-578` esta habilitado;
3. ejecuta
   `Get-LocalGroupMember -Group 'Administradores de Hyper-V'`;
4. ejecuta `Get-VM` y `Get-VM | Get-VMSnapshot`; identifica una unica VM
   Windows 11 x64 desechable y su checkpoint inicial sin modificar otras VMs;
5. si el acceso sigue denegado despues del reinicio, registra la evidencia y no
   ejecutes el paquete en el host;
6. verifica el SHA-256 del MSI ignorado bajo `.artifacts/block6`; si falta,
   descargalo otra vez solo desde la URL oficial del manifiesto;
7. introduce cualquier credencial administrativa de laboratorio de forma
   interactiva; no la escribas en chat, Git, configuracion o scripts;
8. configura el mirror y `ExecutionProfile=local-demo` solo dentro de la VM;
9. ejecuta la matriz de `docs/modules/seven-zip-adapter.md`: estado inicial,
   preflight, instalacion, verificacion, repeticion idempotente, desinstalacion,
   repeticion, mirror ausente, hash corrupto, fallo controlado y restauracion;
10. registra evidencia saneada, tiempos y codigos tipados sin copiar rutas,
    stdout, stderr, nombres internos, usuario, host o secretos;
11. ejecuta `scripts/Validate.ps1`;
12. solo si toda la matriz y la restauracion pasan, marca el Bloque 6
    `completed`, actualiza documentos y propone el commit de cierre;
13. informa archivos, comandos, resultados, evidencia de VM, riesgos
    residuales, estado documental y mensaje de commit sugerido.

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
.\scripts\Test-Secrets.ps1

Si detectas una contradiccion, aplica la precedencia de `README.md`, registra la
evidencia y no decidas silenciosamente.
```
