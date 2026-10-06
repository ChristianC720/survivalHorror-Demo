# Bloque 2 — Fase A (Task 1 y Task 2)

**Fecha:** 2026-10-06 · **Responsable:** Christian Coronel Palma

## Archivos tocados

- `02 - Diseño/02 - Mecánicas.md`
- `02 - Diseño/04 - Controles.md`
- `10 - Referencia/03 - Registro de Decisiones.md`

## Task 1 — Terminología de reconstrucción

- M-16 renombrada a "Reaparición por reconstrucción y terminal del brazo" (título, catálogo, matriz y referencias "M-16 Ciclo de muerte" en M-05, M-07, M-08, M-15, M-17 y M-18).
- M-16 Reglas: "se reconstruye en el punto de reaparición (ciclo de muerte y reconstrucción)". Estado final: "Personaje reconstruido en el punto de reaparición…".
- Registro: fila D-12 conservada y marcada "Modificada por D-18". Filas D-17 a D-22 (2026-10-06). Fichas completas D-18, D-20 y D-21.
- Sin tocar: D-13, `Decisiones a Implementar.md`, `11 - Referencia de Diseño`, la ficha D-06 (histórica).
- Verificación: "clon" y "Permadeath" solo quedan en el Registro (D-12, D-13, ficha D-18), `Decisiones a Implementar.md` y `11 - Referencia de Diseño`.

## Task 2 — M-20 Reciclaje de enemigos y remate

- M-20 creada con plantilla completa (D-20 y D-21; valores no decididos "Por definir"; ~2 s "valor inicial, sujeto a balance"). Enlaza [[06 - Economía y Recursos]] y [[04 - Controles]].
- Catálogo: M-20, Demo, V, DC.
- Matriz: M-20 depende de M-06, M-07, M-10; afecta a M-09, M-14, M-19. Filas M-06, M-07, M-09, M-10, M-14 y M-19 actualizadas.
- Interacciones actualizadas en M-06 (consumo de resistencia por golpe en el suelo), M-07 (casilla del consumible), M-09, M-10 (aviso de maquinaria), M-14 y M-19 (reciclaje en la sala del jefe: Por definir).
- M-14 Penalización: la sala de la que se huyó sigue infestada, incluidos los reciclados.
- Controles: filas "Remate con golpe en el suelo" (`Clic izquierdo` sin apuntar sobre un cuerpo) y "Remate con consumible" (`E` sobre un cuerpo); contexto "Sobre un cuerpo (sin apuntar)" que sustituye empujón/interactuar en ese contexto; retroalimentación del remate.
- Verificación: sin invulnerabilidad (D-05), salud sin cambios (D-07), el consumible no sustituye al objeto defensivo (D-14). Ninguna tecla con dos funciones en el mismo contexto.

## Observaciones no bloqueantes aplicadas

- M-02: el corte por alerta usa el mismo alcance que M-03.
- M-15 y M-19: funcionan sin M-18 / M-17 (Deseables).
