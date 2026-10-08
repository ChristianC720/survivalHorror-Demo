# Bloque 4 — Fase A (Tasks 1, 2 y 3)

**Fecha:** 2026-10-08 · **Responsable:** Christian Coronel Palma

## Archivos tocados

- `10 - Referencia/03 - Registro de Decisiones.md`
- `02 - Diseño/02 - Mecánicas.md`
- `02 - Diseño/07 - Reglas y Estados del Juego.md` (solo renombre)
- `02 - Diseño/01 - Jugabilidad.md` (solo renombres)
- `02 - Diseño/03 - Sistemas.md` (solo renombres)
- `02 - Diseño/05 - Progresión.md` (solo renombres)
- `Decisiones a Implementar.md`
- `01 - Visión/01 - Ficha del Proyecto.md`: **sin cambios**; "Nombre del proyecto" ya era "Identity Dissolution" y "Nombre clave" = Heartbeat. Versión (0.4.1) y fecha (2026-10-07) se dejan para la Task 9.

## Nombres exactos vigentes

| Concepto | Texto exacto |
|---|---|
| Final de 5 muertes (nombre) | **"Asimilación"** (D-42) — en prosa: `el final "Asimilación"` |
| Estado en Reglas y Estados (tabla, transiciones, casos especiales) | `Final: Asimilación` (Game Over) |
| Estado en mermaid de Reglas y Estados | `state "Final: Asimilación" as Asimilacion` (alias `Asimilacion`) |
| Estado en S-08 de Sistemas ("Estados posibles") | `Final: Asimilación` |
| Degradación sonora | `distorsión del sonido vocal` (D-52) |
| Documentos incompletos | `documentos corruptos` / `corrupto` (D-45); "fallas" solo para M-17 |
| Cinemáticas de victoria | `Relevo básico`, `Relevo completo`, `Ambiguo` |
| Evento común de victoria | `Despertar` |
| Interactuables de la decisión final | `terminal central` y `salida` (ambos con `E`, Interactuar) |
| Código de sujeto | `R-H-07` (primera reconstrucción), `R-H-08`…; Emilia `R-H-01 a R-H-06`, "estado desconocido" |
| "Disolución de identidad" | Solo como tema (D-44) o como texto histórico en el Registro (D-08, D-35, ficha D-42) |

Nombres de mecánicas **sin cambios** (catálogo y matriz): M-08 Interacción, recolección e investigación de documentos; M-10 Señales diegéticas; M-16 Reaparición por reconstrucción y terminal del brazo; M-18 Degradación por muerte y final de 5 muertes; M-19 Jefe final: The Mother; M-20 Reciclaje de enemigos y remate. Catálogo: M-18 estado "Definida (final: Asimilación, D-42)"; M-19 estado "Definida (duración por balancear; decisión final y 3 cinemáticas de victoria, D-51)".

## Task 1 — Registro

- Fichas nuevas (después de D-40): D-42, D-44, D-45, D-47, D-49, D-51.
- Filas D-41 a D-52 (2026-10-08), cada una con "Motivo: <pilar>" (D-41 sin pilar).
- D-08 fila: "**Modificada por D-35**. **Modificada por D-42** (…"Asimilación") y **por D-52** (…"distorsión del sonido vocal")".
- D-16: fila "**Precisada por D-51**"; ficha con "**Estado:** Precisada por D-51 …".
- D-35: fila "**Modificada por D-42** (solo el nombre del final…)"; ficha con "**Estado:** Modificada por D-42 …". Su Motivo conserva "Disolución de identidad" como histórico.
- D-40: fila "**Precisada por D-49**"; ficha con "**Estado:** Precisada por D-49 …".

## Task 2 — Mecánicas (cómo quedaron)

- **M-08:** reglas nuevas "Documentos de Emilia (D-52)" (6, 2 por piso, siempre en salas clave secundarias, también en showcase; únicos que cuentan para M-19), "Documentos de Vitalis Konzern y de The Mother (D-52)" (salas procedurales, no cuentan), "Documentos corruptos (D-45)" y "Reacción de Lukas (D-52)". Casos límite: lo necesario para avanzar siempre es legible; contenido de una sala secundaria cuyo documento ya está registrado = **Por definir**. Interactúa con M-19.
- **M-10:** regla "Latido del laboratorio (D-46)": timbre grave distinto del latido del protagonista, nunca depende de la cercanía de enemigos, solo crece con la degradación (M-18) y se acelera en el Despertar (M-19). Caso límite con el tope de D-29.
- **M-16:** cinemática completa = "el momento del olvido" (D-39, D-45); cómo muere Lukas = **Por definir** (D-50). Reglas nuevas "Código de sujeto (D-43, D-47, D-50)" (R-H-07 y +1 por reconstrucción, incluido Reiniciar; solo presentación; salida con calma = **Por definir**), "Mensaje de Emilia (D-50)" (tablet/teléfono corporativo, se pierde en la primera reconstrucción, copiado corrupto en el terminal, se completa en el desenlace) y "Bestiario (D-48)". Casos límite: primer intento sube el código; Nueva partida vuelve a R-H-07.
- **M-18:** "distorsión del sonido vocal (D-52)"; regla "Cuerpo ajeno visible (D-43)" (una pieza ajena más por reconstrucción, 3–5 variantes, valor inicial, sujeto a balance; mano del anillo; se mantiene en la última variante; solo presentación); final **"Asimilación"** (D-35, D-42, D-48) con enlace al estado "Final: Asimilación".
- **M-19:** "Secuencia común de victoria (D-51)" (5 pasos), "Decisión final (D-51)" (terminal central y salida con `E`, sin menú; sin los 6 documentos el examen es automático) y tabla "Cinemáticas de victoria (D-51)". Casos límite: el 6.º documento del mismo intento cuenta; documentos de Vitalis/The Mother no cuentan; amenazas o muerte durante la decisión = **Por definir**.
- **M-20:** regla "Justificación del rastro de identidad (D-47, D-48)".
- **Matriz:** M-08 afecta +M-19; M-10 depende de +M-18, M-19; M-16 afecta +M-19; M-19 depende de +M-08, M-16 y afecta +M-10.
- **Controles:** sin cambios. `E` mantiene una sola función (Interactuar) en la decisión final; si terminal y salida coinciden en alcance, gana el centro de la vista (criterio de D-27).

## Task 3 — Decisiones a Implementar.md

- Nota inicial en "Personaje Principal": en complexión y vestimenta manda la ficha de [[03 - Personajes]].
- Premisa 1 → "(Decisión confirmada — D-46, D-47)"; Premisa oficial → "precisada por D-49"; 03.2 Horror cósmico → "(Decisión confirmada — D-44)" + bloque de tema; 03.3 La criatura → "Decisión confirmada (D-46, D-47, D-48)"; 03.4 The Mother → "Decisión confirmada (D-47, D-51)"; lista Narrativa: The Mother (D-47) y Criatura (D-46) confirmadas.

## Para la Fase B

- **Task 8 (Reglas y Estados):** el estado Victoria y sus transiciones siguen como en v0.4.1; falta añadir el estado "Decisión final" y las 3 cinemáticas (Relevo básico, Relevo completo, Ambiguo) referenciando M-19 y D-51. El estado de derrota ya se llama "Final: Asimilación".
- **Task 8 (Sistemas):** S-08 ya usa "Final: Asimilación" y "final "Asimilación" (D-42)"; S-11 ya usa "distorsión del sonido vocal". Pendiente: S-06 (documentos de Emilia en secundarias), S-07 (código R-H y 6 documentos), S-08 (cinemáticas finales), S-11 (latido del laboratorio), según M-08/M-10/M-16/M-19.
- **Task 8 (Progresión):** el final secundario ya cita "D-08 modificada por D-35 y D-42"; falta el desbloqueo de la elección (6 documentos, M-19).
- **Task 4–7:** enlazar M-08 (documentos corruptos), M-16 (olvido, R-H, mensaje), M-18 (Asimilación, cuerpo ajeno), M-19 (finales) y D-41…D-52.
