# Registro de Decisiones

## Descripción de la sección

Esta sección conserva las decisiones importantes del proyecto y el razonamiento detrás de ellas.

## Decisión D-01 — Cámara Over the Shoulder por defecto

**Fecha:** 2026-10-05

**Tema:** Cámara.

**Problema:** La [[01 - Ficha del Proyecto]] define Over the Shoulder (OTS) como perspectiva principal y la isométrica como vista secundaria manual, mientras que [[02 - Mecánicas]] trataba la isométrica como vista normal de exploración y forzaba OTS al ser detectado.

**Opciones consideradas:**

1. Isométrica por defecto durante la exploración y OTS forzada al ser detectado.
2. OTS por defecto y vista isométrica como reconocimiento manual con costo.
3. Únicamente OTS, eliminando la vista isométrica.

**Decisión tomada:** Opción 2. La cámara OTS es la vista por defecto. La vista isométrica es una acción manual de reconocimiento con costo (ver D-04).

**Motivo:** Refuerza el pilar **Vulnerabilidad Constante**: la vista cercana elimina la visión periférica y trasera durante la mayor parte del juego, y la información amplia solo se obtiene pagando un costo.

**Consecuencias:** Se reescriben M-01, M-02 y M-03 en [[02 - Mecánicas]]. Se alinean la Ficha, la [[02 - Visión del Juego]], los [[04 - Pilares de Diseño]] y el principio 5 de [[01 - Jugabilidad]].

**Responsable:** Christian Coronel Palma

## Decisión D-02 — Sin conos de visión enemigos visibles

**Fecha:** 2026-10-05

**Tema:** Sigilo y percepción enemiga.

**Problema:** La mecánica de sigilo proyectaba sobre el suelo los conos de visión de los enemigos, lo que contradice que la vista isométrica no proporcione información omnisciente.

**Opciones consideradas:**

1. Conos de visión siempre visibles.
2. Conos de visión visibles solo en la vista isométrica.
3. Sin conos visibles; la percepción enemiga se comunica únicamente mediante señales diegéticas.

**Decisión tomada:** Opción 3. Se crea la mecánica M-10 Señales diegéticas para cumplir el principio 3 de [[01 - Jugabilidad]] (los riesgos deben poder interpretarse).

**Motivo:** Refuerza el pilar **Adaptación**: el jugador decide con información incompleta e interpreta sonido, luz y comportamiento en lugar de leer una regla geométrica.

**Consecuencias:** Se elimina toda referencia a conos de visión enemigos en M-09. El diseño de audio y animación de enemigos adquiere mayor responsabilidad.

**Responsable:** Christian Coronel Palma

## Decisión D-04 — Vista isométrica como cámaras de vigilancia de The Mother

**Fecha:** 2026-10-05

**Tema:** Cámara y narrativa.

**Problema:** La vista isométrica necesitaba un costo y una justificación para no convertirse en una vista panorámica gratuita que elimine la tensión.

**Opciones consideradas:**

1. Vista isométrica libre en cualquier sala y sin restricciones.
2. Vista isométrica como acceso a las cámaras de seguridad de The Mother desde el terminal del brazo, con restricciones.
3. Eliminar la vista isométrica.

**Decisión tomada:** Opción 2. La vista solo está disponible en salas con cámara activa; la imagen tiene ruido CRT y un retraso de ~0.3 s; requiere estar quieto o caminar muy lento; muestra solo lo que cubre la cámara y un piso a la vez; se corta al recibir daño o con cualquier enemigo en alerta (precisado en v0.3.0: mismo alcance que M-03, es decir, enemigos en la sala o en el área cercana con ruta de persecución). El generador garantiza un mínimo de salas con cámara por intento y algunas salas no tienen cámara. Narrativamente, el jugador ve por los ojos de la máquina.

**Motivo:** Refuerza **Vulnerabilidad Constante** (observar deja al personaje expuesto), **Adaptación** (la cobertura de cámaras cambia por intento) y **Decisiones con Consecuencias** (observar tiene un costo de tiempo y exposición).

**Consecuencias:** Se crea M-02 Vista de vigilancia. La generación procedural debe distribuir salas con cámara. Se alinean la justificación de cámara de la Ficha y el principio complementario "La cámara debe tener intención".

**Responsable:** Christian Coronel Palma

## Decisión D-06 — Persistencia de conocimiento en el terminal del brazo

**Fecha:** 2026-10-05

**Tema:** Ciclo de muerte y persistencia.

**Problema:** La mecánica de persistencia mezclaba los terminales de sala con la base de datos persistente y afirmaba que los códigos almacenados pierden su valor, lo que contradecía que la persistencia fuera útil.

**Opciones consideradas:**

1. Persistir los códigos concretos encontrados.
2. Persistir conocimiento (bestiario, lore, documentos y reglas del sistema) y fallas de la máquina en el terminal del brazo.
3. No persistir información entre muertes.

**Decisión tomada:** Opción 2. Persisten entre muertes el bestiario, el lore, los documentos y las reglas del sistema (por ejemplo, cómo se forma un código, no su valor), además de las fallas de la máquina. Esto no hace más fuerte al personaje, solo más informado.

**Motivo:** Refuerza el pilar **Adaptación**: el conocimiento adquirido ayuda a comprender las reglas, pero no permite memorizar el escenario.

**Consecuencias:** Se crea M-16 Ciclo de muerte. Los terminales de sala solo sirven para hackear (M-13). Se crea M-17 Fallas de la máquina.

**Responsable:** Christian Coronel Palma

## Decisión D-07 — Salud fija con tres estados

**Fecha:** 2026-10-05

**Tema:** Salud y combate.

**Problema:** El GDD no definía salud, daño ni armas, aunque varias mecánicas dependían de ellos.

**Opciones consideradas:**

1. Barra de salud con regeneración automática.
2. Salud fija sin regeneración con tres estados visibles.
3. Muerte de un solo golpe.

**Decisión tomada:** Opción 2. Salud fija sin regeneración con tres estados visibles: Bien, Herido y Crítico (Herido y Crítico reducen la velocidad). Los enemigos matan en 3-4 golpes. La demo incluye 1 arma de fuego con munición escasa y el empujón. Las curas son escasas y ocupan casillas. Los enemigos son lentos pero letales; enfrentar a más de dos a la vez es casi muerte segura.

**Motivo:** Refuerza **Vulnerabilidad Constante** (cada golpe tiene consecuencias duraderas) y **Decisiones con Consecuencias** (curarse compite por espacio en el inventario).

**Consecuencias:** Se crean M-04 Combate a distancia y M-05 Salud y estados. Se descarta la regeneración de salud.

**Responsable:** Christian Coronel Palma

## Decisión D-13 — Premisa oficial

**Fecha:** 2026-10-05

**Tema:** Narrativa.

**Problema:** El documento inicial de ideas planteaba una guerra entre humanos y clones que no estaba integrada al GDD y no ofrecía una motivación personal al protagonista.

**Opciones consideradas:**

1. Conservar el lore de guerra entre humanos y clones.
2. Adoptar la premisa del hombre que busca la última pista de su esposa fallecida.
3. Mantener la premisa por definir.

**Decisión tomada:** Opción 2. Premisa oficial: "Un hombre busca la última pista que su esposa fallecida le dejó, pero queda atrapado en un ciclo sin fin del que ahora debe salir." El lore de guerra entre humanos y clones queda solo como inspiración visual y mecánica. Los documentos de la esposa persisten en el terminal del brazo.

**Motivo:** Refuerza el pilar **Adaptación** y el principio complementario "La muerte forma parte de la experiencia": el ciclo sin fin justifica narrativamente que el escenario cambie después de cada muerte.

**Consecuencias:** Se actualizan la [[01 - Ficha del Proyecto]], la [[02 - Visión del Juego]] y `Decisiones a Implementar.md`. La relación entre la esposa, la instalación y The Mother queda por definir.

**Estado:** Modificada por D-40 (la esposa está desaparecida, no fallecida). Se conserva como histórico.

**Responsable:** Christian Coronel Palma

## Decisión D-14 — Objeto defensivo consumible en lugar de la liberación de agarre

**Fecha:** 2026-10-05

**Tema:** Combate y agarre.

**Problema:** La liberación de agarre mediante una secuencia rápida con 3 usos por vida resolvía el agarre con habilidad manual sin costo de inventario y añadía un contador ajeno a los recursos del juego.

**Opciones consideradas:**

1. Conservar la secuencia rápida con usos limitados por vida.
2. Reemplazarla por un objeto defensivo consumible que ocupa inventario.
3. Agarre sin defensa posible.

**Decisión tomada:** Opción 2. El objeto defensivo (por ejemplo, descarga eléctrica o navaja) ocupa 1 casilla del inventario, se activa automáticamente al ser agarrado, se consume y aturde al enemigo 2 s (valor inicial, sujeto a balance). Sin el objeto, el agarre baja un estado de salud (o mata si el personaje ya estaba en Crítico).

**Motivo:** Refuerza **Decisiones con Consecuencias** (llevar el objeto compite con curas, munición y objetos clave) y **Vulnerabilidad Constante** (sin el objeto, el agarre siempre cuesta salud).

**Consecuencias:** Se crea M-11 y se elimina la mecánica anterior de liberación de agarre, su secuencia y su contador por vida.

**Responsable:** Christian Coronel Palma

## Decisión D-15 — Exploración vertical por pisos

**Fecha:** 2026-10-05

**Tema:** Estructura del escenario.

**Problema:** Se quería implementar exploración vertical sin romper la generación procedural ni el sentido de la estructura de salas clave.

**Opciones consideradas:**

1. Escaleras y accesos verticales libres en salas procedurales.
2. Pisos conectados por escaleras o elevadores ubicados únicamente dentro de las salas clave.
3. Pasarelas internas y techos rotos dentro de las salas.

**Decisión tomada:** Opción 2. Las escaleras y elevadores existen solo dentro de las salas clave. La posición de la sala clave dentro de cada piso varía por intento. El elevador puede requerir energía o hackeo (secuencia larga). Cada piso garantiza al menos una ruta a su sala clave. Demo: 2 pisos + sala del jefe (valor inicial, sujeto a balance). Llegar a una sala clave reinicia el contador de 5 muertes. Las pasarelas internas y los techos rotos quedan como Post-demo.

**Motivo:** Refuerza el pilar **Adaptación**: la ruta hacia cada sala clave cambia por intento, pero la estructura general sigue siendo comprensible.

**Consecuencias:** Se crea M-15. El generador debe garantizar una ruta válida por piso. La vista de vigilancia muestra un piso a la vez.

**Estado:** Modificada por D-33 (3 pisos, salas clave secundarias y finales). El reinicio del contador en cualquier sala clave queda sustituido por D-35.

**Responsable:** Christian Coronel Palma

## Decisión D-16 — Jefe final: The Mother

**Fecha:** 2026-10-05

**Tema:** Condición de victoria y jefe final.

**Problema:** La condición de victoria estaba por definir y el GDD marcaba la etiqueta "Jefes" sin respaldo.

**Opciones consideradas:**

1. Jefe de combate directo con sistemas nuevos.
2. Encuentro híbrido de 3 fases que reutiliza mecánicas existentes.
3. Sin jefe; alcanzar una sala final.

**Decisión tomada:** Opción 2. The Mother es el jefe final, en 3 fases que reutilizan mecánicas existentes: 1) hackeo de nodos bajo presión (las fallas descubiertas acortan secuencias), 2) supervivencia y huida (la vista de vigilancia queda hostil o inutilizable), 3) pocos disparos a puntos débiles expuestos con munición limitada en la sala. Cada fase dura 1-2 min (valor inicial, sujeto a balance). Derrotarla es la condición de victoria; al ser derrotada revela que existe algo más (la criatura lovecraftiana que es el laboratorio), pero no puede explicarlo. Su sala es una sala clave fija y morir en ella regresa al inicio del ciclo.

**Motivo:** Refuerza **Vulnerabilidad Constante**, **Adaptación** y **Decisiones con Consecuencias** al exigir en un solo encuentro las herramientas aprendidas durante el ciclo, sin aumentar el alcance con sistemas nuevos.

**Consecuencias:** Se crea M-19 y se define la condición de victoria en [[01 - Jugabilidad]]. Riesgo documentado: la fase 3 puede volver el juego de acción, por lo que debe mantenerse corta.

**Responsable:** Christian Coronel Palma

## Decisión D-18 — Reconstrucción del personaje al morir

**Fecha:** 2026-10-06

**Tema:** Muerte y reaparición.

**Problema:** D-12 describía la reaparición como la clonación del personaje (permadeath individual del clon), lo que contradecía la premisa oficial (D-13): el protagonista es un hombre concreto que busca la última pista de su esposa, no una copia nueva en cada intento.

**Opciones consideradas:**

1. Mantener la clonación: cada intento es un clon nuevo.
2. Reconstrucción: siempre es el mismo hombre, recompuesto con piezas ajenas.
3. Reaparición sin justificación narrativa.

**Decisión tomada:** Opción 2. Al morir, el personaje se reconstruye en el punto de reaparición; siempre es el mismo hombre, recompuesto con piezas ajenas. Se pierde todo el inventario y el conocimiento del terminal del brazo persiste (D-06). Modifica D-12, que se conserva como histórico.

**Motivo:** Refuerza el pilar **Adaptación**: la muerte forma parte del ciclo y lo que persiste es el conocimiento, no la fuerza del personaje. Mantiene la coherencia con la premisa oficial.

**Consecuencias:** M-16 pasa a llamarse "Reaparición por reconstrucción y terminal del brazo" en [[02 - Mecánicas]]. La etiqueta "Permadeath" se sustituye por "Ciclo de muerte y reconstrucción" (D-19). El origen de las piezas ajenas y su relación con la instalación quedan **Por definir**.

**Responsable:** Christian Coronel Palma

## Decisión D-20 — Reciclaje único de enemigos eliminados

**Fecha:** 2026-10-06

**Tema:** Enemigos.

**Problema:** Si los enemigos eliminados desaparecen para siempre, vaciar el piso se vuelve la estrategia dominante; si reaparecen sin límite, el jugador entra en un desgaste sin fin.

**Opciones consideradas:**

1. Los enemigos eliminados siguen muertos durante todo el intento.
2. Los enemigos eliminados reaparecen sin límite.
3. Los enemigos eliminados siguen muertos, pero The Mother puede reciclar cada uno una sola vez.

**Decisión tomada:** Opción 3. Antes de desaparecer, el cuerpo vibra y emite un sonido de maquinaria. El enemigo reciclado reaparece cuando el jugador sale de la sala, en otra sala del mismo piso y nunca frente al jugador. Es más rápido, pero muere con menos golpes, y no vuelve a reciclarse.

**Motivo:** Refuerza **Decisiones con Consecuencias** (eliminar a un enemigo no es una solución definitiva y gratuita) y **Vulnerabilidad Constante** (un piso despejado puede volver a poblarse). El límite de un reciclaje evita el desgaste sin fin.

**Consecuencias:** Se crea M-20 Reciclaje de enemigos y remate en [[02 - Mecánicas]]. M-14 aclara que la sala de la que se huyó sigue infestada, incluidos los reciclados. M-10 incorpora el aviso de maquinaria como señal diegética. La proporción de cuerpos reciclados y los valores del enemigo reciclado quedan **Por definir**.

**Responsable:** Christian Coronel Palma

## Decisión D-21 — Remate para evitar el reciclaje

**Fecha:** 2026-10-06

**Tema:** Combate y enemigos.

**Problema:** El reciclaje (D-20) necesitaba una respuesta del jugador que tuviera costo, para que impedirlo fuera una decisión y no un trámite.

**Opciones consideradas:**

1. Sin forma de evitar el reciclaje.
2. Solo un golpe en el suelo con costo de resistencia, tiempo y ruido.
3. Dos formas: golpe en el suelo o consumible de remate.

**Decisión tomada:** Opción 3. **Golpe en el suelo:** cuesta resistencia, dura ~2 s (valor inicial, sujeto a balance), alerta a enemigos cercanos y se ejecuta con `Clic izquierdo` sin apuntar sobre un cuerpo. **Consumible de remate** (por ejemplo, combustible para incinerar): ocupa 1 casilla del inventario, es rápido, no alerta a enemigos y se usa con `E` sobre un cuerpo.

**Motivo:** Refuerza **Decisiones con Consecuencias**: el jugador elige entre gastar tiempo, resistencia y ruido o una casilla del inventario, o asumir que el enemigo volverá.

**Consecuencias:** M-20 documenta ambas formas y [[04 - Controles]] agrega el remate como acción contextual sobre un cuerpo. El golpe en el suelo no otorga protección contra el daño (D-05) y el consumible de remate no sustituye al objeto defensivo (D-14). Riesgo documentado: saturar el inventario, mitigado con un presupuesto bajo del consumible (D-17).

**Responsable:** Christian Coronel Palma

## Decisión D-23 — Señal de vigilancia escalonada según el estado de alerta

**Fecha:** 2026-10-07

**Tema:** Cámara y percepción enemiga.

**Problema:** M-10 dejaba **Por definir** el umbral de "en alerta" que usan M-02 y M-03 para cortar y bloquear la vista de vigilancia (búsqueda o caza). Cortar desde búsqueda volvía la vigilancia casi inútil; cortar solo en caza permitía observar sin costo mientras los enemigos ya buscaban al jugador.

**Opciones consideradas:**

1. "En alerta" = búsqueda: la señal se corta y se bloquea desde el estado de búsqueda.
2. "En alerta" = caza: la señal se corta y se bloquea solo en caza, sin cambios en los estados anteriores.
3. Señal escalonada: sin cambios en sospecha, degradada en búsqueda y cortada y bloqueada en caza.

**Decisión tomada:** Opción 3. En **sospecha** la señal no cambia; en **búsqueda** se degrada (más ruido CRT y más retraso); en **caza** se corta y se bloquea (M-03). Para el corte y el bloqueo, "en alerta" equivale a caza. El alcance es el mismo que el de M-03: enemigos dentro de la sala o en el área cercana con ruta de persecución.

**Motivo:** Refuerza **Vulnerabilidad Constante** (la información se deteriora antes de perderse y observar nunca es seguro) y **Adaptación** (el jugador interpreta la calidad de la señal como indicio del estado de los enemigos, sin indicadores en pantalla, D-02).

**Consecuencias:** Se cierra el **Por definir** de M-10. M-01 (diagrama), M-02 y M-03 usan "caza" como umbral de corte y bloqueo en [[02 - Mecánicas]]. La degradación de la señal se implementa en [[03 - Sistemas]]. La magnitud de la degradación en búsqueda queda sujeta a balance.

**Responsable:** Christian Coronel Palma

## Decisión D-24 — Atención de The Mother

**Fecha:** 2026-10-07

**Tema:** Cámara y percepción enemiga.

**Problema:** La vista de vigilancia solo costaba tiempo y exposición. El equipo quiere que, cuando el jugador usa las cámaras, los monstruos lo sepan y empiecen a buscarlo.

**Opciones consideradas:**

1. Sin consecuencia adicional: la vigilancia solo cuesta tiempo y exposición.
2. Medidor de atención visible en pantalla que avisa cuándo The Mother detecta el uso de las cámaras.
3. Medidor de atención oculto que, al llenarse, envía a los enemigos cercanos hacia el jugador; el jugador lo percibe por la degradación de la señal.

**Decisión tomada:** Opción 3. Un medidor **oculto** (sin barra en pantalla) se llena mientras se usa la vigilancia (~5 s) y baja sin usarla (~15 s) (valores iniciales, sujetos a balance). Al llenarse, los enemigos cercanos pasan a sospecha y luego a búsqueda hacia la posición del jugador. El jugador lo percibe por la degradación de la señal (D-23).

**Motivo:** Refuerza **Decisiones con Consecuencias** (observar más tiempo se paga con enemigos que acuden a buscar al jugador) y **Vulnerabilidad Constante** (la vigilancia nunca es un refugio).

**Consecuencias:** M-02 y M-10 incorporan el medidor en [[02 - Mecánicas]]; [[03 - Sistemas]] implementa el medidor y el cambio de estado de los enemigos. No se añade interfaz nueva, en coherencia con D-02.

**Responsable:** Christian Coronel Palma

## Decisión D-30 — Autoguardado en la sala clave final

**Fecha:** 2026-10-07

**Tema:** Guardado.

**Problema:** D-22 definía una suspensión al salir y la reanudación en el mismo punto. Con 3 pisos (D-33), el equipo quiere que la partida se guarde automáticamente al llegar a una sala clave, con un aviso discreto, sin guardado manual y con solo una opción de "Reiniciar". El autoguardado solo debe proteger el progreso si el jugador sale de la partida o el juego se cierra por un fallo.

**Opciones consideradas:**

1. Mantener la suspensión al salir de D-22.
2. Guardado manual en puntos fijos.
3. Autoguardado al llegar a la sala clave final de los pisos 1 y 2, sin guardado manual, útil solo para continuar después de salir del juego o de un fallo.

**Decisión tomada:** Opción 3. Sin suspensión. Autoguardado al llegar a la **sala clave final** de los pisos 1 y 2 (no en salas clave secundarias ni en la sala del jefe), solo si no hay enemigos en caza; si el jugador llega perseguido, el guardado espera a que la caza termine. Guarda el estado al llegar, incluido el inventario. Aviso discreto y diegético en el terminal del brazo ("sincronizando…") que no interrumpe el juego. Solo sirve para continuar después de salir del juego o de un fallo; no existe guardado manual. El autoguardado se conserva hasta morir, reiniciar o llegar a otra sala final. Al cargar, gracia de 2–3 s (valor inicial, sujeto a balance) en la que los enemigos no atacan. El terminal del brazo y las fallas se siguen autoguardando al cambiar.

**Motivo:** Refuerza **Decisiones con Consecuencias** (salir del juego no borra las decisiones tomadas, ver D-31) y **Vulnerabilidad Constante** (no se puede guardar ni salir para escapar de una caza).

**Consecuencias:** Modifica D-22, que se conserva como histórico. Se elimina la suspensión en [[07 - Reglas y Estados del Juego]] y en [[03 - Sistemas]]. M-15 indica el autoguardado al llegar a la sala final y M-16 aclara que la muerte siempre regresa al inicio del ciclo. Se complementa con D-31 (salir del juego) y D-32 (Reiniciar y Nueva partida).

**Responsable:** Christian Coronel Palma

## Decisión D-31 — Costo de salir del juego

**Fecha:** 2026-10-07

**Tema:** Guardado y ciclo de muerte.

**Problema:** Sin suspensión (D-30), salir del juego podía usarse para escapar de una amenaza o para evitar el costo de una muerte.

**Opciones consideradas:**

1. Salir no tiene costo: se continúa siempre desde el último autoguardado.
2. Salir siempre cuenta como muerte completa.
3. Costo según la situación en el momento de salir.

**Decisión tomada:** Opción 3.

- **Durante alerta o caza:** cuenta como muerte completa (regresa al inicio del ciclo y suma al contador y a la degradación).
- **Con calma, fuera de una sala clave final ya guardada:** suma 1 al contador de muertes y a la degradación, pero al volver se continúa desde la sala final autoguardada.
- **En una sala final ya guardada y sin alerta:** sin costo.
- Un cierre inesperado o un fallo del juego no cuenta.
- El diálogo de confirmación al salir avisa del costo.
- En el primer intento se aplica la misma excepción de D-35: no suma al contador.

**Motivo:** Refuerza **Vulnerabilidad Constante** (salir no es una vía de escape frente a una amenaza) y **Decisiones con Consecuencias** (abandonar la partida tiene un costo proporcional a la situación).

**Consecuencias:** [[07 - Reglas y Estados del Juego]] documenta el diálogo de salida y los casos especiales. M-18 suma al contador y a la degradación las salidas con costo.

**Responsable:** Christian Coronel Palma

## Decisión D-33 — Estructura de 3 pisos y tipos de sala clave

**Fecha:** 2026-10-07

**Tema:** Estructura del escenario.

**Problema:** D-15 fijaba 2 pisos + sala del jefe con un solo tipo de sala clave. El equipo quiere entre 8 y 12 salas generadas por piso, 2 a 3 salas clave con documentos, llaves y otros objetos (sin guardado) y 1 sala clave al final de cada piso (con guardado), en 3 pisos y con la sala del jefe en el último piso (sin guardado).

**Opciones consideradas:**

1. Mantener 2 pisos + sala del jefe (D-15).
2. 3 pisos con un solo tipo de sala clave por piso.
3. 3 pisos con salas clave secundarias y una sala clave final por piso.

**Decisión tomada:** Opción 3. 3 pisos. Cada piso tiene 8–12 salas procedurales, 2–3 **salas clave secundarias** (documentos, llaves y otros objetos; sin guardado) y 1 **sala clave final** (escalera o elevador; autoguardado en los pisos 1 y 2, D-30). La sala de The Mother está en el piso 3 y no guarda. En total, ~10–15 salas por piso (valores iniciales, sujetos a balance).

**Motivo:** Refuerza **Adaptación**: más salas procedurales y salas clave cuya posición cambia por intento, dentro de una estructura por pisos que sigue siendo comprensible.

**Consecuencias:** Modifica D-15, que se conserva como histórico. Las escaleras y los elevadores existen solo en las salas clave finales. Se actualizan M-15 en [[02 - Mecánicas]], el generador en [[03 - Sistemas]], [[05 - Progresión]] y [[06 - Economía y Recursos]]. Se complementa con D-34 (configuraciones), D-35 (contador) y D-36 (llaves).

**Responsable:** Christian Coronel Palma

## Decisión D-35 — Reinicio del contador de 5 muertes

**Fecha:** 2026-10-07

**Tema:** Final secundario.

**Problema:** D-08 reiniciaba el contador al llegar a cualquier sala clave. Con salas clave secundarias y finales en 3 pisos (D-33), el contador casi nunca avanzaría. Además, las muertes del tutorial castigarían a un jugador que aún está aprendiendo.

**Opciones consideradas:**

1. Reiniciar el contador en cualquier sala clave (D-08).
2. Reiniciar el contador en cada sala clave final (pisos 1 y 2).
3. Reiniciar el contador solo en la sala clave final del piso 2.

**Decisión tomada:** Opción 3. Solo llegar a la **sala final del piso 2** reinicia el contador. Las muertes del primer intento (el del tutorial) no cuentan. Reiniciar (D-32) y salir con costo (D-31) suman al contador.

**Motivo:** Refuerza **Vulnerabilidad Constante** (la amenaza de la "Disolución de identidad" se mantiene durante más tiempo) y **Adaptación** (el primer intento sirve para aprender las reglas sin castigo en el contador).

**Consecuencias:** Modifica D-08, que se conserva como histórico. Se actualizan M-15 y M-18 en [[02 - Mecánicas]], [[05 - Progresión]] y [[07 - Reglas y Estados del Juego]].

**Responsable:** Christian Coronel Palma

## Decisión D-39 — Presentación: cinemáticas, transición de piso y menú principal

**Fecha:** 2026-10-07

**Tema:** Presentación.

**Problema:** [[07 - Reglas y Estados del Juego]] dejaba **Por definir** la presentación de la reconstrucción, la pantalla de carga entre pisos, las opciones del menú principal y si los créditos se pueden saltar.

**Opciones consideradas:**

1. Reaparición directa sin cinemática y pantalla de carga entre pisos.
2. Cinemática completa de reconstrucción en cada muerte y pantalla de carga entre pisos.
3. Cinemática completa una sola vez, cinemática corta en cada muerte o reinicio, y transición de piso con cinemática que oculta la carga.

**Decisión tomada:** Opción 3.

- **Cinemática completa** de reconstrucción al terminar la introducción, una sola vez. Su contenido queda **Por definir** según el lore.
- **Cinemática corta** en cada muerte o reinicio: el personaje cae del tanque, con sonido de maquinaria; refleja la degradación (M-18) y se puede saltar.
- **Transición de piso:** cinemática de ascensor o escalera de **mínimo 5 s** que oculta la carga y muestra el aviso de autoguardado; se alarga si la carga tarda. No hay pantalla de carga.
- **Menú principal:** Continuar, Nueva partida, Opciones, Créditos, Salir.
- Los créditos se pueden saltar después de verlos una vez.

**Motivo:** Refuerza **Adaptación**: la muerte se presenta como parte del ciclo, la cinemática corta muestra la degradación sin frenar el reintento y la transición de piso mantiene la continuidad del recorrido.

**Consecuencias:** Se cierra el **Por definir** de presentación en [[07 - Reglas y Estados del Juego]]. M-15 y M-16 en [[02 - Mecánicas]] incorporan las cinemáticas y [[04 - Controles]] agrega la tecla para saltarlas. El contenido de las cinemáticas queda **Por definir** en `03 - Historia y Mundo`.

**Responsable:** Christian Coronel Palma

## Decisión D-40 — Esposa desaparecida, no fallecida

**Fecha:** 2026-10-07

**Tema:** Narrativa.

**Problema:** La premisa oficial (D-13) describía a la esposa del protagonista como fallecida, mientras que el equipo confirma que en realidad está desaparecida. Además, la ficha de [[03 - Personajes]] ya la describe como desaparecida, por lo que el GDD era incoherente.

**Opciones consideradas:**

1. Mantener a la esposa como fallecida (D-13).
2. Esposa desaparecida, con su destino y su relación con la instalación y The Mother **Por definir**.
3. Esposa desaparecida, decidiendo ya qué le ocurrió y su relación con la instalación y The Mother.

**Decisión tomada:** Opción 2. Nueva premisa oficial: "Un hombre busca la última pista que su esposa desaparecida le dejó, pero queda atrapado en un ciclo sin fin del que ahora debe salir." Qué le ocurrió a la esposa y su relación con la instalación y The Mother siguen **Por definir**. Los documentos de la esposa siguen persistiendo en el terminal del brazo (D-13).

**Motivo:** Refuerza el pilar **Adaptación** y el horror cósmico por descubrimiento progresivo e información incompleta: no saber si la esposa vive mantiene abierta la búsqueda y obliga al jugador a reconstruir lo ocurrido a partir de los documentos que persisten entre muertes.

**Consecuencias:** Modifica D-13, que se conserva como histórico. Se actualizan la [[01 - Ficha del Proyecto]], la [[02 - Visión del Juego]], `Decisiones a Implementar.md` y [[03 - Personajes]]. No se añade lore nuevo sobre la esposa.

**Responsable:** Christian Coronel Palma

## Registro

| Fecha | Tema | Decisión | Responsable |
|---|---|---|---|
| 2026-10-05 | D-01 Cámara | Over the Shoulder por defecto; la isométrica es reconocimiento manual con costo | Christian Coronel Palma |
| 2026-10-05 | D-02 Sigilo | Sin conos de visión enemigos visibles; solo señales diegéticas | Christian Coronel Palma |
| 2026-10-05 | D-03 Hackeo | Durante el hackeo el personaje queda inmóvil en OTS; el sellado rápido de puerta usa 2-3 entradas | Christian Coronel Palma |
| 2026-10-05 | D-04 Vista de vigilancia | La isométrica es el acceso a las cámaras de seguridad de The Mother desde el terminal del brazo, con restricciones | Christian Coronel Palma |
| 2026-10-05 | D-05 Resistencia | Se elimina la Evasión Corta y se fusiona el ataque melee en "Empujón / Golpe de culata", sin invulnerabilidad | Christian Coronel Palma |
| 2026-10-05 | D-06 Persistencia | Persisten en el terminal del brazo el conocimiento y las fallas de la máquina | Christian Coronel Palma |
| 2026-10-05 | D-07 Salud | Salud fija sin regeneración con estados Bien, Herido y Crítico | Christian Coronel Palma |
| 2026-10-05 | D-08 Final secundario | 5 muertes consecutivas sin llegar a una sala clave activan el final "Disolución de identidad"; degradación estética acumulativa por muerte. **Modificada por D-35** | Christian Coronel Palma |
| 2026-10-05 | D-09 Fallas de la máquina | The Mother parchea cada falla tras 1-2 usos o al morir; se descubren fallas nuevas en otros intentos | Christian Coronel Palma |
| 2026-10-05 | D-10 Pilares | El Pilar 3 se renombra a "Decisiones con Consecuencias" | Christian Coronel Palma |
| 2026-10-05 | D-11 Estructura | Salas clave fijas + salas procedurales (reemplaza la etiqueta "Lineal") | Christian Coronel Palma |
| 2026-10-05 | D-12 Permadeath | Permadeath individual del clon; los datos persisten. **Modificada por D-18** | Christian Coronel Palma |
| 2026-10-05 | D-13 Premisa | Premisa oficial del hombre que busca la última pista de su esposa fallecida. **Modificada por D-40** | Christian Coronel Palma |
| 2026-10-05 | D-14 Agarre | La liberación de agarre se reemplaza por un objeto defensivo consumible | Christian Coronel Palma |
| 2026-10-05 | D-15 Exploración vertical | Pisos conectados por escaleras o elevadores ubicados solo en salas clave. **Modificada por D-33** | Christian Coronel Palma |
| 2026-10-05 | D-16 Jefe final | The Mother como jefe híbrido de 3 fases; derrotarla es la condición de victoria | Christian Coronel Palma |
| 2026-10-06 | D-17 Recursos | Presupuesto garantizado por piso (mínimo y máximo de cada recurso); el generador decide la ubicación | Christian Coronel Palma |
| 2026-10-06 | D-18 Muerte | El personaje se reconstruye, no se clona: siempre es el mismo hombre, recompuesto con piezas ajenas; se pierde todo el inventario. Modifica D-12 | Christian Coronel Palma |
| 2026-10-06 | D-19 Etiquetas | Se elimina la etiqueta "Permadeath" y se usa "Ciclo de muerte y reconstrucción" | Christian Coronel Palma |
| 2026-10-06 | D-20 Enemigos | Los enemigos eliminados siguen muertos durante el intento, pero The Mother puede reciclar cada uno una sola vez | Christian Coronel Palma |
| 2026-10-06 | D-21 Remate | Dos formas de evitar el reciclaje: golpe en el suelo o consumible de remate | Christian Coronel Palma |
| 2026-10-06 | D-22 Guardado | Suspensión al salir y reanudación en el mismo punto, sin guardado manual; el terminal y las fallas se autoguardan al cambiar. **Modificada por D-30** | Christian Coronel Palma |
| 2026-10-07 | D-23 Señal de vigilancia | Señal escalonada según M-10: sin cambios en sospecha, degradada en búsqueda (más ruido CRT y retraso), cortada y bloqueada en caza; "en alerta" = caza, mismo alcance que M-03. Motivo: Vulnerabilidad Constante y Adaptación | Christian Coronel Palma |
| 2026-10-07 | D-24 Atención de The Mother | Medidor oculto que se llena usando la vigilancia (~5 s) y baja sin usarla (~15 s); al llenarse, los enemigos cercanos pasan a sospecha y luego a búsqueda hacia el jugador. Motivo: Decisiones con Consecuencias y Vulnerabilidad Constante | Christian Coronel Palma |
| 2026-10-07 | D-25 Cobertura de cámaras | Todas las salas clave (secundarias, finales y la del jefe) tienen cámara, más ~1 de cada 3 salas procedurales. Motivo: Adaptación (la cobertura procedural cambia por intento) | Christian Coronel Palma |
| 2026-10-07 | D-26 Persecución entre pisos | Por escalera solo siguen los enemigos en caza, con un breve retraso; por elevador no siguen, pero los perseguidores esperan cerca del elevador si el jugador vuelve. Motivo: Vulnerabilidad Constante y Decisiones con Consecuencias (escalera o elevador cambian el riesgo) | Christian Coronel Palma |
| 2026-10-07 | D-27 Remate | Disponible a ~1.5 m mirando un cuerpo no rematado, también en combate; si hay otro interactivo al lado, gana el que está al centro de la vista. Motivo: Decisiones con Consecuencias (rematar en combate es una elección arriesgada) | Christian Coronel Palma |
| 2026-10-07 | D-28 Jefe | Fase 2 con cámaras hostiles (luz roja y sonido de motor; si una enfoca al jugador, los enemigos acuden a ese punto); sin reciclaje en la sala del jefe; la fase 3 tiene munición para todos los puntos débiles más 1–2 disparos de margen. Motivo: Vulnerabilidad Constante | Christian Coronel Palma |
| 2026-10-07 | D-29 Degradación | La degradación estética solo se acumula y nunca se reinicia; ninguna capa tapa la interfaz crítica ni las señales de M-10. Motivo: Adaptación (el ciclo deja huella sin ocultar información) | Christian Coronel Palma |
| 2026-10-07 | D-30 Guardado | Sin suspensión; autoguardado al llegar a la sala clave final de los pisos 1 y 2 sin enemigos en caza; aviso diegético "sincronizando…"; solo para continuar tras salir o un fallo; gracia de 2–3 s al cargar. Modifica D-22. Motivo: Decisiones con Consecuencias y Vulnerabilidad Constante | Christian Coronel Palma |
| 2026-10-07 | D-31 Salir del juego | En alerta o caza cuenta como muerte completa; con calma fuera de una sala final guardada suma 1 al contador y a la degradación y se continúa desde la sala final; en una sala final guardada sin alerta, sin costo; un cierre inesperado no cuenta. Motivo: Vulnerabilidad Constante y Decisiones con Consecuencias | Christian Coronel Palma |
| 2026-10-07 | D-32 Reiniciar y Nueva partida | "Reiniciar" (menú de pausa) regresa al inicio del ciclo, cuenta como muerte y conserva el terminal; "Nueva partida" (menú principal) borra todo, incluido el terminal, con confirmación. Motivo: Decisiones con Consecuencias (reiniciar no es gratuito) | Christian Coronel Palma |
| 2026-10-07 | D-33 Estructura de pisos | 3 pisos; cada uno con 8–12 salas procedurales, 2–3 salas clave secundarias sin guardado y 1 sala clave final (escalera o elevador); sala de The Mother en el piso 3, sin guardado. Modifica D-15. Motivo: Adaptación | Christian Coronel Palma |
| 2026-10-07 | D-34 Configuraciones | Completa: 8–12 procedurales + 2–3 secundarias + 1 final por piso (~45–60 min). Showcase: 6–8 procedurales + 2 secundarias + 1 final por piso (~30 min), con enemigos y recursos escalados en proporción. Motivo: Adaptación (misma estructura en ambas duraciones) | Christian Coronel Palma |
| 2026-10-07 | D-35 Contador de 5 muertes | Solo lo reinicia llegar a la sala final del piso 2; las muertes del primer intento no cuentan; Reiniciar y salir con costo suman. Modifica D-08. Motivo: Vulnerabilidad Constante y Adaptación | Christian Coronel Palma |
| 2026-10-07 | D-36 Llaves | 1 llave obligatoria por piso que abre la sala final y llaves opcionales para atajos o salas con recursos; objetos clave de 1 casilla que se consumen al usarse; la llave obligatoria siempre es alcanzable antes de la puerta de la sala final. Motivo: Adaptación y Decisiones con Consecuencias (la llave compite por espacio) | Christian Coronel Palma |
| 2026-10-07 | D-37 Valores iniciales | Valores iniciales de munición, disparos, velocidad por salud, resistencia, máximos por casilla, enemigos por piso, atención, enemigo reciclado y gracia al cargar (ver [[02 - Mecánicas]] y [[06 - Economía y Recursos]]). Motivo: Vulnerabilidad Constante y Decisiones con Consecuencias (escasez medible) | Christian Coronel Palma |
| 2026-10-07 | D-38 Tutorial | Integrado en las primeras salas del piso 1, fijas solo en el primer intento; desde el segundo intento esas salas son procedurales. Motivo: Adaptación (se aprende una vez y luego el escenario cambia) | Christian Coronel Palma |
| 2026-10-07 | D-39 Presentación | Cinemática completa de reconstrucción una sola vez; cinemática corta saltable en cada muerte o reinicio; transición de piso de mínimo 5 s sin pantalla de carga; menú principal Continuar, Nueva partida, Opciones, Créditos, Salir; créditos saltables tras verlos una vez. Motivo: Adaptación | Christian Coronel Palma |
| 2026-10-07 | D-40 Premisa | La esposa del protagonista está desaparecida, no muerta: "Un hombre busca la última pista que su esposa desaparecida le dejó, pero queda atrapado en un ciclo sin fin del que ahora debe salir."; qué le ocurrió y su relación con la instalación y The Mother siguen Por definir. Modifica D-13. Motivo: Adaptación (descubrimiento progresivo) y horror cósmico por información incompleta | Christian Coronel Palma |


> **Navegación:** [[00 - Índice]] · ← [[02 - Referencias e Inspiración]] · [[04 - Historial de Versiones]] →
