# Fuentes tecnicas

Consultadas el 2026-06-07.

## Documentos internos

- `mfa reset 1.pdf`: manual interno de registro de MFA para Office 365.
  Describe SMS, llamada, notificacion push y codigo de Microsoft Authenticator.
  No documenta SSPR, password writeback ni restablecimiento de contrasena.

## Windows y distribucion

- WinUI 3:
  https://learn.microsoft.com/windows/apps/winui/
- Politica de soporte .NET:
  https://dotnet.microsoft.com/platform/support/policy
- Rutas de distribucion de aplicaciones Windows:
  https://learn.microsoft.com/windows/apps/package-and-deploy/choose-distribution-path
- Firma MSIX para desarrollo, piloto y produccion:
  https://learn.microsoft.com/windows/msix/package/sign-msix-package-guide
- Opciones de firma:
  https://learn.microsoft.com/windows/apps/package-and-deploy/code-signing-options
- Contenedorizacion MSIX y AppContainer:
  https://learn.microsoft.com/windows/msix/msix-containerization-overview
- Windows Sandbox CLI:
  https://learn.microsoft.com/windows/security/application-security/application-isolation/windows-sandbox/windows-sandbox-cli
- `gpupdate`:
  https://learn.microsoft.com/windows-server/administration/windows-commands/gpupdate
- gMSA:
  https://learn.microsoft.com/windows-server/identity/ad-ds/manage/group-managed-service-accounts/group-managed-service-accounts/getting-started-with-group-managed-service-accounts
- WinGet sources:
  https://learn.microsoft.com/windows/package-manager/winget/source
- Endpoint Central Software Deployment:
  https://www.manageengine.com/products/desktop-central/software-deployment.html
- Endpoint Central Software Templates:
  https://www.manageengine.com/products/desktop-central/help/software_installation/software_deployment_templates.html
- Endpoint Central Auto-update Templates:
  https://www.manageengine.com/products/desktop-central/help/software_installation/auto-update-templates-and-policies.html
- Endpoint Central Software Repository:
  https://www.manageengine.com/products/desktop-central/software-repository.html
- Endpoint Central latest software templates:
  https://www.manageengine.com/products/desktop-central/software-installation/latest-software.html
- SQL Server unattended setup:
  https://learn.microsoft.com/sql/database-engine/install-windows/install-sql-server-from-the-command-prompt
- SSMS command-line install:
  https://learn.microsoft.com/ssms/install/command-line-parameters
- Git for Windows unattended install:
  https://gitforwindows.org/silent-or-unattended-installation.html
- Microsoft DSC:
  https://learn.microsoft.com/powershell/dsc/overview
- WinGet Configuration:
  https://learn.microsoft.com/windows/package-manager/configuration/
- Windows Installer transforms:
  https://learn.microsoft.com/windows/win32/msi/transforms
- MSIX Packaging Tool:
  https://learn.microsoft.com/windows/msix/packaging-tool/tool-overview
- Reempaquetar instaladores:
  https://learn.microsoft.com/windows/msix/packaging-tool/create-app-package

## Identidad y Azure

- Microsoft Entra External ID:
  https://learn.microsoft.com/entra/external-id/external-identities-overview
- Microsoft Entra Workload ID:
  https://learn.microsoft.com/entra/workload-id/workload-identities-overview
- Configuraciones workforce/external:
  https://learn.microsoft.com/entra/external-id/tenant-configurations
- Cuentas locales de external tenant:
  https://learn.microsoft.com/entra/external-id/customers/concept-authentication-methods-customers
- Private Endpoints para App Service:
  https://learn.microsoft.com/azure/app-service/networking/private-endpoint
- Azure OpenAI pricing:
  https://azure.microsoft.com/pricing/details/azure-openai/
- MiniMax API pay-as-you-go:
  https://platform.minimax.io/docs/guides/pricing-paygo
- MiniMax with Hermes Agent:
  https://platform.minimax.io/docs/token-plan/hermes-agent
- OpenRouter with Hermes Agent:
  https://openrouter.ai/docs/cookbook/coding-agents/hermes-integration
- OpenRouter data policy filtering:
  https://openrouter.ai/docs/guides/routing/provider-selection
- OpenRouter provider logging/retention:
  https://openrouter.ai/docs/guides/privacy/logging/
- OpenRouter limites y creditos por clave:
  https://openrouter.ai/docs/api-reference/limits/
- OpenRouter API keys con limite de gasto:
  https://openrouter.ai/docs/client-sdks/python/api-reference/apikeys
- Hermes Agent:
  https://github.com/NousResearch/hermes-agent
- Hermes en WSL2:
  https://github.com/NousResearch/hermes-agent/blob/main/website/docs/user-guide/windows-wsl-quickstart.md
- Hermes nativo Windows beta:
  https://github.com/NousResearch/hermes-agent/blob/main/website/docs/user-guide/windows-native.md
- RTK para Hermes:
  https://github.com/ogallotti/rtk-hermes
- DeepSeek V4 Pro:
  https://openrouter.ai/deepseek/deepseek-v4-pro
- DeepSeek V4 Flash:
  https://openrouter.ai/deepseek/deepseek-v4-flash
- MiniMax M3:
  https://openrouter.ai/minimax/minimax-m3
- Qwen3.7 Plus:
  https://openrouter.ai/qwen/qwen3.7-plus
- Step 3.7 Flash:
  https://openrouter.ai/stepfun/step-3.7-flash
- Kimi K2.6:
  https://openrouter.ai/moonshotai/kimi-k2.6
- Azure App Service:
  https://learn.microsoft.com/azure/app-service/getting-started
- Entra SSPR desde el inicio de Windows:
  https://learn.microsoft.com/entra/identity/authentication/howto-sspr-windows
- Entra SSPR y password writeback:
  https://learn.microsoft.com/entra/identity/authentication/tutorial-enable-sspr-writeback
- Licencias de Entra SSPR:
  https://learn.microsoft.com/entra/identity/authentication/concept-sspr-licensing
- Metodos de autenticacion y reset mediante Microsoft Graph:
  https://learn.microsoft.com/graph/authenticationmethods-get-started
- Temporary Access Pass:
  https://learn.microsoft.com/graph/api/resources/temporaryaccesspassauthenticationmethod
- Credenciales de dominio almacenadas:
  https://learn.microsoft.com/troubleshoot/windows-server/user-profiles-and-logon/cached-domain-logon-information

## Next.js y datos

- Self-hosting Next.js:
  https://nextjs.org/docs/app/guides/self-hosting
- Requisitos de plataformas Next.js:
  https://nextjs.org/docs/app/guides/deploying-to-platforms
- Prisma PostgreSQL:
  https://docs.prisma.io/docs/orm/core-concepts/supported-databases/postgresql

## Entrega e integraciones

- Azure DevOps:
  https://learn.microsoft.com/azure/devops/user-guide/what-is-azure-devops
- Azure Pipelines:
  https://learn.microsoft.com/azure/devops/pipelines/
- GoTo Rescue API:
  https://developer.goto.com/pdf/Rescue_APIGuide.pdf
- OpenText SMAX overview, solo como referencia hasta confirmar producto/version:
  https://www.opentext.com/assets/service-management-automation-x-smax-ds-en.pdf
- Microsoft Teams bots:
  https://learn.microsoft.com/microsoftteams/platform/bots/overview
- Conversaciones con bots de Teams:
  https://learn.microsoft.com/microsoftteams/platform/bots/build-conversational-capability
- Conector Teams de Power Automate:
  https://learn.microsoft.com/connectors/teams/
- Requisitos y limites de Power Automate Desktop:
  https://learn.microsoft.com/power-automate/desktop-flows/requirements
- Power Automate desatendido:
  https://learn.microsoft.com/power-automate/desktop-flows/run-unattended-desktop-flows
- Power Automate con derechos elevados:
  https://learn.microsoft.com/power-automate/desktop-flows/how-to/run-power-automate-elevated-rights
- Licencias Power Automate:
  https://learn.microsoft.com/power-platform/admin/power-automate-licensing/overview
- Power Automate 2026 release wave 1:
  https://learn.microsoft.com/power-platform/release-plan/2026wave1/power-automate/

## Vercel evaluado

- Vercel Functions:
  https://vercel.com/docs/functions/
- Connectivity y costos de red privada:
  https://vercel.com/docs/connectivity
- Secure Compute:
  https://vercel.com/docs/secure-compute

## Seguridad de codigo

- OWASP Source Code Analysis Tools:
  https://owasp.org/www-community/Source_Code_Analysis_Tools
- GitHub CodeQL:
  https://docs.github.com/code-security/code-scanning/introduction-to-code-scanning/about-code-scanning-with-codeql
- CodeQL en repositorios privados:
  https://docs.github.com/code-security/code-scanning/troubleshooting-code-scanning/cannot-enable-codeql-in-a-private-repository
- Semgrep supported languages:
  https://semgrep.dev/docs/supported-languages

## Licencias de asistencia

- Planes actuales de GitHub Copilot:
  https://github.com/features/copilot/plans
- Modelos, precios y cambio a creditos de Copilot:
  https://docs.github.com/copilot/reference/copilot-billing/models-and-pricing
