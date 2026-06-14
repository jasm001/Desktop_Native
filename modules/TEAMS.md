# Canal Teams

## Estado

Documento propietario del Bloque 9 `pending`.

Los Bloques 0 a 8 estan `completed`. No hay bloque principal activo y el
Bloque 9 es la siguiente unidad desbloqueada.

## Hechos confirmados

- Ya existe un bot corporativo de Teams.
- El bot actual crea tickets y consulta los generados desde ese mismo canal.
- Debe ampliarse el bot existente; no se crea un segundo bot.
- Teams y WinUI son canales del mismo asistente.
- Ambos canales consumen la API compartida y las mismas reglas de catalogo,
  propuesta, confirmacion, solicitud, estado y escalamiento.
- Teams nunca ejecuta comandos ni se conecta directamente al DeviceAgent.

## Pendientes externos

Todavia no se conocen ni estan disponibles en este repositorio:

- owner y contacto tecnico del bot;
- plataforma actual: Teams SDK, Copilot Studio u otra;
- repositorio, runtime y proceso de despliegue del bot;
- autenticacion del bot frente al control plane;
- tenant, App Registration, permisos, secretos o workload identity;
- ambientes, DLP, limites, soporte y auditoria;
- mecanismo aprobado para Adaptive Cards o acciones equivalentes;
- capacidad confirmada para consumir la API compartida.

Estos pendientes bloquean la integracion corporativa real, pero no una primera
unidad local de contratos, adaptador fake y pruebas de paridad.
El stopper no bloqueante correspondiente esta registrado en `../WORKFLOW.md`.

## Frontera del canal

`IConversationChannel` representa la traduccion entre una plataforma
conversacional y los contratos del control plane. No contiene reglas de negocio.

Responsabilidades:

- normalizar identidad y contexto minimo autorizado;
- traducir mensajes o acciones conocidas a comandos tipados;
- presentar respuestas, propuestas, confirmaciones y estados;
- propagar correlacion e idempotencia;
- rechazar acciones, payloads o versiones desconocidas;
- mantener payloads saneados y acotados.

No puede:

- decidir autorizacion, licencia o politica;
- mantener un catalogo separado;
- construir comandos de agente;
- llamar al DeviceAgent;
- crear solicitudes sin confirmacion explicita;
- transportar credenciales, contrasenas, tokens o logs completos;
- tratar una sesion abierta de Teams como autorizacion suficiente para acciones
  sensibles o recuperacion de identidad.

## Primer incremento local esperado

Antes de conectar el bot corporativo:

1. auditar los contratos HTTP existentes y el flujo WinUI;
2. definir un contrato versionado de entrada/salida del canal;
3. implementar una frontera `IConversationChannel` y un adaptador local fake o
   recorded, sin Microsoft 365 ni red externa;
4. reutilizar la API compartida para catalogo, confirmacion, solicitud, estado y
   caso;
5. demostrar con fixtures comunes que Teams y WinUI producen las mismas
   decisiones para la misma entrada;
6. probar que consultas no mutan y reintentos confirmados no duplican;
7. documentar el mecanismo de sustitucion por el adaptador corporativo futuro.

La ubicacion concreta del adaptador debe decidirse despues de inspeccionar las
fronteras actuales. No se crea un backend adicional ni se duplican reglas entre
C# y TypeScript.

## Gate del Bloque 9

El bloque solo puede declararse `completed` cuando:

- el bot existente consume la API compartida mediante el mecanismo aprobado;
- identidad, autenticacion, permisos y despliegue estan documentados;
- Teams y WinUI pasan pruebas de paridad;
- consulta, confirmacion, solicitud, estado y escalamiento funcionan;
- no hay comandos privilegiados, reglas duplicadas ni secretos en el canal;
- retries, correlacion, idempotencia y auditoria estan validados;
- los gates corporativos aplicables tienen evidencia.

Hasta disponer del bot y tenant reales, el Bloque 9 puede quedar `in_progress`
con su incremento local validado, pero no `completed`.

## Referencias

- `../core/ARCHITECTURE.md`
- `../core/SECURITY.md`
- `../core/STACK.md`
- `ASSISTANT.md`
- `../context/IDENTITY_AND_TEAMS_RESPONSES.md`
- `../project-management/INFORMATION_REQUESTS.md`
