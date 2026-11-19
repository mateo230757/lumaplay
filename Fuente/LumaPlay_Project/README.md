# LumaPlay 1.1.0 — Windows

LumaPlay es una aplicación local de biblioteca de videojuegos y arcade integrado.

## Distribución para usuarios

Descarga LumaPlay_Setup.exe desde la sección Releases del repositorio.
El instalador gráfico de Inno Setup guía al usuario con páginas Siguiente/Atrás,
licencia, carpeta de destino, tareas, progreso y finalización.

La aplicación se compila previamente en el servidor de CI. El instalador incluye
el ejecutable listo para abrir. El usuario no necesita instalar C#, Visual
Studio, Build Tools ni un compilador.

El requisito de ejecución es Windows 10 22H2 o Windows 11, que ya incluye
.NET Framework 4.8 o una versión superior compatible. No se descarga un runtime
separado durante la instalación.

## Arquitectura

- C# y Windows Forms para la interfaz.
- .NET Framework 4.8.
- System.Web.Extensions para persistencia JSON.
- Inno Setup 6 para crear el instalador gráfico.
- GitHub Actions compila la aplicación y publica el Setup.

## Compilar como desarrollador

La compilación usa el compilador C# disponible en el entorno de desarrollo.
Ejecuta Build.ps1. La salida es la carpeta publish.

## Crear el instalador

Inno Setup 6 compila installer/LumaPlay.iss, usando el ejecutable de publish.
GitHub Actions realiza ambos pasos y publica LumaPlay_Setup.exe en la release
latest.

## Datos y desinstalación

La configuración se guarda en %LOCALAPPDATA%\LumaPlay. El desinstalador
elimina la aplicación, pero conserva los datos locales del usuario.
