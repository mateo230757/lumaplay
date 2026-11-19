# Compilar y mantener LumaPlay

LumaPlay usa C# como lenguaje de programación y Windows Forms para la interfaz.
Esto no significa que los usuarios deban instalar C# o un compilador.

## Herramientas del desarrollador

- .NET 10 SDK.
- Windows 10/11 x64.
- Inno Setup 6 si vas a generar el instalador localmente.

## Publicar la aplicación

Ejecuta Build.ps1. La salida es la carpeta publish, con LumaPlay.exe
self-contained y single-file. No actives trimming de forma indiscriminada
en Windows Forms.

## Generar el Setup

1. Ejecuta Build.ps1.
2. Abre installer/LumaPlay.iss con Inno Setup 6 o compílalo con ISCC.exe.
3. La salida se genera en artifacts/LumaPlay_Setup.exe.

GitHub Actions realiza esos pasos y publica el instalador en la release latest.

## Uso por parte de los usuarios

El usuario final solo descarga y ejecuta LumaPlay_Setup.exe. No instala SDK,
compilador, .NET Framework ni runtime separado.
