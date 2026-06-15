# Auditoria de consistencia documental

Fecha: 2026-06-14.

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
- Bloques 9 y 10: `blocked` por evidencia externa.
- Bloque 11: unico bloque principal `in_progress`.
- Documento propietario del Bloque 9: `modules/TEAMS.md`.
- Documento propietario del Bloque 10: `modules/PILOT_HARDENING.md`.
- Documento propietario del Bloque 11: `modules/ADMIN_PORTAL.md`.
- Gate completo vigente: 136 pruebas .NET, 20 pruebas Node unitarias/de
  contrato, 11 integraciones AdminWeb, 4 del Worker, cuatro migraciones
  PostgreSQL y E2E local.

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
  superficie tecnica del Bloque 8; no existe aun portal administrativo.
- No estan instalados ni implementados Fluent UI, Testing Library, Playwright,
  login de portal, OIDC/Entra, RBAC server-side o rutas administrativas.
- La primera unidad local queda limitada a identidad sintetica fail-closed,
  autorizacion server-side y shell accesible de solo lectura.
- Los roles de produccion son un modelo objetivo; no representan owners,
  permisos ni asignaciones corporativas confirmadas.
- El portal no llama directamente al DeviceAgent ni ejecuta comandos.

## Correcciones realizadas

- `README.md`, `CURRENT_CONTEXT.md`, `WORKFLOW.md` y `DEVELOPMENT_PLAN.md`
  registran a los Bloques 9 y 10 `blocked` y al Bloque 11 como unico
  `in_progress`;
- `WORKFLOW.md` registra `be6c4fc` como ultimo resultado publicado y conserva
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
  y `tests/AdminWeb/README.md` distinguen el control plane existente del portal
  todavia no implementado;
- las evidencias de bloques cerrados que contenian referencias al bloque activo
  fueron convertidas en contexto historico;
- `MASTER_PROMPT.md` se actualiza al final como handoff del Bloque 11.

## Diferencias intencionales

- `context/` y `reference/` conservan respuestas e historia y no representan el
  estado de ejecucion actual.
- Documentos de bloques cerrados pueden mencionar cual era su siguiente gate,
  siempre que lo identifiquen como contexto historico.
- El perfil local puede usar fakes; eso no declara validada una integracion
  corporativa.
- `blocked` en el Bloque 9 significa que su siguiente avance depende de
  evidencia externa; no revierte el incremento local publicado.
- `blocked` en el Bloque 10 significa que el trabajo local reproducible termino,
  pero no existe revision Security ni ensayo en dos endpoints; no equivale a
  `completed`.
- `in_progress` en el Bloque 11 significa que puede iniciar una unidad local
  reemplazable; no implica Entra, RBAC productivo ni portal terminado.

## Resultado

No queda una contradiccion material conocida sobre bloque activo, estado de
Teams, alcance de hardening, estado del ticketing fake o linea base del portal.
Los enlaces Markdown relativos vigentes fueron comprobados sin hallazgos.
