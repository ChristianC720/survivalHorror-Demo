# Verificación GDD 0.3.0 — Bloque 2 (iteración 2)

**Fecha:** 2026-10-06 · **Responsable:** Christian Coronel Palma
Existía `gdd-bloque2-review.json` (CHANGES_REQUESTED, 4 hallazgos): segunda iteración. Se corrigieron los 4 hallazgos y se repitió la verificación global. Las secciones de Task 7 y de búsquedas conservan la evidencia de la iteración 1, actualizada con los resultados de esta iteración.

## Iteración 2 — corrección de hallazgos

| # | Hallazgo | Archivo | Corrección |
|---|---|---|---|
| 1 | Parche de fallas al morir (bloqueante) | `02 - Diseño/07 - Reglas y Estados del Juego.md` | Regla global 4: persisten el terminal y el **registro** de las fallas; The Mother parchea cada falla tras 1-2 usos o al morir (D-09, M-17), así que ninguna falla sigue utilizable en el intento siguiente. Caso "Morir durante el hackeo": al morir se parchea cada falla, incluida la que estuviera en uso; persiste el registro (D-09). Caso "Morir en la sala del jefe": "persisten el terminal y el registro de las fallas (parcheadas al morir, D-09)". Ya coincide con Economía, S-07, M-17 y Progresión. `fallas sin parchear`: 0 ocurrencias |
| 2 | Frontera del contexto "sobre un cuerpo" | `02 - Diseño/04 - Controles.md`, `02 - Diseño/02 - Mecánicas.md` (M-20, Casos límite) | Se marcó **Por definir**: cuándo se entra en el contexto (distancia y si basta con mirar el cuerpo), si sigue activo con cuerpos rematados o reciclados, si aplica en Alerta/Combate con enemigos vivos cerca y qué acción tiene prioridad frente a puertas, paneles, escaleras, elevadores u objetos. Se añadió la restricción derivada de decisiones existentes: la regla que se elija no puede impedir interactuar con la ruta garantizada hacia la sala clave (D-15, principio 9 de Jugabilidad). No se decidió ninguna prioridad ni distancia |
| 3 | Referencia falsa al número de enemigos | `02 - Diseño/05 - Progresión.md` (Curva de dificultad) | "Los valores del presupuesto de cada piso se documentan en Economía. La cantidad de enemigos de cada piso está **Por definir**; solo está decidido que el piso 2 tiene más enemigos que el piso 1" |
| 4 | Máximo del piso 2 frente a 6 casillas | `02 - Diseño/06 - Economía y Recursos.md` (Presupuesto y Balance) | La regla se limitó al piso 1: su máximo (9 recogidas: 3+3+2+1, más objetos clave) supera las 6 casillas; en el piso 2 el máximo (6 recogidas: 2+2+1+1, más objetos clave) iguala las casillas y la presión viene del presupuesto menor. No se cambió ningún valor inicial |

## Task 7 — cambios aplicados

| Archivo | Cambio |
|---|---|
| `02 - Diseño/01 - Jugabilidad.md` | Condición de derrota: línea nueva que enlaza [[07 - Reglas y Estados del Juego]] (estados de muerte, reconstrucción, victoria y final de disolución). Reglas y Estados ya enlazaba a Jugabilidad (Estados principales y regla global 7) |
| `02 - Diseño/02 - Mecánicas.md` | Catálogo: línea "**Relación con otras secciones:**" que enlaza [[03 - Sistemas]] y [[06 - Economía y Recursos]]. Economía ya enlazaba a Mecánicas (intro de Recursos y "No son fuentes"); Sistemas ya enlazaba a Mecánicas (intro de Inventario de sistemas) |
| `01 - Visión/03 - Público y Plataforma.md` | `- [x] Guardado compatible`. `- [ ] Interfaz legible` sigue desmarcada |
| `10 - Referencia/04 - Historial de Versiones.md` | Fila 0.3.0 (2026-10-06, Christian Coronel Palma), mismo formato de 4 columnas que 0.2.0 |
| `01 - Visión/01 - Ficha del Proyecto.md` | Versión del GDD 0.3.0; Última actualización 2026-10-06. Es la única nota con la ficha de versión (grep de "Versión del GDD", "Última actualización" y "0.2.0") |

## Búsquedas globales

Script PowerShell `Select-String` sobre todos los `.md` de la bóveda (excluye `.agents` y `.obsidian`).

| Término | Ocurrencias | Valoración |
|---|---|---|
| clon | `Christian Decisions.md` (3, fuente original, solo lectura); `Decisiones a Implementar.md`:52 (lore descartado, se conserva por Task 1); Registro: ficha D-13 (123, 127, 131), ficha D-18 (211, 215), fila D-12 (286, histórica, "Modificada por D-18"), fila D-18 (292, "no se clona") | Solo registros históricos y D-13 |
| Permadeath | `Christian Decisions.md`:16; Registro: ficha D-18 (211, 223), fila D-12 (286), fila D-19 (293); `11 - Referencia de Diseño/06 - Clasificaciones y Etiquetas.md`:40 | Solo registros históricos y 11. La fila 0.3.0 del Historial se redactó sin la palabra ("etiqueta de muerte permanente") para no añadir ocurrencias |
| cargas | 0 | — |
| 3/3 | 0 | — |
| QTE | 0 | — |
| esquiv | Mecánicas:935 (tabla Descartadas); `11 - Géneros.md`:14 (fuera de alcance) | Solo como descartado |
| invulnerab | Mecánicas:935 (Descartadas); Registro fila D-05 (279); Historial fila 0.2.0 | Solo como eliminado o histórico |
| cono (`\bconos?\b`) | Mecánicas M-12:508 y 510 (linterna); Mecánicas:934 (Descartadas); Registro D-02 (29, 35, 39-41, 47, 276); Historial fila 0.2.0 | Uso activo solo en la linterna; el resto es descartado o histórico |
| Presión Constante | 1: Jugabilidad:170, "presión constante reduciría el impacto del terror" (texto original de Ritmo) | No es el nombre del pilar; igual que en la verificación 0.2.0 |
| sin parchear (iteración 2) | 0 | Hallazgo 1 resuelto |
| supera las 6 (iteración 2) | Economía:78 y 112, ambas referidas solo al piso 1 | Hallazgo 4 resuelto |
| `- -` | 0 | — |

Iteración 2: los recuentos de clon (11), Permadeath (6), cargas, 3/3 y QTE (0) son idénticos a los de la iteración 1. En Mecánicas, las líneas posteriores a M-20 se desplazaron 2 posiciones por el caso límite nuevo.

## Comprobaciones estructurales (11 notas tocadas en Bloque 2)

Notas: Ficha, Público y Plataforma, Jugabilidad, Mecánicas, Sistemas, Controles, Progresión, Economía y Recursos, Reglas y Estados, Registro de Decisiones, Historial de Versiones.

- **Título y "## Descripción de la sección":** presentes en las 11. El texto de la descripción es idéntico al de `HEAD` en las 11 (comparación por script con `git show HEAD:`).
- **Navegación:** una sola línea `> **Navegación:** ...`, es la última línea no vacía y es idéntica a la de `HEAD` en las 11.
- **Encabezados de plantilla:** los `#`/`##` de `HEAD` siguen presentes, salvo "## Plantilla de mecánica" (Mecánicas) y "## Decisión" (Registro). Ambos se sustituyeron en la v0.2.0, que ya está aprobada (`HEAD` es anterior a la v0.2.0). El Bloque 2 no los cambió.
- **Negritas:** ninguna línea fuera de bloques de código tiene un número impar de `**`.
- **"- -":** 0 líneas en toda la bóveda.
- **Wikilinks:** los de las 11 notas apuntan a notas existentes. En toda la bóveda solo hay 2 enlaces rotos, los dos de antes y en archivos sin cambios: `README.md` → `[[Wikilinks]]` (texto explicativo) y `10 - Referencia/02 - Referencias e Inspiración.md` → `[[imagen.png]]`. No se tocaron.
- **Iteración 2:** se repitieron por script las comprobaciones de las 11 notas. La navegación es la última línea no vacía e idéntica a `HEAD`, la descripción es idéntica a `HEAD`, solo faltan los dos encabezados ya sustituidos en la v0.2.0, no hay `**` impares y no hay wikilinks rotos. Los enlaces cruzados de Task 7 siguen presentes: Jugabilidad→Reglas 1, Reglas→Jugabilidad 2, Economía→Mecánicas 2, Mecánicas→Economía 2, Sistemas→Mecánicas 3, Mecánicas→Sistemas 2.
- **Teclas por contexto (Controles):** Exploración: `Clic izquierdo` empujón, `E` interactuar, `Q` vigilancia. Apuntando: `Clic izquierdo` disparo. Inventario: `Clic izquierdo` usar, `Clic derecho` descartar. Hackeo: `WASD`/`Flechas` secuencia, `E` abandonar. Vigilancia: `Q` salir. Sobre un cuerpo (sin apuntar): `Clic izquierdo` golpe en el suelo, `E` consumible de remate (sustituyen al empujón y a interactuar en ese contexto). Ninguna tecla tiene dos funciones en el mismo contexto.

## Archivos fuera de alcance

- Iteración 2: se repitió la comparación (`git rev-parse HEAD:./<ruta>` frente a `git hash-object`) con 39 archivos y 0 diferencias. `git status` de 00, 03 a 09, 11 y README no muestra cambios. Los MD5 de `Christian Decisions.md` y `Decisiones a Implementar.md` son idénticos a los de abajo.
- Carpetas 03 a 09 y 11: `git status` sin cambios. Además se comparó el blob de `HEAD` (`git rev-parse HEAD:<ruta>`) con `git hash-object` del disco: 39 archivos, 0 diferencias.
- `00 - Índice.md`, `README.md`, `00 - Acerca de esta guía.md`: sin cambios según `git status`.
- `Christian Decisions.md` (sin seguimiento en git): MD5 `CDAA4045F9B906BA59B8F518371B83E1`, última escritura 2026-10-05 13:12, antes de que empezara el Bloque 2 (el plan se creó el 2026-10-05 14:35). Sin cambios.
- `Decisiones a Implementar.md` (sin seguimiento en git): MD5 `674F41B292EF7C03E8A1562DDF0BD961`, última escritura 2026-10-05 13:52, de la v0.2.0. Sin cambios en el Bloque 2.

## Puntos "Por definir" que siguen abiertos

- Umbral de "en alerta": búsqueda o caza (M-10, S-02, Reglas y Estados).
- Cobertura mínima de cámaras (M-02, S-01, S-06) y distancia mínima de la sala clave (M-15, S-06).
- Si los enemigos siguen al jugador por escaleras o elevadores (M-15, S-02).
- Cargador y munición por casilla (M-04); máximo por casilla de curas, objeto defensivo y consumible de remate (Economía).
- Porcentaje de reducción de velocidad en Herido y Crítico (M-05); costo de resistencia del empujón y del golpe en el suelo (M-06, S-04).
- Si existe reciclaje en la sala de The Mother y su presupuesto de consumible de remate (M-19, M-20, Economía, Progresión).
- Comportamiento de The Mother en la fase 2 (M-19, S-10); munición mínima de la fase 3.
- Si la degradación estética se reinicia al llegar a una sala clave (M-18, Progresión, Reglas y Estados).
- Guardado: periodo de gracia al reanudar durante una alerta; si la suspensión se elimina al reanudar (Reglas y Estados, S-07).
- Opciones del menú principal; presentación de la secuencia de reconstrucción; pantalla de carga entre pisos; si los créditos se pueden saltar (Reglas y Estados).
- Tutorial: zona propia o parte del piso 1, sus amenazas y recursos; orden exacto de aprendizaje dentro del tutorial y del piso 1 (Progresión).
- Objetos clave: cuáles son y su uso; baterías y fusibles o tarjetas: presupuesto, casillas y duración (Economía).
- Documentos de la esposa: cantidad y contenido; requisito exacto de cada entrada del bestiario (Progresión).
- Lore: relación entre la esposa, la instalación y The Mother, y contenido de la revelación final.
- Controles: reasignación y columna Control (soporte posterior).
- Frontera del contexto "sobre un cuerpo": distancia, cuerpos rematados o reciclados, uso en Alerta/Combate y prioridad frente a interactuables (Controles, M-20). Iteración 2.
- Cantidad de enemigos por piso (Progresión). Iteración 2.
