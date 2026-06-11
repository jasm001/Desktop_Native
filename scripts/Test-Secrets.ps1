[CmdletBinding()]
param()

$ErrorActionPreference = 'Stop'

$localGitleaks = Join-Path $PSScriptRoot '..\.tools\gitleaks\gitleaks.exe'
$gitleaks = if (Test-Path $localGitleaks) {
    Resolve-Path $localGitleaks
}
else {
    (Get-Command gitleaks -ErrorAction Stop).Source
}

& $gitleaks dir (Join-Path $PSScriptRoot '..') --no-banner --redact
if ($LASTEXITCODE -ne 0) {
    throw "Gitleaks failed with exit code $LASTEXITCODE."
}
