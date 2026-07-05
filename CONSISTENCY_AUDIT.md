# Auditoria de consistencia documental

Fecha: 2026-07-05.

## Cobertura

Se contrastaron:

- `README.md`, `CURRENT_CONTEXT.md`, `WORKFLOW.md`, `DEVELOPMENT_PLAN.md` y
  `MASTER_PROMPT.md`;
- documentos normativos de `core/` y `standards/`;
- documentos propietarios de `modules/`;
- evidencia tecnica en `docs/modules/` y `docs/threat-model/`;
- contexto historico y solicitudes de informacion sobre Teams y piloto;
- estado Git y commits publicados.

## Precedencia

La precedencia vigente permanece definida en `README.md`:

1. seguridad y decisiones cerradas;
2. alcance, stack y arquitectura;
3. estandares;
4. documento propietario del modulo;
5. plan de desarrollo;
6. contexto y workflow actuales;
7. referencia e historia.

`MASTER_PROMPT.md` es un mecanismo de handoff, no una fuente normativa.

## Estado verificado

- Bloques 0 a 8: `completed`.
- Incremento local del Bloque 9 publicado en `0448a42`.
- Cierre local del Bloque 10 publicado en `be6c4fc`.
- Segunda unidad local del Bloque 11 publicada en `17e7581`.
- Tercera unidad local del Bloque 11 publicada en `7a65f3e`: Testing
  Library/jsdom y Playwright sobre la superficie administrativa actual.
- Cuarta unidad local del Bloque 11 publicada en `212f274`: cierre del
  esqueleto `/admin/*` con rutas protegidas sinteticas.
- Unidad local del asistente WinUI publicada en `cfa3342`: chat visual de
  Hermes local en memoria, indicador de respuesta, autoscroll y envio con
  Enter, sin persistencia ni acciones.
- Ajuste local de Hermes publicado en `eb99434`: timeout cliente de 120
  segundos.
- Bloques 9 y 10: `blocked` por evidencia externa.
- Bloque 11: unico bloque principal `in_progress`.
- Documento propietario del Bloque 9: `modules/TEAMS.md`.
- Documento propietario del Bloque 10: `modules/PILOT_HARDENING.md`.
- Documento propietario del Bloque 11: `modules/ADMIN_PORTAL.md`.
- Gate completo vigente: 140 pruebas .NET, 40 pruebas Node unitarias/de
  contrato/componente, 12 integraciones AdminWeb, 4 del Worker, cuatro
  migraciones PostgreSQL, E2E local, auditoria de dependencias y escaneo de
  secretos correctos.
- Cuatro unidades locales del Bloque 11 publicadas sin cambiar contratos
  publicos, migraciones, mutaciones o integraciones corporativas.
- `docs/modules/local-mvp-lab.md` define `lab-real-sanitized` como categoria de
  datos reales de laboratorio persistidos y saneados. La categoria permite
  mejorar visualizacion local con evidencia generada por el propio laboratorio,
  pero no admite datos corporativos ni cierra Bloques 9, 10 u 11.
- `core/DECISIONS.md` agrega D-073 y `core/SCOPE.md` distingue datos reales de
  laboratorio saneados de datos reales corporativos o productivos.
- `docs/modules/local-lab-real-data-roadmap.md` registra cinco unidades
  independientes para estado de laboratorio, lecturas operativas reales locales,
  health de conectores simulados, catalogo local curado y recorrido end-to-end
  visual.
- Unidad local intermedia: WinUI puede usar Hermes local compatible con OpenAI
  para texto libre informativo mediante variables de entorno, deshabilitado por
  defecto, limitado a loopback, con historial visual en memoria y sin archivos
  `.env` del repositorio como fuente de secretos. El timeout cliente local queda
  fijado en 120 segundos. No crea solicitudes, comandos, tickets, auditoria,
  outbox ni llamadas al DeviceAgent.
- `SQLitePCLRaw.bundle_e_sqlite3` queda fijado a `3.0.3` para evitar la
  vulnerabilidad alta reportada por NuGet Audit en la transitiva
  `SQLitePCLRaw.lib.e_sqlite3` 2.1.11.

## Alineacion del Bloque 9

Hechos confirmados:

- existe un bot corporativo de Teams;
- se amplia ese bot; no se crea otro;
- Teams y WinUI comparten backend y decisiones;
- Teams no ejecuta comandos ni llama al DeviceAgent.

Pendientes externos:

- owner y plataforma del bot;
- repositorio y proceso de despliegue;
- autenticacion, permisos, tenant y ambientes;
- mecanismo de acciones o tarjetas;
- capacidad real de consumir la API compartida.

La falta de esos datos no bloquea contratos, un adaptador local fake/recorded ni
pruebas de paridad, que ya fueron implementados. El stopper para conectar el bot
real sin esa evidencia esta registrado en `WORKFLOW.md` y mantiene el bloque
`blocked`. D-072 permite continuar el Bloque 11 con identidad, datos y
proveedores locales sin declarar cerrada la integracion Teams.

## Alineacion del Bloque 10

- El bloque tiene un documento propietario y queda `blocked`.
- El threat model dejo de ser un placeholder y enumera activos, fronteras,
  amenazas, controles actuales y evidencia pendiente.
- La primera unidad local implementa un kill switch del DeviceAgent apagado por
  defecto, pruebas de admision/claim/pendientes y un runbook de
  deshabilitacion, rollback y retiro.
- La unidad local final confina `local-demo` a `Development`, rechaza
  configuraciones contradictorias antes de construir el host y evita adjuntar
  excepciones o payloads a los eventos genericos de fallo de workers.
- El alcance local no autoriza UEMS, Entra, Sophos, PKI, Teams, OpenText ni
  endpoints corporativos.
- El gate externo sigue exigiendo revision del threat model y ensayo de
  despliegue/retiro en dos endpoints autorizados.
- El Bloque 11 activo no se usa como evidencia para cerrar hardening.

## Alineacion del Bloque 11

- `modules/ADMIN_PORTAL.md` es el documento propietario y define el gate.
- `src/AdminWeb` conserva el control plane modular, cuatro migraciones y la
  superficie tecnica `/` del Bloque 8.
- `/admin`, `/admin/catalog`, `/admin/operations`, `/admin/audit`,
  `/admin/access`, `/admin/approvals`, `/admin/support` y `/admin/reporting`
  implementan identidad sintetica separada, ocho capabilities fail-closed,
  navegacion accesible, lecturas limitadas de solo lectura y estados sinteticos
  en memoria.
- Las consultas administrativas toman como maximo 25 registros, usan
  selecciones explicitas y omiten payloads de auditoria.
- Testing Library/jsdom y Playwright estan instalados e implementados solo para
  calidad local de la superficie de solo lectura y el esqueleto protegido.
- No estan instalados ni implementados Fluent UI, OIDC/Entra, sesiones, RBAC
  productivo o mutaciones administrativas.
- Los roles de produccion son un modelo objetivo; no representan owners,
  permisos ni asignaciones corporativas confirmadas.
- El portal no llama directamente al DeviceAgent ni ejecuta comandos.
- `docs/modules/admin-portal-foundation.md` registra contratos, alternativas,
  evidencia, sustitucion futura por OIDC/Entra y riesgos residuales.
- `docs/modules/admin-portal-read-model.md` registra las rutas, proyecciones,
  limites, pruebas de no mutacion y QA adaptable de la segunda unidad.
- `docs/modules/admin-portal-local-quality-gate.md` registra la tercera unidad,
  estrategia de Testing Library/Playwright, diferimiento de Fluent UI y riesgos
  residuales.
- `docs/modules/admin-portal-local-skeleton-closure.md` registra la cuarta
  unidad local que cierra el esqueleto protegido sin cerrar el Bloque 11
  completo.
- `modules/ADMIN_PORTAL.md` permite una futura superficie de laboratorio de
  solo lectura para `lab-real-sanitized`, protegida en servidor, sin mutaciones,
  sin llamadas directas al DeviceAgent y sin administrar VM, servicios o
  bridges. Una VM apagada se trata como `not_checked`, `offline` o
  `unavailable`, no como fallo productivo.

## Correcciones realizadas

- `README.md`, `CURRENT_CONTEXT.md`, `WORKFLOW.md` y `DEVELOPMENT_PLAN.md`
  registran a los Bloques 9 y 10 `blocked` y al Bloque 11 como unico
  `in_progress`;
- `WORKFLOW.md` registra `eb99434` como ultimo resultado publicado y conserva
  stoppers separados para Teams y hardening;
- `modules/PILOT_HARDENING.md` define alcance, limites y gate del Bloque 10;
- `docs/threat-model/README.md` contiene el inventario inicial verificable;
- `docs/modules/pilot-hardening-local-kill-switch.md` y
  `docs/runbooks/device-agent-disable-rollback-retirement.md` conservan la
  primera mitigacion local y su operacion;
- `docs/modules/pilot-hardening-local-profile-confinement.md` y
  `docs/modules/pilot-hardening-local-closure.md` conservan la unidad final, el
  inventario y los puntos de sustitucion;
- `modules/TEAMS.md` conserva el incremento local y el gate corporativo;
- `modules/ADMIN_PORTAL.md`, `modules/WEB_DELIVERY.md`, `src/AdminWeb/README.md`
  y `tests/AdminWeb/README.md` distinguen las lecturas locales ya implementadas
  del portal productivo todavia pendiente;
- `docs/modules/local-lab-real-data-roadmap.md` agrega la ruta documental para
  sustituir muestras por datos reales de laboratorio saneados antes de
  implementar nuevas unidades;
- `docs/modules/repository-foundation.md`,
  `docs/modules/control-plane-foundation.md` y
  `docs/modules/local-mvp-lab.md` conservan su evidencia historica sin afirmar
  que el portal actual sigue ausente;
- las evidencias de bloques cerrados que contenian referencias al bloque activo
  fueron convertidas en contexto historico;
- `MASTER_PROMPT.md` queda como handoff para continuar desde la linea base
  publicada en `eb99434`.

## Diferencias intencionales

- `context/` y `reference/` conservan respuestas e historia y no representan el
  estado de ejecucion actual.
- Documentos de bloques cerrados pueden mencionar cual era su siguiente gate,
  o conteos de pruebas de ese momento, siempre que lo identifiquen como
  contexto historico.
- El perfil local puede usar fakes; eso no declara validada una integracion
  corporativa.
- El perfil local tambien puede usar `lab-real-sanitized`; eso mejora la
  demostracion pero no autoriza datos productivos, pilotos ni integraciones
  corporativas.
- `blocked` en el Bloque 9 significa que su siguiente avance depende de
  evidencia externa; no revierte el incremento local publicado.
- `blocked` en el Bloque 10 significa que el trabajo local reproducible termino,
  pero no existe revision Security ni ensayo en dos endpoints; no equivale a
  `completed`.
- `in_progress` en el Bloque 11 significa que puede continuar con unidades
  locales reemplazables; no implica Entra, RBAC productivo ni portal terminado.

## Resultado

No queda una contradiccion material conocida sobre bloque activo, commits
publicados, conteos vigentes, estado de Teams, alcance de hardening, ticketing
fake o linea base del portal. Los conteos anteriores permanecen solo dentro de
evidencias historicas identificables. Los enlaces Markdown relativos vigentes
fueron comprobados sin hallazgos.
