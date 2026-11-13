# Mecánicas

## 1. Descripción de la sección

Una mecánica es una regla o interacción mediante la cual el jugador puede actuar dentro del sistema del juego.

Documenta cada mecánica con suficiente detalle para que otro integrante pueda implementarla.

## 2. Mecánicas principales

Las veinte mecánicas de la demo están implementadas. Las ocho principales sostienen el bucle de juego y las doce secundarias lo complementan. Los tiempos, distancias, porcentajes y cantidades de este documento son valores iniciales sujetos a balance. Los sistemas que agrupan estas mecánicas están en [[03 - Sistemas]], y la obtención y pérdida de recursos en [[06 - Economía y Recursos]]. La prioridad, los pilares y los riesgos de cada mecánica están en la sección 4.

### 2.1 Cámara sobre el hombro y apuntado

**Propósito:** Mantener al jugador cerca de Lukas para generar claustrofobia y enfoque táctico.

**Activación:** Es la vista predeterminada. Mantener el botón de apuntar activa el apuntado.

**Reglas:**

- La vista sobre el hombro es la cámara de todo el juego. Su campo de visión es estrecho y deja fuera la periferia y la espalda de Lukas.
- Al apuntar, la cámara se acerca al hombro y aparece la retícula, un punto pequeño cuya dispersión se cierra en ~1 s si Lukas está quieto. La linterna concentra el haz y la velocidad baja un 30%.
- Mientras apunta, Lukas no puede correr.
- Lukas siempre mira hacia donde apunta la cámara, también sin apuntar. Al moverse camina de lado o de espaldas respecto a la vista.
- La vista isométrica no sirve para explorar. Solo se obtiene a través de la vista de vigilancia.
- Las transiciones de cámara son rápidas y suaves para evitar mareo y confusión.

| Estado de cámara | Se entra al | Se sale al |
| --- | --- | --- |
| Sobre el hombro | Empezar, soltar el apuntado o salir de la vigilancia | Apuntar, activar la vigilancia o ser detectado |
| Apuntando | Mantener el apuntado | Soltar el apuntado |
| Vigilancia | Activarla en una sala con cámara y sin enemigos en caza | Salir con la misma tecla, correr, apuntar, empujar o recibir daño |
| Vigilancia degradada | Pasar un enemigo cercano a búsqueda | No quedar enemigos en búsqueda, o por las mismas salidas de la vigilancia |
| Sobre el hombro bloqueada | Entrar un enemigo cercano en caza | Perder la caza, eliminar a los perseguidores o cruzar una puerta |

**Entrada:** Mouse para mirar y `Clic derecho` mantenido para apuntar. Ver [[04 - Controles]].

**Estado inicial:** Vista sobre el hombro sin retícula.

**Estado final:** Con retícula mientras se apunta y sin ella al soltar.

**Recursos utilizados:** Ninguno.

**Recompensa:** Acertar en los puntos débiles y ver mejor en pasillos oscuros.

**Penalización:** Lukas no ve su espalda ni sus flancos y se mueve más lento al apuntar.

**Interacciones con otras mecánicas:** Vista de vigilancia, Bloqueo de vigilancia, Combate a distancia, Resistencia y empujón y Linterna.

**Casos límite:**

- Pegado a una pared, la cámara acorta la distancia sin atravesar la geometría.
- Un golpe hace vibrar la cámara y solo interrumpe el apuntado si derriba a Lukas.

### 2.2 Vista de vigilancia

**Propósito:** Dar una lectura táctica de la sala a cambio de exposición. El jugador ve con los ojos de la máquina.

**Activación:** Desde el terminal del brazo, en una sala con cámara activa y sin enemigos en caza.

**Reglas:**

- Solo existe en salas con cámara activa. Tienen cámara todas las salas clave, que son las secundarias, las finales y la sala del jefe, y ~1 de cada 3 salas procedurales. En la sala del jefe, las cámaras se vuelven hostiles durante la fase 2.
- La imagen es isométrica, con ruido CRT y ~0,3 s de retraso.
- Solo muestra lo que cubre la cámara de la sala y un piso a la vez.
- Mientras mira, Lukas solo puede quedarse quieto o caminar muy lento. Correr, apuntar o empujar devuelven la vista sobre el hombro.
- No hay retícula ni acciones de combate.
- La vista no indica que la sala sea segura. Las amenazas siguen activas mientras se usa.
- Se corta al recibir daño o cuando un enemigo entra en caza dentro del alcance del bloqueo. En esta mecánica, *en alerta* significa en caza.
- La calidad de la señal depende del estado de alerta más alto entre los enemigos de la sala de Lukas y de las salas conectadas a ella.

| Estado de alerta | Señal |
| --- | --- |
| Sin alerta o sospecha | Normal |
| Búsqueda | Degradada, con más ruido CRT y el doble de retraso, ~0,6 s frente a ~0,3 s |
| Caza | Se corta y queda bloqueada |

- **Atención de The Mother.** Usar la vista llena un medidor oculto, sin barra en pantalla. Se llena en ~5 s de uso y se vacía en ~15 s sin usarla. Hay un medidor por piso y no se vacía al cambiar de sala.
- Al llenarse el medidor, los enemigos cercanos pasan ~2 s en sospecha y luego buscan la posición de Lukas. Los enemigos cercanos son los de la sala de Lukas y los de las salas conectadas.
- El jugador nota el medidor porque el ruido CRT de la señal base sube poco a poco, sin cambiar el retraso.

![Vista de vigilancia degradada por un enemigo en búsqueda|520](../Media/Capturas/VigilanciaDegradada.png)

**Entrada:** `Q` activa y sale. Ver [[04 - Controles]].

**Estado inicial:** Vista sobre el hombro en una sala con cámara activa.

**Estado final:** Vista sobre el hombro, por salida voluntaria o por corte de la señal.

**Recursos utilizados:** Tiempo, exposición y cámaras de la sala.

**Recompensa:** Entender la sala, ubicar enemigos y objetos de forma aproximada y planear rutas.

**Penalización:** Lukas queda casi inmóvil, con información parcial y tardía. El uso prolongado atrae a los enemigos cercanos.

**Interacciones con otras mecánicas:** Cámara sobre el hombro, Bloqueo de vigilancia, Sigilo y ruido, Señales diegéticas, Exploración vertical, Fallas de la máquina y Jefe final.

**Casos límite:**

- Si un enemigo entra en caza en el mismo instante de la activación, esta se cancela.
- Si el medidor se llena con los enemigos cercanos ya en búsqueda o caza, su alerta no baja.
- Cruzar caminando a una sala sin cámara corta la señal.

### 2.3 Bloqueo de vigilancia por detección

**Propósito:** Quitar la información amplia justo cuando descubren al jugador y obligarlo a reaccionar desde la vista sobre el hombro.

**Activación:** Automática e inmediata cuando un enemigo entra en caza.

**Reglas:**

- Si la vista de vigilancia está activa, se corta de inmediato y la cámara queda sobre el hombro.
- La vista no se puede activar mientras haya al menos un enemigo en caza en la sala de Lukas o en una sala conectada. Una caza en otra parte del piso no la afecta.
- Los enemigos en sospecha o búsqueda no bloquean la vista. En búsqueda la señal llega degradada.
- La vista vuelve a estar disponible cuando ocurre una de estas tres cosas:
    1. Lukas elimina a todos los enemigos que lo detectaron.
    2. Lukas rompe la línea de visión el tiempo suficiente para que salgan de la caza.
    3. Lukas cruza una puerta y deja al perseguidor fuera de su sala y de las salas conectadas.

**Entrada:** No tiene.

**Estado inicial:** Cualquier vista, sin enemigos en caza.

**Estado final:** Vista sobre el hombro con la vigilancia bloqueada.

**Recursos utilizados:** Eventos de la IA enemiga.

**Recompensa:** Quien mantiene el sigilo conserva la información de las cámaras.

**Penalización:** Perder el sigilo deja a Lukas sin información mientras lo acorralan.

**Interacciones con otras mecánicas:** Vista de vigilancia, Sigilo y ruido, Señales diegéticas, Punzón de descarga y Escape de combate.

**Casos límite:**

- Un enemigo que detecta a Lukas desde otra sala solo activa el bloqueo si tiene ruta para perseguirlo.
- Liberarse de un agarre con el punzón no levanta el bloqueo si el enemigo sigue en caza tras el aturdimiento.

### 2.4 Combate a distancia

**Propósito:** Dar una herramienta ofensiva real que no elimine el peligro.

**Activación:** Disparar mientras se apunta.

**Reglas:**

- **Arma.** La demo tiene una sola arma, el revólver de servicio del Werkschutz de Kessler Sicherheit AG. Es un diseño de gama alta de 2080, con armazón de cerámica y polímero y el cañón alineado con la recámara inferior.
- Su seguro biométrico reconoce a Lukas como activo de Vitalis gracias al terminal del brazo. Un contador de balas en el armazón reemplaza al indicador de munición del HUD. El revólver no ocupa casilla.
- **Obtención.** La primera vez, Lukas lo encuentra en el cadáver asimilado de un guardia de Kessler durante el tutorial. Desde entonces, The Mother lo deja descargado en una bandeja de efectos personales junto a la cápsula en cada reconstrucción, al lado de 6 balas. Lo hace aunque Lukas haya muerto antes de recogerlo.

| Parámetro | Valor |
| --- | --- |
| Tambor | 6 balas |
| Munición por casilla | 12 balas |
| Munición por recogida | Blíster de 4 cartuchos, o de 6 en el Modo Historia |
| Recarga | Bala a bala, ~0,7 s por bala |
| Cadencia | ~0,8 s entre disparos, por el retroceso |
| Disparos para eliminar | Alto 4, Boca 3, Mudo 3 y Rastrero 2 |
| Disparos para eliminar a un reciclado | 2, o 1 si es un Rastrero |
| Radio de alerta del disparo | ~20 m |

- La recarga se puede cancelar en cualquier momento y conserva las balas ya cargadas. Mientras recarga, Lukas no dispara.
- La dispersión se cierra en ~1 s con Lukas quieto. Moverse, correr o recibir un golpe la abre.
- No hay autoapuntado. La asistencia de apuntado es una opción de accesibilidad, desactivada con teclado y mouse y activada con mando.
- Cada tipo de enemigo tiene un punto débil. Acertarle interrumpe el ataque o el agarre en curso, pero no reduce los disparos necesarios. Ver [[03 - Enemigos y NPC]].
- Disparar no gasta resistencia.
- Cada disparo alerta a los enemigos en ~20 m y llena un poco la atención de The Mother.
- Los enemigos son lentos, pero letales. Enfrentar a más de dos a la vez es casi una muerte segura.

**Entrada:** `Clic izquierdo` apuntando dispara y `R` recarga.

**Estado inicial:** Revólver con la munición disponible, descargado tras cada reconstrucción.

**Estado final:** Menos munición y un enemigo herido, aturdido o eliminado.

**Recursos utilizados:** Munición y casillas del inventario.

**Recompensa:** Eliminar o frenar una amenaza que bloquea el avance.

**Penalización:** Gasta un recurso escaso y el ruido atrae a otros enemigos.

**Interacciones con otras mecánicas:** Cámara sobre el hombro, Resistencia y empujón, Inventario, Sigilo y ruido, Linterna, Jefe final y Reciclaje de enemigos.

**Casos límite:**

- Con el tambor vacío, disparar suena a chasquido y no inicia la recarga.
- Un golpe cancela la recarga y conserva las balas ya cargadas.

### 2.5 Salud y estados

**Propósito:** Dar al daño consecuencias duraderas y fáciles de leer.

**Activación:** Automática al recibir daño o al usar una cura.

**Reglas:**

- La salud no se regenera y tiene tres estados visibles, que son Bien, Herido y Crítico. Bien no penaliza. Herido reduce la velocidad un 15% y Crítico un 30%.
- Internamente, la salud es un valor oculto dividido en tres tramos iguales.
- Una cura sube al tope del estado siguiente, de Crítico a Herido o de Herido a Bien. En Bien con daño devuelve la salud completa.
- Un agarre sin punzón baja al tope del estado inferior.
- Las curas escasean, ocupan una casilla y tardan lo mismo que cualquier acción del inventario.
- Lukas comunica su estado con la postura, la animación y el terminal del brazo.

| Enemigo | Golpes para matar a Lukas |
| --- | ---: |
| Alto | 2 |
| Mudo | 2 |
| Boca | 3 |
| Rastrero | 4 |

**Entrada:** No tiene. Las curas se usan desde el inventario.

**Estado inicial:** Bien.

**Estado final:** Bien, Herido, Crítico o muerte, que inicia la reconstrucción.

**Recursos utilizados:** Curas.

**Recompensa:** Subir un estado de salud con una cura.

**Penalización:** Más lentitud y menos margen de error en Herido y Crítico.

**Interacciones con otras mecánicas:** Combate a distancia, Resistencia y empujón, Inventario, Punzón de descarga y Reaparición por reconstrucción.

**Casos límite:**

- Recibir daño durante la cura cancela la animación sin gastar la cura.
- En Bien sin daño no se puede usar una cura.

### 2.6 Resistencia y empujón

**Propósito:** Obligar a medir el esfuerzo físico de Lukas.

**Activación:** Correr al mantener el botón de correr en movimiento. Empujar al pulsar ataque sin apuntar.

**Reglas:**

| Movimiento | Velocidad |
| --- | --- |
| Agachado | ~0,8 m/s |
| Caminando | ~1,5 m/s |
| Corriendo | ~4 m/s |

- Corriendo, Lukas deja atrás al Alto y a la Boca, pero no al Rastrero ni a los estallidos del Mudo. Correr sirve para escapar de un mal momento, no para recorrer el mapa.
- Una sola barra de resistencia paga tres acciones.

| Acción | Costo |
| --- | --- |
| Correr | ~10% por segundo |
| Empujón o golpe de culata | 35% |
| Golpe en el suelo para rematar | 25% |

- La barra se regenera tras 3 s sin esas acciones y pasa de 0 a 100% en ~4 s, a 25% por segundo. Caminar no interrumpe la regeneración.
- El empujón aparta y aturde un momento al enemigo, pero no protege del daño.
- Disparar no gasta resistencia.
- **Agotamiento.** Si la barra llega a 0, Lukas no puede correr ni empujar durante 2 s.

**Entrada:** `Shift izquierdo` mantenido con una dirección para correr. `Clic izquierdo` sin apuntar para empujar.

**Estado inicial:** Barra al 100%.

**Estado final:** Barra gastada en proporción, o agotamiento si llegó a 0.

**Recursos utilizados:** Barra de resistencia.

**Recompensa:** Ganar distancia o tiempo y ahorrar munición.

**Penalización:** Una barra mal administrada deja a Lukas exhausto e indefenso.

**Interacciones con otras mecánicas:** Cámara sobre el hombro, Combate a distancia, Salud y estados, Sigilo y ruido, Escape de combate y Reciclaje de enemigos.

**Casos límite:**

- Sin resistencia suficiente, el empujón no sale y Lukas jadea de forma audible.
- Con dos enemigos atacando a la vez, el empujón solo afecta al de enfrente.

### 2.7 Inventario de 6 casillas en tiempo real

**Propósito:** Obligar a priorizar recursos y a buscar un lugar seguro antes de usarlos.

**Activación:** Desde el terminal del brazo durante el recorrido.

**Reglas:**

- El inventario tiene 6 casillas fijas.
- Abrirlo no pausa el juego ni detiene a los enemigos.
- Los objetos no se apilan sin límite.
- Usar, equipar o descartar un objeto tarda ~1,5 s de animación.
- Ocupan casilla cinco tipos de objeto. El revólver, la linterna y el anillo no ocupan casilla.

| Objeto | Máximo por casilla |
| --- | ---: |
| Cura | 1 |
| Munición | 12 balas |
| Punzón de descarga | 1 |
| Llave obligatoria | 1 |
| Llave opcional | 1 |

- Cada llave es un objeto clave, ocupa una casilla y se gasta al abrir su puerta. Los presupuestos por piso están en [[06 - Economía y Recursos]].
- Al llegar a la sala clave final de los pisos 1 y 2, el autoguardado conserva el inventario con el que llegó Lukas. Ver [[07 - Reglas y Estados del Juego]].

**Entrada:** `Tab` abre y cierra. `Clic izquierdo` en una casilla usa o equipa y `Clic derecho` descarta.

**Estado inicial:** Inventario oculto.

**Estado final:** Inventario desplegado mientras el juego sigue en tiempo real.

**Recursos utilizados:** Las 6 casillas.

**Recompensa:** Ordenar el equipo, curarse y recargar.

**Penalización:** Abrirlo en un pasillo sin despejar deja a Lukas indefenso.

**Interacciones con otras mecánicas:** Combate a distancia, Salud y estados, Interacción y documentos, Punzón de descarga, Reciclaje de enemigos, Exploración vertical y Reaparición por reconstrucción.

**Casos límite:**

- Con el inventario lleno, solo un objeto clave permite intercambiar. El terminal muestra `INVENTORY FULL — REPLACE WITH ACCESS KEY?` y el objeto reemplazado cae a los pies de Lukas. Si el jugador no acepta, la llave sigue en el suelo.
- Al morir o reiniciar se pierden las llaves del piso, que se regenera con una llave obligatoria nueva y alcanzable.

### 2.8 Interacción, recolección e investigación de documentos

**Propósito:** Obtener objetos, activar mecanismos y descubrir la historia y las reglas.

**Activación:** Acercarse a un objeto, una puerta, un documento o un terminal interactivo.

**Reglas:**

- Una indicación diegética o un aviso mínimo de interfaz señala cada elemento interactivo.
- Si un cuerpo no rematado está junto a otro elemento interactivo, gana el que esté en el centro de la vista.
- **Llaves.** Cada piso tiene una llave obligatoria, que solo abre la puerta de la sala clave final, y llaves opcionales que abren atajos o salas con recursos. Abrir una puerta con llave gasta la llave. Las llaves son los únicos objetos clave del juego.
- Las salas clave secundarias guardan documentos, llaves y, a veces, el interruptor de energía de la sala final. Los textos de los documentos están en [[04 - Diálogos y Narrativa]].
- Los objetos recogidos pasan al inventario si hay espacio.
- Todo lo leído queda en el terminal del brazo y sobrevive a la muerte. El jugador y Lukas se informan juntos.
- **Reacción de Lukas.** Al encontrar un documento, Lukas puede reaccionar con una frase corta o un murmullo, con sonido y texto en pantalla, sin actuación de voz.
- **Leer detiene el juego.** Abrir un documento o un póster lo muestra en una vista de lectura propia que congela el recorrido hasta cerrarla con `E` o `Esc`. La lista del terminal del brazo no pausa, solo la lectura.

| Material | Cantidad | Ubicación | ¿Cuenta para el final? |
| --- | --- | --- | --- |
| Documentos de Emilia | 6, dos por piso | Siempre en salas clave secundarias, también en la configuración showcase, que tiene 2 secundarias por piso | Sí, los 6 acumulados en cualquier intento |
| Documentos de Vitalis Konzern, Kessler Sicherheit AG y The Mother | 12, cuatro por piso | Salas procedurales, 1 o 2 por intento, con prioridad para los no registrados | No |
| Objetos confiscados | 12, cuatro por piso | Salas procedurales, 1 o 2 por intento | No |

- **Objetos confiscados.** Son narrativa escénica. En el piso 1 son pertenencias del personal, en el piso 2 son objetos quitados a los sujetos, incluidos niños, y en el piso 3 son restos del incidente. Se examinan con `E`, no se recogen ni ocupan casilla y quedan en la sección `Objetos confiscados` del terminal.
- **Documentos corruptos.** Algunos documentos se registran corruptos, con partes ilegibles. Sale corrupto cualquier pasaje que nombre a Lukas, se dirija a él o hable de su vida con Emilia, en cualquier documento.
- La corrupción es un fallo de la realidad provocado por el laboratorio o el latido. No es censura y no tiene relación con las fallas de la máquina. Solo se completa en el desenlace si Lukas reunió los 6 documentos de Emilia.
- **Estilo.** Los documentos toman como referencia *Signalis*, con la burocracia de Vitalis Konzern y The Mother frente al diario de Emilia. Hay fragmentos en alemán que no contienen información necesaria. En el mundo no se traducen. Los registros del terminal del brazo muestran su traducción en un subtítulo. La frase clave es *Escucha el latido*.

**Entrada:** `E` frente al elemento, dentro de un cono de ~35° delante de Lukas y de la cámara. Una puerta a su espalda no responde aunque esté al alcance.

**Estado inicial:** Elemento sin usar.

**Estado final:** Objeto recogido, mecanismo activado o documento registrado.

**Recursos utilizados:** Tiempo y espacio en el inventario.

**Recompensa:** Recursos, historia y reglas del sistema.

**Penalización:** Exposición mientras se interactúa o se lee.

**Interacciones con otras mecánicas:** Inventario, Hackeo por secuencias, Exploración vertical, Reaparición por reconstrucción, Fallas de la máquina, Jefe final y Reciclaje de enemigos.

**Casos límite:**

- Si Lukas muere mientras lee, el documento se registra antes de procesar la muerte.
- Una puerta sin su llave suena a cerradura bloqueada y no se abre.
- La corrupción nunca toca una regla del sistema ni una pista de ruta, solo el contenido narrativo.
- Si el documento de Emilia de una sala secundaria ya está registrado, la sala ofrece 1 recurso extra del tipo más escaso del piso, por encima del presupuesto. Ver [[06 - Economía y Recursos]].

## 3. Mecánicas secundarias

Las doce mecánicas siguientes complementan el núcleo. Fallas de la máquina y Degradación por muerte se planearon como deseables y las demás como obligatorias para la demo.

### 3.1 Sigilo y ruido

**Propósito:** Evitar el combate, ahorrar munición y moverse entre coberturas.

**Activación:** Agacharse o moverse con cuidado detrás de coberturas.

**Reglas:**

- La percepción de los enemigos no se muestra en pantalla. Solo se lee por señales diegéticas.
- La visión de los enemigos no atraviesa objetos sólidos. Si una cobertura tapa a Lukas, el enemigo no lo ve.

| Acción | Ruido |
| --- | --- |
| Moverse agachado | Ninguno |
| Caminar | Bajo |
| Correr | Pulso en área que alerta a los enemigos cercanos |
| Disparar | Alerta en ~20 m |
| Golpe en el suelo | Alerta a los enemigos cercanos |
| Punzón de descarga | Ninguno |

- Iluminar a un enemigo duplica la velocidad a la que sube su alerta. La Boca es ciega y no reacciona a la luz.
- El grito de la Boca manda a los enemigos cercanos a buscar hacia ella.
- El Mudo no patrulla. Se activa a ~3 m o al ser iluminado.
- Las velocidades y rangos de detección de cada tipo están en [[03 - Enemigos y NPC]].

**Entrada:** `Ctrl izquierdo` alterna agacharse.

**Estado inicial:** No detectado.

**Estado final:** Oculto o detectado.

**Recursos utilizados:** Coberturas, oscuridad y tiempo.

**Recompensa:** Cruzar salas peligrosas sin gastar munición ni curas.

**Penalización:** Avance más lento y riesgo de quedar acorralado.

**Interacciones con otras mecánicas:** Vista de vigilancia, Bloqueo de vigilancia, Resistencia, Señales diegéticas, Linterna y Reciclaje de enemigos.

**Casos límite:**

- Si el haz toca a un enemigo que reacciona a la luz sin ver a Lukas, por estar de espaldas o tras una cobertura, pasa a sospecha hacia él.
- La detección del reflejo del haz en el suelo queda fuera de la demo.

### 3.2 Señales diegéticas

**Propósito:** Comunicar la percepción y la cercanía de los enemigos sin indicadores en pantalla, según el principio 3 de [[01 - Jugabilidad]].

**Activación:** Automática según el estado de los enemigos y de Lukas.

**Reglas:**

- Cada enemigo tiene tres estados de alerta con sonido propio, que son sospecha, búsqueda y caza.
- Cuando este documento dice *en alerta*, se refiere a caza. Es el estado que corta y bloquea la vigilancia.
- La propia señal de vigilancia es una señal diegética. No cambia en sospecha, se degrada en búsqueda y se corta en caza.
- La atención de The Mother no tiene indicador. El jugador la percibe porque los enemigos cercanos pasan a sospecha y búsqueda y la señal se degrada.
- En la fase 2 del jefe, cada cámara de la sala enciende una luz roja y hace sonar su motor al girar.
- La linterna o los ojos de un enemigo indican hacia dónde mira.
- Los pasos suenan distinto en metal, agua y escombros.
- El grito de la Boca tiene un sonido propio, distinto del latido del laboratorio y de las alertas. Avisa que los enemigos cercanos van hacia ella.
- Unos crujidos de hueso y carne avisan que un cuerpo está a punto de reciclarse.

Hay dos latidos y no deben confundirse.

| Latido | Comportamiento |
| --- | --- |
| De Lukas | Sube con la cercanía de una amenaza. Es aproximado para no funcionar como radar. No reacciona a enemigos detrás de una puerta sellada. |
| Del laboratorio | Tiene un timbre grave propio y nunca depende de la cercanía de enemigos. Crece con la degradación acumulada, sube mientras Lukas está en Crítico y se acelera cuando cae The Mother, en el Despertar. |

El latido del laboratorio se oye por primera vez de fondo en la nota de voz de Emilia. Desde la primera reconstrucción suena en todas las salas. Así no compite con la señal de cercanía, que sigue siendo el latido de Lukas. Es el primer acto del texto. Inquieta, pero no asimila, y ningún enemigo lo emite.

**Entrada:** No tiene.

**Estado inicial:** Ambiente sin señales de amenaza.

**Estado final:** Señales activas según la alerta y la distancia.

**Recursos utilizados:** Audio, animación e iluminación.

**Recompensa:** Anticipar amenazas y decidir a tiempo entre esconderse, rodear o escapar.

**Penalización:** Leer mal una señal lleva a Lukas hacia la amenaza.

**Interacciones con otras mecánicas:** Vista de vigilancia, Bloqueo de vigilancia, Sigilo y ruido, Linterna, Exploración vertical, Degradación, Jefe final y Reciclaje de enemigos.

**Casos límite:**

- Con enemigos en estados distintos, suena el más alto.
- Con la degradación más alta, el latido del laboratorio crece sin tapar el latido de Lukas ni las alertas.

### 3.3 Punzón de descarga

**Propósito:** Defender de los agarres con un objeto que compite por espacio y con el remate silencioso, sin depender de la habilidad manual.

**Activación:** Automática al ser agarrado con un punzón en el inventario. `E` sobre un cuerpo para rematar.

**Reglas:**

- El punzón de descarga es equipo de contención de sujetos de Kessler Sicherheit AG. Es un objeto de un solo uso con dos funciones. Ocupa una casilla, con 1 por casilla.
- **Defensa.** Al ser agarrado, Lukas clava el punzón de forma automática. El punzón se gasta y la descarga aturde al enemigo 2 s.
- **Remate.** Con `E` sobre un cuerpo, la descarga quema su último rastro de identidad. El laboratorio ya no puede reciclarlo y ningún enemigo lo oye.
- Gastarlo en un remate deja a Lukas sin defensa ante el próximo agarre.
- Sin punzón, el agarre baja un estado de salud, o mata si Lukas ya estaba en Crítico.
- Agarran el Alto, el Mudo y el brazo recolector de The Mother. El Rastrero y la Boca no agarran.

**Entrada:** Defensa automática y `E` sobre un cuerpo para el remate. Ver [[04 - Controles]].

**Estado inicial:** Lukas agarrado, o frente a un cuerpo no rematado, con un punzón.

**Estado final:** Enemigo aturdido 2 s o cuerpo rematado, con el punzón gastado. Sin punzón, el agarre baja un estado de salud o mata.

**Recursos utilizados:** El punzón y su casilla.

**Recompensa:** Evitar el daño y ganar 2 s, o quitar una amenaza del piso.

**Penalización:** Ocupa una casilla que podría ser cura o munición. Gastado en un remate, ya no protege del próximo agarre.

**Interacciones con otras mecánicas:** Bloqueo de vigilancia, Salud y estados, Inventario, Escape de combate y Reciclaje de enemigos.

**Casos límite:**

- Con varios punzones, cada agarre gasta uno.
- Un agarre durante un hackeo cancela el hackeo antes de resolverse.
- El remate no se ofrece en cuerpos que ya no pueden reciclarse, que son los reciclados y los de la sala del jefe.

### 3.4 Linterna

**Propósito:** Ver en la oscuridad y apuntar con precisión, a cambio de delatarse.

**Activación:** `F` la enciende y la apaga. Al apuntar, el haz se concentra solo.

**Reglas:**

- Apagada, Lukas ve poco y la luz no lo delata. Encendida, ve con claridad dentro del cono, pero el riesgo de alerta es alto.
- Sin apuntar, el cono es corto y amplio. Al apuntar se estrecha y llega más lejos en la dirección del arma.
- Si el haz ilumina a un enemigo mientras Lukas se oculta, su alerta sube el doble de rápido. Si el enemigo no ve a Lukas, el haz lo pone en sospecha hacia él.
- **Carga.** La linterna no usa baterías ni ocupa casilla. Su carga dura ~120 s de uso continuo.
- Tras ~3 s apagada se recarga sola y pasa de 0 a 100% en ~30 s.
- Al vaciarse parpadea, se apaga y no enciende durante ~10 s. El indicador está en el terminal del brazo.

**Entrada:** `F`.

**Estado inicial:** Encendida o apagada según la zona.

**Estado final:** Encendida o apagada.

**Recursos utilizados:** Carga de la linterna y exposición.

**Recompensa:** Revela pistas, objetos ocultos y enemigos al acecho.

**Penalización:** Delata a Lukas ante enemigos lejanos que alcance el haz.

**Interacciones con otras mecánicas:** Cámara sobre el hombro, Bloqueo de vigilancia, Sigilo y ruido y Señales diegéticas.

**Casos límite:**

- La detección de la luz a través de rejillas o ranuras entre salas queda fuera de la demo.

### 3.5 Hackeo por secuencias

**Propósito:** Sumar estrés y habilidad manual al manipular en tiempo real puertas, elevadores y terminales de sala.

**Activación:** Iniciar la interacción frente a un panel o terminal de sala.

**Reglas:**

- Lukas queda inmóvil en la vista sobre el hombro durante toda la secuencia.
- El terminal del brazo muestra una cadena de direcciones. En la introducción, antes de tener el terminal, la secuencia aparece en la pantalla del teléfono de Lukas, con las mismas reglas.

| Acción | Entradas |
| --- | --- |
| Sellado rápido de puerta | 2 a 3 |
| Puerta estándar | 4 a 5 |
| Elevador o terminal principal | 7 a 8 |

- El juego no se pausa durante la secuencia.
- Cada entrada correcta suena con un tono. Un error emite un pitido y reinicia la secuencia.
- Un golpe cancela el hackeo.
- Una dirección que ya estaba pulsada al empezar el hackeo no cuenta hasta que se suelta.
- El stick izquierdo cuenta una entrada al pasar del 75 % de su recorrido y no la repite mientras se mantiene inclinado.
- Los terminales de sala solo sirven para hackear y no guardan información persistente.

![Secuencia de hackeo en el terminal del brazo|520](../Media/Capturas/HackeoSecuencia.png)

**Entrada:** `E` inicia y abandona. Las `Flechas`, `W` `A` `S` `D`, la cruceta o el stick izquierdo del mando introducen la secuencia.

**Estado inicial:** Panel bloqueado.

**Estado final:** Con éxito, la puerta se abre o se sella, o el sistema o el elevador quedan habilitados. Con un error, la secuencia se reinicia.

**Recursos utilizados:** Tiempo y atención del jugador.

**Recompensa:** Abrir rutas, sellar puertas ante perseguidores y habilitar elevadores.

**Penalización:** Vulnerabilidad total durante la secuencia.

**Interacciones con otras mecánicas:** Cámara sobre el hombro, Escape de combate, Exploración vertical, Fallas de la máquina y Jefe final.

**Casos límite:**

- Si un enemigo entra en caza durante la secuencia, el hackeo sigue hasta que Lukas recibe un golpe o lo abandona.

### 3.6 Escape de combate y retirada

**Propósito:** Romper un combate insostenible y retirarse a otra sala.

**Activación:** Correr hacia una salida durante un enfrentamiento.

**Reglas:**

- Lukas rompe el combate al cruzar el marco de una puerta o al cerrarla tras de sí.
- Los enemigos lo persiguen un momento o golpean la puerta antes de volver a patrullar.
- **Puertas cerradas.** Para un enemigo en patrulla o sospecha, una puerta cerrada es una pared. En búsqueda o caza se acerca y la abre. Ninguno atraviesa una hoja cerrada.
- La sala de inicio no admite enemigos hasta que Lukas entra en otra sala.
- El sellado rápido por hackeo, de 2 a 3 entradas, corta la persecución. Un enemigo tarda ~30 s en romper el sellado, que también se rompe si Lukas cruza la puerta.

**Entrada:** `W` `A` `S` `D` hacia la salida y `E` para cerrar o sellar la puerta.

**Estado inicial:** Caza o combate activo.

**Estado final:** Reagrupación o sigilo en la sala contigua.

**Recursos utilizados:** Resistencia para huir y tiempo para sellar.

**Recompensa:** Sobrevivir a un combate desfavorable sin perder salud ni recursos.

**Penalización:** La sala abandonada sigue infestada al volver, reciclados incluidos.

**Interacciones con otras mecánicas:** Vista de vigilancia, Inventario, Hackeo por secuencias y Reciclaje de enemigos. Huir da tiempo para abrir el inventario, sin garantizar seguridad.

**Casos límite:**

- Un golpe mientras Lukas manipula la puerta cancela la acción.

### 3.7 Exploración vertical por pisos

**Propósito:** Estructurar el intento en pisos conectados sin perder la coherencia procedural.

**Activación:** Usar la escalera o el elevador de la sala clave final.

**Reglas:**

- **Estructura.** El intento recorre 3 pisos de ~10 a 15 salas cada uno.

| Tipo de sala | Por piso | Función | Autoguardado |
| --- | --- | --- | --- |
| Procedural | 8 a 12 | Exploración, amenazas y recursos | No |
| Clave secundaria | 2 a 3 | Documentos, llaves y, a veces, el interruptor de energía | No |
| Clave final | 1 | Escalera o elevador al piso siguiente | Sí, en los pisos 1 y 2 |

- En el piso 3, la sala de The Mother ocupa el lugar de la sala clave final y no guarda la partida.

| Configuración | Procedurales por piso | Secundarias por piso | Final por piso | Duración |
| --- | --- | --- | --- | --- |
| Completa | 8 a 12 | 2 a 3 | 1 | ~45 a 60 min |
| Showcase, para eventos | 6 a 8 | 2 | 1 | ~30 min |

- En el showcase, los enemigos y los recursos se reducen en proporción al número de salas. Los presupuestos están en [[06 - Economía y Recursos]].
- **Enemigos por piso.** La configuración completa tiene 6 a 9 en el piso 1 y 9 a 12 en el piso 2. El piso 3 tiene 12 a 15, además de The Mother.
- Cada sala tiene como máximo 2 enemigos activos y ~1 de cada 3 salas queda vacía a propósito. Si el piso generado tiene pocas salas, manda el límite de 2 por sala ocupada aunque el total quede bajo el rango. La curva de dificultad está en [[05 - Progresión]].
- Las escaleras y los elevadores solo existen en las salas clave finales. Es la única forma de cambiar de piso.
- La sala de inicio de los pisos 2 y 3 tiene el elevador que trajo a Lukas, averiado y sin arreglo. Es solo decorado.
- La posición de las salas clave cambia en cada intento. El resto del piso son salas procedurales.
- **Tutorial.** Ocupa las primeras 4 salas del piso 1. La primera es la fila de cápsulas, la sala clave de inicio del piso. Le siguen 3 salas procedurales fijas en el primer intento: el depósito a oscuras, el túnel y la sala de calderas con 1 Rastrero.
- En las salas del tutorial Lukas no puede morir. Desde el segundo intento, las 3 salas procedurales vuelven a generarse como las demás. Ver [[02 - Niveles]].
- **Salas procedurales.** Combinan seis arquetipos con el tema de cada piso. Los arquetipos son pasillo, almacén, sala abierta, sala de vigilancia, sala de hackeo y sala vertical. Los temas son mantenimiento, clínica ilegal y núcleo.
- **Llaves.** La llave obligatoria del piso abre la puerta de la sala clave final. El generador la deja alcanzable antes de esa puerta. Las llaves opcionales abren atajos o salas con recursos.
- Cada piso garantiza al menos una ruta válida hasta su sala clave final.
- El elevador puede exigir un hackeo largo.
- **Energía.** En los pisos 1 y 2, la sala clave final tiene ~1 de cada 3 posibilidades de empezar sin energía. Nunca ocurre en el primer intento del piso 1 ni en el piso 3.
- El interruptor de energía está en una sala clave secundaria. Restaurarlo con `E` suena a maquinaria y alerta a los enemigos cercanos.
- **Autoguardado.** Al llegar a la sala clave final de los pisos 1 y 2 sin enemigos en caza, la partida se autoguarda con un aviso discreto en el terminal del brazo. Si Lukas llega perseguido, el guardado espera a que termine la caza. Las reglas completas están en [[07 - Reglas y Estados del Juego]].
- **Contador de muertes.** Solo llegar a la sala clave final del piso 2 reinicia el contador de 5 muertes. Si la degradación no se implementa, la exploración vertical funciona igual, sin contador.
- **Cierre de tránsito.** Mientras haya enemigos en caza, la escalera y el elevador de la sala final quedan en cierre. Se enciende el panel rojo `LOCKDOWN` y The Mother emite una línea de protocolo, por ejemplo `Containment breach. Transit suspended.`.
- Al terminar la caza, la partida se autoguarda y el tránsito se abre a la vez. Ningún enemigo cambia de piso.
- **Transición de piso.** Usar la escalera o el elevador reproduce una cinemática de ascensor o escalera de al menos 5 s. Oculta la carga y muestra el aviso de autoguardado. Si la carga tarda, la cinemática se alarga. No hay pantalla de carga.

![Escalera de la sala clave final en cierre con el panel LOCKDOWN|520](../Media/Capturas/CierreTransito.png)

Recorrido de un intento: introducción y cinemática completa → piso 1, con tutorial fijo la primera vez → sala final 1 con autoguardado → piso 2 → sala final 2 con autoguardado y reinicio del contador → piso 3 → sala de The Mother → victoria.

Morir o elegir `Reiniciar` en cualquier punto reproduce la cinemática corta y devuelve a Lukas al inicio del mismo piso. En la sala de The Mother lo devuelve al inicio del piso 3.

**Entrada:** `E` en la escalera o el elevador.

**Estado inicial:** Lukas en un piso sin alcanzar su sala clave final.

**Estado final:** Lukas en el piso siguiente o en la sala del jefe.

**Recursos utilizados:** Tiempo, la llave obligatoria y energía o un hackeo para los elevadores.

**Recompensa:** Avanzar, autoguardar en los pisos 1 y 2 y reiniciar el contador de muertes en el piso 2.

**Penalización:** Hackeos largos bajo amenaza, una casilla ocupada por la llave y tránsito cerrado durante una caza.

**Interacciones con otras mecánicas:** Vista de vigilancia, Inventario, Interacción y documentos, Señales diegéticas, Hackeo por secuencias, Reaparición por reconstrucción, Degradación y Jefe final.

**Casos límite:**

- Si Lukas llega perseguido, puede sellar la sala con un hackeo mientras espera el fin de la caza.
- Si la sala final está sin energía, el generador garantiza una ruta al interruptor de la sala secundaria del mismo piso.

### 3.8 Reaparición por reconstrucción y terminal del brazo

**Propósito:** Convertir el fracaso en conocimiento e integrar la muerte al ciclo y a la historia.

**Activación:** Automática al morir, o al elegir `Reiniciar` en el menú de pausa.

**Reglas:**

- **Reaparición.** Al morir, Lukas se reconstruye al inicio del piso actual. El piso se genera con una semilla nueva, así que cambian las salas procedurales, los recursos y la posición de las salas clave.
- Lukas conserva el inventario con el que llegó al piso y pierde lo recogido en él. Morir en la sala de The Mother devuelve al inicio del piso 3.
- El autoguardado de la sala final no mueve el punto de reaparición. Solo sirve para continuar después de cerrar el juego o de un fallo. Ver [[07 - Reglas y Estados del Juego]].
- Desde el segundo intento, las 3 salas procedurales del tutorial también se generan con la semilla.
- **Cinemática completa.** Se ve una sola vez, al final de la introducción, y muestra todo el proceso de reconstrucción en imágenes fugaces de 1 a 2 s con cortes a negro.
- Lukas muere al final de la introducción, atacado por la espalda en un piso que no existe.
- La cinemática completa es el momento del olvido. Lukas recordaba a Emilia durante la introducción y aquí la pierde de golpe.
- Sale de la cápsula sabiendo que busca algo o a alguien, sin recordar a quién. Recoge de la bandeja de efectos personales un anillo que sabe importante. La cinemática espera hasta que el jugador lo recoge. Nunca se aclara si es el suyo o el de Emilia. El resto está en [[01 - Premisa y Trama]].
- **Cinemática corta.** En cada muerte o reinicio, un plano corto de ~3 s muestra a Lukas de pie ante su cápsula, con sonido de maquinaria. Se ven la degradación acumulada y la pieza ajena de esta reconstrucción. Se salta con `Espacio`.
- Junto a la cápsula, The Mother deja el revólver descargado en una bandeja rotulada `Efectos personales R-H-09: 1 revólver (descargado), 6 balas`.
- **Código de sujeto.** The Mother identifica a Lukas con un código R-H del mismo expediente que Emilia, que va de R-H-01 a R-H-06. La primera reconstrucción lo registra como R-H-07.
- Cada reconstrucción posterior suma 1, como R-H-08 y R-H-09. También suman las de `Reiniciar`, la salida con costo y las del primer intento.
- El código aparece en el terminal del brazo y no tiene efecto mecánico. Se mantiene en el ciclo nuevo que sigue a un final.
- **Mensaje de Emilia.** La última pista de Emilia es una nota de voz programada que llega al teléfono de Lukas. Se presenta como transcripción con sonido vocal y forma de onda, sin actuación de voz.
- El teléfono se pierde en la primera reconstrucción. The Mother copia su contenido al terminal como `efectos personales del sujeto`. El mensaje queda corrupto y solo se completa en el desenlace si Lukas reunió los 6 documentos de Emilia.
- **Bestiario.** Sus entradas son informes clínicos de The Mother sobre sujetos asimilados, con su código. Cada entrada se desbloquea en el primer encuentro y persiste entre muertes.
- **Reiniciar.** Es una opción del menú de pausa. Devuelve al inicio del piso actual, siempre cuenta como muerte y conserva el terminal del brazo. Suma al contador y a la degradación, y lo recogido en el piso se pierde igual que al morir.
- **Nueva partida.** Es una opción del menú principal. Pide confirmación y borra todo el progreso de la ranura elegida, terminal incluido.
- **Terminal del brazo.** Es la base de datos persistente del juego. Conserva entre muertes el bestiario, el lore, los documentos, incluidos los de Emilia, las fallas de la máquina y las reglas del sistema.
- El terminal registra reglas, no valores. Por ejemplo, registra cómo se forma un código, pero no el código.
- Los terminales de sala solo sirven para hackear.
- Consultar el terminal del brazo no pausa. Abrir una entrada para leerla sí.
- Lo que persiste informa a Lukas, pero no lo hace más fuerte.

**Entrada:** `Tab` consulta el terminal junto al inventario, `Esc` abre la pausa con `Reiniciar` y `Espacio` salta la cinemática corta. Ver [[04 - Controles]].

**Estado inicial:** Lukas muerto o reinicio pedido.

**Estado final:** Lukas al inicio del piso actual, con su inventario de llegada y el terminal actualizado.

**Recursos utilizados:** El archivo de guardado.

**Recompensa:** Conocimiento persistente, pistas y avance en la historia.

**Penalización:** Se pierde lo recogido y el progreso en el piso. `Reiniciar` cuesta lo mismo que morir.

**Interacciones con otras mecánicas:** Salud y estados, Inventario, Interacción y documentos, Exploración vertical por pisos, Fallas de la máquina, Degradación por muerte y Jefe final.

**Casos límite:**

- Si Lukas muere mientras se registra información, se guarda antes de procesar la muerte.
- Morir o reiniciar descarta el autoguardado de la sala final. El siguiente se crea al llegar de nuevo a una sala clave final.
- En el primer intento, morir o reiniciar no suma al contador, pero sí sube el código de sujeto.
- `Nueva partida` borra el terminal de su ranura, código incluido, y la siguiente reconstrucción vuelve a ser R-H-07.

### 3.9 Fallas de la máquina

**Propósito:** Premiar lo aprendido entre intentos con ventajas temporales.

**Activación:** Descubrir una falla en documentos, por observación o en terminales, y usarla después.

**Reglas:**

- Una falla es una ventaja de conocimiento, como una puerta trasera que acorta un hackeo o un acceso alternativo dentro del piso.
- Las fallas quedan registradas en el terminal del brazo.
- The Mother parchea cada falla después de 1 o 2 usos o cuando Lukas muere.
- En otros intentos aparecen fallas nuevas.
- La demo tiene 1 o 2 fallas.

**Entrada:** `F` durante un hackeo usa una falla compatible.

**Estado inicial:** Falla sin descubrir.

**Estado final:** Falla parcheada por The Mother.

**Recursos utilizados:** Conocimiento registrado y usos disponibles.

**Recompensa:** Secuencias más cortas o accesos alternativos en el piso.

**Penalización:** Usarla en mal momento la desperdicia.

**Interacciones con otras mecánicas:** Interacción y documentos, Hackeo por secuencias, Reaparición por reconstrucción y Jefe final, donde acorta las secuencias de la fase 1.

**Casos límite:**

- Si el parche llega a mitad de una secuencia, esta termina con su longitud original.

### 3.10 Degradación por muerte y final de 5 muertes

**Propósito:** Dar peso narrativo a cada muerte y un final secundario de derrota.

**Activación:** Automática al morir, al elegir `Reiniciar` o al salir con costo.

**Reglas:**

- Cada muerte aumenta una degradación estética acumulativa, con más corrupción CRT y más distorsión del sonido vocal.
- El texto en pantalla de las líneas de Lukas solo se degrada en la cinemática de Asimilación.
- La cinemática corta de reconstrucción refleja la degradación acumulada. El latido del laboratorio crece con ella.
- **Cuerpo ajeno.** Cada reconstrucción añade al modelo de Lukas una pieza ajena que viene del laboratorio. Hay de 3 a 5 variantes y la mano del anillo puede terminar asimilada.
- Al llegar a la última variante, el modelo se queda en ella. Es solo presentación y no tiene efecto mecánico.
- La degradación solo se acumula y ninguna sala clave la reinicia. Vuelve a 0 solo en el ciclo nuevo tras un final o con `Nueva partida`.
- **Tope visual.** Ninguna capa de la degradación tapa la interfaz crítica ni las señales diegéticas.
- La degradación es solo estética y no afecta la jugabilidad.
- **Asimilación.** Cinco muertes consecutivas desde el último reinicio del contador activan el final Asimilación. El laboratorio termina de absorber a Lukas y lo convierte en uno de los enemigos comunes. Su estado figura como `Final: Asimilación` en [[07 - Reglas y Estados del Juego]].
- **Reinicio del contador.** Solo lo reinicia llegar a la sala clave final del piso 2. Las salas secundarias y la sala final del piso 1 no lo reinician.
- La primera vez que se reinicia, se reproduce la cinemática corta del hallazgo de la foto. Las siguientes veces el reinicio es silencioso.
- Tras el reinicio, el contador sigue corriendo. Cinco muertes en el piso 3 o ante The Mother también terminan en Asimilación.
- **Qué suma.** Suman al contador y a la degradación cada muerte, cada `Reiniciar` y cada salida del juego con costo. Un cierre inesperado o un fallo del juego no cuentan.
- **Primer intento.** En el primer intento, el del tutorial, morir, reiniciar o salir no suma al contador. Sí suma a la degradación.

**Entrada:** No tiene.

**Estado inicial:** Contador en 0 y sin degradación.

**Estado final:** Degradación acumulada con su variante de cuerpo ajeno, o final Asimilación si el contador llega a 5.

**Recursos utilizados:** El contador de muertes desde su último reinicio.

**Recompensa:** Ninguna mecánica. Refuerza la narrativa del ciclo.

**Penalización:** Final de derrota tras 5 muertes consecutivas desde el último reinicio del contador.

**Interacciones con otras mecánicas:** Señales diegéticas, Exploración vertical y Reaparición por reconstrucción.

**Casos límite:**

- Si la quinta suma llega por `Reiniciar` o por una salida con costo, el final espera a la siguiente muerte real de Lukas.
- Ninguna variante del cuerpo ajeno oculta el terminal del brazo ni la interfaz crítica.

### 3.11 Jefe final: The Mother

**Propósito:** Cerrar la demo con un encuentro que exige las herramientas del ciclo, sin sistemas nuevos.

**Activación:** Entrar a la sala de The Mother, sala clave fija al final del piso 3.

**Reglas:**

- **La sala.** Es la central de megafonía desde donde se emitió el latido durante el incidente. Es una sola sala grande que cambia en cada fase. La terminal central es la consola de emisión.
- No hay carne en la sala, porque The Mother no es la criatura. Es un archivo de muestras criogénicas montado en una grúa, con un brazo recolector y un núcleo criogénico.
- Llegar a la sala no guarda la partida. Morir en ella devuelve al inicio del piso 3.
- **El brazo.** Fiel a su protocolo, The Mother trata a Lukas como un espécimen no conforme que hay que recolectar y archivar. Su brazo recolector recorre la sala en las tres fases.
- Si el brazo alcanza a Lukas, lo agarra con las reglas del punzón. Con punzón, el brazo queda aturdido 2 s. Sin punzón, Lukas baja un estado de salud.
- El brazo no mata en el primer agarre en Crítico y deja a Lukas al borde de la muerte. El siguiente agarre en Crítico lo mata si antes no se curó ni usó un punzón.
- Los asimilados pueden matar a Lukas en cualquier fase.

| Parámetro | Valor |
| --- | --- |
| Cámaras hostiles | 3, con barrido de ~20°/s y cono de ~12 m |
| Tiempo de fijación | ~1 s enfocando a Lukas para encender la luz roja y liberar un asimilado |
| Velocidad de la grúa | ~1,6 m/s sobre rieles |
| Alcance del brazo | ~2,2 m |
| Aviso del agarre | ~2,2 s antes, con luz, sonido hidráulico y un anillo rojo que se cierra en el suelo |
| Descanso entre agarres | ~7 s |
| Duración de cada fase | 1 a 2 min |

- **Fases.** Cada fase empieza con una línea de protocolo de The Mother.
    1. **Hackeo de nodos bajo presión.** Lukas hackea 3 nodos con secuencias de terminal principal en las mesas de mezcla y los racks de cinta mientras el brazo patrulla los nodos. Las fallas descubiertas acortan las secuencias. Sin fallas, la fase funciona con la longitud original.
    2. **Supervivencia y huida con cámaras hostiles.** Los altavoces emiten el latido y las cámaras dejan de ser una herramienta del jugador para vigilarlo. Cada vez que una enfoca a Lukas, The Mother libera desde una cápsula de su archivo un asimilado que acude a ese punto.
    3. **Disparos a puntos débiles expuestos.** The Mother abre su núcleo y expone los 3 transformadores de alto voltaje que la alimentan, de 2 disparos cada uno. Al caer el último, la estantería se apaga y The Mother cae.
- En la fase 2 salen 4 asimilados en total, 2 Altos y 2 Rastreros, con un máximo de 2 activos a la vez.
- Una niebla criogénica reduce la visibilidad, sin ralentizar ni tapar las señales diegéticas. Lukas avanza por los puntos ciegos de las cámaras, donde hay 12 balas en 3 blísteres.
- La fase 2 termina cuando los 4 asimilados están eliminados y Lukas llega al núcleo. Si llega antes, salen todos los que falten con el mismo máximo de 2. Esquivar las cámaras nunca bloquea la fase.
- En la fase 3 hay 8 balas junto al núcleo, 6 para los transformadores y 2 de margen, aunque Lukas llegue sin munición. Esta fase es corta para no convertir el juego en acción.
- **Sin reciclaje.** La sala es solo máquina, sin carne, y el laboratorio no levanta cuerpos en ella.
- Derrotar a The Mother es la condición de victoria. Ver [[01 - Jugabilidad]].
- Al caer, The Mother revela que existe algo más, la criatura lovecraftiana que es el laboratorio, pero no puede explicar qué es.

![Fase 2 de The Mother con una cámara hostil en luz roja|520](../Media/Capturas/MotherFase2.png)

**Secuencia de victoria.** La victoria sigue siempre este orden.

1. Cae The Mother.
2. Con los 6 documentos de Emilia, los documentos corruptos y el mensaje de Emilia se completan. Lukas recupera primero el nombre de Emilia y luego el suyo. Sin ellos todo sigue corrupto y no recupera ninguno.
3. Llega el Despertar y el latido del laboratorio se acelera. Ocurre en todos los finales de victoria.
4. El jugador toma la decisión final y se reproduce la cinemática de victoria.
5. Queda un gancho totalmente abierto, en el que solo se oye una nueva llegada y la voz de The Mother iniciando el protocolo.

**Decisión final.** Tras la fase 3 y el Despertar, la sala ofrece dos elementos, la terminal central y la salida. No hay menú. Ambos se usan con `E`, la misma función de interacción de siempre, así que ninguna tecla tiene dos funciones aquí. Si los dos están al alcance, gana el que esté en el centro de la vista. Sin los 6 documentos de Emilia no hay elección y Lukas examina la terminal de forma automática. Con ellos, el jugador elige.

| Condición | Momento final | Cinemática | Resultado |
| --- | --- | --- | --- |
| Sin los 6 documentos | Lukas examina la terminal central de forma automática | Relevo básico | Algo ocurre y Lukas reemplaza a The Mother. El terminal del brazo lo conecta porque es el último con identidad. |
| Con los 6 documentos, examinar la terminal | El jugador examina la terminal central con `E` | Relevo completo | Igual que el básico, más el expediente R-H-01 a R-H-06 con `estado desconocido` y el segundo acto. Al conectarse, Lukas oye el latido y lo entiende, mientras el jugador solo ve un fallo ilegible. Lukas reemplaza a The Mother sabiendo más. |
| Con los 6 documentos, irse | El jugador elige la salida con `E` | Ambiguo | En negro, el latido de Lukas y el del laboratorio se sincronizan en uno y luego hay silencio. No se sabe si Lukas fue asimilado, si se convirtió en The Mother o si pasó otra cosa. |

Asimilación no aparece en esta tabla porque depende del contador de muertes. La demo no tiene final de Renuncia, y los finales *Salida* y *Encuentro con Emilia* quedan fuera.

**Entrada:** Las de vigilancia, combate, resistencia y hackeo, más `E` sobre la terminal central o la salida.

**Estado inicial:** Lukas en la sala con el inventario que traía.

**Estado final:** The Mother derrotada y una cinemática de victoria, que puede ser Relevo básico, Relevo completo o Ambiguo. También puede terminar con la muerte de Lukas.

**Recursos utilizados:** Munición, curas, resistencia, fallas y documentos de Emilia registrados.

**Recompensa:** La victoria, la revelación y, con los 6 documentos de Emilia, la elección del final.

**Penalización:** Morir devuelve al inicio del piso 3.

**Interacciones con otras mecánicas:** Vista de vigilancia, Combate a distancia, Salud y estados, Resistencia, Hackeo, Interacción y documentos, Señales diegéticas, Exploración vertical, Reaparición, Fallas de la máquina y Reciclaje de enemigos.

**Casos límite:**

- El sexto documento de Emilia cuenta para la decisión aunque se consiga en el mismo intento en que cae The Mother.
- Los documentos de Vitalis Konzern y de The Mother no cuentan para la decisión.
- Durante la decisión hay presión sin muerte. Suben los altavoces y el latido, la sala tiembla y las luces fallan, sin daño ni enemigos.

### 3.12 Reciclaje de enemigos y remate

**Propósito:** Que eliminar a un enemigo no sea definitivo ni gratis. El jugador decide si paga tiempo, ruido o una casilla para evitar el reciclaje.

**Activación:** El reciclaje es automático al morir un enemigo. El jugador inicia el remate sobre el cuerpo antes de que se levante.

**Reglas:**

- Un enemigo eliminado sigue muerto durante el intento, pero el laboratorio recicla todos los cuerpos no rematados, cada uno una sola vez.
- **Rastro de identidad.** Los enemigos comunes son personas asimiladas por el laboratorio. Al morir conservan un último rastro de identidad, que el laboratorio consume para terminar de asimilar el cuerpo.
- El cuerpo se levanta más asimilado, más rápido y más frágil. Como ya no le queda identidad, no vuelve a reciclarse.
- The Mother no interviene. Ella reconstruye a quien conserva identidad, y el laboratorio asimila.
- **Aviso.** Unos ~40 s después de morir, el cuerpo se convulsiona y cruje durante ~3 s.
- **Reaparición.** Luego se levanta donde cayó, solo cuando Lukas no lo ve, porque quedó fuera de cuadro o detrás de una pared. Si Lukas lo sigue mirando, espera como mucho ~20 s más y se levanta igual. Mientras espera sigue convulsionándose y se puede rematar.
- **Enemigo reciclado.** Es un 25% más rápido, pero muere con 2 disparos, o con 1 si es un Rastrero. No vuelve a reciclarse.
- En la sala de The Mother no hay reciclaje.
- **Remate.** Evita el reciclaje de un cuerpo y solo se puede hacer mientras el cuerpo siga en la sala. Hay dos formas.

| Remate | Costo | Tiempo | Ruido | Entrada |
| --- | --- | --- | --- | --- |
| Golpe en el suelo | 25% de resistencia | ~2 s | Alerta a enemigos cercanos | `Clic izquierdo` sin apuntar sobre un cuerpo |
| Punzón de descarga | El punzón, que deja a Lukas sin defensa ante el próximo agarre | ~1,5 s | Ninguno | `E` sobre un cuerpo |

- **Disponibilidad.** El remate se ofrece a ~1,5 m mirando un cuerpo no rematado, también en pleno combate con enemigos vivos cerca.
- Si junto al cuerpo hay una puerta, un panel, una escalera, un elevador o un objeto, gana lo que esté en el centro de la vista. Con un cuerpo ya rematado, el remate no se ofrece.
- El golpe en el suelo no protege del daño.
- **Objeto único.** El remate silencioso y la defensa contra agarres usan el mismo punzón de descarga.
- El punzón escasea según el presupuesto por piso de [[06 - Economía y Recursos]].

![Aviso de remate frente a un cuerpo que convulsiona|520](../Media/Capturas/RemateCuerpo.png)

**Entrada:** `Clic izquierdo` sin apuntar sobre un cuerpo para el golpe en el suelo y `E` para el punzón. Ver [[04 - Controles]].

**Estado inicial:** Enemigo eliminado con su cuerpo en la sala.

**Estado final:** Cuerpo rematado, o enemigo reciclado que se levanta donde cayó cuando Lukas deja de verlo.

**Recursos utilizados:** Resistencia o un punzón, más tiempo y exposición.

**Recompensa:** Una amenaza menos en el piso durante el intento.

**Penalización:** El golpe expone a Lukas ~2 s, gasta resistencia y alerta a los enemigos cercanos. No rematar devuelve un enemigo más rápido.

**Interacciones con otras mecánicas:** Resistencia y empujón, Inventario, Sigilo y ruido, Punzón de descarga, Señales diegéticas, Interacción, Escape de combate y Jefe final.

**Casos límite:**

- Un golpe recibido durante el golpe en el suelo cancela el remate y el cuerpo puede reciclarse.
- Sin resistencia suficiente o en agotamiento, el golpe en el suelo no sale y Lukas jadea.
- Sin punzón, `E` sobre un cuerpo no hace nada.
- Un cuerpo nunca bloquea la ruta garantizada, porque basta con centrar la vista en el otro elemento. Así se cumple el principio 9 de [[01 - Jugabilidad]].
- Si Lukas sale de la sala durante el aviso, el cuerpo se levanta allí y lo espera.

## 4. Prioridad, pilares y riesgos

Las prioridades son *Demo*, obligatoria para la demo, y *Deseable*, que se implementa si el alcance lo permite. Los pilares son Vulnerabilidad constante (V), Adaptación (A) y Decisiones con consecuencias (DC). Ver [[04 - Pilares de Diseño]].

| Mecánica | Prioridad | Pilares | Riesgo | Respuesta |
| --- | --- | --- | --- | --- |
| Cámara sobre el hombro y apuntado | Demo | V | Golpes que parecen invisibles | Señales diegéticas de cercanía |
| Vista de vigilancia | Demo | V, A, DC | Eliminar la incertidumbre por uso frecuente | Ruido, retraso, degradación en búsqueda, atención de The Mother y salas sin cámara |
| Bloqueo de vigilancia por detección | Demo | V | Que la detección pierda peso con un desbloqueo rápido | Balancear el tiempo para salir de la caza |
| Combate a distancia | Demo | V, DC | Volverse un juego de acción con munición abundante | Munición escasa obligatoria |
| Salud y estados | Demo | V, DC | Que el daño deje de importar | Curas escasas |
| Resistencia y empujón | Demo | V, DC | Que el empujón barato resuelva todo | Costo alto del empujón |
| Inventario de 6 casillas | Demo | DC | Que el tiempo real no pese | Ninguna zona totalmente segura |
| Interacción y documentos | Demo | A | Romper el ritmo con lecturas largas o bloquear al jugador | Textos breves y corrupción que nunca tapa rutas ni reglas |
| Sigilo y ruido | Demo | V, A | Detección que parece injusta | Señales diegéticas claras |
| Señales diegéticas | Demo | A | Latido usado como radar o confundido con el del laboratorio | Intensidad aproximada, timbre distinto y latido del laboratorio sin relación con la cercanía |
| Punzón de descarga | Demo | V, DC | Que el agarre deje de asustar y el reciclaje pierda peso | Distribución escasa |
| Linterna | Demo | V, DC | Que quede siempre encendida y el dilema desaparezca | Oscuridad no tan densa |
| Hackeo por secuencias | Demo | V, DC | Frustración en lugar de tensión | Secuencias cortas |
| Escape de combate y retirada | Demo | DC | Que huir sea siempre seguro | Persecución breve |
| Exploración vertical por pisos | Demo | A, DC | Sala final cerca del inicio | Distancia mínima de 4 salas, o 3 en showcase |
| Reaparición por reconstrucción | Demo | A | Que el conocimiento resuelva los pisos | Persisten reglas, no valores |
| Fallas de la máquina | Deseable | A, DC | Volver más fuerte al jugador | Parche obligatorio |
| Degradación por muerte | Deseable | A, V | Ocultar información crítica o señales | Tope visual |
| Jefe final: The Mother | Demo | V, A, DC | Que la fase 3 se vuelva acción | Fase corta |
| Reciclaje de enemigos y remate | Demo | V, DC | Desgaste sin fin o inventario saturado | Reciclaje único y punzón escaso |

## 5. Dependencias entre mecánicas

| Mecánica | Depende de | Afecta a |
| --- | --- | --- |
| Cámara sobre el hombro | Ninguna | Vigilancia, combate, resistencia y linterna |
| Vista de vigilancia | Cámara, bloqueo, señales y pisos | Sigilo y jefe |
| Bloqueo de vigilancia | Sigilo y señales | Vigilancia y escape |
| Combate a distancia | Cámara e inventario | Sigilo, jefe y reciclaje |
| Salud y estados | Inventario | Punzón y reaparición |
| Resistencia y empujón | Ninguna | Sigilo, escape, jefe y reciclaje |
| Inventario | Ninguna | Combate, salud, punzón, pisos, reaparición y reciclaje |
| Interacción y documentos | Inventario | Pisos, reaparición, fallas, jefe y reciclaje |
| Sigilo y ruido | Vigilancia, resistencia, linterna y reciclaje | Bloqueo y señales |
| Señales diegéticas | Sigilo, degradación y jefe | Vigilancia, bloqueo, pisos, degradación, jefe y reciclaje |
| Punzón de descarga | Inventario | Bloqueo, salud y reciclaje |
| Linterna | Cámara | Bloqueo y sigilo |
| Hackeo | Fallas | Escape, pisos y jefe |
| Escape de combate | Resistencia, hackeo y reciclaje | Bloqueo e inventario |
| Exploración vertical | Inventario, interacción, señales y hackeo | Vigilancia, degradación y jefe |
| Reaparición por reconstrucción | Salud y degradación | Inventario, fallas, degradación y jefe |
| Fallas de la máquina | Interacción y reaparición | Hackeo y jefe |
| Degradación por muerte | Pisos y reaparición | Señales y reaparición |
| Jefe final | Vigilancia, combate, resistencia, interacción, señales, hackeo, pisos, reaparición, fallas y reciclaje | Señales y reaparición |
| Reciclaje y remate | Combate, resistencia, inventario, interacción y señales | Sigilo, escape y jefe |

## 6. Fuera de la demo

| Mecánica | Motivo | Pilar |
| --- | --- | --- |
| Mover cajas para crear coberturas, con ruido al arrastrarlas | Aumenta el alcance de la IA y la física | V, A |
| Saquear cadáveres | Requiere un sistema de botín | DC |
| Pasarelas internas | Complica la generación vertical | A |
| Techos rotos para bajar entre pisos | Complica la garantía de rutas válidas | A |
| Detección o luz a través de rejillas y ventanas entre salas | Requiere percepción entre salas | V |
| Detección del reflejo de la linterna en el suelo | Requiere muestrear la luz fuera del cuerpo del enemigo | V |
| Modelo propio para cada enemigo reciclado | No hay tiempo de modelado antes de la entrega. El reciclado se distingue por su temblor y su velocidad | A |
| Cámaras que mienten, como elemento de horror cósmico | Aumenta el alcance y complica la lectura de la vigilancia, contra el principio 3 de [[01 - Jugabilidad]] | A |
| Reasignación del mando | La demo usa un mapa fijo de mando | Ninguno |

| Mecánica descartada | Motivo | Pilar afectado |
| --- | --- | --- |
| Conos de visión enemigos visibles | Dan información completa y eliminan la incertidumbre | A |
| Esquiva con invulnerabilidad | Anula el castigo del daño, y el juego no tiene esquiva rápida | V |
| Liberación de agarre por secuencia con usos por vida | La reemplazó el punzón de descarga | V, DC |
| Opción para fijar siempre una perspectiva | Rompe el bloqueo de vigilancia | V |
| Regeneración de salud | Quita peso al daño y valor a las curas | V, DC |

> **Navegación:** [[00 - Índice]] · ← [[01 - Jugabilidad]] · [[03 - Sistemas]] →
