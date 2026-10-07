# Bloque 3 — Verificación global (Task 7, GDD v0.4.0)

**Fecha:** 2026-10-07 · **Responsable:** Christian Coronel Palma · **Iteración:** primera (no existía `gdd-bloque3-review.json`).

## Task 7 — Archivos editados

| Archivo | Cambio |
|---|---|
| `02 - Diseño/01 - Jugabilidad.md` | Condición de victoria: 3 pisos, sala de The Mother en el piso 3 sin autoguardado (D-33), enlace a M-15 para configuraciones completa y showcase. Condición de derrota: "Reiniciar" cuenta como muerte (D-32), costo de salir (D-31), contador de 5 muertes según D-35 (solo la sala final del piso 2 lo reinicia; primer intento no cuenta; Reiniciar y salir con costo suman), degradación solo acumulativa (D-29). Principio 5: corte de vigilancia con enemigo cercano en caza (D-23) |
| `01 - Visión/03 - Público y Plataforma.md` | Sesión mínima ~10 min; total ~45–60 min (completa); showcase ~30 min (D-34). Marcados "(valor inicial, sujeto a balance)"; se retiran los "Por definir" de duración |
| `01 - Visión/01 - Ficha del Proyecto.md` | Versión 0.4.0; última actualización 2026-10-07; nuevo punto del criterio mínimo de la demo para la configuración de showcase (~30 min); justificación de cámara alineada con D-23 (búsqueda degrada, caza corta) |
| `10 - Referencia/04 - Historial de Versiones.md` | Fila 0.4.0 (2026-10-07, Christian Coronel Palma) |
| `.agents/tasks/gdd-pendientes-por-definir.md` | Reescrito: solo Bloque G (contenido y lore) y Controles (reasignación y columna Control) |
| `02 - Diseño/05 - Progresión.md` | Ajuste de integración: la etiqueta "Degradación estética (Deseable, D-08)" pasa a "(Deseable, D-08, D-29)" para no presentar D-08 como regla vigente de la degradación |

## Búsquedas de términos obsoletos

Ámbito: `01 - Visión/`, `02 - Diseño/`, `10 - Referencia/`, `00 - Índice.md`, `README.md`, `00 - Acerca de esta guía.md`.

| Patrón (regex) | Resultados | Valoración |
|---|---|---|
| `suspensi\|suspend` | Reglas y Estados (regla 3: "no existe guardado manual ni suspensión"); Registro (D-22 histórico y fichas D-30/D-31 que describen el cambio); Historial (filas 0.3.0 y 0.4.0) | Sin usos vigentes |
| `2 pisos\|dos pisos` | Solo Registro: ficha D-15 (histórica, con "**Estado:** Modificada por D-33") y ficha D-33 (problema y opción 1) | Sin usos vigentes |
| `D-22` | Reglas y Estados y Sistemas: "(D-30, modifica D-22)"; Registro: fila con "**Modificada por D-30**" y ficha D-30; Historial | Sin usos vigentes |
| `D-08` / `D-15` | Progresión: "D-08 modificada por D-35" y "D-08, D-29"; Registro: filas marcadas "Modificada por D-35 / D-33" | Sin usos vigentes |
| `\bclon` | Solo Registro: fichas D-13 y D-18 y filas D-12/D-18 (históricas: describen el descarte de la clonación) | Sin usos vigentes |
| `cargas\b` | Sin resultados | — |
| `QTE` | Sin resultados | — |
| `esquiv` | Mecánicas, tabla de mecánicas descartadas ("Esquiva con invulnerabilidad", D-05) | Descartada, no vigente |
| `invulnerab` | Mecánicas (tabla de descartadas); Registro (fila D-05 "sin invulnerabilidad"); Historial 0.2.0 | Sin usos vigentes |
| `\bconos?\b` | Mecánicas M-12 (cono de luz de la linterna, 3 usos); Mecánicas tabla de descartadas (D-02); Registro D-02; Historial 0.2.0 | Solo linterna o histórico |
| `reinicia(r)? el contador\|sala clave reinicia\|consecutivas` | Todas las apariciones vigentes indican que solo la sala final del piso 2 reinicia el contador (Jugabilidad, Mecánicas M-15/M-18, Sistemas, Progresión, Reglas). "consecutivas" solo en la fila histórica D-08 del Registro | Coherente con D-35 |
| `30 minutos` | Solo la duración del showcase (Público y Plataforma, Ficha) | Coherente con D-34 |
| `entra en alerta\|enemigo en alerta` | Pilares de Diseño (corte de la vigilancia) y Mecánicas M-13 (hackeo continúa) | Coherente: D-23 define "en alerta" = caza para el corte y el bloqueo. Sin cambios en Pilares (fuera de la lista de Task 7) |

## Estructura de las notas

Comprobación con script sobre las 15 notas de `01 - Visión/`, `02 - Diseño/` y `10 - Referencia/` (fuera de bloques de código):

- **Navegación:** cada nota tiene exactamente una línea `> **Navegación:** ...` y es la última línea con contenido (en `04 - Pilares de Diseño.md` la comparación automática dio falso negativo; la revisión manual confirma que la línea final es la navegación, sin cambios desde 2026-10-05).
- **"## Descripción de la sección":** presente una vez en cada nota.
- **Negritas abiertas:** ninguna línea con número impar de `**`.
- **Wikilinks:** todos resuelven a notas existentes de la bóveda. Única excepción preexistente: `imagen.png` embebida en `10 - Referencia/02 - Referencias e Inspiración.md` (archivo no presente en la bóveda; nota sin cambios desde 2026-10-05, fuera de este bloque).

## Teclas por contexto (`02 - Diseño/04 - Controles.md`)

| Contexto | Asignaciones | Conflictos |
|---|---|---|
| Exploración (OTS) | `WASD` mover, `Shift` correr, `Ctrl` agacharse, `Clic der.` apuntar, `Clic izq.` empujón, `R` recargar, `E` interactuar, `F` linterna, `Q` vigilancia, `Tab` inventario, `Esc` pausa | Ninguno |
| Apuntando | `Clic izq.` disparar | Ninguno |
| Inventario abierto | `Clic izq.` usar/equipar, `Clic der.` descartar, `Tab` cerrar | Ninguno |
| Hackeo | `WASD`/`Flechas` secuencia, `E` abandonar | Ninguno |
| Vista de vigilancia | `Q` salir; correr/apuntar/empujar vuelven a OTS | Ninguno |
| Sobre un cuerpo (sin apuntar) | `Clic izq.` golpe en el suelo, `E` consumible de remate; prioridad por centro de la vista frente a otros interactivos (D-27) | Ninguno (sustituyen a empujón e interactuar dentro del contexto) |
| Menús | `Clic izq.` seleccionar (incluye "Reiniciar"), `Esc` abrir/cerrar pausa | Ninguno |
| Cinemática | `Espacio` saltar la cinemática corta | Ninguno |
| Créditos | `Espacio` saltar (ya vistos) | Ninguno |

## Archivos fuera de alcance

Comprobación por fecha de última modificación (la bóveda no tiene commits de los bloques 2 y 3; `git diff` se compara contra 2026-09-28):

- `Christian Decisions.md`: 2026-10-05 13:12 — sin cambios.
- Carpetas `04` a `09` y `11`: todas las notas en 2026-10-05 10:43 — sin cambios.
- Carpeta `03 - Historia y Mundo`: `01 - Premisa y Trama.md`, `02 - Mundo.md` y `04 - Diálogos y Narrativa.md` en 2026-10-05 10:43 — sin cambios.
- **Hallazgo:** `03 - Historia y Mundo/03 - Personajes.md` tiene fecha 2026-10-07 11:57 y contiene una ficha nueva del protagonista ("Lukas Reiner-Hellen", esposa "Emilia Reiner-Hellen", "Estado Clon", uso de "TBD" y "runs"). No figura en el informe de la Fase A ni en ningún documento de control de `.agents/tasks/`, y su contenido contradice D-18 (reconstrucción, no clonación) y la premisa D-13 (esposa fallecida). No se ha modificado ni revertido en esta tarea (fuera de alcance y posible trabajo del equipo). Requiere confirmación del responsable: si es contenido propio del equipo, conservarlo y reconciliarlo con D-13/D-18 al trabajar `03 - Historia y Mundo`; si no lo es, restaurarlo.

## Iteración 2 — Correcciones de `gdd-bloque3-review.json`

| # | Hallazgo | Corrección | Archivo |
|---|---|---|---|
| 1 | S-07 contradice a Reglas sobre la continuación sin autoguardado | Filas "Con calma…" y "Cierre inesperado…" de la tabla de salida: "o, si no existe, inicio del ciclo". El "Por definir" de dónde se continúa sin autoguardado se sustituye por "inicio del ciclo" (regla global 4 de Reglas). La remisión "se precisa en Reglas" para los estados que cuentan como "alerta" se sustituye por "**Por definir**", como en la regla 4 | `02 - Diseño/03 - Sistemas.md` |
| 2 | Llave del piso 3 en S-06 | Se quita el "Por definir"; la llave obligatoria del piso 3 abre la puerta de la sala de The Mother, que ocupa el lugar de la sala clave final (M-15, D-36), con enlace a Economía | `02 - Diseño/03 - Sistemas.md` |
| 3 | Showcase sin enemigos en Progresión | Tabla de configuraciones: remite a la curva (enemigos) y a Economía (recursos), factor ~0.75. Curva de dificultad: enemigos del showcase piso 1, 5–7; piso 2, 7–9; piso 3, 9–11 + The Mother (factor 0.75 sobre 6–9 / 9–12 / 12–15, redondeado; valores iniciales, sujetos a balance) | `02 - Diseño/05 - Progresión.md` |
| 4 | Criterios de S-02 no comprobables | Reescritos en términos de perseguidores ("de los enemigos que perseguían, solo los que estaban en caza llegan…"; "ninguno de los perseguidores llega…") | `02 - Diseño/03 - Sistemas.md` |
| 5 | Excepción del primer intento en "Salir con calma a mitad de piso" | La excepción se limita al contador (regla global 8); si suma a la degradación queda **Por definir** (S-08) | `02 - Diseño/07 - Reglas y Estados del Juego.md` |
| 6 | `03 - Personajes.md` fuera de alcance | Sin cambios desde este workflow (fecha 2026-10-07 11:57, igual que en la iteración 1). Pendiente de confirmación del responsable | — |

Comprobaciones de la iteración 2:

- **Coherencia de los valores del showcase:** criterio de munición de Economía (máximo < mínimo de enemigos × 3): piso 1, 11 < 15; piso 2, 9 < 21; piso 3, 6 < 27. Criterio de S-06 (enemigos por sala dentro del rango de la configuración completa): piso 1, 0.45–0.78 frente a 0.40–0.82; piso 2, 0.64–1.00 frente a 0.60–1.09; piso 3, 0.82–1.22 frente a 0.80–1.36. Ambos se cumplen.
- **Búsqueda** `(showcase|Showcase).*Por definir|sala de The Mother usa la llave` en `01 - Visión/`, `02 - Diseño/` y `10 - Referencia/`: sin resultados vigentes (solo la fila 0.4.0 del Historial, que describe el cierre).
- **Estructura** de las 3 notas editadas (script): una línea `> **Navegación:** ...` como última línea, "## Descripción de la sección" una vez, ninguna línea con número impar de `**`. Wikilinks: todos resuelven (única excepción preexistente, `imagen.png`).
- **Fuera de alcance:** `Christian Decisions.md` (2026-10-05 13:12) y carpetas 04 a 09 y 11 (2026-10-05 10:43) sin cambios; en la carpeta 03 solo `03 - Personajes.md` tiene fecha 2026-10-07 11:57 (hallazgo 6, sin tocar).

## "Por definir" restantes

Los "Por definir" que quedan en `02 - Diseño/` corresponden a lore/contenido (Bloque G), a controles (reasignación y columna Control) o a detalles no cubiertos por D-23..D-39 que ya listan los informes de la Fase A y la Fase B (p. ej. distancia mínima de la sala clave final, número de puntos débiles de la fase 3, qué estados de M-10 cuentan como "alerta" para el costo de salir, si la cinemática completa se puede saltar).
