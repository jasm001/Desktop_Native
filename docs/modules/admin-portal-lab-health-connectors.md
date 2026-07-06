# Health de conectores de laboratorio

## Objetivo

Implementar la Unidad 3 de `local-lab-real-data-roadmap.md` con proveedores de
health locales, reemplazables y de solo lectura para `/admin/lab`.

La unidad agrega observabilidad honesta para dependencias de laboratorio sin
ejecutar acciones, sin conectar proveedores corporativos y sin convertir
`validate-only` en despliegue real.

## Alcance implementado

- contrato `AdminLabHealthProvider`;
- adaptador `LocalLabHealthProvider` para Hermes local, mirror de artefactos y
  bridge de laboratorio;
- health de ticketing fake a partir del conteo real local ya leido desde
  PostgreSQL;
- extension de tarjetas de laboratorio con `source`, `scope`, `mode` y
  `lastCheckedAt`;
- estados `available`, `offline`, `unavailable`, `misconfigured`,
  `validate-only` y `not_checked`;
- fail-closed fuera de `Development`, sin realizar probes de red o filesystem;
- validacion de que Hermes y bridge solo aceptan URLs loopback;
- validacion de que el mirror local vive fuera del repositorio Git;
- errores acotados y mensajes saneados sin URLs, secretos, headers ni payloads.

## Comportamiento

Hermes local:

- queda `offline` si no esta habilitado para la sesion;
- queda `misconfigured` si falta configuracion local o el endpoint no es
  loopback;
- queda `available` solo si `/models` responde en loopback dentro del timeout
  acotado;
- no renderiza base URL, API key ni headers.

Mirror de artefactos:

- queda `offline` si no hay ruta configurada o la ruta no existe;
- queda `misconfigured` si apunta dentro del repositorio Git o no es directorio;
- queda `available` si el directorio existe fuera de Git;
- no lista archivos ni lee instaladores.

Bridge de laboratorio:

- solo acepta modo `validate-only`;
- queda `validate-only` aunque el health responda correctamente;
- no se muestra como despliegue enviado;
- si no hay endpoint configurado, conserva el patron validate-only documentado
  sin probar red.

Ticketing fake:

- queda `available` cuando PostgreSQL responde al conteo local;
- muestra registros fake persistidos, no tickets OpenText.

## Fronteras

- No agrega migraciones, Route Handlers, Server Actions, formularios ni
  mutaciones.
- No llama al DeviceAgent.
- No inicia ni detiene servicios, VM, Hyper-V, bridge, mirror o Hermes.
- No conecta UEMS, Entra, Teams, OpenText, Rescue, Sophos, PKI ni servicios
  productivos.
- No renderiza secretos, headers, cookies, CSRF, tenants, hostnames reales,
  endpoints ni rutas internas.
- Fuera de `Development` no ejecuta probes.

## Validacion

Pruebas agregadas:

- health disponible con Hermes, mirror y bridge locales;
- dependencias apagadas u offline;
- configuracion invalida o no loopback;
- fail-closed fuera de `Development` sin llamar `fetch` ni `stat`.

Validaciones ejecutadas el 2026-07-05:

- `corepack pnpm@11.5.3 --filter @it-support-native/admin-web run test`;
- `corepack pnpm@11.5.3 --filter @it-support-native/admin-web run lint`;
- `corepack pnpm@11.5.3 run test:integration`;
- `.\scripts\Validate.ps1`.

El recorrido Playwright administrativo se intento. La primera ejecucion fallo
por un `ChunkLoadError` de Turbopack al correr en paralelo con otro gate que
escribia `.next`; los reintentos dentro del sandbox quedaron bloqueados por la
descarga del engine de Prisma y no fue posible solicitar otra ejecucion fuera
del sandbox por limite de uso. La ruta `/admin/lab` mantiene la cobertura
Playwright previa y esta unidad agrega pruebas unitarias de los estados nuevos.

La evidencia de gate vive en `WORKFLOW.md` para la unidad validada.

## Resultado

La Unidad 3 queda implementada como incremento local del portal. Los Bloques 9 y
10 permanecen `blocked`, y el Bloque 11 permanece `in_progress`.
