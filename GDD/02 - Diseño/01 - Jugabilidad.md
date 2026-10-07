# Jugabilidad

## Descripción de la sección

La jugabilidad explica **qué hace el jugador, qué recibe a cambio y cómo evoluciona la experiencia**.

## Bucle principal de juego

**Bucle del proyecto:**

`Explorar → Identificar amenazas y recursos → Combatir / Evadir / Interactuar → Descubrir información  → Morir  → Aplicar conocimiento adquirido → Explorar`

La intención es que cada intento cambie parcialmente las circunstancias del jugador, pero que el conocimiento obtenido anteriormente continúe siendo útil.

## Acciones principales del jugador
| Acción                 | Propósito                                                       | Resultado                                                                                          |
| ---------------------- | --------------------------------------------------------------- | -------------------------------------------------------------------------------------------------- |
| Explorar               | Descubrir habitaciones, rutas, recursos, amenazas e información | El jugador obtiene conocimiento del escenario y encuentra nuevas oportunidades de seguir avanzando |
| Moverse                | Desplazarse y posicionarse dentro del entorno                   | Permite explorar, evitar peligros o prepararse para un enfrentamiento                              |
| Observar               | Analizar el entorno antes de actuar                             | Permite identificar amenazas, objetos, rutas y posibles riesgos                                    |
| Interactuar            | Utilizar elementos del escenario                                | Permite obtener objetos, activar mecanismos, descubrir información o progresar                     |
| Recoger objetos        | Obtener recursos útiles                                         | Los objetos pasan al inventario si existe espacio disponible                                       |
| Administrar inventario | Identificar los recursos necesarios                             | El jugador decide qué conservar, usar o abandonar dentro del límite de 6 espacios                  |
| Utilizar objetos       | Recuperarse, resolver situaciones o consumir recursos           | El objeto produce un beneficio inmediato a cambio de reducir los recursos disponibles              |
| Combatir               | Defenderse o eliminar amenazas                                  | Puede permitir avanzar, pero consume recursos y expone al jugador a recibir daño                   |
| Evadir                 | Evitar un enfrentamiento innecesario                            | Conserva recursos, aunque la amenaza puede permanecer en el escenario                              |
| Escapar                | Romper contacto con una amenaza peligrosa                       | Reduce el riesgo inmediato sin garantizar que el peligro desaparezca                               |
| Investigar             | Examinar elementos relevantes del entorno                       | Puede entregar información narrativa, contextual o jugable                                         |
| Cambiar de estrategia  | Adaptarse a los recursos y amenazas encontradas                 | Permite responder de manera diferente a cada intento                                               |
| Avanzar                | Alcanzar nuevas áreas o cumplir objetivos                       | Acerca al jugador al objetivo del intento                                                          |
| Morir                  | Finalizar el intento actual                                     | Inicia un nuevo ciclo con una nueva configuración de escenario                                     |

## Experiencia segundo a segundo

¿Qué hace el jugador durante los primeros minutos?

Durante los primeros minutos de una partida, el jugador debe comprender rápidamente su estado actual y comenzar a interpretar el espacio que lo rodea.

Las acciones estarán centradas en:
- Moverse por el entorno.
- Controlar u observar la cámara según el contexto.
- Identificar posibles rutas.
- Examinar elementos del escenario.
- Detectar recursos.
- Reconocer amenazas.
- Decidir si interactuar, avanzar o evitar un área.
- Familiarizarse con los controles principales.
- Comprender las primeras reglas del inventario y de la interacción.

La intención es evitar presentar una cantidad excesiva de sistemas simultáneamente.

El jugador deberá aprender principalmente mediante interacción con el entorno y situaciones controladas durante los primeros minutos.

A nivel de sensación, el jugador debería pasar progresivamente de:

`Desorientación → Observación → Comprensión básica → Primeras decisiones

El primer contacto con una amenaza debe comunicar que enfrentarse directamente a los enemigos no siempre es la decisión correcta.

## Experiencia minuto a minuto

¿Qué actividades se repiten durante una sesión?

Durante una sesión, el jugador alternará repetidamente entre exploración, análisis, toma de decisiones y situaciones de tensión. Un segmento típico puede seguir aproximadamente esta estructura:

`Entrar a una zona → Observar → Explorar → Encontrar recursos o información → Detectar amenaza → Evaluar opciones → Actuar → Consumir o conservar recursos → Continuar`

Las actividades recurrentes incluyen:

- Explorar nuevas habitaciones.
- Buscar rutas para avanzar.
- Encontrar recursos.
- Comparar objetos contra el espacio disponible en el inventario.
- Revisar el estado actual del personaje.
- Escuchar o identificar señales de peligro.
- Evitar enemigos.
- Combatir cuando sea necesario.
- Recuperarse después de un enfrentamiento.
- Investigar el entorno.
- Encontrar información narrativa.
- Buscar la ruta hacia el siguiente objetivo.
- Adaptarse a la distribución de la partida.

El juego debe evitar mantener continuamente al jugador en combate.

La tensión debe construirse mediante la posibilidad de que ocurra un enfrentamiento, no únicamente mediante enfrentamientos constantes.

## Experiencia a largo plazo

¿Qué hace que el jugador quiera continuar?

El jugador debe querer continuar principalmente por cuatro razones:

### Supervivencia:
Cada intento representa el reto de avanzar más lejos utilizando la menor cantidad de recursos disponibles y tomando mejores decisiones.

### Aprendizaje:
Aunque el escenario pueda cambiar, el jugador adquiere conocimiento sobre:

- Comportamiento de enemigos.
- Valor de los recursos.
- Funcionamiento de sistemas.
- Situaciones peligrosas.
- Estrategias de supervivencia.
- Patrones del entorno.
- Reglas de generación del escenario.

Este conocimiento constituye una forma importante de progresión del jugador, incluso cuando el personaje no recibe necesariamente mejoras permanentes.

### Descubrimiento:

El jugador continuará avanzando para comprender:

- Qué ocurre en el mundo.
- Qué le sucede al protagonista.
- Qué relación existe entre las muertes y los nuevos intentos.
- Qué representan las amenazas encontradas.
- Qué papel desempeña el antagonista.

### Variación entre intentos:

La generación procedural deberá producir suficiente variación para evitar que cada partida pueda resolverse únicamente mediante memorización y el  jugador debe reconocer reglas y elementos familiares sin conocer exactamente cómo se desarrollará cada intento.

## Condición de victoria

¿Qué debe lograr el jugador para avanzar o terminar?

A nivel de jugabilidad, un intento exitoso deberá requerir que el jugador alcance y complete un objetivo final después de sobrevivir al recorrido del escenario.

La condición de victoria de la demo es **derrotar a The Mother**, el jefe final. Para llegar a ella, el jugador debe atravesar los 3 pisos del escenario y alcanzar su sala, que es una sala clave fija del piso 3 sin autoguardado (D-33). Cada piso combina salas procedurales, salas clave secundarias y una sala clave final con la escalera o el elevador al piso siguiente; su composición en la configuración completa y en la de showcase se documenta en [[02 - Mecánicas]] (M-15). El encuentro consta de 3 fases que reutilizan mecánicas existentes: hackeo de nodos bajo presión, supervivencia y huida, y pocos disparos a puntos débiles expuestos (ver [[02 - Mecánicas]]).

Al ser derrotada, The Mother revela que existe algo más (la criatura lovecraftiana que es el laboratorio), pero no puede explicarlo.

La condición seleccionada permite demostrar el ciclo completo de juego sin ampliar innecesariamente el alcance de la demo, ya que el encuentro no introduce sistemas nuevos.

## Condición de derrota

¿Qué situaciones provocan fracaso?

La principal condición de derrota es la muerte del personaje

Cuando la salud del personaje llegue al límite definido, el intento actual finaliza.

Como consecuencia:

1. Se termina la ejecución actual.
2. Se activa el ciclo de muerte.
3. El escenario anterior deja de representar el recorrido actual.
4. Se inicia un nuevo intento.
5. Se genera una nueva distribución del escenario.
6. El jugador vuelve a comenzar utilizando principalmente el conocimiento adquirido previamente.

Otras condiciones de fracaso podrán existir únicamente si aportan valor al diseño.

"Reiniciar" desde el menú de pausa también finaliza el intento y cuenta como muerte (D-32). Salir del juego durante una alerta o caza cuenta como muerte completa, y salir con calma fuera de una sala clave final ya guardada suma una muerte al contador y a la degradación, aunque al volver se continúa desde la sala clave final autoguardada (D-31). El detalle de estas reglas se documenta en [[07 - Reglas y Estados del Juego]].

Existe además un final secundario de derrota: si el jugador acumula 5 muertes sin llegar a la sala clave final del piso 2, se activa el final de "Disolución de identidad". Solo llegar a la sala clave final del piso 2 reinicia el contador; las demás salas clave no lo reinician. Las muertes del primer intento (el del tutorial) no cuentan, y "Reiniciar" y salir con costo suman al contador (D-35). Cada muerte también provoca una degradación estética acumulativa (mayor corrupción CRT, distorsión de voz y deterioro de los diálogos del protagonista) que no afecta la jugabilidad, solo se acumula y nunca se reinicia (D-29). El detalle de estas reglas se documenta en [[02 - Mecánicas]].

Los estados de muerte, reconstrucción, victoria y final de disolución, con sus transiciones, se documentan en [[07 - Reglas y Estados del Juego]].

## Ritmo

Describe cómo alternan:

- Exploración
- Acción
- Descanso
- Desafío
- Recompensa
- Historia

El ritmo debe alternar entre tensión y recuperación. Mantener al jugador bajo presión constante reduciría el impacto del terror y dificultaría la administración de recursos.

La estructura general deberá alternar según siguientes estados:
### Exploración

Periodo donde el jugador:

- Recorre el escenario.
- Investiga rutas.
- Examina objetos.
- Busca recursos.
- Obtiene información.

La exploración debe mantener una tensión moderada incluso cuando no existan amenazas inmediatas.

### Acción

Momentos de mayor intensidad.

Pueden incluir:

- Combate.
- Persecuciones.
- Escape.
- Situaciones donde sea necesario reaccionar rápidamente.

Deben ser relativamente breves y significativos.

### Descanso

Periodos donde la presión disminuye temporalmente.

Permiten:

- Revisar inventario.
- Recuperarse.
- Procesar información.
- Prepararse para avanzar.

Estos momentos no necesariamente deben significar seguridad absoluta.

### Desafío

Situaciones que requieren tomar una decisión o superar un obstáculo.

Pueden surgir mediante:

- Enemigos.
- Escasez de recursos.
- Rutas bloqueadas.
- Riesgo ambiental.
- Administración del inventario.
- Objetivos específicos.

### Recompensa

Después de asumir un riesgo, el jugador debería poder obtener algún beneficio proporcional.

Las recompensas pueden incluir:

- Recursos.
- Acceso a nuevas áreas.
- Información narrativa.
- Objetos.
- Una ruta más segura.
- Progreso hacia el objetivo principal.

No todos los desafíos deberán otorgar recompensas materiales.

### Historia

La narrativa deberá aparecer principalmente durante periodos donde el jugador pueda prestar atención.

Puede presentarse mediante:

- Entorno.
- Documentos.
- Eventos.
- Diálogos.
- Cambios de perspectiva.
- Elementos visuales.
- Situaciones relacionadas con el ciclo de muerte.

El ritmo narrativo deberá evitar interrumpir repetidamente situaciones donde el jugador necesita concentrarse en sobrevivir.

## Principios de interacción

¿Qué reglas debe seguir el juego para que las acciones del jugador sean claras, consistentes y predecibles?
### 1. Toda acción debe tener retroalimentación directa:

Cuando el jugador realice una acción, debe recibir una respuesta perceptible.
Puede ser:

- Visual.
- Auditiva.
- Animada.
- Mediante interfaz.
- Mediante cambios en el escenario.

El jugador nunca debería preguntarse constantemente si una interacción funcionó.

### 2. Interacciones similares deben comportarse de manera similar:

Si dos elementos permiten el mismo tipo de interacción, deben utilizar reglas y controles consistentes, esto reduce carga cognitiva y evita confundir dificultad con falta de claridad.

### 3. Los riesgos deben poder interpretarse:

El jugador puede ser sorprendido, pero las situaciones importantes no deben depender sistemáticamente de información imposible de conocer. Debe existir algún tipo de señal cuando sea razonable:

- Sonido.
- Animación.
- Iluminación.
- Comportamiento del enemigo.
- Diseño del entorno.
- Feedback de interfaz.

### 4. Las decisiones deben tener consecuencias comprensibles:

Si el jugador consume un recurso, combate o abandona un objeto, debe comprender posteriormente qué efecto produjo esa decisión. La dificultad debe surgir de elegir entre alternativas limitadas, no de reglas ocultas sin explicación.

### 5. La cámara no elimina el control del jugador:

La perspectiva Over the Shoulder es la vista por defecto. La vista isométrica de vigilancia se activa manualmente, tiene un costo y solo está disponible en salas con cámara activa. Los cambios entre ambas perspectivas deberán ser:

- Contextuales.
- Predecibles.
- Visualmente claros.
- Funcionales para la acción que se está realizando.

El cambio no debe desorientar al jugador ni modificar arbitrariamente las reglas de control. Cuando la vista de vigilancia se corta o se bloquea (al recibir daño o con algún enemigo cercano en caza; ver D-23 en [[02 - Mecánicas]]), el jugador regresa a Over the Shoulder con el control completo del personaje.

### 6. La información debe ser legible:

El jugador debe poder conocer con suficiente claridad:

- Estado del personaje.
- Espacio disponible en el inventario.
- Recursos relevantes.
- Objeto seleccionado.
- Interacciones disponibles.

La interfaz puede mantener una estética de terror, pero nunca debe impedir comprender información crítica.

### 7. Combatir debe ser una decisión, no la respuesta:

Siempre que el diseño del escenario lo permita, el jugador debería poder considerar alternativas como:

- Evitar.
- Rodear.
- Escapar.
- Conservar recursos.
- Esperar.
- Combatir.

No todos los encuentros necesitan admitir todas estas opciones, pero el diseño general debe evitar reducir el juego a eliminar enemigos.

### 8. El jugador debe aprender de cada intento:

Una muerte debe aportar algún tipo de aprendizaje:

- Sobre un enemigo.
- Sobre una mecánica.
- Sobre un recurso.
- Sobre una situación.
- Sobre el escenario.
- Sobre la narrativa.

El siguiente intento debe permitir utilizar ese conocimiento, aunque el escenario no sea idéntico.

### 9. La generación procedural debe respetar reglas de diseño:

La aleatoriedad nunca debe producir situaciones incompatibles con la progresión. El generador deberá garantizar condiciones como:

- Existencia de una ruta válida.
- Acceso al objetivo.
- Distribución razonable de recursos.
- Habitaciones compatibles entre sí.
- Situaciones superables.
- Ausencia de bloqueos imposibles.


> **Navegación:** [[00 - Índice]] · ← [[04 - Pilares de Diseño]] · [[02 - Mecánicas]] →
