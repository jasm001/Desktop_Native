# Alcance

## MVP local demostrable

Mientras se resuelve la aprobacion corporativa, el repositorio puede avanzar en
una demostracion local de extremo a extremo sobre una VM Windows 11 personal,
sin datos ni credenciales corporativas.

Este perfil `local-demo` puede incluir:

- DeviceAgent instalado como Windows Service de laboratorio;
- diagnosticos reales del equipo mostrados en WinUI mediante IPC;
- adaptadores cerrados para software libre y redistribuible, probados solo en
  la VM;
- un origen local de artefactos que simula un mirror mediante HTTP o filesystem
  controlado, con manifiestos, versiones y SHA-256;
- API, PostgreSQL, worker, portal y tickets fake ejecutados localmente;
- datos reales de laboratorio persistidos y saneados bajo
  `lab-real-sanitized`, generados solo por componentes locales controlados;
- Hermes como proveedor opcional del chat, ejecutado localmente y consumiendo
  una API externa solo con contenido publico o sintetico;
- RAG local con documentos curados, versionados y sin datos corporativos;
- modo degradado sin conexion basado en catalogo, conocimiento, politicas,
  autorizaciones y artefactos ya disponibles localmente.

El perfil local no equivale a piloto ni produccion. No usa UEMS, OpenText,
Entra, Teams corporativo, PKI corporativa, paquetes comerciales, datos reales
corporativos ni endpoints internos. Las integraciones no disponibles se
mantienen detras de adaptadores fake o contratos reemplazables.

Los datos `lab-real-sanitized` son reales solo dentro del laboratorio: filas
locales de PostgreSQL, health checks locales, evidencia saneada de VM
`local-demo`, manifiestos de artefactos libres y estados `validate-only`. No son
datos productivos ni corporativos y no sustituyen gates externos.

La falta de aprobacion corporativa no obliga a reescribir el dominio. Cambia los
proveedores de identidad, artefactos, IA, tickets, despliegue y hosting.

## MVP productivo

### Experiencia nativa

- Inicio de sesion empresarial.
- Ventana WinUI 3 con Fluent, Mica, tema claro/oscuro y ES/EN.
- Navegacion: Inicio, Catalogo, Asistente, Salud, Solicitudes y Ayuda.
- Notificaciones nativas y estado en bandeja del sistema.
- Accesibilidad por teclado, lector de pantalla, escala y alto contraste.
- Paridad de identidad, composicion y densidad con el mockup aprobado.
- Consultas sobre software permitido, no listado, prohibido, EOL o con licencia.

### Canal Teams

- Consultar catalogo y preguntas frecuentes.
- Crear solicitudes, consultar estado y recibir notificaciones.
- Escalar al mismo backend y OpenText que la aplicacion nativa.
- Solicitar acciones para un equipo registrado sin ejecutar comandos desde
  Teams.
- Requerir Internet y acceso vigente a la cuenta corporativa.
- Separar consulta de accion: preguntar no instala ni crea ticket.
- Solicitar confirmacion antes de crear instalacion o escalamiento.

### Software

- Catalogo administrado.
- Instalar, desinstalar, validar y actualizar paquetes aprobados.
- Progreso real, cancelacion solo cuando sea tecnicamente segura.
- Deteccion posterior y evidencia de resultado.
- Catalogo inicial derivado de `Open Source and Freeware Exceptions.xlsx`.
- Solo se publican entradas normalizadas, compatibles con Windows 11, vigentes y
  con paquete tecnico aprobado.

### Dispositivo

- Hostname, version de Windows, arquitectura, almacenamiento, memoria, red,
  cifrado, estado de actualizaciones y agente.
- Diagnosticos no destructivos.
- Sin exploracion arbitraria de archivos del usuario.

### Solicitudes

- Crear y consultar solicitudes.
- Vincular ticket externo.
- Estados coherentes entre accion tecnica y cierre del usuario.
- Escalamiento manual o automatico con contexto y evidencia.
- Caso interno y metricas para toda accion del bot.
- Ticket OpenText obligatorio cuando el bot no resuelve o el servicio requiere
  atencion humana.
- Integracion OpenText minima: crear, correlacionar y enlazar el ticket; lectura
  de estado si la API del piloto la permite.
- Orientacion para recuperacion de cuenta mediante canales oficiales.
- Evaluacion de `recovery.softtek.com` desde el login de Windows, sin Credential
  Provider propio y sin captura de contrasenas.
- Ticket de recuperacion asistida cuando no exista correo personal enrolado.
- Step-up/MFA en el proveedor oficial, no dentro del formulario del bot.
- Flujo comun en WinUI y Teams: consulta, propuesta, confirmacion y accion.

### Administracion

- Portal web separado por roles.
- Login Entra; altas, bajas y roles administrados manualmente en el MVP.
- Catalogo, artefactos, versiones y politicas.
- Flujos de aprobacion para seguridad, compras y licencias.
- Consulta por usuario/equipo/estado/aplicacion/fecha.
- Auditoria exportable.
- Operacion inicial limitada a una sede de Mexico y un grupo controlado.
- Cola web para agentes con contexto del ticket; Rescue conserva el flujo humano
  actual durante el MVP.
- Vista embebida de OpenText solo si sus politicas permiten iframe; fallback a
  enlace profundo.

## Fuera del MVP

- Implementacion propia de control remoto.
- Segunda aplicacion nativa exclusiva para tecnicos.
- Compras o asignacion automatica de licencias.
- Remediacion autonoma basada en IA.
- Scripts libres cargados por administradores.
- Inventario completo tipo CMDB.
- Soporte macOS/Linux.
- Multitenancy comercial.
- Analitica predictiva.
- Restablecer contrasenas directamente desde el bot.
- Generar o entregar contrasenas temporales durante el primer MVP.
- Considerar una sesion abierta de Teams/Windows como unica prueba de identidad.
- Crear un proveedor de credenciales o reemplazar el login de Windows.
- Usar Power Automate Desktop como ejecutor privilegiado principal.
- Promover el perfil `local-demo` directamente a endpoints corporativos.
- Redistribuir o reempaquetar software cuya licencia no lo permita.
- Considerar Hermes con API externa como inferencia offline.
- Autorizar acciones nuevas sin una politica local o remota valida.

## Fase 2

- Integracion bidireccional completa con OpenText: actividades, asignacion,
  estados, soluciones, SLA y cierre.
- Intune/Graph para inventario y despliegues administrados.
- Integracion profunda con GoTo/LogMeIn Rescue.
- Anillos de despliegue, mantenimiento y rollback.
- Base de conocimiento empresarial con RAG y permisos por documento.
- Paquetes de evidencia saneada para tickets y soporte.
- Reparaciones locales tipadas, firmadas y allowlisted.
- Limpieza segura de almacenamiento y diagnostico guiado de perifericos.
- Coordinacion de mantenimiento, actualizaciones y reinicios con consentimiento.
- Perfiles declarativos de ambientacion y continuidad local sin IA.

## Fase 3

- Remediaciones adicionales aprobadas y versionadas.
- Campanas de cumplimiento.
- Reportes ejecutivos y SLO.
- ARM64.
- Multitenancy solo si el producto pasa de interno a comercial.
- Seleccion previa de aplicaciones y restauracion de configuracion permitida
  despues de una reinstalacion.
- Borrado y reprovisionamiento como ultima capacidad, mediante proveedor externo
  aprobado; nunca por persistencia de firmware o kernel propia.

## Direcciones posteriores al MVP

La decision comercial futura no cambia el MVP. El mismo dominio admite dos
perfiles de despliegue:

- empresarial: ISO/UEMS reinstala Windows, seguridad y bootstrap firmado;
- independiente: Windows limpio y portal autenticado entrega un bootstrap
  firmado, de un solo uso y asociado al usuario/dispositivo.

La rama independiente puede evolucionar en tres niveles sin debilitar las
fronteras de seguridad:

1. local: diagnostico, evidencia, mantenimiento y perfiles;
2. personal/pro: reparacion, ambientacion y reprovisionamiento asistido;
3. equipos pequenos/MSP: campanas, catalogo, auditoria e inventario minimo.

La direccion final queda pendiente de aceptacion corporativa y validacion de
mercado. Ninguna variante permite scripts libres, control remoto generico,
credenciales humanas persistidas o reparaciones generadas por IA.

## Criterios de aceptacion del MVP

- Ninguna accion privilegiada puede originarse directamente desde la UI o IA.
- Toda accion queda correlacionada con usuario, equipo, politica, paquete y
  resultado.
- Repetir una solicitud no duplica una instalacion.
- El agente recupera trabajos tras reinicio.
- Un usuario normal no puede ver otros equipos o tickets.
- El catalogo funciona sin depender del proveedor de IA.
- La app cumple teclado, escalado 200 %, alto contraste y lector de pantalla en
  los flujos principales.
- El paquete tiene identidad de publicador verificable mediante el mecanismo
  aprobado por Security/Endpoint Management y se distribuye por el canal
  empresarial.
- El equipo se registra automaticamente durante instalacion/provisionamiento.
- La perdida de IA no bloquea catalogo, instalaciones autorizadas ni tickets.
- Una instalacion descargada y en ejecucion sobrevive caidas de red/backend.
- Ningun software del Excel se publica sin compatibilidad, EOL y paquete validados.

## Paridad visual

Se conserva:

- Fluent, Mica, radios, profundidad, iconografia y ritmo del mockup;
- jerarquia de dashboard, catalogo, chat, salud, tickets y admin;
- tema claro/oscuro e idioma;
- marca e identidad aprobadas.

Se elimina:

- escritorio Windows simulado;
- taskbar simulada;
- controles de ventana dibujados manualmente;
- iframes a demos locales;
- selector de sesion falso;
- marca de agua de prototipo en builds productivos.
