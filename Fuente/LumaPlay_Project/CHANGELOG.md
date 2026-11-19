# Registro de cambios

## 1.1.0 — Instalación autónoma para Windows

- Migración de .NET Framework 4.8 a .NET 10 para Windows.
- Sustitución de JavaScriptSerializer por System.Text.Json.
- Publicación self-contained y single-file para Windows x64.
- El runtime .NET queda incluido en la aplicación publicada.
- Eliminada la compilación C# durante la instalación del usuario.
- Eliminada la dependencia del compilador csc.exe en el PC del usuario.
- Nuevo asistente gráfico de Inno Setup con Siguiente / Atrás.
- Página de licencia, carpeta de instalación, tareas de accesos directos,
  progreso y opción de iniciar la aplicación al finalizar.
- Desinstalación integrada en Windows; se conservan los datos del perfil.
- Pipeline de GitHub Actions que compila y publica LumaPlay_Setup.exe.
- Página web enlazada a la descarga estable de GitHub Releases.

## 1.0.0 — Primera edición

- Aplicación de escritorio Windows en C# / Windows Forms.
- Asistente de primer inicio para perfil y color de acento.
- Pantallas Inicio, Descubrir, Biblioteca, Logros, Ajustes y Acerca de.
- Biblioteca para registrar y lanzar archivos .exe existentes.
- Juego integrado Neon Drift con pausa, colisiones, puntuación y récord.
- Persistencia JSON local.
- Sitio web estático y demo del arcade en navegador.
