# Contexto actual

Fecha de ultima actualizacion: 2026-06-11.

## Objetivo inmediato

Implementar el Bloque 1: shell nativo WinUI 3 con navegacion, tema,
accesibilidad y layout adaptable usando datos fijos. El portal administrativo
permanece reservado para el Bloque 11.

## Estado del repositorio

- Repositorio independiente inicializado en `main`.
- Remoto: `https://github.com/jasm001/Desktop_Native.git`.
- Solucion .NET 10 y workspace pnpm creados por frontera.
- Nullable, analyzers, warnings como errores y paquetes centralizados activos.
- Pruebas unitarias, de contratos y de arquitectura iniciales activas.
- Gitleaks y CI de GitHub configurados.
- No existen acciones privilegiadas, secretos, datos corporativos ni
  integraciones productivas.

## Producto

El sistema se compone de:

- cliente WinUI 3 sin privilegios;
- Windows Service restringido para acciones locales;
- API compartida y worker;
- canal Teams conectado posteriormente a la misma API;
- portal administrativo web como plano de control.

## Decisiones esenciales

- C#/.NET 10 LTS, WinUI 3, Windows App SDK, MVVM y DI.
- Next.js, TypeScript, Prisma y PostgreSQL para backend y portal.
- UI, IA y Teams nunca ejecutan comandos privilegiados.
- El agente acepta solo acciones tipadas y allowlisted.
- Consultar no equivale a actuar: toda accion requiere propuesta y confirmacion.
- El catalogo funciona sin IA.
- OpenText, UEMS, Entra, IA externa y Azure se desarrollan primero mediante
  contratos y adaptadores fake.
- La prueba de viabilidad empieza con cliente/agente nativo y API compartida.
- El bot de Teams existente se integra despues; no se crea otro bot.
- El portal administrativo es el ultimo bloque funcional del plan inicial.

## Alcance del primer recorrido

El primer recorrido vertical usa datos sinteticos:

1. abrir la app WinUI;
2. consultar un catalogo local de prueba;
3. seleccionar un software permitido;
4. mostrar propuesta y pedir confirmacion;
5. crear una solicitud idempotente;
6. enviar un trabajo a un agente simulado;
7. mostrar progreso y resultado;
8. registrar evidencia local saneada.

No instala software real durante este recorrido.

## Pendientes que no bloquean el esqueleto

- sede y equipos piloto;
- acceso real a OpenText;
- canal y requisitos definitivos de UEMS;
- App Registration Entra;
- hosting Azure;
- proveedor de IA aprobado;
- PKI/firma y reglas Sophos;
- retencion corporativa.

Estos pendientes bloquean integraciones o piloto, no la implementacion local.

## Stopper

Se considera stopper una decision que altere seguridad, privilegios, contratos
publicos, persistencia, alcance o stack. Debe registrarse en `WORKFLOW.md` con:

- evidencia;
- alternativas;
- impacto;
- recomendacion;
- decision requerida.
