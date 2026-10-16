# Interfaz Visual

## Descripción de la sección

Documenta la información que el juego muestra al jugador y las reglas visuales de la interfaz.

**Principio (D-86):** interfaz **diegética**, como en *Signalis*. Los estados se leen en el cuerpo de Lukas, en sus objetos y en el **terminal del brazo**, nunca en indicadores fijos en pantalla. Para que un jugador sin experiencia en el género pueda leerlos, el terminal los muestra con etiquetas de texto claras (por ejemplo `HEALTH: INJURED`). La percepción de los enemigos nunca se muestra (D-02). Flujo de menús en [[02 - UI y Menús]]; accesibilidad en [[03 - Accesibilidad]].

## HUD

No hay HUD permanente. Dónde se lee cada elemento:

- **Vida:** postura y animación de Lukas (M-05) y sección Estado del terminal: electrocardiograma con forma distinta por estado (regular, irregular, errático) más la etiqueta `HEALTH: FINE / INJURED / CRITICAL`.
- **Energía:**
  - **Resistencia:** el jadeo de Lukas (M-06), sin barra.
  - **Carga de la linterna:** barra en el terminal (D-71); en 0, la linterna parpadea.
- **Munición:** cargada, en el contador verde del armazón del revólver, visible al apuntar (D-74); de reserva, en el inventario del terminal.
- **Objetivo:** entorno y líneas de protocolo de The Mother (D-78); contadores de las secundarias en el terminal (Expediente 3/6, Objetos confiscados 5/12).
- **Mapa:** no existe. La vista de vigilancia (M-02) es la única ayuda espacial.
- **Recursos:** inventario de 6 casillas en el terminal (M-07).
- **Indicadores contextuales:** los únicos elementos en pantalla:
  - Retícula de punto al apuntar (M-01).
  - Aviso `E` a ~1.5 m de un interactuable (D-84).
  - Subtítulos y líneas de texto (D-52, D-62).
  - Aviso "sincronizando…" del autoguardado (D-30).

**Ayudas del tutorial:** la primera vez que se usa cada mecánica (D-70), una línea de instrucción de The Mother aparece en el canal de texto de sus líneas de protocolo, con la tecla (por ejemplo `Subject R-H-07: illumination protocol [F]`). Es el mismo canal de texto de D-62, no un indicador nuevo.

**Introducción (D-63):** sin terminal; solo el aviso `E`, los subtítulos y la pantalla del teléfono para el hackeo.

## Jerarquía visual

1. **Amenaza y supervivencia:** la escena iluminada, las siluetas, la postura de Lukas y la retícula. Nada tapa la imagen central.
2. **Interacción inmediata:** aviso `E`, subtítulos y líneas de The Mother (abajo, centrados).
3. **Gestión:** el terminal del brazo, que solo aparece al abrirlo con `Tab` (inventario, estado y registros).

## Estados

| Estado | Cómo se muestra |
|---|---|
| Normal | Terminal teal; texto y electrocardiograma regulares |
| Advertencia | Etiqueta en ámbar más parpadeo (linterna baja, salud Herido) |
| Daño | Vibración de cámara, sonido de impacto, cambio de postura y electrocardiograma (M-05); sin manchas que tapen la pantalla |
| Éxito | Pitido corto y texto `REGISTERED` (documento, objeto confiscado, bestiario) o `ACCESS GRANTED` (hackeo) |
| Error | Pitido de error y texto rojo `ACCESS DENIED` (hackeo fallido, M-13) |
| Bloqueo | Puerta sin llave o sin energía: panel con `LOCKED` / `NO POWER` y línea de protocolo (D-72, D-78); vigilancia cortada en caza (D-23) |
| Crítico | Electrocardiograma errático, etiqueta roja `CRITICAL` y postura arrastrada |

Todos los estados combinan color con texto, forma o sonido (regla visual 1 de [[01 - Dirección de Arte]]).

## Convenciones

**Tipografía:** monoespaciada de terminal de los 90 para datos y lecturas; sans condensada para títulos y menús. Las fuentes concretas están **por definir**, con licencia libre (OFL) o propias. Etiquetas del terminal en inglés y en mayúsculas (D-62).

**Iconografía:** iconos de línea simples, monocromos, del color de la pantalla que los muestra: teal en el terminal y verde en las pantallas de la instalación.

**Tamaños mínimos:** a 1080p, subtítulos ≥ 32 px y texto del terminal ≥ 24 px; escala de texto configurable (valores iniciales, ver [[03 - Accesibilidad]]).

**Contraste:** texto con un contraste mínimo de 4.5:1 sobre su fondo; los efectos CRT (líneas de escaneo, ruido) nunca bajan de ese mínimo.

**Animaciones:**

- El terminal se abre superpuesto con una transición corta de encendido CRT (~0.2 s) mientras el juego sigue en tiempo real (M-07).
- La vista de vigilancia entra con interferencia (M-02).
- No hay animaciones decorativas largas.

**Pantallas por color (D-82, D-86):**

- **Terminal de Lukas:** fósforo teal con líneas de escaneo suaves.
- **Pantallas de la instalación:** verde fósforo (vigilancia, paneles, consolas).
- **Avisos de The Mother:** en rojo dentro del terminal.

## Pantallas

- **Menú principal:** Continuar, Nueva partida, Opciones, Créditos y Salir (D-39), con el estilo de una consola de Vitalis. Fondo en tiempo real de la fila de cápsulas.
- **Pausa:** menú de pausa sobre la imagen oscurecida. En la introducción, sin "Reiniciar" (D-63).
- **Configuración:** opciones **por definir** ([[02 - UI y Menús]]); incluye la escala de texto.
- **Inventario:** sección del terminal del brazo. Las demás secciones son Estado (salud y linterna) y Registros (documentos, expediente de Emilia, bestiario, objetos confiscados, fallas y anillo).
- **Mapa:** no existe en la demo.
- **Victoria:** no hay pantalla; cinemática en el motor y créditos (D-51).
- **Derrota:**
  - Muerte: cinemática corta de reconstrucción, sin pantalla de "Game Over" (D-39).
  - Final Asimilación: cinemática y créditos (D-61).


> **Navegación:** [[00 - Índice]] · ← [[03 - Entornos]] · [[05 - Audio y Música]] →
