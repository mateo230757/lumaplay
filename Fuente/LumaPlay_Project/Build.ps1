param(
    [string]$OutputDirectory = (Join-Path $PSScriptRoot "publish")
)

$ErrorActionPreference = "Stop"
$project = Join-Path $PSScriptRoot "src\LumaPlay.csproj"

if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) {
    throw "Para compilar como desarrollador necesitas instalar el SDK de .NET 10. Los usuarios normales NO necesitan instalarlo: reciben el programa ya compilado dentro del instalador."
}

New-Item -ItemType Directory -Force -Path $OutputDirectory | Out-Null
Write-Host "Publicando LumaPlay como aplicación autónoma de Windows x64..." -ForegroundColor Cyan
Write-Host "El runtime .NET se incluye en el ejecutable. No se usa el compilador del PC del usuario." -ForegroundColor Gray

& dotnet publish $project --configuration Release --runtime win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -p:EnableCompressionInSingleFile=true -p:DebugType=embedded --output $OutputDirectory

if ($LASTEXITCODE -ne 0) {
    throw "Falló la publicación. Revisa los errores de dotnet publish."
}

$exe = Join-Path $OutputDirectory "LumaPlay.exe"
if (-not (Test-Path $exe)) {
    throw "La compilación terminó sin producir LumaPlay.exe."
}
Write-Host "Listo: $exe" -ForegroundColor Green
