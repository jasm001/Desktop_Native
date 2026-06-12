# Gobierno del asistente

## Principio

El asistente organiza una conversacion tipada sobre casos de uso del producto.
No autoriza ni ejecuta acciones, y una consulta nunca crea solicitud, ticket,
trabajo o instalacion.

WinUI y Teams deben consumir la misma maquina de estados y las mismas decisiones
de catalogo. Ningun canal mantiene reglas propias.

## Implementacion local del Bloque 3

`src/Conversation` contiene una maquina de estados pura, sin dependencias de
WinUI, IA, Prisma, persistencia externa, IPC o DeviceAgent.

Estados implementados:

- `Query`;
- `Proposal`;
- `ConfirmationRequired`;
- `RequestCreated`;
- `Cancelled`.

Intenciones fijas:

- `QueryCatalog`;
- `RequestSoftware`;
- `ContinueProposal`;
- `Confirm`;
- `Cancel`.

La secuencia para una solicitud aprobada es:

```text
Query -> Proposal -> ConfirmationRequired -> RequestCreated
```

Una cancelacion desde propuesta o confirmacion termina en `Cancelled`. Software
EOL o prohibido permanece en `Query` con rechazo y alternativas; no entra al
flujo de confirmacion.

## Solicitud sintetica

`RequestCreated` produce un `SyntheticRequest` dentro del snapshot inmutable de
la sesion. Incluye producto, version, tipo, referencia e idempotency key.

Este registro demuestra la transicion y la deduplicacion, pero no es una
`SupportRequest` durable ni una solicitud corporativa. No crea ticket, trabajo,
instalacion o evidencia operativa. La persistencia PostgreSQL y la solicitud
real pertenecen al Bloque 7.

## Idempotencia

Cada comando tiene un identificador obligatorio:

- repetir el mismo `commandId` devuelve `DuplicateCommand` y conserva la misma
  sesion;
- confirmar de nuevo con otro identificador despues de `RequestCreated` devuelve
  `InvalidTransition`;
- la referencia e idempotency key sinteticas son estables para la misma
  conversacion, producto, version y tipo.

## Adaptador WinUI

La vista Asistente ofrece opciones fijas para:

- consultar software aprobado;
- solicitar software aprobado;
- solicitar software prohibido;
- continuar, confirmar o cancelar.

`AssistantViewModel` traduce estados y codigos a texto de presentacion y
habilita comandos. No evalua licencia, estado, alternativas ni transiciones.
El campo de texto libre permanece deshabilitado.
