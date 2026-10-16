# Niveles

## Descripción de la sección

Cada nivel debe documentar su propósito jugable, estructura, dificultad, contenido y condiciones de entrada/salida.

En Identity Dissolution cada **piso es un nivel**; la introducción y la sala de The Mother son niveles propios (D-69). Las reglas de estructura, llaves, autoguardado y persecución entre pisos están en [[02 - Mecánicas]] (M-15); los presupuestos de recursos, en [[06 - Economía y Recursos]]. Todas las cantidades y duraciones son valores iniciales, sujetos a balance.

## Salas procedurales: arquetipos y temas

Los pisos 1 a 3 se construyen con **6 arquetipos jugables** comunes que cada piso viste con su tema (D-69). El ~1/3 de salas vacías de D-37 no es un arquetipo: es un estado que el generador aplica a cualquiera de ellos.

| Arquetipo          | Función jugable                                     | Mecánicas        | Mantenimiento (piso 1)       | Clínica ilegal (piso 2)           | Núcleo (piso 3)           |
| ------------------ | --------------------------------------------------- | ---------------- | ---------------------------- | --------------------------------- | ------------------------- |
| Pasillo            | Conectar salas, huir y romper persecuciones         | M-06, M-10, M-14 | Túnel de tuberías            | Corredor de pacientes             | Galería de cableado       |
| Almacén            | Recursos, estanterías y sigilo                      | M-07, M-09, M-12 | Depósito de herramientas     | Farmacia y archivo de historiales | Almacén de viales         |
| Sala abierta       | Combate con 2 salidas                               | M-04, M-06, M-14 | Sala de calderas             | Sala de espera                    | Sala de contención        |
| Sala de vigilancia | Cámara con buena cobertura y lectura táctica        | M-02, M-03       | Puesto de guardia de Kessler | Control de enfermería             | Sala de monitores         |
| Sala de hackeo     | Puerta o terminal hackeable que abre un atajo       | M-13, M-17       | Cuadro eléctrico             | Cerradura de quirófano            | Nodo de datos             |
| Sala vertical      | Conductos, pasarelas y desniveles dentro de la sala | M-09, M-15       | Conductos de ventilación     | Pozo de montacargas               | Pasarelas sobre el núcleo |

- Cualquier arquetipo puede contener documentos (salvo los de Emilia), objetos confiscados y recursos del presupuesto (D-73, D-79).
- Las salas clave (secundarias, finales y la de The Mother) no son arquetipos: son salas fijas que cambian de posición (D-33).
- Las pasarelas internas entre salas y los techos rotos siguen siendo Post-demo (M-15); la sala vertical solo usa desniveles dentro de la propia sala.

## Plantilla de nivel

### Introducción

**Objetivo principal:** Llegar al laboratorio siguiendo la nota de voz de Emilia (MP-01, ver [[05 - Misiones y Desafíos]]).

**Objetivos secundarios:** Ninguno.

**Duración estimada:** ~5 min.

**Dificultad:** Ninguna; la muerte final está guionizada (D-63).

**Mecánicas introducidas:** Moverse, mirar e interactuar (M-01, M-08) y un primer hackeo en el teléfono personal (M-13, D-63).

**Mecánicas utilizadas:** Las mismas; HUD mínimo y pausa sin "Reiniciar" (D-63).

**Enemigos / obstáculos:** La compuerta hackeable. El Mudo que sirve de señuelo y el atacante, que no se muestra, son guionizados (D-53).

**Recompensas:** Ninguna.

**Puntos de guardado:** Ninguno. La muerte final no suma al contador ni a la degradación (D-63).

**Condición de entrada:** Nueva partida.

**Condición de salida:** Muerte guionizada en el piso que no existe; sigue la cinemática completa de reconstrucción (D-39).

**Eventos narrativos:** Casa de Lukas, foto, nota de voz programada de Emilia, compuerta, elevador que se distorsiona y cae (cinemática), figura a lo lejos, ataque por la espalda e imágenes fugaces (D-53; detalle en [[01 - Premisa y Trama]]).

**Mapa / diagrama:**

```text
Casa → Exterior de Sektor V-7 → Compuerta (hackeo) → Elevador (cinemática) → Piso que no existe → Muerte → Cinemática completa
```

### Piso 1 — Mantenimiento

**Objetivo principal:** Conseguir la llave obligatoria y llegar a la sala clave final (MP-02).

**Objetivos secundarios:** 2 documentos de Emilia; documentos y objetos confiscados del piso (MS-01, MS-02).

**Duración estimada:** ~12–15 min (+~5 min de tutorial en el primer intento).

**Dificultad:** Baja; enemigos lentos y el mayor presupuesto de recursos.

**Mecánicas introducidas:** Tutorial: interacción, terminal, inventario, linterna, correr, sigilo, empujón y disparo (D-70). Resto del piso: vigilancia, hackeo, llaves, reciclaje y remate, energía y transición de piso (orden en [[05 - Progresión]]).

**Mecánicas utilizadas:** Todas las de la demo salvo las cámaras hostiles del jefe.

**Enemigos / obstáculos:** 6–9 enemigos: el Alto y el Rastrero (D-76). Puertas con llave, paneles de hackeo y, a veces, la sala final sin energía (D-72).

**Recompensas:** Presupuesto del piso 1; documentos y objetos confiscados que persisten en el terminal.

**Puntos de guardado:** Autoguardado en la sala clave final sin enemigos en caza (D-30). No reinicia el contador.

**Condición de entrada:** Reconstrucción en la fila de cápsulas (inicio del ciclo, M-16).

**Condición de salida:** Abrir la sala clave final con la llave obligatoria, restaurar la energía si falta y usar la escalera o el elevador (cinemática de ≥5 s, D-39).

**Eventos narrativos:** Cinemática completa (solo tras la introducción) o corta de reconstrucción; bandeja de efectos personales con el revólver (D-74).

**Mapa / diagrama:**

```text
Cápsula → [Tutorial: 4 salas, solo el primer intento] → Salas procedurales ⇄ Salas clave secundarias (llave, documentos, interruptor) → Sala clave final → Piso 2
```

#### Tutorial (primer intento, D-70)

Las 4 salas son fijas solo en el primer intento y forman parte de las salas procedurales del piso 1; desde el segundo intento son procedurales (D-38). Sus recursos cuentan dentro del presupuesto del piso 1. Los controles se enseñan con carteles de seguridad de Vitalis en las paredes de estas salas, que incluyen la tecla de cada acción (D-89; ver [[04 - Interfaz Visual]]).

| Sala | Arquetipo | Enseña | Contenido |
|---|---|---|---|
| 1. Fila de cápsulas | — (sala de reconstrucción) | Interactuar y abrir el terminal del brazo | Anillo en el suelo (D-54); bandeja de efectos personales vacía |
| 2. Depósito a oscuras | Almacén | Linterna, recoger, inventario | Cadáver de un guardia de Kessler ya asimilado con el revólver; 1 blíster de munición y 1 cura |
| 3. Túnel largo | Pasillo | Correr y resistencia | Al final se oye al Rastrero tras una puerta |
| 4. Sala de calderas | Sala abierta | Sigilo, empujón y disparo | 1 Rastrero real (D-70): puede matar, cuenta dentro de los 6–9 enemigos del piso y su muerte no suma al contador (D-35) |

### Piso 2 — Clínica ilegal

**Objetivo principal:** Conseguir la llave obligatoria y llegar a la sala clave final (MP-03).

**Objetivos secundarios:** 2 documentos de Emilia; documentos y objetos confiscados del piso (MS-01, MS-02).

**Duración estimada:** ~12–15 min.

**Dificultad:** Media; más enemigos y menos presupuesto que el piso 1.

**Mecánicas introducidas:** El grito de la Boca (M-10).

**Mecánicas utilizadas:** Todas las del piso 1.

**Enemigos / obstáculos:** 9–12 enemigos: el Alto, el Rastrero y la Boca (D-76). Puertas con llave, paneles de hackeo y, a veces, la sala final sin energía (D-72).

**Recompensas:** Presupuesto del piso 2; reinicio del contador de 5 muertes.

**Puntos de guardado:** Autoguardado en la sala clave final (D-30); es la única sala que reinicia el contador (D-35).

**Condición de entrada:** Transición desde la sala clave final del piso 1.

**Condición de salida:** Igual que el piso 1.

**Eventos narrativos:** La primera vez, la cinemática no saltable del hallazgo de la foto imposible, que espera a que termine la caza (D-56, D-67).

**Mapa / diagrama:**

```text
Llegada → Salas procedurales ⇄ Salas clave secundarias → Sala clave final (foto + contador) → Piso 3
```

### Piso 3 — Núcleo

**Objetivo principal:** Conseguir la llave obligatoria y llegar a la sala de The Mother (MP-04).

**Objetivos secundarios:** 2 documentos de Emilia; documentos y objetos confiscados del piso (MS-01, MS-02).

**Duración estimada:** ~12–15 min.

**Dificultad:** Alta; la mayor densidad de enemigos y el menor presupuesto.

**Mecánicas introducidas:** El Mudo como enemigo jugable (M-09).

**Mecánicas utilizadas:** Todas.

**Enemigos / obstáculos:** 12–15 enemigos: los 4 tipos (D-76). Nunca empieza sin energía: la puerta de The Mother solo exige la llave (D-72).

**Recompensas:** Presupuesto del piso 3.

**Puntos de guardado:** Ninguno (D-30).

**Condición de entrada:** Transición desde la sala clave final del piso 2.

**Condición de salida:** Abrir la puerta de la sala de The Mother con la llave obligatoria (D-36).

**Eventos narrativos:** Registros del incidente en los documentos del piso (D-79).

**Mapa / diagrama:**

```text
Llegada → Salas procedurales ⇄ Salas clave secundarias → Puerta con llave → Sala de The Mother
```

### Sala de The Mother

**Objetivo principal:** Derrotar a The Mother (MP-05).

**Objetivos secundarios:** Ninguno; con los 6 documentos de Emilia, elegir el final (D-51).

**Duración estimada:** ~3–6 min (3 fases de 1–2 min, M-19).

**Dificultad:** La más alta; el brazo recolector puede matar en cualquier fase (D-65).

**Mecánicas introducidas:** Cámaras hostiles (D-28).

**Mecánicas utilizadas:** Hackeo, agarre, señales, huida y disparo a puntos débiles (M-19).

**Enemigos / obstáculos:**

| Fase | Contenido (D-65, D-80) |
|---|---|
| 1 | Hackeo de nodos en las mesas de mezcla mientras el brazo recolector patrulla |
| 2 | Cámaras hostiles: cada enfoque libera un asimilado (Alto o Rastrero) desde una cápsula del archivo. **4 en total**, máximo 2 activos. Niebla criogénica solo visual |
| 3 | El núcleo criogénico expone **3 transformadores** de 2 disparos cada uno; al destruir el último, The Mother cae |

**Recompensas:** **8 balas** fijas en la sala (6 para los transformadores + 2 de margen, D-28, D-80) y 0–1 curas.

**Puntos de guardado:** Ninguno; morir regresa al inicio del ciclo (D-30, M-16).

**Condición de entrada:** Abrir la puerta con la llave obligatoria del piso 3.

**Condición de salida:** Derrotar a The Mother y reproducir un final de victoria (D-51).

**Eventos narrativos:** Líneas de protocolo por fase (D-62), revelación, Despertar y decisión final (M-19).

**Mapa / diagrama:**

```text
Entrada → Fase 1 (nodos) → Fase 2 (cámaras + 4 asimilados) → Fase 3 (3 transformadores) → Caída → Despertar → Decisión final / Relevo básico
```

## Flujo del nivel

Flujo común de los pisos 1 a 3:

```text
Llegada → Salas procedurales → Salas clave secundarias (llave, documentos de Emilia, interruptor si falta energía) → Puerta de la sala final (llave) → Energía (si falta) → Hackeo del elevador (si lo exige) → Escalera o elevador → Piso siguiente
```

**Energía de la sala final (D-72):**

- Los pisos 1 y 2 tienen ~1 de cada 3 probabilidades de empezar con la sala clave final sin energía. No aplica al primer intento del piso 1 ni al piso 3.
- El interruptor está en una sala clave secundaria que elige el generador; la ruta hasta él está garantizada (principio 9 de [[01 - Jugabilidad]]).
- Restaurar la energía es una interacción (`E`) que emite un aviso de maquinaria audible y alerta a los enemigos cercanos (M-09).
- Entre pisos solo se pasa por la sala clave final: no existen rutas alternativas entre pisos (D-72).

## Criterios de calidad

- El objetivo es comprensible.
- Las reglas se pueden aprender.
- El desafío corresponde al nivel de progresión.
- Existe retroalimentación suficiente.
- El jugador entiende por qué tuvo éxito o fracaso.


> **Navegación:** [[00 - Índice]] · ← [[01 - Estructura del Contenido]] · [[03 - Enemigos y NPC]] →
