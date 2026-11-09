# Guardado y Configuración

## 1. Descripción de la sección

Define cómo se conserva el progreso y qué opciones puede modificar el jugador.

Las reglas completas del guardado están en Persistencia, en [[03 - Sistemas]], y en la regla global 3 de [[07 - Reglas y Estados del Juego]]. El formato de los archivos está en [[04 - Datos y Persistencia]]. Los valores numéricos de este documento son iniciales y están sujetos a balance.

## 2. Guardado

**Tipo:** Automático. No existe guardado manual.

**Cuándo se guarda:**

| Dato | Momento |
|---|---|
| Autoguardado del intento | Al llegar a la sala clave final de los pisos 1 y 2 sin enemigos en caza, o al terminar la caza. También al empezar la decisión final |
| Inicio de piso | Al empezar cada piso, con el inventario de llegada |
| Terminal y fallas | Cada vez que cambian, con documentos, bestiario, objetos confiscados y fallas |
| Configuración | Al salir de Opciones |

Cada guardado muestra el aviso diegético `sincronizando…` en el terminal.

**Qué datos se guardan:**

- **Por ranura.** El terminal del brazo, con los registros, los documentos de Emilia y su estado. También las fallas y sus parches, el código de sujeto, el contador de muertes, la degradación, el modo y la marca de primer intento.
- **Autoguardado del intento, por ranura.** El piso actual, el inventario y el estado al llegar, más la marca de salida confirmada. El piso se regenera con una semilla nueva al continuar.
- **Global.** La configuración, la calibración de brillo y el registro de cinemáticas vistas. `Nueva partida` no los borra.

**Cantidad de ranuras:** 3. `Continuar` carga la última ranura usada, y `Nueva partida` y `Cargar partida` eligen una ranura. Ver [[02 - UI y Menús]].

**Qué ocurre si el guardado falla:**

- El juego escribe primero en un archivo temporal y solo reemplaza el guardado anterior si la escritura termina bien. Así un fallo nunca deja la ranura corrupta.
- Si la escritura falla, el terminal muestra `SYNC FAILED`, se conserva el guardado anterior y el juego reintenta en el siguiente cambio.
- Si un archivo está dañado al cargar, la ranura aparece como dañada y se puede borrar.

## 3. Configuración

| Categoría | Opciones |
|---|---|
| Audio | Volumen general, música, efectos, ambiente y voces |
| Video | Resolución, modo de pantalla completa, ventana o sin bordes, V-Sync, límite de FPS, brillo con la calibración y calidad de sombras |
| Controles | Reasignación de teclado y mouse, sensibilidad para mirar y para apuntar, eje vertical invertido, agacharse en modo alternar o mantener y asistencia de apuntado |
| Accesibilidad | Tamaño y fondo de subtítulos, escala del terminal, subtítulos de sonido y reducción de movimiento, de destellos y de efectos CRT y glitch |
| Jugabilidad | Idioma, inglés o español. El modo se elige al crear la partida y no se cambia aquí |

En el juego, la categoría Jugabilidad aparece como la sección Juego de Opciones. El mando tipo Xbox usa un mapa fijo sin reasignación en la demo. Ver [[04 - Controles]].

## 4. Restablecimiento

Cada sección de Opciones tiene `Restablecer valores predeterminados`, con confirmación. Restablecer no toca las partidas ni el registro de cinemáticas vistas. Los controles reasignados se restablecen todos a la vez.

> **Navegación:** [[00 - Índice]] · ← [[03 - Accesibilidad]] · [[01 - Arquitectura Técnica]] →
