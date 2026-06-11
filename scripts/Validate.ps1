[CmdletBinding()]
param()

$ErrorActionPreference = 'Stop'
$repositoryRoot = Resolve-Path (Join-Path $PSScriptRoot '..')
$localDotnet = Join-Path $repositoryRoot '.dotnet\dotnet.exe'
$dotnet = if (Test-Path $localDotnet) {
    $localDotnet
}
else {
    (Get-Command dotnet -ErrorAction Stop).Source
}

$env:DOTNET_CLI_HOME = Join-Path $repositoryRoot '.dotnet-home'
$env:DOTNET_NOLOGO = '1'
$env:DOTNET_SKIP_FIRST_TIME_EXPERIENCE = '1'
$env:NUGET_PACKAGES = Join-Path $repositoryRoot '.packages\nuget'

Push-Location $repositoryRoot
try {
    & $dotnet restore ITSupportNative.slnx --locked-mode
    if ($LASTEXITCODE -ne 0) { throw 'dotnet restore failed.' }

    & $dotnet format ITSupportNative.slnx --verify-no-changes --no-restore
    if ($LASTEXITCODE -ne 0) { throw 'dotnet format failed.' }

    & $dotnet build ITSupportNative.slnx --configuration Release --no-restore
    if ($LASTEXITCODE -ne 0) { throw 'dotnet build failed.' }

    & $dotnet test ITSupportNative.slnx --configuration Release --no-build
    if ($LASTEXITCODE -ne 0) { throw 'dotnet test failed.' }

    corepack pnpm@11.5.3 install --frozen-lockfile
    if ($LASTEXITCODE -ne 0) { throw 'pnpm install failed.' }

    corepack pnpm@11.5.3 run check
    if ($LASTEXITCODE -ne 0) { throw 'pnpm checks failed.' }

    & (Join-Path $PSScriptRoot 'Test-Secrets.ps1')
}
finally {
    Pop-Location
}
