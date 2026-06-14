# Cierre local del endurecimiento para piloto

## Estado

Evidencia local final del Bloque 10. El trabajo reproducible sin infraestructura
corporativa queda agotado; el gate de piloto no esta cerrado y el bloque pasa a
`blocked` hasta recibir evidencia externa.

Este cierre no cambia el MVP: `local-demo` sigue disponible solo en ambiente
.NET `Development`, con datos sinteticos, proveedores locales o fake y sin
conexiones corporativas.

## Inventario verificable

| Frontera | Control local verificable | Evidencia externa pendiente |
| --- | --- | --- |
| Identidad de agente y dispositivo | Identidad de desarrollo cerrada; claim ligado a dispositivo, sujeto, token hasheado y lease | Bootstrap, unicidad, rotacion y revocacion aprobados |
| IPC privilegiado | Named Pipe `CurrentUserOnly`, framing de 64 KiB, contratos versionados y acciones allowlisted | Cuenta restringida, ACL e identidad cliente/servicio en dos endpoints |
| Replay e idempotencia | Claves unicas, hash de payload, advisory locks, leases y resultados idempotentes | Ensayo con identidad y red del piloto |
| Artefactos | Raiz local restringida, longitud y SHA-256 exactos, producto/version/argumentos fijos | Origen UEMS, paquete aprobado, firma o confianza de publicador |
| Configuracion | Perfil `disabled` por defecto; solo `disabled` y `local-demo`; capacidades habilitadas solo con `local-demo`; `local-demo` solo en `Development` | Perfil empresarial nuevo, distribucion protegida y politica por ambiente |
| Logs y evidencia | Eventos de runtime 1001, 2001 y 3001 usan mensajes fijos sin excepcion ni payload; evidencia funcional usa codigos y resumenes acotados | Inventario operativo, acceso, retencion, alertas y respuesta ante compromiso |
| Perdida de red | SQLite recupera trabajos; `running` vuelve a `queued`; control plane usa leases y retries acotados | Ensayo antes, durante y despues en dos endpoints |
| Deshabilitacion y retiro | Kill switch apagado por defecto bloquea admision, claim e inicio/reanudacion; runbook local | Owner, UEMS, revocacion, restauracion y evidencia de dos endpoints |
| Canales | WinUI y Teams recorded solo consumen aplicacion/control plane tipados | Bot Teams real permanece en su stopper independiente |

## Unidad local final

La configuracion del DeviceAgent se valida al iniciar:

- perfiles desconocidos fallan cerrados;
- `local-demo` fuera de `Development` impide iniciar el host;
- ejecucion o sincronizacion habilitadas con perfil `disabled` impiden iniciar;
- el valor predeterminado conserva ejecucion y sincronizacion apagadas.

El rechazo ocurre antes de construir el host y solo escribe un mensaje fijo con
codigo de salida 78; no expone la configuracion ni un stack trace de aplicacion.
La validacion de opciones al iniciar permanece como defensa adicional.

Los fallos no controlados de los workers IPC, trabajos y sincronizacion ya no
adjuntan objetos `Exception` al logger. Solo publican eventos y mensajes fijos.
Esto evita exponer rutas, respuestas, payloads o detalles internos a traves del
logging generico.

No se agregan comandos, argumentos, rutas de entrada, acciones, privilegios,
datos recolectados, contratos publicos, dependencias, identidades ni endpoints.

## Punto de sustitucion

Un piloto no puede promover ni renombrar `local-demo`. Debe agregar un perfil
empresarial explicito en `DeviceAgentConfigurationPolicy` y aportar, en una
unidad posterior:

1. identidad y cuenta restringida aprobadas;
2. fuente de configuracion protegida;
3. paquete, firma o confianza de publicador;
4. owner y procedimiento UEMS;
5. revision Security/Sophos;
6. retencion y respuesta ante compromiso;
7. evidencia de despliegue, deshabilitacion, rollback y retiro en dos endpoints.

La implementacion concreta puede usar capacidades corporativas o proveedores
propios, siempre que conserve los contratos tipados y las fronteras normativas.

## Criterio de aceptacion local

1. Configuracion desconocida o contradictoria impide iniciar el DeviceAgent.
2. `local-demo` sigue funcionando solo en `Development`.
3. Los eventos de fallo del host tienen IDs y mensajes fijos sin excepciones.
4. El kill switch y el runbook validados se conservan.
5. Los gates completos del repositorio, auditoria y secretos pasan.
6. El Bloque 10 queda `blocked`, no `completed`, porque su gate requiere
   revision externa y dos endpoints autorizados.

Validacion final: build Release sin warnings ni errores, 136 pruebas .NET, 20
pruebas Node unitarias/de contrato, 11 integraciones AdminWeb, 4 del Worker,
cuatro migraciones PostgreSQL, E2E local, auditoria sin vulnerabilidades
conocidas y escaneo de secretos sin hallazgos.

## Riesgos residuales

- La configuracion local valida coherencia, no autenticidad ni integridad.
- El kill switch se aplica al reiniciar y no interrumpe un adaptador ya iniciado.
- No existe cuenta restringida, identidad corporativa, UEMS, firma, PKI ni
  aprobacion Sophos en el repositorio.
- Retencion, acceso a logs, alertas y respuesta ante compromiso no estan
  definidos.
- No se ha ensayado el ciclo operativo en dos endpoints autorizados.
