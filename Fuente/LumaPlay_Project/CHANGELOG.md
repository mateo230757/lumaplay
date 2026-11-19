# Registro de cambios

## 1.1.1 — Registro de errores de inicio

- Registro detallado del arranque en %LOCALAPPDATA%\\LumaPlay\\startup.log.
- Captura de excepciones de inicio y errores de Windows Forms.
- Mensaje visible con el tipo de error y la ubicación del registro.

## 1.1.0 — Instalador gráfico y aplicación precompilada

- El ejecutable LumaPlay.exe se compila en GitHub Actions antes de la distribución.
- El usuario ya no compila C# durante la instalación.
- Retirado el requisito de csc.exe en el PC del usuario.
- Instalador gráfico Inno Setup con Siguiente / Atrás.
- Página de licencia, carpeta de instalación y opciones de accesos directos.
- Barra de progreso y opción de iniciar la aplicación al finalizar.
- Desinstalación integrada en Windows; se conservan los datos del perfil.
- Página web enlazada a la descarga estable de GitHub Releases.
- Se mantiene .NET Framework 4.8 para compatibilidad con Windows 10 22H2.

## 1.0.0 — Primera edición

- Aplicación de escritorio Windows en C# / Windows Forms.
- Asistente de primer inicio para perfil y color de acento.
- Pantallas Inicio, Descubrir, Biblioteca, Logros, Ajustes y Acerca de.
- Biblioteca para registrar y lanzar archivos .exe existentes.
- Juego integrado Neon Drift con pausa, colisiones, puntuación y récord.
- Persistencia JSON local.
- Sitio web estático y demo del arcade en navegador.
