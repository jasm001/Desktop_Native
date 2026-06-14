# Arquitectura de tickets y metricas

## Estado

Documento propietario del Bloque 8 `completed` el 2026-06-14.

La implementacion local incluye `BotCase`, transiciones por resultado, consulta
de solo lectura, politica pura de 72 horas, evento tipado de escalamiento,
`ITicketingProvider`, provider fake, `ExternalTicket` y procesamiento
idempotente por worker. La evidencia tecnica vive en
`../docs/modules/case-foundation.md`.

El Bloque 8 usa `ITicketingProvider` fake y datos sinteticos. No conecta
OpenText real, no usa endpoints corporativos y no implementa Teams ni el portal
administrativo.

## Principio

No mezclar dos objetivos distintos:

1. medir y auditar todo lo que el bot intenta/resuelve;
2. pedir atencion humana en OpenText Service Manager.

Toda interaccion operativa crea un `BotCase` interno. Solo algunas crean un
`ExternalTicket` en OpenText.

Durante el Bloque 8, `ExternalTicket` es una representacion sintetica producida
por el provider fake. Las referencias a estados, campos y flujos de OpenText en
este documento definen el contrato futuro; no autorizan una conexion real.

El evento de escalamiento solo se publica para un caso fallido y ya tiene un
consumidor local. El provider fake no usa red ni configuracion corporativa y
crea una unica representacion sintetica por caso.

## Implementacion local cerrada

1. El resultado fallido deja `BotCase` en `escalated`.
2. La misma transaccion publica `bot-case.escalation-requested.v1`.
3. El worker valida el payload y llama `ITicketingProvider`.
4. `FakeTicketingProvider` deriva ID, referencia y descripcion saneada.
5. PostgreSQL persiste un solo `ExternalTicket` por `case_id`.
6. El endpoint del caso expone el ticket cuando el worker termina.

El resultado exitoso nunca publica escalamiento ni crea ticket.

## Modelo recomendado

### Caso interno del bot

Se crea para:

- instalacion/desinstalacion resuelta;
- configuracion resuelta;
- fallo y escalamiento;
- solicitud manual de licencia, backup o formateo;
- rechazo por software prohibido/no aprobado;
- abandono, timeout o falta de respuesta.

Incluye:

- ID interno;
- usuario, equipo, sede y proyecto;
- intencion/categoria;
- software, version y politica;
- diagnostico, pasos y evidencia;
- resultado, tiempos y automatizacion evitada;
- ticket OpenText relacionado si existe;
- satisfaccion y motivo de cierre.

Este caso alimenta las metricas propias del proyecto sin contaminar la cola de
OpenText con tickets tecnicos innecesarios.

### Ticket externo OpenText

Es obligatorio cuando:

- el bot no resuelve;
- hace falta agente humano;
- hay software comercial/licencia;
- el software no esta aprobado;
- se solicita backup o formateo;
- BitLocker requiere recuperacion o enrolamiento humano durante un
  reprovisionamiento;
- una politica corporativa exige ticket real;
- el usuario pide escalamiento.

## Tickets resueltos por el bot

Flujo inicial:

1. crear `BotCase`;
2. ejecutar y documentar;
3. marcar `attended_waiting_user`;
4. pedir confirmacion/calificacion;
5. cerrar al confirmar;
6. si no responde a avisos durante 72 horas, cerrar por `no_response_sla`.

Si OpenText permite un assignment group/cuenta del bot:

- se puede crear un ticket espejo en grupo exclusivo del proyecto;
- asignarlo al bot o integracion tecnica;
- mantenerlo abierto hasta confirmacion o cierre por 72 horas;
- cerrarlo con solucion y correlacion del `BotCase`.

No abrir y cerrar inmediatamente antes de que el usuario confirme: distorsiona
satisfaccion, SLA y evidencia. El ticket espejo es opcional; las metricas internas
son la fuente primaria hasta validar la API y el modelo operativo.

## Escalamiento

Una consulta informativa no crea `BotCase` operativo ni ticket externo. Puede
registrarse como metrica conversacional anonimizada, pero solo se crea solicitud
o escalamiento despues de que el usuario confirme que desea proceder.

Ejemplos:

- "Se puede instalar Postman?": responder licencia/alternativas, sin ticket.
- "Quiero solicitar Postman": mostrar impacto y pedir confirmacion.
- "Si, crea la solicitud": crear `BotCase` y ticket de licencia/escalamiento.
- "Instala Insomnia": validar equipo/catalogo, pedir confirmacion y crear job.

1. El bot conserva el `BotCase` y evidencia.
2. Crea ticket OpenText con descripcion detallada.
3. No fuerza assignee individual.
4. Permite que OpenText enrute por sede al assignment group correspondiente.
5. Estado inicial esperado: `Categorize`, sujeto a la API/configuracion real.
6. OpenText asigna segun disponibilidad de agentes.
7. El portal muestra el enlace; sincroniza estado si la API del piloto lo permite.

Si se habilita un grupo propio del proyecto, se usa solo para tickets resueltos
por automatizacion. Los escalados siguen hacia `IT SERVICE DESK` u otro grupo
calculado por OpenText.

## Taxonomia MVP observada

Jerarquia visible:

```text
IT Support
  > Top 5 IT Support
  > Desktop Support
    > Hardware
    > Office 365 Environment
    > Software
    > Mobile
  > IT Operations
  > Security
```

El MVP opera solamente en `IT Support > Desktop Support > Software`.

Servicios:

| Servicio | Enrutamiento MVP |
| --- | --- |
| Install Software | Automatizar aprobados; escalar fallo |
| Uninstall Software | Automatizar aprobados; escalar fallo/riesgo |
| Software Problems/Configuration | Automatizar solo acciones cerradas; escalar resto |
| License or Subscription Request | Crear ticket manual/escalado a licencias/compras |
| Backup Information | Crear ticket manual/escalado |
| Laptop Formatting | Crear ticket manual/escalado |
| BitLocker durante reprovisionamiento | Crear ticket y continuar solo cuando el procedimiento humano confirme el siguiente paso |
| Software no listado | Escalar a revision de seguridad; ruta exacta por confirmar |
| Software prohibido | Rechazar y registrar; excepcion fuera del flujo normal |

Ejemplos de `Software Problems/Configuration`:

- ajustes de una aplicacion;
- problemas de Zscaler o migracion de dominio;
- cambio controlado del archivo hosts, si seguridad crea una politica explicita;
- reparacion o reinstalacion como parte del diagnostico.

Aunque termine en reinstalacion, conserva la categoria de problema/configuracion
y documenta la accion tomada.

El reprovisionamiento automatizado es una capacidad posterior. Hasta que exista,
`Laptop Formatting` conserva el flujo humano. El ticket nunca contiene PIN,
recovery password ni una formula derivada del asset tag; solo registra que se
requiere intervencion y la correlacion del proceso.

## Campos comunes

Valores iniciales observados:

| Campo | Valor/regla |
| --- | --- |
| Phone | Dato corporativo disponible; no inventar `0000000000` en produccion |
| Site | Sede del usuario, ejemplo `México-Mitikah` |
| Area or CDS | `Help Desk` para este flujo, sujeto al catalogo/API |
| Place | `Online`, salvo contexto diferente |
| Needed By | Mismo dia para el MVP |
| Requested by | Cuenta nominal del bot/integracion |
| Open this ticket on behalf | Usuario propietario/solicitante |
| Preferred method of contact | `Chat` |

Los identificadores internos deben mapearse a IDs de catalogo OpenText, no solo a
los textos visibles de pantalla.

## Descripciones

### Solicitud normal

Descripcion breve:

```text
Solicitud de instalacion de {software} en {hostname}.
```

### Escalamiento

Descripcion detallada y sin secretos:

- solicitud original;
- software/version;
- usuario, equipo y sede;
- diagnosticos ejecutados;
- pasos intentados y tiempos;
- codigos de salida saneados;
- estado actual;
- motivo exacto del escalamiento;
- reinicio/logoff pendiente;
- correlacion del caso interno.

No incluir contrasenas, tokens, PII innecesaria ni logs completos.

## Urgencia

OpenText muestra:

- `1 - Critical`;
- `2 - High`;
- `3 - Average`;
- `4 - Low`.

La seleccion del usuario es una senal, no la prioridad final. El backend aplica
una matriz aprobada de impacto/urgencia:

- instalacion rutinaria como VS Code: `Low` o como maximo `Average`;
- no aceptar `Critical` solo porque el usuario lo eligio;
- seguridad, interrupcion masiva o bloqueo operativo usan reglas separadas;
- conservar `requestedUrgency` y `normalizedUrgency` para auditoria.

La prioridad final puede ser calculada por OpenText a partir de impacto y
urgencia.

## SLA y cierre

- `Needed By` del MVP es el mismo dia; no es una promesa de resolucion inmediata.
- Tras completar o pedir informacion, enviar recordatorios segun politica.
- Sin respuesta durante 72 horas: cierre por SLA/no respuesta.
- Resuelto por bot: cierre del usuario con calificacion o cierre por 72 horas.
- Escalado: OpenText gobierna SLA y cierre; el caso interno sincroniza resultado.

## Integracion

El adaptador debe descubrir y configurar:

- endpoint y version;
- autenticacion;
- IDs de catalogo y campos requeridos;
- assignment groups y reglas por sede;
- estados y transiciones;
- comentarios/activities;
- cierre, solution code y satisfaccion;
- rate limits, reintentos e idempotencia.

Usar outbox para crear/actualizar tickets. Una falla de OpenText no debe borrar el
caso ni repetir tickets.
