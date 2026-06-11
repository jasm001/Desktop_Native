# Alcance

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

## Fase 2

- Integracion bidireccional completa con OpenText: actividades, asignacion,
  estados, soluciones, SLA y cierre.
- Intune/Graph para inventario y despliegues administrados.
- Integracion profunda con GoTo/LogMeIn Rescue.
- Anillos de despliegue, mantenimiento y rollback.
- Base de conocimiento empresarial con RAG y permisos por documento.

## Fase 3

- Remediaciones adicionales aprobadas y versionadas.
- Campanas de cumplimiento.
- Reportes ejecutivos y SLO.
- ARM64.
- Multitenancy solo si el producto pasa de interno a comercial.

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
