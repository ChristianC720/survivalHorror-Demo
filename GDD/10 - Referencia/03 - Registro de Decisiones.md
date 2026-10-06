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
| 2026-10-05 | D-08 Final secundario | 5 muertes consecutivas sin llegar a una sala clave activan el final "Disolución de identidad"; degradación estética acumulativa por muerte | Christian Coronel Palma |
| 2026-10-05 | D-09 Fallas de la máquina | The Mother parchea cada falla tras 1-2 usos o al morir; se descubren fallas nuevas en otros intentos | Christian Coronel Palma |
| 2026-10-05 | D-10 Pilares | El Pilar 3 se renombra a "Decisiones con Consecuencias" | Christian Coronel Palma |
| 2026-10-05 | D-11 Estructura | Salas clave fijas + salas procedurales (reemplaza la etiqueta "Lineal") | Christian Coronel Palma |
| 2026-10-05 | D-12 Permadeath | Permadeath individual del clon; los datos persisten. **Modificada por D-18** | Christian Coronel Palma |
| 2026-10-05 | D-13 Premisa | Premisa oficial del hombre que busca la última pista de su esposa fallecida | Christian Coronel Palma |
| 2026-10-05 | D-14 Agarre | La liberación de agarre se reemplaza por un objeto defensivo consumible | Christian Coronel Palma |
| 2026-10-05 | D-15 Exploración vertical | Pisos conectados por escaleras o elevadores ubicados solo en salas clave | Christian Coronel Palma |
| 2026-10-05 | D-16 Jefe final | The Mother como jefe híbrido de 3 fases; derrotarla es la condición de victoria | Christian Coronel Palma |
| 2026-10-06 | D-17 Recursos | Presupuesto garantizado por piso (mínimo y máximo de cada recurso); el generador decide la ubicación | Christian Coronel Palma |
| 2026-10-06 | D-18 Muerte | El personaje se reconstruye, no se clona: siempre es el mismo hombre, recompuesto con piezas ajenas; se pierde todo el inventario. Modifica D-12 | Christian Coronel Palma |
| 2026-10-06 | D-19 Etiquetas | Se elimina la etiqueta "Permadeath" y se usa "Ciclo de muerte y reconstrucción" | Christian Coronel Palma |
| 2026-10-06 | D-20 Enemigos | Los enemigos eliminados siguen muertos durante el intento, pero The Mother puede reciclar cada uno una sola vez | Christian Coronel Palma |
| 2026-10-06 | D-21 Remate | Dos formas de evitar el reciclaje: golpe en el suelo o consumible de remate | Christian Coronel Palma |
| 2026-10-06 | D-22 Guardado | Suspensión al salir y reanudación en el mismo punto, sin guardado manual; el terminal y las fallas se autoguardan al cambiar | Christian Coronel Palma |


> **Navegación:** [[00 - Índice]] · ← [[02 - Referencias e Inspiración]] · [[04 - Historial de Versiones]] →
