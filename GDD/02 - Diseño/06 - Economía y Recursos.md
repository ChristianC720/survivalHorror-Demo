# Economía y Recursos

## Descripción de la sección

Documenta cualquier recurso cuantificable: dinero, vida, energía, munición, experiencia, materiales, tiempo, puntos, etc.

## Recursos

Las reglas de uso de cada recurso se detallan en [[02 - Mecánicas]]; esta sección documenta cómo se obtienen, se limitan y se pierden.

| Recurso | Obtención | Uso | Límite | Pérdida |
|---|---|---|---|---|
| Salud | Inicia en **Bien** al reconstruirse (M-16). Solo se recupera con curas; no se regenera (D-07) | Absorber daño (M-05) | 3 estados: Bien, Herido y Crítico. Los enemigos matan en 2-4 golpes según su tipo (valores iniciales, sujetos a balance; D-66) | Golpes enemigos y agarres sin punzón de descarga (M-11). En Crítico, el siguiente daño mata |
| Curas | Recolección en salas según el presupuesto del piso (D-17) | Recuperar un estado de salud; no se usan en Bien (M-05) | 1 cura por casilla (valor inicial, sujeto a balance; D-37) | Al usarse, al descartarse, al morir o al reiniciar. Si se recibe daño durante la animación, no se consume |
| Munición | Recolección en salas según el presupuesto del piso (D-17); la sala de The Mother contiene 8 balas para la fase 3 (M-19, D-28, D-80) | Disparar el arma (M-04). Eliminar a un enemigo requiere 2–4 disparos según su tipo; a un reciclado, 2, o 1 si es un Rastrero (valores iniciales, sujetos a balance; D-37, D-66) | Tambor de 6 balas; 12 balas por casilla; cada recogida es un blíster de 3 cartuchos (valores iniciales, sujetos a balance; D-37, D-74) | Al disparar, al descartarse, al morir o al reiniciar. Recibir un golpe durante la recarga no la pierde |
| Resistencia | Regeneración automática tras 3 s sin correr, empujar ni rematar con golpe en el suelo (M-06) | Correr (~10% por segundo), empujón (35%) y golpe en el suelo (25%) (valores iniciales, sujetos a balance; M-06, M-20, D-37) | Una barra (100%). En 0, agotamiento de 2 s | Se consume con cada acción; no se pierde al morir: el personaje se reconstruye con la barra al 100% |
| Punzón de descarga (D-71) | Recolección en salas con presupuesto bajo (D-17) | Un solo uso con 2 funciones: se activa solo al ser agarrado y aturde al enemigo 2 s (M-11), o se usa con `E` sobre un cuerpo para rematarlo sin alertar (M-20) (valores iniciales, sujetos a balance) | 1 por casilla (valor inicial, sujeto a balance; D-37, D-71) | Al ser agarrado, al rematar, al descartarse, al morir o al reiniciar |
| Llaves (D-36) | 1 llave obligatoria por piso, que el generador ubica alcanzable antes de la puerta de la sala clave final; llaves opcionales según el presupuesto del piso (M-08, M-15) | La obligatoria abre la puerta de la sala clave final; las opcionales abren atajos o salas con recursos (M-08) | 1 llave por casilla (valor inicial, sujeto a balance; D-37). Son objetos clave: con el inventario lleno se puede reemplazar otro objeto para recogerlas (M-07) | Se consumen al usarse en su puerta (M-07). Se pierden al morir o al reiniciar; el piso se regenera con una llave obligatoria nueva (M-07, D-36) |
| Documentos y conocimiento | Leer documentos (incluidos los de la esposa), observar e investigar (M-08); las salas clave secundarias contienen documentos (M-15). Los documentos de Vitalis Konzern y de The Mother están en salas procedurales (D-52). Algunos documentos se registran corruptos, sobre todo los relacionados con la memoria de Lukas (D-45) | Información narrativa y reglas del sistema (cómo se forma un código, no su valor) (M-16). La corrupción nunca oculta reglas ni pistas de ruta y solo se completa en el desenlace si se reunieron los 6 documentos de Emilia (M-08, M-19, D-55) | No ocupan casillas; se registran en el terminal del brazo. Hay 12 documentos de Vitalis Konzern, Kessler Sicherheit AG y The Mother, 4 por piso, de los que aparecen 1–2 por intento (D-79) | Ninguna al morir o reiniciar: persisten (M-16) y se autoguardan al cambiar (D-30). Solo "Nueva partida" los borra (D-32) |
| Documentos de Emilia (D-52) | **6** en la demo, 2 por piso, siempre en salas clave secundarias, en ambas configuraciones (M-08, D-34) | Hacer avanzar la historia y, con los 6 acumulados en el terminal del brazo en cualquier intento, desbloquear la elección final entre Relevo completo y Ambiguo (M-19, D-51) | No ocupan casillas. Fuera del presupuesto por piso; no se escalan en el showcase. Son los únicos documentos que cuentan para la elección final | Ninguna al morir o reiniciar: persisten y se autoguardan al cambiar (M-16, D-30). Solo "Nueva partida" los borra y, con ellos, la elección final (D-32) |
| Objetos confiscados (D-73) | Examinarlos en salas procedurales: 12, 4 por piso; aparecen 1–2 por intento | Narrativa escénica; no tienen uso mecánico | No ocupan casillas; se registran en el terminal del brazo | Ninguna: persisten; solo "Nueva partida" los borra (D-32) |
| Fallas de la máquina (Deseable, M-17) | Descubrirlas en documentos, por observación o en terminales | Acortar hackeos u obtener accesos alternativos, incluida la fase 1 de The Mother (M-13, M-19) | 1-2 fallas en la demo; 1-2 usos por falla antes del parche (valores iniciales, sujetos a balance) | The Mother las parchea tras sus usos o al morir (M-17). El registro persiste en el terminal (D-30) |
| Carga de la linterna (D-71) | Se recarga sola tras ~3 s con la linterna apagada | Energía de la linterna (M-12) | Una barra de ~60 s de uso continuo (valor inicial, sujeto a balance); no ocupa casilla | Se gasta con la linterna encendida; en 0 se apaga hasta recuperar un mínimo de carga |

## Fuentes y sumideros

**Fuentes:** formas en las que el jugador obtiene recursos.

- **Recolección en salas procedurales**, según el presupuesto garantizado por piso (D-17): curas, munición, punzón de descarga y llaves.
- **Salas clave secundarias** (2–3 por piso en la configuración completa, 2 en el showcase; D-33, D-34): documentos, llaves y, a veces, el interruptor de energía (M-08, M-15, D-72). Los 2 documentos de Emilia de cada piso siempre están en estas salas (D-52); si ya están registrados, la sala da 1 recurso extra (D-79).
- **Salas que abren las llaves opcionales:** atajos o salas con recursos (D-36). Sus recursos forman parte del presupuesto del piso, no lo amplían.
- **Suministro fijo de la sala de The Mother:** 8 balas, para 3 transformadores de 2 disparos más 2 de margen (M-19, D-28, D-80).
- **Regeneración automática:** la resistencia (M-06) y la carga de la linterna (M-12, D-71).
- **Investigación:** documentos, conocimiento y fallas de la máquina, registrados en el terminal del brazo (M-08, M-16, M-17).
- **Reconstrucción:** el personaje vuelve con la salud en Bien y la resistencia al 100%, pero con el inventario vacío (M-16).
- **Autoguardado de la sala clave final (D-30):** no genera recursos; conserva el inventario con el que se llegó a la sala final de los pisos 1 y 2, solo para continuar después de salir del juego o de un fallo (ver [[07 - Reglas y Estados del Juego]]).
- **No son fuentes:** la regeneración de salud (D-07) y el saqueo de cadáveres (Post-demo, ver [[02 - Mecánicas]]).

**Sumideros:** formas en las que el jugador los gasta.

| Sumidero | Recurso que consume |
|---|---|
| Disparar | Munición |
| Curarse | Curas |
| Ser agarrado | Punzón de descarga o un estado de salud |
| Recibir daño | Salud |
| Correr y empujar | Resistencia |
| Rematar con golpe en el suelo | Resistencia, tiempo (~2 s, valor inicial, sujeto a balance) y ruido |
| Rematar con punzón | Punzón de descarga (y con él, la defensa contra el próximo agarre) |
| Abrir una puerta con llave | La llave (D-36) |
| Descartar | Cualquier objeto del inventario |
| Usar una falla | Usos de la falla hasta el parche |
| Linterna encendida | Carga de la linterna (se recarga sola) |
| Morir | Todo el inventario (D-18) |
| Reiniciar (D-32) | Todo el inventario, igual que morir |
| Salir del juego con costo (D-31) | Según el caso: todo el inventario si cuenta como muerte completa, o lo obtenido después de la sala final autoguardada (ver [[07 - Reglas y Estados del Juego]]) |

Todas las acciones consumen además **tiempo y exposición**, porque el inventario, el terminal y la lectura no pausan el juego (M-07, M-08, M-16). No rematar un cuerpo es un sumidero indirecto: el enemigo reciclado vuelve al piso y obliga a gastar más recursos después (M-20).

## Reglas

Define límites, regeneración, acumulación, intercambio y condiciones especiales.

### Presupuesto por piso (D-17)

- Cada piso garantiza un **mínimo y un máximo** de cada recurso. El generador decide en qué salas se ubica cada uno.
- Todo recurso del presupuesto debe ser alcanzable por una ruta válida (principio 9 de [[01 - Jugabilidad]]). La llave obligatoria siempre es alcanzable antes de la puerta de la sala clave final (D-36).
- Al morir o reiniciar se regenera la distribución de las salas y de los recursos (M-16); el presupuesto se vuelve a repartir.
- **Cada piso tiene menos presupuesto que el anterior**, mientras el número de enemigos aumenta (piso 1: 6–9; piso 2: 9–12; piso 3: 12–15 + The Mother en la configuración completa; M-15, D-37). La presión crece por la combinación de ambos.
- **Criterio de munición:** el máximo de munición de cada piso es menor que los disparos necesarios para eliminar al mínimo de enemigos del piso con 3 disparos cada uno (valor inicial, sujeto a balance). Eliminar a todos los enemigos nunca está cubierto por el presupuesto.

**Valores iniciales por piso, configuración completa** (mínimo-máximo; valores iniciales, sujetos a balance):

| Recurso | Piso 1 | Piso 2 | Piso 3 | Sala de The Mother |
|---|---|---|---|---|
| Curas | 2-3 | 1-2 | 1 | 0-1 |
| Munición (balas; blísteres de 3, D-74) | 12-15 | 9-12 | 6-9 | 8 (3 transformadores × 2 disparos + 2 de margen; M-19, D-28, D-80) |
| Punzón de descarga (D-71) | 2-3 | 1-2 | 0-1 | 0 |
| Llave obligatoria | 1 | 1 | 1 | 0 |
| Llaves opcionales | 1-2 | 0-1 | 0-1 | 0 |

- En el piso 3, la llave obligatoria abre la puerta de la sala de The Mother, que ocupa el lugar de la sala clave final (M-15, D-36).
- **Casillas que ocupa el máximo de cada piso** (con los máximos por casilla de D-37): piso 1, 11 casillas; piso 2, 7; piso 3, 5. En los pisos 1 y 2 el máximo supera las 6 casillas del inventario y obliga a elegir qué llevar; en el piso 3 el máximo ocupa 5, pero lo que se trae de los pisos anteriores sigue compitiendo por el espacio. Las llaves liberan su casilla al usarse.
- Fallas de la máquina (Deseable): 1-2 en toda la demo (M-17), fuera del presupuesto por piso.
- Documentos de Emilia: 2 por piso (6 en la demo), fuera del presupuesto por piso y sin casilla; se aplica igual en el showcase, sin escalado (D-52). Si el documento de Emilia de una sala clave secundaria ya está registrado, la sala contiene en su lugar 1 recurso extra del tipo más escaso del piso en ese intento, por encima del presupuesto (D-58, D-79, M-08).
- Las salas fijas del tutorial en el primer intento contienen 1 cura y 1 blíster de munición, que cuentan dentro del presupuesto del piso 1 (D-38, D-70).
- El mínimo garantizado de cada recurso nunca queda detrás de una puerta con llave opcional; detrás de ellas solo va lo que excede el mínimo, hasta el máximo (D-79).

### Presupuesto del showcase (D-34)

- El showcase usa 6–8 salas procedurales, 2 secundarias y 1 final por piso (~30 min) frente a las 8–12, 2–3 y 1 de la configuración completa (~45–60 min). Los enemigos y los recursos se escalan en proporción al número de salas (factor aproximado de 0.75, valor inicial, sujeto a balance). Los enemigos por piso del showcase se documentan en [[05 - Progresión]].
- La llave obligatoria se mantiene en 1 por piso; la munición de la fase 3 de The Mother no se escala (D-28).
- El criterio de munición se aplica igual: el máximo de cada piso es menor que los disparos necesarios para eliminar al mínimo de enemigos del piso en esa configuración.

**Valores iniciales por piso, showcase** (mínimo-máximo; valores iniciales, sujetos a balance):

| Recurso | Piso 1 | Piso 2 | Piso 3 | Sala de The Mother |
|---|---|---|---|---|
| Curas | 2 | 1 | 1 | 0-1 |
| Munición (balas) | 9-11 | 6-9 | 4-6 | Igual que en la configuración completa (D-28) |
| Punzón de descarga (D-71) | 2 | 1-2 | 0-1 | 0 |
| Llave obligatoria | 1 | 1 | 1 | 0 |
| Llaves opcionales | 0-1 | 0-1 | 0 | 0 |

### Inventario

- **6 casillas fijas**, en tiempo real (M-07).
- **Sin apilado infinito.** Máximo por casilla: curas 1, punzón de descarga 1, llave 1, munición 12 balas (valores iniciales, sujetos a balance; D-37, D-71). El revólver, la linterna y el anillo no ocupan casilla.
- Con el inventario lleno, un objeto clave (incluidas las llaves) puede reemplazar a otro objeto; si no se reemplaza, permanece en el suelo (M-07).
- El inventario se conserva entre pisos durante el intento. Al llegar a la sala clave final de los pisos 1 y 2, el autoguardado registra el inventario que se trae (D-30).

### Pérdida total al morir (D-18)

- Al morir, el personaje se reconstruye al inicio del ciclo en el piso 1 y **pierde todo el inventario**: curas, munición, punzón de descarga y llaves. The Mother devuelve el revólver descargado (D-74).
- **Reiniciar** (menú de pausa) tiene el mismo costo sobre el inventario que morir (D-32). Al morir o reiniciar se descarta el autoguardado de la sala final (M-16).
- **Salir del juego** puede contar como muerte completa o continuar desde la sala final autoguardada con el inventario guardado, según el estado al salir (D-31). Las reglas completas se documentan en [[07 - Reglas y Estados del Juego]].
- Vuelve con la salud en Bien y la resistencia al 100%.
- Persisten los documentos y el conocimiento del terminal del brazo. Las fallas siguen registradas, pero The Mother las parchea al morir (M-17).
- Lo que persiste hace al personaje más informado, no más fuerte (M-16).

### Regeneración, acumulación e intercambio

- **Regeneración:** solo la resistencia y la carga de la linterna se regeneran. La salud no (D-07).
- **Acumulación:** limitada por las 6 casillas, el máximo por casilla, el presupuesto decreciente de cada piso y la pérdida total al morir o reiniciar.
- **Intercambio:** no hay mecánica de intercambio definida para la demo.

### Condiciones especiales

- Recibir daño durante la animación de una cura la cancela sin consumirla (M-05).
- Recibir un golpe durante la recarga la cancela sin perder munición (M-04).
- La sala de The Mother contiene 8 balas (3 transformadores de 2 disparos más 2 de margen), aunque el jugador llegue sin munición (M-19, D-28).
- En la sala de The Mother no hay reciclaje de enemigos (D-28): rematar no es necesario y el presupuesto del punzón de descarga en la sala es 0.
- Si la sala clave final empieza sin energía, el interruptor está en una sala clave secundaria del mismo piso con ruta garantizada (M-15, D-72).
- Una llave obligatoria perdida al morir o reiniciar se sustituye por una nueva al regenerarse el piso (M-07, D-36).

## Balance

¿Qué comportamiento queremos incentivar?

- **Decidir con lo disponible (DC):** el presupuesto garantiza un mínimo, pero nunca lo suficiente para cubrir todo; el máximo de los pisos 1 y 2 supera las 6 casillas y cada casilla compite con las demás, incluidas las llaves.
- **Administrar entre pisos (V, DC):** el presupuesto baja en cada piso mientras los enemigos aumentan; lo que se ahorra en un piso es lo que se tiene para el siguiente.
- **Explorar con riesgo (V, A):** los recursos solo se obtienen recorriendo salas con amenazas y en tiempo real. Las llaves opcionales abren atajos o salas con recursos a cambio de una casilla y de tiempo. Los documentos y las fallas premian investigar y persisten entre intentos; reunir los 6 documentos de Emilia en las salas clave secundarias desbloquea la elección final (D-51).
- **Rematar o huir (DC):** cada enemigo eliminado obliga a elegir entre rematar (tiempo, resistencia y ruido, o el punzón de descarga que protegería del próximo agarre) y asumir que volverá. Huir conserva recursos, pero deja la sala infestada (M-14, M-20).

¿Qué comportamiento queremos evitar?

- **Acumular:** se evita con las 6 casillas, el máximo por casilla, el presupuesto decreciente y la pérdida total al morir o reiniciar (D-18, D-32).
- **Eliminar a todos los enemigos:** el criterio de munición, el ruido de los disparos y del golpe en el suelo, y el reciclaje hacen que limpiar el piso cueste más de lo que el presupuesto permite.
- **Desgaste por reciclaje:** cada enemigo se recicla una sola vez (D-20) y el reciclado muere con 2 disparos (M-20, D-37).
- **Saturar el inventario:** el punzón de descarga tiene presupuesto bajo y el golpe en el suelo permite rematar sin gastarlo (D-21, D-71).
- **Usar la salida del juego para conservar recursos:** salir durante alerta o caza cuenta como muerte completa (D-31).


> **Navegación:** [[00 - Índice]] · ← [[05 - Progresión]] · [[07 - Reglas y Estados del Juego]] →
