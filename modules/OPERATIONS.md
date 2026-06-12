# Operacion del MVP

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

## Offline y continuidad

- Sin IA: catalogo y acciones fijas siguen disponibles.
- Sin backend antes de autorizar: no se inicia una accion nueva.
- Artefacto descargado + trabajo autorizado: el agente puede completar.
- Sin red durante ejecucion: estados/evidencia quedan en SQLite local.
- Al reconectar: sincronizacion idempotente y cierre de ticket.
- Reinicio: el agente recupera la maquina de estados.

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
