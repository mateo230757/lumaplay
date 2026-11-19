param(
    [string]$OutputDirectory = (Join-Path $PSScriptRoot "dist")
)

$ErrorActionPreference = "Stop"
$source = Join-Path $PSScriptRoot "src"
$compilerCandidates = @(
    (Join-Path $env:WINDIR "Microsoft.NET\Framework64\v4.0.30319\csc.exe"),
    (Join-Path $env:WINDIR "Microsoft.NET\Framework\v4.0.30319\csc.exe")
)
$csc = $compilerCandidates | Where-Object { Test-Path $_ } | Select-Object -First 1

if (-not $csc) {
    Write-Host "No se encontró el compilador de C# de .NET Framework 4.x." -ForegroundColor Red
    Write-Host "Instala Visual Studio Build Tools con el paquete de destino .NET Framework 4.8 o el Developer Pack oficial." -ForegroundColor Yellow
    Write-Host "Referencia: https://dotnet.microsoft.com/download/dotnet-framework/net48" -ForegroundColor Cyan
    throw "Falta el compilador de C# de .NET Framework."
}

New-Item -ItemType Directory -Force -Path $OutputDirectory | Out-Null
$exe = Join-Path $OutputDirectory "LumaPlay.exe"
$icon = Join-Path $source "Assets\lumaplay.ico"
$files = @(
    (Join-Path $source "Program.cs"),
    (Join-Path $source "AppSettings.cs"),
    (Join-Path $source "Theme.cs"),
    (Join-Path $source "SetupForm.cs"),
    (Join-Path $source "MainForm.cs"),
    (Join-Path $source "GameCanvas.cs")
)

$compilerArgs = @(
    "/nologo",
    "/target:winexe",
    "/platform:anycpu",
    "/optimize+",
    "/debug-",
    "/out:$exe",
    "/reference:System.dll",
    "/reference:System.Core.dll",
    "/reference:System.Drawing.dll",
    "/reference:System.Windows.Forms.dll",
    "/reference:System.Web.Extensions.dll"
)
if (Test-Path $icon) { $compilerArgs += "/win32icon:$icon" }
$compilerArgs += $files

Write-Host "Compilando LumaPlay..." -ForegroundColor Cyan
& $csc @compilerArgs
if ($LASTEXITCODE -ne 0 -or -not (Test-Path $exe)) {
    Write-Host "La compilación falló. Revisa los mensajes anteriores." -ForegroundColor Red
    throw "Falló la compilación de LumaPlay."
}
Write-Host "Compilación completada: $exe" -ForegroundColor Green
