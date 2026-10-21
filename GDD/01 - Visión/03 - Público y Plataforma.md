# Público y Plataforma

## Descripción de la sección

Define para quién se diseña el videojuego y dónde se jugará. Las decisiones de diseño, interfaz, controles y rendimiento dependen de estas variables.

## Público objetivo

**Edad aproximada:** 20 a 30 años

**Experiencia esperada:**  Intermedia.

**Intereses relevantes:**
- Survival Horror.
- Terror psicológico y ambiental.
- Exploración.
- Administración de recursos.
- Generación procedural.
- Narrativas de misterio y terror.
- Juegos donde combatir no siempre es la mejor solución.
- Experiencias para un solo jugador.
- Rejugabilidad.
- Descubrimiento progresivo de la historia y el mundo.

**Conocimientos previos necesarios:**
Se busca que el jugador no requiera haber jugado previamente títulos del género.

El videojuego deberá enseñar:

- Movimiento.
- Interacción con objetos y escenarios.
- Uso del inventario.
- Administración de recursos.
- Combate.
- Evasión de amenazas.
- Funcionamiento básico de la cámara dinámica.
- Reglas asociadas a la muerte y al reinicio de una partida.

El jugador deberá poder comprender estas mecánicas mediante un tutorial contextual, diseño del entorno y retroalimentación visual o auditiva.

## Plataforma

| Elemento                | Decisión                                                        |
| ----------------------- | --------------------------------------------------------------- |
| Plataforma principal    | PC                                                              |
| Plataformas secundarias | Ninguna                                                         |
| Método de entrada       | Teclado y mouse como método principal; mando con mapa básico fijo (D-120) |
| Resolución objetivo     | 1920 × 1080 como objetivo inicial                               |
| Relación de aspecto     | 16:9 como formato principal                                     |
| Rendimiento objetivo    | 60 FPS                                                          |
| Conexión requerida      | No                                                              |

## Restricciones de plataforma

### Hardware:

El videojuego deberá diseñarse considerando que no todos los usuarios dispondrán de hardware de gama alta y se buscará evitar que los principales sistemas jugables dependan de características gráficas excesivamente costosas.

Deberán considerarse especialmente:
- Cantidad de enemigos activos simultáneamente.
- Complejidad de iluminación y sombras.
- Cantidad de luces dinámicas.
- Partículas y efectos visuales.
- Complejidad de modelos 3D.
- Resolución de texturas.
- Generación procedural de habitaciones.
- Carga y descarga de escenarios.
- Uso de memoria RAM y memoria gráfica.

Los requisitos mínimos y recomendados provisionales están en [[03 - Requisitos Técnicos]] (D-112) y se confirman con el perfilado de la semana 6 del [[03 - Roadmap]].

### Controles:

El diseño deberá priorizar teclado y mouse, considerando el soporte completo para control que se implementará posteriormente.
Las acciones principales deberán poder realizarse sin combinaciones excesivamente complejas, y los controles deberán contemplar como mínimo:

- Movimiento.
- Control de cámara cuando corresponda.
- Interacción.
- Ataque.
- Apuntar, si la mecánica lo requiere.
- Realizar acciones defensivas (empujón / golpe de culata).
- Acceso al inventario.
- Uso de objetos.
- Pausa.

### Memoria y generación procedural:

La generación procedural deberá evitar cargar simultáneamente contenido que el jugador no necesita.

Siempre que la implementación lo permita, los escenarios deberán organizarse de forma que habitaciones, enemigos y recursos puedan activarse o desactivarse dependiendo de la posición del jugador.

### Almacenamiento:

Objetivo: build de **≤ 2 GB** y 3 GB libres para instalarlo (D-112, ver [[03 - Requisitos Técnicos]]).

El tamaño final dependerá principalmente de:

- Modelos 3D.
- Texturas.
- Audio.
- Animaciones.
- Cinemáticas, si existen.
- Cantidad de escenarios y variantes.

### Conectividad:

No se requerirá conexión permanente a Internet para jugar.

## Sesión de juego

**Duración esperada de una sesión:** ~10 minutos como mínimo por sesión, con tutorial incluido (valor inicial, sujeto a balance). El autoguardado de la sala clave final de los pisos 1 y 2 permite cerrar el juego y continuar después (D-30).

**Duración total estimada del juego:** ~45–60 minutos en la configuración completa (3 pisos; D-33, D-34) (valor inicial, sujeto a balance).

**Duración en la configuración de showcase:** ~30 minutos, con menos salas por piso y enemigos y recursos escalados en proporción, para eventos de presentación (D-34) (valor inicial, sujeto a balance). El detalle de ambas configuraciones se documenta en [[02 - Mecánicas]] (M-15).

## Modelo de distribución

Prototipo académico / Demo jugable.

La versión desarrollada inicialmente tiene como propósito demostrar los principales sistemas, mecánicas y decisiones de diseño del videojuego.

Actualmente no se ha definido un modelo comercial para una hipotética versión final.
## Consideraciones

- [x] Controles adaptados a la plataforma
- [ ] Interfaz legible
- [x] Rendimiento considerado
- [x] Resoluciones consideradas
- [x] Guardado compatible


> **Navegación:** [[00 - Índice]] · ← [[02 - Visión del Juego]] · [[04 - Pilares de Diseño]] →
