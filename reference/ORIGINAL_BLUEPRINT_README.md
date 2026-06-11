# IT Support Native

Blueprint para construir desde cero una aplicacion empresarial real de soporte IT
para Windows 11. El repositorio React/Vite ubicado en el directorio padre queda
congelado como referencia visual y de comportamiento; no sera la base tecnica
del producto.

## Decision de aislamiento

Esta carpeta esta ignorada por el `.gitignore` del mockup. Se usa solamente para
cerrar decisiones antes de crear el repositorio definitivo.

Cuando comience la implementacion, crear un repositorio independiente, por
ejemplo:

```text
C:\Users\Ruruu\Documents\IT Support Native
```

No copiar `node_modules`, configuracion Vite, componentes React, endpoints de
demo, datos mock ni historial Git. Solo migrar assets aprobados y especificaciones
visuales que tengan derechos de uso confirmados.

## Producto definido

Una aplicacion nativa Windows 11 para:

- autoservicio de software autorizado;
- diagnostico controlado del equipo;
- consulta y seguimiento de tickets;
- escalamiento a soporte humano;
- inventario y evidencia de cumplimiento;
- administracion y auditoria empresarial.

La interfaz conserva la identidad Fluent del mockup, pero no simula el escritorio
ni la barra de tareas de Windows dentro de otra ventana. Usara ventana, barra de
titulo, navegacion, menus, dialogos, notificaciones y bandeja del sistema nativos.

El producto tiene dos superficies y un agente:

- cliente nativo WinUI 3 para usuarios finales;
- portal web para administracion, soporte, seguridad, compras y auditoria;
- agente Windows sin interfaz administrativa para inventario y acciones locales.

## Documentos

Para no cargar todo el proyecto en cada sesion:

- `context/DEVELOPMENT_CONTEXT.md`: mapa para programacion y revision.
- `context/STARTUP_AND_PENDING_CONTEXT.md`: mapa para permisos y pendientes.
- `executive/`: explicacion simplificada y solicitud para liderazgo.

- `STACK.md`: tecnologias elegidas y descartadas.
- `ARCHITECTURE.md`: componentes, fronteras y flujo de datos.
- `SCOPE.md`: alcance de MVP, fases posteriores y no objetivos.
- `SECURITY.md`: modelo de confianza y ejecucion privilegiada.
- `DELIVERY.md`: estructura del repositorio, calidad y despliegue.
- `DECISIONS.md`: decisiones cerradas y preguntas que requieren al negocio.
- `ADMIN_PORTAL.md`: portal web, roles y provisionamiento de usuarios.
- `CATALOG.md`: conversion del Excel corporativo en catalogo ejecutable.
- `OPERATIONS.md`: red, despliegue, operacion offline, IA y soporte remoto.
- `CODING_STANDARDS.md`: reglas de implementacion y calidad.
- `PILOT_QUESTIONS.md`: preguntas inmediatas para cerrar con las areas.
- `SOURCES.md`: referencias tecnicas oficiales.
- `TICKETING.md`: tickets internos del bot y escalaciones a OpenText.
- `WEB_DELIVERY.md`: desarrollo y despliegue del portal Next.js.
- `PILOT_ASSESSMENT.md`: evaluacion de las respuestas del piloto.
- `INFORMATION_REQUESTS.md`: informacion, accesos y aprobaciones por area.
- `EXECUTION_ADAPTERS.md`: automatizacion segura de instaladores configurables.
- `AI_DEVELOPMENT.md`: roles, modelos y entorno para desarrollo asistido.
- `LICENSES_AND_BUDGET.md`: licencias, costos y piloto de consumo de IA.

## Regla de inicio

No comenzar integraciones reales ni ejecucion privilegiada hasta tener aprobados:

1. modalidad de identidad aprobada para el piloto;
2. acceso de integracion a OpenText Service Manager;
3. canal de distribucion y confianza del certificado interno;
4. catalogo Excel normalizado y responsables de aprobacion;
5. matriz de acciones permitidas;
6. politica de retencion y datos personales;
7. alcance de la sede y equipos del piloto.
