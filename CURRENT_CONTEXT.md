# Contexto actual

Fecha de ultima actualizacion: 2026-06-12.

## Objetivo inmediato

Implementar el Bloque 5: diagnostico local de solo lectura. Debe recolectar solo
Windows/arquitectura, almacenamiento, memoria, red, dominio, version del agente
y prerrequisitos declarados, sin explorar archivos personales ni inventario
corporativo general.

## Estado del repositorio

- Repositorio independiente inicializado en `main`.
- Remoto: `https://github.com/jasm001/Desktop_Native.git`.
- Bloque 0 publicado en `e42fe2c`.
- Bloque 1 publicado en `f18e8cf`; ajustes de toolchain y lockfiles publicados
  en `5f6dae7` y `531faf6`.
- Bloque 2 publicado en `0d1e315`.
- Bloque 3 publicado en `b09f07a`.
- Bloque 4 publicado en `b56bfcb`.
- Los lockfiles Desktop/WindowsUi conservan solo el RID declarado `win-x64`.
- Solucion .NET 10 y workspace pnpm creados por frontera.
- Nullable, analyzers, warnings como errores y paquetes centralizados activos.
- Pruebas unitarias, de contratos y de arquitectura iniciales activas.
- Shell WinUI con cinco vistas, tema claro/oscuro, navegacion por teclado,
  layout adaptable y datos sinteticos validada.
- Dominio puro de catalogo con producto, version, licencia, estado,
  alternativas, busqueda, filtros y decisiones tipadas.
- Fixtures sinteticos cubren aprobado, comercial, no listado, EOL y prohibido.
- La vista Catalogo consume el caso de uso mediante DI y no contiene reglas de
  autorizacion.
- Conversacion determinista con cinco estados, intenciones fijas, confirmacion
  explicita, cancelacion e idempotencia por comando.
- La referencia `SyntheticRequest` vive solo en la sesion local; no es una
  solicitud corporativa persistida.
- La vista Asistente consume la maquina de estados mediante DI y mantiene el
  texto libre deshabilitado.
- Contrato IPC v1 con mensajes tipados para iniciar, consultar y cancelar
  trabajos; no contiene comando ni argumentos libres.
- DeviceAgent dividido entre host Worker y nucleo comprobable.
- Named Pipe de desarrollo limitado al usuario actual y frames de hasta 64 KiB.
- Autorizacion deny-by-default por version, mensaje, accion, target y version.
- Trabajo simulado con progreso, cancelacion, evidencia saneada e idempotencia.
- Estado local durable en SQLite y recuperacion de trabajos interrumpidos.
- Ejecutable Desktop x64 autocontenido para desarrollo local.
- SDK global `10.0.301` validado desde `C:\Program Files\dotnet`.
- La CLI compila y ejecuta la shell. La depuracion en IDE requiere Visual Studio
  2026 version 18.0 o posterior; Visual Studio 2022 no admite `net10.0`.
- Gitleaks y CI de GitHub configurados.
- Build Release y 68 pruebas pasan para el Bloque 4; la validacion completa se
  registra en `WORKFLOW.md`.
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

No instala software real durante este recorrido. El agente simulado ya puede
crear trabajos locales tipados por IPC, pero la shell actual no los invoca y
tampoco crea solicitudes o tickets corporativos.

## Pendientes que no bloquean el esqueleto

- sede y equipos piloto;
- acceso real a OpenText;
- canal y requisitos definitivos de UEMS;
- App Registration Entra;
- hosting Azure;
- proveedor de IA aprobado;
- PKI/firma y reglas Sophos;
- retencion corporativa.
- perfil FortiClient EMS de maquina/pre-logon para una futura campana de
  refresco de politicas;
- prueba de alcance de dominio mediante Axis Atmos en el equipo fisico de
  desarrollo.

Estos pendientes bloquean integraciones o piloto, no la implementacion local.
Los dos pendientes de conectividad de politicas tampoco bloquean el Bloque 5 ni
el flujo principal; condicionan solo la mejora posterior descrita en
`docs/modules/domain-policy-refresh.md`.

El roadmap posterior tambien contempla autoservicio local, evidencia,
reparaciones cerradas, almacenamiento, perifericos, mantenimiento, perfiles y
reprovisionamiento. No modifica el Bloque 5 ni el plan principal. La direccion
empresarial o independiente queda pendiente y se documenta en
`docs/modules/endpoint-self-service.md` y
`docs/modules/device-reprovisioning.md`.

## Stopper

Se considera stopper una decision que altere seguridad, privilegios, contratos
publicos, persistencia, alcance o stack. Debe registrarse en `WORKFLOW.md` con:

- evidencia;
- alternativas;
- impacto;
- recomendacion;
- decision requerida.
