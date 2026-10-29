# Pilares de Diseño

## Descripción de la sección

Los pilares son los principios que deben mantenerse durante todo el desarrollo. Cuando dos ideas entren en conflicto, los pilares ayudan a decidir cuál encaja mejor con la identidad del juego.

## Pilar 1 — Vulnerabilidad Constante

**Descripción:**
El jugador debe sentirse en desventaja, el objetivo no es convertirlo en una fuerza dominante, sino obligarlo a sobrevivir mediante precaución, observación, administración y toma de decisiones.

El combate debe ser una herramienta posible, pero no necesariamente la solución más segura o eficiente.

**¿Cómo se refleja en el juego?**

- Los enemigos pueden representar una amenaza real incluso en encuentros individuales.
- Los recursos disponibles son limitados.
- El inventario está restringido a 6 espacios.
- El jugador debe decidir cuándo combatir, evitar o escapar.
- Recibir daño debe tener consecuencias relevantes.
- Las armas y recursos ofensivos no deben eliminar completamente la sensación de peligro.
- El entorno debe mantener al jugador atento incluso cuando no existe un enemigo visible.
- La progresión no debe transformar al protagonista en un personaje poderoso.

## Pilar 2 — Adaptación

**Descripción:**
El jugador no debe tener control ni conocimiento completo de lo que encontrará durante una partida. Parte de la experiencia consiste en adaptarse a escenarios, recursos y amenazas que pueden variar entre intentos. Pero manteniendo la incertidumbre que debe generar tensión y obligar al jugador a tomar decisiones utilizando información incompleta.

**¿Cómo se refleja en el juego?**

- La distribución de los cuartos cambia entre partidas mediante generación procedural.
- Una ruta utilizada anteriormente no garantiza que vuelva a existir de la misma manera.
- La muerte provoca un nuevo intento dentro de un escenario diferente.
- La distribución de determinados recursos puede variar.
- El jugador debe explorar para comprender la estructura de cada nueva partida.
- El conocimiento adquirido en intentos anteriores ayuda a comprender las reglas del juego, pero no permite memorizar completamente el escenario.
- La generación procedural debe producir situaciones jugables coherentes y no aleatoriedad sin propósito.

## Pilar 3 — Decisiones con Consecuencias

**Descripción:**
La supervivencia debe depender principalmente de las decisiones del jugador. Cada recurso utilizado, objeto transportado, enfrentamiento iniciado o ruta explorada debe implicar una posible ventaja y un posible costo.


**¿Cómo se refleja en el juego?**

- El inventario de 6 espacios obliga a priorizar qué objetos conservar.
- Utilizar un recurso inmediatamente puede facilitar una situación actual, pero dejar al jugador vulnerable posteriormente.
- Combatir puede eliminar una amenaza, pero consumir recursos importantes.
- Evitar un enemigo puede conservar recursos, pero aumentar el riesgo durante la exploración.
- Explorar habitaciones adicionales puede proporcionar objetos útiles, pero también exponer al jugador a nuevas amenazas.
- Las decisiones deben tener consecuencias comprensibles para que el jugador pueda aprender de sus errores.
- El jugador debe poder cambiar su estrategia dependiendo de los recursos y situaciones encontrados durante cada partida.

## Reglas de decisión

Antes de incorporar una característica importante, preguntar:

- ¿Refuerza uno de nuestros pilares?
- ¿Mejora la experiencia principal?
- ¿Justifica el costo de desarrollo?
- ¿Complica innecesariamente otro sistema?
## Principios complementarios extra a considerar:

Aunque los tres pilares anteriores tienen prioridad, existen principios que ayudan a mantener la identidad del proyecto.
### La cámara debe tener intención:

El cambio entre perspectiva isométrica y Over the Shoulder no debe utilizarse únicamente como efecto visual.

Cada cambio deberá responder a una necesidad jugable, narrativa o de presentación concreta.

La perspectiva sobre el hombro es la vista por defecto y mantiene la cercanía, precisión, tensión y participación directa del jugador. La cámara isométrica favorece la percepción del entorno y la observación del espacio, pero es una acción manual de reconocimiento con costo: representa el acceso a las cámaras de seguridad de The Mother desde el terminal del brazo, solo existe en salas con cámara activa, muestra una imagen con ruido y retraso, obliga a permanecer quieto o caminar muy lento y se corta al recibir daño o cuando algún enemigo cercano entra en caza (D-23). Al usarla, el jugador ve por los ojos de la máquina.

### La muerte forma parte de la experiencia:

La muerte no debe sentirse únicamente como una pantalla de derrota.

Debe cerrar un intento y formar parte del ciclo Roguelite del videojuego, provocando cambios en el escenario y permitiendo que el jugador utilice el conocimiento adquirido anteriormente.

### El terror debe surgir de los sistemas:

La experiencia de terror no debe depender exclusivamente de sustos repentinos.
Debe construirse también mediante:

- Escasez de recursos.
- Incertidumbre.
- Vulnerabilidad.
- Diseño de sonido.
- Iluminación.
- Amenazas impredecibles.
- Exploración de espacios desconocidos.
- Consecuencias de las decisiones.

> **Navegación:** [[00 - Índice]] · ← [[03 - Público y Plataforma]] · [[01 - Jugabilidad]] →
> 