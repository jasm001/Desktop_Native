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
$validationArtifacts = Join-Path $repositoryRoot '.artifacts\validate'

Push-Location $repositoryRoot
try {
    & $dotnet restore ITSupportNative.slnx --locked-mode -m:1 --disable-build-servers
    if ($LASTEXITCODE -ne 0) { throw 'dotnet restore failed.' }

    & $dotnet format ITSupportNative.slnx --verify-no-changes --no-restore
    if ($LASTEXITCODE -ne 0) { throw 'dotnet format failed.' }

    & $dotnet restore ITSupportNative.slnx --locked-mode -m:1 `
        --artifacts-path $validationArtifacts --disable-build-servers
    if ($LASTEXITCODE -ne 0) { throw 'isolated dotnet restore failed.' }

    & $dotnet build ITSupportNative.slnx --configuration Release --no-restore -m:1 `
        --artifacts-path $validationArtifacts --disable-build-servers
    if ($LASTEXITCODE -ne 0) { throw 'dotnet build failed.' }

    & $dotnet test ITSupportNative.slnx --configuration Release --no-build -m:1 `
        --artifacts-path $validationArtifacts --disable-build-servers
    if ($LASTEXITCODE -ne 0) { throw 'dotnet test failed.' }

    corepack pnpm@11.5.3 install --frozen-lockfile
    if ($LASTEXITCODE -ne 0) { throw 'pnpm install failed.' }

    corepack pnpm@11.5.3 run check
    if ($LASTEXITCODE -ne 0) { throw 'pnpm checks failed.' }

    corepack pnpm@11.5.3 run test:integration
    if ($LASTEXITCODE -ne 0) { throw 'PostgreSQL integration tests failed.' }

    & (Join-Path $PSScriptRoot 'Test-Secrets.ps1')
}
finally {
    Pop-Location
}
