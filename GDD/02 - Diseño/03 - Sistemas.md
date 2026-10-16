# Sistemas del Juego

## Descripción de la sección

Los sistemas son conjuntos de reglas y componentes que trabajan juntos: combate, inventario, IA, diálogo, progresión, economía, misiones, etc.

## Inventario de sistemas

Cada sistema agrupa mecánicas de [[02 - Mecánicas]]. Las reglas de detalle viven en las mecánicas; aquí se documenta cómo se integran.
**Pilares:** V = Vulnerabilidad Constante, A = Adaptación, DC = Decisiones con Consecuencias (ver [[04 - Pilares de Diseño]]).
**Configuraciones (D-34):** los sistemas funcionan igual en la configuración **completa** y en la **showcase** (evento); solo S-06 cambia sus parámetros. Los criterios de aceptación indican a qué configuración aplican.

| Sistema | Propósito | Dependencias | Estado |
|---|---|---|---|
| S-01 Cámara y vigilancia | Vista OTS, apuntado y vista de vigilancia con su señal escalonada, sus cortes, sus bloqueos y la atención de The Mother (M-01, M-02, M-03; D-23, D-24, D-25). Pilares V, A, DC | S-02, S-06 | Definido |
| S-02 IA y percepción | Patrulla, detección por vista, luz y ruido, estados de alerta, respuesta a la atención de The Mother y persecución dentro del piso (M-09, M-12, M-14, M-15; D-23, D-24, D-96). Pilares V, A | S-01, S-06, S-11 | Definido |
| S-03 Salud y combate | Daño, estados de salud, disparo, empujón y defensa contra agarres (M-04, M-05, M-06, M-11). Pilares V, DC | S-02, S-04, S-05 | Definido (valores iniciales D-37, sujetos a balance) |
| S-04 Resistencia | Barra única para correr, empujar y rematar con golpe en el suelo (M-06, M-20). Pilares V, DC | — | Definido (valores iniciales D-37, sujetos a balance) |
| S-05 Inventario | 6 casillas en tiempo real para curas, munición, punzón de descarga y llaves (M-07, M-08; D-36, D-37, D-71). Pilar DC | S-06, S-08 | Definido |
| S-06 Generación procedural por pisos | Construye los 3 pisos del intento con sus salas clave, sus llaves, su ruta garantizada, su cobertura de cámaras, su presupuesto de enemigos y recursos y los documentos de Emilia en salas clave secundarias, según la configuración (M-08, M-15; D-11, D-17, D-25, D-33, D-34, D-36, D-38, D-52). Pilar A | S-07, S-08 | Definido (distancia mínima a la sala clave final por definir) |
| S-07 Persistencia | Terminal del brazo, código de sujeto R-H, conteo de los 6 documentos de Emilia, fallas de la máquina, autoguardado en la sala clave final y costo de salir del juego (M-08, M-16, M-17; D-30, D-31, D-32, D-43, D-45, D-50, D-52). Pilares A, DC | S-02, S-06, S-08 | Definido |
| S-08 Ciclo de muerte y reconstrucción | Procesa la muerte y el reinicio, reconstruye al personaje, lleva el contador y la degradación y reproduce las cinemáticas de presentación y las cinemáticas finales (Asimilación, Relevo básico, Relevo completo y Ambiguo) (M-15, M-16, M-18, M-19; D-18, D-29, D-32, D-35, D-39, D-42, D-51). Pilar A | S-03, S-07 | Definido (degradación: Deseable) |
| S-09 Reciclaje de enemigos | El laboratorio recicla una vez a cada enemigo eliminado salvo que se remate; sin reciclaje en la sala del jefe (M-20; D-20, D-21, D-27, D-28, D-99). Pilares V, DC | S-02, S-04, S-05, S-11 | Definido |
| S-10 Jefe The Mother | Encuentro final de 3 fases en el piso 3 que reutiliza los demás sistemas; fase 2 con cámaras hostiles (M-19; D-16, D-28). Pilares V, A, DC | S-01, S-03, S-06, S-07 | Definido |
| S-11 Audio dinámico | Señales diegéticas de alerta, latido del protagonista, pasos y avisos del sistema, coordinadas con la señal de vigilancia escalonada, las cámaras hostiles y el tope de la degradación; latido del laboratorio como capa ambiental (M-10; D-23, D-28, D-29, D-46). Pilar A | S-02, S-08, S-10 | Definido |

## Plantilla de sistema

### S-01: Cámara y vigilancia

**Objetivo:** Limitar la visión del jugador la mayor parte del tiempo y ofrecer información amplia solo a cambio de exposición, tiempo, disponibilidad de cámaras y la atención de The Mother.

**Entradas:**

- Movimiento del mouse, `Clic derecho` para apuntar y `Q` para la vista de vigilancia (ver [[04 - Controles]]).
- Estado de alerta más alto de los enemigos dentro del alcance de M-03, eventos de alerta de S-02 y eventos de daño de S-03.
- Salas con cámara activa generadas por S-06 (cobertura D-25).
- Fase actual del encuentro de S-10.

**Procesamiento / reglas:**

- La vista OTS es la cámara por defecto; al apuntar se acerca al hombro y reduce la velocidad de movimiento (M-01).
- La vista de vigilancia solo se activa en una sala con cámara activa y sin enemigos en caza; se muestra en isométrica con ruido CRT, retraso y un solo piso a la vez (M-02). Para la vigilancia, "en alerta" equivale a **caza** (D-23).
- **Señal escalonada (D-23):** la calidad de la señal depende del estado de alerta más alto dentro del alcance de M-03: sin cambios en sospecha, degradada (más ruido CRT y más retraso) en búsqueda, cortada y bloqueada en caza. La tabla y los casos límite están en M-02 de [[02 - Mecánicas]].
- **Atención de The Mother (D-24):** S-01 lleva un medidor **oculto** (sin barra ni indicador en pantalla) que se llena en ~5 s de vigilancia activa y baja en ~15 s sin usarla (valores iniciales, sujetos a balance; D-37). Al llenarse, S-01 envía a S-02 el evento "atención llena" con la posición del jugador. El medidor es único por piso y no se vacía al cambiar de sala; "enemigos cercanos" son los de la sala de Lukas y los de las salas conectadas a ella (D-98).
- Se corta al recibir daño, al correr, apuntar o empujar, al cruzar a una sala sin cámara o cuando un enemigo entra en caza (M-02, M-03).
- El bloqueo se mantiene mientras haya al menos un enemigo en caza en la sala o en el área cercana con ruta de persecución, y se levanta al eliminarlo, al salir de la caza o al cruzar una puerta (M-03). Si al levantarse el bloqueo algún enemigo sigue en búsqueda, la señal vuelve degradada.
- **Sala del jefe (D-28):** durante la fase 2, las cámaras de la sala dejan de ser una herramienta del jugador y pasan a vigilarlo (S-10); la vista de vigilancia no se puede activar en la sala mientras dure esa fase.

**Salidas:** Vista activa (OTS, apuntado, vigilancia, vigilancia degradada o bloqueada), información parcial y retrasada de la sala, evento "atención llena" para S-02, velocidad de movimiento reducida para S-03 y S-04.

**Estados posibles:** OTS, Apuntando, Vigilancia, Vigilancia degradada, OTS bloqueada (ver el diagrama de M-01 en [[02 - Mecánicas]]).

**Dependencias:** S-02 IA y percepción (estado de alerta más alto y eventos de caza), S-06 Generación procedural (salas con cámara).

**Interacciones:** S-02 (la atención llena pone a los enemigos cercanos en sospecha y búsqueda), S-03 (el daño corta la vigilancia), S-04 (no se corre al apuntar), S-08 (la degradación no tapa la señal, D-29), S-10 (cámaras hostiles en la fase 2), S-11 (las señales sustituyen la falta de visión periférica y coinciden con la calidad de la señal).

**Datos que necesita:** Salas con cámara activa del piso actual, estado de alerta más alto dentro del alcance de M-03, rutas de persecución, tiempos de llenado y vaciado de la atención, fase del jefe.

**Datos que genera:** Vista actual, calidad de la señal, estado del bloqueo, nivel de atención de The Mother (interno, nunca visible).

**Riesgos:**

- Una cámara demasiado cerrada produce golpes "invisibles"; se compensa con S-11.
- Si la vigilancia es demasiado frecuente elimina la incertidumbre (Pilar A); se mitiga con el ruido, el retraso, la degradación en búsqueda, la atención de The Mother y las salas procedurales sin cámara.
- Si la diferencia entre la señal base y la degradada no se percibe, el jugador no detecta la atención de The Mother y la búsqueda se siente injusta.

**Criterios de aceptación (ambas configuraciones):**

- La vigilancia no se puede activar con un enemigo en caza dentro del alcance de M-03.
- La vigilancia se corta de inmediato cuando el personaje recibe daño o un enemigo entra en caza.
- Con enemigos solo en sospecha, la señal es idéntica a la señal base; con al menos uno en búsqueda, muestra más ruido CRT y más retraso que la base, sin cortarse.
- Ningún elemento de la interfaz muestra el medidor de atención.
- Tras ~5 s de vigilancia continua (valor inicial, sujeto a balance), los enemigos cercanos en patrulla pasan a sospecha y después a búsqueda hacia la posición del jugador; tras ~15 s sin vigilancia (valor inicial, sujeto a balance) el medidor vuelve a vacío (comprobable en una compilación de depuración).
- Durante la fase 2 del jefe, `Q` no activa la vista de vigilancia.
- La vigilancia nunca muestra más de un piso.
- La cámara no atraviesa la geometría.

### S-02: IA y percepción

**Objetivo:** Mover a los enemigos por la instalación y decidir cuándo detectan y persiguen al jugador según lo que ven, lo que oyen, la luz y la atención de The Mother, sin mostrar su percepción en pantalla.

**Entradas:** Posición y nivel de ruido del personaje (agachado, caminar, correr, disparar, golpe en el suelo), estado de la linterna, coberturas y oclusión del mapa, puertas cerradas o selladas, evento "atención llena" de S-01, uso de escalera o elevador (M-15), punto enfocado por una cámara hostil de S-10.

**Procesamiento / reglas:**

- La visión no atraviesa objetos sólidos (M-09).
- Ir agachado genera 0% de ruido, caminar ruido bajo; correr, disparar y el golpe en el suelo alertan a los enemigos cercanos (M-09, M-20). El remate con punzón de descarga no alerta.
- Iluminar a un enemigo duplica la velocidad a la que sube su alerta (M-12).
- Estados de alerta: **sospecha → búsqueda → caza** (M-10). Un enemigo cuenta como "en alerta" para el corte y el bloqueo de la vigilancia cuando está en **caza** (D-23). S-02 informa a S-01 y a S-11 del estado más alto dentro del alcance de M-03.
- **Atención de The Mother (D-24):** al recibir "atención llena", los enemigos cercanos que están en patrulla pasan a sospecha y luego a búsqueda hacia la posición del jugador. No baja el estado de los que ya están en búsqueda o caza (M-02).
- Al huir, los enemigos persiguen brevemente o golpean la puerta antes de volver a patrulla; el sellado rápido corta la persecución (M-14).
- **Sin persecución entre pisos (D-96, elimina D-26):** mientras haya enemigos en caza, la escalera y el elevador de la sala final quedan en cierre (panel rojo `LOCKDOWN` y línea de protocolo de The Mother, por ejemplo "Containment breach. Transit suspended."); al terminar la caza se crea el autoguardado y se abren. Ningún enemigo cambia de piso.
- **Cámaras hostiles (D-28, D-65):** en la fase 2 del jefe, cada vez que una cámara enfoca al jugador, The Mother libera un asimilado de su archivo que acude a ese punto; nunca hay más de 2 activos (S-10).
- Enfrentar a más de dos enemigos a la vez es casi muerte segura (M-04); S-06 limita a 2 los enemigos activos por sala (D-37).
- **Tipos (D-60, D-66):** el Alto vigila pasillos y golpea a distancia; la Boca grita y pone a los enemigos cercanos en búsqueda hacia ella; el Rastrero flanquea y no agarra; el Mudo no patrulla y se activa a ~3 m o al ser iluminado. Agarran el Alto y el Mudo (M-11). Valores en [[03 - Personajes]].

**Salidas:** Estado de alerta de cada enemigo, estado de alerta más alto dentro del alcance de M-03, eventos "entra en caza" y "sale de la caza", enemigos que siguen al jugador por la escalera, enemigos en espera junto al elevador, ataques y agarres.

**Estados posibles:** Patrulla, Sospecha, Búsqueda, Caza, Espera junto al elevador, Aturdido (empujón, punzón de descarga o punto débil), Muerto.

**Dependencias:** S-01 Cámara y vigilancia (atención de The Mother), S-06 Generación procedural (salas, rutas y coberturas), S-11 Audio dinámico (cada cambio de estado debe tener su señal).

**Interacciones:** S-01 (señal escalonada, corte y bloqueo de la vigilancia), S-03 (ataques y agarres), S-07 (sin enemigos en caza para el autoguardado; estado de alerta para el costo de salir), S-09 (los enemigos muertos pasan al reciclaje y los reciclados vuelven a la IA), S-10 (puntos enfocados por las cámaras hostiles).

**Datos que necesita:** Rutas de patrulla, coberturas, puertas y su estado, posición y ruido del personaje, haz de la linterna, posición del elevador de cada piso, posición del jugador al llenarse la atención.

**Datos que genera:** Estado de alerta por enemigo, ruta de persecución, eventos de detección, lista de perseguidores al usar la escalera o el elevador.

**Riesgos:**

- Sin señales claras la detección se siente injusta; depende de S-11.
- Si la persecución es demasiado corta, escapar siempre es seguro y la tensión desaparece (Pilar V).
- Si el cierre de tránsito dura demasiado, el jugador queda atrapado en la sala final; se mitiga porque la caza termina al perder el objetivo (M-10) y la sala se puede sellar con el hackeo (D-97).

**Criterios de aceptación (ambas configuraciones):**

- Un personaje agachado detrás de una cobertura sólida no es detectado por la vista.
- Todo cambio de estado de alerta produce una señal audible distinta en S-11.
- La interfaz no muestra indicadores de la percepción de los enemigos (D-02).
- Al llenarse la atención de The Mother, ningún enemigo cercano en patrulla permanece en patrulla.
- Al usar la escalera, de los enemigos que perseguían al jugador, solo los que estaban en caza llegan al piso siguiente, y lo hacen después del retraso.
- Al usar el elevador, ninguno de los perseguidores llega al piso siguiente; si el jugador vuelve por el elevador, los que lo perseguían están cerca del elevador.

### S-03: Salud y combate

**Objetivo:** Hacer que cada enfrentamiento cueste recursos y que el daño tenga consecuencias duraderas, sin convertir el juego en uno de acción.

**Entradas:** Ataques y agarres de S-02, disparo y recarga (`Clic izquierdo` apuntando, `R`), empujón (`Clic izquierdo` sin apuntar), curas usadas desde S-05.

**Procesamiento / reglas:**

- Salud fija sin regeneración con 3 estados: **Bien**, **Herido** y **Crítico**. Herido y Crítico reducen la velocidad un 15% y un 30% (valores iniciales, sujetos a balance; D-37) (M-05).
- Los enemigos matan en 2-4 golpes según su tipo (valores iniciales, sujetos a balance; D-66).
- 1 arma, el revólver de Kessler (D-74), con munición escasa: tambor de 6 balas, recarga bala a bala, blísteres de 3 cartuchos, 12 balas por casilla, 2–4 disparos para eliminar a un enemigo según su tipo y 2 para un reciclado, o 1 para un Rastrero reciclado (D-66), (valores iniciales, sujetos a balance; D-37). Recargar impide disparar y un golpe la cancela sin perder munición; acertar a un punto débil interrumpe el ataque (M-04, D-74).
- El empujón aparta y aturde brevemente, consume resistencia de S-04 y no protege contra el daño (M-06, D-05).
- Un agarre consume automáticamente el punzón de descarga y aturde al enemigo 2 s (valor inicial, sujeto a balance); sin objeto, el personaje baja un estado de salud o muere si estaba en Crítico (M-11). Agarran el Alto, el Mudo y el brazo recolector de The Mother (D-65, D-66).

**Salidas:** Estado de salud, munición restante, enemigos heridos, aturdidos o eliminados, evento de muerte del personaje para S-08.

**Estados posibles:** Bien, Herido, Crítico, Muerto.

**Dependencias:** S-02 IA y percepción, S-04 Resistencia, S-05 Inventario (curas, munición y punzón de descarga).

**Interacciones:** S-01 (el daño corta la vigilancia), S-08 (la muerte inicia el ciclo), S-09 (los enemigos eliminados pueden reciclarse), S-10 (fase 3 con disparos a puntos débiles), S-11 (chasquido del cargador vacío, jadeos).

**Datos que necesita:** Golpes por estado, daño por enemigo, munición y cargador, disparos por tipo de enemigo, duración del aturdimiento.

**Datos que genera:** Estado de salud actual, enemigos eliminados con su posición.

**Riesgos:**

- Munición o curas abundantes vuelven el juego de acción (Pilar V); la escasez se controla con el presupuesto de S-06.
- Un empujón barato convierte el cuerpo a cuerpo en la solución por defecto.

**Criterios de aceptación (ambas configuraciones):**

- La salud no se regenera sola en ningún estado.
- Usar una cura en Bien no está permitido; recibir daño durante la cura la cancela sin consumirla.
- El punzón de descarga se consume al ser agarrado (uno por agarre) o al rematar un cuerpo (D-71).
- Con los valores iniciales, cada tipo cae con sus disparos de D-66 y un reciclado con 2 (1 si es un Rastrero).

### S-04: Resistencia

**Objetivo:** Obligar al jugador a medir el esfuerzo físico con una sola barra compartida.

**Entradas:** Correr (`Shift izquierdo` + dirección), empujón y golpe en el suelo (`Clic izquierdo` sin apuntar).

**Procesamiento / reglas:**

- Correr, empujar y rematar con golpe en el suelo consumen la misma barra (M-06, M-20). Disparar no consume resistencia.
- Correr gasta ~10% por segundo; el empujón cuesta el 35% y el golpe en el suelo el 25% de la resistencia (valores iniciales, sujetos a balance; D-37).
- Se regenera tras 3 s sin estas acciones; caminar no interrumpe la regeneración (M-06).
- En 0 entra en agotamiento: no se puede correr ni empujar durante 2 s (M-06).
- Con resistencia insuficiente la acción no se ejecuta y se reproduce un jadeo.

**Salidas:** Valor de la barra, permiso para correr, empujar o rematar, evento de jadeo para S-11.

**Estados posibles:** Llena, Consumiéndose, Regenerándose, Agotamiento.

**Dependencias:** Ninguna.

**Interacciones:** S-01 (no se corre al apuntar), S-02 (correr genera ruido), S-03 (empujón), S-09 (golpe en el suelo).

**Datos que necesita:** Costo por acción, retraso de regeneración, velocidad de regeneración, duración del agotamiento.

**Datos que genera:** Resistencia actual y estado de agotamiento.

**Riesgos:** Si la barra se recupera demasiado rápido, correr y empujar dejan de ser decisiones (Pilar DC).

**Criterios de aceptación (ambas configuraciones):**

- Una acción sin resistencia suficiente no se ejecuta y suena un jadeo.
- Durante el agotamiento no se puede correr, empujar ni rematar con golpe en el suelo.

### S-05: Inventario

**Objetivo:** Forzar la priorización de recursos con espacio limitado y gestión en tiempo real.

**Entradas:** `Tab` para abrir y cerrar; `Clic izquierdo` sobre una casilla para usar o equipar; `Clic derecho` para descartar; recolección con `E` (S-02 nunca se detiene).

**Procesamiento / reglas:**

- **6 casillas fijas**; los objetos no se apilan infinitamente (M-07).
- Abrir el inventario no pausa el juego.
- Equipar, usar o descartar consume ~1.5 s (valor inicial, sujeto a balance).
- Ocupan casilla: curas, munición, punzón de descarga y llaves. El revólver, la linterna y el anillo no ocupan casilla (D-71).
- **Máximo por casilla (D-37):** curas 1, punzón de descarga 1, llave 1, munición 12 balas (valores iniciales, sujetos a balance).
- **Llaves (D-36):** cada llave ocupa 1 casilla y se consume al abrir su puerta (M-07, M-08).
- Con el inventario lleno, un objeto clave puede reemplazar a otro; si no se cambia, permanece en el suelo.
- El autoguardado de la sala clave final incluye el inventario que se trae (D-30, S-07).
- Al morir o reiniciar se pierde todo el inventario, llaves incluidas (D-18, D-32).

**Salidas:** Objetos disponibles para S-03 y S-09, llaves para las puertas de S-06, inventario para el autoguardado de S-07, objetos descartados en el suelo.

**Estados posibles:** Cerrado, Abierto, Animación de uso.

**Dependencias:** S-06 Generación procedural (recursos y llaves colocados), S-08 Ciclo de muerte (vacía el inventario).

**Interacciones:** S-03 (curas, munición, punzón de descarga), S-09 (punzón de descarga como remate), S-07 (el terminal del brazo se consulta junto al inventario con `Tab`; el inventario forma parte del autoguardado).

**Datos que necesita:** Tipos de objeto, cantidad máxima por casilla, duración de las animaciones.

**Datos que genera:** Contenido de las 6 casillas.

**Riesgos:** Si existen zonas totalmente seguras, el tiempo real pierde su efecto; si el punzón de descarga abunda, el agarre y el reciclaje pierden peso (mitigado por el presupuesto de S-06); la llave obligatoria resta una casilla hasta usarse (intencional, Pilar DC).

**Criterios de aceptación (ambas configuraciones):**

- Ninguna acción del inventario detiene a los enemigos.
- Nunca hay más de 6 casillas ocupadas ni una casilla por encima de su máximo.
- Usar una llave en su puerta la elimina del inventario.
- Tras la reconstrucción el inventario está vacío.
- Al continuar desde un autoguardado, el inventario es el mismo que al llegar a la sala clave final.

### S-06: Generación procedural por pisos

**Objetivo:** Construir los 3 pisos del intento con una estructura comprensible (salas clave + salas procedurales) que cambie entre intentos sin dejar pisos imposibles, en la configuración completa o en la showcase (D-11, D-33, D-34).

**Entradas:** Configuración activa (completa o showcase), catálogo de salas procedurales y salas clave, catálogo de documentos (de Emilia, de Vitalis Konzern y de The Mother; D-52), número de intento (primero o posterior, D-38), presupuesto de enemigos y recursos por piso (D-17, D-37), cobertura de cámaras (D-25), evento de reconstrucción de S-08, estado guardado de S-07 al continuar.

**Procesamiento / reglas:**

- **Estructura (D-33):** 3 pisos. Cada piso tiene salas procedurales, salas clave secundarias (documentos, llaves y, a veces, el interruptor de energía; sin guardado; D-72) y 1 sala clave final con escalera o elevador. En el piso 3, la sala de The Mother ocupa el lugar de la sala clave final y no guarda (M-15, M-19).
- **Parámetros por configuración (D-33, D-34, D-37):** valores iniciales, sujetos a balance.

| Parámetro | Completa | Showcase |
|---|---|---|
| Salas procedurales por piso | 8–12 | 6–8 |
| Salas clave secundarias por piso | 2–3 | 2 |
| Sala clave final por piso | 1 (en el piso 3, la sala de The Mother) | 1 (en el piso 3, la sala de The Mother) |
| Enemigos por piso | Piso 1: 6–9; piso 2: 9–12; piso 3: 12–15 + The Mother | Escalados en proporción al número de salas (D-34) |
| Recursos por piso | Presupuesto de [[06 - Economía y Recursos]] | Escalados en proporción al número de salas (ver [[06 - Economía y Recursos]]) |
| Duración aproximada | ~45–60 min | ~30 min |

- **Densidad de enemigos (D-37):** máximo 2 enemigos activos por sala y ~1/3 de las salas vacías a propósito. Como el showcase escala en proporción, conserva la misma densidad por sala que la configuración completa en cada piso. La curva de dificultad se documenta en [[05 - Progresión]].
- Las escaleras y los elevadores existen solo dentro de las salas clave finales. La posición de todas las salas clave cambia en cada intento (M-15).
- **Llaves (D-36):** cada piso tiene 1 llave obligatoria que abre la puerta de la sala clave final y puede tener llaves opcionales para atajos o salas con recursos. La ruta desde la entrada del piso hasta la llave obligatoria no atraviesa la puerta de la sala final; como las puertas con llave opcional solo dan a atajos o salas con recursos, la llave obligatoria nunca queda detrás de ellas. En el piso 3, la llave obligatoria abre la puerta de la sala de The Mother, que ocupa el lugar de la sala clave final (M-15, D-36; ver [[06 - Economía y Recursos]]).
- **Ruta garantizada:** cada piso tiene al menos una ruta válida entrada → llave obligatoria → sala clave final. Si la sala clave final empieza sin energía (~1 de cada 3 en los pisos 1 y 2), el generador garantiza una ruta al interruptor de una sala clave secundaria del mismo piso (D-72). Las salas procedurales combinan 6 arquetipos con el tema del piso (D-69, ver [[02 - Niveles]]).
- **Cobertura de cámaras (D-25):** todas las salas clave (secundarias, finales y la de The Mother) tienen cámara, más ~1 de cada 3 salas procedurales (valor inicial, sujeto a balance).
- **Documentos (D-52, M-08):**
    - **Documentos de Emilia:** 6 en total, 2 por piso, siempre en salas clave secundarias y nunca en salas procedurales. Se aplica igual en la configuración completa (2–3 secundarias por piso) y en la showcase (2 secundarias por piso, D-34); no se escalan con el factor de D-34. Son los únicos que cuentan para la decisión final (M-19, S-07).
    - **Documentos de Vitalis Konzern y de The Mother:** se colocan en salas procedurales como contexto del mundo y no cuentan para la decisión final. Cantidad: 12, 4 por piso, 1–2 por intento (D-79).
    - Como la posición de las salas clave cambia en cada intento (M-15), la posición de los documentos de Emilia dentro del piso también cambia.
    - Si cada documento de Emilia pertenece siempre al mismo piso y cuáles son corruptos (D-45): **Por definir** (contenido en [[04 - Diálogos y Narrativa]]). Cuántos documentos de Emilia puede contener una misma sala clave secundaria: **Por definir**.
    - Si el documento de Emilia de una sala clave secundaria ya está registrado en el terminal del brazo, la sala contiene recursos en su lugar (D-58, M-08).
- **Tutorial (D-38):** en el primer intento, las primeras salas del piso 1 son fijas y contienen el tutorial; desde el segundo intento esas salas son procedurales. Son 4 salas y cuentan dentro de las salas procedurales del piso 1 (D-70).
- Distancia mínima entre la reaparición (o la entrada del piso) y la sala clave final: ≥ 4 salas en la ruta más corta; ≥ 3 en la configuración de showcase (D-100).
- La sala de The Mother es una sala clave fija del piso 3 y contiene munición para todos los puntos débiles de la fase 3 más 1–2 disparos de margen (D-28, S-10).
- **Semilla por piso (D-101):** cada piso se genera con una semilla nueva cada vez que se entra en él, también al continuar desde un autoguardado: la sala clave final guardada se conserva como punto de aparición, pero el resto del piso cambia aunque el contador no suba. No se puede volver a un piso anterior; al cambiar de piso, el anterior se descarta.
- **Regeneración:** al morir o reiniciar se regeneran las salas procedurales, la posición de las salas clave, las llaves y la distribución de enemigos y recursos de todos los pisos (M-16). Al continuar desde un autoguardado, S-06 restaura el estado guardado por S-07 en lugar de generar uno nuevo (D-30).

**Salidas:** Grafo de salas por piso, posición de las salas clave, posición de la llave obligatoria y de las opcionales, salas con cámara, posición de enemigos y recursos, puertas y paneles hackeables, posición de los documentos de Emilia y de los de Vitalis Konzern y The Mother, distribución del piso para el autoguardado.

**Estados posibles:** Sin generar, Generando, Piso válido, Piso inválido (se descarta y se vuelve a generar), Restaurado desde autoguardado.

**Dependencias:** S-07 Persistencia (estado del intento al continuar), S-08 Ciclo de muerte (orden de regenerar).

**Interacciones:** S-01 (salas con cámara), S-02 (rutas de patrulla, coberturas y elevadores), S-05 (recursos y llaves colocados), S-07 (piso actual para el autoguardado; los documentos leídos se registran en el terminal del brazo), S-08 (llegada a la sala clave final del piso 2), S-09 (salas válidas para reaparecer reciclados), S-10 (sala del jefe y su munición).

**Datos que necesita:** Configuración activa, catálogo de salas, catálogo de documentos, reglas de conexión, parámetros por configuración, presupuesto por recurso, cobertura de cámaras, número de intento, distancia mínima a la sala clave final.

**Datos que genera:** Distribución de los 3 pisos del intento actual.

**Riesgos:**

- Si la sala clave final queda siempre cerca del inicio, el piso pierde tensión.
- Un presupuesto alto elimina la escasez (Pilar V); uno demasiado bajo hace pisos injugables.
- En el showcase, escalar mal los enemigos o los recursos rompe la escasez en menos salas.
- Si la generación no es legible, se pierde la Adaptación (principio 9 de [[01 - Jugabilidad]]).

**Criterios de aceptación:**

| Configuración | Criterio |
|---|---|
| Ambas | Cada intento genera exactamente 3 pisos y el piso 3 termina en la sala de The Mother. |
| Ambas | Ningún piso carece de una ruta válida entrada → llave obligatoria → sala clave final que no atraviese la puerta de la sala final ni puertas de llave opcional. |
| Ambas | Cada piso tiene exactamente 1 llave obligatoria y 1 sala clave final (o la sala de The Mother en el piso 3). |
| Ambas | Ninguna escalera o elevador aparece fuera de una sala clave final. |
| Ambas | Todas las salas clave tienen cámara y la proporción de salas procedurales con cámara es ~1/3 por piso, redondeada (tolerancia ±1 sala, valor inicial, sujeto a balance). |
| Ambas | Ninguna sala tiene más de 2 enemigos activos al generarse. |
| Ambas | En el primer intento las salas del tutorial son idénticas en todas las partidas; desde el segundo intento son procedurales. |
| Ambas | Cada piso respeta el mínimo y el máximo de cada recurso de su configuración. |
| Ambas | Al continuar desde un autoguardado, el jugador aparece en la sala clave final guardada y el resto del piso se genera con una semilla nueva (D-101). |
| Ambas | Ningún documento de Emilia aparece fuera de una sala clave secundaria. |
| Ambas | En un intento sin documentos de Emilia registrados, cada piso contiene exactamente 2 documentos de Emilia (6 en el intento). |
| Ambas | Ningún documento de Vitalis Konzern o de The Mother aparece en una sala clave. |
| Completa | Cada piso tiene 8–12 salas procedurales y 2–3 salas clave secundarias. |
| Completa | Enemigos generados: piso 1 entre 6 y 9; piso 2 entre 9 y 12; piso 3 entre 12 y 15, más The Mother. |
| Completa | En pruebas de juego, una partida sin muertes dura ~45–60 min. |
| Showcase | Cada piso tiene 6–8 salas procedurales y exactamente 2 salas clave secundarias (9–11 salas por piso en total). |
| Showcase | La cantidad de enemigos y de recursos por sala de cada piso queda dentro del rango por sala de la configuración completa para ese mismo piso. |
| Showcase | En pruebas de juego, una partida sin muertes dura ~30 min. |

### S-07: Persistencia

**Objetivo:** Conservar entre muertes y sesiones solo el conocimiento, nunca la fuerza del personaje, y permitir continuar el intento tras salir del juego o un fallo sin ofrecer un guardado manual.

**Entradas:** Documentos leídos y reglas descubiertas (M-08), fallas descubiertas, usadas o parcheadas (M-17), contador de muertes y degradación (S-08), evento de reconstrucción de S-08 (código de sujeto), victoria de S-10 (documentos completados), llegada a la sala clave final de los pisos 1 y 2 (S-06), estado de alerta de S-02, inventario de S-05, evento de salir del juego, "Nueva partida" (D-32), arranque del juego tras un cierre.

**Procesamiento / reglas:**

- El **terminal del brazo** conserva bestiario, lore, documentos (incluidos los de la esposa, Emilia), reglas del sistema (cómo se forma un código, no su valor) y fallas de la máquina (M-16). Consultarlo no pausa la partida. Las entradas del bestiario son informes clínicos de The Mother sobre sujetos asimilados y se desbloquean en el primer encuentro (D-48).
- **Código de sujeto (D-43, D-47, D-50):** S-07 guarda el código R-H con el que The Mother registra a Lukas y lo muestra en el terminal del brazo. La primera reconstrucción lo registra como **R-H-07**; cada evento de reconstrucción de S-08 lo sube 1 (R-H-08, R-H-09…), incluidas las de "Reiniciar" y las del primer intento, aunque estas no sumen al contador (D-35). Es solo presentación: no tiene efecto mecánico. "Nueva partida" lo devuelve a R-H-07 (D-32). Una salida con calma con costo (D-31) también lo sube (D-94). Tras un final, el ciclo nuevo conserva el código (D-93).
- **Documentos de Emilia (D-51, D-52):** S-07 lleva el conteo de documentos de Emilia registrados (0 a 6), acumulados en cualquier intento. Los documentos de Vitalis Konzern y de The Mother se registran, pero no suman al conteo. El conteo nunca disminuye al morir, reiniciar o salir; solo "Nueva partida" lo borra. Con 6, S-07 informa a S-10 y S-08 que la decisión final está disponible (M-19).
- **Documentos corruptos (D-45):** algunos documentos se registran corruptos, con partes del texto ilegibles, sobre todo los relacionados con la memoria de Lukas. La corrupción nunca afecta a reglas del sistema ni a pistas de ruta, que siempre se registran legibles (M-08). "Corrupto" no equivale a una falla de la máquina (M-17).
- **Mensaje de Emilia (D-50, D-53):** tras la primera reconstrucción, The Mother copia el contenido del teléfono personal perdido al terminal del brazo como "efectos personales del sujeto"; la nota de voz queda corrupta, como transcripción con sonido vocal y forma de onda (M-16).
- **Desenlace (D-45, D-51, D-55):** al derrotar a The Mother, si el terminal tiene los 6 documentos de Emilia, los documentos corruptos y el mensaje de Emilia se completan (paso 2 de la secuencia común de victoria, M-19); sin ellos siguen corruptos. Tras los créditos, el terminal conserva los documentos completos en el ciclo nuevo (D-93).
- El terminal y las fallas se autoguardan cada vez que cambian. Si el personaje muere mientras se registra información, esta se guarda antes de procesar la muerte.
- The Mother parchea cada falla tras 1-2 usos o al morir (valor inicial, sujeto a balance); el parche también persiste (M-17).
- **Autoguardado del intento (D-30, modifica D-22):** no existe guardado manual y la partida no se conserva en el punto exacto de salida.
    - Se crea al llegar a la **sala clave final** de los pisos 1 y 2, solo si no hay enemigos en caza; si el jugador llega perseguido, queda pendiente hasta que la caza termina. Las salas clave secundarias y la sala de The Mother nunca guardan.
    - Guarda el estado al llegar, incluido el inventario (S-05) y el piso actual; no guarda la distribución, porque el piso se regenera con una semilla nueva al continuar (S-06, D-101).
    - Muestra un aviso discreto y diegético en el terminal del brazo ("sincronizando…") sin interrumpir el control. La cinemática de transición de piso también muestra el aviso (D-39, S-08).
    - Solo sirve para continuar después de salir del juego o de un fallo; no cambia el punto de reaparición (M-16).
    - Se conserva hasta morir, reiniciar o llegar a otra sala clave final, que lo sustituye.
    - Al cargarlo, la sala final guardada empieza sin enemigos dentro; no hay gracia (D-97).
- **Salir del juego (D-31):** el diálogo de confirmación avisa del costo antes de salir. Las reglas completas y los casos especiales están en [[07 - Reglas y Estados del Juego]].

| Situación al salir | Costo | Al volver |
|---|---|---|
| Durante alerta o caza | Muerte completa: suma al contador y a la degradación | Inicio del ciclo en el piso 1 |
| Con calma, fuera de una sala clave final ya guardada | Suma 1 al contador y a la degradación | Desde la sala clave final autoguardada o, si no existe, inicio del ciclo |
| En una sala clave final ya guardada y sin alerta | Ninguno | Desde la sala clave final autoguardada |
| Cierre inesperado o fallo del juego | Ninguno | Desde el último autoguardado o, si no existe, inicio del ciclo |

    En el primer intento, salir no suma al contador (misma excepción de D-35). Si se sale con calma o hay un fallo antes de existir un autoguardado, se continúa desde el inicio del ciclo, porque no hay otro punto guardado (regla global 4 de [[07 - Reglas y Estados del Juego]]). Con D-23, "en alerta" equivale a caza para la vista de vigilancia; para el costo de salir, "alerta" también es solo la caza (D-96).
- S-07 distingue una salida confirmada de un cierre inesperado registrando la salida confirmada en el archivo de guardado antes de cerrar.
- **Nueva partida (D-32):** tras la confirmación, borra todo el archivo de guardado, incluido el terminal del brazo, las fallas, el contador y la degradación.

**Salidas:** Archivo de guardado en disco, contenido del terminal del brazo, código de sujeto actual, conteo de documentos de Emilia y disponibilidad de la decisión final para S-08 y S-10, aviso "sincronizando…", estado del intento para S-06 al continuar, costo de salida para S-08.

**Estados posibles:** Sincronizado, Guardando (aviso "sincronizando…"), Autoguardado pendiente (esperando que termine la caza), Cargando (pantalla de arranque del terminal, D-96).

**Dependencias:** S-02 IA y percepción (enemigos en caza), S-06 Generación procedural (regenera el piso al continuar), S-08 Ciclo de muerte (orden de guardar antes de la muerte y descarte del autoguardado).

**Interacciones:** S-05 (el terminal comparte la tecla `Tab` con el inventario; el inventario forma parte del autoguardado), S-06 (restaura el estado del intento al continuar; coloca los documentos que se registran), S-08 (salir con costo suma al contador y a la degradación; cada reconstrucción sube el código de sujeto; el conteo de documentos decide la cinemática de victoria), S-10 (fallas que acortan la fase 1; la sala del jefe no guarda; conteo de documentos para la decisión final).

**Datos que necesita:** Entradas del terminal, marca de corrupto y tipo (Emilia, Vitalis Konzern o The Mother) de cada documento, código de sujeto, estado de cada falla, contador de muertes y nivel de degradación (si M-18 se implementa), marca de primer intento, estado del intento en la última sala clave final, estado de alerta al salir.

**Datos que genera:** Archivo de guardado (terminal, código de sujeto, conteo de documentos de Emilia, fallas, contador, degradación, autoguardado del intento y marca de salida confirmada).

**Riesgos:**

- Si la información persistente resuelve los pisos, desaparece la incertidumbre; solo persisten reglas, no valores.
- Forzar el cierre del proceso durante una caza se trata como un cierre inesperado y evita el costo de D-31; se acepta y no se cobra (D-96).
- Si el aviso "sincronizando…" interrumpe o tapa información, rompe la tensión; debe ser discreto (D-30).

**Criterios de aceptación (ambas configuraciones):**

- Ningún menú ofrece guardar manualmente.
- Llegar a la sala clave final de los pisos 1 o 2 sin enemigos en caza crea un autoguardado y muestra "sincronizando…" sin pausar ni quitar el control.
- Llegar perseguido no guarda hasta que el último enemigo sale de la caza.
- Las salas clave secundarias y la sala de The Mother nunca crean un autoguardado.
- Al continuar, el jugador aparece en la sala clave final guardada con el mismo inventario, y ningún enemigo ataca durante los primeros 2–3 s.
- Morir o reiniciar descarta el autoguardado del intento y conserva el terminal del brazo.
- Cada situación de la tabla de salida aplica exactamente su costo; un cierre inesperado no suma al contador ni a la degradación.
- "Nueva partida" no borra nada sin confirmación y, tras confirmarla, el terminal del brazo está vacío.
- Ninguna entrada del terminal se pierde al morir, reiniciar ni cerrar el juego.
- Ningún dato persistente mejora la salud, la resistencia o el daño del personaje.
- En una partida nueva, la primera reconstrucción muestra R-H-07 en el terminal del brazo y cada reconstrucción posterior, incluidas las de "Reiniciar" y las del primer intento, muestra el código siguiente.
- El conteo de documentos de Emilia solo suma documentos de Emilia, llega como máximo a 6 y no disminuye al morir, reiniciar ni cerrar el juego.
- Ninguna parte corrupta de un documento oculta una regla del sistema ni una pista de ruta.

### S-08: Ciclo de muerte y reconstrucción

**Objetivo:** Integrar la muerte al ciclo: el personaje se reconstruye, siempre es el mismo hombre recompuesto con piezas ajenas, y vuelve más informado, no más fuerte (D-18). También presenta el ciclo con las cinemáticas de D-39.

**Entradas:** Evento de muerte de S-03 (incluido un agarre en Crítico), "Reiniciar" del menú de pausa (D-32), salida con costo de S-07 (D-31), llegada a la sala clave final del piso 2 (S-06, D-35), fin de la introducción, uso de una escalera o un elevador (M-15), `Espacio` en una cinemática saltable (ver [[04 - Controles]]), victoria de S-10, conteo de documentos de Emilia de S-07, `E` sobre la terminal central o la salida en la decisión final (M-19).

**Procesamiento / reglas:**

- **Muerte y Reiniciar (D-32):** ambos siguen el mismo proceso: S-07 guarda primero cualquier registro pendiente, se pierde todo el inventario (D-18), se descarta el autoguardado del intento, S-06 regenera los pisos y el personaje reaparece al inicio del ciclo en el piso 1 (M-16). Morir en la sala de The Mother también regresa al inicio del ciclo.
- **Cinemáticas (D-39):**

| Cinemática | Cuándo | Contenido | Saltable |
|---|---|---|---|
| Completa de reconstrucción | Una sola vez, al terminar la introducción | Todo el proceso de reconstrucción en imágenes fugaces con cortes a negro; es el momento del olvido de Emilia (D-45, D-53). Lukas muere al final de la introducción atacado por la espalda en un piso que no existe (D-53); resto del detalle en [[01 - Premisa y Trama]] | Tras verla una vez (D-91) |
| Corta de reconstrucción | En cada muerte o reinicio | El personaje cae de su cápsula, en la fila de cápsulas del piso 1 (D-59), con sonido de maquinaria; refleja la degradación acumulada y la pieza ajena visible de la reconstrucción (M-18, D-43) | Sí, con `Espacio` |
| Hallazgo del piso 2 | Una sola vez, la primera vez que se llega a la sala clave final del piso 2 sin enemigos en caza; si los hay, espera a que la caza termine (D-67) | La foto familiar imposible, con la cara de Emilia borrosa; provoca obstinación y acompaña el reinicio del contador (D-56) | No (D-67) |
| Transición de piso | Al usar la escalera o el elevador | Ascensor o escalera; muestra el aviso de autoguardado | No |

- **Introducción (D-53, D-63):** las partes jugables (compuerta y piso que no existe) usan un HUD mínimo: moverse, mirar e interactuar, sin inventario, salud visible, linterna, armas ni vigilancia. La compuerta se hackea en el teléfono de Lukas con las reglas de M-13. El elevador es cinemática. La muerte final está guionizada (trigger con temporizador de respaldo) y no suma al contador ni a la degradación.
- **Transición de piso:** dura un **mínimo de 5 s** y oculta la carga del piso siguiente; si la carga tarda más, la cinemática se alarga. No existe pantalla de carga (D-39).
- **Deseable (M-18), contador de 5 muertes (D-35):** suman cada muerte, "Reiniciar" y salir con costo. Las del primer intento (el del tutorial) no suman. Solo llegar a la sala clave final del piso 2 reinicia el contador; después sigue contando en el piso 3 y en la sala de The Mother (D-56). Al llegar a 5, se activa el final "Asimilación" (D-42), también si la quinta se debe a "Reiniciar" o a salir con costo.
- **Deseable (M-18), degradación (D-29):** cada muerte, reinicio o salida con costo suma degradación estética (corrupción CRT y distorsión del sonido vocal; el texto de Lukas solo se degrada en la cinemática de Asimilación, D-62) sin efecto en la jugabilidad. Solo se acumula y nunca se reinicia, ni siquiera al llegar a una sala clave; solo "Nueva partida" la borra (D-32). **Tope visual:** ninguna capa tapa la interfaz crítica ni las señales de M-10. La degradación sí suma durante el primer intento (D-94); D-35 solo exime al contador. El ciclo nuevo tras un final la reinicia (D-93).
- **Piezas ajenas (D-43):** provienen del laboratorio. En cada reconstrucción se nota una pieza ajena más en el modelo de Lukas (3–5 variantes, valor inicial, sujeto a balance); al llegar a la última variante, el modelo se mantiene en ella. Es solo presentación (M-18).
- **Código de sujeto (D-43):** cada reconstrucción, incluida la primera y las de "Reiniciar", envía a S-07 el evento que sube el código R-H (S-07).
- **Cinemáticas finales (D-42, D-51):** S-08 reproduce el final de derrota y el cierre de la victoria que entrega S-10. Las reglas de la secuencia y de la decisión final están en M-19 y M-18 de [[02 - Mecánicas]]; los estados y transiciones, en [[07 - Reglas y Estados del Juego]].

| Cinemática | Cuándo | Contenido | Saltable |
|---|---|---|---|
| Asimilación (derrota) | Quinta muerte contada desde el último reinicio del contador (D-35, D-56), si M-18 se implementa | El laboratorio termina de asimilar a Lukas y este se convierte en uno de los enemigos comunes que enfrentaba (D-42, D-48): imágenes deformadas de la cápsula, informe de The Mother y Lukas como señuelo en el piso que no existe (D-61) | Tras verla una vez (D-91) |
| Secuencia común de victoria (pasos 1 a 3) | Al derrotar a The Mother (S-10) | Cae The Mother y revela que existe algo más, sin poder explicarlo; solo con los 6 documentos de Emilia, los documentos corruptos y el mensaje de Emilia se completan y Lukas recupera primero el nombre de Emilia y después el suyo (D-45, D-55); **Despertar**: el latido del laboratorio se acelera (S-11) | Tras verla una vez (D-91) |
| Relevo básico | Tras el Despertar, con menos de 6 documentos de Emilia (S-07): Lukas examina la terminal central de forma automática | Algo ocurre y Lukas reemplaza automáticamente a The Mother, sin recordar ningún nombre (D-55); el terminal del brazo lo conecta, porque es el último con identidad | Tras verla una vez (D-91) |
| Relevo completo | Con los 6 documentos, el jugador usa `E` en la terminal central durante la decisión final | Misma base que el Relevo básico, con el expediente R-H-01 a R-H-06 "estado desconocido" (D-49) y el segundo acto: Lukas oye el latido y lo entiende; el jugador solo ve un glitch ilegible (D-61). Lukas reemplaza a The Mother sabiendo más | Tras verla una vez (D-91) |
| Ambiguo | Con los 6 documentos, el jugador usa `E` en la salida durante la decisión final | En negro, el latido de Lukas y el del laboratorio suenan desfasados y se sincronizan en uno; silencio; después, el gancho (D-61). No se sabe si Lukas fue asimilado, si se convirtió en The Mother o si ocurrió otra cosa | Tras verla una vez (D-91) |
| Gancho abierto (paso 5) | Al terminar Relevo básico, Relevo completo o Ambiguo | Solo se escucha una nueva llegada y la voz (sonido) de The Mother iniciando el protocolo (D-51, D-52) | Tras verla una vez (D-91) |

- **Decisión final (D-51):** con los 6 documentos de Emilia, tras el Despertar el jugador recupera el control en la sala de The Mother y elige con `E` entre la terminal central y la salida, sin menú; si ambos están al alcance, gana el que está al centro de la vista (D-27, M-19). Sin los 6 documentos no hay elección. Durante la decisión hay presión sin muerte: los altavoces y el latido suben, la sala tiembla y las luces fallan, pero no hay daño ni enemigos (D-61, M-19).

**Salidas:** Personaje reconstruido con el inventario vacío, orden de regenerar a S-06, orden de descartar el autoguardado a S-07, evento de reconstrucción para el código de sujeto de S-07, contador de muertes y nivel de degradación para S-01, S-07 y S-11, evento Despertar para S-11, cinemática en curso, final "Asimilación" (D-42), cinemática de victoria (Relevo básico, Relevo completo o Ambiguo, D-51).

**Estados posibles:** Cinemática de introducción, Vivo, Muerte, Cinemática corta, Reconstrucción, Transición de piso, Final: Asimilación, Victoria, Decisión final, Relevo básico, Relevo completo, Ambiguo.

**Dependencias:** S-03 Salud y combate, S-07 Persistencia.

**Interacciones:** S-01 (la degradación no tapa la señal de vigilancia), S-05 (vacía el inventario), S-06 (regenera los pisos; carga del piso siguiente durante la transición), S-07 (descarta el autoguardado; aviso durante la transición; costo de salir; sube el código de sujeto; conteo de documentos de Emilia para elegir la cinemática de victoria), S-10 (muerte en la sala del jefe; victoria), S-11 (la degradación no oculta las señales; Despertar y latido del laboratorio en las cinemáticas finales).

**Datos que necesita:** Punto de reaparición, contador de muertes, nivel de degradación, variante del cuerpo ajeno, marca de primer intento, marca de cinemática completa ya vista, estado de la carga del piso siguiente, conteo de documentos de Emilia.

**Datos que genera:** Contador de muertes, nivel de degradación y marca de cinemática completa ya vista.

**Riesgos:**

- Una reconstrucción demasiado lenta rompe el ritmo del ciclo; se mitiga con la cinemática corta saltable.
- La corrupción visual o de audio puede ocultar información crítica; se mitiga con el tope visual (D-29).
- Si la transición de piso se alarga mucho por la carga, la espera se percibe como una pantalla de carga encubierta.

**Criterios de aceptación (ambas configuraciones):**

- Tras la reconstrucción el inventario está vacío, el terminal del brazo está intacto y el personaje está al inicio del ciclo en el piso 1.
- "Reiniciar" produce el mismo resultado que una muerte, incluida la cinemática corta.
- La cinemática completa se reproduce al terminar la introducción y no se reproduce en ninguna muerte ni reinicio.
- La cinemática corta se reproduce en cada muerte y reinicio; saltarla con `Espacio` deja el mismo estado que verla completa.
- La transición de piso nunca dura menos de 5 s, nunca muestra una pantalla de carga y no termina antes de que el piso siguiente esté cargado.
- Si M-18 se implementa: las muertes del primer intento no suman al contador; el contador solo se reinicia en la sala clave final del piso 2; el final se activa exactamente en la quinta muerte contada.
- Si M-18 se implementa: el nivel de degradación nunca disminuye salvo con "Nueva partida" y, con la degradación máxima, la interfaz crítica y las señales de M-10 siguen visibles y audibles.
- Toda victoria reproduce la secuencia común (revelación y Despertar; con los 6 documentos de Emilia, también los documentos completados y la recuperación de los nombres, D-55) antes de su cinemática de victoria, y toda cinemática de victoria termina con el gancho abierto.
- Con menos de 6 documentos de Emilia, los documentos y el mensaje siguen corruptos y Lukas no recupera ningún nombre (D-55).
- Con menos de 6 documentos de Emilia, la victoria reproduce el Relevo básico sin presentar la terminal central ni la salida como elección.
- Con los 6 documentos de Emilia, `E` en la terminal central reproduce el Relevo completo y `E` en la salida reproduce el Ambiguo; no aparece ningún menú de elección.
- El Relevo completo muestra el expediente R-H-01 a R-H-06 con "estado desconocido"; el Relevo básico no lo muestra.

### S-09: Reciclaje de enemigos

**Objetivo:** Evitar que eliminar a un enemigo sea definitivo y gratuito, obligando a decidir entre rematar o asumir que volverá (D-20, D-21).

**Entradas:** Enemigo eliminado por S-03, remate con golpe en el suelo (`Clic izquierdo` sin apuntar sobre un cuerpo) o con punzón de descarga (`E` sobre un cuerpo), salida del jugador de la sala, sala actual (sala del jefe o no).

**Procesamiento / reglas:**

- Los enemigos eliminados siguen muertos durante el intento, pero el laboratorio recicla **todos** los cuerpos no rematados, cada uno **una sola vez**; el aviso empieza ~40 s después de la muerte (M-20, D-99).
- **Sin reciclaje en la sala del jefe (D-28):** los enemigos eliminados en la sala de The Mother no se reciclan.
- **Aviso:** el cuerpo se convulsiona y emite crujidos de hueso y carne durante ~3 s (S-11).
- **Reaparición:** cuando el jugador sale de la sala, en otra sala del mismo piso, nunca frente al jugador.
- **Enemigo reciclado:** un 25% más rápido y muere con 2 disparos (valores iniciales, sujetos a balance; D-37). No vuelve a reciclarse.
- **Remate:** golpe en el suelo (25% de la resistencia, ~2 s, alerta a enemigos cercanos) o punzón de descarga (1 casilla, ~1.5 s, no alerta; gasta la defensa contra agarres, D-71) (valores iniciales, sujetos a balance). Solo mientras el cuerpo sigue en la sala.
- **Disponibilidad del remate (D-27):** a ~1.5 m (valor inicial, sujeto a balance) mirando un cuerpo no rematado, también durante el combate; si hay otro elemento interactivo al lado, gana el que está al centro de la vista (M-20). Con cuerpos que no pueden reciclarse (reciclados y sala del jefe) el contexto no se activa (M-20).
- Recibir un golpe durante el golpe en el suelo lo cancela.
- La sala de la que se huyó sigue infestada, incluidos los reciclados (M-14).

**Salidas:** Cuerpo rematado, cuerpo desaparecido, enemigo reciclado entregado a S-02, ruido para S-02, aviso para S-11.

**Estados posibles:** Cuerpo en la sala, Aviso, Rematado, Desaparecido, Reciclado activo, Reciclado eliminado.

**Dependencias:** S-02 IA y percepción, S-04 Resistencia, S-05 Inventario, S-11 Audio dinámico.

**Interacciones:** S-06 (salas válidas del mismo piso para la reaparición), S-03 (disparos para eliminar a un reciclado), S-10 (sin reciclaje en la sala del jefe).

**Datos que necesita:** Marca de "ya reciclado" por enemigo, posición del jugador, salas del piso actual, marca de sala del jefe.

**Datos que genera:** Estado de cada cuerpo y de cada enemigo reciclado durante el intento.

**Riesgos:**

- Desgaste sin fin; mitigado con el reciclaje único.
- Saturar el inventario con consumibles de remate; mitigado con un presupuesto bajo.

**Criterios de aceptación (ambas configuraciones):**

- Ningún enemigo se recicla más de una vez.
- Ningún enemigo eliminado en la sala de The Mother se recicla.
- Un reciclado nunca reaparece en la sala del jugador ni en su campo de visión.
- El golpe en el suelo alerta a los enemigos cercanos y el punzón de descarga no.
- Con un cuerpo ya rematado, `Clic izquierdo` y `E` recuperan el empujón y la interacción.
- Ninguna tecla tiene dos funciones en el contexto "sobre un cuerpo" (ver [[04 - Controles]]).

### S-10: Jefe The Mother

**Objetivo:** Cerrar la demo con un encuentro que exige las herramientas aprendidas durante el ciclo, sin sistemas nuevos (D-16).

**Entradas:** Entrada a la sala de The Mother (sala clave fija del piso 3), inventario que trae el jugador, fallas registradas en S-07, posición del jugador respecto de las cámaras de la sala.

**Procesamiento / reglas:**

- Llegar a la sala no crea un autoguardado (D-30).
- **Sala (D-59):** la central de megafonía desde donde se emitió el latido en el incidente; es una sola sala que se transforma en cada fase y no tiene carne. The Mother es un archivo de muestras criogénicas en una grúa (D-65). Emite una línea de protocolo al inicio de cada fase (D-62).
- **Brazo recolector (D-65):** recorre la sala en las 3 fases; si alcanza al jugador, lo agarra con las reglas de M-11 (S-03). Lukas puede morir en cualquier fase.
- 3 fases de 1-2 min cada una (valor inicial, sujeto a balance) (M-19):
    1. **Hackeo de nodos bajo presión** (M-13) en las mesas de mezcla y los racks de cinta (D-59), con el brazo recolector patrullando los nodos (D-65); las fallas acortan las secuencias. Sin M-17 funciona con la longitud original.
    2. **Supervivencia y huida con cámaras hostiles (D-28):** cada cámara de la sala muestra una luz roja y emite el sonido de su motor al girar (S-11). Cada vez que una cámara enfoca al jugador, S-10 libera un asimilado desde una cápsula del archivo y envía ese punto a S-02; el asimilado acude a él. Nunca hay más de 2 activos; 4 en total en la fase (D-65, D-80). Una niebla criogénica reduce la visibilidad sin ralentizar ni tapar las señales de M-10 (D-65). El jugador avanza entre los puntos ciegos de las cámaras. La vista de vigilancia no está disponible durante esta fase (S-01). Valores iniciales (D-98): 3 cámaras que barren a ~20°/s con un cono de ~12 m y liberan tras ~1 s enfocando; el brazo recolector recorre los rieles a ~2.5 m/s, alcanza ~3 m y avisa ~1 s antes de agarrar.
    3. **Pocos disparos a puntos débiles expuestos** (M-04): transformadores de alto voltaje que quedan expuestos al abrirse el núcleo criogénico de The Mother (D-59, D-65). Al destruir el último, la estantería se apaga y The Mother cae. La sala contiene munición para todos los puntos débiles más 1–2 disparos de margen (valor inicial, sujeto a balance; D-28), aunque el jugador llegue sin munición. Son 3 transformadores de 2 disparos cada uno (D-80).
- **Sin reciclaje (D-28):** los enemigos eliminados en la sala no se reciclan (S-09).
- Morir en la sala regresa al inicio del ciclo (S-08).
- Derrotarla es la condición de victoria. Al ser derrotada revela que existe algo más (la criatura lovecraftiana que es el laboratorio), pero no puede explicarlo ([[01 - Jugabilidad]], D-47). S-10 entrega la victoria a S-08, que reproduce la secuencia común de victoria, la decisión final y la cinemática de victoria correspondiente (M-19, D-51).
- **Decisión final (D-51):** tras el Despertar, la sala presenta la terminal central y la salida como elementos interactivos solo si S-07 informa los 6 documentos de Emilia (ver S-08).
- D-34 no define diferencias del encuentro entre la configuración completa y la showcase.
- La relación entre Emilia, la instalación y The Mother se documenta en D-47, D-49, [[02 - Mundo]] y [[03 - Personajes]].

**Salidas:** Fase actual, puntos enfocados por las cámaras hostiles para S-02, estado de las cámaras para S-11, victoria y revelación narrativa para S-08, terminal central y salida activas en la decisión final, o muerte del personaje.

**Estados posibles:** Inactivo, Fase 1, Fase 2, Fase 3, Derrotada.

**Dependencias:** S-01 Cámara y vigilancia, S-03 Salud y combate, S-06 Generación procedural (sala fija y su munición), S-07 Persistencia (fallas y conteo de documentos de Emilia).

**Interacciones:** S-02 (los enemigos acuden al punto enfocado), S-04 (huida en la fase 2), S-08 (muerte en la sala), S-09 (sin reciclaje en la sala), S-11 (luz roja, motor de las cámaras y señales del encuentro).

**Datos que necesita:** Nodos de la fase 1 y longitud de sus secuencias, cámaras de la sala con sus puntos ciegos, puntos débiles de la fase 3, munición de la sala.

**Datos que genera:** Progreso de fases, puntos enfocados por las cámaras y evento de victoria.

**Riesgos:**

- La fase 3 puede volver el juego de acción; debe mantenerse corta.
- Si los puntos ciegos no se leen con la luz roja y el sonido del motor, la fase 2 se siente injusta (principio 3 de [[01 - Jugabilidad]]).

**Criterios de aceptación (ambas configuraciones):**

- Las 3 fases se completan solo con mecánicas existentes.
- Entrar en la sala no crea un autoguardado.
- En la fase 2, cada cámara muestra su luz roja y su motor suena al girar; cada enfoque libera un asimilado que se dirige a ese punto y nunca hay más de 2 activos (D-65).
- El brazo recolector puede agarrar al jugador en las 3 fases y aplica la regla de M-11.
- La sala tiene puntos ciegos que permiten atravesar la fase 2 sin ser enfocado.
- Al empezar la fase 3, la sala contiene munición para todos los puntos débiles más 1–2 disparos de margen, aunque el jugador llegue sin munición.
- Ningún enemigo eliminado en la sala se recicla.
- La derrota de The Mother dispara la revelación y la victoria.

### S-11: Audio dinámico

**Objetivo:** Comunicar la percepción y la cercanía de las amenazas sin indicadores en pantalla, de modo que los riesgos puedan interpretarse (principio 3 de [[01 - Jugabilidad]]).

**Entradas:** Estado de alerta de S-02 (incluido el estado más alto dentro del alcance de M-03), distancia a la amenaza más cercana, superficie bajo los pies, estado de las cámaras hostiles de S-10, nivel de degradación de S-08, evento Despertar y cinemáticas finales de S-08, eventos de S-03, S-04, S-09 y M-13.

**Procesamiento / reglas:**

- **Señales de alerta:** un sonido distinto para sospecha, búsqueda y caza; con varios enemigos se prioriza el estado más alto (M-10).
- **Coherencia con la señal de vigilancia (D-23):** la degradación de la señal de las cámaras es una señal diegética de M-10 que presenta S-01. S-01 y S-11 usan el mismo estado de alerta más alto dentro del alcance de M-03, de modo que el sonido de alerta y la calidad de la señal nunca se contradicen (sospecha: señal sin cambios; búsqueda: señal degradada; caza: señal cortada). Así el jugador percibe la atención de The Mother (D-24) sin indicadores.
- **Latido (del protagonista):** sube con la proximidad de una amenaza; su intensidad es aproximada para no funcionar como radar y no se activa por enemigos al otro lado de una puerta sellada. Es la única señal de proximidad basada en un latido.
- **Latido del laboratorio (D-46, M-10):** capa ambiental propia del laboratorio, con un timbre grave distinto del latido del protagonista.
    - Nunca varía con la cercanía de enemigos ni con el estado de alerta; S-11 no le aplica la distancia a la amenaza ni las señales de S-02. Es el primer acto del texto (D-64): ningún enemigo lo emite.
    - Crece con el nivel de degradación acumulada de S-08 (M-18) y sube mientras la salud de Lukas está en Crítico (S-03, D-95); en el final se acelera en el Despertar (paso 3 de la secuencia común de victoria, M-19). En la cinemática Ambiguo se escuchan ambos latidos, el del laboratorio y el del protagonista, que se sincronizan en uno antes del silencio y del gancho (D-51, D-61).
    - Como la degradación solo se acumula (D-29), el latido del laboratorio nunca baja durante la partida, salvo al salir de Crítico, que retira solo ese aumento; "Nueva partida" o el ciclo nuevo tras un final (D-93) lo devuelven a su nivel base.
    - Así no compite con la señal de proximidad: el jugador asocia el latido del protagonista con las amenazas y el del laboratorio con el ciclo.
    - Se oye por primera vez de fondo en la nota de voz de Emilia (introducción y su copia en el terminal) y, desde la primera reconstrucción, en todas las salas de los pisos 1 a 3 (D-95). Nivel base y curva de crecimiento: sujetos a balance.
- **Grito de la Boca (D-64, D-66):** sonido propio, distinto del latido del laboratorio y de las señales de alerta; los enemigos cercanos van hacia ella.
- **Pasos:** suenan distinto según la superficie (metal, agua, escombros).
- **Avisos del sistema:** sonido orgánico antes del reciclaje (M-20, D-99), jadeo por resistencia insuficiente (M-06), chasquido del cargador vacío (M-04), pitido de error del hackeo (M-13).
- **Cámaras hostiles (D-28):** en la fase 2 del jefe, cada cámara emite el sonido de su motor al girar, junto a su luz roja (M-10, S-10).
- **Deseable (M-18), degradación (D-29):** la distorsión del sonido vocal y la corrupción CRT de la degradación se acumulan y nunca se reinician. Ninguna capa tapa las señales de M-10: las señales de alerta, el latido, los avisos del sistema y el motor de las cámaras se mantienen audibles, y la corrupción CRT de la degradación se mantiene distinguible de la señal de vigilancia degradada por búsqueda. El latido del laboratorio crece con la degradación, pero respeta el mismo tope: nunca tapa el latido del protagonista ni las señales de alerta.

**Salidas:** Mezcla de audio en tiempo real; nivel de capa de degradación permitido para S-01 y la interfaz.

**Estados posibles:** Ambiente sin amenaza, Sospecha, Búsqueda, Caza (con la intensidad del latido del protagonista variando dentro de cada estado). El latido del laboratorio es una capa ambiental independiente de estos estados: suena igual en todos ellos y solo cambia con la degradación y en el Despertar.

**Dependencias:** S-02 IA y percepción, S-08 Ciclo de muerte (nivel de degradación, Despertar y cinemáticas finales), S-10 Jefe (estado de las cámaras hostiles).

**Interacciones:** S-01 (compensa la falta de visión periférica; misma prioridad de estado que la señal de vigilancia), S-08 (degradación con tope; latido del laboratorio en el Despertar y en las cinemáticas finales), S-09 (aviso orgánico), S-10 (cámaras hostiles y señales del encuentro final).

**Datos que necesita:** Estado de alerta por enemigo, distancias, puertas selladas, tipo de superficie, estado de giro de cada cámara hostil, nivel de degradación, evento Despertar.

**Datos que genera:** Ninguno persistente.

**Riesgos:**

- Un latido demasiado preciso funciona como radar y elimina la incertidumbre (Pilar A).
- Señales ambiguas hacen que la detección se sienta injusta.
- Con mucha degradación, la corrupción CRT puede confundirse con la señal degradada por búsqueda; se mitiga con el tope de D-29.
- Si el jugador confunde el latido del laboratorio con el del protagonista, interpreta mal la proximidad de las amenazas; se mitiga con el timbre grave distinto y con que el del laboratorio nunca dependa de la cercanía de enemigos (D-46).

**Criterios de aceptación (ambas configuraciones):**

- Cada estado de alerta tiene un sonido distinguible.
- El latido del laboratorio tiene un timbre distinguible del latido del protagonista.
- Con el mismo nivel de degradación, la intensidad del latido del laboratorio es la misma con o sin enemigos cerca y en cualquier estado de alerta.
- El latido del laboratorio solo aumenta al subir el nivel de degradación, con la salud en Crítico y en el Despertar, y solo disminuye al salir de Crítico, con "Nueva partida" o en el ciclo nuevo tras un final.
- Con la degradación máxima, el latido del laboratorio no tapa el latido del protagonista ni las señales de alerta.
- El sonido de alerta y la calidad de la señal de vigilancia corresponden siempre al mismo estado de alerta.
- El latido no responde a enemigos detrás de una puerta sellada.
- Todo cuerpo reproduce el aviso orgánico antes de desaparecer.
- En la fase 2 del jefe, cada cámara que gira emite el sonido de su motor.
- Con la degradación máxima, todas las señales de M-10 siguen audibles y la señal degradada por búsqueda sigue distinguiéndose de la señal base.

## Relaciones entre sistemas

Describe qué sistemas dependen de otros y qué información intercambian.

| Origen | Destino | Información |
|---|---|---|
| S-06 Generación procedural | S-01, S-02, S-05, S-09, S-10 | Salas, salas con cámara, rutas, coberturas y elevadores, recursos y llaves del presupuesto, salas válidas para reciclados, sala del jefe y su munición |
| S-06 Generación procedural | S-07 | Piso actual para el autoguardado |
| S-06 Generación procedural | S-08 | Llegada a la sala clave final del piso 2 (reinicio del contador) |
| S-01 Cámara y vigilancia | S-02 | Evento "atención llena" con la posición del jugador (D-24) |
| S-02 IA y percepción | S-01 | Estado de alerta más alto (señal escalonada) y eventos de caza (corte y bloqueo) |
| S-02 IA y percepción | S-03 | Ataques y agarres |
| S-02 IA y percepción | S-07 | Enemigos en caza (autoguardado pendiente y costo de salir) |
| S-02 IA y percepción | S-11 | Estado de alerta y distancia de cada enemigo |
| S-04 Resistencia | S-03, S-09 | Permiso para empujar y para el golpe en el suelo |
| S-05 Inventario | S-03, S-09 | Curas, munición y punzón de descarga |
| S-05 Inventario | S-07 | Inventario para el autoguardado |
| S-03 Salud y combate | S-09 | Enemigo eliminado y posición del cuerpo |
| S-03 Salud y combate | S-08 | Evento de muerte del personaje |
| S-09 Reciclaje | S-02 | Enemigo reciclado y ruido del golpe en el suelo |
| S-09 Reciclaje | S-11 | Aviso orgánico |
| S-08 Ciclo de muerte | S-05, S-06, S-07 | Vaciar inventario, regenerar los pisos, guardar antes de procesar la muerte y descartar el autoguardado |
| S-08 Ciclo de muerte | S-01, S-11 | Nivel de degradación (con tope visual y sonoro) |
| S-08 Ciclo de muerte | S-07 | Evento de reconstrucción (sube el código de sujeto R-H) |
| S-08 Ciclo de muerte | S-11 | Despertar y cinemáticas finales (latido del laboratorio) |
| S-06 Generación procedural | S-07 | Documentos colocados que el jugador lee y registra (de Emilia, de Vitalis Konzern y de The Mother) |
| S-07 Persistencia | S-06 | Estado del intento al continuar |
| S-07 Persistencia | S-08 | Salida con costo (suma al contador y a la degradación); conteo de documentos de Emilia (cinemática de victoria) |
| S-07 Persistencia | S-10 | Fallas disponibles para la fase 1; conteo de documentos de Emilia (decisión final) |
| S-10 Jefe | S-02 | Punto enfocado por una cámara hostil |
| S-10 Jefe | S-11 | Giro de las cámaras hostiles |
| S-10 Jefe | S-08 | Muerte en la sala; victoria al derrotarla (secuencia común, decisión final y cinemática de victoria) |

```mermaid
flowchart LR
    GEN[S-06 Generación procedural] --> CAM[S-01 Cámara y vigilancia]
    GEN --> IA[S-02 IA y percepción]
    GEN --> INV[S-05 Inventario]
    GEN --> REC[S-09 Reciclaje]
    GEN --> JEFE[S-10 Jefe The Mother]
    GEN -- piso actual --> PER[S-07 Persistencia]
    GEN -- sala final del piso 2 --> MUE[S-08 Ciclo de muerte y reconstrucción]
    CAM -- atención llena --> IA
    IA -- estado de alerta / caza --> CAM
    IA -- ataques y agarres --> COM[S-03 Salud y combate]
    IA -- estado de alerta --> AUD[S-11 Audio dinámico]
    IA -- enemigos en caza --> PER
    RES[S-04 Resistencia] --> COM
    RES --> REC
    INV --> COM
    INV --> REC
    INV -- inventario --> PER
    COM -- enemigo eliminado --> REC
    REC -- reciclado / ruido --> IA
    REC -- aviso orgánico --> AUD
    COM -- muerte --> MUE
    MUE -- vaciar --> INV
    MUE -- regenerar --> GEN
    MUE -- guardar / descartar --> PER
    MUE -- degradación / Despertar --> AUD
    MUE -- degradación --> CAM
    MUE -- reconstrucción / código de sujeto --> PER
    GEN -- documentos --> PER
    PER -- estado al continuar --> GEN
    PER -- salida con costo / documentos de Emilia --> MUE
    PER -- fallas / documentos de Emilia --> JEFE
    JEFE -- punto enfocado --> IA
    JEFE -- cámaras hostiles --> AUD
    JEFE -- muerte / victoria --> MUE
    CAM --> JEFE
    COM --> JEFE
```


> **Navegación:** [[00 - Índice]] · ← [[02 - Mecánicas]] · [[04 - Controles]] →
