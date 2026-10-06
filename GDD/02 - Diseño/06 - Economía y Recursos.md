# Economía y Recursos

## Descripción de la sección

Documenta cualquier recurso cuantificable: dinero, vida, energía, munición, experiencia, materiales, tiempo, puntos, etc.

## Recursos

Las reglas de uso de cada recurso se detallan en [[02 - Mecánicas]]; esta sección documenta cómo se obtienen, se limitan y se pierden.

| Recurso | Obtención | Uso | Límite | Pérdida |
|---|---|---|---|---|
| Salud | Inicia en **Bien** al reconstruirse (M-16). Solo se recupera con curas; no se regenera (D-07) | Absorber daño (M-05) | 3 estados: Bien, Herido y Crítico. Los enemigos matan en 3-4 golpes (valor inicial, sujeto a balance) | Golpes enemigos y agarres sin objeto defensivo (M-11). En Crítico, el siguiente daño mata |
| Curas | Recolección en salas según el presupuesto del piso (D-17) | Recuperar un estado de salud; no se usan en Bien (M-05) | Ocupan casillas del inventario; cantidad por casilla **Por definir** | Al usarse, al descartarse o al morir. Si se recibe daño durante la animación, no se consume |
| Munición | Recolección en salas según el presupuesto del piso (D-17); la sala de The Mother garantiza la mínima para la fase 3 (M-19) | Disparar el arma (M-04) | Ocupa casillas del inventario; cargador y munición por casilla **Por definir** | Al disparar, al descartarse o al morir. Recibir un golpe durante la recarga no la pierde |
| Resistencia | Regeneración automática tras 3 s sin correr, empujar ni rematar con golpe en el suelo (M-06) | Correr, empujón y golpe en el suelo (M-06, M-20) | Una barra (100%). En 0, agotamiento de 2 s | Se consume con cada acción; no se pierde al morir: el personaje se reconstruye con la barra al 100% |
| Objeto defensivo | Recolección en salas según el presupuesto del piso (D-17) | Se activa solo al ser agarrado y aturde al enemigo 2 s (valor inicial, sujeto a balance) (M-11) | 1 casilla por objeto; se consume uno por agarre | Al ser agarrado, al descartarse o al morir |
| Consumible de remate | Recolección en salas con presupuesto bajo (D-17, D-21) | Rematar un cuerpo con `E` sin alertar a enemigos, evitando su reciclaje (M-20) | 1 casilla por objeto | Al usarse, al descartarse o al morir |
| Objetos clave | Los que exija la ruta hacia la sala clave del piso; el generador garantiza que sean alcanzables (principio 9 de [[01 - Jugabilidad]]). Cuáles son: **Por definir** | Avanzar en la ruta del piso (uso concreto **Por definir**) | Ocupan casillas; con el inventario lleno se puede reemplazar otro objeto para recogerlos (M-07) | Al morir (D-18) |
| Documentos y conocimiento | Leer documentos (incluidos los de la esposa), observar e investigar (M-08) | Información narrativa y reglas del sistema (cómo se forma un código, no su valor) (M-16) | No ocupan casillas; se registran en el terminal del brazo | Ninguna: persisten entre muertes (M-16) y se autoguardan al cambiar (D-22) |
| Fallas de la máquina (Deseable, M-17) | Descubrirlas en documentos, por observación o en terminales | Acortar hackeos u obtener accesos alternativos, incluida la fase 1 de The Mother (M-13, M-19) | 1-2 fallas en la demo; 1-2 usos por falla antes del parche (valores iniciales, sujetos a balance) | The Mother las parchea tras sus usos o al morir (M-17). El registro persiste en el terminal (D-22) |
| Baterías (Deseable) | Recolección en salas; presupuesto **Por definir** | Energía de la linterna (M-12). En la demo base la linterna no consume energía | Casillas y duración **Por definir** | Al consumirse con la linterna, al descartarse o al morir |
| Fusibles o tarjetas (Deseable) | Recolección en salas; presupuesto **Por definir** | Sellar una puerta para cortar una persecución (M-14) | Casillas **Por definir** | Al sellar, al descartarse o al morir |

## Fuentes y sumideros

**Fuentes:** formas en las que el jugador obtiene recursos.

- **Recolección en salas procedurales**, según el presupuesto garantizado por piso (D-17): curas, munición, objeto defensivo, consumible de remate, objetos clave y, si se implementan, baterías y fusibles o tarjetas.
- **Suministro fijo de la sala de The Mother:** la munición mínima para la fase 3 (M-19).
- **Regeneración automática:** solo la resistencia (M-06).
- **Investigación:** documentos, conocimiento y fallas de la máquina, registrados en el terminal del brazo (M-08, M-16, M-17).
- **Reconstrucción:** el personaje vuelve con la salud en Bien y la resistencia al 100%, pero con el inventario vacío (M-16).
- **No son fuentes:** la regeneración de salud (D-07) y el saqueo de cadáveres (Post-demo, ver [[02 - Mecánicas]]).

**Sumideros:** formas en las que el jugador los gasta.

| Sumidero | Recurso que consume |
|---|---|
| Disparar | Munición |
| Curarse | Curas |
| Ser agarrado | Objeto defensivo o un estado de salud |
| Recibir daño | Salud |
| Correr y empujar | Resistencia |
| Rematar con golpe en el suelo | Resistencia, tiempo (~2 s, valor inicial, sujeto a balance) y ruido |
| Rematar con consumible | Consumible de remate |
| Descartar | Cualquier objeto del inventario |
| Usar una falla | Usos de la falla hasta el parche |
| Linterna (Deseable) | Baterías |
| Sellar una puerta (Deseable) | Fusibles o tarjetas |
| Morir | Todo el inventario (D-18) |

Todas las acciones consumen además **tiempo y exposición**, porque el inventario, el terminal y la lectura no pausan el juego (M-07, M-08, M-16). No rematar un cuerpo es un sumidero indirecto: el enemigo reciclado vuelve al piso y obliga a gastar más recursos después (M-20).

## Reglas

Define límites, regeneración, acumulación, intercambio y condiciones especiales.

### Presupuesto por piso (D-17)

- Cada piso garantiza un **mínimo y un máximo** de cada recurso. El generador decide en qué salas procedurales se ubica cada uno.
- Todo recurso del presupuesto debe ser alcanzable por una ruta válida (principio 9 de [[01 - Jugabilidad]]).
- Al morir se regenera la distribución de las salas y de los recursos (M-16); el presupuesto se vuelve a repartir.
- El piso 2 tiene menos presupuesto que el piso 1.

**Valores iniciales por piso** (mínimo-máximo; valor inicial, sujeto a balance):

| Recurso | Piso 1 | Piso 2 | Sala de The Mother |
|---|---|---|---|
| Curas | 2-3 | 1-2 | 0-1 |
| Munición (recogidas; cantidad por recogida **Por definir**, M-04) | 2-3 | 1-2 | La mínima para la fase 3 (cantidad **Por definir**, M-19) |
| Objeto defensivo | 1-2 | 1 | 0 |
| Consumible de remate | 1 | 0-1 | **Por definir** (depende de si existe reciclaje en la sala, M-19) |
| Objetos clave | Los que exija la ruta | Los que exija la ruta | **Por definir** |
| Baterías (Deseable) | **Por definir** | **Por definir** | **Por definir** |
| Fusibles o tarjetas (Deseable) | **Por definir** | **Por definir** | **Por definir** |

- El máximo del piso 1 (9 recogidas, más los objetos clave) supera las 6 casillas del inventario para obligar a elegir qué llevar. En el piso 2 el máximo (6 recogidas, más los objetos clave) iguala las casillas; la presión viene del presupuesto menor.
- Fallas de la máquina (Deseable): 1-2 en toda la demo (M-17), fuera del presupuesto por piso.

### Inventario

- **6 casillas fijas**, en tiempo real (M-07).
- **Sin apilado infinito:** cada tipo de objeto tiene un máximo por casilla. Munición por casilla: **Por definir** (M-04). Máximo de curas, objetos defensivos y consumibles de remate por casilla: **Por definir**.
- Con el inventario lleno, un objeto clave puede reemplazar a otro objeto; si no se reemplaza, permanece en el suelo (M-07).

### Pérdida total al morir (D-18)

- Al morir, el personaje se reconstruye en el punto de reaparición y **pierde todo el inventario**: curas, munición, objeto defensivo, consumible de remate, objetos clave y, si se implementan, baterías y fusibles o tarjetas.
- Vuelve con la salud en Bien y la resistencia al 100%.
- Persisten los documentos y el conocimiento del terminal del brazo. Las fallas siguen registradas, pero The Mother las parchea al morir (M-17).
- Lo que persiste hace al personaje más informado, no más fuerte (M-16).

### Regeneración, acumulación e intercambio

- **Regeneración:** solo la resistencia se regenera. La salud no (D-07).
- **Acumulación:** limitada por las 6 casillas, el máximo por casilla y la pérdida total al morir.
- **Intercambio:** no hay mecánica de intercambio definida para la demo.

### Condiciones especiales

- Recibir daño durante la animación de una cura la cancela sin consumirla (M-05).
- Recibir un golpe durante la recarga la cancela sin perder munición (M-04).
- Si el jugador llega a The Mother sin munición, la sala proporciona la mínima para la fase 3 (M-19).
- Si un elevador está sin energía y no se puede hackear, debe existir una ruta alternativa a la fuente de energía en el mismo piso (M-15).
- Si existe reciclaje en la sala de The Mother: **Por definir** (M-19, M-20).

## Balance

¿Qué comportamiento queremos incentivar?

- **Decidir con lo disponible (DC):** el presupuesto garantiza un mínimo, pero nunca lo suficiente para cubrir todo; el máximo del piso 1 supera las 6 casillas y cada casilla compite con las demás.
- **Explorar con riesgo (V, A):** los recursos solo se obtienen recorriendo salas con amenazas y en tiempo real. Los documentos y las fallas premian investigar y persisten entre intentos.
- **Rematar o huir (DC):** cada enemigo eliminado obliga a elegir entre rematar (tiempo, resistencia y ruido, o una casilla) y asumir que volverá. Huir conserva recursos, pero deja la sala infestada (M-14, M-20).

¿Qué comportamiento queremos evitar?

- **Acumular:** se evita con las 6 casillas, el máximo por casilla y la pérdida total al morir (D-18).
- **Eliminar a todos los enemigos:** la munición escasa, el ruido de los disparos y del golpe en el suelo, y el reciclaje hacen que limpiar el piso cueste más de lo que el presupuesto permite.
- **Desgaste por reciclaje:** cada enemigo se recicla una sola vez (D-20) y el reciclado muere con menos golpes (M-20).
- **Saturar el inventario:** el consumible de remate tiene presupuesto bajo y el golpe en el suelo permite rematar sin ocupar casillas (D-21).


> **Navegación:** [[00 - Índice]] · ← [[05 - Progresión]] · [[07 - Reglas y Estados del Juego]] →
