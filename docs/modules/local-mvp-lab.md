# MVP local de laboratorio

## Objetivo

Construir una demostracion funcional de extremo a extremo mientras se resuelve
la aprobacion corporativa. El entorno es una VM Windows 11 personal, sin datos,
credenciales, paquetes ni endpoints corporativos.

El laboratorio demuestra arquitectura y comportamiento. No acredita despliegue,
seguridad, licenciamiento ni operacion corporativa.

## Estado actual

Los incrementos de adaptador y control plane del laboratorio estan completados:

- `seven-zip.msi.v1` para 7-Zip 26.01 x64;
- mirror filesystem temporal con manifiesto y SHA-256;
- instalacion, verificacion, idempotencia y desinstalacion en VM Windows 11;
- fallos cerrados por mirror ausente y hash corrupto;
- restauracion del checkpoint y confirmacion del estado inicial.
- API Next.js, PostgreSQL y worker locales;
- recorrido WinUI -> API -> agente simulado -> evidencia validado.
- casos internos, evento de escalamiento y ticketing fake idempotente.

El Windows Service instalado y la conexion IPC de Salud desde WinUI siguen
siendo componentes previstos. WinUI ya puede habilitar texto libre contra
Hermes local compatible con OpenAI mediante variables de entorno, solo para
orientacion informativa y sin crear solicitudes ni acciones. RAG local curado
sigue pendiente. El portal ya tiene unidades locales de solo lectura, pero no
forma parte de los gates cerrados de los Bloques 6, 7 y 8 ni representa una
integracion corporativa.

El laboratorio externo Copilot Studio + Power Automate + Bunny Bridge +
Endpoint Central queda registrado solo como patron reutilizable en
`lab-bridge-reuse-notes.md`. Sus dominios, hostnames y usuarios simulan una
operacion empresarial realista, pero siguen siendo valores de laboratorio y no
constituyen supuestos productivos confirmados.

## Datos reales de laboratorio persistidos

El laboratorio puede pasar de muestras sinteticas a datos reales de laboratorio
persistidos bajo la categoria `lab-real-sanitized`.

`lab-real-sanitized` significa evidencia generada por componentes locales o de
laboratorio controlado, saneada antes de persistirse o mostrarse. No significa
datos productivos, corporativos ni de piloto. Esta categoria no cierra los
Bloques 9, 10 u 11 y no sustituye Entra, UEMS, OpenText, Teams, Security, PKI,
hosting ni retencion aprobados.

Fuentes permitidas:

- filas reales creadas en PostgreSQL local por `SupportRequest`, `ExecutionJob`,
  `BotCase`, `ExternalTicket`, auditoria y outbox;
- health checks reales de API local, worker, PostgreSQL, Hermes local, mirror
  local y bridges de laboratorio;
- estado saneado de la VM Windows 11 personal cuando este disponible;
- resultados reales de adaptadores ejecutados solo en VM `local-demo`;
- hashes, versiones, manifiestos y licencias de artefactos libres permitidos;
- folios, correlaciones y estados `validate-only` de bridges de laboratorio.

No se permiten:

- datos, usuarios, tickets, hostnames, tenants, endpoints, paquetes,
  credenciales o identificadores corporativos;
- secretos en repositorio, Markdown, fixtures, capturas o logs;
- cookies, CSRF, headers de sesion, rutas internas o IDs concretos de UEMS;
- salida completa de instaladores, logs completos, prompts completos o archivos
  personales;
- afirmar que una validacion de laboratorio equivale a despliegue, piloto o
  produccion.

Reglas de persistencia:

1. cada dato persistido debe indicar ambiente `Development` o perfil
   `local-demo`;
2. cada vista debe etiquetar la fuente como `fake`, `local`, `validate-only` o
   `lab-real-sanitized`;
3. las consultas administrativas siguen limitadas y de solo lectura;
4. el portal no arranca, detiene ni administra la VM, servicios o bridges;
5. si la VM o una API de laboratorio estan apagadas, la UI muestra
   `not_checked`, `offline` o `unavailable` sin intentar corregirlo;
6. toda integracion nueva debe quedar detras de interfaces reemplazables y
   fallar cerrada fuera de `Development`.

El documento de ruta para implementar esta categoria por unidades vive en
`local-lab-real-data-roadmap.md`.

## Perfil `local-demo`

Componentes previstos:

- WinUI sin privilegios;
- DeviceAgent instalado como Windows Service restringido en la VM;
- IPC con ACL explicita entre el usuario de WinUI y la identidad del servicio;
- diagnosticos reales mostrados en Salud del equipo;
- API, PostgreSQL y worker locales;
- portal e identidad de desarrollo;
- ticketing, Teams, UEMS y demas integraciones mediante fakes;
- referencia externa de bridge validado en modo `validate-only`, sin mezclar
  UEMS, Copilot Studio ni Power Automate como dependencias de este repo;
- mirror local de artefactos;
- Hermes local con API externa opcional para texto libre informativo;
- documentos e indice RAG locales pendientes de curacion;
- modo degradado sin conexion.

La configuracion `local-demo` debe ser identificable y fallar al intentar usarse
como perfil `pilot` o `production`.

## Mirror local

El mirror simula un servidor de artefactos mediante HTTP local o filesystem
controlado. Los instaladores no se versionan en Git.

Solo se usan paquetes:

- libres y de bajo riesgo;
- con licencia que permita redistribucion;
- sin credenciales, drivers ni dependencia corporativa;
- con version y arquitectura fijadas;
- con modo unattended documentado;
- con SHA-256 obligatorio;
- probados en snapshot de VM.

El manifiesto versionado relaciona `artifactId`, producto, version, origen,
licencia, hash, firma disponible y adaptador compatible.

El reempaquetado se prueba solo cuando la licencia permita modificacion y
redistribucion. Un MSIX o wrapper de laboratorio no se considera aprobado para
usuarios reales.

## Hermes y RAG

Hermes se ejecuta localmente como proveedor opcional del chat WinUI. La
inferencia puede usar una API externa de prueba, por lo que requiere Internet y
no se considera IA offline. La configuracion de arranque vive en
`../runbooks/local-hermes-chat.md`.

Permanecen locales:

- documentos Markdown curados;
- chunks y metadatos;
- indice de recuperacion;
- catalogo sintetico;
- fuentes y versiones;
- reglas e intenciones deterministas.

No se indexan ni envian tickets, correos, hostnames, diagnosticos reales,
credenciales, logs completos o documentos corporativos. La clave API vive fuera
del repositorio.

Si la API no esta disponible, el producto usa busqueda local, respuestas
curadas, catalogo y la maquina de estados determinista.

## Operacion sin conexion

El DeviceAgent puede continuar sin red cuando la accion y sus dependencias ya
estan disponibles localmente.

Permitido:

- diagnosticos locales;
- consulta de catalogo y conocimiento cacheados;
- trabajos previamente autorizados con artefacto validado;
- acciones de la politica `local-demo` confirmadas localmente dentro de la VM;
- persistencia de progreso y evidencia;
- sincronizacion posterior idempotente.

No permitido:

- descargar artefactos ausentes;
- crear autorizaciones nuevas fuera de la politica instalada;
- ampliar la allowlist;
- ejecutar texto, scripts o argumentos producidos por Hermes;
- ocultar que el backend, mirror o proveedor de IA no estan disponibles.

## Recorrido demostrable

1. Iniciar WinUI y el Windows Service.
2. Mostrar diagnosticos reales de la VM.
3. Consultar catalogo y conocimiento local.
4. Usar Hermes cuando exista conexion y fallback determinista cuando no.
5. Proponer una accion conocida y solicitar confirmacion.
6. Resolver un artefacto del mirror local y verificar SHA-256.
7. Ejecutar `Detect`, `Preflight`, `Install` y `Verify`.
8. Repetir la instalacion para validar idempotencia.
9. Ejecutar `Uninstall` y verificar ausencia.
10. Conservar evidencia local y simular sincronizacion al recuperar conexion.

## Limite de avance

Los Bloques 6, 7 y 8 ya cerraron sus gates locales. El Bloque 9 completo su
incremento local y esta `blocked` por la integracion corporativa de Teams. El
Bloque 10 completo su trabajo local acotado y esta `blocked`; el MVP conserva
hardening, evidencia y runbooks locales/fake conforme a D-072. El Bloque 11
esta `in_progress` con identidad sintetica, navegacion y lecturas
administrativas acotadas, sin integraciones corporativas. Ningun bloque puede
declararse corporativamente validado mientras falten:

- UEMS y proceso de distribucion/retiro;
- cuenta e identidad restringida del servicio;
- Security/Sophos y threat model aprobado;
- Entra y tenant corporativo;
- OpenText de prueba;
- bot de Teams existente;
- PKI/firma y confianza de publicador;
- hosting, retencion y conectividad aprobados.

La decision corporativa cambia proveedores y gates de despliegue, no los
contratos centrales ni las fronteras de seguridad.
