# Jugabilidad

## 1. Descripción de la sección

La jugabilidad explica **qué hace el jugador, qué recibe a cambio y cómo evoluciona la experiencia**.

## 2. Bucle principal de juego

**Bucle del proyecto:**

`Explorar → Identificar amenazas y recursos → Combatir, evadir o interactuar → Descubrir información → Morir → Aplicar lo aprendido → Explorar`

Cada intento cambia en parte las circunstancias del jugador, pero lo aprendido en intentos anteriores sigue siendo útil. Los valores numéricos de este documento son iniciales y están sujetos a balance.

## 3. Acciones principales del jugador

| Acción | Propósito | Resultado |
|---|---|---|
| Explorar | Descubrir salas, rutas, recursos, amenazas e información | El jugador conoce el escenario y encuentra nuevas formas de avanzar |
| Moverse | Desplazarse y tomar posición en el entorno | Permite explorar, evitar peligros o prepararse para un enfrentamiento |
| Observar | Analizar el entorno antes de actuar | Revela amenazas, objetos, rutas y riesgos |
| Interactuar | Usar elementos del escenario | Entrega objetos, activa mecanismos, revela información o abre el paso |
| Recoger objetos | Obtener recursos útiles | El objeto pasa al inventario si hay espacio |
| Administrar el inventario | Decidir qué recursos hacen falta | El jugador elige qué conservar, usar o abandonar dentro de las 6 casillas |
| Usar objetos | Recuperarse, resolver situaciones o gastar recursos | El objeto da un beneficio inmediato y reduce lo disponible |
| Combatir | Defenderse o eliminar amenazas | Puede abrir el paso, pero gasta recursos y expone a Lukas al daño |
| Evadir | Evitar un enfrentamiento innecesario | Conserva recursos, aunque la amenaza sigue en el escenario |
| Escapar | Romper el contacto con una amenaza peligrosa | Reduce el riesgo inmediato sin garantizar que el peligro desaparezca |
| Investigar | Examinar elementos relevantes del entorno | Puede entregar información narrativa, de contexto o de reglas |
| Cambiar de estrategia | Adaptarse a los recursos y amenazas del intento | Permite responder distinto en cada intento |
| Avanzar | Llegar a nuevas zonas o cumplir objetivos | Acerca al jugador al objetivo del intento |
| Morir | Terminar el intento actual | Empieza un ciclo nuevo con otra configuración del escenario |

## 4. Experiencia segundo a segundo

En los primeros minutos, el jugador entiende su estado actual y empieza a leer el espacio que lo rodea. Sus acciones se concentran en:

- Moverse por el entorno.
- Controlar u observar la cámara según el contexto.
- Identificar rutas posibles.
- Examinar elementos del escenario.
- Detectar recursos.
- Reconocer amenazas.
- Decidir si interactúa, avanza o evita una zona.
- Familiarizarse con los controles principales.
- Entender las primeras reglas del inventario y de la interacción.

El juego no presenta muchos sistemas a la vez. El jugador aprende sobre todo al interactuar con el entorno y en situaciones controladas.

La sensación avanza en este orden:

`Desorientación → Observación → Comprensión básica → Primeras decisiones`

El primer contacto con una amenaza comunica que enfrentar a los enemigos de frente no siempre es la decisión correcta.

## 5. Experiencia minuto a minuto

Durante una sesión, el jugador alterna entre exploración, análisis, decisiones y momentos de tensión. Un tramo típico sigue esta estructura:

`Entrar a una zona → Observar → Explorar → Encontrar recursos o información → Detectar una amenaza → Evaluar opciones → Actuar → Gastar o conservar recursos → Continuar`

Las actividades que se repiten son:

- Explorar salas nuevas.
- Buscar rutas para avanzar.
- Encontrar recursos.
- Comparar objetos con el espacio libre del inventario.
- Revisar el estado del personaje.
- Escuchar o identificar señales de peligro.
- Evitar enemigos.
- Combatir cuando haga falta.
- Recuperarse después de un enfrentamiento.
- Investigar el entorno.
- Encontrar información narrativa.
- Buscar la ruta hacia el siguiente objetivo.
- Adaptarse a la distribución del intento.

El juego no mantiene al jugador en combate continuo. La tensión nace de la posibilidad de un enfrentamiento, no de enfrentamientos constantes.

## 6. Experiencia a largo plazo

El jugador quiere continuar por cuatro razones.

**Supervivencia.** Cada intento es el reto de llegar más lejos con menos recursos y mejores decisiones.

**Aprendizaje.** Aunque el escenario cambie, el jugador aprende sobre:

- El comportamiento de los enemigos.
- El valor de los recursos.
- El funcionamiento de los sistemas.
- Las situaciones peligrosas.
- Las estrategias de supervivencia.
- Los patrones del entorno.
- Las reglas de generación del escenario.

Ese conocimiento es una forma de progresión del jugador, aunque el personaje no reciba mejoras permanentes.

**Descubrimiento.** El jugador avanza para entender:

- Qué ocurre en el mundo.
- Qué le pasa al protagonista.
- Qué relación hay entre las muertes y los nuevos intentos.
- Qué representan las amenazas.
- Qué papel tiene la antagonista.

**Variación entre intentos.** La generación procedural produce suficiente variación para que ningún intento se resuelva solo con memoria. El jugador reconoce reglas y elementos familiares, pero no sabe con exactitud cómo se desarrollará cada intento.

## 7. Condición de victoria

Un intento exitoso exige sobrevivir al recorrido del escenario y completar un objetivo final.

La condición de victoria de la demo es **derrotar a The Mother**, el jefe final. Para llegar a ella, el jugador atraviesa los 3 pisos y alcanza su sala, que es una sala clave fija del piso 3 sin autoguardado. Cada piso combina salas procedurales, salas clave secundarias y una sala clave final con la escalera o el elevador al piso siguiente. Su composición en la configuración completa y en la de showcase está en [[02 - Mecánicas]].

El encuentro tiene 3 fases que reutilizan mecánicas existentes. Son el hackeo de nodos bajo presión, la supervivencia y huida, y unos pocos disparos a puntos débiles expuestos. Al caer, The Mother revela que existe algo más, la criatura lovecraftiana que es el laboratorio, pero no puede explicarlo.

Toda victoria sigue la misma secuencia:

1. Cae The Mother.
2. Solo si se reunieron los 6 documentos de Emilia, los documentos corruptos y el mensaje de Emilia se completan. Lukas recupera primero el nombre de Emilia y después el suyo.
3. Llega el **Despertar** y el latido del laboratorio se acelera.
4. Se reproduce la cinemática de victoria.
5. Todo cierra con un gancho abierto.

La cinemática depende de los **6 documentos de Emilia** acumulados en el terminal del brazo en cualquier intento. Si se tienen, también depende de la decisión final entre la terminal central y la salida, sin menú de elección. Las reglas están en [[02 - Mecánicas]] y los estados en [[07 - Reglas y Estados del Juego]].

| Condición | Cinemática de victoria | Resultado |
|---|---|---|
| Victoria sin los 6 documentos de Emilia | **Relevo básico** | Lukas examina la terminal central de forma automática y reemplaza a The Mother sin recordar ningún nombre |
| Victoria con los 6 documentos, el jugador examina la terminal central | **Relevo completo** | Lukas reemplaza a The Mother sabiendo más. Incluye el expediente R-H-01 a R-H-06 con `estado desconocido`. Lukas oye y entiende el segundo acto, que el jugador ve ilegible |
| Victoria con los 6 documentos, el jugador elige la salida | **Ambiguo** | El latido de Lukas y el del laboratorio se sincronizan en uno. No se sabe qué fue de Lukas |

Ningún final ofrece salvación. No existe final de Renuncia, y los finales *Salida* y *Encuentro con Emilia* quedan fuera de la demo. El único final de derrota es Asimilación, descrito en la condición de derrota.

Esta condición demuestra el ciclo completo de juego sin ampliar el alcance de la demo, porque el encuentro no introduce sistemas nuevos.

## 8. Condición de derrota

La condición principal de derrota es la muerte del personaje. Cuando la salud llega a su límite, el intento actual termina y ocurre lo siguiente:

1. Termina el recorrido actual.
2. Se activa el ciclo de muerte.
3. El escenario anterior deja de representar el recorrido actual.
4. Empieza un nuevo intento.
5. Se genera una nueva distribución del escenario.
6. El jugador vuelve a empezar apoyado sobre todo en lo que ya aprendió.

Otras condiciones de fracaso solo existirán si aportan valor al diseño.

Elegir `Reiniciar` en el menú de pausa también termina el intento y cuenta como muerte. Salir del juego durante una caza cuenta como muerte completa. Salir con calma fuera de una zona segura suma una muerte al contador y a la degradación, y al volver se continúa desde la sala clave final autoguardada. El detalle está en [[07 - Reglas y Estados del Juego]].

Existe además un final secundario de derrota. Si el jugador acumula 5 muertes consecutivas, se activa el final **Asimilación**, en el que el laboratorio termina de asimilar al protagonista. Solo llegar a la sala clave final del piso 2 reinicia el contador, que después sigue contando en el piso 3 y en la sala de The Mother. Las demás salas clave no lo reinician. Las muertes del primer intento, el del tutorial, no cuentan. `Reiniciar` y salir con costo suman al contador.

Cada muerte también provoca una degradación estética acumulativa, con más corrupción CRT y más distorsión del sonido vocal del protagonista. El texto de sus líneas solo se degrada en la cinemática de Asimilación. La degradación no afecta la jugabilidad y solo se acumula. Vuelve a 0 únicamente en el ciclo nuevo tras un final o con `Nueva partida`. El detalle está en [[02 - Mecánicas]].

Los estados de muerte, reconstrucción, victoria y `Final: Asimilación`, con sus transiciones, están en [[07 - Reglas y Estados del Juego]].

## 9. Ritmo

El ritmo alterna tensión y recuperación. La presión constante restaría impacto al terror y complicaría la administración de recursos. La partida alterna estos momentos.

**Exploración.** El jugador recorre el escenario, investiga rutas, examina objetos, busca recursos y obtiene información. La exploración mantiene una tensión moderada aunque no haya amenazas cerca.

**Acción.** Son los momentos de mayor intensidad, como combates, persecuciones, escapes y situaciones que exigen reaccionar rápido. Son breves y tienen peso.

**Descanso.** La presión baja por un tiempo. El jugador revisa el inventario, se recupera, procesa información y se prepara para avanzar. Descansar no significa estar a salvo.

**Desafío.** Son situaciones que exigen decidir o superar un obstáculo. Surgen de los enemigos, la escasez de recursos, las rutas bloqueadas, el riesgo del entorno, el inventario y los objetivos concretos.

**Recompensa.** Después de un riesgo, el jugador obtiene un beneficio proporcional. Puede ser un recurso, el acceso a otra zona, información narrativa, un objeto, una ruta más segura o un avance hacia el objetivo. No todo desafío da recompensas materiales.

**Historia.** La narrativa aparece sobre todo cuando el jugador puede prestar atención. Llega por el entorno, los documentos, los eventos, las líneas de texto de The Mother y de Lukas, los cambios de perspectiva, los elementos visuales y el ciclo de muerte. No hay diálogos. El ritmo narrativo evita interrumpir los momentos en que el jugador necesita concentrarse en sobrevivir.

## 10. Principios de interacción

**1. Toda acción tiene retroalimentación directa.** Cada acción del jugador recibe una respuesta perceptible. Puede ser visual, sonora, animada, de interfaz o un cambio en el escenario. El jugador nunca debería preguntarse si una interacción funcionó.

**2. Las interacciones parecidas se comportan igual.** Si dos elementos permiten el mismo tipo de interacción, usan las mismas reglas y los mismos controles. Así baja la carga mental y la dificultad no se confunde con falta de claridad.

**3. Los riesgos se pueden interpretar.** El jugador puede llevarse sorpresas, pero las situaciones decisivas no dependen de información imposible de conocer. Cuando es razonable, hay una señal de sonido, animación, iluminación, comportamiento del enemigo, diseño del entorno o interfaz.

**4. Las decisiones tienen consecuencias comprensibles.** Si el jugador gasta un recurso, pelea o abandona un objeto, después entiende qué efecto tuvo. La dificultad sale de elegir entre opciones limitadas, no de reglas ocultas.

**5. La cámara no quita el control al jugador.** La vista sobre el hombro es la predeterminada. La vista isométrica de vigilancia se activa a mano, tiene un costo y solo existe en salas con cámara activa. Los cambios entre ambas vistas son contextuales, predecibles, claros y útiles para la acción en curso. Ninguno desorienta ni cambia las reglas de control. Cuando la vigilancia se corta o se bloquea por daño o por un enemigo cercano en caza, el jugador vuelve a la vista sobre el hombro con control completo.

**6. La información es legible.** El jugador conoce con claridad el estado del personaje, el espacio libre del inventario, los recursos relevantes, el objeto seleccionado y las interacciones disponibles. La interfaz mantiene una estética de terror, pero nunca impide entender información crítica.

**7. Combatir es una decisión, no la respuesta.** Siempre que el escenario lo permite, el jugador puede evitar, rodear, escapar, conservar recursos, esperar o combatir. No todos los encuentros admiten todas las opciones, pero el diseño evita reducir el juego a eliminar enemigos.

**8. Cada intento enseña algo.** Una muerte deja un aprendizaje sobre un enemigo, una mecánica, un recurso, una situación, el escenario o la narrativa. El siguiente intento permite usar ese conocimiento, aunque el escenario no sea idéntico.

**9. La generación procedural respeta reglas de diseño.** El azar nunca produce situaciones que bloqueen la progresión. El generador garantiza una ruta válida, el acceso al objetivo, una distribución razonable de recursos, salas compatibles entre sí, situaciones superables y la ausencia de bloqueos imposibles.

> **Navegación:** [[00 - Índice]] · ← [[04 - Pilares de Diseño]] · [[02 - Mecánicas]] →
