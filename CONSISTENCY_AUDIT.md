# Auditoria de consistencia documental

Fecha: 2026-06-11.

## Cobertura

Se contrastaron los documentos operativos, el plan modular, las decisiones
normativas, los estandares de entrega, el modulo de catalogo, la documentacion
del shell WinUI, el codigo actual, los lockfiles y los commits publicados.

## Precedencia

La precedencia documental esta definida en `README.md`. `MASTER_PROMPT.md` es
solo un mecanismo de handoff y no puede modificar alcance, stack, arquitectura,
seguridad o decisiones cerradas.

Ante una contradiccion material:

1. no elegir silenciosamente;
2. conservar evidencia;
3. registrar alternativas e impacto en `WORKFLOW.md`;
4. detener solo la parte afectada hasta obtener decision.

## Estado verificado

- Bloque 0 completado y publicado en `e42fe2c`.
- Bloque 1 completado y publicado en `f18e8cf`.
- Toolchain .NET 10 publicado en `5f6dae7`.
- El commit `531faf6` agrego RIDs no declarados a dos lockfiles. El handoff
  actual los regenero para `win-x64` y `scripts/Validate.ps1` vuelve a pasar.
- Bloque 2 es la siguiente unidad; no existe otro bloque principal activo.
- La shell WinUI contiene cinco vistas y solo datos sinteticos.
- No existen instalaciones, tickets, solicitudes, diagnosticos reales, IPC o
  integraciones productivas.
- El portal administrativo permanece reservado para el Bloque 11.
- La CLI con SDK global .NET `10.0.301` restaura, compila, prueba y ejecuta el
  cliente.
- El gate completo valida restore bloqueado, formato, build Release sin
  warnings, 6 pruebas, workspace pnpm y Gitleaks.
- Visual Studio 2022 no admite `net10.0`; el IDE compatible es Visual Studio
  2026 version 18.0 o posterior.

## Consistencia del siguiente bloque

El Bloque 2 debe implementar dominio y casos de uso puros para catalogo
sintetico:

- producto, version, licencia y estado;
- aprobado, comercial, no listado, EOL y prohibido;
- alternativas;
- busqueda y filtros;
- decisiones `informar`, `proponer`, `escalar` y `rechazar`.

El dominio no depende de WinUI, Prisma, IA, backend ni integraciones externas.
El Excel corporativo no se importa todavia y nunca se interpreta directamente
como allowlist ejecutable.

## Diferencias intencionales

- El mockup React/Vite es referencia visual, no base de producto.
- `reference/` conserva material historico y tiene menor precedencia.
- El portal pertenece al producto, pero se implementa al final del plan.
- Las integraciones faltantes se representan con contratos y fakes solo cuando
  el bloque activo los necesita.

## Incertidumbres externas

Sede piloto, OpenText, UEMS, Entra, Azure, proveedor de IA, PKI, Sophos y
retencion corporativa siguen pendientes. No bloquean el Bloque 2 y no autorizan
inventar credenciales, permisos, endpoints o reglas corporativas.
