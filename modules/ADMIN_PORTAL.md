# Portal administrativo web

## Estado

Documento propietario del Bloque 11 `in_progress`, unico bloque principal
activo.

Los Bloques 0 a 8 estan `completed`. Los Bloques 9 y 10 permanecen `blocked`
por evidencia externa. D-072 permite avanzar este bloque con identidad, datos y
proveedores locales reemplazables sin cerrar esos gates.

## Linea base real

`src/AdminWeb` ya contiene el control plane Next.js modular, APIs v1,
Prisma/PostgreSQL, identidad sintetica de desarrollo, auditoria, outbox,
solicitudes, trabajos, casos y ticketing fake.

La primera unidad local agrega `/admin`, una identidad sintetica de portal
separada y autorizacion server-side fail-closed. La segunda unidad agrega
navegacion real hacia `/admin/catalog`, `/admin/operations` y `/admin/audit`,
capabilities de lectura separadas y proyecciones Prisma limitadas. La tercera
unidad agrega pruebas de componentes y Playwright. La cuarta unidad cierra el
esqueleto local con `/admin/access`, `/admin/approvals`, `/admin/support` y
`/admin/reporting`, todas sinteticas, protegidas en servidor y de solo lectura.
La pagina `/` permanece como superficie tecnica.

Todavia no contiene:

- autenticacion OIDC/Entra, sesiones productivas ni MFA;
- RBAC productivo, scopes por sede/area ni asignaciones reales;
- Fluent UI React;
- formularios o mutaciones administrativas;
- pruebas por rol productivo;
- Entra, MFA, grupos, usuarios o datos corporativos;
- OpenText, Rescue, Teams o UEMS reales.

La pagina `/` actual es una superficie tecnica del Bloque 8. No debe
confundirse con el portal administrativo terminado.

## Primera unidad local

La primera unidad esta implementada y permanece pequena y no mutante:

1. definir una identidad de portal separada de la identidad del agente;
2. permitir una identidad sintetica solo con ambiente de desarrollo y
   configuracion explicita;
3. negar acceso por defecto fuera de ese perfil;
4. aplicar autorizacion server-side antes de renderizar una ruta administrativa;
5. crear un shell minimo y accesible con datos sinteticos o de solo lectura;
6. probar acceso permitido, denegado y ausencia de efectos laterales;
7. documentar el punto de sustitucion por OIDC/Entra sin conectarlo.

No se crean aun usuarios, roles o scopes corporativos. Los nombres de roles
productivos de este documento son el modelo objetivo, no asignaciones reales.
La especificacion y evidencia viven en
`../docs/modules/admin-portal-foundation.md`.

## Segunda unidad local

La segunda unidad permanece de solo lectura:

1. separa capabilities para dashboard, catalogo, operaciones y auditoria;
2. protege cada ruta en servidor antes de resolver datos;
3. usa el catalogo sintetico ya existente;
4. consulta como maximo 25 solicitudes y 25 eventos recientes;
5. selecciona campos explicitos y excluye el payload de auditoria;
6. mantiene vacios accesibles cuando PostgreSQL no tiene actividad;
7. prueba limites y ausencia de efectos laterales sobre PostgreSQL real.

No se crean roles, scopes, asignaciones, tablas ni migraciones nuevas. El unico
rol local sigue siendo `DeveloperAllAccess`. La evidencia vive en
`../docs/modules/admin-portal-read-model.md`.

## Tercera unidad local

La tercera unidad cierra calidad local no bloqueante sin ampliar la superficie
operativa:

1. agrega Testing Library/jsdom para componentes del shell, tablas y estados
   vacios;
2. agrega Playwright para recorridos reales de `/admin`, `/admin/catalog`,
   `/admin/operations`, `/admin/audit` y acceso denegado;
3. ejecuta los recorridos contra PostgreSQL efimero y servidores Next locales
   separados para identidad invalida y valida;
4. verifica desktop, movil, teclado, foco inicial, estado activo, ausencia de
   overflow horizontal y ausencia de formularios o botones mutantes en `main`;
5. conserva el rechazo de roles/capabilities desconocidos sin crear roles,
   owners, asignaciones o scopes productivos;
6. documenta que Fluent UI React se difiere porque adoptarlo ahora aumentaria
   reescritura y riesgo sin mejorar el cierre de pruebas locales.

No se agregan migraciones, mutaciones, integraciones, secretos ni conexiones
corporativas. La evidencia vive en
`../docs/modules/admin-portal-local-quality-gate.md`.

## Cuarta unidad local

La cuarta unidad cierra el esqueleto local no productivo:

1. agrega capabilities locales `portal.identity.read`,
   `portal.approvals.read`, `portal.support.read` y
   `portal.reporting.read`;
2. agrega rutas server-rendered `/admin/access`, `/admin/approvals`,
   `/admin/support` y `/admin/reporting`;
3. reutiliza el shell y la identidad sintetica del portal;
4. mantiene contenido en memoria, sin Prisma nuevo, migraciones, formularios,
   Server Actions, Route Handlers ni mutaciones;
5. representa identidad/acceso, aprobaciones, tickets/soporte remoto y
   reportes/configuracion como estados locales vacios o diferidos;
6. amplia Testing Library y Playwright para acceso permitido/denegado, teclado,
   estado activo, solo lectura y ausencia de overflow.

No se agregan Entra, OpenText, Rescue, Teams, UEMS, roles productivos, owners,
scopes, secretos ni datos corporativos. La evidencia vive en
`../docs/modules/admin-portal-local-skeleton-closure.md`.

## Siguiente avance

El siguiente avance no bloqueado puede pasar del esqueleto local hacia datos
reales de laboratorio persistidos si permanece dentro de las fronteras actuales:

1. agregar una superficie de laboratorio de solo lectura para estado de
   conectores y componentes locales;
2. mantenerla protegida en servidor, de solo lectura y etiquetada como
   `local-demo`;
3. reutilizar el shell y la identidad de portal existentes;
4. cubrir acceso permitido/denegado, teclado, foco, estados activos, estados
   vacios, dependencias apagadas y ausencia de overflow;
5. no agregar mutaciones, migraciones, integraciones corporativas, roles
   productivos, owners ni scopes reales.

Aunque el esqueleto local este cerrado, el Bloque 11 seguira `in_progress`
hasta cumplir los gates productivos de este documento.

## Datos reales de laboratorio persistidos

El portal puede leer la categoria `lab-real-sanitized` definida en
`../docs/modules/local-mvp-lab.md`. Esta categoria permite visualizar evidencia
real generada por el laboratorio local, como filas de PostgreSQL local, health
checks de servicios locales, resultados saneados de VM `local-demo`, manifiestos
de artefactos libres y estados `validate-only` de bridges de laboratorio.

Restricciones para cualquier superficie administrativa que lea esta categoria:

- identidad de portal sintetica solo en `Development`;
- capability de lectura separada y autorizacion server-side fail-closed;
- sin formularios, Server Actions, Route Handlers mutantes ni migraciones salvo
  que una unidad posterior los justifique explicitamente;
- sin secretos, payloads completos, prompts completos, logs completos, cookies,
  CSRF, headers, tenants, endpoints o identificadores corporativos;
- sin llamadas directas al DeviceAgent;
- sin comandos, scripts, rutas, argumentos operativos ni texto ejecutable;
- sin arranque, apagado o administracion de VM, servicios o bridges desde el
  portal.

La VM Windows 11 de laboratorio no es una dependencia siempre encendida. Una
vista administrativa puede mostrarla como `not_checked`, `offline` o
`unavailable` cuando no se haya iniciado manualmente o no exista una API local
habilitada para reportar salud. El portal no debe iniciar Hyper-V ni inferir que
una VM apagada es un fallo del producto.

La ruta de unidades independientes vive en
`../docs/modules/local-lab-real-data-roadmap.md`. Sus unidades no completan el
Bloque 11 ni desbloquean los Bloques 9 o 10.

## Responsabilidad

Es el plano de control del producto. Se usa para gestionar identidades, equipos,
catalogo, aprobaciones, tickets, sesiones de soporte, telemetria y auditoria.
No ejecuta comandos ni se conecta directamente a endpoints.

Los agentes de mesa de ayuda operan desde este portal y abren el cliente de
Rescue mediante deep link/integracion. No se construye una segunda aplicacion
nativa para tecnicos durante el MVP.

La vista de OpenText puede intentar integrar `https://help.softtek.com/sm/ess.do`
en un iframe solo si el portal permite framing mediante CSP/X-Frame-Options y la
sesion corporativa funciona con cookies entre sitios. Si OpenText lo bloquea, el
portal muestra un enlace profundo/nueva pestana. No se relajaran protecciones ni
se usara un proxy para saltar controles del portal corporativo.

## Roles de produccion

| Rol | Alcance |
| --- | --- |
| Platform Super Admin | Configuracion global y recuperacion; uso excepcional |
| Identity Admin | Invitar, desactivar y asignar alcance a usuarios |
| Catalog Admin | Normalizar productos, versiones y paquetes |
| Security Approver | Aprobar/rechazar software no listado y excepciones |
| License/Purchasing Approver | Validar licencias, costo y centro/proyecto |
| Help Desk Agent | Ver cola asignada, diagnostico y escalar/atender |
| Help Desk Lead | Reasignar, supervisar SLA y agentes de su area |
| Site Operator | Operar equipos de una sede sin acceso global |
| Auditor | Lectura y exportacion de evidencia |
| Reporting Viewer | Metricas agregadas sin acciones |

Los roles se pueden combinar solo cuando la segregacion de funciones lo permita.
En desarrollo existe `DeveloperAllAccess`; el backend rechaza ese rol fuera de
`Development`.

## Provisionamiento objetivo

- Sin auto-registro.
- Un super admin invita por correo empresarial.
- El dominio permitido reduce errores, pero no sustituye autenticacion.
- El IdP mantiene contrasena/OTP, MFA, recuperacion y bloqueo.
- El portal almacena identificador, correo, nombre, estado y roles; nunca hashes
  propios si se usa un IdP externo.
- Alta, baja, cambio de rol, reset y exportacion quedan auditados.
- Acceso puede limitarse por pais, sede, area, grupo y proyecto.

El piloto corporativo debera integrar Microsoft Entra para login y MFA. La
aplicacion no recibe la contrasena: redirige al login Microsoft mediante OIDC.
Los roles se asignaran manualmente en la base de datos durante el MVP. Grupos
Entra, provisionamiento automatico y roles corporativos quedan para despues.
Nada de esta integracion esta habilitado en el perfil local.

## Modulos

1. Dashboard operativo.
2. Usuarios, roles y scopes.
3. Sedes, areas, proyectos y equipos.
4. Catalogo y artefactos.
5. Aprobaciones de seguridad/licencias/compras.
6. Trabajos y ejecuciones.
7. Tickets OpenText.
8. Soporte remoto Rescue.
9. Base de conocimiento y revision de soluciones.
10. Auditoria, reportes y configuracion.

## Portal de autoservicio posterior

Si el producto opera de forma independiente, el portal puede ofrecer al usuario:

- dispositivos propios enrolados;
- perfiles de ambientacion disponibles;
- aplicaciones seleccionadas para despues de una reinstalacion;
- descarga de bootstrap firmado con token de un solo uso;
- progreso de preparacion y reprovisionamiento;
- revocacion/desenrolamiento del dispositivo;
- paquetes de evidencia que el usuario autorizo compartir.

El bootstrap no incluye secretos ni el perfil completo. Tras autenticar, canjea
un token corto vinculado a usuario, dispositivo, expiracion y una sola
instalacion.

Desenrolar revoca certificados, tokens, campanas y acceso futuro antes de una
instalacion limpia. No intenta borrar remotamente un equipo sin autorizacion
reforzada y controles adicionales.

El modulo de tickets muestra primero los casos propios del bot y sus metricas.
Las escalaciones incluyen enlace al ticket real de OpenText; no se replica desde
cero toda la interfaz de Service Manager.

## Flujos de aprobacion

- `approved_freeware`: instalacion automatizable segun politica.
- `commercial`: requiere licencia/compras y asociacion a proyecto.
- `customer_provided`: requiere aprobacion formal del cliente/proyecto.
- `not_listed`: requiere seguridad y actualizacion del catalogo.
- `prohibited`: rechazo; solo excepcion formal fuera del flujo normal.

Nadie puede crear el paquete, aprobarlo y publicarlo a produccion por si solo.

## Super admin

- En operacion formal, mantener dos cuentas break-glass.
- MFA resistente a phishing cuando el IdP lo permita.
- Credenciales almacenadas por procedimiento corporativo.
- Alertas por cada inicio de sesion o cambio realizado.
- Sin uso para soporte diario.
- Revision periodica de asignaciones y sesiones.

## Gate del Bloque 11

El bloque solo puede declararse `completed` cuando:

- autenticacion y autorizacion se aplican server-side y fallan cerradas;
- roles y scopes tienen pruebas de acceso permitido y denegado;
- las mutaciones administrativas requieren identidad, autorizacion,
  confirmacion, correlacion e idempotencia;
- migraciones y auditoria cubren los datos nuevos;
- Playwright valida recorridos principales por rol y accesibilidad;
- el portal no ejecuta comandos ni llama directamente al DeviceAgent;
- OpenText y Rescue usan adaptadores o enlaces aprobados, no bypasses;
- no hay secretos, PII innecesaria ni identidades corporativas en fixtures;
- los gates completos del repositorio siguen pasando.

Las unidades locales actuales no cierran este gate.

## Referencias

- `../core/SECURITY.md`
- `../core/ARCHITECTURE.md`
- `../core/STACK.md`
- `WEB_DELIVERY.md`
- `../docs/modules/control-plane-foundation.md`
- `../docs/modules/control-plane-local-flow.md`
- `../docs/modules/admin-portal-local-skeleton-closure.md`
- `../docs/modules/local-lab-real-data-roadmap.md`
