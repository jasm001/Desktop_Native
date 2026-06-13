# Adaptadores de instalacion y configuracion

Los Bloques 4 y 5 no implementaron adaptadores reales. La accion
`software.install.simulated.v1` se conserva para probar IPC, autorizacion,
estado y recuperacion sin llamar procesos hijos.

El Bloque 6 esta `in_progress` con el primer adaptador cerrado:

```text
seven-zip.msi.v1
  install: software.install.7zip.v1 / seven-zip / 26.01
  uninstall: software.uninstall.7zip.v1 / seven-zip / 26.01
```

La implementacion, manifiesto y pruebas automatizadas se publicaron en
`f808425`, pero el bloque no esta completado. El usuario agrego su cuenta a
`Administradores de Hyper-V`; falta reiniciar o renovar la sesion, verificar la
VM y su checkpoint y ejecutar la matriz. El MSI no se ha ejecutado en el host.

## Adaptador 7-Zip 26.01

El incremento fija el MSI x64 oficial:

- archivo `7z2601-x64.msi`;
- longitud `2,002,432` bytes;
- SHA-256
  `A47EA8DCF8BC08E6DE474CAE77C828E031FA22CB528F6095DEFFFEBF11CD02F2`;
- ProductCode `{23170F69-40C1-2702-2601-000001000000}`;
- version MSI `26.01.00.0`;
- sin firma Authenticode;
- licencia GNU LGPL con componentes BSD y restriccion unRAR;
- redistribucion binaria condicionada a reproducir los avisos relacionados.

El manifiesto vive en
`deploy/local-demo/manifests/seven-zip-26.01-x64.json`. El binario y
`license.txt` viven fuera de Git en el mirror de laboratorio.

`SevenZip2601X64Adapter` implementa `Detect`, `Preflight`, `Install`, `Verify` y
`Uninstall`. El perfil de ejecucion es `disabled` por defecto y solo acepta el
valor exacto `local-demo`; fuera de ese perfil las acciones reales no se cargan
en la politica de autorizacion.

La lista de procesos y argumentos esta fijada dentro del adaptador:

```text
%SystemRoot%\System32\msiexec.exe
  /i <artefacto resuelto y validado> /qn /norestart
  /x {23170F69-40C1-2702-2601-000001000000} /qn /norestart
```

La ruta del artefacto se obtiene de un root local confiable y del nombre fijo
del manifiesto. No llega desde IPC. Antes de instalar se valida perfil,
Windows x64, longitud y SHA-256. La deteccion lee la entrada MSI allowlisted en
el registro de desinstalacion de 64 bits y exige la version exacta.

La operacion tiene timeout fijo de cinco minutos. No existe retry interno:
`1618`, timeout y cualquier salida no allowlisted fallan cerrados y requieren
una nueva solicitud explicita despues de detectar el estado. `0` es exito;
`3010` es exito verificado con reinicio pendiente; `1641` se rechaza porque
indica un reinicio iniciado pese a `/norestart`.

La cancelacion es segura mientras el trabajo sigue `Queued`. Una vez iniciado
`msiexec`, IPC devuelve `JobNotCancellable`; el runner solo termina el proceso
por timeout interno. stdout y stderr se consumen y descartan. La evidencia
persistida contiene exclusivamente codigos y textos fijos, sin rutas, hash,
ProductCode, argumentos, salida o excepciones.

El detalle y la matriz VM viven en
`../docs/modules/seven-zip-adapter.md`.

## Laboratorio local

El primer adaptador puede consumir un artefacto desde un mirror local simulado
para probar el recorrido completo sin UEMS. El mirror vive fuera del repositorio
y sirve solo software libre con redistribucion permitida.

El adaptador no recibe URL, ruta, ejecutable o switches desde WinUI, Hermes,
backend o IPC. Resuelve internamente un `artifactId` versionado contra un
manifiesto allowlisted y verifica SHA-256 antes de ejecutar.

La matriz de laboratorio cubre:

- mirror disponible y artefacto valido;
- mirror ausente o timeout;
- hash corrupto;
- `Detect` antes y despues;
- `Preflight`;
- `Install` y repeticion idempotente;
- `Verify`;
- `Uninstall`;
- restauracion del snapshot de VM.

La cobertura automatizada usa dobles de artefacto, deteccion y proceso; nunca
ejecuta `msiexec`. La ejecucion real sigue reservada a la VM.

Un paquete reempaquetado se conserva solo si su licencia permite modificacion y
redistribucion. El resultado no se promueve fuera del laboratorio sin revision
de licencia, seguridad, firma y proceso de distribucion.

## Objetivo

Automatizar instalaciones configurables sin dar al modelo control libre del
escritorio, mouse, teclado o shell administrativo.

No se adopta un agente tipo OpenClaw para operar instaladores privilegiados. Un
controlador visual general es fragil ante cambios de version, idioma, escala,
ventanas inesperadas y dialogos de seguridad. Ademas convierte cualquier error
de clasificacion o prompt injection en acciones administrativas.

## Orden de preferencia

1. MSI/MSIX con propiedades documentadas.
2. CLI silenciosa oficial del fabricante.
3. Archivo de respuesta/configuracion.
4. Layout offline o bundle corporativo.
5. Transform MST firmado y versionado.
6. WinGet Configuration/DSC revisado y fijado.
7. Adaptador propio C# o PowerShell firmado, sin texto arbitrario.
8. Instalacion manual/escalada.

La automatizacion visual solo puede usarse en laboratorio para descubrir pasos o
probar un paquete, nunca como mecanismo productivo en endpoints.

## Contrato de adaptador

Cada `SoftwareExecutionAdapter` define:

- producto y versiones soportadas;
- arquitecturas y prerrequisitos;
- origen y hashes;
- parametros permitidos;
- secretos requeridos, normalmente ninguno;
- `Detect`, `Preflight`, `Install`, `Configure`, `Verify`, `Repair`, `Uninstall`;
- codigos de salida;
- timeout y politica de reinicio;
- rollback o compensacion;
- archivos/registro/servicios que puede modificar;
- evidencia saneada;
- pruebas en VM limpia y Windows 11 objetivo.

El agente elige el adaptador por ID/version. La IA no construye argumentos.

## SQL Server

SQL Server Setup permite instalacion desde command prompt y
`/CONFIGURATIONFILE`. El paquete corporativo:

- fija edicion, features, instance name, rutas y autenticacion permitida;
- acepta licencia de forma explicita en el manifiesto aprobado;
- no incluye passwords en `ConfigurationFile.ini`;
- usa secretos efimeros desde un canal protegido si fueran necesarios;
- verifica servicios, version, puertos y estado posterior;
- define si el usuario puede elegir entre perfiles previamente aprobados.

No se muestra el wizard al usuario para que el bot improvise opciones.

## SSMS

SSMS admite comandos de install/update/repair/uninstall, `--quiet`,
`--passive`, `--config`, `--installPath`, layouts y `--norestart`.

El adaptador puede ofrecer perfiles cerrados:

- instalacion estandar;
- instalacion desde layout offline;
- actualizar;
- reparar;
- desinstalar.

## Git for Windows

Git for Windows documenta flags de instalacion silenciosa. Las decisiones como
PATH, terminal, credential manager o asociaciones se guardan como un perfil de
paquete revisado, no se eligen mediante clicks durante la solicitud.

## Configuracion posterior

Para cambios como archivos, registro, variables, servicios o features:

- preferir DSC/configuracion declarativa;
- separar `Install` de `Configure`;
- implementar `Get/Test/Set` para idempotencia;
- restringir rutas y claves;
- guardar estado previo cuando exista rollback seguro.

Los cambios al archivo `hosts`, certificados, firewall o seguridad requieren una
politica especifica y aprobacion adicional.

## Casos sin modo unattended

Si un instalador no ofrece mecanismo soportado:

1. buscar empaquetado empresarial oficial;
2. solicitar al fabricante switches/response file;
3. capturar/reempaquetar con una herramienta empresarial;
4. ofrecer alternativa aprobada;
5. escalar instalacion manual.

## Reempaquetado

No siempre se necesita que el fabricante modifique el instalador. Microsoft MSIX
Packaging Tool puede ejecutar una vez el wizard en una VM limpia, capturar los
cambios y producir un MSIX. La instalacion posterior ya no repite los clicks.

Requiere:

- EULA/licencia que permita reempaquetado y distribucion interna;
- VM limpia de la arquitectura objetivo;
- revisar servicios, drivers, shell extensions y custom actions;
- firma del paquete resultante;
- pruebas de install/update/uninstall/rollback;
- repetir el empaquetado cuando cambie la version.

No todos los instaladores convierten bien. Drivers, servicios complejos,
licenciamiento ligado a hardware, launchers autoactualizables o cambios profundos
del sistema pueden exigir el instalador original o trabajo manual.

## Automatizacion visual encapsulada

Puede existir un wrapper interno con UI Automation/AutoIt que complete un wizard.
No es equivalente a entregar esas herramientas al bot, pero sigue dependiendo de
version, idioma, escala y ventanas inesperadas.

Solo se evalua cuando:

- MSIX/captura no funciona;
- no existe CLI, response file ni MST;
- Security y licenciamiento lo aprueban;
- version e idioma quedan fijados;
- el wrapper falla cerrado ante cualquier desviacion;
- corre como paquete probado, no como herramienta generica;
- no introduce ni captura credenciales.

El bot invoca el ID del paquete aprobado. No controla los clicks ni recibe una
API general de automatizacion visual.

## Reparaciones y perfiles declarativos

Las reparaciones locales usan el mismo contrato cerrado que los instaladores:

- `Detect`;
- `Preflight`;
- `Remediate`;
- `Verify`;
- `Rollback` o compensacion declarada;
- timeout, cancelacion y evidencia saneada.

Casos iniciales posteriores al MVP:

- servicio conocido detenido;
- cola de impresion en estado reparable;
- componentes de Windows Update;
- cache de una aplicacion expresamente soportada;
- limpieza de ubicaciones temporales allowlisted;
- comprobaciones de camara, audio, Bluetooth, pantalla, impresora y dock.

No se aceptan scripts enviados por IA, portal o usuario. Los bundles offline
estan firmados y versionados.

Los perfiles de ambientacion declaran un resultado deseado, por ejemplo
`office-standard.v1` o `developer-windows.v1`. Incluyen aplicaciones, ajustes
permitidos, prerrequisitos y verificaciones; no contienen credenciales ni copias
de archivos personales.
