# Audio y Música

## Descripción de la sección

Define la identidad sonora y cómo el audio comunica información al jugador.

Las reglas de las señales jugables están en S-11 Audio dinámico ([[03 - Sistemas]]) y M-10 Señales diegéticas ([[02 - Mecánicas]]); aquí se define la identidad sonora y la mezcla. Producción musical en FL Studio.

## Música

**Estilo (D-87):** cruda, oscura e industrial, con melancolía y minimalismo. Contraste entre lo frágil y lo mecánico: sonidos suaves y humanos frente a máquinas. Referencias: Lustmord, John Carpenter, Trent Reznor y Atticus Ross, Hildur Guðnadóttir, Akira Yamaoka y Petri Alanko.

**Instrumentación:**

- **Frágil:** piano, guitarra acústica y sonidos frágiles y suaves en general.
- **Base:** sintetizadores y bajos.
- **Industrial:** estática, zumbidos eléctricos, máquinas, engranajes y otros sonidos de la instalación.

**Temas principales (leitmotivs, D-87):**

- **Emilia:** piano y guitarra acústica frágiles. Menú principal, nota de voz de la introducción y finales Relevo.
- **The Mother:** industrial, con máquinas, engranajes, estática y zumbidos. Encuentro con el jefe, en un loop por fase.
- **El latido:** bajos y subgraves con el ritmo del latido del laboratorio. Se cuela en los otros dos y domina en el Despertar y en la Asimilación.
- Los finales combinan los motivos (por ejemplo, en Ambiguo, Emilia sobre el latido).

**Música dinámica:** Sí, solo en momentos puntuales.

- **Exploración:** sin música. Solo ambiente: drones industriales, maquinaria y el latido del laboratorio (S-11). Así la música nunca tapa las señales de M-10.
- **Con música:** menú, cinemáticas, sala de The Mother y finales.
- **Loops sin duración fija:** los eventos sin duración determinada, como el jefe, usan loops que cambian de capa en cada fase y enlazan en compás con la siguiente.

## Efectos de sonido

| Evento | Sonido | Propósito |
|---|---|---|
| Estados de alerta (sospecha, búsqueda, caza) | Un sonido distinto por estado; prioridad al más alto | Leer la percepción enemiga sin indicadores (M-10, D-23) |
| Latido del protagonista | Latido que sube con la proximidad; aproximado | Proximidad de amenazas sin radar (S-11) |
| Latido del laboratorio | Latido grave, de timbre distinto; solo crece con la degradación | Capa del ciclo; primer acto del texto (D-64) |
| Grito de la Boca | Grito propio | Avisa que los enemigos cercanos van hacia ella (D-64) |
| Pasos | Según superficie: metal, agua, escombros | Ruido propio y de los enemigos (M-10) |
| Maquinaria antes del reciclaje | Motor y engranajes | Aviso de que un cuerpo va a reciclarse (M-20) |
| Jadeo | Respiración de Lukas | Resistencia insuficiente (M-06) |
| Revólver | Disparo seco y moderno, recarga bala a bala, chasquido de tambor vacío | Combate y ruido a ~20 m (M-04, D-74) |
| Punzón de descarga | Zumbido eléctrico y descarga | Defensa y remate (M-11, M-20) |
| Hackeo | Pitido por entrada y pitido de error | Resultado del hackeo (M-13) |
| Interruptor de energía | Arranque ruidoso de maquinaria | Alerta a enemigos cercanos (D-72) |
| Cámaras hostiles | Motor al girar | Leer los puntos ciegos en la fase 2 (D-28) |
| Terminal del brazo | Encendido CRT y pitido de registro | Confirmar registro de documentos y objetos |
| Reconstrucción | Maquinaria de la cápsula | Inicio del ciclo (D-39) |
| Autoguardado | Señal suave de "sincronizando…" | Confirmar el guardado (D-30) |

## Audio espacial

Sí. Audio 3D posicional con el sistema incluido en Unity: atenuación por distancia y oclusión simple por puertas, de modo que un enemigo tras una puerta sellada no activa el latido (M-10). Se recomiendan auriculares para leer la dirección de las amenazas; la mezcla también funciona en estéreo con altavoces. El latido del laboratorio y la música no son posicionales.

## Voz

**Narración:** No hay narrador (D-62).

**Diálogos hablados:** No hay actuación de voz (D-52). Emilia, The Mother y Lukas se representan con sonidos más texto en pantalla; subtítulos obligatorios. El sonido vocal de Lukas (grave, rasposo y parco) se distorsiona con la degradación (M-18).

**Idiomas:** Solo de texto: inglés (principal) y español (D-62). El alemán nunca se traduce (D-58).

## Mezcla

**Prioridades (de mayor a menor):**

1. Señales de M-10: alertas, latido del protagonista, grito de la Boca, avisos del sistema y motor de las cámaras hostiles.
2. Sonidos de Lukas y de combate: pasos, disparos, golpes y jadeo.
3. Ambiente y latido del laboratorio.
4. Música.

**Reglas:**

- La música y el ambiente bajan de volumen (*ducking*) mientras suena una señal de prioridad 1.
- **Jefe:** la música de The Mother nunca tapa el motor de las cámaras ni las señales de alerta.
- **Degradación (D-29):** el latido del laboratorio y la distorsión del sonido vocal crecen hasta un tope que nunca supera a las señales de M-10.
- **Cinemáticas:** la música y el diseño sonoro tienen prioridad. Los subtítulos siempre se ven.
- **Volúmenes separados en Opciones:** general, música, efectos y ambiente (ver [[02 - UI y Menús]]).
- Valores concretos de mezcla: **por definir** en pruebas.


> **Navegación:** [[00 - Índice]] · ← [[04 - Interfaz Visual]] · [[01 - UX y Flujo del Jugador]] →
