param(
    [string]$OutputDirectory = (Join-Path $PSScriptRoot "publish")
)

$ErrorActionPreference = "Stop"
$source = Join-Path $PSScriptRoot "src"
$compilerCandidates = @(
    (Join-Path $env:WINDIR "Microsoft.NET\Framework64\v4.0.30319\csc.exe"),
    (Join-Path $env:WINDIR "Microsoft.NET\Framework\v4.0.30319\csc.exe")
)
$csc = $compilerCandidates | Where-Object { Test-Path $_ } | Select-Object -First 1

if (-not $csc) {
    throw "No se encontró csc.exe en el entorno de compilación. El usuario final no necesita el compilador."
}

New-Item -ItemType Directory -Force -Path $OutputDirectory | Out-Null
$exe = Join-Path $OutputDirectory "LumaPlay.exe"
$icon = Join-Path $source "Assets\lumaplay.ico"
$manifest = Join-Path $PSScriptRoot "app.manifest"
$files = @(
    (Join-Path $source "Program.cs"),
    (Join-Path $source "AppSettings.cs"),
    (Join-Path $source "Theme.cs"),
    (Join-Path $source "SetupForm.cs"),
    (Join-Path $source "MainForm.cs"),
    (Join-Path $source "GameCanvas.cs")
)

if (-not (Test-Path $manifest)) {
    throw "No se encontró app.manifest. Es necesario para declarar la compatibilidad de Windows."
}

$compilerArgs = @(
    "/nologo",
    "/target:winexe",
    "/platform:anycpu",
    "/optimize+",
    "/debug-",
    "/out:$exe",
    "/win32manifest:$manifest",
    "/reference:System.dll",
    "/reference:System.Core.dll",
    "/reference:System.Drawing.dll",
    "/reference:System.Windows.Forms.dll",
    "/reference:System.Web.Extensions.dll"
)
if (Test-Path $icon) { $compilerArgs += "/win32icon:$icon" }
$compilerArgs += $files

Write-Host "Compilando LumaPlay 1.1.2 con manifiesto explícito de compatibilidad Windows..." -ForegroundColor Cyan
& $csc @compilerArgs
if ($LASTEXITCODE -ne 0 -or -not (Test-Path $exe)) {
    throw "Falló la compilación de LumaPlay."
}
Write-Host "Compilación completada: $exe" -ForegroundColor Green
