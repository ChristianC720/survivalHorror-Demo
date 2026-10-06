# Corrección de mecánicas del GDD 0.2.0 (D-01..D-16), iteración 2

Esta iteración corrige los tres hallazgos bloqueantes de la revisión anterior en `02 - Mecánicas.md`, `03 - Registro de Decisiones.md` y `04 - Pilares de Diseño.md`. El diagrama de M-01 ya permite caminar lento durante la vigilancia. M-10 define "en alerta" como término común con umbral **Por definir**, sin tomar una decisión nueva. Los valores nuevos de M-11, M-17 y D-14 llevan la marca de balance. Se revisó el texto final de los 10 archivos contra D-01..D-16 y el estándar de formato, y no queda ninguna contradicción con una decisión confirmada.

Watch for: dos observaciones no bloqueantes. El alcance de "cualquier enemigo en alerta" no es igual en M-02 y en M-03 (confirmed). Dos mecánicas Demo dependen de mecánicas Deseable (M-19 de M-17, M-15 de M-18) sin indicar qué pasa si la Deseable no se implementa (confirmed).

**Verdict**: APPROVED

## High-level view

Las correcciones bloqueantes de la iteración 1 están aplicadas. La transición del diagrama de estados es `Vigilancia --> OTS: salir / correr, apuntar o empujar / recibir daño`. M-02, M-03 y el diagrama remiten al umbral de M-10. El aturdimiento de 2 s y "Demo: 1-2 fallas" tienen la marca "(valor inicial, sujeto a balance)". La viñeta vacía de Pilares se eliminó. Se acepta no añadir fichas para D-03, D-05 y D-08..D-12: la tarea las limitaba a nueve decisiones y completarlas exigiría inventar "Opciones consideradas" que nunca se registraron.

Lo que queda abierto está en la frontera entre mecánicas, no en una decisión concreta. El corte de la vigilancia (M-02) y su bloqueo (M-03) usan alcances espaciales distintos para "en alerta". La regla de priorización Demo/Deseable también se rompe en dos puntos: M-19 y M-15 cuentan con M-17 y M-18 sin decir cómo se comportan si esas mecánicas se recortan.

<details>
<summary>Issues (2)</summary>

1. **Alcance de "cualquier enemigo en alerta"** (no bloqueante, confirmed): M-02 y D-04 cortan la vigilancia con cualquier enemigo en alerta, pero M-03 solo la bloquea si el enemigo está en la sala o en un área cercana con ruta de persecución. Aclarar en M-02 que el corte usa el mismo alcance que M-03, o marcar el alcance como Por definir.
2. **Dependencias Demo → Deseable** (no bloqueante, confirmed): M-19 (Demo) usa M-17 (Deseable) para acortar la fase 1, y M-15 (Demo) reinicia el contador de M-18 (Deseable). Añadir en M-19 y M-15 una línea que indique que la mecánica funciona sin la Deseable.

</details>

<details>
<summary>Details</summary>

### Alcance espacial del corte de vigilancia

M-02 (Reglas) dice que la vista "se corta al recibir daño o cuando cualquier enemigo entra en alerta". Su Activación exige "sin enemigos en alerta", y D-04 repite "con cualquier enemigo en alerta". En cambio, M-03 inhabilita la entrada solo "mientras exista al menos un enemigo en alerta [...] dentro de la sala o área cercana". Su caso límite añade que un enemigo de otra sala solo cuenta si tiene ruta para perseguir. Con la lectura literal de M-02, un enemigo en alerta en el otro extremo del piso corta la vista, pero M-03 permite reactivarla de inmediato. Así, el corte y el bloqueo usan reglas distintas. No contradice D-04, que no define un alcance, pero quien implemente tendrá que elegir uno (confirmed). Basta con remitir M-02 al alcance de M-03 o declararlo Por definir.

### Mecánicas Demo que dependen de mecánicas Deseable

El catálogo marca M-17 Fallas de la máquina y M-18 Degradación y final de 5 muertes como **Deseable**. Sin embargo, M-19 (Demo) dice que "las fallas descubiertas (M-17) acortan las secuencias" de la fase 1, y M-15 (Demo) dice que "llegar a una sala clave reinicia el contador de 5 muertes (M-18)". Si el alcance obliga a recortar las Deseables, la fase 1 del jefe pierde la recompensa por aprender y la regla de M-15 apunta a un sistema que no existe (confirmed). El fallo es leve porque ambas Demo siguen siendo jugables sin las Deseables, pero el documento no lo dice. M-12 y M-14 sí usan la convención de marcar la parte Deseable dentro de una mecánica Demo, y conviene aplicarla aquí también.

</details>

<details>
<summary>File map</summary>

- `01 - Visión/01 - Ficha del Proyecto.md`: versión 0.2.0, justificación de cámara D-01/D-04, etiquetas "Salas clave fijas" y "Exploración vertical", premisa.
- `01 - Visión/02 - Visión del Juego.md`: fantasía del jugador con la premisa y límite de cámara alineado.
- `01 - Visión/03 - Público y Plataforma.md`: casillas desmarcadas y acción defensiva sin esquiva.
- `01 - Visión/04 - Pilares de Diseño.md`: Pilar 3 renombrado, principio de cámara alineado y viñeta vacía eliminada (iteración 2).
- `02 - Diseño/01 - Jugabilidad.md`: condición de victoria (The Mother), final de 5 muertes y principio 5.
- `02 - Diseño/02 - Mecánicas.md`: M-01..M-19; en la iteración 2, diagrama de M-01, umbral "en alerta" en M-10 y marcas de balance en M-11 y M-17.
- `02 - Diseño/04 - Controles.md`: tabla Acción|Teclado|Mouse|Control, contextos de entrada, reasignación y retroalimentación.
- `10 - Referencia/03 - Registro de Decisiones.md`: fichas y tabla D-01..D-16; marca de balance en D-14 (iteración 2).
- `10 - Referencia/04 - Historial de Versiones.md`: fila 0.2.0 (2026-10-05).
- `Decisiones a Implementar.md`: marcas de confirmación D-13 y D-16.

La carpeta del GDD no está bajo un control de versiones utilizable (el repositorio padre muestra otro proyecto) y no hay diff. La evidencia de cambios es la nota `.agents/tasks/gdd-verificacion.md` y la lectura directa de los archivos.

</details>
