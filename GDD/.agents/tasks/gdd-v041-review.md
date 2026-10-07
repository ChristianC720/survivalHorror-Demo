# Corrección narrativa v0.4.1: esposa desaparecida (D-40) y ficha de Personajes alineada

El cambio corrige la premisa oficial: la esposa del protagonista pasa de "fallecida" a "desaparecida" con la nueva decisión D-40, que modifica D-13. La premisa se actualiza en la Ficha del Proyecto, la Visión del Juego y `Decisiones a Implementar.md`. D-13 queda como histórico con el estado "Modificada por D-40". La ficha de Personajes se alinea con D-18/D-19 cambiando solo el texto de los valores (clon → reconstrucción, runs → intentos, `TBD` → `Por definir`). La versión sube a 0.4.1 con fecha 2026-10-07.

Watch for: nada bloqueante. Hay una viñeta duplicada en `Decisiones a Implementar.md` y un cambio de etiqueta ("Estado Clon" → "Estado de reconstrucción") en Personajes. Ambos son menores (confirmed).

**Verdict**: APPROVED

## High-level view

D-40 sigue la plantilla de las fichas existentes y elige la opción que deja **Por definir** el destino de la esposa y su relación con la instalación y The Mother. Lo único que repite sobre ella (sus documentos persisten en el terminal del brazo) ya estaba en D-13, así que no se inventa lore (confirmed). D-13 conserva su texto original y lleva una línea `**Estado:**` con el formato de D-15. Su fila en la tabla está marcada **Modificada por D-40** y la fila de D-40 tiene fecha 2026-10-07 y responsable Christian Coronel Palma.

Personajes.md mantiene 111 líneas, y 12 cambian solo por sustituciones dentro de frases. Encabezados, viñetas, etiquetas en negrita, backticks, línea de navegación, descripción física, vestuario y nombres propios son iguales a la copia original (confirmed). "Piezas ajenas" sale literalmente de D-18, así que no es lore nuevo.

La premisa nueva aparece idéntica en la Ficha, en `Decisiones a Implementar.md` y en D-40. La Ficha muestra 0.4.1 / 2026-10-07 y el Historial tiene la fila 0.4.1 (confirmed).

<details>
<summary>Issues (2)</summary>

1. **Viñeta redundante en Decisiones a Implementar**: la nueva viñeta D-40 ya dice que la relación con la instalación y The Mother sigue Por definir. La viñeta antigua "La relación entre la esposa, la instalación y The Mother todavía debe definirse" repite lo mismo (confirmed). No bloqueante: se puede quitar la viñeta antigua.
2. **Etiqueta "Estado de reconstrucción" frente a "Estado Humano / Original"**: en L56 de Personajes es el único cambio dentro de una etiqueta en negrita. Rompe el paralelismo con la etiqueta hermana (confirmed). No bloqueante: lo pide D-18 y el usuario quería quitar "clon".

</details>

<details>
<summary>Details</summary>

## Etiqueta del estado visual en Personajes.md

Al comparar el original con la versión actual, el único marcador estructural cuyo texto cambia es `- **Estado Clon (resto del juego):**` → `- **Estado de reconstrucción (resto del juego):**` (confirmed). El marcador `- **…:**` y la descripción completa del vestuario se mantienen. La etiqueta hermana sigue siendo "Estado Humano / Original", así que las dos etiquetas ya no tienen la misma forma. Es solo estético y no cambia el aspecto del personaje.

## Viñeta duplicada en Decisiones a Implementar

El bloque de la premisa ahora tiene dos viñetas que dicen que la relación con la instalación y The Mother está sin definir: la nueva viñeta D-40 y la viñeta antigua de D-13 (confirmed). No contradice nada. Quitar la antigua deja el bloque más limpio.

</details>

<details>
<summary>Archivos</summary>

- `01 - Visión/01 - Ficha del Proyecto.md`: versión 0.4.1 y premisa con "desaparecida".
- `01 - Visión/02 - Visión del Juego.md`: fantasía del jugador con "esposa desaparecida".
- `Decisiones a Implementar.md`: premisa actualizada, viñeta D-40 y "run" → "intento".
- `10 - Referencia/03 - Registro de Decisiones.md`: estado de D-13, ficha D-40 y filas D-13 y D-40.
- `10 - Referencia/04 - Historial de Versiones.md`: fila 0.4.1.
- `03 - Historia y Mundo/03 - Personajes.md`: 12 líneas con sustituciones de texto.

Diff completo de Personajes.md en `.agents/tasks/gdd-v041-verificacion.md`.

</details>
