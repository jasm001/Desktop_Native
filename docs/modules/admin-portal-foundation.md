# Fundacion local del portal administrativo

## Estado

Primera unidad local del Bloque 11 implementada. El bloque permanece
`in_progress`; esta unidad no representa autenticacion productiva, RBAC
corporativo ni cierre del portal.

## Alcance exacto

La ruta `/admin` agrega:

- identidad sintetica de portal `local-portal-operator-001`, separada de
  `development-user-001` y `local-agent-001`;
- habilitacion exclusiva con `IT_SUPPORT_ENVIRONMENT=development`,
  `LOCAL_DEVELOPMENT_PORTAL_IDENTITY_ENABLED=true` y el rol local conocido
  `DeveloperAllAccess`;
- autorizacion server-side para la capability
  `portal.dashboard.read` antes de obtener o renderizar contenido protegido;
- politica deny-by-default para ambientes, flags, roles y capabilities
  desconocidos;
- shell accesible y adaptable de solo lectura con datos sinteticos;
- una vista de acceso no disponible que no filtra contenido protegido;
- paleta visual azul, blanca y verde con foco visible y navegacion semantica.

La pagina `/` conserva la superficie tecnica del control plane. No se agregan
formularios, Server Actions, Route Handlers, tablas Prisma, migraciones,
auditoria, outbox ni efectos laterales.

## Contraste de identidades

| Identidad | Sujeto local | Uso |
| --- | --- | --- |
| Usuario de API | `development-user-001` | Solicitudes confirmadas del control plane |
| Agente | `local-agent-001` | Claim y resultado de trabajos sinteticos |
| Portal | `local-portal-operator-001` | Lectura del shell administrativo |

La identidad de portal no reutiliza headers, tokens ni sujetos del agente. Su
configuracion vive en variables propias y no autoriza APIs del agente.

## Contratos internos

- `PortalPrincipal`: sujeto, nombre sintetico y rol local conocido.
- `PortalCapability`: capability cerrada `portal.dashboard.read`.
- `getDevelopmentPortalIdentity`: valida ambiente, flag y rol exactos.
- `authorizePortalCapability`: rechaza primero roles y capabilities
  desconocidos, despues verifica la matriz local.
- `requireDevelopmentPortalAccess`: compone identidad y autorizacion para una
  ruta server-side.
- `getAdminOverview`: devuelve un objeto inmutable en memoria; no realiza I/O.

Los componentes React reciben un principal ya autorizado y datos ya resueltos.
No contienen reglas de identidad, dominio o persistencia.

## Alternativas consideradas

- Reutilizar `getDevelopmentIdentity`: rechazado porque mezcla al solicitante de
  API con el operador administrativo.
- Reutilizar la identidad del agente: rechazado porque rompe la separacion entre
  usuario y dispositivo y ampliaria privilegios.
- Agregar Entra/OIDC ahora: rechazado porque tenant, App Registration, sesiones
  y owners no estan aprobados.
- Agregar Fluent UI: diferido; el shell minimo no necesita una dependencia
  nueva para cumplir semantica, foco y layout.
- Agregar Playwright: diferido hasta que existan recorridos y roles suficientes.
  Esta unidad usa Vitest y verificacion manual de navegador.
- Leer Prisma para mostrar metricas: rechazado en esta unidad para conservar
  ausencia verificable de efectos y dependencia de base de datos.

## Punto de sustitucion OIDC/Entra

Una implementacion futura reemplazara
`getDevelopmentPortalIdentity` por un proveedor server-side de sesion OIDC que
produzca `PortalPrincipal`. La politica de capabilities y la llamada
`requireDevelopmentPortalAccess` permanecen como frontera de autorizacion.

El reemplazo requerira App Registration, tenant, contrato de claims, sesiones,
MFA, owners, asignaciones reales, scopes y segregacion de funciones. Ninguno de
esos valores se simula en esta unidad.

## Accesibilidad y respuesta adaptable

- enlace para saltar al contenido;
- `aside`, `nav`, `main`, headings, listas y `dl` semanticos;
- `aria-current` en la navegacion seleccionada;
- foco visible de alto contraste;
- controles nativos de enlace utilizables por teclado;
- iconos decorativos ocultos a tecnologias de asistencia;
- layout lateral amplio, navegacion superior intermedia y composicion vertical
  movil;
- soporte de `prefers-reduced-motion`.

## Evidencia y criterio de aceptacion

- ambiente `test`, `production`, flag ausente o deshabilitado: acceso denegado;
- rol desconocido: acceso denegado;
- capability desconocida: acceso denegado;
- perfil exacto de desarrollo y capability conocida: acceso permitido;
- principal de portal distinto a usuario de API y agente;
- overview inmutable en memoria y sin I/O;
- sin cambios a Prisma, migraciones, APIs, contratos, worker o DeviceAgent.

Validacion ejecutada:

- 16 pruebas unitarias de AdminWeb, incluidas 6 nuevas del portal;
- lint y TypeScript estricto;
- build Next.js con `/admin` dinamica y server-rendered;
- 136 pruebas .NET, 10 contratos web y 3 unitarias del worker;
- 11 integraciones AdminWeb y 4 del worker sobre PostgreSQL efimero;
- cuatro migraciones existentes aplicadas sin cambios;
- E2E WinUI/control plane/DeviceAgent;
- QA de navegador en `1440x1000` y `390x844`, sin overflow;
- primer foco de teclado en el enlace para saltar contenido;
- perfil `production` sin heading, identidad ni secciones protegidas;
- auditoria de dependencias sin vulnerabilidades conocidas;
- escaneo de secretos sin hallazgos.

## Riesgos residuales

- no existe autenticacion real, sesion, expiracion, CSRF ni logout;
- `DeveloperAllAccess` sigue siendo un rol sintetico de desarrollo;
- no existen scopes por sede o area, segregacion ni alertas;
- la vista usa datos sinteticos, no consultas administrativas paginadas;
- faltan Fluent UI, Testing Library, Playwright por rol y auditoria de
  mutaciones;
- faltan hosting, observabilidad, retencion y revision Security del portal.

Estos riesgos impiden cerrar el Bloque 11, pero no bloquean esta unidad local.
