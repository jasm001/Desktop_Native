# Contexto actual

Fecha de ultima actualizacion: 2026-06-14.

## Objetivo inmediato

Los Bloques 0 a 8 estan `completed`. No hay bloque principal activo. El
siguiente bloque desbloqueado es el Bloque 9: adaptar el canal Teams existente a
la API compartida sin crear otro bot ni adelantar endurecimiento o portal.

El cierre tecnico del Bloque 8 vive en
`docs/modules/case-foundation.md` y su gobierno en `modules/TICKETING.md`.
El documento propietario del siguiente bloque es `modules/TEAMS.md`.

## Estado del repositorio

- Rama principal `main`; remoto
  `https://github.com/jasm001/Desktop_Native.git`.
- El Bloque 8 esta publicado y cerrado en `cf262b4`.
- `src/AdminWeb` es el control plane Next.js modular; no es el portal del
  Bloque 11.
- Prisma/PostgreSQL tiene cuatro migraciones versionadas.
- Cada confirmacion crea una sola `SupportRequest`, `ExecutionJob` y `BotCase`.
- Exito deja el caso en `attended_waiting_user` sin ticket.
- Fallo deja el caso en `escalated` y publica
  `bot-case.escalation-requested.v1` en la misma transaccion.
- `src/Worker` valida el evento y usa `ITicketingProvider`.
- `FakeTicketingProvider` es determinista, local, sin red y sin configuracion
  corporativa.
- `external_tickets` permite un solo ticket sintetico por caso.
- La consulta HTTP del caso devuelve el ticket nullable sin efectos laterales.
- La politica de 72 horas es pura; no existe scheduler ni cierre automatico.
- Auditoria append-only, outbox, leases y reintentos acotados permanecen activos.
- WinUI y DeviceAgent conservan el recorrido local del Bloque 7 y no recibieron
  nuevas capacidades privilegiadas.
- OpenText, Teams, Entra, UEMS, Hermes/RAG y portal productivos siguen
  deshabilitados.
- El gate completo mantiene 113 pruebas .NET; Node tiene 17 pruebas
  unitarias/de contrato, 11 integraciones AdminWeb y 4 del Worker, mas el E2E
  WinUI/DeviceAgent sobre PostgreSQL efimero.

## Siguiente reanudacion

1. Confirmar que los Bloques 0 a 8 permanecen `completed`.
2. Leer `modules/TEAMS.md` antes de activar el Bloque 9.
3. Reutilizar contratos y decisiones de la API compartida; no crear reglas de
   catalogo, casos o ticketing dentro del canal.
4. Mantener Teams y WinUI equivalentes para la misma entrada confirmada.
5. Registrar un stopper si se intenta conectar el bot real sin owner,
   plataforma, autenticacion, permisos y ambiente aprobados.

## Limites vigentes

- No usar datos, credenciales, endpoints ni identificadores corporativos.
- No conectar OpenText real.
- No permitir que Teams, WinUI, IA o portal ejecuten comandos.
- No adelantar Bloques 10 u 11.
- Registrar stopper en `WORKFLOW.md` si una decision cambia seguridad,
  persistencia, contratos publicos, stack o alcance.
