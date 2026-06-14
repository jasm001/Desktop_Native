# Solicitudes de informacion y accesos

Este documento separa lo minimo para validar el MVP de los controles que se
solicitaran cuando el proyecto pase a piloto formal o produccion. No solicitar
secretos por correo.

## Paquete minimo para desbloquear el Bloque 10

Entregar evidencia saneada, no secretos ni configuracion productiva completa:

- procedimiento UEMS para desplegar, actualizar, deshabilitar, hacer rollback y
  retirar en dos endpoints autorizados;
- matriz de la cuenta restringida del agente y ausencia de logon interactivo;
- identidad de usuario y dispositivo, con bootstrap, rotacion y revocacion;
- revision Security/Sophos del threat model, privilegios, datos, artefactos y
  respuesta ante compromiso;
- owner operativo y mecanismo protegido del kill switch;
- inventario de logs, acceso, retencion, alertas y respuesta;
- paquete aprobado y mecanismo de firma o confianza de publicador;
- endpoints, ventana, responsables, criterio de restauracion y resultados del
  ensayo.

Hasta recibir este paquete, el Bloque 10 permanece `blocked` y `local-demo`
permanece limitado a `Development`.

## Nivel 1: MVP de viabilidad

## Lider de Mesa de Ayuda

Solicitar:

- autorizacion para desarrollar y demostrar el MVP;
- sede candidata y grupo aproximado de usuarios;
- contacto funcional para validar flujos;
- permiso para usar dos equipos de prueba;
- criterios de exito: instalaciones, escalaciones y tiempo ahorrado;
- presupuesto o recursos disponibles, si existen.

Entregable: aprobacion inicial, contacto y alcance de la prueba.

## UEMS Endpoint Central

Solicitar al owner:

- confirmacion de Software Repository HTTP/WAN;
- ejecucion como System, parametros, deteccion, retry y rollback;
- distribucion de certificados/configuracion;
- anillos/grupos para una sede piloto;
- logs y reporte de despliegue;
- requisitos para publicar cliente, agente y actualizaciones;
- contacto y proceso para solicitar despliegue a usuarios piloto.

Entregable: procedimiento para empaquetar, desplegar, actualizar y retirar
nuestra aplicacion.

## Security, Compliance y Sophos

Contactos:

- `security@softtek.com`;
- `IT.SecandComp@softtek.com`.

Solicitar:

- confirmar que el flujo de `../reference/templates/NO_LISTED.md` continua
  vigente para software no listado, EOL o prohibido;
- revisar el resumen de arquitectura y responder especificamente:
  - si aceptan un Windows Service restringido que instala solo paquetes
    allowlisted;
  - si puede ejecutarse como System/cuenta de servicio exclusiva;
  - que datos no pueden salir a proveedores de IA;
  - que logs/evidencia puede conservar el portal;
  - si requieren firma o excepcion Sophos para el binario piloto;
  - que acciones quedan prohibidas aun con consentimiento;
- indicar el proceso de excepcion Sophos si aparece un bloqueo;
- aprobar o rechazar Vercel/Supabase con datos sinteticos.

Esta es la revision minima del threat model: validar fronteras de confianza,
privilegios, datos enviados, acciones permitidas, actualizaciones y respuesta
ante compromiso. No se les pide disenar el producto.

Entregable: restricciones concretas y autorizacion/rechazo de la prueba.

## Digital Office y OpenText

Solicitar:

- contacto tecnico de `help.softtek.com`;
- ambiente o mecanismo de prueba;
- API/metodo soportado para crear tickets;
- autenticacion tecnica del bot;
- IDs/campos de `IT Support > Desktop Support > Software`;
- posibilidad de crear grupo/cuenta tecnica del bot;
- si se permite asignar/cerrar tickets propios por API;
- si el portal permite iframe; si no, se usara enlace externo.

Para evitar tickets duplicados o perdidos:

- si aceptan un `clientRequestId`/campo externo unico;
- respuesta al reintentar la misma solicitud;
- limites por minuto/hora y respuesta al excederlos;
- timeouts y ventanas de mantenimiento;
- si ofrecen webhooks/eventos o debe hacerse polling;
- autenticacion, firma y reintentos de webhooks;
- estados/transiciones permitidos por API.

Entregable: prueba no productiva de crear, consultar y cerrar un ticket, o
confirmacion del alcance disponible.

## Login Microsoft

Estas preguntas estan redactadas para que una persona no tecnica pueda
trasladarlas. Los nombres entre parentesis ayudan a identificar la configuracion
con otro equipo:

- Podemos registrar el portal para que los usuarios entren con su correo
  `@softtek.com` y el MFA habitual? El equipo tecnico puede identificarlo como
  `App Registration OIDC` y proporcionar tenant ID, client ID y URL de retorno.
- El portal `recovery.softtek.com` usa Microsoft Self-Service Password Reset
  (`SSPR`) u otra tecnologia?
- Para `softtek.com`, el cambio del portal se refleja en correo y equipo. Esta
  sincronizacion esta implementada como `password writeback` u otro mecanismo?
- Los equipos se registran directamente con Microsoft Entra, combinan Entra y
  dominio local, o dependen solo del dominio? Los nombres tecnicos son Entra
  joined, hybrid joined y domain joined.
- Puede abrirse `recovery.softtek.com` de forma soportada desde la pantalla de
  inicio de Windows?
- La red, proxy o firewall permiten abrir las paginas de recuperacion desde la
  pantalla de inicio?
- Existe una API oficial para solicitar o ejecutar ese restablecimiento, o todo
  se realiza manualmente?
- Cual es el canal corporativo oficial para alguien sin acceso simultaneo a
  Windows, correo y Teams?

La aplicacion no recibe ni almacena la contraseña. Microsoft muestra su pantalla
de login, valida correo, password y MFA, y devuelve tokens.

Durante el MVP, altas/bajas y roles del panel se gestionan manualmente en nuestra
base de datos. Grupos Entra y aprovisionamiento automatico quedan para despues.

Entregable: inicio de sesion corporativo funcional.

Para recuperacion, entregable adicional: matriz de casos de
`recovery.softtek.com` y prueba con un usuario no administrativo/equipo de
laboratorio. El producto no solicita acceso para leer o establecer contraseñas.

Para el MVP, solicitar permiso para automatizar alta, seguimiento y cierre del
ticket de recuperacion. La ejecucion del reset permanece humana hasta que
Identity/Security aprueben API, permisos, step-up, segregacion y entrega segura.

Ya se confirmo:

- `recovery.softtek.com` cambia la contrasena mediante correo personal
  registrado, codigo y CAPTCHA;
- esta disponible para cuentas corporativas con medio personal enrolado;
- `softtek.com` refleja el cambio en correo y equipo;
- `softtekprojects.com` usa una contrasena de dominio separada;
- lideres autorizados de sede realizan el reset manual cuando no hay
  autoservicio;
- la sesion de Teams suele permanecer abierta hasta que cambia la contrasena;
- la credencial temporal no se agrega al ticket.

El manual `mfa reset 1.pdf` confirma el registro de SMS, llamada, push y codigo
de Microsoft Authenticator. No identifica si el portal usa SSPR ni si existe
password writeback.

## Teams y Power Platform

Solicitar:

- owner y contacto tecnico del bot personalizado que ya existe en Teams;
- plataforma actual del bot: Teams SDK, Copilot Studio u otra;
- licencias disponibles de Power Automate y Copilot Studio;
- disponibilidad de Power Automate Process para RPA desatendido;
- ambientes, DLP, conectores y cuentas de servicio permitidas;
- si Power Automate puede crear tickets mediante API/conector OpenText;
- si el bot puede consumir un backend propio y consultar catalogo/tickets de
  otros canales;
- limites, owners, soporte y auditoria de flows.

Entregable: prototipo que consulta catalogo sintetico y crea una solicitud, mas
confirmacion escrita de que Power Automate no sera el ejecutor privilegiado del
endpoint salvo una evaluacion tecnica y de seguridad posterior.

## Windows Endpoint y agente

Solicitar:

- permiso para crear una cuenta virtual/de servicio exclusiva;
- privilegios minimos necesarios, incluyendo instalacion elevada;
- confirmacion de que no puede iniciar sesion interactivamente;
- distribucion del servicio mediante UEMS;
- regla de reinicio/logoff con consentimiento;
- proceso para retirar/deshabilitar el agente;
- excepcion Sophos solo si aparece un bloqueo real.

No se modifica ni reemplaza `.\itlocal`; permanece reservado a agentes humanos.

Entregable: matriz breve de permisos y prueba en dos endpoints.

## Network, VPN y Active Directory

Nosotros entregamos la lista de destinos necesarios:

- portal/API;
- autenticacion Microsoft;
- OpenText;
- UEMS;
- proveedor de IA aprobado, si aplica.

Solicitar:

- salida HTTPS hacia esos destinos;
- permiso/cuenta de servicio mediante aprobacion del lider;
- confirmar que `gpupdate` solo se ejecuta con LAN/VPN activa;
- flujo de escalamiento fuera de oficina sin VPN.

No se solicita automatizar FortiClient durante el MVP.

Entregable: conectividad permitida y ruta de escalamiento.

## Hosting y datos

Solicitar:

- si proporcionaran Azure para web/API/PostgreSQL;
- presupuesto mensual;
- ambientes disponibles;
- reglas de datos para desarrollo/prueba.

Sin recursos corporativos inicialmente:

- Vercel/Supabase solo con datos sinteticos y adaptadores fake;
- desarrollo local en equipos asignados;
- sin credenciales OpenText, agentes ni tickets reales.

No solicitar Blob para instaladores en el MVP. UEMS y URLs oficiales son la ruta
inicial. Para excepciones sin template:

1. UEMS crea paquete desde URL oficial;
2. el usuario descarga desde URL oficial y el agente valida/ejecuta;
3. se escala si ninguna opcion es aprobada.

Un mirror local usado dentro de una VM personal para software libre
redistribuible es una herramienta de laboratorio, no una solicitud de
infraestructura ni un repositorio corporativo. No cambia este entregable.

Entregable: perfil de hosting permitido y presupuesto.

## IA

Solicitar:

- si se permite Hermes local consumiendo OpenRouter;
- si puede usarse WSL2 en el equipo de desarrollo;
- proveedores/modelos autorizados;
- si se exige `data_collection: deny` y Zero Data Retention;
- categorias de datos permitidas;
- piloto de USD 50 mensuales durante dos meses;
- tarjeta/cuenta corporativa, owner y centro de costo;
- alternativa corporativa por API, si existe.

OpenRouter permite filtrar proveedores por politica de datos, pero prompts siguen
siendo procesados por OpenRouter y el proveedor seleccionado. Deben aprobarse
ambas politicas y la configuracion ZDR.

El RAG inicial puede usar notas Markdown/Obsidian curadas, puntuales y sin PII.
Obsidian es la herramienta de autor; el backend indexa una copia versionada en un
vector store aprobado.

Entregable: proveedor/modelo permitido, reglas de datos y limite de gasto.

## GitHub y desarrollo

Solicitar:

- organizacion/repositorio privado;
- acceso para desarrolladores;
- GitHub Actions/runners;
- branch protection y revisores;
- si GitHub Code Security esta incluido;
- politica para Codex y herramientas externas;
- recotizacion posterior a junio de 2026 para Visual Studio, GitHub Enterprise
  y Copilot;
- creditos de IA incluidos en Copilot y costo de excedentes;
- condiciones exactas del asiento y limite mensual de Claude Enterprise.

Sin Code Security:

- Semgrep CE para SAST;
- Gitleaks para secretos;
- Trivy para dependencias, filesystem, IaC y SBOM;
- OWASP ZAP para el portal;
- analyzers .NET, ESLint y auditorias de paquetes.

CodeQL en VS Code sirve como complemento local para consultas sobre bases CodeQL;
no sustituye el escaneo CI de un repositorio privado.

Claude Enterprise y su limite de USD 50 mensuales se evalua como herramienta
complementaria, no como dependencia del producto ni unico medio de desarrollo.

Entregable: repositorio, reglas de contribucion y herramientas permitidas.

## Catalogo

No reconfirmar el Excel: version 134 es la fuente vigente.

- Permitido: evaluar automatizacion con UEMS/URL oficial.
- No listado: escalar con el template existente.
- EOL: rechazar o pedir excepcion mediante el template.
- Prohibido: ofrecer alternativas y escalar si son insuficientes.
- Validar EOL solo contra la pagina oficial del fabricante.

La lista por pais/cliente/proyecto se integra posteriormente si puede exponerse
en formato de solo lectura.

## Nivel 2: despues de aprobar el MVP

Solicitar entonces:

- threat model formal y sign-off;
- PKI/firma y cadena de actualizacion;
- Azure, PostgreSQL, Key Vault y observabilidad;
- retencion formal por clase de dato;
- roles Entra mediante grupos/provisionamiento;
- cuentas break-glass;
- API completa/webhooks OpenText;
- integracion Rescue;
- mirror propio para excepciones;
- RPO/RTO, backups, WAF y private endpoints;
- CodeQL/GitHub Code Security si el presupuesto lo permite;
- SBOM, SCA y aprobaciones de release;
- proceso corporativo para paquetes reempaquetados;
- expansion a otras sedes/paises.
