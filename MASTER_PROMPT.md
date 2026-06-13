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
- Bloque 6 es la siguiente unidad y no hay otro bloque principal activo.
- La shell WinUI usa catalogo y conversacion sinteticos; no crea tickets,
  solicitudes corporativas ni instalaciones reales.
- El DeviceAgent expone un protocolo IPC v1 tipado mediante Named Pipe, usa
  autorizacion deny-by-default y solo ejecuta un trabajo sintetico allowlisted.
- El estado de trabajos vive en SQLite local y soporta idempotencia,
  cancelacion, progreso y recuperacion simulada.
- El DeviceAgent obtiene snapshots diagnosticos efimeros de solo lectura con
  fallos parciales tipados y prerrequisitos allowlisted.
- La shell todavia no invoca el DeviceAgent.
- Existe una VM Windows 11 personal sin datos ni credenciales corporativas para
  pruebas de laboratorio.
- El perfil `local-demo` permite un mirror local simulado solo para software
  libre redistribuible, con manifiesto y SHA-256; no es storage productivo.
- El roadmap local posterior contempla Windows Service, Salud real por IPC,
  API/persistencia local, Hermes con API externa opcional, RAG local y modo
  degradado sin conexion.
- Hermes con API externa no es IA offline. Sin conexion permanecen disponibles
  conocimiento local, flujos deterministas y acciones ya autorizadas con
  artefactos locales.
- SDK fijado: .NET 10.0.301.
- `scripts/Validate.ps1` pasa completo: build Release sin warnings, 82 pruebas,
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
7. inspecciona `src/Contracts/Agent`, `src/DeviceAgent/Core`,
   `src/DeviceAgent` y sus pruebas antes de crear contratos o abstracciones;
8. consulta `docs/modules/desktop-shell.md` solo si conectas estados del
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
Ejecuta el Bloque 6 de `DEVELOPMENT_PLAN.md`: primer adaptador real validado en
una VM Windows 11.

Criterio de aceptacion:
- seleccionar un paquete libre, aprobado para la prueba, silencioso, de bajo
  riesgo y sin credenciales, drivers ni reinicio obligatorio;
- registrar version fijada, arquitecturas soportadas, origen oficial, licencia,
  hash o firma disponible y evidencia del modo unattended;
- servir el artefacto desde un origen oficial o desde el mirror local de
  laboratorio con manifiesto versionado y SHA-256 obligatorio;
- crear un adaptador cerrado y versionado con `Detect`, `Preflight`, `Install`,
  `Verify` y `Uninstall`;
- declarar timeout, codigos de salida, politica de retry y comportamiento de
  reinicio;
- usar una lista fija de ejecutable y argumentos; no aceptar comando, ruta,
  switches ni argumentos desde IPC, UI, backend o IA;
- verificar origen e integridad antes de ejecutar y sanear stdout, stderr,
  rutas, nombres internos y excepciones;
- mantener autorizacion deny-by-default por accion, target, version y adaptador;
- conservar idempotencia: repetir `Install` sobre el estado objetivo no duplica
  ni corrompe la instalacion;
- soportar cancelacion solo donde sea segura y devolver estado tipado cuando no
  pueda interrumpirse;
- cubrir seleccion, autorizacion, preflight, idempotencia, timeout, codigos de
  salida, fallo controlado, verificacion y desinstalacion con pruebas;
- validar en una VM Windows 11 con snapshot inicial, instalacion, repeticion,
  verificacion, desinstalacion, fallo del mirror/hash y restauracion del
  snapshot;
- mantener la accion sintetica existente para pruebas que no requieren VM;
- actualizar `modules/EXECUTION_ADAPTERS.md`, `modules/DEVICE_AGENT.md` y crear
  o actualizar el documento tecnico propietario bajo `docs/modules/`;
- actualizar `CURRENT_CONTEXT.md` y `WORKFLOW.md` con evidencia real, sin marcar
  el Bloque 6 `completed` antes de terminar la matriz en VM;
- no implementar todavia backend, tickets, portal, UEMS real, telemetria
  productiva, inventario general, remediaciones generales ni el Bloque 7;
- no mezclar Hermes/RAG, conexion WinUI-DeviceAgent o instalacion del Windows
  Service dentro del Bloque 6 salvo que se definan y validen como incrementos
  separados despues del gate del adaptador.

Forma de trabajo:
1. confirma que el Bloque 6 es la unidad activa y define un solo paquete y
   version como incremento;
2. verifica que existe una VM Windows 11 desechable con snapshot; si no existe,
   registra el bloqueo y no ejecutes el paquete en el host;
3. documenta la evidencia oficial del paquete y su modo unattended antes de
   descargar o ejecutar;
4. inspecciona patrones, contratos y fronteras existentes;
5. registra cualquier contradiccion o decision que altere contratos publicos,
   seguridad, alcance, privilegios o persistencia;
6. implementa el adaptador completo y sus pruebas automatizadas;
7. ejecuta el gate del repositorio y la matriz manual/automatizada en la VM;
8. actualiza documentacion propietaria y seguimiento con evidencia verificable;
9. si cambian dependencias, regenera y valida los lockfiles;
10. informa archivos, comandos, resultados, evidencia de VM, riesgos
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
