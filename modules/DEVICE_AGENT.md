# Gobierno del DeviceAgent

## Principio

El DeviceAgent es la unica frontera que puede ejecutar acciones locales. WinUI,
Teams, IA y backend solo pueden enviar contratos tipados y autorizados; nunca
envian shell, PowerShell libre, rutas arbitrarias ni argumentos generados.

## Implementacion local del Bloque 4

`src/Contracts/Agent` define el protocolo v1 compartido. Los unicos mensajes de
entrada son:

- `agent.job.start`;
- `agent.job.get`;
- `agent.job.cancel`.

`StartAgentJobRequest` contiene identificadores de solicitud, idempotencia,
accion, target y version. No expone comando, argumentos ni contenido ejecutable.

`src/DeviceAgent/Core` contiene:

- despacho deny-by-default por version y tipo de mensaje;
- allowlist exacta por accion, target y version;
- maquina de estados de trabajos simulados;
- persistencia SQLite local;
- progreso, cancelacion y evidencia saneada;
- recuperacion de trabajos interrumpidos;
- framing de mensajes con limite de 64 KiB;
- servidor Named Pipe restringido al usuario actual en desarrollo.

`src/DeviceAgent` es un host fino que registra el nucleo mediante DI. No contiene
adaptadores de instalacion ni acciones privilegiadas.

## Politica sintetica

La unica accion local permitida en este bloque es:

```text
software.install.simulated.v1 / secure-transfer / 6.5
```

Esta entrada existe solo para probar el protocolo y las reglas. No instala,
descarga, detecta ni modifica software. Cualquier otra combinacion se rechaza.

## Persistencia y recuperacion

El estado vive en:

```text
%LOCALAPPDATA%\ITSupportNative\DeviceAgent\jobs.db
```

La base SQLite conserva snapshots de trabajos e idempotency keys. Un trabajo
`Running` encontrado al reiniciar se devuelve a `Queued`, incrementa
`RecoveryCount`, agrega evidencia fija y puede continuar la simulacion.

El esquema local es tecnico y no contiene credenciales, salida de procesos,
inventario general ni datos corporativos.

## Seguridad del IPC

El perfil de desarrollo usa `PipeOptions.CurrentUserOnly`. Esto crea una ACL que
limita el pipe al usuario de Windows que hospeda el agente. La configuracion de
servicio productiva necesitara una ACL explicita entre la identidad restringida
del servicio y el grupo autorizado de clientes; no se reutilizara
`CurrentUserOnly` cuando ambos procesos usen identidades diferentes.

La ACL no sustituye la autorizacion del mensaje. El agente tambien valida:

- version de protocolo;
- tipo exacto de mensaje;
- identificadores ASCII acotados;
- accion, target y version allowlisted;
- idempotency key;
- estado valido para cancelacion.

## Fuera del alcance del Bloque 4

- instalaciones, desinstalaciones o diagnosticos reales;
- WinGet, MSI, MSIX o PowerShell firmado;
- trabajo emitido por backend;
- identidad de dispositivo o usuario corporativa;
- firma de politicas o mensajes;
- Windows Service instalado;
- sincronizacion de evidencia con API o tickets;
- conexion de la shell a una solicitud real.

Los diagnosticos locales de solo lectura empiezan en el Bloque 5. Los
adaptadores reales empiezan en el Bloque 6 y solo despues de validar el paquete
en una VM Windows 11.

## Capacidad posterior condicionada

Un flujo futuro puede recibir campanas explicitas para refrescar politicas de
equipo. El agente permanecera inactivo entre consultas HTTPS salientes,
aplicara jitter y no inferira cambios mediante ping o inspeccion general de AD.

La accion sera tipada y allowlisted. No aceptara switches enviados por backend,
no guardara credenciales FortiClient/Windows y no iniciara una sesion de tecnico.
La conexion automatica solo se habilitara si Network y Security aprueban un
mecanismo de maquina o pre-logon administrado.

Esta capacidad no forma parte del Bloque 5 ni cambia el diagnostico de solo
lectura. Se detalla en `../docs/modules/domain-policy-refresh.md`.
