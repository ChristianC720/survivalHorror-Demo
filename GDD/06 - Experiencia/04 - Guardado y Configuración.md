# Guardado y Configuración

## Descripción de la sección

Define cómo se conserva el progreso y qué opciones puede modificar el jugador.

Reglas completas del guardado en S-07 ([[03 - Sistemas]]) y en [[07 - Reglas y Estados del Juego]].

## Guardado

**Tipo:** Automático. No existe guardado manual (D-30).

**Cuándo se guarda:**

- **Autoguardado del intento:** al llegar a la sala clave final de los pisos 1 y 2 sin enemigos en caza, o al terminar la caza (D-30, D-96); y al empezar la decisión final (D-92).
- **Terminal y fallas:** cada vez que cambian (documentos, bestiario, objetos confiscados, fallas).
- **Configuración:** al salir de Opciones.
- Aviso diegético "sincronizando…" en el terminal.

**Qué datos se guardan:**

- **Por ranura:** terminal del brazo (registros, documentos de Emilia y su estado), fallas y parches, código de sujeto, contador de muertes, degradación, modo, marca de primer intento, autoguardado del intento (piso actual, inventario y estado al llegar; el piso se regenera con una semilla nueva al continuar, D-101) y marca de salida confirmada.
- **Global (no se borra con Nueva partida):** configuración, calibración de brillo y registro de cinemáticas vistas (D-91).

**Cantidad de ranuras:** 3 (D-105). Continuar carga la última ranura usada; Nueva partida y Cargar partida eligen ranura ([[02 - UI y Menús]]).

**Qué ocurre si el guardado falla:** el juego escribe primero en un archivo temporal y solo reemplaza el guardado anterior si la escritura termina bien, así que un fallo nunca deja la ranura corrupta. Si falla, el terminal muestra `SYNC FAILED`, se conserva el guardado anterior y se reintenta en el siguiente cambio. Si un archivo está dañado al cargar, la ranura aparece como dañada y se puede borrar.

## Configuración

| Categoría | Opciones |
|---|---|
| Audio | Volumen general, música, efectos, ambiente y voces; salida estéreo o auriculares |
| Video | Resolución, modo de pantalla (completa, ventana, sin bordes), V-Sync, límite de FPS, brillo (calibración), calidad de sombras |
| Controles | Reasignación de teclado y ratón; sensibilidad para mirar y apuntar; invertir eje vertical; agacharse en modo alternar o mantener; asistencia de apuntado (D-104) |
| Accesibilidad | Tamaño y fondo de subtítulos, escala del terminal, subtítulos de sonido (D-102), reducción de movimiento, de destellos y de efectos CRT y glitch (D-106) |
| Jugabilidad | Idioma (inglés o español); el modo se elige al crear la partida y no se cambia aquí (D-103) |

## Restablecimiento

Cada sección de Opciones tiene "Restablecer valores predeterminados", con confirmación. Restablecer no toca las partidas ni el registro de cinemáticas vistas. Los controles reasignados se restablecen todos a la vez.


> **Navegación:** [[00 - Índice]] · ← [[03 - Accesibilidad]] · [[01 - Arquitectura Técnica]] →
