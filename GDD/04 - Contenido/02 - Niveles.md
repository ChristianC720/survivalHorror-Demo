# Niveles

## 1. Descripción de la sección

Cada nivel debe documentar su propósito jugable, estructura, dificultad, contenido y condiciones de entrada/salida.

## 2. Plantilla de nivel

En Identity Dissolution cada piso es un nivel, y la introducción y la sala de The Mother son niveles propios. Las reglas de estructura, llaves, energía, autoguardado y cierre de tránsito están en Exploración vertical por pisos, en [[02 - Mecánicas]]. Los presupuestos de recursos están en [[06 - Economía y Recursos]]. Las cantidades y duraciones de esta nota son valores iniciales sujetos a balance.

### 2.1 Introducción

**Objetivo principal:** Llegar al laboratorio siguiendo la nota de voz de Emilia. Es la misión Introducción de [[05 - Misiones y Desafíos]].

**Objetivos secundarios:** Ninguno.

**Duración estimada:** ~5 min.

**Dificultad:** Ninguna, porque la muerte final está guionizada.

**Mecánicas introducidas:** Moverse, mirar e interactuar, más un primer hackeo en el teléfono personal.

**Mecánicas utilizadas:** Las mismas, con HUD mínimo y una pausa sin `Reiniciar`.

**Enemigos / obstáculos:** La compuerta hackeable. El Mudo que sirve de señuelo y el atacante, que no se muestra, están guionizados.

**Recompensas:** Ninguna.

**Puntos de guardado:** Ninguno. La muerte final no suma al contador ni a la degradación.

**Condición de entrada:** `Nueva partida`.

**Condición de salida:** La muerte guionizada en el piso que no existe, seguida de la cinemática completa de reconstrucción.

**Eventos narrativos:** Casa de Lukas, foto, nota de voz de Emilia, compuerta, elevador que se distorsiona y cae, figura a lo lejos, ataque por la espalda e imágenes fugaces. El detalle está en [[01 - Premisa y Trama]].

**Mapa / diagrama:**

```text
Casa → Exterior de Sektor V-7 → Compuerta (hackeo) → Elevador (cinemática) → Piso que no existe → Muerte → Cinemática completa
```

### 2.2 Piso 1, mantenimiento

**Objetivo principal:** Conseguir la llave obligatoria y llegar a la sala clave final.

**Objetivos secundarios:** Los 2 documentos de Emilia del piso, más los documentos y objetos confiscados que aparezcan.

**Duración estimada:** ~12 a 15 min, más ~5 min de tutorial en el primer intento.

**Dificultad:** Baja, con enemigos lentos y el mayor presupuesto de recursos.

**Mecánicas introducidas:** El tutorial enseña todas las acciones del juego, entre indicaciones guiadas y carteles. El resto del piso pone en práctica vigilancia, hackeo, llaves, reciclaje y remate, energía y transición de piso, en el orden de [[05 - Progresión]].

**Mecánicas utilizadas:** Todas las de la demo salvo las cámaras hostiles del jefe.

**Enemigos / obstáculos:** 6 a 9 enemigos, entre el Alto y el Rastrero. Puertas con llave, paneles de hackeo y, a veces, la sala final sin energía.

**Recompensas:** El presupuesto del piso 1, más documentos y objetos confiscados que persisten en el terminal.

**Puntos de guardado:** Autoguardado en la sala clave final sin enemigos en caza. No reinicia el contador.

**Condición de entrada:** Reconstrucción en la fila de cápsulas, al inicio de cada ciclo o tras morir en el piso 1.

**Condición de salida:** Abrir la sala clave final con la llave, restaurar la energía si falta y usar la escalera o el elevador, con una cinemática de 5 s o más.

**Eventos narrativos:** La cinemática completa de reconstrucción, solo tras la introducción, o la corta. La bandeja de efectos personales con el revólver.

**Mapa / diagrama:**

```text
Fila de cápsulas → [Tutorial: depósito, túnel y calderas, solo el primer intento] → Salas procedurales ⇄ Salas clave secundarias (llave, documentos, interruptor) → Sala clave final → Piso 2
```

#### 2.2.1 Tutorial del primer intento

- El tutorial tiene 4 salas. La primera es la fila de cápsulas `KEY_F1_CapsuleRow`, que es la sala clave de inicio del piso 1 en todos los intentos.
- Las otras 3 son salas procedurales que el primer intento fija en este orden: `ROOM_F1_Storage_A`, `ROOM_F1_Corridor_A` y `ROOM_F1_Open_A`. Desde el segundo intento vuelven al reparto procedural.
- Sus recursos cuentan dentro del presupuesto del piso 1.
- Cada sala pide unas acciones guiadas, con su tecla en pantalla. La salida se abre al completarlas, o sola tras ~120 s para que nadie se quede atascado.
- Los carteles de seguridad de Vitalis de las paredes enseñan el resto de las acciones, con la tecla de cada una. Ver [[04 - Interfaz Visual]]. Entre carteles e indicaciones se enseñan todas las acciones del juego.
- Los carteles solo aparecen en el primer intento. Los textos están en [[05 - Textos del Juego]].
- Lukas no puede morir en estas 4 salas. Los golpes y agarres hacen daño, pero lo dejan como mucho al borde de Crítico.

| Sala | Arquetipo | Acciones guiadas | Carteles | Contenido |
| --- | --- | --- | --- | --- |
| 1. Fila de cápsulas | Sala clave de inicio, con cámara | Abrir el terminal del brazo | Interactuar, terminal y pausa | Bandeja de efectos personales con el anillo y 6 balas, y con el revólver desde la segunda reconstrucción |
| 2. Depósito a oscuras | Almacén, sin energía | Encender la linterna y recoger un objeto | Inventario, recoger, usar y descartar objetos, y curarse | Cadáver de un guardia de Kessler ya asimilado con el revólver, 1 blíster de munición y 1 cura |
| 3. Túnel largo | Pasillo | Correr | Correr y resistencia, vigilancia y hackeo | Al final se oye al Rastrero tras una puerta |
| 4. Sala de calderas | Sala abierta | Agacharse, apuntar y atacar | Sigilo, empujón, apuntar, disparar, recargar y remate | 1 Rastrero real que hace daño pero no puede matar. Cuenta dentro de los 6 a 9 enemigos del piso |

![Plano del tutorial del piso 1 con sus 4 salas|520](../Media/Planos/TutorialPiso1.png)

### 2.3 Piso 2, clínica ilegal

**Objetivo principal:** Conseguir la llave obligatoria y llegar a la sala clave final.

**Objetivos secundarios:** Los 2 documentos de Emilia del piso, más los documentos y objetos confiscados que aparezcan.

**Duración estimada:** ~12 a 15 min.

**Dificultad:** Media, con más enemigos y menos presupuesto que el piso 1.

**Mecánicas introducidas:** El grito de la Boca.

**Mecánicas utilizadas:** Todas las del piso 1.

**Enemigos / obstáculos:** 9 a 12 enemigos, entre el Alto, el Rastrero y la Boca. Puertas con llave, paneles de hackeo y, a veces, la sala final sin energía.

**Recompensas:** El presupuesto del piso 2 y el reinicio del contador de 5 muertes.

**Puntos de guardado:** Autoguardado en la sala clave final, que es la única sala que reinicia el contador.

**Condición de entrada:** La transición desde la sala clave final del piso 1.

**Condición de salida:** La misma que en el piso 1.

**Eventos narrativos:** La primera vez, la cinemática del hallazgo de la foto imposible. No se puede saltar y espera a que termine la caza.

**Mapa / diagrama:**

```text
Llegada → Salas procedurales ⇄ Salas clave secundarias → Sala clave final (foto + contador) → Piso 3
```

### 2.4 Piso 3, núcleo

**Objetivo principal:** Conseguir la llave obligatoria y llegar a la sala de The Mother.

**Objetivos secundarios:** Los 2 documentos de Emilia del piso, más los documentos y objetos confiscados que aparezcan.

**Duración estimada:** ~12 a 15 min.

**Dificultad:** Alta, con la mayor densidad de enemigos y el menor presupuesto.

**Mecánicas introducidas:** El Mudo como enemigo jugable.

**Mecánicas utilizadas:** Todas.

**Enemigos / obstáculos:** 12 a 15 enemigos de los 4 tipos. Nunca empieza sin energía, así que la puerta de The Mother solo exige la llave.

**Recompensas:** El presupuesto del piso 3.

**Puntos de guardado:** Ninguno.

**Condición de entrada:** La transición desde la sala clave final del piso 2.

**Condición de salida:** Abrir la puerta de la sala de The Mother con la llave obligatoria.

**Eventos narrativos:** Los registros del incidente en los documentos del piso.

**Mapa / diagrama:**

```text
Llegada → Salas procedurales ⇄ Salas clave secundarias → Puerta con llave → Sala de The Mother
```

### 2.5 Sala de The Mother

**Objetivo principal:** Derrotar a The Mother.

**Objetivos secundarios:** Ninguno. Con los 6 documentos de Emilia, elegir el final.

**Duración estimada:** ~3 a 6 min, en 3 fases de 1 a 2 min.

**Dificultad:** La más alta. El brazo recolector solo mata en un segundo agarre en Crítico, y los asimilados de la fase 2 sí pueden matar.

**Mecánicas introducidas:** Las cámaras hostiles.

**Mecánicas utilizadas:** Hackeo, agarre, señales, huida y disparo a puntos débiles.

**Enemigos / obstáculos:** Las 3 fases de Jefe final, en [[02 - Mecánicas]].

| Fase | Contenido |
| --- | --- |
| 1 | Hackeo de 3 nodos en las mesas de mezcla mientras el brazo recolector patrulla |
| 2 | Cámaras hostiles que liberan 4 asimilados, 2 Altos y 2 Rastreros, con un máximo de 2 activos. Niebla criogénica solo visual. Termina al eliminar a los 4 y llegar al núcleo |
| 3 | El núcleo criogénico expone 3 transformadores de 2 disparos cada uno. Al destruir el último, The Mother cae |

**Recompensas:** 20 balas fijas en la sala, 12 en los puntos ciegos para la fase 2 y 8 junto al núcleo para la fase 3. Además, 0 o 1 curas.

**Puntos de guardado:** Ninguno. Morir devuelve al inicio del piso 3.

**Condición de entrada:** Abrir la puerta con la llave obligatoria del piso 3.

**Condición de salida:** Derrotar a The Mother y ver un final de victoria.

**Eventos narrativos:** Una línea de protocolo por fase, la revelación, el Despertar y la decisión final.

**Mapa / diagrama:**

```text
Entrada → Fase 1 (nodos) → Fase 2 (cámaras + 4 asimilados → núcleo) → Fase 3 (3 transformadores) → Caída → Despertar → Decisión final o Relevo básico
```

## 3. Flujo del nivel

Flujo común de los pisos 1 a 3:

```text
Llegada → Salas procedurales → Salas clave secundarias (llave, documentos de Emilia, interruptor si falta energía) → Puerta de la sala final (llave) → Energía (si falta) → Hackeo del elevador (si lo exige) → Escalera o elevador → Piso siguiente
```

- La energía de la sala final sigue las reglas de Exploración vertical por pisos, en [[02 - Mecánicas]].
- El generador elige la sala clave secundaria del interruptor y garantiza la ruta hasta él, según el principio 9 de [[01 - Jugabilidad]].
- Entre pisos solo se pasa por la sala clave final. No hay rutas alternativas entre pisos.

## 4. Criterios de calidad

- Cada piso garantiza al menos una ruta válida hasta su sala clave final.
- El tutorial enseña todas las acciones del juego en salas donde Lukas no puede morir.
- La puerta cerrada, el panel sin energía y las líneas de protocolo comunican el objetivo sin marcadores.
- La dificultad sube de piso en piso con más enemigos, tipos nuevos y menos recursos.
- Cada cambio de alerta de un enemigo tiene una señal audible distinta.
- Tras morir, el jugador vuelve al inicio del piso con un escenario nuevo y el terminal intacto.

## 5. Salas procedurales: arquetipos y temas

Los pisos 1 a 3 usan 6 arquetipos jugables comunes que cada piso viste con su tema. La tercera parte de salas vacías no es un arquetipo. Es un estado que el generador aplica a cualquiera de ellos.

| Arquetipo | Función jugable | Mecánicas | Mantenimiento, piso 1 | Clínica ilegal, piso 2 | Núcleo, piso 3 |
| --- | --- | --- | --- | --- | --- |
| Pasillo | Conectar salas, huir y romper persecuciones | Resistencia y empujón, Señales diegéticas y Escape de combate | Túnel de tuberías | Corredor de pacientes | Galería de cableado |
| Almacén | Recursos, estanterías y sigilo | Inventario, Sigilo y ruido y Linterna | Depósito de herramientas | Farmacia y archivo de historiales | Almacén de viales |
| Sala abierta | Combate con 2 salidas | Combate a distancia, Resistencia y empujón y Escape de combate | Sala de calderas | Sala de espera | Sala de contención |
| Sala de vigilancia | Cámara con buena cobertura y lectura táctica | Vista de vigilancia y Bloqueo de vigilancia | Puesto de guardia de Kessler | Control de enfermería | Sala de monitores |
| Sala de hackeo | Puerta o terminal hackeable que abre un atajo | Hackeo por secuencias y Fallas de la máquina | Cuadro eléctrico | Cerradura de quirófano | Nodo de datos |
| Sala vertical | Conductos, pasarelas y desniveles dentro de la sala | Sigilo y ruido y Exploración vertical | Conductos de ventilación | Pozo de montacargas | Pasarelas sobre el núcleo |

- Cualquier arquetipo puede contener documentos, salvo los de Emilia, además de objetos confiscados y recursos del presupuesto.
- Las salas clave, que son las secundarias, las finales y la de The Mother, no son arquetipos. Son salas fijas que cambian de posición.
- Las pasarelas entre salas y los techos rotos quedan para después de la demo. La sala vertical solo usa desniveles dentro de la propia sala.

> **Navegación:** [[00 - Índice]] · ← [[01 - Estructura del Contenido]] · [[03 - Enemigos y NPC]] →
