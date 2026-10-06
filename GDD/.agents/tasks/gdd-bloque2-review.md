# Bloque 2 del GDD Heartbeat (v0.3.0): plantillas de Diseño, reconstrucción y reciclaje de enemigos (revisión 2)

El Bloque 2 rellena las plantillas de Sistemas, Progresión, Economía y Recursos y Reglas y Estados. También cambia la terminología de clonación por reconstrucción (D-18, D-19), agrega M-20 Reciclaje de enemigos y remate (D-20, D-21) con su acción contextual en Controles y registra D-17..D-22. Esta es la segunda pasada. Los cuatro hallazgos de la primera están resueltos en el texto: la persistencia de fallas ya aplica D-09 igual en todas las notas, la frontera del contexto "sobre un cuerpo" está marcada **Por definir** con la restricción de la ruta garantizada, Progresión ya no remite a Economía para el número de enemigos y la regla de las 6 casillas se limita al piso 1. La terminología, la navegación, los wikilinks y los archivos fuera de alcance no se volvieron a verificar; se aceptan con la evidencia de `gdd-bloque2-verificacion.md`. El lore pendiente (esposa, instalación, The Mother, revelación final y origen de las piezas ajenas) sigue **Por definir**.

Watch for: dos imprecisiones no bloqueantes. La tabla de transiciones de Reglas y Estados asocia el reinicio del contador de muertes a usar la escalera o el elevador, mientras que la regla global 5 lo asocia a llegar a la sala clave (likely). M-02 restringe ahora el corte de la vigilancia al alcance de M-03, pero la ficha de D-04 sigue diciendo "cualquier enemigo en alerta" (confirmed).

**Verdict**: APPROVED

## High-level view

La frontera del contexto "sobre un cuerpo" en Controles y M-20 deja abiertos la distancia, los cuerpos rematados o reciclados, el uso en Alerta/Combate y la prioridad frente a los interactuables. Solo fija la restricción que deriva de D-15: un cuerpo no puede bloquear la ruta garantizada. No se inventó ninguna prioridad que el equipo no haya decidido.

Reglas y Estados tiene dos fuentes para el reinicio del contador de muertes. La regla global 5 lo asocia a llegar a la sala clave y una fila de transiciones, a usar la escalera o el elevador. Solo importa si M-18 (Deseable) se implementa, pero puede cambiar cuándo se dispara el final de disolución.

La precisión de M-02 (cortar la vigilancia con el alcance de M-03) no se reflejó en la ficha de D-04, que sigue diciendo "cualquier enemigo en alerta". La mecánica y la decisión registrada no dicen lo mismo.

<details>
<summary>Issues (2)</summary>

1. **Momento del reinicio del contador (no bloqueante)**: la fila de transición "Usar una escalera o un elevador en una sala clave" dice "Reinicia el contador de muertes", y la regla global 5, M-15, M-18 y el caso "Morir en la sala del jefe" lo reinician al llegar a la sala clave. Cambiar la condición de la fila por "El contador ya se reinició al llegar a la sala clave (regla 5)".
2. **Alcance del corte de vigilancia frente a D-04 (no bloqueante)**: M-02 corta la vista solo con enemigos en alerta "dentro de la sala o en el área cercana con ruta de persecución" (mismo alcance que M-03), pero la ficha de D-04 dice "con cualquier enemigo en alerta". Anotar la precisión en el Registro (por ejemplo, "Precisada en v0.3.0: mismo alcance que M-03") o confirmar con el equipo que la lectura de D-04 es esa.

</details>

<details>
<summary>Details</summary>

## Reinicio del contador de muertes en Reglas y Estados

La regla global 5 dice "llegar a una sala clave reinicia el contador de muertes consecutivas (M-15, M-18)". El caso especial "Morir en la sala del jefe" se apoya en esa lectura: "el contador de muertes ya se había reiniciado al entrar". M-15, M-18, D-15 y la etapa 3 de Progresión dicen lo mismo. En cambio, la tabla de transiciones pone el reinicio como condición de otro evento:

```
| Jugando | Usar una escalera o un elevador en una sala clave | Jugando (piso siguiente o sala de The Mother) |
  Reinicia el contador de muertes (M-15, M-18). ...
```

El caso que separa las dos lecturas es morir dentro de la sala clave 1 antes de usar la escalera, por ejemplo durante el hackeo largo del elevador, que M-15 y Progresión describen como la amenaza típica de la sala clave (likely). Según la regla 5, esa muerte cuenta desde 0. Si se implementa el reinicio desde la tabla de transiciones, cuenta como una muerte más de la racha y puede disparar el final de disolución (M-18, Deseable). El texto confirma la discrepancia; su efecto depende de que M-18 se implemente. Basta con que la fila remita a la regla 5.

## Alcance del corte de vigilancia y la ficha de D-04

En la Fase A se aplicó la observación 1 de `gdd-pendientes-por-definir.md`. M-02 dice ahora que la vista se corta cuando un enemigo entra en alerta "con el mismo alcance que M-03: dentro de la sala o en el área cercana con ruta de persecución", y la transición correspondiente de Reglas y Estados usa el mismo alcance. La ficha de D-04 en el Registro sigue diciendo "se corta al recibir daño o con cualquier enemigo en alerta", sin anotación (confirmed). Literalmente, un enemigo en alerta en otra sala sin ruta de persecución cortaría la vista según D-04, pero no según M-02. No se clasifica como contradicción bloqueante, porque M-03, que también deriva de D-01 y D-04, siempre limitó el bloqueo a ese alcance, y porque la precisión es la opción que proponía el propio documento de pendientes. Aun así, el Registro debería reflejarla para que la ficha no contradiga la mecánica, o el equipo debería confirmarla, ya que fija la lectura de una decisión.

</details>

<details>
<summary>File map</summary>

- `02 - Diseño/02 - Mecánicas.md`: M-16 renombrada, M-20 nueva (con el caso límite "Por definir" de la frontera del contexto), catálogo, matriz, interacciones de M-06, M-07, M-09, M-10, M-14 y M-19, alcance de corte en M-02 y enlaces a Sistemas y Economía.
- `02 - Diseño/04 - Controles.md`: filas de remate, contexto "Sobre un cuerpo (sin apuntar)" con su frontera **Por definir** y retroalimentación del remate.
- `02 - Diseño/03 - Sistemas.md`: inventario S-01..S-11, plantilla por sistema, tabla y diagrama de relaciones.
- `02 - Diseño/05 - Progresión.md`: etapas, orden de aprendizaje, personaje sin mejoras, desbloqueos y curvas; cantidad de enemigos **Por definir**.
- `02 - Diseño/06 - Economía y Recursos.md`: tabla de recursos, fuentes y sumideros, presupuesto por piso (regla de 6 casillas limitada al piso 1), pérdida al morir y balance.
- `02 - Diseño/07 - Reglas y Estados del Juego.md`: estados, subestados, transiciones, diagrama, reglas globales (regla 4 alineada con D-09) y casos especiales.
- `02 - Diseño/01 - Jugabilidad.md`: enlace a Reglas y Estados en la condición de derrota.
- `01 - Visión/03 - Público y Plataforma.md`: "Guardado compatible" marcado; "Interfaz legible" sigue desmarcada.
- `01 - Visión/01 - Ficha del Proyecto.md`: versión 0.3.0 y fecha 2026-10-06.
- `10 - Referencia/03 - Registro de Decisiones.md`: D-12 marcada como modificada por D-18, filas D-17..D-22, fichas D-18, D-20 y D-21.
- `10 - Referencia/04 - Historial de Versiones.md`: fila 0.3.0.

Diff completo: `git diff HEAD -- .` en la raíz del GDD (el `HEAD` es anterior a la v0.2.0, así que el diff incluye también los cambios ya aprobados de esa versión).

</details>
