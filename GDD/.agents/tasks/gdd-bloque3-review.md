# Cierre de los "Por definir" de diseño del GDD (Bloque 3, v0.4.0), segunda pasada

Esta pasada revisa las correcciones de la iteración 2 sobre los seis hallazgos de la primera revisión. Las correcciones tocan S-02, S-06 y S-07 en Sistemas, las configuraciones y la curva de dificultad en Progresión, y un caso especial en Reglas y Estados. Los tres hallazgos bloqueantes y los dos menores están corregidos en el texto final. Las correcciones no traen restos de suspensión ni de 2 pisos. Tampoco añaden salas que reinicien el contador o autoguarden, ni lore inventado, ni cambios de formato. El resto de las notas no ha cambiado desde la primera pasada, que ya contrastó D-23..D-39.

Watch for: `03 - Historia y Mundo/03 - Personajes.md` sigue con contenido nuevo fuera de alcance que contradice D-13 y D-18. No bloquea, pero el responsable tiene que confirmarlo (confirmed).

**Verdict**: APPROVED

## High-level view

S-07 coincide ahora con la regla global 4 de Reglas y Estados. Sin autoguardado, salir con calma o un fallo llevan al inicio del ciclo. Las dos notas dejan "Por definir" qué estados, además de la caza, cuentan como "alerta" para el costo de salir.

La llave del piso 3 tiene una sola versión. S-06, Economía, M-15 y D-36 dicen que abre la sala de The Mother.

Progresión da los enemigos por piso del showcase: 5–7, 7–9 y 9–11, más The Mother. Salen del factor ~0.75 de Economía, llevan "(valores iniciales, sujetos a balance)" y remiten a Economía para los recursos. La remisión de Economía a Progresión ya apunta a contenido que existe.

Los criterios de S-02 hablan ahora de perseguidores y se pueden comprobar. En Reglas, la excepción del primer intento al salir con calma se limita al contador. Si suma a la degradación queda "Por definir", igual que en S-08. Nadie ha tocado `03 - Personajes.md` en este workflow, así que sigue pendiente.

<details>
<summary>Issues (1)</summary>

1. **Personajes.md modificado fuera de alcance** (no bloqueante): el responsable debe confirmar si la ficha nueva de `03 - Historia y Mundo/03 - Personajes.md` (protagonista y esposa con nombre propio, "Estado Clon", "TBD", "runs") es del equipo. Si lo es, se reconcilia con D-13 y D-18 al trabajar `03 - Historia y Mundo`. Si no, se restaura. No se edita desde este workflow.

</details>

<details>
<summary>Details</summary>

## Cierre forzado durante una caza

Reglas deja "Por definir" cómo se distingue un fallo de un cierre forzado por el jugador. S-07 distingue la salida confirmada del cierre inesperado con una marca en el archivo de guardado, y deja "Por definir" la mitigación del cierre forzado durante una caza. Mientras no se defina, el jugador puede forzar el cierre durante una caza para evitar el costo de D-31 (confirmed). Las dos notas lo declaran abierto y D-23..D-39 no lo cubren, así que no bloquea la versión. Queda como pendiente de diseño.

## Cambio fuera de alcance en Personajes

`03 - Personajes.md` tiene fecha 2026-10-07 11:57, anterior a las iteraciones de Task 7. Contiene un protagonista y una esposa con nombre propio, un "Estado Clon" y los términos "TBD" y "runs". Contradice D-18 (reconstrucción, no clonación) y D-13 (confirmed en la primera pasada y en la verificación). Ninguna nota de diseño de v0.4.0 depende de ese contenido.

</details>

<details>
<summary>Archivos revisados</summary>

- `02 - Diseño/03 - Sistemas.md`: S-02 (criterios de persecución entre pisos), S-06 (llaves, parámetros y criterios por configuración), S-07 (tabla de salida y continuación sin autoguardado).
- `02 - Diseño/05 - Progresión.md`: tabla de configuraciones, curva de dificultad con enemigos del showcase, etiqueta de la degradación (D-08, D-29), navegación final.
- `02 - Diseño/07 - Reglas y Estados del Juego.md`: casos especiales "Salir con calma a mitad de piso" y "Cierre inesperado", regla global 8, estado Muerte.
- `.agents/tasks/gdd-bloque3-verificacion.md`: búsquedas de términos obsoletos, estructura, wikilinks, teclas por contexto y archivos fuera de alcance, tomadas de la nota sin volver a ejecutarlas.

La única comprobación propia fue una búsqueda de "primer intento" en Reglas y Estados. Confirmó la corrección 5 y su coherencia con la regla global 8.

</details>
