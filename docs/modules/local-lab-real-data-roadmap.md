# Ruta de datos reales de laboratorio persistidos

## Estrategia

El objetivo es mejorar la visualizacion del producto final reemplazando muestras
sin vida operativa por datos reales generados en el laboratorio local. La ruta no
conecta servicios productivos, no usa datos corporativos y no cierra los Bloques
9, 10 u 11.

La categoria propietaria es `lab-real-sanitized`, definida en
`local-mvp-lab.md`. El portal administrativo la consume bajo el gobierno de
`modules/ADMIN_PORTAL.md`: server-side, fail-closed, solo lectura y sin
mutaciones.

## Ruta local

Las cinco unidades son independientes. Cada una debe poder validarse y
documentarse sin depender de que la unidad siguiente exista.

Permitido en toda la ruta:

- `Development` y perfil `local-demo` solamente;
- PostgreSQL local o efimero;
- servicios locales, fakes y bridges de laboratorio `validate-only`;
- datos reales creados por el propio recorrido local;
- VM Windows 11 personal cuando este encendida por decision del operador;
- estados honestos cuando una dependencia este apagada o no comprobada.

Prohibido en toda la ruta:

- datos, endpoints, tenants, usuarios, hostnames o tickets corporativos;
- secretos en archivos del repositorio;
- mutaciones administrativas desde el portal;
- llamadas directas del portal al DeviceAgent;
- arranque, apagado o administracion de VM desde el portal;
- afirmar instalacion o despliegue real si el proveedor solo valido la
  solicitud.

## Unidad 1: vista de estado del laboratorio

Crear una superficie administrativa de solo lectura para mostrar el estado de
conectores y componentes locales.

Entregables:

- ruta administrativa protegida para laboratorio;
- capability local separada de lectura;
- tarjetas de estado para API local, PostgreSQL, worker, Hermes, mirror,
  bridges de laboratorio y VM;
- estados `not_checked`, `available`, `offline`, `unavailable`,
  `misconfigured` y `validate-only`;
- etiquetas visibles de fuente `local`, `fake`, `validate-only` o
  `lab-real-sanitized`;
- manejo explicito de VM apagada sin fallo de pagina.

Gate:

- autorizacion server-side y acceso denegado probados;
- ninguna tarjeta ejecuta comandos, inicia servicios, arranca VM o llama al
  DeviceAgent;
- secretos y URLs sensibles nunca se renderizan;
- desktop, movil, teclado, solo lectura y ausencia de overflow cubiertos.

Estado: `completed` como incremento local validado en
`admin-portal-lab-real-read-model.md`.

## Unidad 2: lecturas operativas reales locales

Reemplazar estados vacios o puramente sinteticos por lecturas reales de
PostgreSQL local ya generadas por los flujos existentes.

Entregables:

- resumen de `SupportRequest`, `ExecutionJob`, `BotCase`, `ExternalTicket`,
  auditoria y outbox;
- consultas limitadas con selecciones explicitas y sin payload sensible;
- diferenciacion entre datos `fake`, `synthetic` y `lab-real-sanitized`;
- estados vacios cuando la base no tenga actividad;
- documentacion de que los tickets siguen siendo fake aunque el registro sea
  real en laboratorio.

Gate:

- no se agregan migraciones ni mutaciones;
- no se exponen payloads completos, prompts, secretos ni salida operativa;
- las lecturas fallan cerrado si la identidad o capability no corresponde;
- pruebas unitarias/componentes y Playwright cubren lectura y ausencia de
  formularios mutantes.

Estado: `completed` como incremento local validado en
`admin-portal-lab-real-read-model.md`.

## Unidad 3: conectores simulados con health real

Agregar proveedores de laboratorio que reporten salud real sin ejecutar acciones
productivas.

Entregables:

- adaptadores de health para Hermes, mirror local, bridge `validate-only` y
  ticketing fake;
- contrato de estado con fuente, alcance, modo y ultima comprobacion;
- soporte de dependencia apagada o no configurada;
- errores saneados y acotados.

Gate:

- fuera de `Development` todo falla cerrado;
- no se registran secretos ni headers;
- `validate-only` nunca se muestra como despliegue enviado;
- pruebas cubren disponible, apagado, mal configurado y denegado.

Estado: `completed` como incremento local validado en
`admin-portal-lab-health-connectors.md`.

## Unidad 4: catalogo local curado

Usar un catalogo de laboratorio basado en artefactos libres reales permitidos,
sin convertirlo en catalogo corporativo.

Entregables:

- entradas de laboratorio con producto, version, arquitectura, licencia, origen
  publico, hash y adaptador compatible;
- distincion clara entre catalogo sintetico, catalogo de laboratorio y catalogo
  corporativo futuro;
- validaciones para licencia redistribuible, version fija y SHA-256;
- estados de artefacto disponible, ausente y hash no coincidente.

Gate:

- no se versionan instaladores en Git;
- no se agregan paquetes comerciales ni corporativos;
- fallos de artefacto ocurren antes de iniciar ejecucion;
- documentacion y pruebas conservan `local-demo` como unico perfil habilitado.

Estado: `pending`.

## Unidad 5: recorrido end-to-end visual de laboratorio

Componer una demostracion visual que use datos reales de laboratorio persistidos
sin depender de integraciones productivas.

Entregables:

- recorrido WinUI -> API local -> worker -> agente simulado o VM -> evidencia
  saneada -> caso -> ticket fake;
- vista administrativa que muestre la trazabilidad completa por correlacion;
- estados de degradacion cuando Hermes, mirror, bridge o VM no esten
  disponibles;
- evidencia de idempotencia y ausencia de duplicados.

Gate:

- una consulta no crea solicitudes;
- solo la confirmacion explicita crea solicitud, trabajo y caso;
- no se ejecutan comandos generados por IA ni texto operativo;
- el cierre documenta que el recorrido es demostracion local, no piloto.

Estado: `pending`.

Nota posterior: al completar las cinco unidades de este roadmap, conviene crear
un roadmap separado para WinUI. Ese documento debe definir como la app nativa
consume datos reales de laboratorio mediante APIs/contratos de solo lectura, sin
mezclar el gobierno del portal con la experiencia nativa.

## Secuencia recomendada

Avanzar primero por las Unidades 1 y 2. Juntas dan visibilidad real del
laboratorio sin requerir que la VM este siempre encendida ni introducir
conectores de accion.

La Unidad 3 ya estabiliza el contrato de health local. Las Unidades 4 y 5
dependen de que la categoria `lab-real-sanitized` y los conectores locales ya
esten visibles y probados en el portal.
