# 10 — UI, Terminal y Menús

**Objetivo:** interfaz diegética (terminal del brazo), los pocos elementos en pantalla, menús, Opciones, accesibilidad y localización en inglés y español.

**Responsable:** Audio, narrativa y UI.

**Referencias:** `GDD/05 - Arte y Audio/04 - Interfaz Visual.md` (D-86), `GDD/06 - Experiencia/02 - UI y Menús.md`, `GDD/06 - Experiencia/03 - Accesibilidad.md`, `GDD/06 - Experiencia/04 - Guardado y Configuración.md`, `GDD/02 - Diseño/07 - Reglas y Estados del Juego.md` (menús y confirmaciones), D-62, D-89, D-90, D-102..D-106.

**Depende de:** guía 01; datos de las guías 02, 06 y 08.

## Pasos

### 1. Base técnica

- uGUI con TextMeshPro. Todos los `Canvas` de interfaz en *Screen Space – Overlay*: se dibujan a resolución nativa aunque el mundo se renderice a ~540p (guía 12).
- *Canvas Scaler*: *Scale With Screen Size*, referencia 1920 × 1080, *Match* 0.5.
- Fuentes (D-126): `VT323` (terminal y datos) e `IBM Plex Sans` (títulos y menús), como *TMP Font Asset* con los caracteres de inglés y español. Opción "Fuente para dislexia": un `FontSwitcher` cambia `TMP_Text.font` de todos los textos a `OpenDyslexic`.
- Tamaños mínimos a 1080p: subtítulos ≥ 32 px, terminal ≥ 24 px; contraste ≥ 4.5:1 (D-86).
- Etiquetas del terminal en inglés y mayúsculas (D-62), también en la versión en español del resto de textos.

### 2. Elementos en pantalla (los únicos, D-86)

| Elemento | Cuándo | Fuente de datos |
|---|---|---|
| Retícula de punto | Mientras se apunta | Guía 03 |
| Aviso `E` | Interactuable a ~1.5 m al centro de la vista | `InteractionProbe.Current` (guía 06) |
| Subtítulos y líneas de The Mother | Al sonar un sonido vocal o una línea de protocolo | `SubtitleView` |
| "sincronizando…" / `SYNC FAILED` | Al guardar | `SaveService` (guía 08) |

No hay barras de salud, resistencia, munición ni mapa.

### 3. Terminal del brazo

`TerminalView` (un `Canvas` con material CRT teal y encendido de ~0.2 s). Se abre con `Tab` **sin pausar** (guía 06). Secciones:

- **INVENTORY:** las 6 casillas (`InventoryModel`); clic izquierdo usar/equipar, clic derecho descartar; reserva de munición.
- **STATUS:** electrocardiograma con forma distinta por estado (regular, irregular, errático) + `HEALTH: FINE / INJURED / CRITICAL`; barra de carga de la linterna; código de sujeto `R-H-0N`.
- **RECORDS:** documentos (con partes corruptas en glitch morado; nunca en reglas ni rutas), expediente de Emilia `n/6`, bestiario, objetos confiscados `n/12`, fallas, anillo y la nota de voz.
- **HACK:** la secuencia del hackeo (guía 06).
- Avisos de The Mother en rojo. Estados: advertencia en ámbar con parpadeo; éxito `REGISTERED` / `ACCESS GRANTED`; error `ACCESS DENIED` (tabla "Estados" de Interfaz Visual). Siempre texto + color.

### 4. Menús

Flujo: calibración de brillo (solo el primer inicio) → pantalla de título (negro, logotipo y latido; cualquier tecla) → menú principal en un terminal verde dentro de la sala de cápsulas (D-90).

| Pantalla | Opciones |
|---|---|
| Menú principal | Continuar (gris sin partida), Nueva partida, Cargar partida, Opciones, Créditos, Salir |
| Selección de ranura | 3 ranuras con código de sujeto, piso y fecha; ranura dañada con "Borrar" (D-105) |
| Selección de modo | Supervivencia / Historia, solo al crear la partida (D-103) |
| Pausa | Volver, Reiniciar (no en la introducción ni en la decisión final), Opciones, Salir |
| Confirmaciones | Nueva partida, Reiniciar (avisa del costo), Salir durante el recorrido (avisa del costo según `GetExitCost`, guía 08), Salir del juego (D-96) |
| Carga | Negro con texto de arranque en teal (`SYNCHRONIZING SUBJECT R-H-09…`) y el latido; sin barra (D-96) |
| Créditos | Saltables con `Espacio` si ya se vieron |

La navegación con ratón usa el `EventSystem` de `Bootstrap`.

### 5. Opciones (`SettingsService`)

Guarda en `GlobalData` al salir de Opciones; cada sección tiene "Restablecer" con confirmación.

| Sección | Implementación |
|---|---|
| Audio | Parámetros expuestos del `AudioMixer` (guía 11): general, música, efectos, ambiente, voces |
| Video | `Screen.SetResolution`, `FullScreenMode`, `QualitySettings.vSyncCount`, `Application.targetFrameRate`, calidad de sombras (cambia de `URP_High` a `URP_Low`), brillo (exposición de un *Volume* global) |
| Controles | Reasignación con `InputAction.PerformInteractiveRebinding()`; rechaza teclas repetidas en el mismo mapa; guarda con `SaveBindingOverridesAsJson()`; sensibilidad de mirar y de apuntar, eje Y invertido, agacharse alternar/mantener, asistencia de apuntado (D-104) |
| Accesibilidad | Tamaño y fondo de subtítulos, escala del terminal, subtítulos de sonido (D-102), reducción de movimiento, destellos y CRT/glitch (D-106) |
| Juego | Idioma inglés / español |

Los carteles del tutorial y el aviso de interacción leen el enlace del último dispositivo usado (`GetBindingDisplayString` con el *control scheme* activo; escucha `InputUser.onChange` o `PlayerInput.onControlsChanged`) y cambian al vuelo (D-89, D-132).

### 6. Subtítulos de sonido (D-102)

Desactivados por defecto. `AudioService` (guía 11) publica `SoundCaption(id, dirección)` para los sonidos marcados como importantes; `SubtitleView` muestra `[Pasos rápidos, detrás]` con la dirección aproximada relativa a la cámara (delante, detrás, izquierda, derecha), sin distancia ni posición en pantalla.

### 7. Localización

Paquete Localization: tablas `UI`, `Terminal`, `Documents`, `Protocol`, `Subtitles`, *locales* `en` (por defecto) y `es`. El alemán nunca se traduce (D-58). Los textos están en borrador en `GDD/03 - Historia y Mundo/05 - Textos del Juego.md`; cada `id` es una clave de tabla. Los tramos `{c}…{/c}` son corrupción (D-127): `CorruptedTextFormatter` los sustituye por glifos corruptos en morado salvo que el desenlace los haya completado; `{n}` se reemplaza por el número del código de sujeto.

## Criterios de aceptación

- No hay HUD permanente; solo los 4 elementos de la tabla del paso 2.
- Abrir el terminal no pausa: los enemigos siguen moviéndose.
- Todo estado se lee con texto además de color.
- Cambiar una tecla actualiza los carteles del tutorial; una tecla repetida en el mismo contexto se rechaza.
- Cambiar el idioma cambia todos los textos salvo las etiquetas del terminal y el alemán.
- A 1080p, subtítulos ≥ 32 px y terminal ≥ 24 px.
