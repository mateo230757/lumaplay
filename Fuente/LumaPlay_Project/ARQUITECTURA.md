# Arquitectura de LumaPlay 1.1.0

## Objetivo

El usuario descarga un instalador gráfico único y obtiene una aplicación
precompilada para Windows. No compila código C# ni instala herramientas
de desarrollo en su PC.

## Tecnologías

- C# como lenguaje de implementación.
- Windows Forms para la interfaz.
- .NET Framework 4.8.
- System.Web.Extensions para serialización JSON.
- Inno Setup 6 para el asistente de instalación.
- GitHub Actions en runner Windows para compilar el ejecutable y generar Setup.

## Componentes de la aplicación

- MainForm.cs: ventana principal y navegación.
- SetupForm.cs: creación del perfil en el primer inicio.
- Theme.cs: paleta y paneles de interfaz.
- GameCanvas.cs: juego Neon Drift.
- AppSettings.cs: perfil, biblioteca y estadísticas en JSON.
- Program.cs: inicialización de Windows Forms y carga del perfil.

## Datos

- Archivo: %LOCALAPPDATA%\LumaPlay\settings.json.
- Los juegos externos se representan mediante rutas locales a sus ejecutables.
- No se copian, modifican ni descargan juegos externos.
- El formato JSON mantiene nombres de propiedades compatibles con la versión 1.0.

## Flujo de compilación y distribución

1. El desarrollador actualiza el repositorio.
2. GitHub Actions utiliza un runner Windows con .NET Framework y csc.exe.
3. Build.ps1 compila el código fuente y produce LumaPlay.exe.
4. Inno Setup empaqueta el ejecutable en LumaPlay_Setup.exe.
5. El pipeline calcula SHA-256 y publica el instalador en GitHub Releases.
6. La página web enlaza al asset estable de la última release.

El compilador se utiliza solo en el entorno de desarrollo/CI. Nunca se ejecuta
en el PC del usuario ni se incluye como requisito del instalador.

## Asistente de instalación

Inno Setup proporciona páginas visuales:
- Bienvenida.
- Licencia.
- Selección de carpeta.
- Opciones de accesos directos.
- Resumen y confirmación.
- Barra de progreso.
- Finalización con opción de iniciar LumaPlay.

Los botones Siguiente, Atrás y Cancelar los proporciona el propio asistente.

## Runtime

La aplicación utiliza .NET Framework 4.8. Windows 10 22H2 incluye .NET
Framework 4.8 y Windows 11 trae una versión posterior compatible. Por eso,
para esos sistemas operativos no hace falta instalar un runtime aparte.

## Desinstalación

Inno Setup registra LumaPlay en Aplicaciones instaladas de Windows y proporciona
su propio desinstalador. Los datos del usuario en LocalAppData se conservan.

## Alcance funcional

Esta edición es una plataforma local: no promete tienda, cuentas de nube,
pagos, chat, multijugador, hosting ni servicios de descarga de terceros.
