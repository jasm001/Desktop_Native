# Estandares de desarrollo

## Arquitectura y POO

- SOLID aplicado con criterio; evitar interfaces sin frontera real.
- MVVM estricto en WinUI: Views sin logica de negocio.
- CommunityToolkit.Mvvm para `ObservableProperty`, `RelayCommand` y mensajeria.
- Inyeccion de dependencias con `Microsoft.Extensions.DependencyInjection`.
- Features/modulos con dependencias dirigidas hacia dominio/aplicacion.
- Un modulo no se concentra en un archivo gigante; separar UI, casos de uso,
  dominio, infraestructura, contratos y pruebas.
- Records para valores y mensajes inmutables; clases para entidades con ciclo.
- Resultados tipados y excepciones solo para fallas excepcionales.

## C# y .NET

- `Nullable` habilitado.
- Warnings como errores en CI.
- Analyzers de .NET y reglas editorconfig.
- `async`/`await` de extremo a extremo para I/O.
- Nunca `.Result`, `.Wait()` ni bloqueo del UI thread.
- `CancellationToken` en operaciones externas/largas.
- `HttpClientFactory`, timeouts, retry selectivo e idempotencia.
- Pattern matching y switch expressions cuando mejoren claridad.
- Secretos y datos sensibles nunca en excepciones/logs.

## WinUI 3

- `{x:Bind}` por defecto; `{Binding}` solo con justificacion.
- Recursos de tema, no colores fijos.
- Mica/Acrylic con fallback y sin sacrificar contraste.
- `VisualStateManager` y `AdaptiveTrigger` para layout adaptable.
- Carga diferida con `x:Load`/estrategia equivalente en vistas pesadas.
- Navegacion y estado recuperables tras cierre/reinicio.
- Toasts nativos para progreso y finalizacion.
- Accesibilidad: AutomationProperties, teclado, foco, 200 %, alto contraste.

No asumir que una app WinUI de escritorio recibe suspension como UWP. Persistir
estado ante cambios y cierre, y diseñar trabajos largos en el Windows Service,
no en tareas de fondo dependientes del proceso UI.

## Portal web

- TypeScript estricto.
- Next.js App Router con fronteras claras entre Server y Client Components.
- Server Components por defecto; `"use client"` solo cuando se necesita estado,
  eventos o APIs de navegador.
- Server Actions para mutaciones internas; Route Handlers para agente, webhooks,
  integraciones, streaming y APIs externas.
- Componentes Fluent accesibles.
- Prisma solo en infraestructura server-side; nunca importado en cliente.
- Inicializar Prisma, Redis y SDK externos mediante getters/singletons lazy para
  que `next build` no dependa de secretos de runtime.
- TanStack Query solo para interaccion cliente que requiera cache/mutaciones
  frecuentes; no duplicar datos ya resueltos por Server Components.
- OpenAPI para contratos con agente e integraciones.
- Validacion en cliente por UX y siempre nuevamente en backend.
- RBAC nunca implementado solo ocultando botones.
- `proxy.ts` puede redirigir trafico, pero cada Server Action, Route Handler y
  caso de uso vuelve a autenticar y autorizar. Nunca es la unica barrera.
- Mantener Next.js/React en versiones estables soportadas y aplicar parches de
  seguridad antes de promover un release.
- Pruebas Playwright para roles, aprobaciones y auditoria.

## Agente y seguridad

- Sin shell generico.
- Cada accion implementa contrato tipado y pruebas.
- Procesos hijos con argumentos separados, no command strings concatenados.
- Validar hash, firma/editor, fuente, arquitectura y version.
- Limitar tiempo, recursos y salida; matar arbol de procesos cuando corresponda.
- Logs estructurados con correlacion, sin secretos.
- Toda operacion debe soportar retry seguro o declarar que no es reintentable.

## Datos y backend

- Migraciones versionadas y probadas.
- PostgreSQL real efimero en integration tests.
- Transacciones en limites consistentes.
- Outbox para OpenText, notificaciones y trabajos.
- Auditoria append-only.
- Paginacion, filtros y limites en consultas administrativas.
- No exponer entidades de persistencia como contratos HTTP.

## Correcciones y deuda

- No aplicar parches locales que contradigan el modelo o dupliquen reglas.
- Antes de corregir, comparar comportamiento esperado, contratos, datos y
  consumidores afectados.
- Corregir la causa en el modulo propietario y actualizar dependencias, pruebas,
  migraciones y documentacion relacionadas.
- Un hotfix urgente puede ser minimo, pero debe incluir prueba de regresion y una
  tarea fechada para su integracion estructural.
- No dejar flags permanentes, condicionales por usuario, valores magicos ni
  excepciones silenciosas como solucion final.
- Refactorizar cuando la correccion revela una frontera equivocada; evitar
  reescrituras no relacionadas.

## Documentacion modular

- El archivo operativo `Context` mantiene estado y decisiones vigentes del
  repositorio final.
- El archivo operativo `workflow` mantiene completado, en curso, siguiente y
  validaciones del repositorio.
- Durante el arranque, `../CURRENT_CONTEXT.md` y `../WORKFLOW.md` cumplen esa
  funcion y deben migrarse al repositorio definitivo.
- `docs/architecture/`: ADR y diagramas por modulo.
- `docs/modules/<module>.md`: responsabilidad, contratos, datos y pruebas.
- `docs/runbooks/`: despliegue, rollback, incidentes e integraciones.
- `docs/security/`: threat model, permisos y hallazgos.
- Cada cambio funcional actualiza el documento del modulo; no crear un unico MD
  acumulativo imposible de mantener.
- Los documentos deben permitir que otra persona o agente de IA recupere contexto
  puntual sin leer todo el repositorio.

## Seguridad automatizada

- Semgrep CE como SAST inicial sin licencia adicional.
- CodeQL en CI si GitHub Code Security esta disponible; cubre C# y
  JavaScript/TypeScript.
- CodeQL para VS Code es complemento local para consultas sobre bases CodeQL, no
  sustituye el escaneo continuo del repositorio privado.
- Dependabot o Renovate para actualizaciones; `npm audit`/NuGet audit como senal,
  no como unica defensa.
- Gitleaks para secretos.
- Trivy para filesystem, contenedores, IaC y SBOM.
- OWASP ZAP baseline contra el portal desplegado de test.
- Analyzers .NET y ESLint no sustituyen SAST.
- Hallazgos High/Critical bloquean promocion salvo excepcion aprobada y expirable.

## Pull requests y definicion de terminado

- Cambio pequeno, revisable y con ADR cuando altera arquitectura.
- Pruebas proporcionales al riesgo.
- Threat model actualizado para nuevas acciones privilegiadas.
- Telemetria y runbook para funciones operativas.
- Sin warnings, secretos, dependencias vulnerables conocidas ni TODO criticos.
- Evidencia de accesibilidad y rollback antes de promover al piloto.
