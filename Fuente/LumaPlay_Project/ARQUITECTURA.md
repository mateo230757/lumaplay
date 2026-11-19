# Arquitectura de LumaPlay 1.1.0

## Objetivo

Entregar una aplicación Windows x64 instalada como un producto de escritorio:
el usuario descarga un único instalador gráfico y no necesita configurar
herramientas de desarrollo ni instalar el runtime .NET por separado.

## Tecnologías

- C# como lenguaje de implementación.
- Windows Forms para la interfaz de escritorio.
- .NET 10 (net10.0-windows).
- System.Text.Json para configuración JSON.
- Publicación autocontenida win-x64, single-file.
- Inno Setup 6 para el asistente de instalación.
- GitHub Actions en Windows para compilar y publicar el instalador.

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
- El formato JSON mantiene nombres de propiedades compatibles con los datos
  creados por la versión anterior.

## Flujo de compilación y distribución

1. El desarrollador actualiza el repositorio.
2. GitHub Actions usa un runner Windows e instala el SDK de .NET 10.
3. dotnet publish compila LumaPlay para win-x64, self-contained y single-file.
4. Inno Setup empaqueta ese ejecutable en LumaPlay_Setup.exe.
5. El pipeline calcula SHA-256 y publica el instalador en GitHub Releases.
6. La página web enlaza al asset estable de la última release.

El SDK, compilador y herramientas existen únicamente en el entorno de compilación.
No se copian al PC del usuario.

## Asistente de instalación

El instalador Inno Setup tiene el flujo visual estándar:
- Bienvenida.
- Licencia.
- Selección de carpeta.
- Opciones de accesos directos.
- Resumen y confirmación.
- Barra de progreso.
- Finalización con opción de iniciar LumaPlay.

Los botones Siguiente, Atrás y Cancelar los proporciona el propio asistente.

## Runtime .NET

La aplicación se publica como self-contained y single-file. El archivo final
incluye el runtime .NET y las dependencias administradas necesarias. Por ello,
el usuario no tiene que instalar .NET Framework, .NET Runtime ni el SDK.
El paquete es específico para Windows x64.

## Desinstalación

Inno Setup registra LumaPlay en Aplicaciones instaladas de Windows y proporciona
su propio desinstalador. Los datos del usuario en LocalAppData se conservan al
desinstalar, salvo que el usuario decida borrarlos manualmente.

## Alcance funcional

Esta edición es una plataforma local: no promete tienda, cuentas de nube,
pagos, chat, multijugador, hosting ni servicios de descarga de terceros.
