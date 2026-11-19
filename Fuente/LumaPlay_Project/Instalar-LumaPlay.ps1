$ErrorActionPreference = "Stop"
$projectRoot = $PSScriptRoot

Write-Host ""
Write-Host "  LUMAPLAY  |  ASISTENTE DE INSTALACIÓN" -ForegroundColor Magenta
Write-Host "  --------------------------------------" -ForegroundColor DarkGray
Write-Host "Este asistente compilará la aplicación en este equipo y preparará sus accesos." -ForegroundColor Gray
Write-Host ""

$defaultPath = Join-Path $env:LOCALAPPDATA "Programs\LumaPlay"
$chosenPath = Read-Host "Carpeta principal de instalación [$defaultPath]"
if ([string]::IsNullOrWhiteSpace($chosenPath)) { $chosenPath = $defaultPath }
$installPath = [IO.Path]::GetFullPath([Environment]::ExpandEnvironmentVariables($chosenPath.Trim().Trim('"')))

$shortcutAnswer = Read-Host "¿Crear un acceso directo en el Escritorio? (S/n) [S]"
$createDesktop = [string]::IsNullOrWhiteSpace($shortcutAnswer) -or $shortcutAnswer.Trim().ToLowerInvariant() -ne "n"
$shortcutName = "LumaPlay"
if ($createDesktop) {
    $typedName = Read-Host "Nombre del acceso directo [$shortcutName]"
    if (-not [string]::IsNullOrWhiteSpace($typedName)) { $shortcutName = $typedName.Trim() }
    $shortcutName = [IO.Path]::GetFileNameWithoutExtension($shortcutName)
    foreach ($invalidChar in [IO.Path]::GetInvalidFileNameChars()) { $shortcutName = $shortcutName.Replace([string]$invalidChar, "_") }
    if ([string]::IsNullOrWhiteSpace($shortcutName)) { $shortcutName = "LumaPlay" }
}
$startMenuAnswer = Read-Host "¿Crear también un acceso en el menú Inicio? (S/n) [S]"
$createStartMenu = [string]::IsNullOrWhiteSpace($startMenuAnswer) -or $startMenuAnswer.Trim().ToLowerInvariant() -ne "n"

try {
    New-Item -ItemType Directory -Force -Path $installPath | Out-Null
    $buildTemp = Join-Path $env:TEMP ("LumaPlay-build-" + [Guid]::NewGuid().ToString("N"))
    & (Join-Path $projectRoot "Build.ps1") -OutputDirectory $buildTemp
    if ($LASTEXITCODE -ne 0) { throw "No se pudo compilar LumaPlay." }

    Copy-Item -Path (Join-Path $buildTemp "LumaPlay.exe") -Destination (Join-Path $installPath "LumaPlay.exe") -Force
    Copy-Item -Path (Join-Path $projectRoot "Desinstalar-LumaPlay.ps1") -Destination (Join-Path $installPath "Desinstalar-LumaPlay.ps1") -Force
    Copy-Item -Path (Join-Path $projectRoot "LEEME.txt") -Destination (Join-Path $installPath "LEEME.txt") -Force
    @(
        "DesktopCreated=$([int]$createDesktop)",
        "DesktopShortcut=$shortcutName",
        "StartMenuCreated=$([int]$createStartMenu)"
    ) | Set-Content -Path (Join-Path $installPath "LumaPlay.install.txt") -Encoding UTF8
    Remove-Item -Path $buildTemp -Recurse -Force -ErrorAction SilentlyContinue

    $exePath = Join-Path $installPath "LumaPlay.exe"
    $shell = New-Object -ComObject WScript.Shell
    if ($createDesktop) {
        $desktop = [Environment]::GetFolderPath("Desktop")
        $shortcut = $shell.CreateShortcut((Join-Path $desktop ($shortcutName + ".lnk")))
        $shortcut.TargetPath = $exePath
        $shortcut.WorkingDirectory = $installPath
        $shortcut.IconLocation = $exePath
        $shortcut.Description = "Abrir LumaPlay"
        $shortcut.Save()
    }
    if ($createStartMenu) {
        $menu = Join-Path ([Environment]::GetFolderPath("Programs")) "LumaPlay"
        New-Item -ItemType Directory -Force -Path $menu | Out-Null
        $shortcut = $shell.CreateShortcut((Join-Path $menu "LumaPlay.lnk"))
        $shortcut.TargetPath = $exePath
        $shortcut.WorkingDirectory = $installPath
        $shortcut.IconLocation = $exePath
        $shortcut.Description = "Abrir LumaPlay"
        $shortcut.Save()
    }

    Write-Host ""
    Write-Host "¡LumaPlay quedó instalado!" -ForegroundColor Green
    Write-Host "Ubicación: $installPath" -ForegroundColor Gray
    $launchNow = Read-Host "¿Abrir LumaPlay ahora? (S/n) [S]"
    if ([string]::IsNullOrWhiteSpace($launchNow) -or $launchNow.Trim().ToLowerInvariant() -ne "n") {
        Start-Process -FilePath $exePath
    }
}
catch {
    Write-Host "ERROR: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host "No se completó la instalación. No se eliminaron tus juegos ni archivos personales." -ForegroundColor Yellow
    Read-Host "Presiona ENTER para cerrar esta ventana" | Out-Null
    exit 1
}
