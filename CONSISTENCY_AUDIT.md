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
- Bloque 9, canal Teams existente: `blocked` por evidencia externa.
- Bloque principal activo: Bloque 10, endurecimiento para piloto,
  `in_progress`.
- Documento propietario del Bloque 9: `modules/TEAMS.md`.
- Documento propietario del Bloque 10: `modules/PILOT_HARDENING.md`.
- Bloque 11 permanece `pending`.
- Gate completo vigente: 125 pruebas .NET, 20 pruebas Node unitarias/de
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
`blocked`. D-072 permite continuar el Bloque 10 con proveedores locales sin
declarar cerrada la integracion Teams.

## Alineacion del Bloque 10

- El bloque tiene un documento propietario y es el unico `in_progress`.
- El threat model dejo de ser un placeholder y enumera activos, fronteras,
  amenazas, controles actuales y evidencia pendiente.
- El alcance local no autoriza UEMS, Entra, Sophos, PKI, Teams, OpenText ni
  endpoints corporativos.
- El gate externo sigue exigiendo revision del threat model y ensayo de
  despliegue/retiro en dos endpoints autorizados.
- El portal administrativo permanece reservado para el Bloque 11.

## Correcciones realizadas

- `README.md`, `CURRENT_CONTEXT.md`, `WORKFLOW.md` y `DEVELOPMENT_PLAN.md`
  registran al Bloque 9 `blocked`, al Bloque 10 como unico `in_progress` y al
  Bloque 11 `pending`;
- `WORKFLOW.md` registra `0448a42` como ultimo resultado publicado y elimina una
  entrada duplicada de auditoria de dependencias;
- `modules/PILOT_HARDENING.md` define alcance, limites y gate del Bloque 10;
- `docs/threat-model/README.md` contiene el inventario inicial verificable;
- `modules/TEAMS.md` conserva el incremento local y el gate corporativo;
- `MASTER_PROMPT.md` apunta al primer incremento local del Bloque 10 y protege
  los Bloques 9 y 11.

## Diferencias intencionales

- `context/` y `reference/` conservan respuestas e historia y no representan el
  estado de ejecucion actual.
- Documentos de bloques cerrados pueden mencionar cual era su siguiente gate,
  siempre que lo identifiquen como contexto historico.
- El perfil local puede usar fakes; eso no declara validada una integracion
  corporativa.
- `blocked` en el Bloque 9 significa que su siguiente avance depende de
  evidencia externa; no revierte el incremento local publicado.

## Resultado

No queda una contradiccion material conocida sobre bloque activo, estado de
Teams, alcance de hardening, estado del ticketing fake o limites del portal.
