$ErrorActionPreference = "Stop"
$programPath = $PSScriptRoot
$dataFolder = Join-Path $env:LOCALAPPDATA "LumaPlay"
$desktopCreated = $false
$desktopName = "LumaPlay"
$startMenuCreated = $false
$installInfo = Join-Path $programPath "LumaPlay.install.txt"
if (Test-Path $installInfo) {
    foreach ($line in Get-Content $installInfo) {
        if ($line -match '^DesktopCreated=(.*)$') { $desktopCreated = ($Matches[1] -eq "1") }
        elseif ($line -match '^DesktopShortcut=(.*)$') { $desktopName = $Matches[1] }
        elseif ($line -match '^StartMenuCreated=(.*)$') { $startMenuCreated = ($Matches[1] -eq "1") }
    }
}
$desktopShortcut = Join-Path ([Environment]::GetFolderPath("Desktop")) ($desktopName + ".lnk")
$startMenuFolder = Join-Path ([Environment]::GetFolderPath("Programs")) "LumaPlay"

Write-Host "DESINSTALAR LUMAPLAY" -ForegroundColor Magenta
$confirm = Read-Host "¿Eliminar los archivos de la aplicación de $programPath? (s/N)"
if ($confirm.Trim().ToLowerInvariant() -ne "s") { Write-Host "Operación cancelada."; exit 0 }

if ($desktopCreated -and (Test-Path $desktopShortcut)) { Remove-Item $desktopShortcut -Force }
if ($startMenuCreated -and (Test-Path $startMenuFolder)) { Remove-Item $startMenuFolder -Recurse -Force }
foreach ($fileName in @("LumaPlay.exe", "Desinstalar-LumaPlay.ps1", "LEEME.txt", "LumaPlay.install.txt")) {
    $filePath = Join-Path $programPath $fileName
    if (Test-Path $filePath) { Remove-Item $filePath -Force }
}
if (Test-Path $programPath) {
    $remaining = Get-ChildItem -LiteralPath $programPath -Force -ErrorAction SilentlyContinue
    if (($remaining | Measure-Object).Count -eq 0) { Remove-Item -LiteralPath $programPath -Force }
}

$removeData = Read-Host "¿Eliminar también el perfil, récords y ajustes locales? (s/N)"
if ($removeData.Trim().ToLowerInvariant() -eq "s" -and (Test-Path $dataFolder)) {
    Remove-Item $dataFolder -Recurse -Force
}
Write-Host "Desinstalación completada." -ForegroundColor Green
