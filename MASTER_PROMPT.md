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
- Bloques 0 a 4 completados y publicados en `main`.
- Bloque 3 publicado en `b09f07a`.
- Bloque 4 publicado en `b56bfcb`.
- Bloque 5 es la siguiente unidad y no hay otro bloque principal activo.
- La shell WinUI usa catalogo y conversacion sinteticos; no crea tickets,
  solicitudes corporativas ni instalaciones reales.
- El DeviceAgent expone un protocolo IPC v1 tipado mediante Named Pipe, usa
  autorizacion deny-by-default y solo ejecuta un trabajo sintetico allowlisted.
- El estado de trabajos vive en SQLite local y soporta idempotencia,
  cancelacion, progreso y recuperacion simulada.
- La shell todavia no invoca el DeviceAgent.
- SDK fijado: .NET 10.0.301.
- `scripts/Validate.ps1` pasa completo: build Release sin warnings, 68 pruebas,
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
6. para el Bloque 5, lee `modules/DEVICE_AGENT.md` y
   `docs/modules/device-agent-ipc.md`;
7. consulta `docs/modules/desktop-shell.md` solo si integras el resultado con la
   vista Salud del equipo;
8. inspecciona `src/Contracts/Agent`, `src/DeviceAgent/Core`,
   `src/DeviceAgent` y sus pruebas antes de crear contratos o abstracciones.

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
- manten el nucleo comprobable sin dependencia de WinUI;
- corrige causas, no parches aislados, y actualiza pruebas y documentacion;
- no declares terminado un cambio sin build y pruebas aplicables.

Tarea:
Ejecuta el Bloque 5 de `DEVELOPMENT_PLAN.md`: diagnostico local de solo lectura.

Criterio de aceptacion:
- crear modelos y resultados tipados para un snapshot diagnostico acotado;
- recolectar mediante APIs .NET/Windows de solo lectura:
  - version de Windows y arquitectura;
  - capacidad y espacio disponible de almacenamiento, sin enumerar archivos;
  - memoria total y disponible;
  - estado de red y alcance de dominio con timeout y resultado explicito;
  - version del DeviceAgent;
  - prerrequisitos declarados para una accion tipada;
- separar contratos, casos de uso y colectores de infraestructura;
- hacer que fallos parciales produzcan estados tipados y saneados, sin abortar
  todo el snapshot ni filtrar excepciones o datos sensibles;
- mantener resultados deterministas en estructura, orden y unidades;
- extender IPC solo con mensajes versionados y allowlisted, sin campos de
  ejecucion libre;
- cubrir reglas, fallos parciales, cancelacion, limites y fronteras con pruebas
  unitarias, de contrato e integracion;
- actualizar `modules/DEVICE_AGENT.md` y crear o actualizar el documento tecnico
  propietario bajo `docs/modules/`;
- actualizar `CURRENT_CONTEXT.md` y `WORKFLOW.md`;
- si conectas Salud del equipo, mantener WinUI como adaptador fino y conservar
  una experiencia segura cuando el agente no este disponible;
- no persistir snapshots diagnosticos en SQLite salvo que un documento de mayor
  precedencia lo exija; la SQLite actual es para la cola tecnica de trabajos;
- no implementar remediacion, instalaciones, tickets, backend, portal,
  telemetria productiva, inventario general ni integraciones corporativas;
- no adelantar el adaptador real del Bloque 6.

Forma de trabajo:
1. confirma el Bloque 5 y define un incremento pequeno verificable;
2. inspecciona patrones, APIs disponibles y fronteras existentes;
3. registra cualquier contradiccion o decision que altere contratos publicos,
   seguridad, alcance o persistencia;
4. implementa el incremento completo;
5. actualiza pruebas y documentacion propietaria;
6. ejecuta el gate completo;
7. si cambian dependencias, regenera y valida los lockfiles;
8. informa archivos, comandos, resultados, riesgos residuales, estado
   documental y mensaje de commit sugerido.

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
