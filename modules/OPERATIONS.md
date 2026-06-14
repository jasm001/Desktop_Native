# Operacion del MVP

## Estado

Documento transversal de referencia; no es un bloque activo. Durante el
Bloque 10 aporta requisitos operativos de despliegue, continuidad,
deshabilitacion y retiro. El Bloque 9 permanece bloqueado por su integracion
externa. Este documento no autoriza OpenText, UEMS, Rescue, Power Automate ni
operaciones productivas reales.

## Topologia recomendada

- Portal, API y worker en Azure.
- PostgreSQL administrado en Azure con acceso privado.
- Para software cubierto por templates oficiales, UEMS descarga desde el
  fabricante, crea el paquete y lo conserva en su Software Repository HTTP/WAN.
- Blob privado o repositorio nuevo se usa solo para software aprobado no cubierto,
  layouts offline o versiones fijadas.
- Los paquetes historicos locales no se reutilizan sin revalidacion.
- VM separada para gateway/modelo de IA si se elige autocontenido.
- OpenText y Rescue conectados por adaptadores.
- Equipos se comunican hacia Azure por HTTPS saliente; sin puertos entrantes.

La API de usuario/agente puede tener endpoint publico protegido por WAF, tokens,
identidad del dispositivo, rate limits y autorizacion. Base de datos, secretos y
administracion permanecen privados. Exponer una URL HTTPS no equivale a exponer
la base de datos ni aceptar operaciones anonimas.

Un endpoint completamente privado exige VPN/LAN para cada uso y bloquearia a
usuarios remotos justo cuando necesitan soporte. Debe decidirse por flujo, no
aplicar privacidad de red indiscriminadamente.

## VM frente a PaaS

Azure App Service reduce operacion para API/portal. Una VM requiere hardening,
parches, EDR, backups, proxy, certificados y monitoreo. Usar VM cuando:

- el modelo de IA necesita GPU/runtime persistente;
- OpenText requiere un conector dentro de red;
- una politica corporativa impide PaaS;
- existe una dependencia Windows/IIS no soportada.

No alojar modelo de IA y API privilegiada en el mismo proceso o identidad.

## Registro automatico

El paquete de despliegue recibe configuracion no secreta:

- environment/tenant;
- sede o grupo de despliegue;
- URL de bootstrap;
- identificador de campaña.

Primer inicio:

1. genera identidad local protegida;
2. canjea token bootstrap de un solo uso;
3. registra hostname, device ID y postura minima;
4. recibe certificado/token rotativo y politica;
5. marca instalacion y version en auditoria.

El usuario no configura servidor, credenciales ni sede manualmente.

UEMS puede distribuir cliente, servicio y configuracion inicial usando privilegios
de sistema. El servicio se crea con identidad exclusiva, restringida y sin logon
interactivo. El producto no necesita una cuenta administrativa compartida.

El proyecto no audita ni modifica la configuracion interna de UEMS. El owner de
Endpoint Central recibe nuestros artefactos/requisitos y controla el despliegue,
actualizacion y rollback en la sede piloto.

### Perfiles posteriores de despliegue

La operacion futura admite dos ramas sin cambiar el dominio:

- empresarial: la ISO/UEMS corporativa reinstala Windows, UEMS, Sophos y un
  bootstrap firmado del agente;
- independiente: el usuario instala Windows 11 limpio, entra a un portal
  autenticado y descarga un bootstrap firmado y de un solo uso asociado a su
  perfil.

En ambas ramas el instalador contiene solo configuracion publica minima. El
perfil, aplicaciones y politicas se descargan despues de validar usuario,
dispositivo, expiracion y revocacion.

## Offline y continuidad

- Sin IA: catalogo y acciones fijas siguen disponibles.
- Sin backend antes de autorizar: no se inicia una accion nueva.
- Artefacto descargado + trabajo autorizado: el agente puede completar.
- Sin red durante ejecucion: estados/evidencia quedan en SQLite local.
- Al reconectar: sincronizacion idempotente y cierre de ticket.
- Reinicio: el agente recupera la maquina de estados.
- Las reparaciones offline proceden unicamente de bundles firmados y allowlisted.
- La base local de conocimiento explica resultados, pero no genera scripts.

En `local-demo`, "sin conexion" significa que no existe acceso al backend,
mirror o API de IA. El agente puede:

- obtener diagnosticos locales;
- mostrar catalogo y conocimiento cacheados;
- continuar un trabajo previamente autorizado cuyo artefacto ya esta validado;
- ejecutar una accion incluida en la politica local de desarrollo, con
  confirmacion explicita, solo dentro de la VM;
- conservar progreso y evidencia en SQLite;
- sincronizar idempotentemente cuando regresa la conexion.

No puede descargar un artefacto ausente, inventar una autorizacion, ampliar la
allowlist ni generar comandos desde conocimiento local.

## Mantenimiento y experiencia del usuario

- Agrupar instalaciones, politicas y actualizaciones compatibles para reducir
  reinicios.
- Ejecutar mantenimiento pesado solo en una ventana aprobada, con alimentacion
  y condiciones suficientes.
- Informar antes de actuar y permitir posposicion dentro de la politica.
- Para una actualizacion critica, mostrar fecha limite, motivo, duracion,
  cuenta regresiva y aviso para guardar trabajo.
- Si el arranque requiere PIN de BitLocker, avisar que el usuario debe estar
  presente; el producto no conoce el PIN.
- Verificar el resultado despues del reinicio y conservar evidencia saneada.

## Borrado y reprovisionamiento posterior

El borrado se implementa al final del roadmap. El agente prepara y correlaciona
el proceso, pero no puede sobrevivir a la reinstalacion. Una ISO/UEMS,
Autopilot/provisioning package o bootstrap descargado debe reinstalarlo.

Antes de iniciar:

- mostrar impacto y datos que se perderan;
- exigir confirmacion de respaldo completado por el usuario en OneDrive u otro
  canal aprobado;
- guardar fuera del endpoint hostname, asset tag, perfil y aplicaciones
  elegidas;
- comprobar energia, conectividad, autorizacion, revocacion y mecanismo de
  recuperacion;
- crear ticket si BitLocker o enrolamiento de dominio requiere intervencion
  humana.

Despues de reinstalar, el agente valida identidad, recupera el perfil, verifica
agentes de seguridad y prepara las aplicaciones seleccionadas cuando el usuario
inicia sesion.

No se implementa persistencia propia en UEFI/kernel, autologon con credenciales
humanas ni calculo de PIN desde el asset tag.

La especificacion vive en `../docs/modules/device-reprovisioning.md`.

## Red corporativa y politicas

- Detectar red de oficina o VPN antes de acciones de dominio.
- Si se requiere `gpupdate`, ejecutar adaptador cerrado con timeout.
- Informar cuando una politica requiere logoff o reboot.
- No encender una VPN con credenciales guardadas del usuario.
- Ofrecer instruccion o deep link al cliente VPN aprobado.
- Permitir que el trabajo espere hasta que el controlador de dominio sea
  alcanzable.
- El perfil FortiClient conocido para el MVP requiere identidad de usuario y
  queda fuera de automatizacion.
- Axis Atmos puede evaluarse como prerrequisito del piloto, pero el agente no
  inicia sesion en nombre del usuario.
- Para el MVP, si no hay red corporativa ya activa, `gpupdate` se escala.
- En oficinas puede ejecutarse el adaptador de politica si el controlador de
  dominio es alcanzable y el usuario acepta logoff/reinicio.

### Mejora posterior: campanas de refresco de politicas

Despues del MVP se puede evaluar un flujo administrado para sustituir el
procedimiento manual del tecnico sin almacenar sus credenciales:

1. el owner de AD/UEMS publica una campana explicita en el backend;
2. el DeviceAgent consulta por HTTPS saliente con un intervalo inicial de una
   hora y jitter para evitar consultas simultaneas;
3. la campana identifica alcance, vigencia y una accion tipada, no comandos;
4. WinUI informa al usuario y permite aceptar o posponer;
5. el equipo obtiene conectividad mediante un mecanismo corporativo aprobado de
   maquina, pre-logon o acceso privado;
6. el agente valida DNS corporativo, controlador de dominio y acceso requerido
   a `SYSVOL`/`NETLOGON`;
7. el agente ejecuta `group-policy.refresh.computer.v1` con timeout y evidencia
   saneada;
8. logoff y reinicio requieren una segunda confirmacion y una ventana segura;
9. tras reiniciar, el agente verifica el resultado y reporta la correlacion de
   la campana.

El sondeo no usa ping para detectar cambios de GPO. Una respuesta ICMP solo
demuestra alcance parcial y AD no envia al endpoint una notificacion generica de
que existe una politica nueva. La fuente de verdad para este flujo es la campana
administrativa explicita.

Esta mejora no usa una cuenta de tecnico, una sesion oculta ni credenciales VPN
persistidas. Si no existe conectividad de maquina aprobada, conserva el
comportamiento del MVP: orientar o escalar.

La especificacion tecnica, gates y evidencia pendiente viven en
`../docs/modules/domain-policy-refresh.md`.

## Identidad y contrasenas

- El asistente no pide, almacena ni transmite contrasenas.
- Con sesion iniciada, puede abrir `recovery.softtek.com`, iniciar la
  verificacion oficial, revisar conectividad y guiar al usuario.
- El login y MFA se muestran en UI controlada por Microsoft; el cliente no
  captura la ultima contrasena.
- Una sesion activa de Windows/Teams no se considera prueba suficiente para
  autorizar un reset.
- Sin acceso a Windows, se evalua si `recovery.softtek.com` puede abrirse desde
  la pantalla de inicio mediante una capacidad soportada.
- Confirmar Entra joined/hybrid joined, tecnologia del portal, URLs/proxy y
  password writeback.
- En hybrid join, la nueva contrasena requiere linea de vista al controlador de
  dominio para actualizar credenciales almacenadas.
- Si Teams usa la misma identidad bloqueada, se necesita un canal alterno de
  soporte.
- Si no existe correo personal enrolado, WinUI crea un ticket de recuperacion y
  el grupo autorizado conserva la responsabilidad del reset.
- Power Automate puede coordinar aprobacion/notificaciones, pero no transportar
  la contrasena temporal.
- Informar que el cambio dentro de 48-72 horas es una instruccion operativa y
  que deben evitarse cambios consecutivos durante las primeras 24 horas.
- Registrar que la instruccion fue mostrada, sin seguimiento posterior.
- No cerrar, bloquear ni volver a contactar automaticamente al cumplirse 72
  horas. Si Identity robustece la politica en el futuro, se integra como una
  regla nueva.
- Fuera de LAN/VPN, Windows puede aceptar credenciales almacenadas aunque la
  contrasena este expirada; eso permite continuidad temporal, no correccion.

## Teams y Power Automate

- Teams funciona como canal conversacional remoto del mismo backend.
- Power Automate puede manejar mensajes, aprobaciones, recordatorios y tickets.
- Ningun flow autoriza por si mismo una instalacion.
- Power Automate Desktop no es el ejecutor principal del endpoint.
- Cualquier solicitud de Teams/Power Automate vuelve a pasar por politicas,
  vinculacion de dispositivo, idempotencia y auditoria del backend.

## Soporte remoto

La integracion API de GoTo/LogMeIn Rescue se difiere. En el MVP:

- el bot escala a OpenText;
- el agente humano revisa el ticket;
- contacta al usuario por Teams;
- comparte la URL de Rescue con el flujo actual;
- documenta el resultado en OpenText.

No usar la cuenta del servicio del agente para dar escritorio remoto full. La
elevacion se hace con el mecanismo aprobado de Rescue o credencial administrativa
nominal/JIT, nunca revelada al usuario ni almacenada por la app.

Construir un reemplazo open source de Rescue no pertenece al MVP: introduce
streaming, relay/NAT, cifrado, grabacion, consentimiento, elevacion y una
superficie de ataque critica.

## OpenText Service Manager

- El ticket debe quedar a nombre del usuario solicitante.
- `Requested by` usa la identidad tecnica nominal del bot.
- `Open this ticket on behalf` usa al usuario del equipo.
- `Preferred method of contact` usa `Chat`.
- Mantener `externalTicketId` y correlacion interna.
- Usar outbox y reintentos para no perder tickets.
- Solicitar al equipo actual del bot de Teams endpoint, autenticacion, campos,
  assignment groups, rate limits y ambiente de prueba.

La captura corresponde a Service Manager; no asumir API o version de SMAX hasta
que OpenText/administradores confirmen producto y version exactos.

La taxonomia y reglas operativas observadas se detallan en `TICKETING.md`.

## IA y conocimiento

- Gateway central aplica prompts, politicas, limites y observabilidad.
- Hermes puede correr en una VM/equipo local y usar MiniMax/OpenRouter mediante
  API. En ese modo el agente es local, pero el modelo y los datos procesados son
  remotos. Self-hosting real requeriria pesos, licencia y capacidad de inferencia.
- Azure OpenAI ofrece consumo bajo demanda por tokens y capacidad provisionada.
- Comparar costo de tokens contra VM/GPU, operacion, disponibilidad y licencias.
- Soluciones humanas se redactan, revisan, prueban y publican como conocimiento.
- No hacer fine-tuning continuo con tickets sin clasificacion y consentimiento.

Para el MVP local:

- Hermes se ejecuta localmente como adaptador de conversacion;
- la inferencia puede consumir una API externa con clave personal de prueba;
- solo se envian datos publicos o sinteticos;
- documentos, chunks, metadatos e indice RAG permanecen locales;
- cada respuesta recuperada conserva fuente y version;
- si la API falla, WinUI vuelve a busqueda local, respuestas curadas y la
  maquina de estados determinista;
- el proveedor de IA propone intenciones o acciones conocidas, pero nunca
  autoriza ni produce contenido ejecutable.

Esta configuracion no es inferencia offline. Un modelo con pesos locales seria
otro proveedor y requeriria evaluacion separada de hardware, licencia y
seguridad.
