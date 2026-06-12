# Adaptadores de instalacion y configuracion

El Bloque 4 no implementa estos adaptadores. La accion
`software.install.simulated.v1` solo prueba IPC, autorizacion y estado; no llama
WinGet, MSI, MSIX, PowerShell ni procesos hijos.

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
