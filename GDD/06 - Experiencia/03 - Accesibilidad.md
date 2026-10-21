# Accesibilidad

## Descripción de la sección

La accesibilidad busca que el mayor número posible de jugadores pueda comprender y utilizar el videojuego.

## Opciones previstas

- [x] **Tamaño de texto:** escala de subtítulos y del terminal (mínimos a 1080p: subtítulos ≥ 32 px, terminal ≥ 24 px; [[04 - Interfaz Visual]]).
- [x] **Subtítulos:** obligatorios para todas las voces (D-52); se puede ajustar tamaño y fondo.
- [x] **Fuente para dislexia (D-126):** opcional y desactivada por defecto; usa OpenDyslexic en documentos, terminal, menús y subtítulos.
- [x] **Contraste:** el texto mantiene 4.5:1 sobre su fondo; opción de fondo opaco para subtítulos.
- [ ] **Modo para daltonismo:** no hay filtro. En su lugar, ningún estado se comunica solo con color (regla visual 1 de [[01 - Dirección de Arte]]).
- [x] **Reasignación de controles:** todas las acciones de teclado y ratón ([[04 - Controles]]).
- [x] **Sensibilidad:** separada para mirar y apuntar; invertir eje vertical.
- [x] **Reducción de movimiento (D-106):** quita la vibración de cámara al recibir daño y suaviza el balanceo.
- [x] **Reducción de destellos (D-106):** suaviza parpadeos (linterna sin carga, luces que fallan, descarga del punzón).
- [x] **Reducción de efectos CRT y glitch (D-106):** baja líneas de escaneo, ruido y glitch (vigilancia, degradación y documentos), sin borrar la diferencia entre la señal normal y la degradada (D-23).
- [x] **Asistencia de apuntado (D-104):** opcional; el punto frena un poco sobre los enemigos. Desactivada por defecto con teclado y ratón; activada por defecto con mando.
- [x] **Dificultad ajustable (D-103):** Modo Supervivencia y Modo Historia, elegidos al crear la partida.
- [x] **Volumen independiente:** general, música, efectos, ambiente y voces (sonidos vocales).
- [x] **Indicadores visuales para sonidos importantes (D-102):** subtítulos de sonido opcionales, desactivados por defecto.

## Consideraciones

- **Subtítulos de sonido (D-102):** describen lo que se oye con su dirección aproximada, por ejemplo `[Pasos rápidos, detrás]`, `[Grito lejano, izquierda]` o `[Crujidos cerca]`. No dan distancia exacta ni posición en pantalla, así que respetan D-02: describen el sonido, no la percepción del enemigo. Cubren los estados de alerta, el latido del protagonista, el grito de la Boca, el aviso de reciclaje, las cámaras hostiles y los avisos del sistema.
- **Modo Historia (D-103):** ~50% menos daño recibido (cada enemigo necesita el doble de golpes para matar a Lukas, D-116), detección enemiga más lenta y +1 al mínimo de cada recurso por piso (1 blíster en munición; sin cambio en las llaves; el máximo sube si queda por debajo, D-131). Se mantienen la muerte, la reconstrucción, la degradación y todos los finales, incluida la Asimilación. Los valores son iniciales, sujetos a balance.
- **Oscuridad:** calibración de brillo en el primer inicio (D-106); los recogibles destellan bajo la luz (D-84).
- **Interfaz diegética:** los estados se leen en el terminal con etiquetas de texto claras (D-86), no solo con iconos o color.
- **Tiempo real:** el inventario y el terminal no pausan (M-07); la pausa siempre está disponible, también en la decisión final (D-92).
- **Pixelado:** el render a ~540p no se puede desactivar (D-81); la interfaz y los subtítulos se dibujan a resolución nativa para que sean legibles.
- **Idiomas:** solo texto, en inglés y español (D-62).


> **Navegación:** [[00 - Índice]] · ← [[02 - UI y Menús]] · [[04 - Guardado y Configuración]] →
