# Plan modular de inicio

## Estrategia

El orden reduce riesgo: primero experiencia nativa y reglas puras, despues IPC y
ejecucion controlada, luego backend e integraciones, y al final administracion.
Cada bloque debe producir algo demostrable sin depender de servicios
corporativos aun no aprobados.

## Ruta de demostracion local

Mientras se resuelve la aprobacion corporativa, los Bloques 6-11 pueden avanzar
en una VM Windows 11 y servicios locales usando datos publicos/sinteticos,
identidad de desarrollo y adaptadores fake.

La demostracion local puede incluir:

- DeviceAgent como Windows Service de laboratorio y diagnosticos reales en
  WinUI;
- mirror local simulado para software libre redistribuible;
- API, PostgreSQL, worker y portal locales;
- Hermes local con API externa opcional y RAG local curado;
- continuidad sin conexion mediante conocimiento, politicas, autorizaciones,
  acciones y artefactos ya disponibles.

El orden funcional no cambia. Si una integracion queda `blocked` por evidencia
externa, D-072 permite preparar el siguiente bloque con proveedores locales y
reemplazables, sin declarar cerrado el gate anterior. Los gates corporativos de
UEMS, Security, Entra, OpenText, Teams, PKI, hosting y conectividad permanecen
pendientes y se cierran antes del piloto.

El detalle vive en `docs/modules/local-mvp-lab.md`.

## Bloque 0: fundacion del repositorio

Crear la solucion y workspace descritos en `standards/DELIVERY.md`.

Entregables:

- repositorio local conectado de forma segura a
  `https://github.com/jasm001/Desktop_Native.git`;
- rama principal `main`, `.gitignore` y `README.md` inicial;
- solucion .NET y workspace Node;
- proyectos vacios por frontera;
- configuracion nullable, warnings como errores y analyzers;
- formato, pruebas unitarias y escaneo de secretos;
- documentos operativos `Context` y `workflow`, mas estructura `docs/modules`.

Gate: se verifico el estado remoto, restore, build y pruebas vacias pasan en
Windows, y el primer commit puede publicarse en `main` sin sobrescribir historia.

## Bloque 1: shell nativo WinUI

Implementar navegacion, tema, accesibilidad y layout adaptable con datos fijos.

Vistas iniciales:

- Inicio;
- Catalogo;
- Asistente;
- Solicitudes;
- Salud del equipo.

Gate: smoke test, teclado, tema claro/oscuro y escalado principal verificados.

## Bloque 2: dominio y catalogo sintetico

Crear modelos y casos de uso puros para:

- producto, version, licencia y estado;
- aprobado, comercial, no listado, EOL y prohibido;
- alternativas;
- busqueda y filtros;
- decision `informar`, `proponer`, `escalar` o `rechazar`.

Usar fixtures pequenos. El importador del Excel se agrega despues de estabilizar
el modelo.

Gate: pruebas unitarias cubren reglas y no existe dependencia de UI, Prisma o IA.

## Bloque 3: conversacion determinista

Implementar estados:

- `query`;
- `proposal`;
- `confirmation_required`;
- `request_created`;
- `cancelled`.

La primera version usa intents y opciones fijas. No necesita LLM.

Gate: ninguna consulta crea solicitud; reintentos no duplican acciones.

## Bloque 4: agente simulado e IPC

Crear el contrato versionado entre WinUI y DeviceAgent, inicialmente fake.

Entregables:

- mensajes tipados;
- Named Pipe con ACL en perfil de desarrollo;
- maquina de estados del trabajo;
- progreso, cancelacion y evidencia saneada;
- recuperacion simulada tras reinicio.

Gate: pruebas de contrato y autorizacion rechazan mensajes desconocidos.

## Bloque 5: diagnostico de solo lectura

Agregar colectores sin privilegios destructivos:

- Windows y arquitectura;
- almacenamiento y memoria;
- red y alcance de dominio;
- version del agente;
- prerrequisitos de una accion.

Gate: no explora archivos personales ni inventario corporativo general.

## Bloque 6: primer adaptador en VM

Elegir un paquete libre, aprobado, silencioso y de bajo riesgo. Implementar:

- `Detect`;
- `Preflight`;
- `Install`;
- `Verify`;
- `Uninstall`;
- timeout, codigos de salida y retry declarado.

Primero se valida en VM Windows 11 con snapshot. No usar la PC principal como
primer endpoint privilegiado.

Para el perfil `local-demo`, el artefacto puede servirse desde un mirror local
simulado si la licencia permite redistribucion y el manifiesto fija origen,
version, arquitectura y SHA-256. Esto no crea un repositorio productivo.

Gate: instalacion, repeticion idempotente, desinstalacion y fallo controlado.

Estado: `completed` el 2026-06-13. La matriz paso en una VM Windows 11 x64
desechable con checkpoint: instalacion y desinstalacion reales, repeticion
idempotente, verificacion, mirror ausente, hash corrupto y restauracion al estado
inicial. El MSI no se ejecuto en el host.

## Bloque 7: API compartida y persistencia

Crear monolito modular Next.js sin portal administrativo completo:

- contratos HTTP;
- identidad local de desarrollo;
- dispositivos;
- catalogo;
- solicitudes y trabajos;
- auditoria y outbox;
- PostgreSQL/Prisma;
- worker durable.

Gate: recorrido WinUI -> API -> agente simulado -> evidencia funciona localmente.

Estado: `completed` el 2026-06-13. El recorrido local usa PostgreSQL real
efimero, worker Node separado, cliente HTTP de WinUI y comunicaciones salientes
del DeviceAgent para ejecutar solo la accion sintetica allowlisted. No conecta
servicios externos ni ejecuta instalaciones reales.

## Bloque 8: casos, tickets y OpenText fake

Implementar `BotCase`, escalamiento, SLA de 72 horas e `ITicketingProvider` fake.

Gate: consultas no crean caso operativo; confirmaciones crean una sola solicitud;
fallos generan escalamiento detallado sin secretos.

Estado: `completed` el 2026-06-14. Cada solicitud confirmada crea un `BotCase`;
los fallos publican un evento de escalamiento tipado en la misma transaccion y
el worker usa `ITicketingProvider` fake para persistir exactamente un
`ExternalTicket` sintetico. La consulta HTTP devuelve el ticket cuando existe,
los reintentos son idempotentes y la politica pura de 72 horas permanece sin
scheduler. No existe conexion OpenText real ni se adelantaron Teams o portal.

## Bloque 9: canal Teams existente

Definir y probar el adaptador para que el bot corporativo existente consuma la
API compartida:

- consultar catalogo;
- proponer alternativas;
- confirmar solicitud;
- consultar estado;
- escalar.

Gate: Teams y WinUI producen las mismas decisiones para la misma entrada.

Estado: `blocked`. El primer incremento local ya define contratos
versionados, `IConversationChannel`, un adaptador recorded sin red, consumo de
las capacidades HTTP existentes y pruebas de paridad Teams/WinUI. La
integracion real requiere conocer owner, plataforma, repositorio,
autenticacion, permisos y ambiente del bot existente. El gobierno y la evidencia
viven en `modules/TEAMS.md` y
`docs/modules/teams-channel-local-increment.md`.

## Bloque 10: endurecimiento para piloto

Cerrar gates externos necesarios para dos endpoints:

- UEMS;
- cuenta restringida;
- Security/Sophos;
- identidad;
- kill switch;
- logs y retiro;
- paquete y confianza de publicador.

Gate: threat model revisado y despliegue/retiro ensayados.

Estado: `in_progress`. Es el unico bloque principal activo. El alcance,
controles locales permitidos, evidencia y gates externos viven en
`modules/PILOT_HARDENING.md`; el threat model de trabajo vive en
`docs/threat-model/README.md`. El bloque no se declara `completed` hasta revisar
el threat model y ensayar despliegue y retiro en dos endpoints autorizados.

## Bloque 11: portal administrativo web

Construir el portal sobre contratos ya usados por WinUI y Teams:

1. shell y login;
2. usuarios, roles y scopes;
3. catalogo;
4. casos, trabajos y tickets;
5. aprobaciones;
6. auditoria y reportes;
7. enlaces OpenText/Rescue.

Gate: autorización server-side, Playwright por rol, migraciones y auditoria.

## Regla de escalamiento

No adelantar un bloque completo para simular progreso. Se permite crear contratos
o fakes del bloque futuro cuando el bloque actual los necesita, pero no construir
la integracion real antes de cumplir su gate.
