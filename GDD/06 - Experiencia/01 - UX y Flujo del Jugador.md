# UX y Flujo del Jugador

## 1. Descripción de la sección

UX (experiencia de usuario) describe qué tan comprensible, cómoda y coherente resulta la interacción con el videojuego.

Los estados y las transiciones completos están en [[07 - Reglas y Estados del Juego]] y la interfaz en [[04 - Interfaz Visual]]. Los valores numéricos de este documento son iniciales y están sujetos a balance.

## 2. Flujo principal

```text
Primer inicio: calibración de brillo
Título (negro, logotipo y latido) → Menú principal (terminal de la sala de cápsulas)
  → Nueva partida (ranura y modo) → Introducción → Reconstrucción completa
  → Piso 1 (tutorial, primer intento) → Piso 2 → Piso 3 → Sala de The Mother → Final → Créditos
  → Continuar: ciclo nuevo con el terminal conservado

Durante el recorrido:
  Pausa ⇄ Juego
  Muerte → Reconstrucción corta → Inicio del piso actual (semilla nueva)
  Quinta muerte consecutiva → Final: Asimilación
```

## 3. Primeros minutos

| Momento | Qué descubre el jugador | Cómo |
|---|---|---|
| Primer inicio | Que el juego es oscuro y necesita ver los negros | Calibración de brillo |
| Casa, en cinemática | A Lukas, la foto y la nota de voz de Emilia, con un latido de fondo que no sabe explicar | Cinemática con subtítulos |
| Compuerta | Moverse, mirar, interactuar y hackear | HUD mínimo y hackeo en el teléfono |
| Piso que no existe | Que algo está mal y que la muerte es inevitable | Pasillo único y muerte guionizada |
| Reconstrucción completa | Que Lukas olvidó y vuelve como R-H-07 | Cinemática |
| Fila de cápsulas | El terminal, el anillo, la bandeja y el único lugar seguro | Línea guía con la tecla del terminal, carteles de Vitalis y el anillo en el suelo |
| Depósito a oscuras | Linterna, recoger, inventario y revólver | Línea guía para encender la linterna y recoger, carteles y destello bajo la linterna |
| Túnel | Correr y resistencia | Línea guía para correr, carteles y jadeo |
| Sala de calderas | Sigilo, apuntado, empujón y disparo | Línea guía para agacharse, apuntar y atacar, frente a 1 Rastrero real que hace daño pero no mata |
| Primera muerte, ya fuera del tutorial | Que la muerte es real y el ciclo. Pierde lo recogido en el piso, conserva el terminal y encuentra un piso distinto | Reconstrucción corta y semilla nueva |

El tutorial no mata. En la fila de cápsulas y las 4 salas del tutorial, los golpes y los agarres hacen daño, pero Lukas queda en el borde de Crítico. Cada sala abre su salida cuando el jugador hace las acciones que pide. Si no las hace, la salida se abre sola a los ~120 s.

## 4. Retroalimentación

**Entrada → Respuesta visual → Respuesta sonora → Resultado**

| Acción | Entrada | Respuesta visual | Respuesta sonora | Resultado |
|---|---|---|---|---|
| Moverse o correr | `WASD` o `Shift` | Animación y postura según la salud | Pasos según la superficie y jadeo sin resistencia | Ruido para los enemigos |
| Linterna | `F` | Haz blanco y destellos en los recogibles | Clic | Ver a cambio de delatarse |
| Interactuar | `E` | Borde teal a 3 m o menos y aviso `E` a ~1,5 m | Sonido del objeto | Recoger, leer o abrir |
| Apuntar y disparar | `Clic derecho` y `Clic izquierdo` | Retícula de punto que se tiñe sobre un objetivo, marcador de impacto y contador verde del revólver | Disparo o tambor vacío | Daño y ruido a ~20 m |
| Recargar | `R` | Animación bala a bala | Cartucho por bala | Tambor cargado |
| Empujón | `Clic izquierdo` sin apuntar | Animación de empujón | Golpe | Aleja al enemigo |
| Remate | `Clic izquierdo` o `E` sobre un cuerpo | Golpe en el suelo de ~2 s o punzón de ~1,5 s | Golpe ruidoso o descarga silenciosa | El cuerpo no se recicla |
| Curarse | Inventario | Animación de ~1,5 s y mejora de la postura | Kit médico | Sube un estado de salud |
| Recibir daño | Ninguna | Destello rojo, vibración de cámara y cambio de postura | Impacto y quejido | Baja un estado de salud |
| Hackear | `E` y la secuencia | Secuencia en el terminal y panel verde | Pitido por entrada o de error | Abrir, sellar o activar |
| Vigilancia | `Q` | Imagen verde con ruido CRT | Interferencia | Información parcial y atención de The Mother |
| Registrar | Leer o examinar | `REGISTERED` en el terminal | Pitido de registro | Persiste entre muertes |
| Cambiar de piso | `E` en la escalera o el elevador | Cinemática de 5 s o más, o `LOCKDOWN` en caza | Ascensor o alarma de cierre | Piso siguiente y autoguardado |

Las respuestas inmediatas de cada entrada están en [[04 - Controles]].

## 5. Fricción intencional

**Forma parte del diseño y no se suaviza:**

- La oscuridad obliga a buscar con la linterna, y la linterna delata. Responde al pilar Vulnerabilidad constante.
- El inventario de 6 casillas funciona en tiempo real y la munición escasea.
- La recarga es bala a bala y la dispersión crece al moverse.
- No hay mapa. La vigilancia da información parcial y llena la atención de The Mother.
- Al morir se pierde lo recogido en el piso, cada entrada a un piso genera uno nuevo y existe el contador de 5 muertes.
- El cierre de tránsito durante una caza impide huir cambiando de piso.

**Es mala experiencia y se evita:**

| Problema | Respuesta |
|---|---|
| Golpes invisibles por la cámara sobre el hombro | Las señales diegéticas y los subtítulos de sonido los compensan |
| Perder un objeto en la oscuridad sin poder encontrarlo | Los recogibles destellan bajo cualquier luz |
| No saber por qué se murió | Cada ataque tiene animación y sonido de aviso propios |
| Perder progreso por un cierre inesperado | El terminal se autoguarda en cada cambio. Ver Persistencia en [[03 - Sistemas]] |
| Saltar sin querer una cinemática | Las narrativas se saltan manteniendo `Espacio` |
| Confundir la señal degradada con la degradación estética | El tope de la degradación lo impide |
| Bloquearse sin ruta | La llave obligatoria siempre es alcanzable |

## 6. Descubrimiento

**Qué hacer:** Lo indican el entorno y las líneas de protocolo de The Mother, como una puerta cerrada, `NO POWER` o `Restricted access: level key required.`. Los avisos breves arriba al centro explican cada bloqueo y el objetivo de cada fase del jefe. Los contadores de las secundarias están en el terminal.

**Dónde ir:** No hay mapa ni marcadores. El jugador lee el entorno, con la señalización de Vitalis, la luz de emergencia y las puertas, y usa la vista de vigilancia. La sala final siempre está a 4 salas o más.

**Qué puede interactuar:** Los objetos destellan bajo la luz. Los recogibles a 3 m o menos muestran un borde teal, y las puertas y los paneles una luz de estado. El aviso `E` aparece a ~1,5 m. En el tutorial, la línea guía y los carteles enseñan cada acción.

**Qué ocurrió:** Lo cuentan los registros del terminal, con documentos, bestiario y fallas, y las señales diegéticas. Quien active los subtítulos de sonido también los lee.

**Por qué ganó o perdió:**

- La muerte muestra el ataque que la causó.
- La cinemática corta refleja la degradación.
- La Asimilación llega tras 5 muertes consecutivas, y la degradación acumulada la anticipa.
- La victoria depende de derrotar a The Mother, y el final, de los 6 documentos de Emilia.

> **Navegación:** [[00 - Índice]] · ← [[05 - Audio y Música]] · [[02 - UI y Menús]] →
