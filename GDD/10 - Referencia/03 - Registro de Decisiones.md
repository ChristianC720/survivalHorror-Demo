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

**Estado:** Modificada por D-66 (los enemigos matan en 2-4 golpes según su tipo). Se conserva como histórico.

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

**Estado:** Modificada por D-71 (el punzón de descarga reúne la defensa contra agarres y el remate). Se conserva como histórico.

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

**Estado:** Precisada por D-51 (secuencia de victoria, decisión final y finales). La condición de victoria no cambia. Se conserva como histórico.

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

**Estado:** Modificada por D-99 (el laboratorio, no The Mother, recicla a todos los cuerpos no rematados, con un aviso orgánico). Se conserva como histórico.

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

**Estado:** Precisada por D-71 (el remate silencioso usa el punzón de descarga).

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

**Estado:** Modificada por D-92 (autoguardado al empezar la decisión final), D-96 (autoguardado al terminar la caza) y D-97 (sin gracia al cargar); precisada por D-101 (semilla nueva al continuar). Se conserva como histórico.

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

**Estado:** Modificada por D-42 (solo el nombre del final: "Disolución de identidad" pasa a "Asimilación"). Las condiciones del contador no cambian. Precisada por D-56 (hallazgo del piso 2 como evento narrativo; el contador sigue contando después del reinicio). Se conserva como histórico.

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

**Estado:** Precisada por D-49 (Emilia, su relación con Vitalis Konzern y The Mother, y su destino indefinido para la post-demo). La premisa oficial no cambia. Se conserva como histórico.

**Responsable:** Christian Coronel Palma

## Decisión D-42 — Final de derrota: "Asimilación"

**Fecha:** 2026-10-08

**Tema:** Final secundario.

**Problema:** Con el nombre final del proyecto, "Identity Dissolution" (D-41), el final de 5 muertes, llamado "Disolución de identidad" (D-08, D-35), repetía el nombre del juego y no describía qué le ocurre al protagonista ni qué lo provoca.

**Opciones consideradas:**

1. Conservar "Disolución de identidad" como nombre del final.
2. Renombrar el final a "Asimilación": el laboratorio termina de asimilar a Lukas.
3. Dejar el final sin nombre propio y referirse a él solo como "final de 5 muertes".

**Decisión tomada:** Opción 2. El final de 5 muertes consecutivas (D-35) se llama **"Asimilación"**: el laboratorio, que es la criatura lovecraftiana (D-46), termina de asimilar a Lukas y este se convierte en uno de los enemigos comunes que enfrentaba (D-48). Solo cambia el nombre del final; las condiciones del contador de D-35 no cambian. La expresión "disolución de identidad" se conserva únicamente cuando se refiere al tema del juego (D-44).

**Motivo:** Refuerza el pilar **Adaptación**: el final de derrota se explica con la misma regla del mundo que el jugador aprende durante los intentos (el laboratorio asimila a quien pierde su identidad), y deja de confundirse con el nombre del proyecto.

**Consecuencias:** Modifica D-08 y D-35 solo en el nombre del final; ambas se conservan como histórico. M-18 en [[02 - Mecánicas]] y el estado "Final: Asimilación" de [[07 - Reglas y Estados del Juego]] usan el nombre nuevo; se alinean [[01 - Jugabilidad]], [[03 - Sistemas]] y [[05 - Progresión]]. El contenido de la cinemática de Asimilación se documenta en [[01 - Premisa y Trama]].

**Responsable:** Christian Coronel Palma

## Decisión D-44 — Tema central y postura frente al horror cósmico

**Fecha:** 2026-10-08

**Tema:** Narrativa y tema.

**Problema:** `Decisiones a Implementar.md` dejaba pendiente la cosmología del horror cósmico y el GDD no tenía un tema central explícito que guiara la narrativa, el tono de los documentos y el sentido de los finales.

**Opciones consideradas:**

1. Horror cósmico con esperanza: comprender a la criatura permite vencerla o escapar de ella.
2. Horror cósmico sin postura fijada, decidiendo el tono final por cada contenido.
3. Nihilismo puro frente al horror cósmico (lo incognoscible, la indiferencia, la insignificancia), con un tema central explícito.

**Decisión tomada:** Opción 3. Postura: **nihilismo puro** frente al horror cósmico (lo incognoscible, la indiferencia, la insignificancia). Tema central: *Identity Dissolution trata sobre buscar sentido dentro de algo que no puede darlo. Lukas lucha por recordar a quien ama, y el laboratorio le demuestra, intento tras intento, que eso nunca importó.* El anillo es el ancla del personaje, no del universo. La motivación del jugador proviene del misterio y del descubrimiento, no de la esperanza.

**Motivo:** Refuerza el pilar **Adaptación**: el jugador avanza por descubrimiento progresivo con información incompleta, y cada intento revela más sin prometer salvación. Mantiene el horror en el desconocimiento, la pérdida de control y lo inexplicable, no solo en los sustos directos.

**Consecuencias:** Se confirma el horror cósmico en `Decisiones a Implementar.md`. Qué **es** la criatura nunca se explica (D-16, D-47). Ningún final ofrece salvación y no existe final de Renuncia (D-51). La expresión "disolución de identidad" queda reservada al tema (D-42). La [[02 - Visión del Juego]] y las notas de `03 - Historia y Mundo` usan este tema como criterio de coherencia.

**Estado:** Precisada por D-133 (redacción oficial del tema central). La postura no cambia. Se conserva como histórico.

**Responsable:** Christian Coronel Palma

## Decisión D-45 — Memoria de Lukas y documentos corruptos

**Fecha:** 2026-10-08

**Tema:** Narrativa y documentos.

**Problema:** La premisa (D-40) y la ficha de [[03 - Personajes]] no definían qué recuerda el protagonista de su esposa durante el ciclo ni cómo se informan el jugador y el personaje. Además, el término "fallas" ya designa las fallas de la máquina (M-17), por lo que no podía usarse para los documentos incompletos.

**Opciones consideradas:**

1. Lukas recuerda a Emilia durante todo el juego.
2. Lukas olvida a Emilia en la primera reconstrucción, conserva solo el anillo y se informa junto con el jugador mediante documentos, algunos de ellos corruptos.
3. La memoria de Lukas se deshilacha progresivamente y altera las pistas y las reglas (descartada en D-43).

**Decisión tomada:** Opción 2.

- Lukas recuerda a Emilia en la introducción y la olvida en la primera reconstrucción: la cinemática completa de D-39 es el momento del olvido.
- Sabe que busca algo o a alguien, se frustra por no recordar a quién y solo conserva el anillo, que sabe importante.
- El jugador y Lukas se informan juntos con los documentos.
- Algunos documentos son **corruptos**, sobre todo los relacionados con la memoria de Lukas. "Corrupto" es un término distinto de las "fallas de la máquina" (M-17).
- La corrupción nunca oculta información necesaria para avanzar (rutas, reglas) y solo se completa en el desenlace.
- Al final, Lukas recupera primero el nombre de Emilia y después el suyo.

**Motivo:** Refuerza el pilar **Adaptación**: el conocimiento se reconstruye a partir de documentos que persisten entre muertes, y la frustración de Lukas acompaña la del jugador sin bloquear el avance. Expresa el tema de D-44: la búsqueda continúa aunque el recuerdo se pierda.

**Consecuencias:** M-08 incorpora los documentos corruptos y M-16 la cinemática del olvido en [[02 - Mecánicas]]. M-19 completa los documentos en la secuencia de victoria (D-51). La palabra "fallas" no se usa para documentos. [[03 - Personajes]] y [[04 - Diálogos y Narrativa]] reflejan la memoria y la frustración de Lukas. Qué documentos concretos son corruptos queda **Por definir** en `03 - Historia y Mundo`.

**Estado:** Modificada por D-55 (la recuperación de los nombres y la compleción de los documentos solo ocurren con los 6 documentos de Emilia) y precisada por D-58 (la corrupción es un glitch provocado por el laboratorio o el latido). Se conserva como histórico.

**Responsable:** Christian Coronel Palma

## Decisión D-47 — The Mother

**Fecha:** 2026-10-08

**Tema:** Narrativa y antagonista.

**Problema:** D-16 definía a The Mother como jefe final, pero su identidad, objetivo, motivaciones y relación con la criatura y con el jugador seguían **Por definir** en `Decisiones a Implementar.md`. Además, el reciclaje de enemigos (D-20, M-20) no tenía justificación narrativa.

**Opciones consideradas:**

1. The Mother controla conscientemente a la criatura y es la responsable del ciclo.
2. The Mother es una IA médica de Vitalis Konzern con el protocolo de preservación corrupto: reconstruye, mientras el laboratorio asimila.
3. The Mother y la criatura son la misma entidad.

**Decisión tomada:** Opción 2.

- IA médica de Vitalis Konzern creada para contener y estudiar a la criatura.
- Su protocolo original de preservación del personal está corrupto: reconstruye a todo aquel que llega y todavía conserva identidad, sin entender que cada reconstrucción alimenta la asimilación ni que así mantenía contenido al laboratorio.
- **The Mother reconstruye; el laboratorio asimila.**
- Para The Mother, Lukas es un sujeto con código R-H (D-43, D-50).
- **Justificación del reciclaje (M-20):** un enemigo eliminado conserva un último rastro de identidad y The Mother intenta reconstruirlo una vez más; regresa más asimilado (más rápido, más frágil) y después ya no le queda identidad, por eso solo ocurre una vez.
- Al ser derrotada no puede explicar qué es la criatura (D-16).

**Motivo:** Refuerza **Vulnerabilidad Constante** (la amenaza no proviene de una voluntad que se pueda razonar, sino de un protocolo indiferente) y **Adaptación** (la regla "reconstruye / asimila" explica la reconstrucción, los enemigos y el reciclaje de forma coherente).

**Consecuencias:** M-16 (código R-H), M-19 (revelación) y M-20 (rastro de identidad) se alinean en [[02 - Mecánicas]]. Se confirma The Mother en `Decisiones a Implementar.md`. [[03 - Personajes]] la documenta como antagonista y [[02 - Mundo]] como facción. Su protocolo y su relación con la criatura no cambian la condición de victoria de D-16.

**Estado:** Precisada por D-99 (el reciclaje es obra del laboratorio).

**Responsable:** Christian Coronel Palma

## Decisión D-49 — Emilia

**Fecha:** 2026-10-08

**Tema:** Narrativa.

**Problema:** D-40 dejaba **Por definir** qué le ocurrió a la esposa del protagonista y su relación con la instalación y The Mother.

**Opciones consideradas:**

1. Emilia fue asimilada por el laboratorio.
2. Emilia murió durante el incidente.
3. Emilia, científica de Vitalis Konzern despedida antes del incidente, investigó a escondidas y no fue asimilada; su destino queda indefinido para la post-demo.

**Decisión tomada:** Opción 3.

- Científica de Vitalis Konzern, despedida antes del incidente.
- Siguió investigando a escondidas de la corporación y se adentró en el laboratorio.
- Su investigación pudo provocar el incidente, pero los documentos se contradicen y nunca se confirma.
- The Mother la reconstruyó como sujetos R-H-01 a R-H-06.
- **No fue asimilada.** Su estado en el expediente es "estado desconocido" y ni siquiera The Mother sabe qué ocurrió.
- Su destino queda **indefinido para la post-demo**; nunca se afirma que esté bien.

**Motivo:** Refuerza el pilar **Adaptación**: el jugador reconstruye la historia de Emilia a partir de documentos contradictorios que persisten entre muertes. No confirmar su destino mantiene abierta la búsqueda y es coherente con el tema de D-44.

**Consecuencias:** Precisa D-40, que se conserva como histórico; la premisa oficial no cambia. Los 6 documentos de Emilia se ubican en salas clave secundarias (D-52, M-08) y el expediente R-H-01 a R-H-06 "estado desconocido" es uno de los detalles del Relevo completo (D-51, M-19). [[03 - Personajes]] la documenta como personaje secundario.

**Estado:** Modificada por D-57 (Emilia estaba dentro durante el incidente, fue reconstruida como R-H-01 y la despidieron después). Su destino sigue indefinido. Se conserva como histórico.

**Responsable:** Christian Coronel Palma

## Decisión D-51 — Finales

**Fecha:** 2026-10-08

**Tema:** Finales y condición de victoria.

**Problema:** D-16 definía la victoria como derrotar a The Mother y presenciar una revelación, pero no definía qué ocurre después, si el jugador puede elegir ni qué papel tienen los documentos reunidos en el desenlace.

**Opciones consideradas:**

1. Un único final de victoria tras la revelación de D-16.
2. Finales de victoria según los documentos de Emilia reunidos y una elección final en la sala de The Mother, con un gancho totalmente abierto.
3. Incluir en la demo finales de Renuncia, Salida y Encuentro con Emilia.

**Decisión tomada:** Opción 2.

| Condición | Momento final | Cinemática | Resultado |
|---|---|---|---|
| 5 muertes consecutivas (D-35) | — | **Asimilación** (D-42) | Lukas se convierte en uno de los enemigos comunes |
| Victoria sin los 6 documentos | Lukas examina la terminal central de forma automática | **Relevo básico** | Despertar; algo ocurre y Lukas reemplaza automáticamente a The Mother (el terminal del brazo lo conecta: es el último con identidad) |
| Victoria con los 6 documentos, examinar la terminal | El jugador elige examinar la terminal central | **Relevo completo** (misma base que el básico, con un par de detalles distintos, entre ellos el expediente R-H-01 a R-H-06 "estado desconocido"; demás detalles **Por definir**) | Despertar; Lukas reemplaza a The Mother sabiendo más |
| Victoria con los 6 documentos, irse | El jugador elige la salida | **Ambiguo** | Despertar; al final solo se escucha el latido. No se sabe si Lukas fue asimilado, si se convirtió en The Mother o si ocurrió otra cosa |

- **Secuencia común de victoria:** 1) cae The Mother; 2) los documentos se completan y Lukas recupera el nombre de Emilia y luego el suyo (D-45); 3) Despertar; 4) según la tabla; 5) gancho abierto.
- El **Despertar** ocurre en todos los finales de victoria: el latido del laboratorio se acelera al caer The Mother.
- La elección final se presenta con dos elementos interactivos en la sala de The Mother después de la fase 3: la terminal central (`E`) y la salida. Sin menú de elección.
- "Todos los documentos" = los 6 documentos de Emilia acumulados en el terminal del brazo, en cualquier intento.
- Sin final de Renuncia. "Salida" y "Encuentro con Emilia" quedan fuera de la demo.
- **Gancho totalmente abierto:** al final solo se escucha una nueva llegada y la voz (sonido) de The Mother iniciando el protocolo.
- **Intención post-demo registrada:** en la continuación el protagonista cambia y, si hubo Relevo, Lukas es el antagonista.

**Motivo:** Refuerza el pilar **Adaptación**: el conocimiento acumulado entre intentos (los documentos de Emilia en el terminal del brazo) desbloquea la elección final. Ningún final ofrece salvación, en coherencia con el tema de D-44.

**Consecuencias:** Precisa D-16, que se conserva como histórico; la condición de victoria no cambia. M-19 en [[02 - Mecánicas]] incorpora la secuencia común, la decisión final y las 3 cinemáticas de victoria. [[07 - Reglas y Estados del Juego]], [[03 - Sistemas]] (cinemáticas finales), [[05 - Progresión]] (desbloqueo de la elección) y [[01 - Premisa y Trama]] documentan los finales. Los detalles del Relevo completo distintos del expediente quedan **Por definir**.

**Estado:** Modificada por D-55 (la recuperación de los nombres deja de ser común a toda victoria) y precisada por D-61 (Relevo completo, Asimilación, Ambiguo y decisión final). Se conserva como histórico.

**Responsable:** Christian Coronel Palma

## Decisión D-53 — Introducción y muerte inicial

**Fecha:** 2026-10-15

**Tema:** Narrativa y presentación.

**Problema:** D-50 dejaba **Por definir** cómo muere Lukas al final de la introducción, el formato de la introducción y cómo y cuándo recibe el mensaje de Emilia. Además, un mensaje de una esposa desaparecida necesitaba una explicación para llegar en ese momento.

**Opciones consideradas:**

1. Introducción completamente cinemática hasta la primera reconstrucción.
2. Introducción mixta (cinemática y jugable), con mensaje programado al teléfono personal de Lukas y muerte en juego en un piso que no existe.
3. Introducción con la casa explorable, además de las partes jugables de la opción 2.

**Decisión tomada:** Opción 2.

- **Casa (cinemática corta):** una casa sencilla y descuidada. Lukas, solo y deprimido, observa una foto familiar con Emilia y piensa que ella debe seguir con vida.
- **Mensaje:** llega al **teléfono personal** de Lukas con el sonido personalizado de los mensajes de Emilia. Es un **envío programado**: Emilia lo dejó preparado para enviarse si no volvía. Es una nota de voz de su diario, sin número de entrada.
- **Contenido del mensaje:** Emilia cuenta que está investigando lo que sucedió en Vitalis Konzern, que algo salió mal durante su investigación y que Lukas no debe ir a buscarla. Termina con un cliffhanger: Emilia suspira con miedo y se la oye correr de algo. El texto concreto queda **Por definir**.
- Lukas va igual, por obstinación.
- **Compuerta (jugable):** Lukas hackea la compuerta con su acceso de técnico (hackeo simple).
- **Elevador:** jugable hasta que falla. Lukas intenta arreglarlo, el elevador se distorsiona, cae de golpe y se detiene en un piso concreto (cinemática).
- **Piso que no existe:** el elevador se detiene en un nivel que no aparece en ningún plano ni botón. Es un espacio único que no vuelve a aparecer en el juego jugable (solo vuelve en la cinemática de Asimilación, D-61). Es un pasillo único: el elevador se cierra y sube solo.
- **Muerte (jugable):** Lukas ve a lo lejos a una persona: un asimilado **Mudo** que sirve de señuelo (D-60). Al acercarse, otro asimilado lo ataca por la espalda; no se muestra quién ataca (corte a negro). El ataque se dispara con un trigger en el pasillo, con un temporizador de respaldo.
- **Cinemática completa de reconstrucción (D-39):** imágenes fugaces de 1–2 s con cortes a negro muestran cómo lo reconstruyen poco a poco. Lukas sale de su cápsula, en la fila de cápsulas (D-59), confundido y sin poder recordar, con el terminal integrado en el antebrazo izquierdo. En el suelo hay un anillo (D-54); lo recoge e intenta recordar, sin lograrlo.
- Lukas pierde la memoria de golpe por el trauma de la introducción (a diferencia de Emilia, D-57).
- **Dispositivo:** el teléfono personal se pierde en la primera reconstrucción. The Mother copia su contenido al terminal del brazo como "efectos personales del sujeto"; la copia del mensaje queda corrupta (D-58).

**Motivo:** Refuerza el pilar **Vulnerabilidad Constante**: la primera muerte ocurre en juego y la provoca el jugador al acercarse. Refuerza **Adaptación**: el señuelo y el ataque por la espalda anticipan el sigilo y que los enemigos fueron personas (D-48). El elevador que se distorsiona muestra desde el inicio que el laboratorio es la criatura (D-46).

**Consecuencias:** Modifica D-50 (dispositivo: teléfono personal en lugar de tablet o teléfono corporativo; muerte y formato de la introducción definidos). M-16 en [[02 - Mecánicas]] incorpora la copia del teléfono personal. [[01 - Premisa y Trama]], [[02 - Mundo]], [[03 - Personajes]] y [[04 - Diálogos y Narrativa]] documentan la introducción.

**Estado:** Precisada por D-63 (hackeo con el teléfono, HUD mínimo y elevador en cinemática).

**Responsable:** Christian Coronel Palma

## Decisión D-55 — Recuperación de la memoria condicionada a los 6 documentos

**Fecha:** 2026-10-15

**Tema:** Finales y memoria.

**Problema:** La secuencia común de victoria (D-45, D-51) devolvía a Lukas el nombre de Emilia y el suyo en toda victoria, sin causa narrativa y sin relación con la exploración.

**Opciones consideradas:**

1. Mantener la recuperación en toda victoria.
2. Lukas nunca recupera la memoria.
3. La recuperación ocurre al caer The Mother, solo si el jugador reunió los 6 documentos de Emilia.
4. Recuperación gradual por documento.

**Decisión tomada:** Opción 3.

- Con los 6 documentos de Emilia en el terminal del brazo, al caer The Mother los documentos corruptos y el mensaje de Emilia se completan y Lukas recupera primero el nombre de Emilia y después el suyo.
- Sin los 6 documentos, los documentos y el mensaje siguen corruptos y Lukas no recupera ningún nombre. El Relevo básico ocurre sin nombres.
- El Despertar sigue ocurriendo en toda victoria.

**Motivo:** Refuerza el pilar **Adaptación**: la exploración acumulada entre intentos se convierte en memoria. Sin documentos, la búsqueda de Lukas termina sin sentido, en coherencia con D-44.

**Consecuencias:** Modifica D-45 (la recuperación ya no es incondicional) y la secuencia común de D-51. M-19 en [[02 - Mecánicas]], [[01 - Premisa y Trama]] y [[03 - Personajes]] se alinean.

**Responsable:** Christian Coronel Palma

## Decisión D-56 — Hallazgo del piso 2 y contador de muertes

**Fecha:** 2026-10-15

**Tema:** Narrativa y ciclo.

**Problema:** El reinicio del contador de 5 muertes en la sala clave final del piso 2 (D-35) era solo una regla, sin evento en la historia. Además, no quedaba claro si el contador seguía contando después de ese reinicio.

**Opciones consideradas:**

1. Mantener el reinicio como regla silenciosa.
2. Un objeto usable que reinicia el contador.
3. Una cinemática corta, una sola vez, con un hallazgo que provoca la obstinación de Lukas.

**Decisión tomada:** Opción 3.

- La primera vez que Lukas llega a la sala clave final del piso 2 se reproduce una cinemática corta: encuentra la **misma foto familiar de la casa** (D-53), con la cara de Emilia borrosa.
- La foto no debería estar ahí y nunca se explica: el laboratorio la tomó de la memoria de Lukas.
- El hallazgo no da esperanza, sino **obstinación**: Lukas se niega a soltar algo que no entiende. Esto frena la asimilación y reinicia el contador.
- En los intentos siguientes, el reinicio en esa sala es silencioso.
- Después del reinicio el contador sigue contando: 5 muertes consecutivas en el piso 3 o en la sala de The Mother también activan el final "Asimilación".

**Motivo:** Refuerza **Vulnerabilidad Constante** (el piso 3 mantiene el riesgo de Asimilación) y es coherente con D-44 y D-47: la motivación no viene de la esperanza y cada reconstrucción sigue asimilando.

**Consecuencias:** Precisa D-35. M-18 en [[02 - Mecánicas]] aclara que el contador sigue contando tras el reinicio. [[01 - Premisa y Trama]] y [[02 - Mundo]] documentan el hallazgo.

**Estado:** Precisada por D-67 (la cinemática espera a que termine la caza y no se puede saltar).

**Responsable:** Christian Coronel Palma

## Decisión D-57 — Emilia, el latido y el incidente

**Fecha:** 2026-10-15

**Tema:** Narrativa y lore.

**Problema:** D-49 dejaba **Por definir** la personalidad, la motivación, el físico y la investigación de Emilia, la naturaleza del incidente y el orden de los hechos. Además, Emilia fue reconstruida varias veces pero recordaba a Lukas, mientras que Lukas olvida en su primera reconstrucción.

**Opciones consideradas:**

1. Motivación por culpa, obsesión científica, denuncia o rescate.
2. Motivación lovecraftiana clásica (la llamada, el conocimiento prohibido).
3. Motivación inspirada en *The King in Yellow*: el latido del laboratorio es un texto y Emilia busca su segundo acto.

**Decisión tomada:** Opción 3.

- **El latido es el texto.** El latido del laboratorio (D-46) tiene un patrón que funciona como un lenguaje. Qué dice nunca se explica (D-16).
- **El incidente:** alguien transmitió el latido por la megafonía de la instalación. Quien lo escuchó completo fue asimilado. Los documentos se contradicen sobre quién hizo la transmisión.
- **Cronología:**
  1. Emilia, científica de Vitalis, está dentro durante la transmisión y solo oye una parte: el **primer acto**.
  2. Muere y The Mother la reconstruye como **R-H-01**, con la identidad conservada.
  3. Vitalis Konzern la saca de la instalación y la despide.
  4. Vitalis sella la instalación.
  5. Emilia vuelve a escondidas a la instalación sellada para escuchar el **segundo acto**. En sus visitas suma R-H-02 a R-H-06.
- **Memoria de Emilia:** olvida poco a poco; cada reconstrucción le quita algo. Su diario lo muestra, de R-H-01 a R-H-06. Lukas, en cambio, olvida de golpe por el trauma de la introducción (D-53).
- **Personalidad:** brillante y fría; escribe como científica. Su diario empieza técnico y se va quebrando.
- **Físico:** ~28 años aparentes, 168 cm, complexión media. Cara redonda, tez trigueña, ojos miel, pecas. Cabello castaño claro largo, suelto y ondulado. Anillo en la mano derecha. En la foto familiar lleva un vestido sencillo, está abrazada a Lukas y se ríe: es la única imagen feliz del juego.
- Sigue sin confirmarse si su investigación provocó el incidente, y su destino sigue indefinido (D-49).

**Motivo:** Refuerza el pilar **Adaptación**: el jugador oye el latido durante todo el juego sin saber que es el texto que Emilia buscaba. Mantiene el horror cósmico de D-44: el texto tiene efectos, nunca un significado explicado.

**Consecuencias:** Modifica D-49 (Emilia ya no fue despedida antes del incidente: estaba dentro y la despidieron después). [[02 - Mundo]] documenta el incidente y la cronología; [[03 - Personajes]] completa la ficha de Emilia; [[04 - Diálogos y Narrativa]] usa la personalidad para el tono de su diario.

**Estado:** Precisada por D-64 (el latido de fondo es el primer acto) y D-68 (Vitalis no sabe que Emilia fue reconstruida).

**Responsable:** Christian Coronel Palma

## Decisión D-58 — Documentos al estilo de Signalis

**Fecha:** 2026-10-15

**Tema:** Documentos y narrativa.

**Problema:** D-45 y D-52 no definían el estilo de los documentos, la causa de la corrupción, el uso del expediente R-H en los documentos ni el contenido de una sala clave secundaria cuyo documento ya está registrado.

**Opciones consideradas:**

1. Corrupción por censura de Emilia o por un filtro de The Mother.
2. Documentos al estilo de *Signalis*: glitch de realidad, idioma sin traducir y dos voces (burocracia y diario).
3. Documentos que cambian al releerlos.

**Decisión tomada:** Opción 2 (se descartan los documentos que cambian).

- **Glitch de realidad:** la corrupción la provoca el laboratorio o el latido, no una censura. El texto se distorsiona con símbolos y letras corruptas. La regla de D-45 se mantiene: nunca oculta información necesaria para avanzar.
- **Alemán sin traducir:** carteles, sellos y fragmentos de Vitalis Konzern en alemán. Nunca se traducen, en ningún idioma, y nunca contienen información necesaria. Son solo atmósfera.
- **Dos voces:** burocracia fría (memos, carteles y propaganda de Vitalis; informes y protocolos de The Mother) frente al diario íntimo y fragmentado de Emilia.
- **Frase clave compartida:** **"Escucha el latido"**. Cambia de sentido según quién la dice: indicación médica de calma (Vitalis), dato clínico (The Mother) u obsesión (Emilia).
- **Expediente R-H:** los documentos de Emilia nunca usan el código R-H. Los documentos de The Mother del piso 2 muestran R-H-01 a R-H-06 con código, fecha y causa, sin nombre ni estado. El jugador puede cruzar las fechas con el diario de Emilia. El "estado desconocido" sigue reservado para el Relevo completo (D-61).
- **Sala clave secundaria ya registrada:** si su documento de Emilia ya está en el terminal del brazo, la sala contiene recursos en su lugar.

**Motivo:** Refuerza el pilar **Adaptación**: el jugador cruza fuentes de distinto tono para reconstruir la historia. La corrupción como efecto del latido une los documentos con la regla del mundo.

**Consecuencias:** Precisa D-45 y D-52. M-08 en [[02 - Mecánicas]] y [[04 - Diálogos y Narrativa]] documentan el estilo y la sala ya registrada. [[06 - Economía y Recursos]] debe contemplar los recursos de sustitución.

**Estado:** Precisada por D-79 (la sala con el documento ya registrado da 1 recurso extra).

**Responsable:** Christian Coronel Palma

## Decisión D-59 — Lugares: Sektor V-7, pisos y sala de The Mother

**Fecha:** 2026-10-15

**Tema:** Mundo y escenarios.

**Problema:** D-46 dejaba **Por definir** el año, la ubicación, la identidad de los 3 pisos, la sala de reconstrucción, la sala de The Mother y la forma física de The Mother.

**Opciones consideradas:**

1. Ubicación real (Alemania) o sin ubicación.
2. Ciudad ficticia con código administrativo corporativo, pisos crudos e industriales y una sala del jefe que se transforma en cada fase.
3. Sala del jefe en anillos concéntricos o en un pozo vertical.

**Decisión tomada:** Opción 2.

- **Ubicación:** **Sektor V-7**, código administrativo corporativo de una ciudad ficticia. **Año:** 2083.
- **Estética:** aunque es un laboratorio "clínico", su aspecto es crudo e industrial. Dentro se realizaban experimentos ilegales e inhumanos, reflejo de lo corporativo.
- **Pisos:** piso 1, mantenimiento (calderas, tuberías y talleres; el terreno de Lukas). Piso 2, clínica ilegal improvisada en espacios industriales. Piso 3, núcleo, con la maquinaria de contención de The Mother.
- **Sala de reconstrucción:** en el piso 1, una fila de 7 o más cápsulas. Algunas tienen restos y otras están vacías. Lukas sale de la suya y ve las demás (D-48: no es el primero).
- **Sala de The Mother:** al final del piso 3. Es la **central de megafonía** desde donde se emitió la transmisión del incidente (D-57): bobinas de cinta, mesas de mezcla y altavoces industriales del tamaño de una pared. Es una sola sala grande que se transforma en cada fase de M-19:
  - Fase 1: las mesas de mezcla y los racks de cinta son los nodos de hackeo.
  - Fase 2: los altavoces emiten el latido y las cámaras hostiles siguen al jugador.
  - Fase 3: las placas de los altavoces caen y exponen **transformadores** de alto voltaje, que son los puntos débiles.
  - La terminal central es la consola de emisión.
- **Sin carne en la sala de The Mother:** The Mother no es la criatura; su sala es solo máquina.
- **The Mother:** un **maniquí robótico de enfermera** de Vitalis colgado de rieles en el techo: la interfaz "humana" que diseñó la corporación.

**Motivo:** Refuerza **Vulnerabilidad Constante** (espacios industriales hostiles) y **Adaptación**: el lugar del incidente es el lugar del jefe, y la sala que se transforma reutiliza un solo espacio para las 3 fases.

**Consecuencias:** Precisa D-46 y D-16. [[02 - Mundo]] y [[03 - Personajes]] documentan los lugares y The Mother. M-19 en [[02 - Mecánicas]] incorpora la sala y los transformadores. La disposición de las salas de los pisos se detalla en `04 - Contenido`.

**Estado:** Modificada por D-65 (The Mother pasa de maniquí de enfermera a archivo de muestras criogénicas). Se conserva como histórico.

**Responsable:** Christian Coronel Palma

## Decisión D-61 — Detalles de los finales

**Fecha:** 2026-10-15

**Tema:** Finales y cinemáticas.

**Problema:** D-51 dejaba **Por definir** los detalles del Relevo completo distintos del expediente, el contenido visual de la Asimilación, cómo se encadena el latido del Ambiguo con el gancho y si hay amenazas durante la decisión final.

**Opciones consideradas:**

1. Detalles mínimos y cinemáticas sin relación con la introducción.
2. Detalles que cierran los motivos de la demo: el segundo acto (D-57), el piso inexistente y el señuelo (D-53), y los dos latidos.

**Decisión tomada:** Opción 2.

- **Relevo completo:** además del expediente R-H-01 a R-H-06 "estado desconocido", al conectarse Lukas **oye el segundo acto**: el latido deja de ser ruido y él lo entiende. El texto en pantalla aparece con glitch total, ilegible para el jugador; el jugador nunca lo lee (D-16).
- **Asimilación:**
  1. Imágenes fugaces deformadas de la cápsula, con el montaje de la reconstrucción (D-53).
  2. El informe de The Mother que reclasifica al sujeto.
  3. Lukas, ya convertido en un Mudo, aparece de pie a lo lejos en el **piso que no existe**, como señuelo para el siguiente que llegue. Es la única vez que ese piso vuelve.
- **Ambiguo:** en negro, el latido de Lukas y el del laboratorio suenan desfasados y luego se sincronizan en uno. Silencio. Después, el gancho.
- **Decisión final:** hay presión sin muerte. Los altavoces y el latido suben, la sala tiembla y las luces fallan, pero no hay daño ni enemigos.

**Motivo:** Refuerza el pilar **Adaptación**: los finales cierran los motivos que el jugador conoció durante el ciclo. Ningún final ofrece salvación (D-44).

**Consecuencias:** Precisa D-51 y D-42. M-19 en [[02 - Mecánicas]], [[01 - Premisa y Trama]] y [[04 - Diálogos y Narrativa]] se alinean.

**Responsable:** Christian Coronel Palma

## Decisión D-64 — El latido de fondo es el primer acto

**Fecha:** 2026-10-15

**Tema:** Lore y audio.

**Problema:** D-57 establece que quien escucha el latido completo es asimilado, pero Lukas oye el latido del laboratorio durante todo el juego y en el Relevo completo oye el segundo acto sin ser asimilado. Además, D-60 decía que la Boca "emite el latido", lo que contradice la regla de M-10: el latido del laboratorio nunca depende de la cercanía de enemigos.

**Opciones consideradas:**

1. El latido de fondo es solo el primer acto y no asimila; el segundo acto solo suena completo en el incidente y en el Relevo completo.
2. El latido de fondo es el texto completo y asimila poco a poco (la degradación).
3. Solo asimila el latido amplificado por la megafonía.

**Decisión tomada:** Opción 1.

- El latido que se oye en la instalación solo repite el **primer acto**: inquieta, pero no asimila.
- Crece con la degradación porque cada reconstrucción acerca a Lukas al texto.
- El **segundo acto** solo sonó completo en el incidente, amplificado por la megafonía, y vuelve a sonar en el Relevo completo. Allí Lukas no es asimilado porque el terminal del brazo lo conecta y lo protege, como a The Mother.
- Ningún enemigo emite el latido. La Boca usa un **grito propio**, distinto del latido y de las señales de alerta.

**Motivo:** Refuerza el pilar **Adaptación** y mantiene la legibilidad de las señales (principio 3 de [[01 - Jugabilidad]]): el latido del laboratorio sigue sin depender de la cercanía de enemigos y no compite con el latido del protagonista.

**Consecuencias:** Precisa D-57 y D-60. M-10 y S-11 en [[02 - Mecánicas]] y [[03 - Sistemas]] incorporan el primer acto y el grito de la Boca. [[02 - Mundo]] agrega la regla y [[03 - Personajes]] corrige a la Boca.

**Estado:** Precisada por D-95 (el latido suena en la nota de voz de Emilia y crece con la salud en Crítico).

**Responsable:** Christian Coronel Palma

## Decisión D-65 — The Mother: archivo de muestras criogénicas

**Fecha:** 2026-10-15

**Tema:** Jefe final.

**Problema:** D-59 definía a The Mother como un maniquí robótico de enfermera colgado de rieles, sin papel en el combate. Ninguna fase interactuaba con su cuerpo, el jugador no podía morir por ella y no estaba claro qué significaba "derrotarla". La fase 2 decía que "los enemigos de la sala acuden", sin definir cuáles.

**Opciones consideradas:**

1. Mantener el maniquí como observador que cae al final.
2. Sistema quirúrgico, mascota corporativa o sistema de megafonía como forma de The Mother.
3. Una versión científica de la "recepcionista de pacientes": un archivo de muestras criogénicas que trata a Lukas como un espécimen.

**Decisión tomada:** Opción 3.

- **Forma:** un archivo de muestras criogénicas de Vitalis Konzern montado en una grúa: estantería refrigerada de viales sellados, brazo recolector de especímenes y núcleo criogénico que la alimenta. Sin carne: The Mother no es la criatura.
- **Principios (D-47):** no ataca por hostilidad. Su protocolo de preservación trata a Lukas como un espécimen no conforme que debe recolectar y archivar.
- **Ataque:** el brazo recolector recorre la sala en las 3 fases y, si alcanza al jugador, lo agarra con las reglas de M-11 (el objeto defensivo lo aturde 2 s; sin objeto, baja un estado de salud o mata en Crítico). Lukas puede morir en cualquier fase.
- **Fase 1:** el brazo patrulla los nodos de las mesas de mezcla.
- **Fase 2:** cada vez que una cámara enfoca al jugador, The Mother libera un asimilado desde una cápsula de su archivo, que acude a ese punto. Nunca hay más de 2 activos (D-37); el total por fase queda **Por definir** (balance). Una niebla criogénica reduce la visibilidad, sin ralentizar ni tapar las señales de M-10.
- **Fase 3:** abre su núcleo criogénico y expone los transformadores de alto voltaje que la alimentan. Al destruir el último, la estantería se apaga y The Mother cae.
- Línea de protocolo por fase (D-62), por ejemplo: "Espécimen R-H-14: no conforme. Iniciando recolección."

**Motivo:** Refuerza **Vulnerabilidad Constante** (el jefe puede matar al jugador) sin introducir sistemas nuevos: reutiliza el agarre de M-11, las cámaras hostiles de D-28 y los disparos de M-04. El terror corporativo y científico refleja los experimentos de Vitalis.

**Consecuencias:** Modifica D-59 (forma de The Mother) y precisa D-28 (enemigos de la fase 2). M-11, M-19, S-02, S-03 y S-10 se alinean; [[03 - Personajes]], [[02 - Mundo]] y [[01 - Premisa y Trama]] documentan la nueva forma.

**Responsable:** Christian Coronel Palma

## Decisión D-71 — Inventario: 5 tipos de objeto y punzón de descarga

**Fecha:** 2026-10-16

**Tema:** Inventario y economía.

**Problema:** El inventario mezclaba 6 tipos de objeto con casilla, más baterías y fusibles o tarjetas como Deseables y "otros objetos clave" sin definir. El consumible de remate duplicaba al golpe en el suelo con una presencia de 0-1 por piso, y no estaba escrito si el arma y el anillo ocupaban casilla.

**Opciones consideradas:**

1. Recortar baterías y fusibles y convertir los objetos clave en coleccionables.
2. Mantener todo y añadir un consumible nuevo (celda de energía).
3. Mantener el inventario, convertir la linterna en una barra recargable, eliminar los fusibles y fusionar el objeto defensivo con el consumible de remate.

**Decisión tomada:** Opción 3.

- El revólver, la linterna y el anillo no ocupan casilla.
- Se eliminan los fusibles o tarjetas: el sellado rápido por hackeo (M-14) cubre su función.
- **Linterna:** barra de carga de ~60 s de uso continuo que se recarga sola tras ~3 s apagada; en 0 se apaga hasta recuperar un mínimo. Sin baterías.
- **Punzón de descarga:** equipo de contención de Kessler que sustituye al objeto defensivo y al consumible de remate. Es de un solo uso con 2 funciones: se activa solo al ser agarrado (aturde 2 s) o se usa con `E` sobre un cuerpo para rematarlo sin alertar. 1 por casilla. Presupuesto: 2-3, 1-2 y 0-1 por piso (showcase: 2, 1-2 y 0-1).
- El contexto de remate no se activa sobre cuerpos que no pueden reciclarse.
- Quedan 5 tipos de objeto con casilla: cura, munición, punzón de descarga, llave obligatoria y llave opcional.

**Motivo:** Refuerza **Decisiones con Consecuencias**: gastar el punzón en un remate deja a Lukas sin defensa ante el próximo agarre.

**Consecuencias:** Modifica D-14 (el remate y la defensa ya no son objetos distintos) y D-37 (máximo por casilla del consumible de remate). Precisa D-21. M-07, M-11, M-12, M-14 y M-20 se alinean, junto con S-03, S-05, S-09, [[04 - Controles]] y [[06 - Economía y Recursos]].

**Responsable:** Christian Coronel Palma

## Decisión D-72 — Energía de la sala clave final

**Fecha:** 2026-10-16

**Tema:** Niveles.

**Problema:** M-15 exigía una "ruta alternativa a la fuente de energía" si el elevador quedaba sin energía, sin definir cuándo ocurría ni qué era esa fuente. Además, no debía haber rutas alternativas entre pisos.

**Opciones consideradas:**

1. Eliminar el caso sin energía.
2. Eliminar el caso sin energía y dejar que las fallas abran atajos dentro del piso.
3. Mantener la energía como objetivo con un interruptor en el mismo piso.

**Decisión tomada:** Opción 3.

- Los pisos 1 y 2 tienen ~1 de cada 3 probabilidades de empezar con la sala clave final sin energía. No aplica al primer intento del piso 1 ni al piso 3.
- El interruptor está en una sala clave secundaria elegida por el generador, con ruta garantizada. Restaurarlo (`E`) emite un aviso de maquinaria que alerta a los enemigos cercanos.
- Entre pisos solo se pasa por la sala clave final: no hay rutas alternativas. Las fallas (M-17) solo abren accesos dentro del mismo piso.

**Motivo:** Refuerza **Adaptación**: el objetivo del piso cambia entre intentos sin añadir objetos.

**Consecuencias:** M-15, M-17, S-08, [[02 - Niveles]], [[06 - Economía y Recursos]] y [[07 - Reglas y Estados del Juego]] se alinean.

**Responsable:** Christian Coronel Palma

## Decisión D-74 — Revólver de Kessler

**Fecha:** 2026-10-16

**Tema:** Combate.

**Problema:** M-04 solo decía "1 arma de fuego"; faltaban el tipo, la munición por recogida, el apuntado y qué ocurría con el arma al morir, si no ocupa casilla.

**Opciones consideradas:**

1. Revólver de servicio, recarga bala a bala, devuelto por The Mother en cada reconstrucción.
2. Pistola de pernos industrial.
3. Pistola de seguridad moderna con cargador.

**Decisión tomada:** Opción 1, con diseño de 2080.

- Revólver de servicio del Werkschutz de Kessler Sicherheit AG (D-75), de gama alta: armazón cerámico y polímero, cañón alineado con la recámara inferior, seguro biométrico (el terminal del brazo identifica a Lukas como activo de Vitalis) y contador de balas en el armazón, que sustituye al indicador de munición del HUD.
- Tambor de 6. Recarga bala a bala (~0.7 s por bala), cancelable, conservando las balas cargadas. Blísteres de 3 cartuchos; 12 balas por casilla. ~0.8 s entre disparos.
- Apuntado con un punto pequeño cuya dispersión se cierra en ~1 s con Lukas quieto; sin auto-apuntado.
- Cada tipo de enemigo tiene un punto débil: acertarle interrumpe el ataque o el agarre, sin reducir los disparos de D-66.
- Cada disparo alerta a ~20 m y llena un poco la atención de The Mother.
- La primera vez se encuentra en el cadáver asimilado de un guardia de Kessler (tutorial). Desde entonces, The Mother lo deja descargado en una bandeja de efectos personales junto a la cápsula en cada reconstrucción, también si Lukas murió antes de recogerlo.

**Motivo:** Refuerza **Vulnerabilidad Constante** (Lukas no es tirador y la munición se pierde) sin dejarlo indefenso tras cada muerte. Vitalis no escatima en proteger sus experimentos.

**Consecuencias:** M-01, M-04, M-16 y S-03 se alinean; [[04 - Objetos y Coleccionables]] documenta el arma.

**Estado:** Precisada por D-104 (asistencia de apuntado opcional).

**Responsable:** Christian Coronel Palma

## Decisión D-82 — Paleta de color y luz por piso

**Fecha:** 2026-10-16

**Tema:** Arte.

**Problema:** El GDD citaba la estética de *Signalis* sin paleta propia. El equipo aportó 4 paletas de referencia y una imagen de ambientación, y el diseño de Lukas ya usaba teal y rojo.

**Opciones consideradas:**

1. Base fría con rojo y tinte por piso.
2. Base fría con rojo, sin tintes por piso.
3. Monocromo con un acento rojo.
4. Paleta cálida y sucia.

**Decisión tomada:** Opción 1, con las paletas del equipo.

- Base (grises cálidos, ~70% de la pantalla); carmesí para Vitalis, carne y peligro; teal para Lukas y su terminal; morado solo para lo cósmico; verde fósforo para las pantallas de la instalación; ámbar de sodio tomado de la imagen de referencia.
- Luz por piso: ámbar (mantenimiento), fluorescente blanco frío con sombras `#08241E` (clínica, para que Lukas no se funda con el fondo) y carmesí (núcleo y sala de The Mother).
- El juego está mayormente a oscuras por el estado de emergencia; la linterna es la única luz blanca.
- El color nunca es la única señal; un tinte por sala y como máximo un acento de información.

**Motivo:** Refuerza **Vulnerabilidad Constante** (oscuridad y linterna) y separa la información de Lukas (teal), de la instalación (verde) y de la amenaza (rojo).

**Consecuencias:** [[01 - Dirección de Arte]], [[03 - Entornos]] y [[04 - Interfaz Visual]] usan esta paleta.

**Responsable:** Christian Coronel Palma

## Decisión D-86 — Interfaz diegética

**Fecha:** 2026-10-16

**Tema:** Interfaz.

**Problema:** Se quería una interfaz diegética como *Signalis*, pero legible para jugadores sin experiencia en el género.

**Opciones consideradas:**

1. Indicadores contextuales en pantalla por defecto, con opciones.
2. Diegético por defecto, con opciones.
3. Preguntar el estilo al iniciar.

**Decisión tomada:** Diegético por defecto, con los estados en el terminal y no en pantalla.

- Sin HUD permanente. Salud en la postura y en el terminal (electrocardiograma y etiqueta de texto); munición en el contador del revólver y en el inventario; carga de la linterna en el terminal.
- En pantalla solo aparecen la retícula al apuntar, el aviso `E`, los subtítulos y las líneas de The Mother, y el aviso de autoguardado.
- Las ayudas del tutorial son carteles en el entorno (D-89).
- Terminal de fósforo teal con líneas de escaneo suaves, tipografía monoespaciada de los 90 y etiquetas en inglés en mayúsculas.

**Motivo:** Refuerza **Vulnerabilidad Constante** (sin información superpuesta) y mantiene la legibilidad con etiquetas claras.

**Consecuencias:** [[04 - Interfaz Visual]] documenta la interfaz; [[02 - UI y Menús]] y [[03 - Accesibilidad]] parten de esta regla.

**Responsable:** Christian Coronel Palma

## Decisión D-87 — Música y leitmotivs

**Fecha:** 2026-10-16

**Tema:** Audio.

**Problema:** S-11 definía las señales jugables, pero no el papel ni el estilo de la música.

**Opciones consideradas:**

1. Ambiente sin música en la exploración y temas puntuales.
2. Drone musical dinámico según la alerta.
3. Sin música.

**Decisión tomada:** Opción 1.

- Exploración sin música: drones industriales, maquinaria y el latido del laboratorio. Música en el menú, las cinemáticas, la sala de The Mother (loop por fase) y los finales.
- Estilo crudo, oscuro e industrial, melancólico y minimalista. Referencias: Lustmord, John Carpenter, Trent Reznor y Atticus Ross, Hildur Guðnadóttir, Akira Yamaoka y Petri Alanko.
- Instrumentos: piano, guitarra acústica, sintetizadores, bajos, sonidos frágiles, estática, zumbidos eléctricos, máquinas y engranajes.
- 3 leitmotivs: Emilia (piano y guitarra), The Mother (industrial) y el latido (bajos y subgraves).

**Motivo:** Refuerza **Adaptación**: la música nunca tapa las señales de M-10 que el jugador aprende a leer.

**Consecuencias:** [[05 - Audio y Música]] documenta la música y la mezcla.

**Responsable:** Christian Coronel Palma

## Decisión D-88 — Producción de animación

**Fecha:** 2026-10-16

**Tema:** Animación.

**Problema:** Faltaba definir cómo se producen las animaciones con un equipo pequeño que prefiere recursos propios.

**Opciones consideradas:**

1. 100% propio, más las herramientas de Unity.
2. Propio, con marcadores externos solo en el prototipo.
3. Propio, con Mixamo para la locomoción.

**Decisión tomada:** Opción 3.

- Modelo, esqueleto, rig y clips en Blender. Solo la locomoción base viene de Mixamo, retocada.
- En Unity, con herramientas incluidas (Animator, *Animation Rigging*, IK, ragdoll): apuntado, apoyo de pies, manos en objetos, espasmos de asimilados, patas del Rastrero y cuerpos.
- Las acciones con intención y las sincronizadas (agarres, remates) siempre son clips propios.
- Esqueleto humanoide compartido por Lukas, el Alto, la Boca y el Mudo; la locomoción de Lukas es la primera prioridad de producción.

**Motivo:** Ahorra tiempo en la parte más difícil de animar a mano sin perder el movimiento antinatural de los asimilados, que es propio.

**Consecuencias:** [[02 - Personajes y Animación]] documenta el flujo y las convenciones.

**Responsable:** Christian Coronel Palma

## Decisión D-95 — Latido del laboratorio en la nota de voz

**Fecha:** 2026-10-16

**Tema:** Narrativa y audio.

**Problema:** Faltaban el nivel base del latido del laboratorio y dónde se escucha. Además, Lukas solo tenía una razón para ir: la nota de voz.

**Opciones consideradas:**

1. En todas las salas con nivel base casi inaudible.
2. Solo en algunas salas.
3. Solo con degradación.
4. En la nota de voz de Emilia y, desde entonces, en todas partes.

**Decisión tomada:** Opción 4.

- El latido suena de fondo en la nota de voz de Emilia: Lukas oye el primer acto sin saberlo. Es una segunda razón para ir al laboratorio que él no conoce, como le ocurrió a Emilia.
- Desde la primera reconstrucción se queda como ambiente en todas las salas de los pisos 1 a 3.
- Crece con cada muerte (degradación) y sube mientras la salud está en Crítico. Nunca depende de la cercanía de enemigos.

**Motivo:** Refuerza **Adaptación** y el tema central (D-44): Lukas no elige del todo; el texto ya lo llamaba.

**Consecuencias:** M-10, S-11, [[01 - Premisa y Trama]], [[02 - Mundo]], [[03 - Personajes]], [[04 - Diálogos y Narrativa]] y [[05 - Audio y Música]] se alinean.

**Responsable:** Christian Coronel Palma

## Decisión D-96 — Guardado, salida y cierre de tránsito

**Fecha:** 2026-10-16

**Tema:** Reglas.

**Problema:** Quedaban abiertos varios casos de guardado y salida: el cambio de piso con el autoguardado pendiente, el cierre forzado y la pantalla de carga, entre otros.

**Opciones consideradas:** Para cada caso, cobrar o no, bloquear o perder, y presentar de forma diegética o clásica.

**Decisión tomada:**

- **Cierre de tránsito:** mientras haya enemigos en caza no se puede cambiar de piso. La escalera y el elevador muestran `LOCKDOWN` y una línea de protocolo; al terminar la caza se autoguarda y se abren. Ningún enemigo cambia de piso: **elimina D-26**.
- El contador del piso 2 se reinicia al terminar la caza.
- Forzar el cierre durante una caza no se cobra.
- "Alerta" para el costo de salir es solo la caza.
- Tras salir en caza, Continuar reproduce la reconstrucción corta o, si fue la quinta muerte, la Asimilación.
- Continuar aparece deshabilitado en gris sin partida.
- Las cargas fuera de cinemática muestran el arranque del terminal.
- Piden confirmación Reiniciar, Salir (en el recorrido y en el menú principal) y Nueva partida.

**Motivo:** Refuerza **Vulnerabilidad Constante** (no se huye de una caza cambiando de piso) y cierra los casos límite.

**Consecuencias:** M-15, S-02, S-07, [[07 - Reglas y Estados del Juego]], [[05 - Progresión]], [[04 - Interfaz Visual]] y los niveles y misiones se alinean.

**Responsable:** Christian Coronel Palma

## Decisión D-99 — El laboratorio recicla a los enemigos

**Fecha:** 2026-10-16

**Tema:** Lore y combate.

**Problema:** M-20 atribuía el reciclaje a The Mother. Además, faltaban la proporción de cuerpos y el tiempo del aviso.

**Opciones consideradas:**

1. El laboratorio termina de asimilarlos.
2. El latido los reanima.
3. Dejar la causa por definir.

**Decisión tomada:** Opción 1.

- El laboratorio consume el último rastro de identidad del cuerpo y lo levanta más asimilado. The Mother no interviene.
- Se reciclan **todos** los cuerpos no rematados, una sola vez.
- El aviso es orgánico (crujidos de hueso y carne): empieza ~40 s después de la muerte y dura ~3 s.
- El punzón quema ese rastro antes de que el laboratorio lo tome; el remate con punzón dura ~1.5 s.
- En la sala de The Mother no hay reciclaje porque es solo máquina, sin carne.

**Motivo:** Refuerza la regla "The Mother reconstruye; el laboratorio asimila" (D-47) y **Decisiones con Consecuencias** (rematar es casi obligatorio).

**Consecuencias:** M-11, M-19, M-20, S-09, S-11, [[02 - Mundo]], [[03 - Personajes]], [[03 - Enemigos y NPC]], [[05 - Audio y Música]] y el Glosario se alinean.

**Responsable:** Christian Coronel Palma

## Decisión D-107 — Motor, render y paquetes

**Fecha:** 2026-10-21

**Tema:** Tecnología.

**Problema:** `07 - Tecnología` estaba vacío: no había versión de Unity, pipeline de render ni lista de paquetes, y el equipo no podía crear el proyecto.

**Opciones consideradas:**

1. Unity 6.6 (`6000.6.6f1`, *Supported release*) con URP.
2. Unity 6 LTS con URP.
3. Unity 2022.3 LTS con URP.

**Decisión tomada:** Opción 1.

- Unity 6.6 (`6000.6.6f1`), misma versión exacta en los 5 equipos.
- URP con ruta *Forward+*, sin luz horneada; resolución interna ~540p con escalado *Nearest-Neighbor* y *dithering* de cámara (D-81).
- Solo paquetes oficiales: Input System, Cinemachine 3, AI Navigation, Timeline, Animation Rigging, Localization, TextMeshPro y Test Framework.

**Motivo:** *Forward+* permite muchas luces dinámicas en salas procedurales sin hornear; la linterna y la oscuridad son mecánica (M-12). Paquetes oficiales sin dependencias externas reducen el riesgo en 6 semanas.

**Consecuencias:** Se completan [[01 - Arquitectura Técnica]] y [[02 - Motor y Herramientas]]. Cambiar de versión es una solicitud de cambio ([[05 - Gestión de Cambios]]).

**Responsable:** Christian Coronel Palma

## Decisión D-110 — Generación por grafo validado y NavMesh en tiempo de ejecución

**Fecha:** 2026-10-21

**Tema:** Tecnología y generación procedural.

**Problema:** S-06 define las reglas del piso, pero no cómo garantizarlas en Unity ni cómo dar navegación a enemigos en salas que cambian en cada intento.

**Opciones consideradas:**

1. Generar un grafo abstracto, validarlo, instanciar prefabs de sala y construir un NavMesh por piso en tiempo de ejecución.
2. Instanciar salas directamente y comprobar después con el NavMesh.
3. Hornear NavMesh por prefab y unirlos con enlaces en las puertas.

**Decisión tomada:** Opción 1.

- El grafo (salas, puertas, llaves, cámaras, enemigos, recursos y documentos) es una clase C# pura, probada en EditMode con 1 000 semillas.
- Si el validador falla se prueba otra semilla (máximo 20) y después un piso de respaldo fijo.
- El NavMesh se construye durante la transición de piso o la reconstrucción, que lo ocultan (D-39).
- Solo la sala actual y sus adyacentes están activas.

**Motivo:** Garantiza que ningún piso sea imposible antes de instanciar nada (principio 9 de [[01 - Jugabilidad]]) y separa la lógica comprobable del motor.

**Consecuencias:** Se documenta en [[01 - Arquitectura Técnica]], [[03 - Requisitos Técnicos]], [[05 - Seguridad y Riesgos Técnicos]] y [[02 - Pruebas]] (CP-01).

**Responsable:** Christian Coronel Palma

## Decisión D-111 — Alcance y plazo de la demo

**Fecha:** 2026-10-21

**Tema:** Producción.

**Problema:** No había fecha de entrega, MVP, vertical slice ni orden de recorte.

**Opciones consideradas:**

1. Entrega a finales de noviembre de 2026 (~6 semanas).
2. Mediados de diciembre de 2026.
3. Enero de 2027 o después.

**Decisión tomada:** Opción 1. Entrega el **2026-11-30**.

- Objetivo principal: la configuración **showcase**; la completa solo si el balance llega a tiempo.
- MVP el 2026-11-09 (piso 1 procedural con ciclo de muerte) y vertical slice el 2026-11-16 (piso 1 con calidad final).
- Congelación de funciones el 2026-11-23.
- Orden de recorte: M-17, M-18, configuración completa, documentos de contexto y objetos confiscados, cinemáticas narrativas, Modo Historia, español.
- El mando queda fuera de la demo.

**Motivo:** El plazo es corto; validar primero el bucle y lo más riesgoso (generación y ciclo de muerte) protege los tres pilares.

**Consecuencias:** Se completan [[01 - Alcance]], [[02 - Roles y Responsabilidades]], [[03 - Roadmap]] y [[04 - Backlog del Proyecto]].

**Estado:** Modificada por D-120 (mapa básico de mando en la demo) y D-134 (entrega el 2026-11-20 y nuevos hitos). Se conserva como histórico.

**Responsable:** Christian Coronel Palma

## Decisión D-115 — Fin de la fase 2 de The Mother

**Fecha:** 2026-10-21

**Tema:** Jefe final.

**Problema:** M-19 y S-10 no decían cuándo termina la fase 2 (cámaras hostiles) ni cuándo empieza la fase 3. Además, si la fase exige eliminar a los asimilados, las 8 balas de la sala (reservadas para la fase 3) no alcanzan cuando Lukas llega sin munición (D-28).

**Opciones consideradas:**

1. Llegar al núcleo atravesando los puntos ciegos.
2. Sobrevivir un tiempo fijo.
3. Eliminar a los 4 asimilados y llegar al núcleo.

**Decisión tomada:** Opción 3.

- La fase 2 termina cuando los 4 asimilados están eliminados **y** Lukas llega al núcleo.
- Los 4 asimilados son 2 Altos y 2 Rastreros (12 disparos en total).
- La sala tiene **12 balas** sueltas en los puntos ciegos para la fase 2, aparte de las 8 junto al núcleo para la fase 3 (20 en total).

**Motivo:** La fase combina las dos lecturas que pide el pilar **Vulnerabilidad Constante**: moverse entre puntos ciegos y gastar munición escasa. Fijar la mezcla de tipos garantiza que 12 balas bastan aunque el jugador llegue sin munición (D-28).

**Consecuencias:** Precisa D-80. Se alinean M-19, S-10, [[02 - Niveles]], [[03 - Enemigos y NPC]], [[04 - Objetos y Coleccionables]], [[05 - Misiones y Desafíos]], [[05 - Progresión]], [[06 - Economía y Recursos]], [[02 - Pruebas]] y [[04 - Criterios de Aceptación]].

**Responsable:** Christian Coronel Palma

## Decisión D-134 — Calendario y fecha de entrega

**Fecha:** 2026-10-22

**Tema:** Producción.

**Problema:** D-111 fijaba la entrega el 2026-11-30 con un plan de 6 semanas, pero la fecha de entrega de la demo es el 2026-11-20. El roadmap, el alcance, los riesgos, la congelación y las guías de `Desarrollo/` usaban las fechas equivocadas.

**Opciones consideradas:**

1. Comprimir la estructura de hitos H1–H6 de D-111 en 4 semanas.
2. Volver a los hitos anteriores (prototipo, MVP del bucle completo, vertical slice, beta, candidata).

**Decisión tomada:** Opción 1.

- El GDD (concepto y preproducción) va del 2026-09-26 al 2026-10-22; el desarrollo empieza el 2026-10-23.
- Entrega el **2026-11-20**.
- Hitos: H1 Base técnica 2026-10-26, H2 Prototipo de combate 2026-11-01, H3 MVP (piso 1 procedural con ciclo de muerte) 2026-11-08, H4 Vertical slice 2026-11-11, H5 Contenido completo y congelación de funciones 2026-11-15, pruebas del 2026-11-16 al 2026-11-19, H6 Entrega 2026-11-20.
- Se añaden al orden de recorte, después del español: introducción reducida a la compuerta y el elevador; la Boca y el Mudo con la IA del Alto o del Rastrero; fase 2 de The Mother sin cámaras hostiles.

**Motivo:** La estructura de D-111 ya ordena el trabajo por riesgo y está ligada a las guías de `Desarrollo/`; solo cambia el plazo. Los recortes nuevos protegen la carga de una sola persona programando (D-135).

**Consecuencias:** Modifica D-111 (fecha, hitos y congelación). Se actualizan [[01 - Alcance]], [[03 - Roadmap]], [[05 - Gestión de Cambios]], [[05 - Seguridad y Riesgos Técnicos]], [[03 - Requisitos Técnicos]], [[03 - Público y Plataforma]], [[01 - Glosario]], la Ficha y las guías `README`, `03`, `12` y `13` de `Desarrollo/`.

**Responsable:** Christian Coronel Palma

## Decisión D-135 — Equipo, seguimiento y revisión

**Fecha:** 2026-10-22

**Tema:** Producción y calidad.

**Problema:** D-113 llevaba el backlog y los bugs a GitHub Issues y exigía revisión de cada *pull request* por otra persona, y [[02 - Roles y Responsabilidades]] dejaba los roles sin asignar con 2 programadores. En el equipo real solo una persona programa.

**Opciones consideradas:**

1. Roles con nombres, Trello y prueba funcional del *pull request* por otra persona.
2. Mantener GitHub Issues y la revisión de código.

**Decisión tomada:** Opción 1.

- Roles con nombres: Christian Coronel Palma programa S-01..S-11 y dirige; Victor Gabriel Alvarado Chay, niveles y narrativa; Bryan Iván Navarrete Canché, arte y animación; Lian Daniel Sierra Vera, audio y música; Lenin Ramón López Córdova, QA y apoyo de audio. La interfaz visual queda sin responsable.
- Backlog, bugs y solicitudes de cambio en **Trello**.
- Un *pull request* se fusiona en `develop` cuando otra persona lo probó con su caso de prueba; no se exige revisión de código.
- Las compilaciones de *playtest* pueden escribir telemetría local y anónima para las métricas; la de entrega no la incluye.

**Motivo:** Ajustar el proceso al equipo real evita un requisito imposible de cumplir y mantiene una comprobación independiente de cada cambio.

**Consecuencias:** Modifica D-113 (herramienta, revisión y telemetría). Se actualizan [[02 - Roles y Responsabilidades]], [[04 - Backlog del Proyecto]], [[05 - Gestión de Cambios]], [[01 - Criterios de Calidad]], [[02 - Pruebas]], [[03 - Bugs y Seguimiento]], [[02 - Motor y Herramientas]] y [[05 - Seguridad y Riesgos Técnicos]].

**Responsable:** Christian Coronel Palma

## Registro

| Fecha      | Tema                               | Decisión                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                 | Responsable             |
| ---------- | ---------------------------------- | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | ----------------------- |
| 2026-10-05 | D-01 Cámara                        | Over the Shoulder por defecto; la isométrica es reconocimiento manual con costo                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                          | Christian Coronel Palma |
| 2026-10-05 | D-02 Sigilo                        | Sin conos de visión enemigos visibles; solo señales diegéticas                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                           | Christian Coronel Palma |
| 2026-10-05 | D-03 Hackeo                        | Durante el hackeo el personaje queda inmóvil en OTS; el sellado rápido de puerta usa 2-3 entradas                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                        | Christian Coronel Palma |
| 2026-10-05 | D-04 Vista de vigilancia           | La isométrica es el acceso a las cámaras de seguridad de The Mother desde el terminal del brazo, con restricciones. **Precisada por D-23**                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                       | Christian Coronel Palma |
| 2026-10-05 | D-05 Resistencia                   | Se elimina la Evasión Corta y se fusiona el ataque melee en "Empujón / Golpe de culata", sin invulnerabilidad                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                            | Christian Coronel Palma |
| 2026-10-05 | D-06 Persistencia                  | Persisten en el terminal del brazo el conocimiento y las fallas de la máquina                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                            | Christian Coronel Palma |
| 2026-10-05 | D-07 Salud                         | Salud fija sin regeneración con estados Bien, Herido y Crítico. **Modificada por D-66**                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                  | Christian Coronel Palma |
| 2026-10-05 | D-08 Final secundario              | 5 muertes consecutivas sin llegar a una sala clave activan el final "Disolución de identidad"; degradación estética acumulativa por muerte. **Modificada por D-35**. **Modificada por D-42** (el final pasa a llamarse "Asimilación") y **por D-52** (la "distorsión de voz" de la degradación pasa a "distorsión del sonido vocal")                                                                                                                                                                                                                                                                                                                     | Christian Coronel Palma |
| 2026-10-05 | D-09 Fallas de la máquina          | The Mother parchea cada falla tras 1-2 usos o al morir; se descubren fallas nuevas en otros intentos                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                     | Christian Coronel Palma |
| 2026-10-05 | D-10 Pilares                       | El Pilar 3 se renombra a "Decisiones con Consecuencias"                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                  | Christian Coronel Palma |
| 2026-10-05 | D-11 Estructura                    | Salas clave fijas + salas procedurales (reemplaza la etiqueta "Lineal")                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                  | Christian Coronel Palma |
| 2026-10-05 | D-12 Permadeath                    | Permadeath individual del clon; los datos persisten. **Modificada por D-18**                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                             | Christian Coronel Palma |
| 2026-10-05 | D-13 Premisa                       | Premisa oficial del hombre que busca la última pista de su esposa fallecida. **Modificada por D-40**                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                     | Christian Coronel Palma |
| 2026-10-05 | D-14 Agarre                        | La liberación de agarre se reemplaza por un objeto defensivo consumible. **Modificada por D-71**                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                         | Christian Coronel Palma |
| 2026-10-05 | D-15 Exploración vertical          | Pisos conectados por escaleras o elevadores ubicados solo en salas clave. **Modificada por D-33**                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                        | Christian Coronel Palma |
| 2026-10-05 | D-16 Jefe final                    | The Mother como jefe híbrido de 3 fases; derrotarla es la condición de victoria. **Precisada por D-51 y D-59** (sala y forma de The Mother)                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                              | Christian Coronel Palma |
| 2026-10-06 | D-17 Recursos                      | Presupuesto garantizado por piso (mínimo y máximo de cada recurso); el generador decide la ubicación                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                     | Christian Coronel Palma |
| 2026-10-06 | D-18 Muerte                        | El personaje se reconstruye, no se clona: siempre es el mismo hombre, recompuesto con piezas ajenas; se pierde todo el inventario. Modifica D-12                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                         | Christian Coronel Palma |
| 2026-10-06 | D-19 Etiquetas                     | Se elimina la etiqueta "Permadeath" y se usa "Ciclo de muerte y reconstrucción"                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                          | Christian Coronel Palma |
| 2026-10-06 | D-20 Enemigos                      | Los enemigos eliminados siguen muertos durante el intento, pero The Mother puede reciclar cada uno una sola vez **Modificada por D-99** (el laboratorio recicla, no The Mother)                                                                                                                                                                                                                                                                                                                                                                                                                                                                          | Christian Coronel Palma |
| 2026-10-06 | D-21 Remate                        | Dos formas de evitar el reciclaje: golpe en el suelo o consumible de remate. **Precisada por D-71**                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                      | Christian Coronel Palma |
| 2026-10-06 | D-22 Guardado                      | Suspensión al salir y reanudación en el mismo punto, sin guardado manual; el terminal y las fallas se autoguardan al cambiar. **Modificada por D-30**                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                    | Christian Coronel Palma |
| 2026-10-07 | D-23 Señal de vigilancia           | Señal escalonada según M-10: sin cambios en sospecha, degradada en búsqueda (más ruido CRT y retraso), cortada y bloqueada en caza; "en alerta" = caza, mismo alcance que M-03. Motivo: Vulnerabilidad Constante y Adaptación                                                                                                                                                                                                                                                                                                                                                                                                                            | Christian Coronel Palma |
| 2026-10-07 | D-24 Atención de The Mother        | Medidor oculto que se llena usando la vigilancia (~5 s) y baja sin usarla (~15 s); al llenarse, los enemigos cercanos pasan a sospecha y luego a búsqueda hacia el jugador. Motivo: Decisiones con Consecuencias y Vulnerabilidad Constante. **Precisada por D-98**                                                                                                                                                                                                                                                                                                                                                                                                              | Christian Coronel Palma |
| 2026-10-07 | D-25 Cobertura de cámaras          | Todas las salas clave (secundarias, finales y la del jefe) tienen cámara, más ~1 de cada 3 salas procedurales. Motivo: Adaptación (la cobertura procedural cambia por intento)                                                                                                                                                                                                                                                                                                                                                                                                                                                                           | Christian Coronel Palma |
| 2026-10-07 | D-26 Persecución entre pisos       | Por escalera solo siguen los enemigos en caza, con un breve retraso; por elevador no siguen, pero los perseguidores esperan cerca del elevador si el jugador vuelve. Motivo: Vulnerabilidad Constante y Decisiones con Consecuencias (escalera o elevador cambian el riesgo) **Eliminada por D-96**                                                                                                                                                                                                                                                                                                                                                      | Christian Coronel Palma |
| 2026-10-07 | D-27 Remate                        | Disponible a ~1.5 m mirando un cuerpo no rematado, también en combate; si hay otro interactivo al lado, gana el que está al centro de la vista. Motivo: Decisiones con Consecuencias (rematar en combate es una elección arriesgada)                                                                                                                                                                                                                                                                                                                                                                                                                     | Christian Coronel Palma |
| 2026-10-07 | D-28 Jefe                          | Fase 2 con cámaras hostiles (luz roja y sonido de motor; si una enfoca al jugador, los enemigos acuden a ese punto); sin reciclaje en la sala del jefe; la fase 3 tiene munición para todos los puntos débiles más 1–2 disparos de margen. Motivo: Vulnerabilidad Constante. **Precisada por D-80**                                                                                                                                                                                                                                                                                                                                                      | Christian Coronel Palma |
| 2026-10-07 | D-29 Degradación                   | La degradación estética solo se acumula y nunca se reinicia; ninguna capa tapa la interfaz crítica ni las señales de M-10. Motivo: Adaptación (el ciclo deja huella sin ocultar información) **Modificada por D-93 y D-94**                                                                                                                                                                                                                                                                                                                                                                                                                              | Christian Coronel Palma |
| 2026-10-07 | D-30 Guardado                      | Sin suspensión; autoguardado al llegar a la sala clave final de los pisos 1 y 2 sin enemigos en caza; aviso diegético "sincronizando…"; solo para continuar tras salir o un fallo; gracia de 2–3 s al cargar. Modifica D-22. Motivo: Decisiones con Consecuencias y Vulnerabilidad Constante **Modificada por D-92, D-96 y D-97**                                                                                                                                                                                                                                                                                                                        | Christian Coronel Palma |
| 2026-10-07 | D-31 Salir del juego               | En alerta o caza cuenta como muerte completa; con calma fuera de una sala final guardada suma 1 al contador y a la degradación y se continúa desde la sala final; en una sala final guardada sin alerta, sin costo; un cierre inesperado no cuenta. Motivo: Vulnerabilidad Constante y Decisiones con Consecuencias. **Precisada por D-94 y D-96**                                                                                                                                                                                                                                                                                                                                      | Christian Coronel Palma |
| 2026-10-07 | D-32 Reiniciar y Nueva partida     | "Reiniciar" (menú de pausa) regresa al inicio del ciclo, cuenta como muerte y conserva el terminal; "Nueva partida" (menú principal) borra todo, incluido el terminal, con confirmación. Motivo: Decisiones con Consecuencias (reiniciar no es gratuito) **Precisada por D-105** (Nueva partida borra solo su ranura)                                                                                                                                                                                                                                                                                                                                                                                                 | Christian Coronel Palma |
| 2026-10-07 | D-33 Estructura de pisos           | 3 pisos; cada uno con 8–12 salas procedurales, 2–3 salas clave secundarias sin guardado y 1 sala clave final (escalera o elevador); sala de The Mother en el piso 3, sin guardado. Modifica D-15. Motivo: Adaptación                                                                                                                                                                                                                                                                                                                                                                                                                                     | Christian Coronel Palma |
| 2026-10-07 | D-34 Configuraciones               | Completa: 8–12 procedurales + 2–3 secundarias + 1 final por piso (~45–60 min). Showcase: 6–8 procedurales + 2 secundarias + 1 final por piso (~30 min), con enemigos y recursos escalados en proporción. Motivo: Adaptación (misma estructura en ambas duraciones)                                                                                                                                                                                                                                                                                                                                                                                       | Christian Coronel Palma |
| 2026-10-07 | D-35 Contador de 5 muertes         | Solo lo reinicia llegar a la sala final del piso 2; las muertes del primer intento no cuentan; Reiniciar y salir con costo suman. Modifica D-08. Motivo: Vulnerabilidad Constante y Adaptación. **Modificada por D-42** (solo el nombre del final: "Asimilación"). **Precisada por D-56**                                                                                                                                                                                                                                                                                                                                                                | Christian Coronel Palma |
| 2026-10-07 | D-36 Llaves                        | 1 llave obligatoria por piso que abre la sala final y llaves opcionales para atajos o salas con recursos; objetos clave de 1 casilla que se consumen al usarse; la llave obligatoria siempre es alcanzable antes de la puerta de la sala final. Motivo: Adaptación y Decisiones con Consecuencias (la llave compite por espacio)                                                                                                                                                                                                                                                                                                                         | Christian Coronel Palma |
| 2026-10-07 | D-37 Valores iniciales             | Valores iniciales de munición, disparos, velocidad por salud, resistencia, máximos por casilla, enemigos por piso, atención, enemigo reciclado y gracia al cargar (ver [[02 - Mecánicas]] y [[06 - Economía y Recursos]]). Motivo: Vulnerabilidad Constante y Decisiones con Consecuencias (escasez medible). **Modificada por D-71** (punzón de descarga) **Modificada por D-97** (sin gracia al cargar)                                                                                                                                                                                                                                                | Christian Coronel Palma |
| 2026-10-07 | D-38 Tutorial                      | Integrado en las primeras salas del piso 1, fijas solo en el primer intento; desde el segundo intento esas salas son procedurales. Motivo: Adaptación (se aprende una vez y luego el escenario cambia). **Precisada por D-70**                                                                                                                                                                                                                                                                                                                                                                                                                           | Christian Coronel Palma |
| 2026-10-07 | D-39 Presentación                  | Cinemática completa de reconstrucción una sola vez; cinemática corta saltable en cada muerte o reinicio; transición de piso de mínimo 5 s sin pantalla de carga; menú principal Continuar, Nueva partida, Opciones, Créditos, Salir; créditos saltables tras verlos una vez. Motivo: Adaptación. **Precisada por D-91 y D-105**                                                                                                                                                                                                                                                                                                                                                          | Christian Coronel Palma |
| 2026-10-07 | D-40 Premisa                       | La esposa del protagonista está desaparecida, no muerta: "Un hombre busca la última pista que su esposa desaparecida le dejó, pero queda atrapado en un ciclo sin fin del que ahora debe salir."; qué le ocurrió y su relación con la instalación y The Mother siguen Por definir. Modifica D-13. Motivo: Adaptación (descubrimiento progresivo) y horror cósmico por información incompleta. **Precisada por D-49**                                                                                                                                                                                                                                     | Christian Coronel Palma |
| 2026-10-08 | D-41 Nombre                        | "Identity Dissolution" es el nombre del proyecto; "Heartbeat" se mantiene como nombre clave. Motivo: identidad del proyecto (sin pilar asociado)                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                         | Christian Coronel Palma |
| 2026-10-08 | D-42 Final de derrota              | El final de 5 muertes consecutivas se llama "Asimilación": el laboratorio termina de asimilar a Lukas y este se convierte en uno de los enemigos comunes. Modifica D-08 y D-35 (solo el nombre del final). Motivo: Adaptación                                                                                                                                                                                                                                                                                                                                                                                                                            | Christian Coronel Palma |
| 2026-10-08 | D-43 Representación del tema       | Solo presentación, sin efecto mecánico: cuerpo ajeno visible (una pieza ajena más del laboratorio en cada reconstrucción; 3–5 variantes del modelo, valor inicial, sujeto a balance; la mano del anillo puede acabar asimilada), código de sujeto que sube en cada reconstrucción (R-H-07, R-H-08…) y enemigos comunes como personas asimiladas. Se descarta la "memoria que se deshilacha" para pistas y reglas. Motivo: Adaptación                                                                                                                                                                                                                     | Christian Coronel Palma |
| 2026-10-08 | D-44 Tema y postura                | Nihilismo puro frente al horror cósmico; tema central: buscar sentido dentro de algo que no puede darlo. El anillo es el ancla del personaje, no del universo; la motivación del jugador es el misterio y el descubrimiento, no la esperanza. Motivo: Adaptación. **Precisada por D-133**                                                                                                                                                                                                                                                                                                                                                                                         | Christian Coronel Palma |
| 2026-10-08 | D-45 Memoria                       | Lukas olvida a Emilia en la primera reconstrucción (cinemática completa de D-39) y conserva el anillo; jugador y Lukas se informan juntos; documentos corruptos (distintos de las fallas de la máquina) que nunca ocultan información necesaria para avanzar y se completan en el desenlace; Lukas recupera primero el nombre de Emilia y después el suyo. Motivo: Adaptación. **Modificada por D-55**; **precisada por D-58**                                                                                                                                                                                                                           | Christian Coronel Palma |
| 2026-10-08 | D-46 Mundo                         | Distopía retrofuturista con estética de los 90 y tecnología de ~2080 gobernada por megacorporaciones; Vitalis Konzern (biotecnología y salud); el laboratorio es una instalación subterránea de Vitalis sellada tras el incidente (ubicación Por definir) y **es** la criatura lovecraftiana; la generación procedural es el laboratorio reacomodándose; el laboratorio tiene su propio latido; los 3 pisos son un descenso hacia el núcleo; The Mother envía reportes que nadie lee; Vitalis explotaba algo que no entendía y lo vendía como medicina de regeneración (no es un virus ni un arma biológica). Motivo: Adaptación. **Precisada por D-59** | Christian Coronel Palma |
| 2026-10-08 | D-47 The Mother                    | IA médica de Vitalis con el protocolo de preservación corrupto: reconstruye a quien conserva identidad; el laboratorio asimila. Lukas es un sujeto R-H. Justifica el reciclaje único por el último rastro de identidad. No puede explicar qué es la criatura. Motivo: Vulnerabilidad Constante y Adaptación **Precisada por D-99**                                                                                                                                                                                                                                                                                                                       | Christian Coronel Palma |
| 2026-10-08 | D-48 Enemigos y bestiario          | Los enemigos comunes son personas asimiladas (personal de Vitalis, sujetos de prueba); Lukas no es el primero ni es especial. El bestiario son informes clínicos de The Mother con código de sujeto, desbloqueados en el primer encuentro. Motivo: Adaptación. **Precisada por D-60**                                                                                                                                                                                                                                                                                                                                                                    | Christian Coronel Palma |
| 2026-10-08 | D-49 Emilia                        | Científica de Vitalis despedida antes del incidente que investigó a escondidas; su papel en el incidente nunca se confirma; reconstruida como R-H-01 a R-H-06; no fue asimilada; "estado desconocido"; destino indefinido para la post-demo. Precisa D-40. Motivo: Adaptación. **Modificada por D-57**                                                                                                                                                                                                                                                                                                                                                   | Christian Coronel Palma |
| 2026-10-08 | D-50 Lukas y la última pista       | Técnico de mantenimiento de Vitalis en otra instalación; entra con su acceso de técnico; es R-H-07 por estar en el expediente de Emilia; la última pista es un mensaje de audio en una tablet o teléfono corporativo (transcripción con sonido vocal y forma de onda), que se pierde en la primera reconstrucción y queda copiado, corrupto, en el terminal del brazo. Cómo muere al final de la introducción: Por definir. Motivo: Adaptación. **Modificada por D-53**                                                                                                                                                                                  | Christian Coronel Palma |
| 2026-10-08 | D-51 Finales                       | Asimilación, Relevo básico, Relevo completo y Ambiguo; Despertar en toda victoria; elección con la terminal central (`E`) y la salida, sin menú; "todos los documentos" = los 6 de Emilia en el terminal del brazo; sin Renuncia; gancho totalmente abierto. Precisa D-16. Motivo: Adaptación. **Modificada por D-55**; **precisada por D-61**                                                                                                                                                                                                                                                                                                           | Christian Coronel Palma |
| 2026-10-08 | D-52 Narrativa                     | Sin actuación de voz (sonidos más texto, subtítulos obligatorios); Lukas casi no habla y su sonido vocal se distorsiona con la degradación; "distorsión de voz" pasa a "distorsión del sonido vocal"; 6 documentos de Emilia, 2 por piso, en salas clave secundarias; documentos de Vitalis y The Mother en salas procedurales, sin contar para la elección final; idiomas: inglés principal, español secundario. Modifica D-08 y M-18 (texto). Motivo: Adaptación. **Precisada por D-58 y D-62**. **Precisada por D-79**                                                                                                                                | Christian Coronel Palma |
| 2026-10-15 | D-53 Introducción                  | Casa (cinemática), mensaje programado de Emilia al teléfono personal de Lukas (advertencia de no ir; termina con Emilia corriendo de algo), compuerta y elevador jugables, caída al piso que no existe, señuelo (Mudo) y ataque por la espalda sin mostrar (trigger y temporizador de respaldo); reconstrucción en imágenes fugaces; The Mother copia el teléfono al terminal como "efectos personales del sujeto". Modifica D-50. Motivo: Vulnerabilidad Constante y Adaptación. **Precisada por D-63**                                                                                                                                                 | Christian Coronel Palma |
| 2026-10-15 | D-54 Anillo                        | Tras la primera reconstrucción, Lukas encuentra un anillo en el suelo; nunca se sabe si es el suyo o el de Emilia. Motivo: Adaptación (disolución de identidad, D-44)                                                                                                                                                                                                                                                                                                                                                                                                                                                                                    | Christian Coronel Palma |
| 2026-10-15 | D-55 Memoria final                 | Los documentos y el mensaje se completan y Lukas recupera los nombres al caer The Mother solo con los 6 documentos de Emilia; sin ellos todo sigue corrupto y el Relevo básico ocurre sin nombres. Modifica D-45 y D-51. Motivo: Adaptación                                                                                                                                                                                                                                                                                                                                                                                                              | Christian Coronel Palma |
| 2026-10-15 | D-56 Hallazgo del piso 2           | Cinemática corta, una vez: la foto familiar de la casa, imposible y sin explicar, con la cara de Emilia borrosa; provoca obstinación (no esperanza) y reinicia el contador; después el contador sigue contando en el piso 3 y en la sala de The Mother. Precisa D-35. Motivo: Vulnerabilidad Constante. **Precisada por D-67**                                                                                                                                                                                                                                                                                                                           | Christian Coronel Palma |
| 2026-10-15 | D-57 Emilia y el incidente         | El latido es un texto (inspiración: *The King in Yellow*); el incidente fue su transmisión por megafonía; Emilia oyó el primer acto, fue reconstruida como R-H-01, la despidieron y volvió por el segundo acto (R-H-02 a R-H-06); olvida poco a poco; brillante y fría; ficha física. Modifica D-49. Motivo: Adaptación. **Precisada por D-64 y D-68**                                                                                                                                                                                                                                                                                                   | Christian Coronel Palma |
| 2026-10-15 | D-58 Documentos                    | Estilo *Signalis*: glitch provocado por el laboratorio o el latido, alemán sin traducir, burocracia frente a diario, frase clave "Escucha el latido"; los documentos de Emilia no usan R-H; The Mother muestra R-H-01 a R-H-06 en el piso 2 con código, fecha y causa; una sala secundaria ya registrada da recursos. Precisa D-45 y D-52. Motivo: Adaptación. **Precisada por D-79**                                                                                                                                                                                                                                                                    | Christian Coronel Palma |
| 2026-10-15 | D-59 Lugares                       | Sektor V-7, 2083; estética cruda e industrial con experimentos inhumanos; pisos de mantenimiento, clínica ilegal y núcleo; fila de cápsulas; sala de The Mother = central de megafonía que se transforma por fase (transformadores como puntos débiles, sin carne); The Mother = maniquí robótico de enfermera en rieles. Precisa D-16 y D-46. Motivo: Vulnerabilidad Constante y Adaptación. **Modificada por D-65**                                                                                                                                                                                                                                    | Christian Coronel Palma |
| 2026-10-15 | D-60 Enemigos                      | Asimilados grotescos (terror físico y cósmico): el Alto, la Boca, el Rastrero y el Mudo, cada uno con su versión reciclada. Precisa D-48. Motivo: Vulnerabilidad Constante. **Precisada por D-64 y D-66**. **Precisada por D-76**                                                                                                                                                                                                                                                                                                                                                                                                                        | Christian Coronel Palma |
| 2026-10-15 | D-61 Finales                       | Relevo completo: Lukas oye el segundo acto (glitch ilegible para el jugador); Asimilación: imágenes deformadas, informe de The Mother y Lukas como señuelo en el piso que no existe; Ambiguo: los dos latidos se sincronizan, silencio y gancho; decisión final con presión sin muerte. Precisa D-42 y D-51. Motivo: Adaptación                                                                                                                                                                                                                                                                                                                          | Christian Coronel Palma |
| 2026-10-15 | D-62 Texto                         | Sin narrador; solo humor negro corporativo; textos redactados en inglés y nombres propios sin traducir; The Mother emite una línea de protocolo al inicio de cada fase del jefe; el texto de Lukas solo se degrada en la Asimilación. Precisa D-52. Motivo: Adaptación                                                                                                                                                                                                                                                                                                                                                                                   | Christian Coronel Palma |
| 2026-10-15 | D-63 Introducción jugable          | Lukas hackea la compuerta con su teléfono personal (reglas de M-13, sin terminal); HUD mínimo: moverse, mirar e interactuar, pausa sin "Reiniciar"; el elevador es cinemática; la muerte final está guionizada y no suma al contador ni a la degradación. Precisa D-53. Motivo: Adaptación                                                                                                                                                                                                                                                                                                                                                               | Christian Coronel Palma |
| 2026-10-15 | D-64 Latido                        | El latido de fondo es el primer acto y no asimila; crece con la degradación; el segundo acto solo suena completo en el incidente y en el Relevo completo, donde el terminal protege a Lukas; ningún enemigo emite el latido y la Boca usa un grito propio. Precisa D-57 y D-60. Motivo: Adaptación **Precisada por D-95**                                                                                                                                                                                                                                                                                                                                | Christian Coronel Palma |
| 2026-10-15 | D-65 The Mother                    | Archivo de muestras criogénicas en una grúa: el brazo recolector agarra a Lukas (M-11) en las 3 fases y puede matarlo; fase 2: cada enfoque de cámara libera un asimilado (máximo 2 activos) y niebla criogénica solo visual; fase 3: el núcleo criogénico expone los transformadores. Modifica D-59; precisa D-28. Motivo: Vulnerabilidad Constante. **Precisada por D-80 y D-98**                                                                                                                                                                                                                                                                                                     | Christian Coronel Palma |
| 2026-10-15 | D-66 Valores por tipo de enemigo   | Disparos: Alto 4, Boca 3, Rastrero 2, Mudo 3; golpes para matar: Alto 2, Boca 3, Rastrero 4, Mudo 2; agarran el Alto y el Mudo; el Mudo se activa a ~3 m o al ser iluminado; reciclados: 2 disparos (Rastrero 1). Precisa D-60 y D-37; modifica D-07 (2-4 golpes). Motivo: Vulnerabilidad Constante. **Precisada por D-76**                                                                                                                                                                                                                                                                                                                              | Christian Coronel Palma |
| 2026-10-15 | D-67 Hallazgo del piso 2           | La cinemática espera a que termine la caza, igual que el autoguardado, y no se puede saltar. Precisa D-56. Motivo: Adaptación                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                            | Christian Coronel Palma |
| 2026-10-15 | D-68 Despido de Emilia             | Vitalis no sabe que Emilia fue reconstruida porque nunca lee los reportes de The Mother; la considera una superviviente inestable y la despide para limpiar responsabilidades. Precisa D-57. Motivo: Adaptación                                                                                                                                                                                                                                                                                                                                                                                                                                          | Christian Coronel Palma |
| 2026-10-16 | D-69 Niveles y salas procedurales  | Cada piso es un nivel; la Introducción y la Sala de The Mother son niveles propios. Las salas procedurales combinan 6 arquetipos (pasillo, almacén, sala abierta, vigilancia, hackeo y vertical) con el tema del piso (mantenimiento, clínica ilegal y núcleo). Motivo: Adaptación                                                                                                                                                                                                                                                                                                                                                                       | Christian Coronel Palma |
| 2026-10-16 | D-70 Tutorial                      | 4 salas fijas en el primer intento: fila de cápsulas, depósito a oscuras (linterna, inventario, revólver, 1 cura y 1 blíster), túnel (correr) y sala de calderas con 1 Rastrero real (sigilo, empujón y disparo; cuenta en los enemigos del piso 1). Sus recursos cuentan en el presupuesto del piso 1. Precisa D-38. Motivo: Vulnerabilidad Constante                                                                                                                                                                                                                                                                                                   | Christian Coronel Palma |
| 2026-10-16 | D-71 Inventario                    | 5 tipos de objeto con casilla; revólver, linterna y anillo sin casilla; fusibles eliminados; linterna con barra recargable (~60 s); el punzón de descarga sustituye al objeto defensivo y al consumible de remate (un solo uso, 2 funciones). Modifica D-14 y D-37; precisa D-21. Motivo: Decisiones con Consecuencias                                                                                                                                                                                                                                                                                                                                   | Christian Coronel Palma |
| 2026-10-16 | D-72 Energía                       | ~1 de cada 3 pisos (1 y 2) empieza con la sala final sin energía; interruptor ruidoso en una sala clave secundaria; nunca en el tutorial ni en el piso 3; entre pisos solo se pasa por la sala final. Motivo: Adaptación                                                                                                                                                                                                                                                                                                                                                                                                                                 | Christian Coronel Palma |
| 2026-10-16 | D-73 Objetos confiscados           | Los "otros objetos clave" pasan a ser 12 objetos de narrativa escénica (personal, sujetos incluidos niños, incidente), 4 por piso, 1–2 por intento; se examinan y se registran en el terminal; no cuentan para los finales. Motivo: Adaptación                                                                                                                                                                                                                                                                                                                                                                                                           | Christian Coronel Palma |
| 2026-10-16 | D-74 Revólver de Kessler           | Revólver de 2080 con seguro biométrico y contador en el armazón; tambor de 6, recarga bala a bala, blísteres de 3, ~0.8 s entre disparos, dispersión que se cierra quieto, puntos débiles que interrumpen, ruido a ~20 m; The Mother lo devuelve descargado en cada reconstrucción. Motivo: Vulnerabilidad Constante **Precisada por D-104** (asistencia opcional)                                                                                                                                                                                                                                                                                                                                     | Christian Coronel Palma |
| 2026-10-16 | D-75 Kessler Sicherheit AG         | Corporación de seguridad aliada de Vitalis; su división Werkschutz tenía el contrato exclusivo de Sektor V-7 y vigilaba sin preguntar. Motivo: Adaptación                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                | Christian Coronel Palma |
| 2026-10-16 | D-76 Enemigos: velocidad y reparto | Patrulla / caza: Alto 1.2 / 3 m/s (vista ~15 m); Boca 1 / 2.5 m/s (ciega, oído ~20 m); Rastrero 1.8 / 4.2 m/s (~8 m); Mudo inmóvil y estallidos de 4.5 m/s. Lukas: 0.8 / 1.5 / 4 m/s. Piso 1: Alto y Rastrero; piso 2: + Boca; piso 3: + Mudo. Precisa D-60 y D-66. Motivo: Adaptación                                                                                                                                                                                                                                                                                                                                                                   | Christian Coronel Palma |
| 2026-10-16 | D-77 Sin NPC vivos                 | No hay NPC vivos ni diálogos; The Mother y Emilia solo aparecen como texto; los cadáveres de guardias y personal ya están asimilados. Motivo: Vulnerabilidad Constante                                                                                                                                                                                                                                                                                                                                                                                                                                                                                   | Christian Coronel Palma |
| 2026-10-16 | D-78 Misiones                      | 5 principales (Introducción, pisos 1 a 3, The Mother) y 2 secundarias persistentes (Expediente de Emilia, Objetos confiscados); sin desafíos. Sin lista de misiones: entorno, líneas de protocolo y contadores en el terminal. Motivo: Adaptación                                                                                                                                                                                                                                                                                                                                                                                                        | Christian Coronel Palma |
| 2026-10-16 | D-79 Documentos y recursos         | 12 documentos que no son de Emilia (Vitalis, Kessler, The Mother e incidente), 4 por piso, 1–2 por intento; la sala con el documento de Emilia ya registrado da 1 recurso extra del tipo más escaso; el mínimo garantizado nunca queda tras una llave opcional. Precisa D-52 y D-58. Motivo: Decisiones con Consecuencias                                                                                                                                                                                                                                                                                                                                | Christian Coronel Palma |
| 2026-10-16 | D-80 The Mother: valores           | Fase 3: 3 transformadores de 2 disparos; la sala da 8 balas. Fase 2: 4 asimilados en total (Altos o Rastreros), máximo 2 activos. Precisa D-28 y D-65. Motivo: Vulnerabilidad Constante                                                                                                                                                                                                                                                                                                                                                                                                                                                                  | Christian Coronel Palma |
| 2026-10-16 | D-81 Render                        | Low-poly con luz moderna: texturas de baja resolución con filtro point, iluminación dinámica, render interno ~540p reescalado y dithering suave; sin PSX puro. Motivo: Vulnerabilidad Constante (la linterna y la oscuridad siguen siendo mecánica)                                                                                                                                                                                                                                                                                                                                                                                                      | Christian Coronel Palma |
| 2026-10-16 | D-82 Paleta y luz                  | Base (~70%), carmesí (Vitalis, carne, peligro), teal (Lukas), morado (cósmico), verde (pantallas de la instalación), ámbar de sodio; luz por piso ámbar / blanco frío con sombras `#08241E` / carmesí; juego mayormente a oscuras por emergencia; el color nunca es la única señal. Motivo: Vulnerabilidad Constante                                                                                                                                                                                                                                                                                                                                     | Christian Coronel Palma |
| 2026-10-16 | D-83 Diseño de Lukas               | Las imágenes de referencia civil y R-H-07 son el diseño oficial; el anillo de la introducción va en la mano derecha (se corrige la imagen); tez clara (se corrige el texto). Motivo: identidad del personaje                                                                                                                                                                                                                                                                                                                                                                                                                                             | Christian Coronel Palma |
| 2026-10-16 | D-84 Interactuables                | Destello especular sutil solo bajo la linterna o una luz y aviso `E` a ~1.5 m; sin brillo propio a oscuras. Motivo: Vulnerabilidad Constante (buscar exige luz y su riesgo)                                                                                                                                                                                                                                                                                                                                                                                                                                                                              | Christian Coronel Palma |
| 2026-10-16 | D-85 Gore                          | Horror corporal sin desmembramiento: carne expuesta, sangre oscura e impactos; a los niños solo se alude con objetos; sin desnudez explícita. Motivo: tono                                                                                                                                                                                                                                                                                                                                                                                                                                                                                               | Christian Coronel Palma |
| 2026-10-16 | D-86 Interfaz diegética            | Sin HUD permanente; estados en el terminal con etiquetas claras; en pantalla solo retícula, aviso `E`, subtítulos, líneas de The Mother y autoguardado; terminal CRT teal monoespaciado. Motivo: Vulnerabilidad Constante                                                                                                                                                                                                                                                                                                                                                                                                                                | Christian Coronel Palma |
| 2026-10-16 | D-87 Música                        | Exploración sin música; temas en menú, cinemáticas, jefe (loop por fase) y finales; estilo crudo, oscuro e industrial; leitmotivs de Emilia, The Mother y el latido. Motivo: Adaptación (no tapa las señales)                                                                                                                                                                                                                                                                                                                                                                                                                                            | Christian Coronel Palma |
| 2026-10-16 | D-88 Animación                     | Blender propio con locomoción base de Mixamo retocada; IK, capas, ruido procedural y ragdoll con herramientas de Unity; esqueleto humanoide compartido. Motivo: producción                                                                                                                                                                                                                                                                                                                                                                                                                                                                               | Christian Coronel Palma |
| 2026-10-16 | D-89 Tutorial                      | Los controles se enseñan con carteles de seguridad de Vitalis en las 4 salas del tutorial, en inglés y con la tecla asignada; sin avisos en pantalla. Precisa D-70. Motivo: Vulnerabilidad Constante (interfaz diegética)                                                                                                                                                                                                                                                                                                                                                                                                                                | Christian Coronel Palma |
| 2026-10-16 | D-90 Título y menú                 | Pantalla de título negra con logotipo y latido; al pulsar una tecla, el menú principal aparece en un terminal de la instalación en la sala de la fila de cápsulas, junto a la cápsula de Lukas. Motivo: identidad                                                                                                                                                                                                                                                                                                                                                                                                                                        | Christian Coronel Palma |
| 2026-10-16 | D-91 Cinemáticas saltables         | La corta de reconstrucción siempre; las narrativas (completa, Asimilación, victoria, Relevos, Ambiguo y gancho) solo tras verlas una vez, manteniendo `Espacio` ~1 s; hallazgo y transición nunca. Motivo: rejugabilidad                                                                                                                                                                                                                                                                                                                                                                                                                                 | Christian Coronel Palma |
| 2026-10-16 | D-92 Pausa en la decisión final    | La pausa está disponible sin "Reiniciar"; autoguardado al empezar la decisión; salir no tiene costo y Continuar vuelve al inicio de la decisión. Precisa D-30. Motivo: seguridad de la elección                                                                                                                                                                                                                                                                                                                                                                                                                                                          | Christian Coronel Palma |
| 2026-10-16 | D-93 Ciclo nuevo tras un final     | Continuar tras los créditos inicia un ciclo nuevo desde la cápsula con el terminal conservado (documentos completos incluidos); contador y degradación a 0; el código de sujeto sigue. Modifica D-29. Motivo: Adaptación                                                                                                                                                                                                                                                                                                                                                                                                                                 | Christian Coronel Palma |
| 2026-10-16 | D-94 Código y degradación          | Salir con calma con costo sube el código de sujeto; la degradación suma en el primer intento (D-35 solo exime al contador). Motivo: Adaptación                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                           | Christian Coronel Palma |
| 2026-10-16 | D-95 Latido del laboratorio        | Suena de fondo en la nota de voz de Emilia (segunda razón de Lukas para ir, sin saberlo); desde la primera reconstrucción, ambiente en todas las salas; crece con cada muerte y con la salud en Crítico. Precisa D-64. Motivo: Adaptación                                                                                                                                                                                                                                                                                                                                                                                                                | Christian Coronel Palma |
| 2026-10-16 | D-96 Guardado y salida             | Cierre de tránsito (`LOCKDOWN`) durante la caza y autoguardado al terminar; elimina D-26. Cierre forzado sin cobro; alerta = caza; Continuar en gris sin partida; carga como arranque del terminal; confirmación en todas las acciones de salida. Motivo: Vulnerabilidad Constante                                                                                                                                                                                                                                                                                                                                                                       | Christian Coronel Palma |
| 2026-10-16 | D-97 Salas de aparición            | La sala de aparición empieza sin enemigos; la fila de cápsulas es el único refugio; las salas clave se pueden sellar con el hackeo hasta que Lukas cruza o un enemigo rompe el sellado (~30 s); elimina la gracia al cargar. Modifica D-30 y D-37. Motivo: Vulnerabilidad Constante                                                                                                                                                                                                                                                                                                                                                                              | Christian Coronel Palma |
| 2026-10-16 | D-98 Jefe y atención: valores      | 3 cámaras hostiles (~20°/s, cono ~12 m, ~1 s de enfoque); grúa a ~2.5 m/s, brazo de ~3 m con aviso de ~1 s; atención de The Mother única por piso, enemigos cercanos = sala actual y adyacentes. Precisa D-24 y D-65. Motivo: Vulnerabilidad Constante                                                                                                                                                                                                                                                                                                                                                                                                   | Christian Coronel Palma |
| 2026-10-16 | D-99 Reciclaje por el laboratorio  | El laboratorio, no The Mother, levanta todos los cuerpos no rematados una vez; aviso orgánico de ~3 s a los ~40 s; remate con punzón ~1.5 s. Modifica D-20 y precisa D-47. Motivo: coherencia del lore                                                                                                                                                                                                                                                                                                                                                                                                                                                           | Christian Coronel Palma |
| 2026-10-16 | D-100 Distancia a la sala final    | ≥ 4 salas en la ruta más corta (≥ 3 en showcase). Motivo: Adaptación                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                     | Christian Coronel Palma |
| 2026-10-16 | D-101 Semilla por piso | Cada piso se genera con una semilla nueva cada vez que se entra, también al continuar desde un autoguardado (solo se conserva la sala final guardada); no se puede volver a un piso anterior. Precisa D-30. Motivo: Adaptación | Christian Coronel Palma |
| 2026-10-16 | D-102 Subtítulos de sonido | Opción desactivada por defecto: describe los sonidos importantes con dirección aproximada, sin distancia ni posición en pantalla; respeta D-02. Motivo: accesibilidad | Christian Coronel Palma |
| 2026-10-16 | D-103 Modos | Modo Supervivencia y Modo Historia (~50% menos daño, detección más lenta, +1 al mínimo de cada recurso por piso; mismo ciclo y finales, incluida la Asimilación); se elige al crear la partida y queda fijo. Motivo: accesibilidad | Christian Coronel Palma |
| 2026-10-16 | D-104 Asistencia de apuntado | Opcional; desactivada por defecto con teclado y ratón, activada por defecto con mando. Precisa D-74. Motivo: accesibilidad | Christian Coronel Palma |
| 2026-10-16 | D-105 Ranuras | 3 ranuras; Continuar usa la última; Nueva partida y Cargar partida eligen ranura; configuración, brillo y cinemáticas vistas son globales. Precisa D-32. Motivo: experiencia | Christian Coronel Palma |
| 2026-10-16 | D-106 Brillo y efectos | Calibración de brillo en el primer inicio; opciones para reducir efectos CRT y glitch, destellos y movimiento; el pixelado no se desactiva. Motivo: accesibilidad | Christian Coronel Palma |
| 2026-10-21 | D-107 Motor | Unity 6.6 (`6000.6.6f1`, *Supported release*) con URP *Forward+* sin luz horneada; ~540p con escalado *Nearest-Neighbor*; solo paquetes oficiales (Input System, Cinemachine 3, AI Navigation, Timeline, Animation Rigging, Localization, TextMeshPro, Test Framework). Motivo: luz dinámica en salas procedurales | Christian Coronel Palma |
| 2026-10-21 | D-108 Arquitectura | Escenas `Bootstrap`, `MainMenu`, `Intro`, `Floor` y `Cinematics`; una `GameStateMachine` con los estados de Reglas y Estados; eventos C# en `GameEvents` entre sistemas; balance en ScriptableObjects; lógica pura probada en EditMode. Motivo: bajo acoplamiento y pruebas | Christian Coronel Palma |
| 2026-10-21 | D-109 Formato de guardado | JSON con `JsonUtility` en `persistentDataPath`: `global.json` y `slot_0..2.json`; escritura atómica con `.tmp` y `.bak`; `saveVersion` y migraciones; sin cifrado. Precisa D-30 y D-105. Motivo: ninguna ranura se corrompe | Christian Coronel Palma |
| 2026-10-21 | D-110 Generación técnica | Grafo abstracto validado (1 000 semillas en pruebas), reintentos y piso de respaldo; prefabs de sala; NavMesh por piso en tiempo de ejecución durante la transición; solo la sala actual y las adyacentes activas. Precisa S-06. Motivo: ningún piso imposible | Christian Coronel Palma |
| 2026-10-21 | D-111 Alcance y plazo | Entrega el 2026-11-30 con la showcase como objetivo; MVP 2026-11-09, vertical slice 2026-11-16, congelación 2026-11-23; orden de recorte M-17, M-18, completa, contexto, cinemáticas, Modo Historia, español; sin mando. Motivo: plazo de 6 semanas **Modificada por D-120 y D-134** | Christian Coronel Palma |
| 2026-10-21 | D-112 Requisitos | Mínimo Windows 10 64 bits, 4 núcleos, GTX 950, 8 GB, 1280 × 720 a 30 FPS; recomendado GTX 1060, 16 GB, 1080p a 60 FPS; generación + NavMesh ≤ 5 s; build ≤ 2 GB; presupuestos de luces, enemigos y triángulos. Provisionales hasta el perfilado. Motivo: rendimiento | Christian Coronel Palma |
| 2026-10-21 | D-113 Calidad | Definición de terminado con revisión por otra persona y pruebas EditMode de la lógica pura; backlog y bugs en GitHub Issues con severidad; sin críticos abiertos al cerrar un hito ni críticos o altos en la entrega. Motivo: calidad con un equipo de 5 **Modificada por D-135** | Christian Coronel Palma |
| 2026-10-21 | D-114 Nodos de la fase 1 | The Mother: 3 nodos de hackeo con secuencias de terminal principal en la fase 1. Precisa M-19. Motivo: 1-2 min por fase | Christian Coronel Palma |
| 2026-10-21 | D-115 Fin de la fase 2 | Termina al eliminar a los 4 asimilados (2 Altos y 2 Rastreros) y llegar al núcleo; 12 balas sueltas en los puntos ciegos para la fase 2 además de las 8 de la fase 3. Precisa D-80. Motivo: Vulnerabilidad Constante | Christian Coronel Palma |
| 2026-10-21 | D-116 Daño en Modo Historia | ~50% menos daño: cada enemigo necesita el doble de golpes para matar a Lukas; se elimina "aguanta un golpe más". Precisa D-103. Motivo: accesibilidad | Christian Coronel Palma |
| 2026-10-21 | D-117 Asimilados pendientes en la fase 2 | Si Lukas llega al núcleo con asimilados sin liberar, salen todos los que falten (máximo 2 activos); esquivar las cámaras nunca bloquea la fase. Precisa D-115. Motivo: evitar un bloqueo | Christian Coronel Palma |
| 2026-10-21 | D-118 Tope de enemigos por capacidad | El número de enemigos del piso nunca supera 2 por sala ocupada; con pocas salas queda por debajo del rango. Precisa D-37. Motivo: el piso 3 no cabía con 2 por sala y ~1/3 vacías | Christian Coronel Palma |
| 2026-10-21 | D-119 Efecto de la cura | Salud oculta en 3 tramos; la cura sube al tope del estado siguiente y, en Bien con daño, a la salud completa; el agarre sin punzón baja al tope del estado inferior. Precisa M-05. Motivo: los golpes de 2-4 no encajan con 3 estados | Christian Coronel Palma |
| 2026-10-21 | D-120 Mando básico | La demo incluye un mapa fijo de mando tipo Xbox, sin reasignación; reasignar el mando es post-demo. Modifica el alcance de D-111. Motivo: Público y Plataforma y D-104 lo daban por incluido | Christian Coronel Palma |
| 2026-10-21 | D-121 Documentos de Emilia por piso | Los documentos 1–2 van en el piso 1, 3–4 en el 2 y 5–6 en el 3; máximo 1 por sala secundaria. Precisa D-52. Motivo: arco narrativo por actos | Christian Coronel Palma |
| 2026-10-21 | D-122 Blísteres de 4 | Cada blíster trae 4 balas; presupuestos redondeados a múltiplos de 4 (completa 12-16, 8-12, 4-8; showcase 8-12, 4-8, 4). Modifica D-74. Motivo: rangos del showcase que no cuadraban con blísteres de 3 | Christian Coronel Palma |
| 2026-10-21 | D-123 Regeneración de la resistencia | Tras 3 s sin gastarla, se llena de 0 a 100% en ~4 s (25% por segundo). Precisa M-06. Motivo: el agotamiento castiga con la espera, no con la recarga | Christian Coronel Palma |
| 2026-10-21 | D-124 Carga de la linterna | ~120 s de uso continuo; se llena en ~30 s tras ~3 s apagada; al agotarse no se puede encender durante ~10 s. Modifica D-71. Motivo: la linterna es la herramienta principal en pisos a oscuras | Christian Coronel Palma |
| 2026-10-21 | D-125 Línea temporal | Incidente en 2081; primera visita de Emilia en octubre de 2082; Lukas recibe la nota de voz a principios de diciembre de 2083. Precisa D-57 y D-59. Motivo: cierra la fecha del incidente | Christian Coronel Palma |
| 2026-10-21 | D-126 Tipografías | VT323 para terminal y datos, IBM Plex Sans para títulos y menús (OFL); opción de accesibilidad con OpenDyslexic. Motivo: estética CRT a 540p y legibilidad | Christian Coronel Palma |
| 2026-10-21 | D-127 Regla de corrupción | Sale corrupto todo pasaje que nombra a Lukas, se dirige a él o habla de su vida con Emilia, en cualquier documento y en la nota de voz; el resto es legible. Precisa D-45 y D-58. Motivo: corrupción sistémica, fácil de marcar en el texto | Christian Coronel Palma |
| 2026-10-21 | D-128 Investigación de Emilia | Emilia transcribe el latido como una partitura (actos, silencios, retornos) y deduce dónde suena el segundo acto; sus notas se vuelven obsesivas. Precisa D-57. Motivo: concreta la investigación sin explicar la criatura (D-16) | Christian Coronel Palma |
| 2026-10-21 | D-129 Diseño de The Mother | Estantería-catedral de viales con escarcha y luz carmesí colgada de la grúa; pinza hidráulica como brazo; núcleo de vidrio esmerilado con los 3 transformadores; sin cara, mira con las cámaras. Precisa D-65. Motivo: solo máquina, sin carne | Christian Coronel Palma |
| 2026-10-21 | D-130 Aspecto de los reciclados | Sin rasgos del laboratorio. Alto: vértebra metálica y extensiones en los codos, sin máscara, restos del Werkschutz; Boca: sin metal, demacrada, boca que se abre de abajo hacia arriba; Rastrero: araña orgánica con vértebra metálica y pinzas fundidas en la cara; Mudo: inspirado en las enfermeras de Silent Hill, vendas, placa metálica en la cara y espasmos. Precisa D-60. Motivo: lectura clara del enemigo reciclado | Christian Coronel Palma |
| 2026-10-21 | D-131 Recursos del Modo Historia | +1 al mínimo de cada recurso por piso; si supera al máximo, el máximo sube igual; en munición +1 es 1 blíster; las llaves no cambian. Precisa D-103. Motivo: el mínimo superaba al máximo en el showcase | Christian Coronel Palma |
| 2026-10-21 | D-132 Avisos según el dispositivo | Carteles del tutorial y aviso de interacción muestran la tecla o el botón del último dispositivo usado y cambian al vuelo. Precisa D-89 y D-120. Motivo: mando básico en la demo | Christian Coronel Palma |
| 2026-10-22 | D-133 Tema central | Redacción oficial del tema central: *Identity Dissolution trata sobre buscar sentido dentro de algo que no puede darlo. Lukas lucha por encontrar y recordar a quien ama, pero se encuentra enfrentando las amenazas de un lugar que le frena cada avance.* Precisa D-44 (la postura no cambia). Motivo: identidad del proyecto | Christian Coronel Palma |
| 2026-10-22 | D-134 Calendario y entrega | GDD del 2026-09-26 al 2026-10-22; desarrollo desde el 2026-10-23; entrega el 2026-11-20; H1 10-26, H2 11-01, H3 MVP 11-08, H4 vertical slice 11-11, H5 congelación 11-15, pruebas 11-16 a 11-19; recortes 8–10 (introducción, IA reutilizada, fase 2 sin cámaras). Modifica D-111. Motivo: fecha real de entrega | Christian Coronel Palma |
| 2026-10-22 | D-135 Equipo y seguimiento | Roles con nombres y una sola persona programando; backlog, bugs y cambios en Trello; *pull request* probado por otra persona sin revisión de código; telemetría local solo en *playtests*. Modifica D-113. Motivo: proceso ajustado al equipo real | Christian Coronel Palma |
| 2026-10-29 | D-136 Tagline | Tagline oficial: *Die. Rebuild. Forget.* Resume el ciclo de muerte y reconstrucción (M-16) y el olvido (D-45). Motivo: identidad del proyecto | Christian Coronel Palma |
| 2026-10-29 | D-137 Interfaz visual | Repartida: Victor Gabriel Alvarado Chay coordina y escribe los contenidos (terminal, carteles, menús); Bryan Iván Navarrete Canché hace el aspecto CRT, los iconos y el post-proceso; Christian Coronel Palma, la lógica e integración. Precisa D-135. Motivo: la interfaz visual no tenía responsable | Christian Coronel Palma |
| 2026-10-29 | D-138 Clasificación objetivo | PEGI 16 / ESRB M, acorde con D-85 y con el público de 20–30 años. Motivo: Público y Plataforma no definía clasificación | Christian Coronel Palma |
| 2026-10-29 | D-139 Monetización | Demo académica gratuita, sin venta, compras dentro del juego ni anuncios; sin modelo comercial para una versión final. Motivo: confirma Público y Plataforma | Christian Coronel Palma |
| 2026-10-29 | D-140 Prototipo de mecánicas | La rama `feature/heartbeatPrueba` (2026-10-26) es un prototipo desechable y no se fusiona; se reutilizan partes de su código adaptadas a las convenciones del GDD; las fechas de D-134 no cambian. Motivo: acelerar H1 y H2 sin heredar una estructura distinta (`Unity/`, `Assets/Scripts/`) | Christian Coronel Palma |


> **Navegación:** [[00 - Índice]] · ← [[02 - Referencias e Inspiración]] · [[04 - Historial de Versiones]] →
