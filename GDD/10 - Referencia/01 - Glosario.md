# Glosario

## 1. Descripción de la sección

Centraliza términos técnicos, de diseño y propios del universo del videojuego.

Las reglas de cada término están en la nota que se enlaza. Los valores numéricos de este documento son iniciales y están sujetos a balance.

| Término | Definición |
|---|---|
| GDD | Documento de Diseño de Videojuego. |
| Mecánica | Regla o interacción mediante la cual el jugador puede actuar. Ver [[02 - Mecánicas]]. |
| Sistema | Conjunto de componentes y reglas que trabajan juntos. Ver [[03 - Sistemas]]. |
| MVP | Producto Mínimo Viable. En el proyecto es el hito H3 ([[01 - Alcance]]). |
| Vertical Slice | Muestra pequeña pero representativa del producto final. En el proyecto es el hito H4 ([[01 - Alcance]]). |
| Greybox | Versión jugable con geometría gris y sin arte final. Sirve para probar sistemas antes del arte. |
| Vista sobre el hombro (OTS) | Cámara en tercera persona sobre el hombro de Lukas. Es la vista predeterminada del juego. |
| Vista de vigilancia | Vista isométrica de las cámaras de seguridad de The Mother. Se activa desde el terminal del brazo y llega con ruido, retraso y costo. |
| Señal degradada | Imagen de vigilancia con más ruido CRT y más retraso. Aparece cuando un enemigo cercano está en búsqueda. |
| Bloqueo de vigilancia | Corte de la vista de vigilancia cuando un enemigo cercano entra en caza. Impide volver a activarla mientras dure la caza. |
| Atención de The Mother | Medidor oculto que se llena al usar la vista de vigilancia. Al llenarse, los enemigos cercanos pasan a sospecha y búsqueda. |
| Patrulla, sospecha, búsqueda y caza | Estados de alerta de los enemigos, comunicados con señales diegéticas. *En alerta* equivale a caza. |
| Señales diegéticas | Información de peligro comunicada dentro del mundo con sonido, luz o comportamiento, sin indicadores en pantalla. |
| Bien, Herido y Crítico | Estados visibles de la salud de Lukas. La salud no se regenera. |
| Agotamiento | Estado en que la resistencia llegó a 0. Lukas no puede correr, empujar ni rematar con golpe en el suelo. |
| Hackeo por secuencias | Entrada de una cadena de flechas en el terminal del brazo para abrir paneles. El juego no se pausa. |
| Sellado | Hackeo corto de 2 a 3 entradas que cierra una puerta. Dura hasta que Lukas la cruza o un enemigo la rompe en ~30 s. |
| Punto débil | Zona de cada enemigo que, al recibir un disparo, interrumpe su ataque o su agarre. No reduce los disparos necesarios. |
| Intento | Recorrido desde una reconstrucción hasta la muerte o la victoria. |
| Reconstrucción | Proceso con el que The Mother recompone a Lukas tras cada muerte. Siempre es el mismo hombre, con una pieza ajena más. |
| Inventario de llegada | Inventario que Lukas tenía al llegar al piso actual. Es el que recupera al morir, reiniciar o salir con costo. |
| Cinemática completa y corta | La completa muestra la reconstrucción una sola vez, al terminar la introducción. La corta es un plano de Lukas ante su cápsula en cada muerte o reinicio. |
| Sala procedural | Sala generada en cada intento para la exploración, las amenazas y los recursos. |
| Arquetipo de sala | Forma jugable de una sala procedural que cada piso viste con su tema. Son pasillo, almacén, sala abierta, vigilancia, hackeo y vertical. |
| Sala clave secundaria | Sala fija cuya posición cambia en cada intento. Contiene documentos, llaves y, a veces, el interruptor de energía. |
| Sala clave final | Sala fija con la escalera o el elevador al piso siguiente. Autoguarda en los pisos 1 y 2. |
| Fila de cápsulas | Sala de reconstrucción del piso 1 y primera sala del tutorial. Es el único refugio, porque los enemigos nunca entran. |
| Llave obligatoria y opcional | La obligatoria abre la sala clave final. La opcional abre atajos o salas con recursos. |
| Energía de la sala final | Estado en que la sala clave final empieza sin energía y exige usar un interruptor. Ocurre en ~1 de cada 3 casos en los pisos 1 y 2. |
| Cierre de tránsito | Bloqueo de la escalera y el elevador de la sala final mientras haya enemigos en caza, con el panel `LOCKDOWN`. Al terminar la caza se autoguarda y se abren. |
| Autoguardado | Guardado automático en la sala clave final de los pisos 1 y 2. Solo sirve para continuar tras salir del juego o un fallo. |
| Zona segura | Sala de inicio del piso o sala final ya autoguardada, siempre sin caza. Salir al menú ahí no tiene costo. |
| Semilla | Número que fija el resultado del generador de un piso. Cada entrada a un piso usa una nueva y la consola de depuración la muestra para reproducir bugs. |
| Configuración completa y showcase | La completa dura ~45 a 60 min y es la predeterminada. La showcase dura ~30 min, es para eventos y se activa con `-showcase`. |
| Modo Supervivencia y Modo Historia | Modos que se eligen al crear la partida. En el Historia, Lukas aguanta el doble de golpes, la detección va a la mitad y el mínimo de cada recurso por piso sube en 1. |
| Ranura | Una de las 3 partidas guardadas. `Continuar` usa la última y `Nueva partida` borra solo la suya. |
| Terminal del brazo | Terminal integrado en el antebrazo izquierdo de Lukas. Reúne inventario, documentos, bestiario, fallas y código de sujeto, y persiste entre muertes. |
| Código de sujeto | Registro de The Mother para el expediente Reiner-Hellen. R-H-01 a R-H-06 son de Emilia y de R-H-07 en adelante son de Lukas. |
| Documento corrupto | Documento con partes ilegibles por un glitch del laboratorio o del latido. Es distinto de una falla de la máquina. |
| Falla de la máquina | Ventaja de conocimiento temporal sobre el sistema de The Mother. Ella la parchea tras 1 o 2 usos o cuando Lukas muere. |
| Objetos confiscados | 12 objetos de narrativa escénica que se examinan y se registran en el terminal del brazo. |
| Punzón de descarga | Consumible de Kessler de un solo uso. Defiende de forma automática contra agarres o remata en silencio con `E`. |
| Revólver de Kessler | Única arma de la demo. The Mother lo devuelve descargado en una bandeja de efectos personales tras cada reconstrucción. |
| Reciclaje | El laboratorio termina de asimilar a un enemigo eliminado y no rematado. Se levanta donde cayó una sola vez, más rápido y más frágil. |
| Remate | Acción que evita el reciclaje, con golpe en el suelo o con punzón de descarga. |
| Contador de 5 muertes | Contador que activa el final Asimilación. Solo lo reinicia la sala clave final del piso 2. |
| Degradación | Deterioro estético acumulativo por muerte. Incluye corrupción CRT, distorsión del sonido vocal y cuerpo ajeno. |
| Asimilación | Pérdida total de identidad a manos del laboratorio. También es el nombre del final de derrota. |
| Hallazgo del piso 2 | Cinemática no saltable de la foto familiar imposible, la primera vez que Lukas llega a la sala final del piso 2. |
| Cámaras hostiles | Cámaras de la sala de The Mother en la fase 2. Cada enfoque libera un asimilado hacia ese punto. |
| Decisión final | Momento tras la victoria en que, con los 6 documentos de Emilia, Lukas elige con `E` la terminal central o la salida. |
| Secuencia común de victoria | Pasos que reproduce toda victoria antes de su cinemática, incluido el Despertar. |
| Despertar | Aceleración del latido del laboratorio al caer The Mother, en toda victoria. |
| Relevo básico y completo | Finales de victoria en los que Lukas reemplaza a The Mother. El básico ocurre sin los 6 documentos de Emilia y el completo con ellos. |
| Ambiguo | Final de victoria en el que Lukas elige la salida. Su destino no se aclara. |
| Gancho abierto | Cierre de todo final de victoria. Muestra una nueva llegada y la voz de The Mother iniciando el protocolo. |
| Latido del laboratorio | Sonido ambiental grave, distinto del latido de Lukas, que es el primer acto del texto. Crece con cada muerte y con la salud en Crítico. |
| Primer acto y segundo acto | Partes del texto que es el latido. El primero suena de fondo y no asimila. El segundo solo suena completo en el incidente y en el Relevo completo. |
| El incidente | Transmisión del latido por la megafonía de la instalación. Asimiló a quienes lo oyeron completo. |
| El Alto, la Boca, el Rastrero y el Mudo | Los 4 tipos de asimilados, los enemigos comunes de la demo. |
| Piso que no existe | Nivel sin plano ni botón donde muere Lukas en la introducción. Solo vuelve en la cinemática de Asimilación. |
| Kessler Sicherheit AG y Werkschutz | Corporación de seguridad aliada de Vitalis Konzern y su división de seguridad interna en Sektor V-7. |
| Congelación de funciones | Fecha desde la que solo se corrigen errores o se recorta alcance. Coincide con el cierre del hito H5, el 12 de noviembre de 2026. |
| Bestiario | Sección del terminal del brazo con las fichas clínicas de los 4 tipos de asimilados, el anexo de reciclados y la entrada de The Mother. |
| Entrada de bestiario de The Mother | Ficha del bestiario que explica la naturaleza de The Mother, con el formato de las entradas de enemigos. |
| Modo de telemetría | Registro local de métricas para los playtests. Se activa al lanzar el juego con `-telemetry`, igual que `-showcase`. Ver [[02 - Pruebas]]. |
| Definición de terminado | Condiciones que debe cumplir una característica para darla por terminada. Ver [[01 - Criterios de Calidad]]. |

## 2. Convenciones de nombres

- **Identity Dissolution** es el nombre del juego. **Heartbeat** se usa solo como nombre clave del proyecto.
- **Vitalis Konzern** es el único nombre de la corporación. No se abrevia ni se traduce.
- **Kessler Sicherheit AG** y **Werkschutz** no se traducen.
- **Punzón de descarga**, no *objeto defensivo* ni *consumible de remate*.
- **The Mother** va siempre en inglés y con el artículo *The*, también en español.
- **Sektor V-7** es una ubicación y no se traduce.
- **R-H-XX** es el código de sujeto, con dos dígitos, como R-H-07 o R-H-14.
- **Lukas Reiner-Hellen** y **Emilia Reiner-Hellen** llevan guion en el apellido.
- **Asimilación** nombra el final de derrota. **Disolución de identidad** nombra solo el tema del juego.
- **Corrupto** se usa para documentos. **Falla** se usa solo para las fallas de la máquina.
- **Distorsión del sonido vocal**, no *distorsión de voz*, porque el juego no tiene actuación de voz.
- **Reconstrucción**, no *clonación* ni *respawn*.
- **Cápsula** para la sala de reconstrucción, no *tanque*.
- Las mecánicas y los sistemas se citan por su nombre, sin códigos.


> **Navegación:** [[00 - Índice]] · ← [[04 - Criterios de Aceptación]] · [[02 - Referencias e Inspiración]] →
