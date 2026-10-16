# Personajes y Animación

## Descripción de la sección

Documenta cómo deben verse y comportarse visualmente los personajes.

Aspecto, rol y valores de cada personaje en [[03 - Personajes]] y [[03 - Enemigos y NPC]]; paleta y reglas visuales en [[01 - Dirección de Arte]]. Los conteos de clips son valores iniciales, sujetos a producción.

## Requisitos por personaje

| Personaje | Modelo/Sprite | Animaciones | VFX | Estado |
| --------- | ------------- | ----------- | --- | ------ |
| Lukas civil (introducción) | Modelo 3D low-poly; chaqueta de campo verde-gris, sin emisivos; anillo en la mano derecha (D-83) | Locomoción, interacción, hackeo con el teléfono, muerte guionizada (~8 clips) | Ninguno | Diseño de referencia |
| Lukas R-H-07 | Mismo cuerpo; overol teal con costuras luminosas y ribetes rojos, terminal en el antebrazo izquierdo; 3–5 variantes de cuerpo ajeno (D-43) | Juego completo (~25–30 clips) | Emisivo de costuras y terminal; destello del punzón | Diseño de referencia |
| El Alto | Humanoide de ~2.5 m, encorvado, brazos hasta el suelo; restos de uniforme | Patrulla, caza, golpe largo, agarre, aturdido, muerte (~8–10 clips) | Brillo morado iridiscente en la piel | Concepto |
| La Boca | Humanoide, cara partida con boca vertical | Deambular, caza, mordida, grito, aturdido, muerte (~8–10 clips) | Ondas de distorsión en el grito (sutiles) | Concepto |
| El Rastrero | Cuadrúpedo, cara invertida | Locomoción procedural con IK, flanqueo, zarpazos, muerte (~6–8 clips) | — | Concepto |
| El Mudo | Humanoide sin cara; pose de cadáver o de persona quieta | Inmóvil, estallido, agarre, pausa, muerte (~6–8 clips) | — | Concepto |
| Versiones recicladas | Las mismas mallas con más rasgos del laboratorio (texturas y piezas añadidas) | Las de su tipo, más rápidas | Más morado y carne expuesta | Concepto |
| The Mother | Estantería criogénica en una grúa con brazo recolector, núcleo y 3 transformadores; solo máquina (D-65) | Patrulla de la grúa, agarre del brazo, apertura del núcleo, caída (~6–8 clips + procedural) | Niebla criogénica, chispas de transformadores, luz roja de cámaras | Concepto |
| Cadáveres asimilados | Variantes de personal de Vitalis, guardias de Kessler y sujetos (D-77) | Ragdoll y poses fijas | — | Concepto |
| Lukas Mudo (Asimilación) | Malla del Mudo con piezas de Lukas (D-61) | Solo cinemática | — | Concepto |
| Emilia | Sin modelo 3D: solo la foto de la casa y la foto imposible con la cara borrosa (D-56) | — | Glitch en la foto | Concepto |

## Animaciones

**Flujo de producción (D-88):** el equipo modela, crea el esqueleto, hace el rig y anima en Blender. Solo la **locomoción base** se toma de Mixamo y se retoca en Blender; todo lo demás es propio. En Unity, con las herramientas incluidas en el motor (Animator, *Animation Rigging*, IK y ragdoll), el código mezcla y modifica los clips:

- Gira el torso, los brazos y la cabeza hacia donde se apunta o se mira.
- Apoya los pies en escalones y escombros (IK) y coloca la mano en puertas, paneles y el arma.
- Añade temblores, espasmos y movimientos rotos a los asimilados con ruido sobre los huesos.
- Mueve las patas del Rastrero con IK procedural.
- Usa ragdoll en los cuerpos.
- Mezcla capas (por ejemplo, caminar abajo y recargar arriba).

Las acciones con intención y las sincronizadas entre dos personajes (agarres, remates) siempre son clips hechos en Blender. **La locomoción de Lukas es la primera prioridad de producción**: si se ve mal, se nota en todo el juego.

### Movimiento

- **Lukas:** idle (quieto y respirando; cambia con el estado de salud), caminar, correr, agachado y caminar agachado, apuntar caminando, girar. La postura cambia con la salud: erguido en Bien, encorvado y cojeando en Herido, arrastrando el paso en Crítico (M-05).
- **No hay salto** (fuera del alcance de la demo). Las caídas y los desniveles se resuelven con IK y transiciones cortas.
- **Asimilados:** locomoción humanoide base con ruido procedural y poses forzadas; ritmos rotos y antinaturales.
- **Rastrero:** cuatro patas con IK procedural sobre un ciclo base.
- **The Mother:** desplazamiento de la grúa por rieles; el brazo recolector se mueve con IK hacia el objetivo.

### Interacción

- Interactuar (`E`): abrir puertas, recoger del suelo y de estantes, usar paneles y la llave.
- Abrir el terminal del brazo (levanta el antebrazo izquierdo) y hackear (M-13; Lukas inmóvil, D-03).
- Usar una cura (~1.5 s, cancelable con un golpe; M-05, M-07).
- Examinar documentos y objetos confiscados (M-08).
- Restaurar la energía en el interruptor (D-72).
- Introducción: hackear la compuerta con el teléfono personal (D-63).

### Combate

- **Lukas:** apuntar, disparar (retroceso del revólver), recarga bala a bala cancelable (~0.7 s por bala), empujón o golpe de culata, golpe en el suelo, remate con el punzón de descarga (M-04, M-11, M-20).
- **Enemigos:** ataques de su tipo (golpe largo, mordida, zarpazos), grito de la Boca, estallido del Mudo.
- **Agarres (M-11):** clip sincronizado entre Lukas y el Alto, el Mudo o el brazo de The Mother; variante de liberación con el punzón (aturdimiento de 2 s).

### Daño

- Reacción de Lukas al golpe (vibración de cámara y cambio de postura, M-05).
- Reacción de los enemigos al disparo y al punto débil (interrumpe el ataque, D-74); aturdimiento por el punzón.
- Impactos de bala con sangre oscura, sin desmembramiento (D-85).

### Muerte / derrota

- **Lukas:** muerte por golpe y muerte por agarre; da paso a la cinemática corta de reconstrucción (D-39).
- **Enemigos:** muerte a ragdoll; el cuerpo queda en el suelo hasta el reciclaje o el remate (M-20).
- **The Mother:** caída de la estantería al destruir el último transformador (M-19).
- **Cinemáticas (motor, en tiempo real):** reconstrucción completa y corta, caída de la cápsula, Despertar y finales (D-39, D-51, D-61).

## Convenciones

Valores iniciales, sujetos a pruebas de rendimiento (60 FPS en hardware medio; [[03 - Público y Plataforma]]).

- **Escala:** 1 unidad = 1 m. Lukas mide 1.78 m.
- **Esqueleto:** un esqueleto humanoide compartido (compatible con Unity Humanoid) para Lukas, el Alto, la Boca y el Mudo, para reutilizar clips entre ellos. El Rastrero y The Mother tienen esqueletos propios.
- **Polígonos (aprox.):** Lukas ~5–8k triángulos; enemigos ~4–8k; The Mother ~15k; cadáveres ~2–4k.
- **Texturas:** 512 px para personajes y 1024 px para The Mother, con filtro *point*.
- **Clips:** 30 fps, cortos y en bucle cuando corresponda.
- **Formato:** FBX exportado desde Blender.
- **Nombres:** `CH_<Personaje>_<Variante>` para modelos (por ejemplo `CH_Lukas_RH07`, `CH_Alto_Reciclado`) y `AN_<Personaje>_<Acción>` para clips (por ejemplo `AN_Lukas_ReloadRound`).
- **Origen:** todo el contenido es propio, salvo la locomoción base de Mixamo, retocada (D-88).


> **Navegación:** [[00 - Índice]] · ← [[01 - Dirección de Arte]] · [[03 - Entornos]] →
