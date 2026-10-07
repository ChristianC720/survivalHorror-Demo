# Controles

## Descripción de la sección

Define la relación entre las acciones del jugador y las entradas físicas del dispositivo.

## Mapa de controles

| Acción | Teclado | Mouse | Control |
|---|---|---|---|
| Mover | `W` `A` `S` `D` | — | Stick izquierdo |
| Mover la cámara / mirar | — | Movimiento del mouse | Stick derecho |
| Correr | Mantener `Shift izquierdo` + dirección | — | Por definir (soporte posterior) |
| Agacharse / sigilo | `Ctrl izquierdo` (alternar) | — | Por definir (soporte posterior) |
| Apuntar | — | Mantener `Clic derecho` | Por definir (soporte posterior) |
| Ataque principal (disparar) | — | `Clic izquierdo` mientras se apunta | Por definir (soporte posterior) |
| Ataque secundario (empujón / golpe de culata) | — | `Clic izquierdo` sin apuntar | Por definir (soporte posterior) |
| Recargar | `R` | — | Por definir (soporte posterior) |
| Interactuar (recoger, leer, abrir, iniciar hackeo, usar escalera o elevador) | `E` | — | Por definir (soporte posterior) |
| Linterna encender / apagar | `F` | — | Por definir (soporte posterior) |
| Vista de vigilancia activar / salir | `Q` | — | Por definir (soporte posterior) |
| Inventario y base de datos del terminal abrir / cerrar | `Tab` | — | Por definir (soporte posterior) |
| Usar / equipar objeto (inventario abierto) | — | `Clic izquierdo` sobre la casilla | Por definir (soporte posterior) |
| Descartar objeto (inventario abierto) | — | `Clic derecho` sobre la casilla | Por definir (soporte posterior) |
| Introducir secuencia de hackeo (personaje inmóvil) | `W` `A` `S` `D` / `Flechas` | — | Por definir (soporte posterior) |
| Abandonar hackeo | `E` | — | Por definir (soporte posterior) |
| Objeto defensivo | Automático al ser agarrado (sin entrada) | — | Automático |
| Remate con golpe en el suelo (a ~1.5 m mirando un cuerpo no rematado) | — | `Clic izquierdo` sin apuntar | Por definir (soporte posterior) |
| Remate con consumible (a ~1.5 m mirando un cuerpo no rematado) | `E` | — | Por definir (soporte posterior) |
| Menú (pausa) abrir / cerrar | `Esc` | — | Por definir (soporte posterior) |
| Seleccionar opción de menú (incluye "Reiniciar" en el menú de pausa) | — | `Clic izquierdo` sobre la opción | Por definir (soporte posterior) |
| Saltar cinemática (cinemática corta de reconstrucción) o créditos ya vistos | `Espacio` | — | Por definir (soporte posterior) |

**Contextos de entrada:** algunas teclas cambian de función según el contexto, pero ninguna tiene dos funciones dentro del mismo contexto.

- **Exploración (OTS):** `WASD` mueve al personaje y `Clic izquierdo` ejecuta el empujón.
- **Apuntando:** `Clic izquierdo` dispara.
- **Inventario abierto:** el mouse controla la interfaz; apuntar, disparar y empujar no están disponibles. El juego no se pausa.
- **Hackeo:** el personaje queda inmóvil en OTS; `WASD` / `Flechas` introducen la secuencia y `E` la abandona.
- **Vista de vigilancia:** el personaje solo puede permanecer quieto o caminar muy lento; correr, apuntar o empujar devuelven la vista a OTS. `Q` sale de la vista.
- **Sobre un cuerpo (sin apuntar):** acción contextual de remate ([[02 - Mecánicas]], M-20). `Clic izquierdo` ejecuta el golpe en el suelo en lugar del empujón y `E` usa el consumible de remate en lugar de interactuar. Al apuntar, `Clic izquierdo` vuelve a disparar.
    - **Frontera del contexto (D-27):** se entra en el contexto a ~1.5 m (valor inicial, sujeto a balance) mirando un cuerpo no rematado, también durante el combate con enemigos vivos cerca. Con un cuerpo ya rematado el contexto no se activa y `Clic izquierdo` y `E` recuperan el empujón y la interacción. Si hay otro elemento interactivo al lado del cuerpo (puerta, panel, escalera, elevador u objeto), gana el que está al centro de la vista; así el jugador siempre puede interactuar con la ruta garantizada hacia la sala clave final (D-33, D-36, principio 9 de [[01 - Jugabilidad]]). El caso de los cuerpos que no pueden reciclarse (enemigos reciclados y sala del jefe) se documenta en M-20.
- **Menús (pausa y principal):** el mouse controla la interfaz y `Clic izquierdo` selecciona una opción. `Esc` abre y cierra el menú de pausa, que incluye "Reiniciar" ([[02 - Mecánicas]], M-16; D-32). Las opciones del menú principal y el diálogo de salida se documentan en [[07 - Reglas y Estados del Juego]].
- **Cinemática:** las acciones del personaje no están disponibles. `Espacio` salta la cinemática corta de reconstrucción (D-39). La cinemática de transición de piso no se puede saltar porque dura un mínimo de 5 s y oculta la carga (M-15). Si la cinemática completa de reconstrucción de la introducción se puede saltar: **Por definir**.
- **Créditos:** `Espacio` los salta solo después de haberlos visto una vez (D-39).

## Principios de control

- Los controles deben ser consistentes.
- Las acciones críticas deben ser fáciles de identificar.
- Debe existir retroalimentación visual, sonora o háptica cuando corresponda.
- Evitar combinaciones innecesariamente complejas.

## Reasignación

¿El jugador puede cambiar los controles?

**Por definir.** Se recomienda permitir la reasignación de todas las acciones de teclado y mouse desde el menú de configuración, validando que ninguna tecla quede asignada a dos acciones dentro del mismo contexto. El soporte completo para control se definirá posteriormente.

## Entrada y retroalimentación

Describe qué ocurre inmediatamente después de cada entrada importante.

- **Apuntar:** la cámara se acerca al hombro, aparece la retícula, la linterna concentra su haz y la velocidad de movimiento se reduce.
- **Disparar:** retroceso, destello y sonido del disparo; la munición del cargador disminuye. Si el cargador está vacío se reproduce un chasquido.
- **Empujón / golpe de culata:** animación de impacto, sonido de golpe y aturdimiento breve del enemigo; el personaje respira con más fuerza al consumir resistencia.
- **Correr:** pasos más fuertes y respiración agitada; al agotarse la resistencia, jadeo audible.
- **Interactuar:** animación de interacción y sonido del objeto, puerta o terminal.
- **Remate:** el golpe en el suelo reproduce una animación de ~2 s (valor inicial, sujeto a balance) con sonido de impacto audible para enemigos cercanos; el consumible de remate reproduce una animación breve y silenciosa para los enemigos. Sin resistencia suficiente se reproduce un jadeo; sin consumible, `E` no tiene efecto.
- **Vista de vigilancia:** transición con interferencia CRT; la imagen aparece con ruido y retraso.
- **Hackeo:** cada entrada correcta emite un tono; un error emite un pitido y reinicia la secuencia.
- **Reiniciar:** se cierra el menú de pausa y se reproduce la cinemática corta de reconstrucción (D-32, D-39).
- **Saltar cinemática:** la cinemática corta termina y el control vuelve al personaje en el punto de reaparición.
- **Inventario:** el terminal del brazo se ilumina y se despliega la interfaz sin pausar el juego.
- **Recibir daño o agarre:** vibración de cámara, sonido de impacto y cambio visible del estado de salud.


> **Navegación:** [[00 - Índice]] · ← [[03 - Sistemas]] · [[05 - Progresión]] →
