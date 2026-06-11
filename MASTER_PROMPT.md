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
- el working tree puede contener la auditoria/handoff documental y una
  correccion validada de lockfiles x64; inspeccionalos y no los descartes.

Estado confirmado:
- Bloque 0 completado: fundacion del repositorio.
- Bloque 1 completado: shell WinUI con Inicio, Catalogo, Asistente, Solicitudes
  y Salud del equipo.
- Bloque 2 es la siguiente unidad y no hay otro bloque principal activo.
- La shell usa datos sinteticos y no crea solicitudes, tickets, instalaciones o
  acciones reales.
- SDK fijado: .NET 10.0.301.
- La CLI global restaura, compila, prueba y ejecuta el cliente.
- `scripts/Validate.ps1` pasa completo con los lockfiles Desktop/WindowsUi
  limitados al RID declarado `win-x64`.
- Visual Studio 2022 no admite net10.0; para depuracion en IDE se requiere
  Visual Studio 2026 version 18.0 o posterior.

Antes de editar:
1. inspecciona `git status` y conserva cambios existentes;
2. lee `CURRENT_CONTEXT.md`, `WORKFLOW.md` y `DEVELOPMENT_PLAN.md`;
3. lee la precedencia de `README.md`;
4. lee `core/SCOPE.md`, `core/STACK.md`, `core/ARCHITECTURE.md`,
   `core/SECURITY.md` y `core/DECISIONS.md`;
5. lee `standards/CODING_STANDARDS.md` y `standards/DELIVERY.md`;
6. para el Bloque 2, lee `modules/CATALOG.md`;
7. consulta `docs/modules/desktop-shell.md` solo para integrar el dominio con la
   shell existente.

Reglas no negociables:
- no cambies stack, alcance, arquitectura o fronteras de seguridad sin registrar
  un stopper en `WORKFLOW.md`;
- WinUI, Teams y la IA nunca ejecutan comandos privilegiados;
- el DeviceAgent nunca acepta shell, PowerShell libre o argumentos generados;
- consultar nunca crea ticket, solicitud o instalacion;
- toda accion futura debe ser tipada, autorizada, idempotente, auditable y
  cancelable cuando sea seguro;
- no uses secretos, datos corporativos, credenciales ni endpoints productivos;
- usa MVVM, DI, nullable, async, accesibilidad y modulos pequenos;
- corrige causas, no parches aislados, y actualiza pruebas y documentacion;
- no declares terminado un cambio sin build y pruebas aplicables.

Tarea:
Ejecuta el Bloque 2 de `DEVELOPMENT_PLAN.md`.

Criterio de aceptacion:
- crear modelos y casos de uso puros para producto, version, licencia, estado y
  alternativas;
- representar aprobado, comercial, no listado, EOL y prohibido;
- implementar busqueda y filtros deterministas;
- devolver decisiones tipadas: informar, proponer, escalar o rechazar;
- usar fixtures sinteticos pequenos;
- cubrir reglas y casos limite con pruebas unitarias;
- mantener el dominio sin dependencias de WinUI, Prisma, IA o integraciones;
- no importar todavia el Excel corporativo;
- no implementar instalaciones, tickets, solicitudes, IPC, agente real, backend
  ni portal administrativo;
- si conectas la shell, mantenla como adaptador fino y no dupliques reglas en los
  ViewModels.

Forma de trabajo:
1. confirma el bloque y define un incremento pequeno verificable;
2. inspecciona patrones y fronteras existentes antes de crear proyectos o
   abstracciones;
3. implementa el incremento completo;
4. actualiza `modules/CATALOG.md`, el documento tecnico propietario que
   corresponda, `CURRENT_CONTEXT.md` y `WORKFLOW.md`;
5. ejecuta restore bloqueado, formato, build Release, pruebas, workspace checks
   y escaneo de secretos;
6. si cambian dependencias, regenera y valida los lockfiles;
7. informa archivos, comandos, resultados, riesgos, estado documental y mensaje
   de commit sugerido.

Comandos base:
dotnet restore ITSupportNative.slnx --locked-mode
dotnet format ITSupportNative.slnx --verify-no-changes --no-restore
dotnet build ITSupportNative.slnx --configuration Release --no-restore
dotnet test ITSupportNative.slnx --configuration Release --no-build
corepack pnpm@11.5.3 run check
.\scripts\Test-Secrets.ps1

Si detectas una contradiccion, aplica la precedencia de `README.md`, registra la
evidencia y no decidas silenciosamente.
```
