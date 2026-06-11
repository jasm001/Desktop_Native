# Seguridad

## Principios

- Zero trust entre cliente, backend, agente e integraciones.
- Menor privilegio y elevacion limitada por operacion.
- Denegar por defecto.
- Acciones versionadas, firmadas, auditables e idempotentes.
- Secretos fuera de codigo, paquetes y logs.
- La IA no forma parte de la frontera de autorizacion.

## Autenticacion y autorizacion

- Destino productivo: Microsoft Entra ID workforce con Authorization Code + PKCE.
- Piloto: Microsoft Entra corporativo con MFA y usuarios `@softtek.com`.
- Desarrollo: cuenta sembrada con todos los roles solo en entorno local.
- Produccion: roles separados y alcance por sede/area; ningun perfil general.
- Cuentas break-glass se implementan cuando el portal pase de MVP a operacion
  formal.
- Servicios cloud: managed identities u OIDC de workload.
- Agentes: identidad unica por dispositivo, bootstrap de un solo uso, rotacion y
  registro automatico durante despliegue.
- Backend: politicas que combinan rol, tenant, dispositivo, ubicacion, paquete y
  tipo de accion.
- WinUI y Teams autentican al usuario, pero ninguna identidad conversacional
  autoriza por si sola una accion privilegiada.
- Una solicitud remota de Teams debe vincularse a un dispositivo registrado y
  volver a validar politica, riesgo y confirmacion local cuando aplique.

## Ejecucion local

- UI sin privilegios administrativos.
- Servicio separado para operaciones privilegiadas.
- IPC con ACL explicita, identidad del cliente y mensajes con version.
- Allowlist por identificador de accion; nunca comando de texto.
- En MVP, origen oficial/UEMS, version y parametros validados antes de instalar.
- Hash/firma del tercero se registra cuando UEMS/fabricante lo exponga; un mirror
  propio con verificacion estricta queda para despues.
- Timeout, limites de recursos y salida saneada.
- Rollback definido por paquete cuando sea viable.
- El agente no usa una cuenta administrativa humana compartida.
- El agente no usa la cuenta compartida `.\itlocal` ni conoce su contrasena.
- La cuenta exclusiva del agente no permite inicio de sesion interactivo, RDP,
  acceso de red general ni uso por tecnicos.
- Sus permisos se asignan por capability/accion; no es un administrador local
  generico disponible para cualquier proceso.
- El acceso administrativo de tecnicos conserva el mecanismo corporativo actual.
  Cambiar `.\itlocal` o implantar LAPS queda fuera del alcance del proyecto.
- Preferir cuenta virtual local; evaluar gMSA solo para operaciones de dominio y
  en equipos donde Active Directory pueda administrar su identidad.
- El canal de soporte remoto usa identidad nominal del tecnico, consentimiento o
  politica aprobada y auditoria de sesion; la cuenta del agente no concede acceso
  remoto interactivo.

## IA

- Enviar solo contexto necesario.
- No enviar tokens, secretos, logs completos ni documentos sin clasificacion.
- Salida estructurada y validada por esquema.
- Sin herramientas de shell, PowerShell, registro, archivos o instaladores.
- Confirmacion humana para acciones con impacto.
- Funcionalidad principal disponible si el proveedor falla.
- Las soluciones aportadas por agentes humanos pasan por revision y curacion
  antes de entrar a una base de conocimiento/RAG. No se entrena automaticamente
  con conversaciones productivas.
- Hermes local no cambia la clasificacion de datos si llama MiniMax/OpenRouter:
  proveedores externos no reciben prompts/tickets corporativos sin aprobacion
  explicita de AI Governance y Security.
- El asistente no solicita, procesa ni conserva contrasenas de correo, dominio,
  VPN o Windows.
- La recuperacion de identidad delega en `recovery.softtek.com` o en el
  procedimiento humano aprobado. SSPR/password writeback quedan por confirmar.
- MFA/step-up ocurre en el proveedor de identidad; una sesion existente no basta
  para autorizar el reset.
- Las contrasenas temporales no pasan por chatbot, Power Automate, tickets, base
  de datos, logs ni telemetria.
- Una futura API de reset exige permisos minimos, workload identity, aprobacion
  independiente, rate limits, alertas y auditoria reforzada.
- Power Automate puede orquestar solicitudes y aprobaciones, pero sus flows y
  conexiones no reciben credenciales administrativas del agente Windows.

## Datos y auditoria

- TLS para transito y cifrado administrado para almacenamiento.
- PII minimizada; separar identidad de telemetria tecnica cuando sea posible.
- Logs sin prompts completos, tokens ni salida de instaladores por defecto.
- Retencion por clase de dato y requisito legal.
- Audit events append-only con correlacion extremo a extremo.
- Exportaciones administrativas registradas.
- Registro de equipo, instalaciones offline y sincronizacion posterior conservan
  correlacion e idempotencia.
- No recolectar inventario general ya gestionado por otras areas. Limitarse a
  identidad del endpoint, version del agente, postura minima y evidencia de las
  acciones del producto.

## Cadena de suministro

- MVP: solicitar firma/confianza de publicador si Endpoint Management o Sophos lo
  requieren; no comprar certificado publico antes de validar viabilidad.
- Produccion amplia: Azure Artifact Signing o certificado publico cuando el
  presupuesto y la distribucion lo requieran.
- Nunca se distribuye la clave privada de firma a endpoints o administradores.
- Firma de MSIX, ejecutables, scripts y manifiestos.
- SBOM por release.
- Dependencias fijadas y escaneo de vulnerabilidades/licencias.
- Builds reproducibles en runners controlados.
- Ambientes y aprobaciones separados.
- Ningun secreto persistente en estaciones de desarrollo o CI.

## Threat model obligatorio

Antes del piloto, modelar al menos:

- suplantacion de agente o dispositivo;
- elevacion por Named Pipes;
- replay de trabajos;
- paquete sustituido o comprometido;
- administrador interno abusivo;
- fuga de PII por IA o logs;
- integracion de tickets comprometida;
- downgrade del agente;
- perdida de conectividad durante una operacion;
- manipulacion de evidencia.
- abuso del portal por un super admin;
- robo de cuenta de compras/seguridad para aprobar software;
- compromiso del repositorio interno de instaladores;
- agente infectado que intenta reproducir o modificar trabajos;
- persistencia de una exclusion antivirus demasiado amplia.

## Antivirus y exclusiones

No se solicita una exclusion general para la carpeta o proceso del producto.
Cualquier excepcion de antivirus debe:

- ser aprobada por Antivirus Management;
- usar ruta, hash, certificado/editor o comportamiento lo mas especifico posible;
- estar limitada a version, sede y periodo;
- quedar inventariada y revisarse al cambiar el binario.
