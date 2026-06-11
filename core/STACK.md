# Stack aprobado

Fecha de decision: 2026-06-07.

## Cliente Windows

| Area | Eleccion |
| --- | --- |
| Lenguaje/runtime | C# sobre .NET 10 LTS |
| UI | WinUI 3 y Windows App SDK |
| Patron | MVVM con CommunityToolkit.Mvvm, features e inyeccion de dependencias |
| API local | Contratos tipados y Named Pipes con ACL |
| Empaquetado piloto | MSIX o instalador empresarial firmado con PKI interna |
| Distribucion piloto | UEMS Endpoint Central, pendiente de confirmacion formal |
| Arquitecturas | x64 y x86 cuando el paquete lo requiera; ARM64 fuera del MVP |
| Telemetria | OpenTelemetry, sin contenido sensible por defecto |
| Pruebas | xUnit, pruebas de contratos y automatizacion UI de Windows |

Motivo: WinUI 3 es el framework nativo moderno de Microsoft para escritorio
Windows y permite Fluent, Mica, alta densidad de pixeles, accesibilidad y APIs de
Windows sin envolver una web.

## Agente del dispositivo

| Area | Eleccion |
| --- | --- |
| Proceso | .NET 10 Worker Service como Windows Service |
| Identidad | Cuenta virtual/local exclusiva, sin logon interactivo; gMSA solo si existe requisito de dominio |
| IPC | Named Pipes autenticados y con autorizacion por operacion |
| Ejecucion | Adaptadores tipados para WinGet, MSI, MSIX, PowerShell firmado e Intune |
| Estado local | SQLite solo para cola durable y cache tecnico, cifrado cuando aplique |
| Actualizacion | Politica empresarial versionada e independiente de la UI |

El agente no expone una consola generica ni acepta comandos arbitrarios. Cada
accion se resuelve desde un identificador de catalogo versionado y una politica
firmada.

## Portal administrativo web

| Area | Eleccion |
| --- | --- |
| Lenguaje | TypeScript |
| Framework | Next.js App Router sobre Node.js LTS |
| UI | React, Fluent UI React y CSS/Tailwind solo para layout/utilidades |
| Datos | Prisma ORM sobre PostgreSQL |
| Validacion | Esquemas tipados en formularios, Server Actions y Route Handlers |
| Pruebas | Vitest/Testing Library y Playwright |
| Desarrollo MVP | Vercel + Supabase PostgreSQL, sujeto a datos sinteticos |
| Hosting corporativo | Azure App Service o Azure Container Apps |

El portal administra usuarios, roles, sedes, catalogo, aprobaciones, trabajos,
tickets, telemetria y auditoria. No puede conectarse directamente a equipos ni
ejecutar comandos; todas las acciones pasan por backend, politicas y agente.

Next.js puede ejecutarse en Vercel, en cualquier servidor Node.js o como
contenedor. Se usara `output: "standalone"` para mantener portabilidad.

## Backend y worker

| Area | Eleccion |
| --- | --- |
| API web/control | Route Handlers/Server Actions de Next.js |
| Arquitectura | Monolito modular TypeScript |
| Persistencia | PostgreSQL con Prisma ORM |
| Autenticacion | Abstraccion OIDC; Entra workforce como destino |
| Identidad del portal | Microsoft Entra corporativo con MFA |
| Autorizacion | Roles y politicas por sede, area, equipo y accion |
| Trabajos asincronos | Worker Node.js separado, outbox y colas persistentes |
| Cache | Ninguno en MVP; Redis solo con necesidad medida |
| Observabilidad | OpenTelemetry, logs estructurados, metricas y trazas |
| Contratos con agentes | OpenAPI/versionado y mensajes firmados |

El monolito modular reduce operacion y mantiene fronteras claras. No se adoptan
microservicios hasta que escalamiento, equipos o despliegues independientes lo
justifiquen.

El worker no vive dentro de funciones serverless: sincroniza OpenText, procesa
outbox, expiraciones de 72 horas y trabajos de agente desde un proceso durable.
En Azure puede vivir en Container Apps Job/Worker o App Service WebJob. Durante
desarrollo puede ejecutarse como proceso Node separado.

## Infraestructura recomendada

- Azure App Service o Container Apps para Next.js y worker, con PostgreSQL y
  secretos en red privada.
- Azure Database for PostgreSQL.
- Azure Key Vault para secretos, certificados y rotacion.
- Azure Monitor/Application Insights como destino de OpenTelemetry.
- UEMS Endpoint Central templates + Software Repository HTTP/WAN para obtener,
  almacenar y distribuir paquetes durante el MVP.
- Azure Blob privado solo si un paquete aprobado no puede gestionarse desde UEMS.
- UEMS Endpoint Central tambien distribuye cliente/agente, configuracion y
  certificados aprobados.
- GitHub Actions o Azure DevOps con OIDC y ambientes protegidos.

Una VM es valida para el gateway de IA autocontenido o para una integracion que
requiera software persistente. No es la primera opcion para la API: obliga al
equipo a parchar SO, runtime, proxy, certificados, backups y alta disponibilidad.

Vercel + Supabase es un perfil valido para desarrollo/demo con datos sinteticos.
No recibira inventario real, tickets reales, credenciales OpenText ni endpoints
de agentes hasta que seguridad, residencia, contratos y presupuesto lo aprueben.
El perfil corporativo preferido permanece en Azure.

## Integraciones por adaptador

- OpenText Service Manager: `ITicketingProvider`.
- Microsoft Graph/Intune: `IDeviceManagementProvider`.
- WinGet/MSI/MSIX: `ISoftwareExecutionProvider`.
- Gateway de IA autocontenido o Azure OpenAI: `IAssistantProvider`.
- GoTo/LogMeIn Rescue: `IRemoteSupportProvider`.
- Microsoft Teams SDK: `IConversationChannel`.
- Power Automate: `IWorkflowAutomationProvider`, solo para aprobaciones,
  notificaciones y procesos administrativos.
- `recovery.softtek.com`: recuperacion oficial de identidad; confirmar si usa
  Entra SSPR/password writeback. No se implementa un reset propio.

La IA puede clasificar, explicar y recopilar contexto. No puede autorizar,
construir ni ejecutar comandos.

MiniMax, DeepSeek y Qwen solo pueden evaluarse tras aprobacion de AI/Security.
Hermes Agent puede ejecutarse localmente y consumir MiniMax directamente o por
OpenRouter. El procesamiento del modelo sigue ocurriendo en el proveedor remoto.
Eso reduce infraestructura local, pero no evita salida de prompts ni costo API.

Para construir el producto se adopta un perfil hibrido: Hermes en WSL2 para
orquestacion, investigacion y web; Visual Studio/MSBuild en Windows para WinUI,
MSIX y pruebas nativas. El soporte nativo Windows de Hermes se considera beta y
una VM Ubuntu no sustituye las validaciones Windows. La seleccion de modelos,
roles y limites vive en `../standards/AI_DEVELOPMENT.md`.

El acceso corporativo a Microsoft Copilot tampoco concede automaticamente un
endpoint Azure OpenAI. Se necesita recurso, deployment, identidad, cuotas,
region y autorizacion API propios.

## Canales y Power Platform

El bot de Teams se implementa como canal del backend compartido mediante Teams
SDK o la plataforma corporativa aprobada. Requiere un endpoint HTTPS y
conectividad a Microsoft 365. No reemplaza la aplicacion nativa cuando una tarea
necesita diagnostico local, continuidad offline o ejecucion privilegiada.

Power Automate se evalua como complemento. Los cloud flows y el conector Teams
son apropiados para mensajes, formularios, aprobaciones y tickets. Power
Automate Desktop requiere registro de maquinas, sesiones y licenciamiento para
RPA desatendido. La documentacion vigente indica limitaciones de elevacion y
sesion; una capacidad anunciada para 2026 no se adopta hasta verificar
disponibilidad, seguridad y soporte en el tenant.

La recuperacion desde el inicio de Windows se evaluara contra
`recovery.softtek.com` cuando equipos, conectividad y tecnologia subyacente lo
permitan. WinUI y Teams solo orientan y registran evidencia; nunca capturan
contrasenas.

## No elegidos

- Electron/Tauri/WebView como cliente principal: no cumplen el objetivo nativo.
- React/Vite reutilizado: queda como referencia visual, no como base.
- .NET MAUI: agrega abstraccion multiplataforma que este producto no necesita.
- Microservicios desde el primer dia: costo operativo sin demanda demostrada.
- SQLite como base central: insuficiente para concurrencia, auditoria y reportes.
- PowerShell libre generado por IA: riesgo de ejecucion remota no controlada.
- Un backend ASP.NET adicional para el MVP: duplicaria modelos, autenticacion,
  migraciones y despliegues. Se reconsidera solo si una integracion corporativa
  exige .NET.

## Base oficial consultada

- WinUI 3: https://learn.microsoft.com/windows/apps/winui/
- Soporte .NET: https://dotnet.microsoft.com/platform/support/policy
- MSIX empresarial:
  https://learn.microsoft.com/windows/msix/desktop/managing-your-msix-deployment-enterprise
- Worker como Windows Service:
  https://learn.microsoft.com/dotnet/core/extensions/windows-service
- Firma y distribucion interna:
  https://learn.microsoft.com/windows/apps/package-and-deploy/choose-distribution-path
- Azure App Service privado:
  https://learn.microsoft.com/azure/app-service/networking/private-endpoint
- Self-hosting de Next.js:
  https://nextjs.org/docs/app/guides/self-hosting
- Prisma con PostgreSQL:
  https://docs.prisma.io/docs/orm/core-concepts/supported-databases/postgresql
- Hermes Agent con MiniMax:
  https://platform.minimax.io/docs/token-plan/hermes-agent
