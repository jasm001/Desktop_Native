# Mapas de contexto

Estos archivos indican que documentos debe leer una persona o agente segun la
tarea. No duplican las especificaciones y no reemplazan los documentos raiz.

- `DEVELOPMENT_CONTEXT.md`: contexto para disenar, programar, probar o revisar.
- `STARTUP_AND_PENDING_CONTEXT.md`: contexto para permisos, presupuesto,
  accesos, decisiones pendientes e inicio del proyecto.
- `IDENTITY_AND_TEAMS_RESPONSES.md`: respuestas internas y validacion de
  identidad, recuperacion y bot de Teams.

## Regla de uso

1. Leer `README.md` del proyecto.
2. Elegir uno de estos dos mapas.
3. Cargar solo los documentos marcados como obligatorios.
4. Abrir documentos adicionales cuando la tarea realmente los toque.
5. Consultar `SOURCES.md` solo para verificar una decision o referencia.

No cargar todos los Markdown en cada sesion. Eso aumenta costo, reduce cache
util y mezcla decisiones tecnicas con solicitudes administrativas.
