# Bloque 4 — Fase C: integración y verificación (GDD v0.5.0)

**Fecha:** 2026-10-08 · **Responsable:** Christian Coronel Palma · **Iteración:** 1 (no existía `gdd-bloque4-review.json`)

## Archivos cambiados en la Task 9

| Archivo | Cambio |
|---|---|
| `01 - Visión/02 - Visión del Juego.md` | Concepto central: nombre Identity Dissolution (clave Heartbeat, D-41), párrafo "Tema central (D-44)" con el texto literal de D-44 y párrafo "Postura" (nihilismo puro; el laboratorio es la criatura, D-46; nunca se explica, D-16/D-47; el anillo es ancla del personaje, no del universo; motivación por misterio y descubrimiento; sin salvación, D-51). Fantasía del jugador: párrafo nuevo (Lukas técnico de Vitalis, D-50; olvido y anillo, D-45; documentos corruptos; asimilación por reconstrucción, D-43). Se conservan la premisa, los encabezados y la navegación |
| `01 - Visión/01 - Ficha del Proyecto.md` | Versión del GDD 0.4.1 → 0.5.0; última actualización 2026-10-07 → 2026-10-08 |
| `10 - Referencia/04 - Historial de Versiones.md` | Fila 0.5.0 (2026-10-08) |
| `.agents/tasks/gdd-pendientes-por-definir.md` | Reescrito para v0.5.0 |

## Búsquedas globales

Conjunto de búsqueda (51 archivos):

```powershell
$files = Get-ChildItem -Recurse -Filter *.md | Where-Object { $_.FullName -notmatch '\\\.agents\\' -and $_.Name -ne 'Christian Decisions.md' -and $_.FullName -notmatch '\\11 - Referencia de Dise' }
$files | Select-String -Pattern '<patrón>'
```

| Patrón | Coincidencias | Resultado |
|---|---|---|
| `Final de disoluci` | 0 | No vigente |
| `distorsi[oó]n de voz` | 3 | Solo histórico: Registro fila D-08 (649) y fila D-52 (693), que describen el renombre; Historial fila 0.5.0 (16). Ningún uso vigente |
| `Gesund` | 0 | No aparece |
| `disoluci[oó]n de identidad` | 7 | Solo en el Registro: Motivo de D-35 (410, histórico), Estado de D-35 (414), ficha D-42 (476, 480, 484), ficha D-44 (510) y fila D-08 (649). Todas históricas o referidas al tema |
| `\bfallas?\b` | 69 líneas en 10 archivos | Revisadas las que están a ≤ 60 caracteres de "document": todas se refieren a M-17 (fallas de la máquina) en listas junto a documentos, o a las reglas D-45 que separan ambos términos. En `03 - Historia y Mundo`: Personajes 108, 138, 143 (fallas de la máquina) y Diálogos 46 ("Corrupto" es distinto de las fallas de la máquina) |
| `documentos? (con fallas?\|fallid\|defectuos)` | 0 | "fallas" no se usa para documentos; el término vigente es "corrupto" |

## Diff de la ficha de Lukas

Comparación por script del bloque `#### Lukas Reiner-Hellen` hasta `### Antagonista` entre `.agents/tasks/gdd-v050-personajes-original.md` y `03 - Historia y Mundo/03 - Personajes.md`. Cada línea se reduce a su esqueleto (encabezado, línea en blanco, viñeta, etiqueta en negrita o texto):

- Líneas del bloque: original 59, actual 59.
- Diferencias de esqueleto (encabezados, viñetas, etiquetas, líneas en blanco y orden): **0**.
- Líneas con texto distinto: 16, todas valores de campo: L3 Rol, L7 Función narrativa, L16 Personalidad, L18 Motivaciones, L22 y L24 (viñetas de Objetivos), L28–L30 (viñetas de Conflicto), L34–L35 (viñetas de Relaciones), L37 Arco del personaje, L41 y L44 (viñetas de Habilidades), L50 (viñeta de Debilidades), L53 Frases o tono de diálogo.
- Sin cambios: Función jugable, Descripción física (incluidos los dos estados visuales y el anillo), Referencias visuales y los nombres propios.
- Las 40 líneas anteriores al bloque (título, descripción y plantilla) y la línea de navegación son idénticas a la copia.

## Estructura, negritas y wikilinks

Script sobre las 20 notas de `01 - Visión`, `02 - Diseño`, `03 - Historia y Mundo`, `10 - Referencia` y `Decisiones a Implementar.md` (fuera de bloques de código):

- Título `# ` y "## Descripción de la sección" (una vez) en las 19 notas del GDD.
- Línea `> **Navegación:** ...` como última línea no vacía en las 19 notas (en `04 - Pilares de Diseño.md` el script dio falso negativo por un carácter previo; `Get-Content -Tail` confirma la línea y la nota no cambió desde 2026-10-05 14:23).
- Negritas abiertas: ninguna línea con número impar de `**` en las 20 notas.
- Wikilinks: todos resuelven. Solo los 2 rotos preexistentes, en archivos sin cambios: `README.md` → `[[Wikilinks]]` (texto explicativo) y `10 - Referencia/02 - Referencias e Inspiración.md` → `[[imagen.png]]`. Los enlaces nuevos de la Visión (`[[01 - Premisa y Trama]]`, `[[02 - Mundo]]`) resuelven.

## Archivos fuera de alcance

Por fecha de última escritura (el bloque no tiene commits):

- `Christian Decisions.md`: 2026-10-05 13:12 — sin cambios.
- `README.md`: 2026-10-05 10:43 — sin cambios.
- Carpetas `04` a `09` y `11 - Referencia de Diseño`: todas sus notas con última escritura máxima 2026-10-05 10:43 — sin cambios.
- Archivos de la bóveda escritos el 2026-10-08: Registro, Mecánicas, Decisiones a Implementar, las 4 notas de `03 - Historia y Mundo`, Reglas y Estados, Sistemas, Progresión, Economía y Recursos, Jugabilidad (Fases A y B) y Visión del Juego, Ficha del Proyecto e Historial (Task 9). Ninguno fuera de alcance. No se crearon, borraron ni renombraron notas.

## Observaciones sin corregir

- `Decisiones a Implementar.md`, 03.4: la nota "Decisión confirmada (D-16)" conserva "siguen por definir"; justo debajo, "Decisión confirmada (D-47, D-51)" los resuelve. Se deja como histórico del archivo.
- `01 - Visión/03 - Público y Plataforma.md`, "Almacenamiento": **Por definir.** preexistente, fuera del Bloque 4.

## Puntos que siguen "Por definir"

Lista completa en `.agents/tasks/gdd-pendientes-por-definir.md`. Resumen:

- **Historia y mundo:** cómo muere Lukas en la introducción (D-50) y formato de la introducción; año, ubicación geográfica, ciudad y otras instalaciones de Vitalis; naturaleza y fecha del incidente y orden de los hechos de Emilia; cómo y cuándo recibe Lukas el mensaje y su texto; identidad visual de los pisos y aspecto de la sala de reconstrucción y la de The Mother; qué parte del expediente R-H se revela en el piso 2; mundo tras la demo; campos vacíos de The Mother y Emilia y líneas concretas; tipos de enemigos y entradas del bestiario.
- **Documentos y narrativa:** texto de cada documento y cuáles son corruptos; si cada documento de Emilia es fijo por piso y cuántos caben en una sala; contenido de una sala secundaria ya registrada; cantidad y contenido de documentos de Vitalis y The Mother; objetos clave distintos de las llaves; terminal tras los créditos; narrador en texto, textos de The Mother en el jefe, degradación del texto de Lukas, humor; idioma de redacción original y tratamiento de nombres propios en español.
- **Finales y cinemáticas:** detalles del Relevo completo además del expediente; latido final del Ambiguo y su encadenado con el gancho; contenido visual de la Asimilación; qué cinemáticas se pueden saltar; amenazas, muerte y pausa durante la decisión final; qué ofrece Continuar tras un final.
- **Diseño, reglas y balance:** si la salida con calma sube el código R-H; degradación en el primer intento; curva del latido del laboratorio; puntos débiles y cámaras hostiles del jefe; reciclaje y remate; distancia mínima y salas del tutorial; alcance de vigilancia y alerta; casos de guardado y salida; munición por recogida, baterías, fusibles y máximos por casilla; contenido de Opciones.
- **Controles:** reasignación de teclas y columna de gamepad.
