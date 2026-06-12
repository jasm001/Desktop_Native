# Gobierno del DeviceAgent

## Principio

El DeviceAgent es la unica frontera que puede ejecutar acciones locales. WinUI,
Teams, IA y backend solo pueden enviar contratos tipados y autorizados; nunca
envian shell, PowerShell libre, rutas arbitrarias ni argumentos generados.

## Implementacion local de los Bloques 4 y 5

`src/Contracts/Agent` define el protocolo v1 compartido. Los unicos mensajes de
entrada son:

- `agent.job.start`;
- `agent.job.get`;
- `agent.job.cancel`;
- `agent.diagnostics.get`.

`StartAgentJobRequest` contiene identificadores de solicitud, idempotencia,
accion, target y version. No expone comando, argumentos ni contenido ejecutable.

`src/DeviceAgent/Core` contiene:

- despacho deny-by-default por version y tipo de mensaje;
- allowlist exacta por accion, target y version;
- caso de uso de diagnostico independiente de persistencia;
- colectores de Windows, almacenamiento, memoria, red y version;
- evaluacion determinista de prerrequisitos declarados;
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

La misma entrada declara prerrequisitos sinteticos y acotados para demostrar la
evaluacion tipada: Windows, arquitectura x64, 1 GiB disponible, 512 MiB de
memoria disponible y red activa. Declarar un prerrequisito no autoriza ni
ejecuta la accion.

## Diagnostico de solo lectura

`agent.diagnostics.get` recibe solo `actionId`, `targetId` y `targetVersion`.
La combinacion debe existir exactamente en la politica instalada. La respuesta
`agent.diagnostics.snapshot` conserva secciones fijas:

- version numerica de Windows y arquitectura;
- capacidad y espacio disponible agregados de volumenes fijos;
- memoria fisica total y disponible;
- disponibilidad de red y alcance saneado del dominio;
- version del DeviceAgent;
- prerrequisitos ordenados con estado `Satisfied`, `NotSatisfied` o `Unknown`.

Los valores de capacidad usan bytes. No se devuelven letras o nombres de
unidad, dominio, host, interfaz, IP, excepciones ni rutas. El probe de dominio
resuelve con timeout el sufijo de dominio reportado por Windows y solo devuelve
un estado tipado; no valida controladores, SYSVOL, NETLOGON ni salud de GPO.

Cada colector falla de forma independiente. Un fallo se transforma en
`Unavailable`, `TimedOut` o `NotApplicable` con un codigo fijo y no aborta las
otras secciones. La cancelacion solicitada por el caller si cancela el snapshot
completo.

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

Los snapshots diagnosticos no se escriben en SQLite. Se construyen bajo demanda
y se devuelven por IPC; la base local sigue siendo exclusiva de la cola durable
de trabajos.

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

## Fuera del alcance despues del Bloque 5

- instalaciones, desinstalaciones o remediaciones reales;
- WinGet, MSI, MSIX o PowerShell firmado;
- trabajo emitido por backend;
- identidad de dispositivo o usuario corporativa;
- firma de politicas o mensajes;
- Windows Service instalado;
- sincronizacion de evidencia con API o tickets;
- conexion de la shell a una solicitud real.

Los adaptadores reales empiezan en el Bloque 6 y solo despues de validar el
paquete en una VM Windows 11.

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

## Roadmap de capacidades locales

El orden posterior previsto es:

1. diagnostico de solo lectura;
2. paquete de evidencia para soporte;
3. reparaciones locales firmadas y cerradas;
4. limpieza segura de almacenamiento;
5. diagnostico guiado de perifericos;
6. coordinador de mantenimiento y reinicios;
7. perfiles declarativos de ambientacion;
8. operacion local limitada sin backend o IA;
9. campanas de actualizacion del producto;
10. seleccion de aplicaciones para reprovisionamiento;
11. borrado y reprovisionamiento.

El agente no carga scripts arbitrarios, incluso cuando no hay Internet. Las
acciones offline forman parte de un paquete firmado, versionado y probado. Un
RAG local puede explicar resultados o elegir entre acciones allowlisted, pero no
produce contenido ejecutable.

Los detalles viven en `../docs/modules/endpoint-self-service.md` y
`../docs/modules/device-reprovisioning.md`.
