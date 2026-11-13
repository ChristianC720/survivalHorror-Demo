# Controles

## 1. Descripción de la sección

Define la relación entre las acciones del jugador y las entradas físicas del dispositivo.

## 2. Mapa de controles

La demo se juega en PC con teclado y mouse, o con un mando tipo Xbox con mapa fijo. No tiene entrada táctil. Los valores numéricos de este documento son iniciales y están sujetos a balance.

| Acción | Teclado | Mouse | Control |
|---|---|---|---|
| Moverse | `W` `A` `S` `D` | No aplica | Stick izquierdo |
| Mirar y mover la cámara | No aplica | Movimiento del mouse | Stick derecho |
| Correr | Mantener `Shift izquierdo` y una dirección | No aplica | Mantener `LB` |
| Agacharse, en modo alternar | `Ctrl izquierdo` | No aplica | `B` |
| Apuntar | No aplica | Mantener `Clic derecho` | Mantener `LT` |
| Ataque principal: disparar | No aplica | `Clic izquierdo` mientras se apunta | `RT` mientras se apunta |
| Ataque secundario: empujón o golpe de culata | No aplica | `Clic izquierdo` sin apuntar | `RT` sin apuntar |
| Recargar | `R` | No aplica | `X` |
| Interactuar: recoger, leer, abrir, iniciar hackeo, usar escalera o elevador | `E` | No aplica | `A` |
| Encender o apagar la linterna | `F` | No aplica | Cruceta arriba |
| Entrar o salir de la vista de vigilancia | `Q` | No aplica | `Y` |
| Abrir o cerrar el inventario y la base de datos del terminal | `Tab` | No aplica | `View` |
| Usar o equipar un objeto con el inventario abierto | No aplica | `Clic izquierdo` sobre la casilla | `A` sobre la casilla, con cruceta o stick izquierdo para moverse |
| Descartar un objeto con el inventario abierto | No aplica | `Clic derecho` sobre la casilla | `X` sobre la casilla |
| Introducir la secuencia de hackeo, con Lukas inmóvil | `Flechas` o `W` `A` `S` `D` | No aplica | Cruceta o stick izquierdo |
| Abandonar el hackeo | `E` | No aplica | `A` |
| Explotar una falla descubierta durante el hackeo | `F` | No aplica | `Y` |
| Punzón de descarga como defensa | Automático al ser agarrado | No aplica | Automático |
| Remate con golpe en el suelo, a ~1,5 m mirando un cuerpo no rematado | No aplica | `Clic izquierdo` sin apuntar | `RT` sin apuntar |
| Remate con punzón de descarga, a ~1,5 m mirando un cuerpo no rematado | `E` | No aplica | `A` |
| Menú: abrir o cerrar la pausa | `Esc` | No aplica | `Menu` |
| Elegir una opción de menú, incluida `Reiniciar` en la pausa | No aplica | `Clic izquierdo` sobre la opción | Cruceta o stick izquierdo para moverse, `A` para elegir y `B` para volver |
| Saltar una cinemática o los créditos ya vistos | `Espacio` | No aplica | `A`, pulsado o mantenido igual que `Espacio` |

### 2.1 Contextos de entrada

Algunas teclas cambian de función según el contexto, pero ninguna tiene dos funciones dentro del mismo contexto.

**Exploración.** En la vista sobre el hombro, `W` `A` `S` `D` mueve a Lukas y `Clic izquierdo` empuja. `E` solo actúa sobre lo que está dentro de un cono de ~35° frente a la cámara y delante de Lukas.

**Apuntando.** `Clic izquierdo` dispara.

**Inventario abierto.** El mouse controla la interfaz y no se puede apuntar, disparar ni empujar. El juego no se pausa.

**Lectura.** El juego está detenido. `E`, `Esc` o `B` cierran la lectura y las direcciones cambian de página. Aquí `Esc` no abre el menú de pausa.

**Hackeo.** Lukas queda inmóvil en la vista sobre el hombro. Las `Flechas` o `W` `A` `S` `D` introducen la secuencia. En el mando la introducen la cruceta o el stick izquierdo. `F` explota una falla compatible si hay una, y `E` abandona.

**Vista de vigilancia.** Lukas solo puede quedarse quieto o caminar muy lento. Correr, apuntar o empujar devuelven la vista sobre el hombro, y `Q` sale de la vista.

### 2.2 Frente a un cuerpo

Mirando un cuerpo no rematado a ~1,5 m y sin apuntar, se activa el remate descrito en [[02 - Mecánicas]]. El contexto existe también en pleno combate con enemigos vivos cerca. `Clic izquierdo` da el golpe en el suelo en lugar del empujón. `E` clava el punzón de descarga en lugar de interactuar, solo en cuerpos que todavía pueden reciclarse. Al apuntar, `Clic izquierdo` vuelve a disparar.

Con un cuerpo ya rematado el contexto no aparece, y `Clic izquierdo` y `E` recuperan el empujón y la interacción. Si junto al cuerpo hay una puerta, un panel, una escalera, un elevador o un objeto, gana lo que esté en el centro de la vista. Así el jugador siempre puede usar la ruta garantizada hacia la sala clave final, como pide el principio 9 de [[01 - Jugabilidad]]. Los cuerpos que no pueden reciclarse, que son los reciclados y los de la sala del jefe, se tratan en [[02 - Mecánicas]].

Dentro del contexto, el aviso en pantalla muestra `[Clic] Golpe en el suelo` y, si Lukas lleva un punzón, `[E] Punzón de descarga`. Las teclas que muestra son las del dispositivo en uso.

### 2.3 Menús, cinemáticas y créditos

En los menús de pausa y principal, el mouse controla la interfaz y `Clic izquierdo` elige una opción. `Esc` abre y cierra el menú de pausa, que incluye `Reiniciar`. Las opciones del menú principal y el diálogo de salida están en [[07 - Reglas y Estados del Juego]].

En las cinemáticas, las acciones de Lukas no están disponibles.

| Cinemática | ¿Se puede saltar? |
| --- | --- |
| Reconstrucción corta | Sí, pulsando `Espacio` |
| Transición de piso | No. Dura al menos 5 s y oculta la carga |
| Narrativas: reconstrucción completa, Asimilación, secuencia de victoria, Relevos, Ambiguo y gancho | Sí, manteniendo `Espacio` ~1 s, solo si ya se vio una vez |
| Hallazgo del piso 2 | No |
| Créditos | Sí, con `Espacio`, solo si ya se vieron una vez |

### 2.4 Introducción jugable

En la introducción solo se puede mover, mirar e interactuar con `E`. No hay inventario, linterna, armas ni vigilancia. `Esc` abre una pausa sin `Reiniciar`. La compuerta se hackea en la pantalla del teléfono de Lukas con las mismas entradas que el hackeo normal.

## 3. Principios de control

- **Consistencia.** Cada tecla conserva su función en todo el juego y solo cambia por contexto. `E` siempre interactúa, incluso en la decisión final, y el contexto frente a un cuerpo se anuncia con su aviso.
- **Acciones críticas fáciles de identificar.** Disparar, empujar, correr e interactuar usan las entradas principales del mouse y del teclado. El aviso contextual muestra la tecla del dispositivo en uso.
- **Retroalimentación.** Cada entrada importante tiene respuesta visual o sonora, descrita en la sección 5.
- **Sin combinaciones complejas.** La única combinación es correr, que pide mantener `Shift izquierdo` mientras Lukas se mueve.

## 4. Reasignación

Sí. Todas las acciones de teclado y mouse se reasignan en `Opciones > Controles`. El juego impide asignar una tecla a dos acciones del mismo contexto.

También se ajustan la sensibilidad de mirar y la de apuntar por separado, el eje vertical invertido, agacharse en modo alternar o mantener y la asistencia de apuntado. Los carteles del tutorial muestran la tecla asignada, o el botón del mando si es el último dispositivo usado.

En la demo, el mando tipo Xbox usa el mapa fijo de la columna Control, sin reasignación. Reasignar el mando queda para después de la demo. Ver [[04 - Guardado y Configuración]].

## 5. Entrada y retroalimentación

| Entrada | Respuesta inmediata |
| --- | --- |
| Apuntar | La cámara se acerca al hombro, aparece la retícula, la linterna concentra el haz y Lukas se mueve más lento. |
| Disparar | Retroceso, destello y sonido del disparo. Baja la munición del tambor y, sin balas, suena un chasquido. |
| Empujón o golpe de culata | Animación y sonido de impacto y aturdimiento breve del enemigo. Lukas respira con más fuerza al gastar resistencia. |
| Correr | Pasos más fuertes y respiración agitada. Al agotarse la resistencia, se oye un jadeo. |
| Interactuar | Animación de interacción y sonido del objeto, la puerta o el terminal. |
| Remate | El golpe en el suelo dura ~2 s y su impacto lo oyen los enemigos cercanos. El punzón es breve y silencioso para los enemigos. Sin resistencia suficiente suena un jadeo y, sin punzón, `E` no hace nada. |
| Vista de vigilancia | Transición con interferencia CRT. La imagen llega con ruido y retraso. |
| Hackeo | Cada entrada correcta suena con un tono. Un error suena con un pitido y reinicia la secuencia. |
| Reiniciar | Se cierra la pausa y se reproduce la cinemática corta de reconstrucción. |
| Saltar cinemática | Termina la cinemática corta y el control vuelve a Lukas en el punto de reaparición. |
| Inventario | El terminal del brazo se ilumina y despliega la interfaz sin pausar el juego. |
| Recibir daño o un agarre | Vibración de cámara, sonido de impacto y cambio visible del estado de salud. |

> **Navegación:** [[00 - Índice]] · ← [[03 - Sistemas]] · [[05 - Progresión]] →
