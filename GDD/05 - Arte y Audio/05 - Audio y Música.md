# Audio y Música

## 1. Descripción de la sección

Define la identidad sonora y cómo el audio comunica información al jugador.

Las reglas de las señales jugables están en Señales diegéticas, en [[02 - Mecánicas]], y en Audio dinámico, en [[03 - Sistemas]]. Esta nota define la identidad sonora y la mezcla. La música se produce en REAPER y FL Studio. Los valores numéricos de este documento son iniciales y están sujetos a balance.

## 2. Música

**Estilo:** Cruda, oscura e industrial, con melancolía y minimalismo. Contrasta lo frágil con lo mecánico, es decir, sonidos suaves y humanos frente a máquinas. Las referencias son Lustmord, John Carpenter, Trent Reznor y Atticus Ross, Hildur Guðnadóttir, Akira Yamaoka y Petri Alanko.

**Instrumentación:**

- **Frágil.** Piano, guitarra acústica y sonidos frágiles y suaves en general.
- **Base.** Sintetizadores y bajos.
- **Industrial.** Estática, zumbidos eléctricos, máquinas, engranajes y otros sonidos de la instalación.

**Temas principales:** Son tres leitmotivs.

| Leitmotiv | Sonido | Dónde suena |
|---|---|---|
| Emilia | Piano y guitarra acústica frágiles | Menú principal, nota de voz de la introducción y finales Relevo. La pantalla de título solo tiene el latido |
| The Mother | Industrial, con máquinas, engranajes, estática y zumbidos | Encuentro con el jefe, en un loop por fase |
| El latido | Bajos y subgraves con el ritmo del latido del laboratorio | Se cuela en los otros dos y domina en el Despertar y en la Asimilación |

Los finales combinan los motivos. En Ambiguo, por ejemplo, suena Emilia sobre el latido.

**Música dinámica:** Sí, solo en momentos puntuales.

- **Exploración.** No hay música. Solo suena el ambiente, con drones industriales, maquinaria y el latido del laboratorio. Así la música nunca tapa las señales diegéticas.
- **Con música.** El menú, las cinemáticas, la sala de The Mother y los finales.
- **Loops sin duración fija.** Los eventos sin duración determinada, como el jefe, usan loops que cambian de capa en cada fase. Cada loop enlaza en compás con el siguiente.

## 3. Efectos de sonido

| Evento | Sonido | Propósito |
|---|---|---|
| Estados de alerta: sospecha, búsqueda y caza | Un sonido distinto por estado. Suena el más alto | Leer la percepción enemiga sin indicadores |
| Latido del protagonista | Latido aproximado que sube con la cercanía | Cercanía de amenazas sin radar |
| Latido del laboratorio | Latido grave, de timbre distinto. Suena de fondo en la nota de voz de Emilia y en todas las salas desde la primera reconstrucción. Crece con cada muerte y con la salud en Crítico | Capa del ciclo y primer acto del texto |
| Grito de la Boca | Grito propio | Avisar que los enemigos cercanos van hacia ella |
| Pasos | Según la superficie, metal, agua o escombros | Ruido propio y de los enemigos |
| Aviso de reciclaje | Crujidos de hueso y carne del cuerpo que convulsiona, ~3 s, a los ~40 s de la muerte | Avisar que el laboratorio va a levantar un cuerpo |
| Jadeo | Respiración de Lukas | Resistencia insuficiente |
| Revólver | Disparo seco y moderno, recarga bala a bala y chasquido de tambor vacío | Combate y ruido a ~20 m |
| Punzón de descarga | Zumbido eléctrico y descarga | Defensa y remate |
| Hackeo | Pitido por entrada y pitido de error | Resultado del hackeo |
| Interruptor de energía | Arranque ruidoso de maquinaria | Alertar a los enemigos cercanos |
| Cámaras hostiles | Motor al girar | Leer los puntos ciegos en la fase 2 del jefe |
| Terminal del brazo | Encendido CRT y pitido de registro | Confirmar el registro de documentos y objetos |
| Reconstrucción | Maquinaria de la cápsula | Inicio del ciclo |
| Cierre de tránsito | Cierre metálico y alarma corta | Escalera o elevador bloqueados durante una caza |
| Autoguardado | Señal suave de `sincronizando…` | Confirmar el guardado |

## 4. Audio espacial

Sí. El audio es 3D posicional con el sistema de Unity, con atenuación por distancia y oclusión por paredes y puertas. Así un enemigo detrás de una puerta sellada no activa el latido. Se recomiendan auriculares para leer la dirección de las amenazas, aunque la mezcla también funciona en estéreo con altavoces. El latido del laboratorio y la música no son posicionales.

**Oclusión.** Cada pared entre la fuente y el oyente reduce el sonido un 70%, y cada puerta cerrada o sellada lo reduce un 50%. Las reducciones se acumulan. La misma oclusión reduce el radio de ruido que perciben los enemigos. Las señales diegéticas no se atenúan.

## 5. Voz

**Narración:** No hay narrador.

**Diálogos hablados:** No hay actuación de voz. Emilia, The Mother y Lukas se representan con sonidos y texto en pantalla, y los subtítulos son obligatorios. El sonido vocal de Lukas es grave, rasposo y parco, y se distorsiona con la degradación.

**Idiomas:** Solo de texto, en inglés como idioma principal y en español. El alemán solo se traduce en los subtítulos de los registros del terminal del brazo.

## 6. Mezcla

**Prioridades, de mayor a menor:**

1. Señales diegéticas, que son las alertas, el latido del protagonista, el grito de la Boca, los avisos del sistema y el motor de las cámaras hostiles.
2. Sonidos de Lukas y de combate, como pasos, disparos, golpes y jadeo.
3. Ambiente y latido del laboratorio.
4. Música.

**Reglas:**

- La música y el ambiente bajan de volumen, con *ducking*, mientras suena una señal de prioridad 1. Bajan −8 dB, se mantienen 0,6 s y se recuperan en 1,5 s.
- **Jefe.** La música de The Mother nunca tapa el motor de las cámaras ni las señales de alerta.
- **Degradación.** El latido del laboratorio y la distorsión del sonido vocal crecen hasta un tope que nunca supera a las señales diegéticas.
- **Cinemáticas.** La música y el diseño sonoro tienen prioridad. Los subtítulos siempre se ven.
- **Volúmenes separados en Opciones.** General, música, efectos, ambiente y voces. Ver [[04 - Guardado y Configuración]].

**Grupos del mezclador.** Cada prioridad tiene su grupo, que son señales, Lukas y combate, ambiente con el latido del laboratorio, música y voces. El grupo de señales no tiene control de volumen en Opciones.

| Volumen | Valor por defecto |
|---|---|
| General | 100% |
| Música | 80% |
| Efectos | 100% |
| Ambiente | 100% |
| Voces | 100% |

> **Navegación:** [[00 - Índice]] · ← [[04 - Interfaz Visual]] · [[01 - UX y Flujo del Jugador]] →
