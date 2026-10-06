# Verificación GDD 0.2.0 (iteración 2)

## Correcciones de la revisión (iteración 2)

1. BLOQUEANTE diagrama M-01: `Vigilancia --> OTS: salir / correr, apuntar o empujar / recibir daño` (ya no "moverse"). Las transiciones a OTS_Bloqueado ahora dicen "enemigo en alerta (ver M-10)" y "detectado (enemigo en alerta)".
2. BLOQUEANTE término "alerta": M-10 tiene una regla nueva que define **en alerta** como el término que usan M-02 y M-03, con umbral **Por definir** (búsqueda o caza). No se tomó decisión nueva. M-02 (Reglas), M-03 (Activación y Reglas) y el diagrama remiten a ese umbral; ya no aparece "estado de caza" ni "alerta o caza" fuera de M-10. Ficha, Pilares, Jugabilidad y D-04 ya usaban "en alerta", así que coinciden sin cambios.
3. BLOQUEANTE marcas de balance: "(valor inicial, sujeto a balance)" añadido en M-11 (Reglas, Estado final, Recompensa), M-17 "Demo: 1-2 fallas" y la ficha D-14 del Registro.
4. NO BLOQUEANTE fichas D-03, D-05, D-08..D-12: no se añadieron. La tarea pedía fichas solo para D-01, D-02, D-04, D-06, D-07, D-13..D-16. Para las demás habría que inventar "Opciones consideradas" que no están registradas, y eso sería contenido nuevo. Siguen en la tabla Registro.
5. NO BLOQUEANTE viñeta vacía en Pilares: eliminada la línea `- ` al final de "Reglas de decisión".

Archivos tocados en esta iteración: 02 - Diseño/02 - Mecánicas.md, 10 - Referencia/03 - Registro de Decisiones.md, 01 - Visión/04 - Pilares de Diseño.md.

Punto nuevo "Por definir": umbral de "en alerta" dentro de sospecha → búsqueda → caza (M-10).

# Verificación GDD 0.2.0 (iteración 1)

## Archivos cambiados

- 01 - Visión/01 - Ficha del Proyecto.md (versión 0.2.0, fecha 2026-10-05, género principal Survival Horror, justificación de cámara D-01/D-04, etiquetas, premisa bajo Elevator Pitch)
- 01 - Visión/02 - Visión del Juego.md (premisa en Concepto central, Fantasía del jugador, frase de cámara en Límites)
- 01 - Visión/03 - Público y Plataforma.md (desmarcadas "Interfaz legible" y "Guardado compatible"; "Esquivar..." → "Realizar acciones defensivas (empujón / golpe de culata)")
- 01 - Visión/04 - Pilares de Diseño.md (Pilar 3 renombrado; "La cámara debe tener intención" alineado a D-01/D-04)
- 02 - Diseño/01 - Jugabilidad.md (condición de victoria D-16, final de 5 muertes D-08 con enlace a [[02 - Mecánicas]], principio 5 alineado a D-01)
- 02 - Diseño/02 - Mecánicas.md (reescrito: catálogo, M-01..M-19, Post-demo, Descartadas, matriz, 2 diagramas mermaid)
- 02 - Diseño/04 - Controles.md (mapa sin columna Táctil ni fila Saltar, contextos de entrada, Reasignación, Entrada y retroalimentación)
- 10 - Referencia/03 - Registro de Decisiones.md (fichas D-01, D-02, D-04, D-06, D-07, D-13, D-14, D-15, D-16; tabla D-01..D-16)
- 10 - Referencia/04 - Historial de Versiones.md (fila 0.2.0)
- Decisiones a Implementar.md (premisa confirmada D-13; The Mother jefe final D-16; marcas "Confirmado/Confirmada" en Jefes y Condición de victoria)

Comparación MD5 antes/después: solo los 10 archivos anteriores cambiaron. Christian Decisions.md, 03 - Sistemas, 05-07 y carpetas 03-09 y 11 sin cambios.

## Búsquedas (vault completo, excluyendo Christian Decisions.md y .agents)

| Término | Ocurrencias restantes | Valoración |
|---|---|---|
| Presión Constante | 1 — 01 - Jugabilidad.md:168 ("presión constante reduciría el impacto del terror", texto original de Ritmo) | No contradictorio; es el motivo de D-10 |
| omnisciente | 2 — Ficha:45 ("no proporciona información omnisciente"), Registro D-02 (Problema) | No contradictorio (negación / histórico) |
| Lineal | 1 en Registro (fila D-11, histórico); 11 - Referencia de Diseño (biblioteca, fuera de alcance) | Etiqueta eliminada de la Ficha |
| Liberation | 0 | — |
| Evasión Corta | 1 — Registro fila D-05 (histórico de la eliminación) | No contradictorio |
| esquiv | Mecánicas tabla Descartadas; 11 - Géneros.md (fuera de alcance) | No contradictorio |
| invulnerab | Mecánicas Descartadas; Registro D-05; Historial 0.2.0 | Solo como eliminado/descartado |
| cargas | 0 | — |
| 3/3 | 0 | — |
| QTE | 0 | — |
| cono (palabra) | Mecánicas M-12 (linterna, 2 líneas); Mecánicas Descartadas; Registro D-02 y fila D-02 | Ningún cono de visión enemigo activo |
| "forzar OTS" / "regresa a la vista isométrica" | 0 | — |

## Otras comprobaciones

- Las 19 mecánicas contienen los 11 campos de plantilla + **Impacto en pilares** (comprobado por script).
- Ninguna línea con "- -" en el vault.
- Ninguna línea con número impar de "**" en los archivos editados.
- Todos los wikilinks de los archivos editados apuntan a notas existentes.
- Línea de navegación final presente en los 9 archivos que la tenían (Decisiones a Implementar.md no tenía).
- Controles: ninguna tecla con dos funciones en el mismo contexto (Clic izquierdo: empujón sin apuntar / disparo apuntando / usar en inventario; E: interactuar / abandonar hackeo; WASD: mover / secuencia con personaje inmóvil).
- Toda mecánica mencionada en el GDD (manipulación de cajas, saqueo de cadáveres, rejillas, etc.) aparece en el catálogo o en Post-demo/Descartadas.

## Puntos "Por definir"

- Mínimo de salas con cámara por intento (M-02) y distancia mínima de la sala clave (M-15).
- Cargador y munición por casilla (M-04); % de reducción de velocidad en Herido/Crítico (M-05); costo de resistencia del empujón (M-06).
- Comportamiento de enemigos que siguen por escaleras (M-15).
- Si la degradación estética se reinicia al llegar a una sala clave (M-18).
- Comportamiento de The Mother en la fase 2 y forma de la vista hostil (M-19).
- Relación entre la esposa, la instalación y The Mother (lore).
- Reasignación de controles y columna Control (soporte posterior).
