# Conversacion determinista

## Responsabilidad

`src/Conversation` coordina intenciones tipadas contra
`CatalogDecisionService`. Su salida es un nuevo snapshot de sesion y un codigo
de resultado; no realiza I/O.

## Modelo

- `ConversationSession`: estado, propuesta pendiente, solicitud sintetica y
  comandos procesados.
- `ConversationCommand`: identificador, intencion y referencia opcional de
  producto.
- `PendingConversationRequest`: producto, version y tipo de solicitud.
- `SyntheticRequest`: resultado local con referencia e idempotency key.
- `ConversationTurn`: sesion resultante, codigo, decision de catalogo y banderas
  de transicion/deduplicacion.

Las colecciones expuestas son de solo lectura y cada transicion crea una nueva
sesion.

## Reglas

- `QueryCatalog` siempre usa el proposito informativo del catalogo.
- `RequestSoftware` solo crea una propuesta para decisiones `Propose` o
  `Escalate`.
- `ContinueProposal` es obligatorio antes de `Confirm`.
- `Confirm` solo funciona desde `ConfirmationRequired`.
- `Cancel` solo funciona desde `Proposal` o `ConfirmationRequired`.
- `RequestCreated` y `Cancelled` son terminales.
- comandos duplicados e invalidos no crean una segunda solicitud.

## Fuera de alcance

- clasificacion de texto libre o LLM;
- persistencia durable;
- solicitudes corporativas, BotCase o tickets;
- trabajos, IPC o DeviceAgent;
- autenticacion, autorizacion o ejecucion.

El adaptador WinUI del Bloque 7 puede persistir una solicitud sintetica
confirmada sin cambiar este dominio puro. El siguiente Bloque 8 agregara casos y
ticketing fake fuera de `src/Conversation`.

## Pruebas

`tests/Unit/ConversationServiceTests.cs` cubre todos los estados, decisiones de
catalogo, confirmacion explicita, cancelacion, transiciones invalidas,
reintentos e idempotency keys.

`tests/WindowsUi/AssistantViewModelTests.cs` verifica que el adaptador muestra
consulta, confirmacion y rechazo sin crear reglas propias.

`tests/Architecture` impide que el assembly de conversacion dependa de Desktop,
DeviceAgent o WinUI.
