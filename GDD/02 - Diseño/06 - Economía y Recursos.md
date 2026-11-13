# Economía y Recursos

## 1. Descripción de la sección

Documenta cualquier recurso cuantificable: dinero, vida, energía, munición, experiencia, materiales, tiempo, puntos, etc.

## 2. Recursos

Las reglas de uso de cada recurso están en [[02 - Mecánicas]]. Esta sección documenta cómo se obtienen, cómo se limitan y cómo se pierden. Los valores numéricos de este documento son iniciales y están sujetos a balance.

| Recurso | Obtención | Uso | Límite | Pérdida |
|---|---|---|---|---|
| Salud | Empieza en Bien al reconstruirse. Solo se recupera con curas y no se regenera | Absorber daño | 3 estados, que son Bien, Herido y Crítico. Los enemigos matan en 2 a 4 golpes según su tipo | Golpes enemigos y agarres sin punzón. En Crítico, el siguiente daño mata |
| Curas | Recogida en salas según el presupuesto del piso | Recuperar un estado de salud. En Bien solo sirven con daño, y entonces devuelven la salud completa | 1 cura por casilla | Al usarse, al descartarse y al morir o reiniciar si se recogieron en el piso actual. Si Lukas recibe daño durante la animación, no se gasta |
| Munición | Recogida en salas según el presupuesto del piso. La sala de The Mother tiene 12 balas para la fase 2 y 8 para la fase 3 | Disparar. Un enemigo exige 2 a 4 disparos según su tipo, y un reciclado 2, o 1 si es un Rastrero | Tambor de 6 balas y 12 balas por casilla. Cada recogida es un blíster de 4 cartuchos | Al disparar, al descartarse y al morir o reiniciar si se recogió en el piso actual. Un golpe durante la recarga no la pierde |
| Resistencia | Se regenera sola tras 3 s sin correr, empujar ni rematar con golpe en el suelo | Correr, a ~10% por segundo, empujón, 35%, y golpe en el suelo, 25% | Una barra al 100%. En 0, agotamiento de 2 s | Se gasta con cada acción. No se pierde al morir, porque Lukas se reconstruye con la barra al 100% |
| Punzón de descarga | Recogida en salas con presupuesto bajo | Un solo uso con dos funciones. Se activa solo al ser agarrado y aturde 2 s, o se usa con `E` sobre un cuerpo para rematarlo sin alertar | 1 por casilla | Al ser agarrado, al rematar, al descartarse y al morir o reiniciar si se recogió en el piso actual |
| Llaves | 1 llave obligatoria por piso, que el generador deja alcanzable antes de la puerta de la sala final. Llaves opcionales según el presupuesto | La obligatoria abre la sala clave final. Las opcionales abren atajos o salas con recursos | 1 llave por casilla. Son objetos clave, así que con el inventario lleno pueden reemplazar a otro objeto | Se gastan al usarse en su puerta. Se pierden al morir o reiniciar, y el piso se regenera con una llave obligatoria nueva |
| Documentos y conocimiento | Leer documentos, observar e investigar. Las salas secundarias tienen documentos. Los de Vitalis Konzern y de The Mother están en salas procedurales. Algunos se registran corruptos, sobre todo los ligados a la memoria de Lukas | Información narrativa y reglas del sistema, como cómo se forma un código, no su valor. La corrupción nunca oculta reglas ni rutas y solo se completa en el desenlace con los 6 documentos de Emilia | No ocupan casilla y quedan en el terminal del brazo. Hay 18 documentos de Vitalis Konzern, Kessler Sicherheit AG y The Mother, seis por piso, y aparecen 1 o 2 por intento | Ninguna al morir o reiniciar. Persisten y se autoguardan al cambiar. Solo `Nueva partida` los borra |
| Documentos de Emilia | 6 en la demo, dos por piso, siempre en salas clave secundarias, en ambas configuraciones | Hacen avanzar la historia. Con los 6 acumulados en cualquier intento, desbloquean la elección entre Relevo completo y Ambiguo | No ocupan casilla. Están fuera del presupuesto por piso y no se escalan en el showcase. Son los únicos documentos que cuentan para la elección final | Ninguna al morir o reiniciar. Persisten y se autoguardan al cambiar. Solo `Nueva partida` los borra, y con ellos la elección final |
| Objetos confiscados | Examinarlos en salas procedurales. Son 12, cuatro por piso, y aparecen 1 o 2 por intento | Narrativa escénica sin uso mecánico | No ocupan casilla y quedan en el terminal del brazo | Ninguna. Persisten y solo `Nueva partida` los borra |
| Fallas de la máquina, deseable | Descubrirlas en documentos, por observación o en terminales | Acortar hackeos u obtener accesos alternativos, también en la fase 1 de The Mother | 1 o 2 fallas en la demo, con 1 o 2 usos por falla antes del parche | The Mother las parchea tras sus usos o al morir. El registro persiste en el terminal |
| Carga de la linterna | Se recarga sola tras ~3 s apagada y se llena en ~30 s | Encender la linterna | Una barra de ~120 s de uso continuo. No ocupa casilla | Se gasta con la linterna encendida. En 0 se apaga y no enciende durante ~10 s |

## 3. Fuentes y sumideros

**Fuentes:**

- **Recogida en salas procedurales**, según el presupuesto garantizado por piso. Entrega curas, munición, punzones y llaves.
- **Salas clave secundarias**, 2 a 3 por piso en la configuración completa y 2 en el showcase. Guardan documentos, llaves y, a veces, el interruptor de energía. Los 2 documentos de Emilia de cada piso siempre están aquí. Si ya están registrados, la sala da 1 recurso extra.
- **Salas que abren las llaves opcionales.** Son atajos o salas con recursos. Sus recursos forman parte del presupuesto del piso y no lo amplían.
- **Suministro fijo de la sala de The Mother.** Son 20 balas. Hay 12 en los puntos ciegos para los 4 asimilados de la fase 2 y 8 junto al núcleo para los 3 transformadores de 2 disparos, más 2 de margen.
- **Suministro inicial.** Al construirse el piso 1, hay 6 balas en la bandeja de efectos personales junto a la cápsula donde despierta Lukas, fuera del presupuesto del piso.
- **Regeneración automática** de la resistencia y de la carga de la linterna.
- **Investigación.** Documentos, conocimiento y fallas de la máquina, registrados en el terminal del brazo.
- **Reconstrucción.** Lukas vuelve con la salud en Bien, la resistencia al 100% y el inventario con el que llegó al piso.
- **Autoguardado de la sala clave final.** No genera recursos. Conserva el inventario con el que se llegó a la sala final de los pisos 1 y 2, solo para continuar tras salir del juego o un fallo. Ver [[07 - Reglas y Estados del Juego]].
- **No son fuentes** la regeneración de salud ni el saqueo de cadáveres, que queda fuera de la demo.

**Sumideros:**

| Sumidero | Recurso que gasta |
|---|---|
| Disparar | Munición |
| Curarse | Curas |
| Ser agarrado | Un punzón de descarga o un estado de salud |
| Recibir daño | Salud |
| Correr y empujar | Resistencia |
| Rematar con golpe en el suelo | Resistencia, ~2 s de tiempo y ruido |
| Rematar con punzón | El punzón y, con él, la defensa contra el próximo agarre |
| Abrir una puerta con llave | La llave |
| Descartar | Cualquier objeto del inventario |
| Usar una falla | Usos de la falla hasta el parche |
| Linterna encendida | Carga de la linterna, que se recarga sola |
| Morir | Todo lo recogido en el piso actual |
| Reiniciar | Todo lo recogido en el piso actual, igual que morir |
| Salir del juego con costo | Lo recogido en el piso si cuenta como muerte completa, o lo obtenido después de la sala final autoguardada. Ver [[07 - Reglas y Estados del Juego]] |

Toda acción gasta además **tiempo y exposición**, porque el inventario y la lista del terminal no pausan el juego. Solo la pausa y la vista de lectura lo detienen. No rematar un cuerpo es un sumidero indirecto, porque el enemigo reciclado vuelve y obliga a gastar más después.

## 4. Reglas

### 4.1 Presupuesto por piso

- Cada piso garantiza un mínimo y un máximo de cada recurso. El generador decide en qué salas va cada uno.
- Todo recurso del presupuesto es alcanzable por una ruta válida, según el principio 9 de [[01 - Jugabilidad]]. La llave obligatoria siempre es alcanzable antes de la puerta de la sala final.
- Al morir o reiniciar se regenera la distribución de salas y recursos, y el presupuesto se vuelve a repartir.
- **Cada piso tiene menos presupuesto que el anterior**, mientras los enemigos aumentan. En la configuración completa son 6 a 9 en el piso 1, 9 a 12 en el piso 2 y 12 a 15 en el piso 3. The Mother se suma aparte. La presión crece por la suma de ambas cosas.
- **Criterio de munición.** El máximo de munición de cada piso es menor que los disparos necesarios para eliminar al mínimo de enemigos del piso con 3 disparos cada uno. Eliminar a todos los enemigos nunca está cubierto por el presupuesto.
- **Excepción del piso 1.** Su máximo de 20 balas supera los 18 disparos de su mínimo de enemigos, y además están las 6 balas iniciales. Se acepta porque el Alto necesita 4 disparos y el piso 1 enseña el combate.

**Valores por piso en la configuración completa**, en rango de mínimo a máximo:

| Recurso | Piso 1 | Piso 2 | Piso 3 | Sala de The Mother |
|---|---|---|---|---|
| Curas | 3 a 4 | 2 a 3 | 2 | 0 a 1 |
| Munición, en balas y blísteres de 4 | 16 a 20 | 12 a 16 | 8 a 12 | 20. Son 12 para la fase 2 y 8 para la fase 3, que cubren 3 transformadores de 2 disparos más 2 de margen |
| Punzón de descarga | 2 a 3 | 1 a 2 | 0 a 1 | 0 |
| Llave obligatoria | 1 | 1 | 1 | 0 |
| Llaves opcionales | 1 a 2 | 0 a 1 | 0 a 1 | 0 |

- En el piso 3, la llave obligatoria abre la puerta de la sala de The Mother, que ocupa el lugar de la sala final.
- **Casillas que ocupa el máximo de cada piso.** Con los máximos por casilla, el piso 1 ocupa 12 casillas, el piso 2 ocupa 9 y el piso 3 ocupa 6. En los pisos 1 y 2, el máximo supera las 6 casillas del inventario y obliga a elegir qué llevar.
- En el piso 3, el máximo ocupa las 6 casillas, y lo que llega de los pisos anteriores sigue compitiendo por el espacio. Las llaves liberan su casilla al usarse.
- Las fallas de la máquina, de 1 a 2 en toda la demo, están fuera del presupuesto por piso.
- Los documentos de Emilia, 2 por piso y 6 en la demo, están fuera del presupuesto y no ocupan casilla. Se aplica igual en el showcase, sin escalado.
- Si el documento de Emilia de una sala secundaria ya está registrado, la sala da en su lugar 1 recurso extra. Es del tipo más escaso del piso en ese intento y va por encima del presupuesto.
- Las salas fijas del tutorial, en el primer intento, tienen 1 cura y 1 blíster de munición, que cuentan dentro del presupuesto del piso 1.
- El mínimo garantizado de cada recurso nunca queda detrás de una puerta con llave opcional. Detrás de ellas solo va lo que excede el mínimo, hasta el máximo.

### 4.2 Modo Historia

- En el Modo Historia, el mínimo garantizado de cada recurso sube en 1 por piso. Si supera al máximo, el máximo sube lo mismo.
- En el Modo Historia, cada blíster trae 6 balas en lugar de 4. En munición, sumar 1 equivale a 1 blíster de 6 balas. Las llaves no cambian.
- El Modo Supervivencia usa los presupuestos de esta sección sin cambios. Los demás efectos del Modo Historia están en [[03 - Accesibilidad]].

### 4.3 Presupuesto del showcase

- El showcase usa 6 a 8 salas procedurales, 2 secundarias y 1 final por piso, para ~30 min. La configuración completa usa 8 a 12, 2 a 3 y 1, para ~45 a 60 min.
- Los enemigos y los recursos se escalan en proporción al número de salas, con un factor de ~0,75. Los enemigos por piso del showcase están en [[05 - Progresión]].
- La llave obligatoria sigue en 1 por piso, y la munición de la fase 3 de The Mother no se escala.
- El criterio de munición se aplica igual. El máximo de cada piso es menor que los disparos para eliminar al mínimo de enemigos del piso en esta configuración.

**Valores por piso en el showcase**, en rango de mínimo a máximo:

| Recurso | Piso 1 | Piso 2 | Piso 3 | Sala de The Mother |
|---|---|---|---|---|
| Curas | 2 a 3 | 2 | 1 a 2 | 0 a 1 |
| Munición, en balas y blísteres de 4 | 12 a 16 | 8 a 12 | 8 | Igual que en la configuración completa |
| Punzón de descarga | 2 | 1 a 2 | 0 a 1 | 0 |
| Llave obligatoria | 1 | 1 | 1 | 0 |
| Llaves opcionales | 0 a 1 | 0 a 1 | 0 | 0 |

### 4.4 Inventario

- Tiene 6 casillas fijas y funciona en tiempo real.
- No hay apilado sin límite. El máximo por casilla es 1 cura, 1 punzón, 1 llave y 12 balas. El revólver, la linterna y el anillo no ocupan casilla.
- Con el inventario lleno, un objeto clave, como una llave, puede reemplazar a otro objeto. Si no se reemplaza, queda en el suelo.
- El inventario se conserva entre pisos durante el intento. Al llegar a la sala final de los pisos 1 y 2, el autoguardado registra el inventario con el que llegó Lukas.

### 4.5 Pérdida al morir

- Al morir, Lukas se reconstruye al inicio del piso actual con el inventario con el que llegó a ese piso. Pierde todo lo recogido en él, llaves incluidas. The Mother devuelve el revólver descargado.
- `Reiniciar`, en el menú de pausa, cuesta lo mismo que morir. Al morir o reiniciar se descarta el autoguardado de la sala final.
- Salir del juego puede contar como muerte completa o continuar desde la sala final autoguardada con el inventario guardado, según el estado al salir. Ver [[07 - Reglas y Estados del Juego]].
- Lukas vuelve con la salud en Bien y la resistencia al 100%.
- Persisten los documentos y el conocimiento del terminal del brazo. Las fallas siguen registradas, pero The Mother las parchea al morir.
- Lo que persiste hace a Lukas más informado, no más fuerte.

### 4.6 Regeneración, acumulación e intercambio

- **Regeneración.** Solo se regeneran la resistencia y la carga de la linterna. La salud no.
- **Acumulación.** La limitan las 6 casillas, el máximo por casilla, el presupuesto decreciente y la pérdida de lo recogido al morir o reiniciar.
- **Intercambio.** No hay intercambio en la demo.

### 4.7 Condiciones especiales

- Recibir daño durante la animación de una cura la cancela sin gastarla.
- Recibir un golpe durante la recarga la cancela sin perder munición.
- La sala de The Mother tiene 12 balas para la fase 2 y 8 para la fase 3 aunque el jugador llegue sin munición.
- En la sala de The Mother no hay reciclaje. Rematar no hace falta y el presupuesto de punzones en la sala es 0.
- Si la sala final empieza sin energía, el interruptor está en una sala secundaria del mismo piso con ruta garantizada.
- Una llave obligatoria perdida al morir o reiniciar se sustituye por una nueva al regenerarse el piso.

## 5. Balance

**Comportamientos que queremos incentivar:**

- **Decidir con lo disponible.** El presupuesto garantiza un mínimo, pero nunca alcanza para todo. El máximo de los pisos 1 y 2 supera las 6 casillas, y cada casilla compite con las demás, llaves incluidas.
- **Administrar entre pisos.** El presupuesto baja en cada piso mientras los enemigos aumentan. Lo que se ahorra en un piso es lo que se tiene para el siguiente.
- **Explorar con riesgo.** Los recursos solo se obtienen recorriendo salas con amenazas, en tiempo real. Las llaves opcionales abren atajos o salas con recursos a cambio de una casilla y de tiempo.
- Los documentos y las fallas premian investigar y persisten entre intentos. Reunir los 6 documentos de Emilia en las salas secundarias desbloquea la elección final.
- **Rematar o huir.** Cada enemigo eliminado obliga a elegir entre rematar y asumir que volverá. Rematar cuesta tiempo, resistencia y ruido, o el punzón que protegería del próximo agarre. Huir conserva recursos, pero deja la sala infestada.

**Comportamientos que queremos evitar:**

- **Acumular.** Lo impiden las 6 casillas, el máximo por casilla, el presupuesto decreciente y la pérdida de lo recogido al morir o reiniciar.
- **Eliminar a todos los enemigos.** Lo impiden el criterio de munición, el ruido de los disparos y del golpe en el suelo y el reciclaje. Limpiar el piso cuesta más de lo que el presupuesto permite.
- **Desgaste por reciclaje.** Cada enemigo se recicla una sola vez y el reciclado muere con 2 disparos.
- **Saturar el inventario.** El punzón tiene presupuesto bajo y el golpe en el suelo permite rematar sin gastarlo.
- **Usar la salida del juego para conservar recursos.** Salir durante una caza cuenta como muerte completa.

> **Navegación:** [[00 - Índice]] · ← [[05 - Progresión]] · [[07 - Reglas y Estados del Juego]] →
