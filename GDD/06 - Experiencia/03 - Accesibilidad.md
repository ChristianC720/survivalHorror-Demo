# Accesibilidad

## 1. Descripción de la sección

La accesibilidad busca que el mayor número posible de jugadores pueda comprender y utilizar el videojuego.

## 2. Opciones previstas

Los valores numéricos de este documento son iniciales y están sujetos a balance.

- [x] **Tamaño de texto.** Escala de los subtítulos y del terminal. A 1080p los mínimos son 32 px para subtítulos y 24 px para el terminal. Ver [[04 - Interfaz Visual]].
- [x] **Subtítulos.** Son obligatorios para todas las voces. Se ajustan su tamaño y su fondo.
- [x] **Fuente para dislexia.** Es opcional y viene desactivada. Usa OpenDyslexic en documentos, terminal, menús y subtítulos.
- [x] **Contraste.** El texto mantiene 4,5:1 sobre su fondo. Los subtítulos admiten un fondo opaco.
- [ ] **Modo para daltonismo.** No hay filtro. En su lugar, ningún estado se comunica solo con color, según la regla visual 1 de [[01 - Dirección de Arte]].
- [x] **Reasignación de controles.** Cubre todas las acciones de teclado y mouse. Ver [[04 - Controles]].
- [x] **Sensibilidad.** Separada para mirar y para apuntar, con opción de invertir el eje vertical.
- [x] **Reducción de movimiento.** Quita la vibración de cámara al recibir daño y suaviza el balanceo.
- [x] **Reducción de destellos.** Suaviza los parpadeos de la linterna sin carga, de las luces que fallan y de la descarga del punzón.
- [x] **Reducción de efectos CRT y glitch.** Baja las líneas de escaneo, el ruido y el glitch de la vigilancia, la degradación y los documentos. Conserva la diferencia entre la señal normal y la degradada.
- [x] **Asistencia de apuntado.** Es opcional. El punto frena un poco sobre los enemigos. Viene desactivada con teclado y mouse, y activada con mando.
- [x] **Dificultad ajustable.** Modo Supervivencia y Modo Historia, elegidos al crear la partida.
- [x] **Volumen independiente.** General, música, efectos, ambiente y voces, que son los sonidos vocales.
- [x] **Indicadores visuales para sonidos importantes.** Los subtítulos de sonido son opcionales y vienen desactivados.

## 3. Consideraciones

**Subtítulos de sonido.** Describen lo que se oye con su dirección aproximada, por ejemplo `[Pasos rápidos, detrás]`, `[Grito lejano, izquierda]` o `[Crujidos cerca]`. No dan la distancia exacta ni la posición en pantalla. Así describen el sonido y no la percepción del enemigo, que nunca se muestra. Cubren los estados de alerta, el latido del protagonista, el grito de la Boca, el aviso de reciclaje, las cámaras hostiles y los avisos del sistema.

![Subtítulo de sonido de pasos detrás de Lukas|520](../Media/Capturas/SubtitulosSonido.png)

**Modo Historia.**

- Lukas recibe ~50% menos daño, así que cada enemigo necesita el doble de golpes para matarlo.
- La detección enemiga es más lenta.
- El mínimo de cada recurso por piso sube en 1. El detalle está en [[06 - Economía y Recursos]].
- Cada blíster de munición da 6 balas, frente a las 4 del Modo Supervivencia.
- Se mantienen la muerte, la reconstrucción, la degradación y todos los finales, incluida la Asimilación.

**Oscuridad.** El primer inicio incluye la calibración de brillo. Los recogibles destellan bajo la luz.

**Interfaz diegética.** El terminal muestra los estados con etiquetas de texto claras, además de iconos y color.

**Tiempo real.** El inventario y el terminal no pausan. La pausa siempre está disponible, también en la decisión final.

**Pixelado.** El render a ~540p no se puede desactivar. La interfaz y los subtítulos se dibujan a resolución nativa para que se lean bien.

**Idiomas.** Solo de texto, en inglés y en español.

> **Navegación:** [[00 - Índice]] · ← [[02 - UI y Menús]] · [[04 - Guardado y Configuración]] →
