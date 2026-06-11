# Desarrollo asistido por IA

Fecha de evaluacion: 2026-06-07.

Este documento cubre herramientas para construir el producto. No define aun el
modelo que atendera a usuarios finales. Desarrollo asistido, IA del producto y
licencias de IDE son presupuestos y aprobaciones separados.

## Principio operativo

No mantener seis agentes conversando permanentemente. Cada intercambio entre
agentes multiplica contexto, costo y posibilidades de desviacion.

El flujo inicial usa tres roles activos:

1. `builder`: implementa una tarea pequena y ejecuta validaciones;
2. `reviewer`: revisa diff, arquitectura, seguridad y criterios de terminado;
3. `researcher`: consulta fuentes oficiales, prepara contexto y documenta.

Vision/UI y QA son modos de trabajo invocados bajo demanda, no procesos
residentes. El usuario conserva aprobacion de alcance, cambios privilegiados,
dependencias, migraciones y releases.

## Asignacion inicial de modelos

| Funcion | Modelo inicial | Escalamiento | Motivo |
| --- | --- | --- | --- |
| Configurar Hermes y tareas rutinarias | Step 3.7 Flash durante promocion; despues DeepSeek V4 Flash | DeepSeek V4 Pro | Rapido y economico |
| Programador C#/WinUI y backend complejo | DeepSeek V4 Pro | Codex/revision humana aprobada | Mejor perfil de coding y trabajo agentico del grupo evaluado |
| Documentacion, busqueda y resumen | DeepSeek V4 Flash | Step 3.7 Flash o V4 Pro | Evitar gastar el modelo principal en trabajo mecanico |
| Reviewer/director | DeepSeek V4 Pro con contexto reducido al diff | Segundo reviewer independiente | Razonamiento fuerte sin reenviar el repositorio completo |
| Capturas, UI y referencia visual | MiniMax M3 | Kimi K2.6 para casos dificiles | M3 es multimodal y mas barato; Kimi se reserva para visual coding |
| QA web | Playwright + DeepSeek V4 Flash | V4 Pro para diagnostico | Las aserciones deterministas validan; el LLM interpreta fallas |
| QA WinUI | Windows App Driver/UI Automation y pruebas .NET en Windows | Revision visual multimodal | Ubuntu no valida una aplicacion Windows nativa |

Qwen3.7 Plus es una alternativa multimodal valida, pero no aporta una ventaja
clara de costo frente a MiniMax M3 para el primer piloto. Kimi K2.6 es capaz,
pero su salida y cache son mas costosos; no debe ser el modelo por defecto.

La asignacion se confirma con un benchmark propio de 15 a 25 tareas:

- crear y corregir un modulo C#;
- respetar MVVM, DI, nullable y async;
- modificar XAML sin romper accesibilidad;
- crear un flujo Next.js con autorizacion server-side;
- encontrar una regresion real;
- ejecutar build/tests y corregir hasta pasar;
- resumir el cambio sin inventar resultados.

Los benchmarks publicos orientan, pero no demuestran calidad especifica en
WinUI 3 ni en este repositorio.

## Datos y proveedores

Elegir modelo no basta. Debe fijarse tambien el proveedor que lo sirve.

- habilitar `data_collection: deny` y exigir proveedor compatible;
- preferir Zero Data Retention cuando exista;
- no permitir fallback a un proveedor con entrenamiento/logging incompatible;
- no enviar tickets, correos, hostnames, secretos ni datos productivos;
- usar claves distintas por rol y ambiente;
- registrar modelo, proveedor, costo y tarea, no prompts sensibles;
- revocar la clave cuando termine el piloto.

Al 2026-06-07, OpenRouter muestra Zero Retention para proveedores concretos de
Kimi K2.6. MiniMax M3 y Step 3.7 Flash indican que no entrenan con prompts, pero
la retencion aparece como desconocida. DeepSeek V4 Flash puede mostrar
entrenamiento de prompts segun proveedor. Esto debe validarse y fijarse en la
configuracion, no asumirse por el nombre del modelo.

Una cuenta, suscripcion o tarjeta personal no debe recibir codigo corporativo
sin autorizacion expresa. Puede usarse para prototipos sinteticos o publicos,
pero no como atajo a la revision de Security/AI Governance.

## Entorno recomendado

### Opcion principal: WSL2 mas Windows host

- Hermes vive en WSL2, ruta `~/.hermes`.
- RTK y `rtk-hermes` se instalan dentro de la misma distribucion WSL2 y del
  mismo virtualenv de Hermes. Su comportamiento es equivalente al de Ubuntu.
- El repositorio nativo permanece en una ruta Windows accesible al IDE.
- Hermes puede editar y orquestar, pero build, MSBuild, Windows App SDK, MSIX y
  pruebas WinUI se ejecutan en Windows.
- El portal Next.js y herramientas Linux pueden ejecutarse en WSL2.
- Las pruebas privilegiadas se hacen solo en VM/sandbox o endpoints designados.

Hermes recomienda WSL2 como la ruta Windows mas probada. Su soporte nativo de
Windows esta en beta temprana. WSL2 mejora compatibilidad, pero no es una
frontera de seguridad suficiente para ejecutar codigo no confiable.

### VM Ubuntu

Es adecuada para Hermes, documentacion, investigacion, backend y portal web. No
puede ser el unico entorno del proyecto porque no compila ni prueba de forma
representativa WinUI 3/MSIX. Si se usa, trabaja mediante ramas y pull requests;
un runner Windows o el host ejecuta las validaciones nativas.

### Aislamiento Windows

Para instaladores, servicios, registro, `Program Files`, elevacion y rollback,
usar una VM Windows 11 con snapshots/checkpoints. La PC fisica no es el primer
entorno de ejecucion de acciones privilegiadas generadas durante desarrollo.

## Flujo de trabajo

1. El researcher prepara fuentes oficiales y criterios verificables.
2. El builder implementa una sola issue con permisos limitados.
3. CI local ejecuta formato, build, pruebas, SAST y secret scan.
4. El reviewer recibe diff, resultados y documentos del modulo, no toda la
   conversacion.
5. Una persona aprueba cambios de arquitectura, seguridad o privilegios.
6. Las pruebas WinUI se ejecutan en Windows.
7. Un stopper se registra con evidencia, intentos y decision requerida.

El configurador instala y mantiene herramientas; no dirige diariamente al
programador. El reviewer supervisa calidad. El researcher reporta bloqueos al
reviewer y al usuario mediante un formato estructurado.

## Controles Hermes

- allowlist de herramientas y comandos;
- sin secretos productivos en memoria, vault o historial;
- ramas/worktrees separados por tarea;
- concurrencia inicial maxima de dos subagentes;
- maximo de iteraciones y timeout por tarea;
- presupuesto por clave y por mes;
- commits pequenos y reversibles;
- prohibido aprobar su propio cambio;
- detenerse ante migraciones destructivas, permisos, firma o datos reales.

RTK puede reducir salida repetitiva de comandos, pero el ahorro observado debe
medirse. No reemplaza limites de contexto, resumen de sesiones ni seleccion
correcta de archivos.

### RTK en WSL2

RTK no se instala en Windows para este perfil. Se instala dentro de WSL2:

```bash
curl -fsSL https://raw.githubusercontent.com/rtk-ai/rtk/refs/heads/master/install.sh | sh

HERMES_PY="$HOME/.hermes/hermes-agent/venv/bin/python"
"$HERMES_PY" -m pip install --upgrade rtk-hermes
```

Habilitar el plugin directamente en `~/.hermes/config.yaml`:

```yaml
plugins:
  enabled:
    - rtk-rewrite
```

Configuracion inicial conservadora:

```bash
export RTK_HERMES_MODE=rewrite
export RTK_HERMES_TIMEOUT_MS=2000
export RTK_HERMES_PREVIEW_MARKER=true
export RTK_HERMES_BACKENDS=local
```

Estas variables deben persistirse en el perfil que realmente inicia Hermes.
Para SSH, Docker u otro backend, no habilitarlo hasta instalar RTK tambien en
ese entorno.

Verificacion:

```bash
rtk --version
rtk rewrite "git status"
"$HERMES_PY" -m pip show rtk-hermes
```

Despues de reiniciar Hermes, usar `/rtk status` y `/rtk stats` cuando la version
instalada soporte esos comandos.

RTK reduce el texto que las herramientas devuelven al modelo y evita agregar un
schema MCP variable, lo cual ayuda a mantener estable el contexto. El porcentaje
de prompt-cache hit depende ademas del proveedor, prefijo estable, modelo,
orden de herramientas y reutilizacion exacta del contexto. Por tanto:

- mantener system prompt, herramientas e instrucciones base estables;
- agregar contexto cambiante al final;
- no alternar modelo/proveedor dentro de una misma sesion;
- resumir o abrir sesion nueva cuando el historial deje de ser reutilizable;
- medir tokens evitados por RTK y cache hit como metricas diferentes.

## Criterio de salida del piloto

Despues de dos semanas registrar por rol:

- tareas completadas y aceptadas;
- costo por tarea aceptada;
- tokens de entrada, salida y cache;
- porcentaje de builds/tests exitosos;
- regresiones encontradas por reviewer;
- intervenciones humanas;
- tiempo ahorrado frente al flujo manual.

Con esos datos se conserva, cambia o elimina cada modelo. No se firma un
compromiso de seis meses antes de esta medicion.
