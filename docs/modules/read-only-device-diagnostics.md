# Diagnostico local de solo lectura

## Responsabilidad

El Bloque 5 obtiene un snapshot tecnico pequeno desde el DeviceAgent. La
operacion informa estado y evalua prerrequisitos declarados; no crea trabajo,
solicitud, ticket, instalacion, remediacion ni inventario general.

## Contrato

Solicitud IPC:

```text
agent.diagnostics.get
  actionId
  targetId
  targetVersion
```

La referencia debe coincidir exactamente con una accion instalada en la
allowlist. No se aceptan campos de ejecucion libre.

Respuesta:

```text
agent.diagnostics.snapshot
  capturedAtUtc
  windows
  storage
  memory
  network
  agent
  actionPrerequisites[]
```

Cada seccion usa un `DiagnosticCollectionStatus` y un codigo fijo. Las unidades
de almacenamiento y memoria son bytes. Los prerrequisitos se ordenan por
`AgentPrerequisiteKind`.

## Colectores

`WindowsDiagnosticCollector` usa `Environment.OSVersion` y
`RuntimeInformation.OSArchitecture`.

`StorageDiagnosticCollector` usa `DriveInfo` sobre volumenes fijos listos,
agrega capacidad y espacio disponible y rechaza mas de 32 volumenes. No enumera
directorios ni archivos y no devuelve nombres o letras de unidad.

`MemoryDiagnosticCollector` usa `GlobalMemoryStatusEx` de `kernel32.dll` para
memoria fisica total y disponible.

`NetworkDiagnosticCollector` usa `NetworkInterface.GetIsNetworkAvailable` y un
probe de dominio con timeout. El probe lee el sufijo de dominio mediante
`IPGlobalProperties`, intenta resolverlo con `Dns` y descarta el nombre y las
direcciones. Los unicos resultados son `Reachable`, `Unreachable`, `TimedOut`,
`NotApplicable` o `Unknown`.

Esta comprobacion no demuestra salud de Active Directory, controlador de
dominio, SYSVOL, NETLOGON, Kerberos o GPO. Es una senal acotada para
prerrequisitos; las comprobaciones corporativas profundas siguen fuera del
Bloque 5.

`AgentVersionDiagnosticCollector` devuelve la version numerica del ensamblado
del nucleo del DeviceAgent.

## Fallos y cancelacion

Los cinco colectores se ejecutan de forma independiente. Una excepcion se
convierte en un resultado saneado `Unavailable`; no se copia el mensaje, tipo,
stack trace ni dato de entrada. Un timeout de dominio produce un estado
`TimedOut` explicito sin ocultar que la red local si estaba disponible.

La cancelacion del caller se propaga y cancela el snapshot completo. No se
devuelve un snapshot parcial despues de una cancelacion solicitada.

## Prerrequisitos

La politica allowlisted declara requisitos por accion. La accion sintetica del
Bloque 4 declara:

- Windows;
- x64;
- 1 GiB de almacenamiento disponible;
- 512 MiB de memoria disponible;
- red disponible.

Cada resultado es `Satisfied`, `NotSatisfied` o `Unknown`. Un diagnostico
ausente produce `Unknown`, no un rechazo inventado. La evaluacion no ejecuta ni
autoriza la accion.

## Privacidad y persistencia

El snapshot no contiene:

- hostname, usuario o identidad corporativa;
- nombre de dominio, controlador, interfaz, IP o DNS;
- letra, etiqueta o ruta de volumen;
- archivos personales o conteos de archivos;
- logs, excepciones o salida de procesos;
- secretos, tokens o credenciales.

El snapshot no se guarda en SQLite. `jobs.db` sigue dedicado a trabajos e
idempotency keys.

## Pruebas

- unitarias para modelos, secciones tipadas y unidades;
- de contrato para serializacion, estructura fija y ausencia de campos libres;
- de integracion para orden y estados de prerrequisitos, allowlist, fallos
  parciales, saneamiento, cancelacion, limites, colectores Windows y frontera de
  persistencia;
- intercambio real por Named Pipe para `agent.diagnostics.get`.
