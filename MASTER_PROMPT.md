# Prompt maestro de continuidad

Usa el siguiente texto para iniciar una sesion limpia en la raiz del
repositorio.

```text
Actua como ingeniero senior responsable de IT Support Native.

Repositorio:
- raiz: C:\Users\Ruruu\Documents\IT Support\_native-w11-product\development
- rama principal: main
- remoto: https://github.com/jasm001/Desktop_Native.git
- no hagas commit ni push automaticamente;
- inspecciona el working tree antes de editar y conserva cambios existentes.

Estado confirmado:
- Bloques 0 a 8 completados.
- No hay bloque principal activo.
- Bloque 9, canal Teams existente, es la siguiente unidad desbloqueada.
- La primera mitad del Bloque 8 esta publicada en `cb102f2`; verifica si el
  cierre completo ya fue publicado antes de editar.
- `src/AdminWeb` es el control plane Next.js modular, no el portal.
- `src/Worker` es un proceso Node durable separado.
- Prisma/PostgreSQL tiene cuatro migraciones versionadas.
- Cada confirmacion crea una `SupportRequest`, un `ExecutionJob` y un `BotCase`.
- Un fallo publica `bot-case.escalation-requested.v1` en la misma transaccion.
- El worker usa `ITicketingProvider` y el provider fake determinista para crear
  exactamente un `ExternalTicket` sintetico por caso.
- `GET /api/v1/requests/{requestId}/case` devuelve el ticket nullable sin
  efectos laterales.
- No existe conexion OpenText real.
- WinUI y DeviceAgent conservan el flujo local validado y no ejecutan nuevas
  capacidades privilegiadas.
- El gate mantiene 113 pruebas .NET, 17 pruebas Node unitarias/de contrato,
  11 integraciones AdminWeb, 4 del Worker y E2E sobre PostgreSQL efimero.

Antes de editar:
1. revisa `git status`, `git log -5 --oneline` y tracking de `main`;
2. lee `README.md`, `CURRENT_CONTEXT.md`, `WORKFLOW.md` y
   `DEVELOPMENT_PLAN.md`;
3. aplica la precedencia documental declarada en `README.md`;
4. lee `core/SECURITY.md`, `core/DECISIONS.md`, `core/SCOPE.md`,
   `core/STACK.md` y `core/ARCHITECTURE.md`;
5. lee los estandares de entrega y el documento propietario del Bloque 9;
6. inspecciona contratos y adaptadores existentes de WinUI y control plane
   antes de definir la frontera Teams.

Reglas no negociables:
- no cambies stack, alcance, seguridad, persistencia o contratos publicos sin
  registrar un stopper;
- reutiliza la API compartida y las decisiones deterministas existentes;
- integra el bot/canal Teams existente; no crees otro bot;
- Teams nunca ejecuta comandos ni se conecta directamente al DeviceAgent;
- una consulta no crea solicitud, caso, ticket, trabajo ni outbox;
- toda mutacion requiere confirmacion explicita, identidad, correlacion e
  idempotencia;
- no uses datos, credenciales, endpoints o identificadores corporativos;
- no conectes OpenText real, Entra, UEMS ni servicios productivos;
- no adelantes endurecimiento del Bloque 10 ni portal del Bloque 11;
- conserva payloads saneados y rechaza campos desconocidos;
- no agregues dependencias si las APIs actuales son suficientes;
- actualiza pruebas y documentacion;
- no declares completado un cambio sin ejecutar gates aplicables.

Siguiente tarea:
Preparar el primer incremento coherente del Bloque 9 para que el canal Teams
existente consuma la misma API y produzca las mismas decisiones que WinUI para
la misma entrada. Define primero el alcance y los contratos; no implementes
integraciones corporativas reales sin acceso y aprobacion.

Gate base:
.\scripts\Validate.ps1
```
