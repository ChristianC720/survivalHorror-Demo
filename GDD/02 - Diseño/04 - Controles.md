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
| Remate con golpe en el suelo (sobre un cuerpo) | — | `Clic izquierdo` sin apuntar | Por definir (soporte posterior) |
| Remate con consumible (sobre un cuerpo) | `E` | — | Por definir (soporte posterior) |
| Menú (pausa) | `Esc` | — | Por definir (soporte posterior) |

**Contextos de entrada:** algunas teclas cambian de función según el contexto, pero ninguna tiene dos funciones dentro del mismo contexto.

- **Exploración (OTS):** `WASD` mueve al personaje y `Clic izquierdo` ejecuta el empujón.
- **Apuntando:** `Clic izquierdo` dispara.
- **Inventario abierto:** el mouse controla la interfaz; apuntar, disparar y empujar no están disponibles. El juego no se pausa.
- **Hackeo:** el personaje queda inmóvil en OTS; `WASD` / `Flechas` introducen la secuencia y `E` la abandona.
- **Vista de vigilancia:** el personaje solo puede permanecer quieto o caminar muy lento; correr, apuntar o empujar devuelven la vista a OTS. `Q` sale de la vista.
- **Sobre un cuerpo (sin apuntar):** acción contextual de remate ([[02 - Mecánicas]], M-20). `Clic izquierdo` ejecuta el golpe en el suelo en lugar del empujón y `E` usa el consumible de remate en lugar de interactuar. Al apuntar, `Clic izquierdo` vuelve a disparar.
    - **Frontera del contexto:** **Por definir.** Falta decidir cuándo se entra en el contexto (distancia y si basta con mirar el cuerpo), si sigue activo con cuerpos ya rematados o enemigos reciclados, si aplica en Alerta/Combate con enemigos vivos cerca y qué acción tiene prioridad cuando el cuerpo coincide con una puerta, un panel, una escalera, un elevador o un objeto. La regla que se elija no puede impedir interactuar con la ruta garantizada hacia la sala clave (D-15, principio 9 de [[01 - Jugabilidad]]).

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
- **Inventario:** el terminal del brazo se ilumina y se despliega la interfaz sin pausar el juego.
- **Recibir daño o agarre:** vibración de cámara, sonido de impacto y cambio visible del estado de salud.


> **Navegación:** [[00 - Índice]] · ← [[03 - Sistemas]] · [[05 - Progresión]] →
