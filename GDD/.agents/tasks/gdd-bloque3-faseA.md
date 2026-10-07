# Bloque 3 — Fase A (Task 1 y Task 2)

**Fecha:** 2026-10-07 · **Responsable:** Christian Coronel Palma

## Archivos tocados

- `10 - Referencia/03 - Registro de Decisiones.md`
- `02 - Diseño/02 - Mecánicas.md`
- `02 - Diseño/04 - Controles.md`

## Task 1 — Registro de Decisiones

- Fichas completas nuevas (después de D-21): D-23, D-24, D-30, D-31, D-33, D-35, D-39.
- Filas D-23 a D-39 (2026-10-07). Cada fila sin ficha incluye "Motivo: <pilar>".
- D-08 → "**Modificada por D-35**", D-15 → "**Modificada por D-33**", D-22 → "**Modificada por D-30**" (filas conservadas). La ficha D-15 recibe una línea "**Estado:** Modificada por D-33" (el reinicio del contador queda sustituido por D-35).

## Task 2 — Mecánicas

- M-01 (diagrama): umbral = caza; estado nuevo `Vigilancia_Degradada` (búsqueda).
- M-02: activación sin enemigos en caza; cobertura D-25; tabla de señal escalonada D-23; atención de The Mother D-24 (~5 s / ~15 s); casos límite nuevos.
- M-03: bloqueo solo con enemigos en caza; búsqueda no bloquea pero degrada.
- M-04: cargador 6, 12 balas por casilla, 3–4 disparos (reciclado 2). M-05: −15% / −30%. M-06: correr ~10%/s, empujón 35%, golpe en el suelo 25%.
- M-07: llaves como objetos clave (1 casilla, se consumen), máximos por casilla D-37, inventario incluido en el autoguardado. M-08: llave obligatoria/opcionales, salas clave secundarias (contenido Por definir por lore), prioridad por centro de la vista frente a cuerpos.
- M-09 y M-10: atención D-24, "en alerta" = caza, señal escalonada, cámaras hostiles D-28, tope D-29.
- M-15: reescrita con D-33 (tabla de tipos de sala), D-34 (tabla de configuraciones), enemigos por piso D-37, tutorial D-38, llaves D-36, autoguardado D-30, contador D-35, persecución D-26, transición ≥5 s D-39. Diagrama sustituido por "Estructura del intento" de la especificación. Pilares en catálogo: A, DC.
- M-16: cinemáticas completa y corta (D-39), Reiniciar y Nueva partida (D-32), reaparición siempre en el piso 1, autoguardado descartado al morir o reiniciar.
- M-18: D-29 (solo acumula, tope visual) y D-35 (solo la sala final del piso 2 reinicia el contador; primer intento no cuenta; Reiniciar y salir con costo suman).
- M-19: piso 3, sin autoguardado, fase 2 con cámaras hostiles, munición de la fase 3, sin reciclaje.
- M-20: reciclado +25% velocidad y 2 disparos, golpe 25%, consumible hasta 2 por casilla, disponibilidad D-27, sin reciclaje en la sala del jefe.
- Catálogo (estados de M-04, M-05, M-06, M-15, M-20) y matriz (M-02, M-04, M-07, M-08, M-09, M-10, M-15, M-16, M-18, M-19, M-20) actualizados.

## Task 2 — Controles

- Filas nuevas: seleccionar opción de menú (incluye "Reiniciar"), saltar cinemática (`Espacio`). Remate con frontera D-27.
- Contextos nuevos: Menús (pausa y principal), Cinemática, Créditos. Ninguna tecla con dos funciones en el mismo contexto (`Espacio` solo se usa en Cinemática y Créditos).
- Retroalimentación: Reiniciar y Saltar cinemática.

## "Por definir" que quedan en Mecánicas (todos fuera de D-23..D-39 o de lore)

- Objetos clave concretos distintos de las llaves y contenido de documentos (M-08, lore).
- Contenido de la cinemática completa (M-16, lore).
- Distancia mínima de la sala clave final (M-15).
- Número de puntos débiles de la fase 3 (M-19).
- Proporción de cuerpos reciclados, tiempo hasta el aviso y duración del consumible de remate (M-20).
- Si el contexto de remate se activa con cuerpos que no pueden reciclarse (reciclados, sala del jefe) (M-20).
- En Controles, además: si la cinemática completa se puede saltar; reasignación y columna Control.

## Referencias listas para la Fase B

- **Task 3 (Economía):** M-07 (máximos por casilla, llaves), M-15 (enemigos por piso, showcase escalado), M-19 (munición de la fase 3), M-20 (consumible de remate). D-34, D-36, D-37.
- **Task 4 (Reglas y Estados):** M-15 y M-16 enlazan [[07 - Reglas y Estados del Juego]] para D-30, D-31 y D-32; Controles enlaza Reglas para el menú principal y el diálogo de salida. Estados: cinemática de introducción, reconstrucción corta y transición de piso (D-39). Gracia al cargar 2–3 s (D-30, D-37).
- **Task 5 (Sistemas):** S-01/S-02 ← M-02, M-03, M-10, M-15 (D-23, D-24, D-26); S-06 ← M-15 (D-33, D-34, D-36, D-38); S-07 ← M-07/M-15/M-16 (D-30); S-08 ← M-16/M-18 (D-29, D-32, D-35, D-39); S-09 ← M-20 (D-27, D-28); S-10 ← M-19 (D-28); S-11 ← M-02/M-10 (degradación de la señal, cámaras hostiles).
- **Task 6 (Progresión):** M-15 (3 pisos, tutorial D-38, enemigos por piso, showcase), M-18 (D-35).

## Observación para la Fase B

- D-31 usa literalmente "durante alerta o caza". Con D-23, "en alerta" = caza solo para la vigilancia. Al redactar Reglas y Estados conviene precisar qué estados de M-10 cuentan como "alerta" para el costo de salir (sin decidirlo si no está aprobado).
