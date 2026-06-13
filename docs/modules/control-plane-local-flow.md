# Recorrido local del control plane

## Estado

Incremento de cierre del Bloque 7 completado y validado el 2026-06-13.

## Objetivo

Completar el gate local:

```text
WinUI -> API -> worker/outbox -> agente simulado -> evidencia -> API -> WinUI
```

El recorrido usa Next.js y PostgreSQL reales. El agente inicia todas sus
comunicaciones HTTP de forma saliente y solo ejecuta la accion sintetica
allowlisted `software.install.simulated.v1 / secure-transfer / 6.5`.

## Alcance

- cliente HTTP de desarrollo consumible por WinUI;
- confirmacion explicita que persiste una solicitud mediante la API v1;
- worker separado que publica el trabajo desde el outbox sin ejecutarlo;
- endpoints v1 tipados para claim y resultado del agente;
- identidad sintetica de agente, exacta y exclusiva de desarrollo;
- lease, retry acotado e idempotencia de claim/resultado;
- evidencia saneada, ordenada y asociada al trabajo;
- actualizacion transaccional de trabajo, solicitud, auditoria y outbox al
  recibir el resultado;
- adaptador HTTP del DeviceAgent que reclama, ejecuta la simulacion existente y
  reporta el resultado;
- harness automatizado que inicia la API y worker, usa el cliente de WinUI,
  ejecuta el agente simulado y verifica el estado/evidencia final.

## Fronteras

- Next.js no abre Named Pipes ni se conecta a procesos privilegiados.
- WinUI no llama al DeviceAgent para ejecutar acciones.
- El agente solo recibe identificadores tipados; no recibe comandos, rutas,
  scripts, switches ni argumentos.
- La identidad de desarrollo falla cerrada fuera del perfil exacto local.
- Consultar catalogo o estado no crea solicitud, trabajo, auditoria ni outbox.

## Fuera de alcance

- instalacion o desinstalacion real;
- promocion de `local-demo` a piloto o produccion;
- Windows Service instalado;
- portal administrativo, OpenText, tickets, Teams, UEMS, Hermes o RAG;
- Entra, MFA, RBAC productivo, secretos o endpoints corporativos;
- telemetria productiva, retencion corporativa o despliegue cloud.

## Criterio de cierre

- el recorrido completo funciona contra PostgreSQL efimero real: cumplido;
- repetir solicitud o resultado no duplica efectos y los leases se agotan tras
  tres claims: cumplido;
- evidencia, auditoria y outbox permanecen correlacionados: cumplido;
- los contratos rechazan contenido ejecutable y errores no saneados: cumplido;
- `scripts/Validate.ps1` pasa completo: cumplido;
- Bloque 6 permanece `completed` y Bloque 7 pasa a `completed`: cumplido tras
  cerrar el gate final.

## Evidencia

- migraciones Prisma aplicadas con `migrate deploy` sobre PostgreSQL 18 efimero;
- 10 pruebas de integracion de AdminWeb y 3 del worker;
- build standalone de Next.js y bundle ESM ejecutable del worker;
- cliente HTTP real de WinUI crea y consulta la solicitud;
- DeviceAgent reclama por HTTP saliente, reutiliza su simulacion SQLite y
  reporta tres codigos de evidencia allowlisted;
- el estado final contiene una solicitud, un trabajo completado, tres
  evidencias, dos outbox completados y dos efectos sinteticos.
