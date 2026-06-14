# Canal Teams

## Estado

Documento propietario del Bloque 9 `blocked`.

Los Bloques 0 a 8 estan `completed`. El Bloque 10 es el unico bloque principal
`in_progress`; el Bloque 9 conserva su incremento local y espera evidencia
externa.

El primer incremento local fue implementado y validado el 2026-06-14. Su
evidencia tecnica vive en
`../docs/modules/teams-channel-local-increment.md`.

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

Estos pendientes bloquean la integracion corporativa real y el cierre del
Bloque 9. La unidad local de contratos, adaptador recorded y pruebas de paridad
ya fue completada. El stopper correspondiente esta registrado en
`../WORKFLOW.md`.

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

## Primer incremento local implementado

Antes de conectar el bot corporativo se completo:

1. auditoria de contratos HTTP y del flujo WinUI;
2. contrato estricto `conversation-channel.v1` en C# y TypeScript;
3. frontera `IConversationChannel` y
   `RecordedTeamsConversationChannel`, sin red ni formato Microsoft supuesto;
4. `ConversationChannelService` sobre la maquina y reglas existentes;
5. propagacion de correlacion, dispositivo e idempotencia al control plane;
6. consulta tipada de solicitud y `BotCase`;
7. fixtures comunes y paridad observable Teams/WinUI;
8. rechazo de versiones, acciones, campos y contenido ejecutable desconocidos;
9. documentacion del punto de sustitucion por el adaptador corporativo.

La API existente no ofrece una mutacion durable para `HumanReview`. El canal
exige confirmacion y devuelve `capability_unavailable` sin crear una instalacion
ni fingir un escalamiento. Esta capacidad debe agregarse como caso de uso del
control plane en una unidad posterior, no como regla del adaptador.

No se creo un backend adicional ni se duplicaron reglas entre C# y TypeScript.

## Gate del Bloque 9

El bloque solo puede declararse `completed` cuando:

- el bot existente consume la API compartida mediante el mecanismo aprobado;
- identidad, autenticacion, permisos y despliegue estan documentados;
- Teams y WinUI pasan pruebas de paridad;
- consulta, confirmacion, solicitud, estado y escalamiento funcionan;
- no hay comandos privilegiados, reglas duplicadas ni secretos en el canal;
- retries, correlacion, idempotencia y auditoria estan validados;
- los gates corporativos aplicables tienen evidencia.

Hasta disponer del bot y tenant reales, el Bloque 9 queda `blocked` con su
incremento local validado y no puede declararse `completed`.

## Referencias

- `../core/ARCHITECTURE.md`
- `../core/SECURITY.md`
- `../core/STACK.md`
- `ASSISTANT.md`
- `../context/IDENTITY_AND_TEAMS_RESPONSES.md`
- `../project-management/INFORMATION_REQUESTS.md`
