# Chat local con Hermes

## Objetivo

Habilitar temporalmente texto libre en la vista Asistente de WinUI usando un
API Server local compatible con OpenAI. Esta configuracion es solo para demo
local y no cambia la frontera operativa del producto.

Hermes puede responder orientacion informativa. No autoriza acciones, no crea
solicitudes, no llama al DeviceAgent y no genera comandos, scripts, rutas ni
argumentos ejecutables.

## Prerrequisitos

- Hermes API Server levantado en loopback.
- Endpoint compatible con OpenAI en `http://127.0.0.1:8765/v1`.
- Modelo disponible: `it-support`.
- API key disponible solo en el entorno local de PowerShell o en variables de
  entorno de usuario del equipo de desarrollo.
- `DATABASE_URL` migrada si tambien se mostrara AdminWeb:

```powershell
corepack pnpm@11.5.3 --filter @it-support-native/admin-web prisma:migrate:deploy
$env:IT_SUPPORT_ENVIRONMENT='development'
$env:LOCAL_DEVELOPMENT_IDENTITY_ENABLED='true'
corepack pnpm@11.5.3 --filter @it-support-native/admin-web db:seed:development
```

## Validar Hermes

No guardes la clave en Git, `.env`, capturas o logs compartidos. Para una
validacion puntual, definela solo en la sesion local:

```powershell
$env:IT_SUPPORT_HERMES_API_KEY='<api-key-local>'
Invoke-RestMethod `
  -Uri 'http://127.0.0.1:8765/v1/models' `
  -Headers @{ Authorization = "Bearer $env:IT_SUPPORT_HERMES_API_KEY" }
```

La respuesta debe incluir el modelo `it-support`.

## Habilitar WinUI

Configura estas variables en la misma terminal desde donde se inicia la app:

```powershell
$env:IT_SUPPORT_HERMES_CHAT_ENABLED='true'
$env:IT_SUPPORT_HERMES_BASE_URL='http://127.0.0.1:8765/v1'
$env:IT_SUPPORT_HERMES_MODEL='it-support'
$env:IT_SUPPORT_HERMES_API_KEY='<api-key-local>'
```

Luego ejecuta la app:

```powershell
dotnet run --project .\src\Desktop\ITSupportNative.Desktop.csproj `
  --configuration Debug --no-restore
```

En la vista `Asistente`, el cuadro inferior queda habilitado. Las opciones
deterministas existentes siguen disponibles y son las unicas que pueden avanzar
hacia propuesta, confirmacion y solicitud sintetica.

La app espera hasta 120 segundos por cada respuesta de Hermes local. Ese limite
esta del lado de WinUI y da margen para que Hermes use skills locales de
validacion antes de responder.

## Arranque sin terminal

No crees un `.env` dentro del repositorio. Para evitar repetir variables en cada
terminal de desarrollo, puedes persistirlas en el perfil de usuario de Windows.
Esto sigue siendo solo `local-demo`; la clave queda fuera de Git y debe retirarse
cuando termine la demo.

```powershell
[Environment]::SetEnvironmentVariable(
  'IT_SUPPORT_HERMES_CHAT_ENABLED',
  'true',
  'User')
[Environment]::SetEnvironmentVariable(
  'IT_SUPPORT_HERMES_BASE_URL',
  'http://127.0.0.1:8765/v1',
  'User')
[Environment]::SetEnvironmentVariable(
  'IT_SUPPORT_HERMES_MODEL',
  'it-support',
  'User')

$secureKey = Read-Host 'API key local de Hermes' -AsSecureString
$keyHandle = [Runtime.InteropServices.Marshal]::SecureStringToBSTR($secureKey)
try {
  $plainKey = [Runtime.InteropServices.Marshal]::PtrToStringBSTR($keyHandle)
  [Environment]::SetEnvironmentVariable(
    'IT_SUPPORT_HERMES_API_KEY',
    $plainKey,
    'User')
}
finally {
  if ($keyHandle -ne [IntPtr]::Zero) {
    [Runtime.InteropServices.Marshal]::ZeroFreeBSTR($keyHandle)
  }
}
```

Cierra y vuelve a abrir la app para que el nuevo proceso herede esas variables.
Si la app se lanza desde una terminal ya abierta, abre una terminal nueva.

## Apagar Hermes

Cierra la app y elimina las variables de la sesion:

```powershell
Remove-Item Env:IT_SUPPORT_HERMES_CHAT_ENABLED -ErrorAction SilentlyContinue
Remove-Item Env:IT_SUPPORT_HERMES_BASE_URL -ErrorAction SilentlyContinue
Remove-Item Env:IT_SUPPORT_HERMES_MODEL -ErrorAction SilentlyContinue
Remove-Item Env:IT_SUPPORT_HERMES_API_KEY -ErrorAction SilentlyContinue
```

Si las guardaste en el perfil de usuario de Windows, retiralas tambien:

```powershell
@(
  'IT_SUPPORT_HERMES_CHAT_ENABLED',
  'IT_SUPPORT_HERMES_BASE_URL',
  'IT_SUPPORT_HERMES_MODEL',
  'IT_SUPPORT_HERMES_API_KEY'
) | ForEach-Object {
  [Environment]::SetEnvironmentVariable($_, $null, 'User')
}
```

Con Hermes apagado o mal configurado, el campo de texto queda deshabilitado y la
vista conserva las opciones deterministas.

## Limites

- Solo se aceptan endpoints `http` o `https` de loopback.
- El texto enviado se acota a 1000 caracteres.
- No se envia historial completo, diagnosticos, tokens de la app, evidencia,
  credenciales, hostnames ni datos corporativos.
- Los errores de red, timeout de 120 segundos o payload vacio degradan a
  mensaje informativo.
- Esta configuracion no es inferencia offline ni integracion productiva de IA.
