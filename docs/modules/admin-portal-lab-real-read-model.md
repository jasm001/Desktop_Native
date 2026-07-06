# Portal de laboratorio con datos reales saneados

## Objetivo

Implementar las Unidades 1 y 2 de
`local-lab-real-data-roadmap.md` sin convertir el laboratorio en piloto ni
cerrar el Bloque 11 completo.

La unidad agrega una vista administrativa de solo lectura para observar el
estado del laboratorio local y las lecturas reales persistidas en PostgreSQL
local bajo la categoria `lab-real-sanitized`.

## Alcance implementado

- ruta protegida `/admin/lab`;
- capability separada `portal.lab.read`;
- autorizacion server-side fail-closed mediante la identidad sintetica de
  portal ya existente;
- tarjetas de estado para AdminWeb, PostgreSQL local, Worker, Hermes, mirror de
  artefactos, bridge de laboratorio y VM Windows 11;
- estados `available`, `offline`, `unavailable`, `misconfigured`,
  `validate-only` y `not_checked`;
- etiquetas de fuente `local`, `fake`, `validate-only` y
  `lab-real-sanitized`;
- resumen de filas reales locales de `SupportRequest`, `ExecutionJob`,
  `BotCase`, `ExternalTicket`, auditoria y outbox;
- lecturas limitadas con selecciones explicitas;
- tabla compacta de outbox sin payload;
- tabla compacta de tickets fake persistidos sin datos corporativos.

La ruta usa el shell administrativo existente y no agrega nuevas APIs publicas,
migraciones, Server Actions, formularios ni mutaciones.

## Fronteras

- La pagina no inicia servicios, no arranca Hyper-V y no administra la VM.
- La VM Windows 11 puede mostrarse como `not_checked`, `offline` o
  `unavailable` sin que eso sea un fallo del producto.
- Hermes, mirror y bridges pueden quedar `not_checked` hasta que exista un
  contrato de health de laboratorio en una unidad posterior.
- El portal no llama directamente al DeviceAgent.
- No se renderizan secretos, API keys, headers, cookies, CSRF, endpoints
  corporativos, tenants, hostnames reales ni payloads completos.
- Los tickets siguen siendo fake aunque el registro sea una fila real del
  laboratorio local.

## Autorizacion

`portal.lab.read` se agrega al conjunto cerrado de capabilities del portal. Como
las demas rutas administrativas locales, solo se concede a la identidad
`DeveloperAllAccess` cuando el entorno es `Development` y el flag explicito de
portal esta habilitado. Identidades, roles o capabilities desconocidos se
rechazan.

## Validacion

Validaciones ejecutadas el 2026-07-05:

- `corepack pnpm@11.5.3 --filter @it-support-native/admin-web run test`;
- `corepack pnpm@11.5.3 --filter @it-support-native/admin-web run lint`;
- `corepack pnpm@11.5.3 run test:integration`;
- `corepack pnpm@11.5.3 --filter @it-support-native/admin-web run test:e2e`.

La integracion directa del paquete AdminWeb contra la `DATABASE_URL` local de
sesion no se uso como evidencia porque esa base estaba sucia y el trigger
append-only rechazo limpiar `audit_events`. El gate oficial
`corepack pnpm@11.5.3 run test:integration` creo una base PostgreSQL efimera,
aplico las cuatro migraciones y paso correctamente.

## Resultado

Las Unidades 1 y 2 quedan implementadas como incremento local validado. Los
Bloques 9 y 10 permanecen `blocked`, y el Bloque 11 permanece `in_progress`
hasta cumplir sus gates productivos.

Nota posterior: la Unidad 3 agrega ese contrato de health en
`admin-portal-lab-health-connectors.md`.
