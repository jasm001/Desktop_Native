# Portal administrativo web

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

## Provisionamiento MVP

- Sin auto-registro.
- Un super admin invita por correo empresarial.
- El dominio permitido reduce errores, pero no sustituye autenticacion.
- El IdP mantiene contrasena/OTP, MFA, recuperacion y bloqueo.
- El portal almacena identificador, correo, nombre, estado y roles; nunca hashes
  propios si se usa un IdP externo.
- Alta, baja, cambio de rol, reset y exportacion quedan auditados.
- Acceso puede limitarse por pais, sede, area, grupo y proyecto.

El MVP integra Microsoft Entra para login corporativo y MFA. La aplicacion no
recibe la contrasena: redirige al login Microsoft mediante OIDC. Los roles se
asignan manualmente en la base de datos durante el MVP. Grupos Entra,
provisionamiento automatico y roles corporativos quedan para despues.

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
