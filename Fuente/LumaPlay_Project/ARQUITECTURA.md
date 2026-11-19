# Arquitectura de LumaPlay 1.0.0

## Objetivo de esta edición

LumaPlay 1.0.0 es una aplicación de escritorio local. La prioridad es entregar una base real y ejecutable en Windows: navegación, biblioteca, persistencia, instalación guiada y un juego integrado. La interfaz no simula servicios de red que todavía no existen.

## Capas

### Interfaz

- `MainForm.cs`: ventana principal y navegación entre Inicio, Descubrir, Biblioteca, Logros, Ajustes y Acerca de.
- `SetupForm.cs`: primer inicio y creación del perfil local.
- `Theme.cs`: colores de la interfaz y paneles degradados.
- `GameCanvas.cs`: superficie de juego, bucle de actualización, entrada de teclado, colisiones y dibujo de Neon Drift.

### Datos

- `AppSettings.cs`: perfil, color de acento, récord, número de partidas y biblioteca.
- Serialización JSON mediante `JavaScriptSerializer`.
- Ubicación: `%LOCALAPPDATA%\LumaPlay\settings.json`.
- Los juegos externos se representan mediante una ruta de ejecutable; no se duplican sus carpetas ni recursos.

### Instalación

- `Instalar-LumaPlay.ps1`: recoge la ubicación, crea accesos y copia el ejecutable compilado.
- `Build.ps1`: compila el código C# con el compilador de .NET Framework.
- `Desinstalar-LumaPlay.ps1`: retira los archivos de LumaPlay y pregunta por separado si se borran los datos del perfil.

## Flujo de primer inicio

1. El usuario extrae el paquete.
2. El instalador pregunta carpeta, nombre del acceso directo y destinos.
3. El instalador compila `LumaPlay.exe`.
4. La aplicación abre `SetupForm` si no existe un perfil inicializado.
5. El usuario elige nombre y color de acento.
6. `MainForm` carga las secciones y `AppSettings` guarda los cambios.

## Añadir un juego externo

1. El usuario selecciona `Añadir juego`.
2. Un selector de archivos filtra ejecutables `.exe`.
3. Se guarda ruta, nombre, descripción local y estadísticas básicas.
4. `Process.Start` abre el ejecutable cuando se pulsa `Abrir`.
5. Quitar el elemento elimina únicamente el registro de LumaPlay.

**Nota:** un juego externo puede necesitar su launcher oficial, permisos o dependencias propias. La biblioteca no descarga ni instala el juego.

## Neon Drift

- Cinco carriles discretos.
- Obstáculos generados progresivamente.
- Movimiento mediante flechas o A/D.
- ESPACIO inicia o pausa; ESC vuelve a la biblioteca.
- Puntuación, récord persistente y logros calculados a partir de estadísticas.
- La demo web es una implementación independiente del mismo concepto, con récord guardado en `localStorage` del navegador.

## Evolución posible

| Versión | Área | Trabajo necesario |
|---|---|---|
| 1.1 | Biblioteca | Portadas, categorías, favoritos, etiquetas y búsqueda avanzada. |
| 1.2 | Experiencia | Actualizador con versiones firmadas, instalador visual y copias de seguridad de perfil. |
| 2.0 | Cuentas | Servicio de identidad, API, base de datos, sincronización y recuperación de cuenta. |
| 2.1 | Catálogo | Servicio de catálogo, almacenamiento de paquetes, verificación de hashes y canal de actualizaciones. |
| 3.0 | Comunidad | Publicación de juegos, revisión de contenido, permisos, reportes y moderación. |
| 4.0 | Multijugador | Servicios de sesión, emparejamiento, relay/servidores dedicados, antitrampas y observabilidad. |

## Requisitos de una plataforma en línea real

Una plataforma del tamaño de Steam o Roblox necesita infraestructura de backend, cuentas, seguridad, pagos (si se venden títulos), CDN, distribución de archivos, actualizaciones, moderación, protección de datos, soporte y operación continua. Ninguno de esos servicios se finge en esta edición local.
