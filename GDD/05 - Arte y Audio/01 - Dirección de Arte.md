# Dirección de Arte

## 1. Descripción de la sección

Define la identidad visual del videojuego y las reglas que deben seguir los recursos gráficos.

## 2. Identidad visual

Los porcentajes y los tonos de este documento son valores iniciales, sujetos a revisión en el motor y a balance.

**Estilo:** Low-poly con luz moderna. Los modelos tienen pocos polígonos y texturas de baja resolución con filtro *point*, sin suavizado. Se apoyan en la iluminación dinámica, las sombras y la niebla de Unity. La imagen se renderiza a una resolución interna de ~540p, se reescala a la resolución de pantalla y lleva un *dithering* suave. No hay temblor de vértices ni texturas afines, así que no es PSX puro. La luz por píxel hace falta para que la linterna y la oscuridad funcionen como mecánica.

**Referencias:** *Signalis* es la inspiración visual principal por su atmósfera, su low-poly y sus texturas crudas con detalle gráfico. También el terror físico de *Silent Hill*. Ver [[02 - Referencias e Inspiración]].

**Sensación buscada:** Vulnerabilidad y frío, según el pilar Vulnerabilidad constante. La instalación está en estado de emergencia y casi a oscuras. La poca luz que existe delata tanto como revela.

**Nivel de realismo:** Estilizado. Lukas y el personal tienen proporciones humanas realistas, y los asimilados, una deformación grotesca. Los detalles finos se resuelven en la textura, no en la geometría.

**Formas predominantes:**

- **Instalación.** Formas industriales y rectas, como tuberías, rejillas, paneles y maquinaria de los 90 con tecnología de 2083.
- **Asimilados.** Formas orgánicas, alargadas y rotas.
- **The Mother.** Formas mecánicas y verticales, con estantería, grúa y brazo. La carne nunca aparece en The Mother ni en su sala.

**Iluminación:** El juego está casi siempre a oscuras porque la instalación está en estado de emergencia. La luz ambiental es casi nula. Las fuentes son focos aislados de emergencia con el tinte del piso y, en segundo plano, las pantallas. La linterna de Lukas es la única luz blanca y la más fuerte de la escena. El detalle está en [[03 - Entornos]].

**Materiales:** Hormigón, metal pintado y oxidado, rejilla, azulejo clínico, cristal, plástico médico y carne asimilada. El equipo de Kessler usa polímero y cerámica. No hay PBR complejo. Cada material usa color de textura, una máscara de brillo para el destello especular de los recogibles y emisivos para pantallas y costuras.

## 3. Paleta de color

Los colores son los del proyecto en Unity, tomados de sus materiales, luces, escenas e interfaz. Las cuatro paletas y la imagen de ambientación de la sección 5 quedan como referencia de origen.

**Colores principales:**

- **Base, para la estructura.** `#2A2629` `#3A3438` `#4B4448` `#55505A`. Son grises cálidos con matiz ciruela. Cubren hormigón, metal, ropa, cadáveres y sombras, y ocupan ~70% de la pantalla en cualquier piso. La luz ambiental de los pisos es `#0D0B0C`, casi negra, así la oscuridad conserva matiz.
- **Cian, para Vitalis Konzern.** `#19C4DC`. Se usa en el logotipo, las etiquetas y las bandas corporativas. Es más saturado y luminoso que el teal de Lukas.
- **Carmesí, para la carne y el peligro.** `#922235`, con emisivo `#DB3350`. Aparece en los ribetes del overol de Lukas y en la carne de los asimilados. También en el tinte del piso 3 y de la sala de The Mother, cuya luz es `#D91F2E`. Lo usan además la luz roja de las cámaras hostiles y los avisos de The Mother en el terminal.

**Colores secundarios:**

- **Teal, para Lukas.** `#2F7F7A` en el material del overol. Las costuras y la luz tenue de Lukas son `#6E9FA0`, y el texto del terminal del brazo es `#4FE0CC`. Ver [[03 - Personajes]].
- **Morado, para lo cósmico.** `#5B3A78`, y `#4A4250` en la piel asimilada. Queda reservado al latido y a lo que no es humano. Eso incluye el piso que no existe, el glitch de los documentos y el tinte de la corrupción CRT por degradación.
- **Verde fósforo, para las pantallas de la instalación.** `#59FF8C` en los emisivos y `#73F273` en la interfaz. Aparece en la vista de vigilancia, los paneles de hackeo de las puertas, las consolas y el contador del revólver de Kessler.
- **Ámbar, para la luz de sodio.** `#FFB347` es la luz de emergencia del piso 1. Los avisos de la interfaz usan `#F2B33D`.
- **Blanco frío, para la clínica.** El fluorescente del piso 2 es `#C7E6FF`.
- **Blanco cálido, para la linterna.** `#FFF2DE`.

| Zona | Luz dominante | Acentos |
|---|---|---|
| Introducción, casa y exterior | Base con luz blanca sucia | Ninguno. Lukas civil no tiene emisivos |
| Piso 1, Mantenimiento | Ámbar de sodio `#FFB347` | Verde de consolas |
| Piso 2, Clínica ilegal | Fluorescente blanco frío `#C7E6FF` | Verde de consolas y carmesí en restos de experimentos |
| Piso 3, Núcleo y sala de The Mother | Carmesí `#D91F2E` | Verde de consolas y morado en lo cósmico |
| Piso que no existe | Base casi negra | Morado |

**Colores para información jugable:**

| Color | Significado |
|---|---|
| Teal | Información de Lukas, como el terminal, el inventario y los estados |
| Verde | Información de la instalación, como cámaras, paneles y el contador del revólver |
| Rojo, `#922235` en el mundo y `#FF5242` en la interfaz | Amenaza o The Mother. Nunca es decoración neutra |
| Cian | La marca de Vitalis. Nunca es información del jugador |
| Morado | Algo no es humano. Nunca aparece en lo cotidiano |
| Blanco cálido | Solo la linterna |

## 4. Reglas visuales

1. **El color nunca es la única señal.** Todo estado comunicado con color tiene también forma, texto, parpadeo o sonido. Rojo y verde son el par más difícil con deuteranopía. Ver [[03 - Accesibilidad]].
2. **Un tinte de luz por sala y como máximo un acento de información.** Carmesí, morado, verde, teal y ámbar juntos saturan la imagen.
3. **El verde `#59FF8C` solo va en emisivos pequeños**, como texto y lecturas de pantallas. Nunca cubre superficies grandes.
4. **Cada color fuerte tiene un solo sentido.** El rojo saturado es amenaza, el cian es Vitalis y el morado es lo cósmico. Ninguno se usa como decoración.
5. **La oscuridad es una mecánica, no un filtro.** Las salas se leen con la linterna. Sin ella solo se distinguen siluetas, focos de emergencia y pantallas.
6. **Los recogibles solo destacan bajo la luz.** Muestran un destello especular sutil al iluminarlos con la linterna o con un foco. A oscuras no tienen brillo propio visible.
7. **La silueta de Lukas siempre se separa del fondo.** Por eso la clínica no usa luz teal. Una luz de contorno fría, `#9EB8D1`, detrás y por encima de Lukas, ilumina solo su cuerpo.
8. **Horror corporal sin desmembramiento.** Hay cuerpos deformados, carne expuesta, sangre oscura e impactos de bala, sin desmembramiento ni vísceras. A los niños solo se alude con objetos, nunca con cuerpos. No hay desnudez explícita.
9. **La degradación nunca tapa información.** La corrupción CRT por degradación es morada. Se distingue de la señal de vigilancia degradada por búsqueda, que es verde con ruido.

## 5. Referencias

| Paleta base | Paleta carmesí |
| --- | --- |
| ![Paleta base|300](../Media/Referencia/PaletaBase.png) | ![Paleta carmesí|300](../Media/Referencia/PaletaCarmesi.png) |

| Paleta morados, con morado y teal | Paleta verde de pantallas |
| --- | --- |
| ![Paleta morados|300](../Media/Referencia/PaletaMorados.png) | ![Paleta verde|300](../Media/Referencia/PaletaVerde.png) |

La imagen de ambientación muestra los tintes ámbar, verde teal y rojo por altura. Solo sirve como guía de color, porque es pixel art 2D y su texto en español no sigue la regla de idiomas del juego.

![Ambientación con colores oscuros y fríos|520](../Media/Referencia/ReferenciaConColoresOscurosFrios.jpg)

El diseño de referencia de Lukas civil y de R-H-07 está en [[02 - Visión del Juego]].

> **Navegación:** [[00 - Índice]] · ← [[05 - Misiones y Desafíos]] · [[02 - Personajes y Animación]] →
