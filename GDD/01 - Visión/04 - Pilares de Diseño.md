# Pilares de Diseño

## 1. Descripción de la sección

Los pilares son los principios que deben mantenerse durante todo el desarrollo. Cuando dos ideas entren en conflicto, los pilares ayudan a decidir cuál encaja mejor con la identidad del juego.

## 2. Pilar 1: Vulnerabilidad constante

**Descripción:** Lukas está en desventaja. El juego no lo convierte en una fuerza dominante. Lo obliga a sobrevivir con precaución, observación y buenas decisiones. Pelear es posible, pero rara vez es lo más seguro o eficiente.

**¿Cómo se refleja en el juego?**

- Un solo enemigo ya es una amenaza real. Los enemigos matan a Lukas en 2 a 4 golpes según su tipo.
- La salud no se regenera y las curas escasean.
- El inventario tiene 6 casillas.
- El jugador decide en cada encuentro si pelea, lo evita o huye.
- El revólver y los demás recursos ofensivos no borran el peligro. Enfrentar a más de dos enemigos a la vez es casi una muerte segura.
- El entorno mantiene la tensión aunque no haya enemigos a la vista.
- La progresión informa a Lukas, pero nunca lo vuelve más fuerte.

## 3. Pilar 2: Adaptación

**Descripción:** El jugador nunca sabe del todo qué encontrará. Las salas, los recursos y las amenazas cambian entre intentos. Esa incertidumbre genera tensión y lo obliga a decidir con información incompleta.

**¿Cómo se refleja en el juego?**

- La generación procedural reordena las salas en cada intento, así que una ruta conocida puede no repetirse.
- Morir lleva a un escenario distinto.
- La distribución de recursos cambia.
- El jugador explora para entender cada piso nuevo.
- Lo aprendido en intentos anteriores enseña las reglas, pero no permite memorizar el mapa.
- La generación produce situaciones jugables y coherentes, no azar sin propósito. Cada piso garantiza una ruta válida a su sala final.

## 4. Pilar 3: Decisiones con consecuencias

**Descripción:** Sobrevivir depende de lo que decide el jugador. Cada recurso que usa, cada objeto que carga, cada pelea y cada ruta trae una ventaja posible y un costo posible.

**¿Cómo se refleja en el juego?**

- Seis casillas obligan a elegir qué guardar.
- Gastar un recurso ahora puede resolver el momento y dejar a Lukas expuesto después.
- Pelear elimina una amenaza a cambio de munición, salud o resistencia.
- Esquivar ahorra recursos, pero deja el peligro vivo en el piso.
- Explorar una sala extra puede dar objetos útiles o nuevos problemas.
- Las consecuencias se entienden, así que el jugador aprende de sus errores.
- El jugador cambia de estrategia según los recursos y situaciones de cada intento.

![Inventario de 6 casillas en el terminal del brazo|520](../Media/Capturas/Inventario.png)

## 5. Reglas de decisión

Antes de incorporar una característica importante, preguntar:

- ¿Refuerza uno de nuestros pilares?
- ¿Mejora la experiencia principal?
- ¿Justifica el costo de desarrollo?
- ¿Complica innecesariamente otro sistema?

## 6. Principios complementarios

Los tres pilares tienen prioridad. Estos principios ayudan a mantener la identidad del proyecto.

### 6.1 La cámara tiene intención

Cada cambio de cámara responde a una necesidad jugable, narrativa o de presentación, nunca a un efecto visual sin propósito. La vista sobre el hombro es la predeterminada y da cercanía, precisión y tensión.

La isométrica ayuda a entender el espacio, pero cuesta. Son las cámaras de seguridad de The Mother vistas desde el terminal del brazo, y solo existen en salas con cámara activa. La imagen llega con ruido y retraso. Lukas debe quedarse quieto o caminar muy lento, y la vista se corta con el daño o cuando un enemigo cercano entra en caza. Al usarla, el jugador ve con los ojos de la máquina.

### 6.2 La muerte es parte de la experiencia

Morir no es solo una pantalla de derrota. Cierra un intento, forma parte del ciclo roguelite, cambia el escenario y deja que el jugador aproveche lo que aprendió.

### 6.3 El terror nace de los sistemas

El miedo no depende solo de sustos repentinos. También lo construyen la escasez, la incertidumbre, la vulnerabilidad, el diseño de sonido y la iluminación. Se suman las amenazas impredecibles, los espacios desconocidos y el peso de cada decisión.

> **Navegación:** [[00 - Índice]] · ← [[03 - Público y Plataforma]] · [[01 - Jugabilidad]] →
