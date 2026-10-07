# Verificación GDD v0.4.1 — Corrección narrativa (D-40)

Primera iteración: `.agents/tasks/gdd-v041-review.json` no existía (`Test-Path` → `False`).

Todas las búsquedas se ejecutaron con PowerShell `Select-String` sobre `Get-ChildItem -Recurse -Filter *.md`, excluyendo `\.agents\`, `Christian Decisions.md` y `11 - Referencia de Diseño`. Nota: la herramienta `grep_search` del entorno no indexa `03 - Personajes.md` (no lo devolvió aunque contenía "clon"/"TBD"), por eso se usó `Select-String`.

## Archivos editados

| Archivo | Cambio |
|---|---|
| `01 - Visión/01 - Ficha del Proyecto.md` | L13 Versión del GDD 0.4.0 → 0.4.1; L18 Última actualización 2026-10-07 (ya lo era); L68 Premisa "fallecida" → "desaparecida" |
| `01 - Visión/02 - Visión del Juego.md` | L23 Fantasía del jugador "esposa fallecida" → "esposa desaparecida" |
| `Decisiones a Implementar.md` | L48 "(Decisión confirmada — D-13; modificada por D-40)"; L50 premisa con "desaparecida"; L52 nueva viñeta D-40 (destino y relación con la instalación y The Mother **Por definir**); L136 "Estructura de una run." → "Estructura de un intento." (terminología) |
| `10 - Referencia/03 - Registro de Decisiones.md` | Ficha D-13 conservada como histórico + L137 `**Estado:** Modificada por D-40 (...)` (mismo formato que D-15); nueva ficha D-40 (L442) antes de `## Registro`; fila D-13 de la tabla con `**Modificada por D-40**` (L480); nueva fila D-40 (L507), 2026-10-07, Christian Coronel Palma |
| `10 - Referencia/04 - Historial de Versiones.md` | Fila 0.4.1 (2026-10-07) |
| `03 - Historia y Mundo/03 - Personajes.md` | Solo texto de valores (ver diff) |

No se editaron: `Christian Decisions.md`, carpetas 02, 04–09 y 11, ni otros archivos de `03 - Historia y Mundo` (LastWriteTime de `01 - Premisa y Trama.md`, `02 - Mundo.md`, `04 - Diálogos y Narrativa.md` y `Christian Decisions.md`: 05/10/2026; los de `02 - Diseño` son de bloques anteriores, ≤ 12:35 del 07/10, antes de esta tarea). No se crearon, borraron ni renombraron notas del vault. Sin commits de git.

## Búsqueda de referencias a la esposa como muerta (antes de editar)

Comando: `Select-String -Pattern 'esposa|Emilia|duelo'` y `'fallec|difunt|viud|muri[oó]|muerte de su esposa|esposa muerta'`.

Resultado antes: "fallecida" en Ficha L68, Visión L23, Decisiones a Implementar L50, Registro L128/L131/L456. Ninguna referencia en `02 - Diseño` presenta a la esposa como muerta (solo "documentos de la esposa" y "relación… Por definir"), por lo que no se tocó esa carpeta. `03 - Personajes.md` ya decía "desaparecida (destino desconocido)".

## Búsqueda final (después de editar)

Comando: `Select-String -Pattern <patrón>` por cada patrón.

- **fallec:** solo Registro L128, L131 (ficha D-13 histórica), L137 (estado "Modificada por D-40… no fallecida"), L442/L448/L452 (ficha D-40 que describe el cambio), L480 (fila D-13 marcada **Modificada por D-40**) e Historial L15 (fila 0.4.1 que describe el cambio). Todos históricos/explicativos.
- **difunt:** 0 resultados.
- **viud|muri[oó]|muerte de su esposa|esposa muerta:** 1 resultado, Personajes L71 "una esposa que no sabe si vive o murió": expresa incertidumbre, coherente con D-40; se conserva.
- **clon:** Decisiones a Implementar L53 (lore descartado de humanos/clones), Registro L123/L127/L131 (ficha D-13, lore descartado), L215/L219 (ficha D-18, histórico de D-12), L479 (fila D-12 **Modificada por D-18**), L485 (fila D-18 "no se clona"). Todos históricos.
- **TBD:** 0 resultados.
- **runs:** 0 resultados. **\brun\b:** 0 resultados.
- **Permadeath:** Registro L215, L227 (ficha D-18), L479 (fila D-12), L486 (fila D-19). Todos históricos.

## Estándar de las notas editadas

Comando: primera línea, conteo de `^## Descripción de la sección` y última línea no vacía de cada archivo editado.

| Archivo | Título | `## Descripción de la sección` | Navegación final |
|---|---|---|---|
| Ficha del Proyecto | `# Ficha del Proyecto` | 1 | intacta |
| Visión del Juego | `# Visión del Juego` | 1 | intacta |
| Registro de Decisiones | `# Registro de Decisiones` | 1 | intacta |
| Historial de Versiones | `# Historial de Versiones` | 1 | intacta |
| Personajes | `# Personajes` | 1 | intacta (`> **Navegación:** [[00 - Índice]] · ← [[02 - Mundo]] · [[04 - Diálogos y Narrativa]] →`) |

## Personajes.md — diff contra `gdd-v041-personajes-original.md`

Copia original guardada antes de editar con `Copy-Item`; hash SHA256 idéntico al original en ese momento (`14C006DC…94A4`).

Comando: comparación línea a línea (`Get-Content -Encoding UTF8` de ambos y `-ne` por índice).

Líneas: original 111, nueva 111. Líneas distintas: 12 (L47, L49, L56, L60, L66, L70, L72, L76, L77, L79, L86, L95). El resto (99 líneas) es idéntico.

| Línea | Original | Nuevo |
|---|---|---|
| L47 | "…para referenciar runs anteriores." | "…para referenciar intentos anteriores." |
| L49 | "…justifica el ciclo de clonación como forma de investigación: cada run deja rastro que el siguiente clon puede retomar." | "…justifica el ciclo de muerte y reconstrucción como forma de investigación: cada intento deja rastro que el siguiente intento puede retomar." |
| L56 | "- **Estado Clon (resto del juego):** … persistente en cada clon." | "- **Estado de reconstrucción (resto del juego):** … persistente en cada reconstrucción." (la descripción visual de la vestimenta queda idéntica) |
| L60 | "Tras la clonación, esa búsqueda es lo que persiste con más claridad entre runs." | "Tras cada reconstrucción, esa búsqueda es lo que persiste con más claridad entre intentos." |
| L66 | "- \`TBD\`: si ambos objetivos…" | "- \`Por definir\`: si ambos objetivos…" |
| L70 | "…el propio ciclo de clonación." | "…el propio ciclo de muerte y reconstrucción." |
| L72 | "- Identitario: cada muerte lo reemplaza por un clon; …" | "- Identitario: cada muerte lo reconstruye con piezas ajenas; …" |
| L76 | "…motor de la historia. \`TBD\`: su vínculo con la instalación." | "…motor de la historia. \`Por definir\`: su vínculo con la instalación." |
| L77 | "- \`TBD\`: otros personajes, antagonista y NPC." | "- \`Por definir\`: otros personajes, antagonista y NPC." |
| L79 | "**Arco del personaje:** \`TBD\`. … renace como clon y debe asumir el ciclo. La dirección final del arco depende de los \`TBD\` narrativos (…)" | "**Arco del personaje:** \`Por definir\`. … se reconstruye con piezas ajenas y debe asumir el ciclo. La dirección final del arco depende de los puntos narrativos \`Por definir\` (…)" |
| L86 | "…rastros de runs anteriores (solo en estado clon)." | "…rastros de intentos anteriores (solo en estado de reconstrucción)." |
| L95 | "…tono cansado y melancólico. \`TBD\`: líneas concretas." | "…tono cansado y melancólico. \`Por definir\`: líneas concretas." |

### Conteo estructural (original → nuevo)

Comando: `($lineas | Select-String -Pattern <p>).Count` sobre ambos archivos, más conteo de `**` y de backticks.

| Elemento | Patrón | Original | Nuevo |
|---|---|---|---|
| Encabezados | `^#` | 10 | 10 (lista idéntica y en el mismo orden: `HEADINGS_IGUALES=True`) |
| Filas de tabla | `^\|` | 0 | 0 |
| Callouts / citas | `^>` | 1 | 1 |
| Viñetas | `^\s*-\s` | 20 | 20 |
| Etiquetas en negrita al inicio | `^\*\*[^*]+:\*\*` | 28 | 28 |
| Imágenes / embeds | `!\[` | 0 | 0 |
| Separadores | `^---` | 0 | 0 |
| Líneas vacías | `^\s*$` | 49 | 49 |
| Marcas `**` (total) | `\*\*` | 62 | 62 |
| Backticks (total) | `` ` `` | 12 | 12 |

Comprobación de prefijo estructural por línea (marcador de encabezado/viñeta/cita + etiqueta en negrita): solo difiere L56, donde cambia el texto de la etiqueta "Estado Clon" → "Estado de reconstrucción"; el marcador `- **…:**` se mantiene. Ninguna línea cambió su número de `**` ni de backticks.

### Decisiones de alcance en Personajes.md

- Se conservan nombres propios (Lukas Reiner-Hellen, Emilia Reiner-Hellen), el anillo, la descripción física y toda la vestimenta, porque el usuario pidió no modificar visualmente al personaje. Diferencias con `Decisiones a Implementar.md` que se mantienen a propósito: "complexión delgada" (allí "Atlética + delgada") y "chaqueta de campo civil" (allí "Chamarra de cuero"). La terminal en el antebrazo izquierdo ya coincidía.
- "seis slots" se conserva: es coherente con M-07 (6 casillas) y no contradice ninguna decisión.
- "una esposa que no sabe si vive o murió" se conserva: expresa la incertidumbre de D-40 sin decidir su destino.
- No se añadió lore sobre la esposa ni sobre The Mother; los huecos siguen como `Por definir`.
