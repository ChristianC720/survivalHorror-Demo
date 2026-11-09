# Interfaz Visual

## 1. Descripción de la sección

Documenta la información que el juego muestra al jugador y las reglas visuales de la interfaz.

La interfaz es **diegética**, como en *Signalis*. Los estados se leen en el cuerpo de Lukas, en sus objetos y en el **terminal del brazo**, nunca en indicadores fijos en pantalla. El terminal los muestra con etiquetas de texto claras, como `HEALTH: INJURED`, para que un jugador sin experiencia en el género pueda leerlos. La percepción de los enemigos nunca se muestra. El flujo de menús está en [[02 - UI y Menús]] y la accesibilidad en [[03 - Accesibilidad]]. Los valores numéricos de este documento son iniciales y están sujetos a balance.

## 2. HUD

No hay HUD permanente. La tabla indica dónde se lee cada elemento.

| Elemento | Dónde se lee |
|---|---|
| Vida | En la postura y la animación de Lukas, y en la sección Estado del terminal. Ahí un electrocardiograma cambia de forma por estado, regular, irregular o errático, junto a la etiqueta `HEALTH: FINE / INJURED / CRITICAL` |
| Resistencia | En el jadeo de Lukas, sin barra |
| Carga de la linterna | En una barra del terminal. En 0, la linterna parpadea |
| Munición | La cargada, en el contador verde del armazón del revólver, visible al apuntar. La de reserva, en el inventario del terminal |
| Objetivo | En el entorno y en las líneas de protocolo de The Mother. Los contadores de las secundarias están en el terminal, como `Expediente 3/6` y `Objetos confiscados 5/12` |
| Mapa | No existe. La vista de vigilancia es la única ayuda espacial |
| Recursos | En el inventario de 6 casillas del terminal |
| Indicadores contextuales | Son los únicos elementos en pantalla. Se listan abajo |

**Indicadores contextuales:**

- **Retícula.** Es un punto que aparece al apuntar. Se tiñe de rojo sobre un enemigo vivo y de ámbar sobre un punto débil, hasta 60 m.
- **Marcador de impacto.** Cuatro marcas alrededor de la retícula durante 0,18 s al acertar. Son claras en un impacto normal y ámbar en un punto débil. Al matar se ven rojas y 1,5 veces más grandes.
- **Aviso `E`.** Aparece a ~1,5 m de un interactuable. Con mando muestra el botón `A`. Los avisos y los carteles siguen al último dispositivo usado y cambian al vuelo.
- **Borde teal.** Los recogibles pequeños a 3 m o menos y a la vista de Lukas muestran un contorno teal. Son los objetos, el revólver y los documentos. Las puertas y los paneles muestran en su lugar una pequeña luz de estado que pulsa.
- **Avisos breves.** Una línea ámbar arriba al centro durante 3 s, que se desvanece en 0,4 s. Ver la tabla de avisos.
- **Destello de daño.** Toda la pantalla se tiñe de rojo `#FF5242` al 30% de opacidad al recibir un golpe, y se apaga en 0,35 s.
- **Subtítulos y líneas de texto.**
- **Aviso de autoguardado.** `SYNCHRONIZING…` en teal o, si falla, `SYNC FAILED` en rojo.

| Situación | Aviso breve |
|---|---|
| Puerta cerrada | `Locked: you need a key`, `No power: find the mains switch` o `Complete the procedure to open this door` |
| Puerta que se abre o energía restaurada | `Exit unlocked` o `Power restored` |
| Recogida e inventario | `{objeto} +{cantidad}` o `Inventory full` |
| Revólver vacío | `Empty: reload [R]`, con la tecla asignada |
| Salud | `Injured`, `Critical: heal from the terminal` o `Condition improved` |
| Resistencia | `Out of breath` |
| Agarre | `Grabbed: run!` o, con punzón, `Discharge spike used: you broke free` |
| Alerta | `They know where you are`, al entrar un enemigo en caza |
| Jefe | El objetivo de cada fase. `Hack the 3 mixing consoles. Keep away from the arm`, `Avoid the red cameras. Reach the core` y `Shoot the 3 glowing transformers`, con contadores `Console 1/3` y `Transformer 1/3` |

Los avisos se leen en inglés o en español según el idioma elegido.

**Ayudas del tutorial.** El tutorial es guiado. Encima de los avisos breves, una línea teal fija muestra la siguiente acción de la sala con su tecla, como `Turn on the flashlight [F]`. La línea solo existe en el tutorial del primer intento. Las 4 salas tienen además carteles de seguridad de Vitalis en las paredes, en inglés y con humor corporativo. Cada cartel muestra la tecla de una acción, como `IN CASE OF DARKNESS: PRESS [F]`. La tecla sigue la asignación actual de controles.

**Introducción.** No hay terminal. Solo aparecen el aviso `E`, los subtítulos y la pantalla del teléfono para el hackeo.

![Cartel de seguridad de Vitalis en una sala del tutorial|520](../Media/Capturas/CartelTutorial.png)

## 3. Jerarquía visual

1. **Amenaza y supervivencia.** La escena iluminada, las siluetas, la postura de Lukas y la retícula. Nada tapa la imagen central.
2. **Interacción inmediata.** El aviso `E` y el borde teal sobre lo interactuable. Los avisos breves van arriba al centro, y los subtítulos y las líneas de The Mother, abajo al centro.
3. **Gestión.** El terminal del brazo, que solo aparece al abrirlo con `Tab` y muestra inventario, estado y registros.

## 4. Estados

| Estado | Cómo se muestra |
|---|---|
| Normal | Terminal teal, con texto y electrocardiograma regulares |
| Advertencia | Etiqueta en ámbar y parpadeo, por ejemplo con la linterna baja o la salud en Herido |
| Daño | Destello rojo breve y translúcido, vibración de cámara, sonido de impacto, cambio de postura y cambio del electrocardiograma |
| Éxito | Pitido corto y texto `REGISTERED` al registrar un documento, un objeto confiscado o una entrada del bestiario. `ACCESS GRANTED` al completar un hackeo |
| Error | Pitido de error y texto rojo `ACCESS DENIED` al fallar un hackeo |
| Bloqueo | Una puerta sin llave o sin energía muestra `LOCKED` o `NO POWER` en su panel, con una línea de protocolo. La escalera o el elevador muestran `LOCKDOWN` durante una caza. La vigilancia se corta en caza |
| Crítico | Electrocardiograma errático, etiqueta roja `CRITICAL`, aviso breve y postura arrastrada |

Todos los estados combinan el color con texto, forma o sonido, según la regla visual 1 de [[01 - Dirección de Arte]].

![Terminal del brazo en la sección Estado con la salud en Herido|520](../Media/Capturas/TerminalEstado.png)

## 5. Convenciones

**Tipografía:** **VT323**, monoespaciada de terminal de los 90, para datos y lecturas. **IBM Plex Sans** para títulos y menús. Ambas tienen licencia libre OFL. La opción de accesibilidad `Fuente para dislexia` sustituye ambas por **OpenDyslexic**. Las etiquetas del terminal van en inglés y en mayúsculas.

**Iconografía:** Iconos de línea simples y monocromos, del color de la pantalla que los muestra. Son teal en el terminal y verdes en las pantallas de la instalación.

**Tamaños mínimos:** A 1080p, los subtítulos miden 32 px o más y el texto del terminal 24 px o más. La escala de texto es configurable. Ver [[03 - Accesibilidad]].

**Contraste:** El texto tiene un contraste mínimo de 4,5:1 sobre su fondo. Los efectos CRT, como las líneas de escaneo y el ruido, nunca lo bajan de ese mínimo.

**Animaciones:**

- El terminal se abre superpuesto con un encendido CRT corto, de ~0,2 s, mientras el juego sigue en tiempo real.
- La vista de vigilancia entra con interferencia.
- No hay animaciones decorativas largas.

**Color de cada pantalla:**

- El terminal de Lukas es de fósforo teal con líneas de escaneo suaves.
- Las pantallas de la instalación son de verde fósforo, como la vigilancia, los paneles y las consolas.
- Los avisos de The Mother aparecen en rojo dentro del terminal.

## 6. Pantallas

| Pantalla | Contenido |
|---|---|
| Calibración de brillo | Solo en el primer inicio. Símbolos sobre negro y una barra de brillo |
| Pantalla de título | Fondo negro con el logotipo y el latido del laboratorio. Cualquier tecla o botón lleva al menú principal |
| Menú principal | `Continuar`, `Nueva partida`, `Cargar partida`, `Opciones`, `Créditos` y `Salir`. Se muestra en un terminal verde de la instalación, en la sala de la fila de cápsulas, junto a la cápsula de Lukas |
| Pausa | Menú sobre la imagen oscurecida. No ofrece `Reiniciar` en la introducción ni en la decisión final |
| Configuración | Audio, Video, Controles, Accesibilidad y Juego. Ver [[04 - Guardado y Configuración]] |
| Inventario | Sección del terminal del brazo. Las otras secciones son Estado, con salud y linterna, y Registros |
| Registros | Documentos, expediente de Emilia, bestiario, objetos confiscados, fallas y anillo |
| Mapa | No existe en la demo |
| Carga | Solo cuando hay que esperar fuera de una cinemática. Pantalla negra con texto de arranque del terminal en teal, como `SYNCHRONIZING SUBJECT R-H-09…`, y el latido. No hay barra de progreso |
| Confirmaciones | `Reiniciar`, `Salir` durante el recorrido con su costo, `Nueva partida` y `Salir` desde el menú principal |
| Victoria | No hay pantalla. Se reproduce una cinemática en el motor y después los créditos |
| Derrota | La muerte reproduce la cinemática corta de reconstrucción, sin pantalla de Game Over. El final Asimilación reproduce su cinemática y los créditos |

## 7. Rediseño aplicado

- **Foco.** La fila enfocada se ilumina, con texto oscuro y el marcador `>`. El estado nunca depende solo del color.
- **Terminal del brazo.** Es una pantalla CRT con carcasa. La cabecera muestra el sujeto y `VITALIS KONZERN` en cian. Una tira con pulso, salud, luz y tambor aparece en todas las pestañas. El inventario es una cuadrícula de 3 × 2 con iconos de línea.
- **Lectura.** Tiene una vista propia, separada de la lista. Los documentos se leen en la pantalla del terminal. Los pósters aparecen en papel crema con la banda cian de Vitalis.
- **Hackeo.** Las cajas de tecla están abajo a la izquierda. La tecla introducida va rellena, la siguiente con marco y el resto tenue. En la introducción se usa un teléfono vertical claro.
- **Mundo.** Hay paneles pequeños sobre la puerta, con `LOCKED / KEY REQUIRED` o `NO POWER / RESTORE MAINS`. Junto al elevador aparece `OUT OF ORDER / AUSSER BETRIEB`.
- **Vigilancia.** La imagen muestra `CAM  LIVE`. Al cortarse o negarse por una caza aparece un `NO SIGNAL` breve y translúcido.

> **Navegación:** [[00 - Índice]] · ← [[03 - Entornos]] · [[05 - Audio y Música]] →
