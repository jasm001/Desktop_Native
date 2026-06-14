# Confinamiento del perfil local-demo y fallos de workers

## Estado

Unidad tecnica local final del Bloque 10. Evita promocion accidental de la
configuracion de laboratorio, pero no define un perfil empresarial ni sustituye
UEMS, identidad, firma, Security/Sophos o aprobacion operativa.

## Brecha

Los controles individuales fallan cerrados, pero la configuracion del host no
tenia una validacion central de combinaciones. Un operador local podia escribir
un perfil desconocido o intentar habilitar `local-demo`, ejecucion o
sincronizacion fuera del ambiente de desarrollo.

## Alcance tecnico

Una politica pura valida al iniciar el DeviceAgent:

- solo se aceptan los perfiles `disabled` y `local-demo`;
- `local-demo` solo se acepta con ambiente .NET `Development`;
- `JobExecutionEnabled=true` solo se acepta con `local-demo`;
- `ControlPlaneSyncEnabled=true` solo se acepta con `local-demo`;
- cualquier combinacion desconocida o contradictoria termina antes de construir
  el host, con codigo de salida y mensaje fijos;
- `ValidateOnStart` conserva la misma politica como defensa adicional.

Los workers IPC, de trabajos y de sincronizacion publican fallos mediante
eventos fijos 1001, 2001 y 3001. No entregan el objeto `Exception`, payloads,
rutas ni respuestas al proveedor de logging.

El valor predeterminado sigue siendo `disabled`, con ejecucion y sincronizacion
apagadas. La politica no agrega acciones, argumentos, rutas de entrada,
identidades, endpoints ni privilegios.

## Alternativas

- Crear ahora perfiles `pilot` o `production`: descartado porque sus
  identidades, paquete, firma, UEMS y owners no estan definidos.
- Corregir combinaciones invalidas silenciosamente: descartado porque ocultaria
  una configuracion insegura.
- Validar solo dentro de cada worker: descartado porque permitiria que el
  proceso iniciara parcialmente con una configuracion contradictoria.

## Evidencia y aceptacion

1. La configuracion predeterminada inicia deshabilitada.
2. Un perfil desconocido falla cerrado.
3. `local-demo` fuera de `Development` falla cerrado.
4. Ejecucion o sincronizacion habilitadas fuera de `local-demo` fallan cerrado.
5. `local-demo` en `Development` conserva el MVP local existente.
6. No cambia ningun contrato IPC o HTTP ni la allowlist.
7. El rechazo de configuracion no emite valores ni stack trace de aplicacion.
8. Los fallos de workers usan IDs y mensajes fijos sin detalles de excepcion.
9. Un perfil futuro requiere una unidad separada despues de decidir el modelo
   empresarial o propio y sus controles.

## Punto de sustitucion

La politica se evalua antes de construir el host y vuelve a registrarse con
`ValidateOnStart`. El helper de logging vive en el core compartido por sus
workers. Una futura configuracion empresarial debe agregar un perfil nuevo y
explicito con sus propias validaciones; no puede reutilizar ni renombrar
`local-demo`.

## Riesgo residual

La politica valida coherencia local, no autenticidad. La proteccion, firma,
distribucion, ownership y auditoria de la configuracion siguen siendo gates
externos.

## Validacion

- `dotnet format`: correcto;
- build Release: correcto, 0 warnings y 0 errores;
- .NET: 136 pruebas, incluidas siete nuevas acumuladas en esta unidad;
- smoke de configuracion no soportada: mensaje fijo y codigo de salida 78;
- Node: 20 pruebas unitarias/de contrato;
- PostgreSQL: 11 integraciones AdminWeb, 4 Worker y cuatro migraciones;
- E2E local: correcto con tres evidencias saneadas;
- auditoria de dependencias: sin vulnerabilidades conocidas;
- escaneo de secretos: sin hallazgos.
