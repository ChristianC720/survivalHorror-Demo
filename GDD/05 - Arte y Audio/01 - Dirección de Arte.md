# Dirección de Arte

## Descripción de la sección

Define la identidad visual del videojuego y las reglas que deben seguir los recursos gráficos.

## Identidad visual

**Estilo:** Low-poly con luz moderna (D-81). Modelos de pocos polígonos y texturas de baja resolución con filtro *point* (sin suavizado), sobre iluminación dinámica, sombras y niebla de Unity. La imagen se renderiza a una resolución interna de ~540p, se reescala a la resolución de pantalla y lleva un *dithering* suave. Sin temblor de vértices ni texturas afines (no es PSX puro): la luz por píxel es necesaria para que la linterna y la oscuridad funcionen como mecánica (M-12).

**Referencias:** *Signalis* (atmósfera, low-poly y texturas crudas) y el terror físico de *Silent Hill* (D-60). Ver [[02 - Referencias e Inspiración]].

**Sensación buscada:** Vulnerabilidad y frío (Pilar 1). Una instalación en estado de emergencia, casi a oscuras, donde la poca luz que existe delata tanto como revela.

**Nivel de realismo:** Estilizado. Proporciones humanas realistas en Lukas y el personal; deformación grotesca en los asimilados. Los detalles finos se resuelven en la textura, no en la geometría.

**Formas predominantes:** Industriales y rectas en la instalación: tuberías, rejillas, paneles y maquinaria de los 90 con tecnología de 2083 (D-46, D-59). Orgánicas, alargadas y rotas en los asimilados. Mecánicas y verticales en The Mother: estantería, grúa y brazo (D-65). La carne nunca aparece en The Mother ni en su sala (D-59).

**Iluminación:** El juego está mayormente a oscuras: la instalación está en **estado de emergencia** (D-82). Luz ambiental casi nula; focos aislados de emergencia con el tinte del piso y pantallas como fuentes secundarias. La linterna de Lukas es la única luz blanca y la más fuerte de la escena. Detalle en [[03 - Entornos]].

**Materiales:** Hormigón, metal pintado y oxidado, rejilla, azulejo clínico, cristal, plástico médico, polímero y cerámica (equipo de Kessler, D-74), carne asimilada. Sin PBR complejo: color de textura, una máscara de brillo para el destello especular de los recogibles (D-84) y emisivos para pantallas y costuras.

## Paleta de color

Cuatro paletas de referencia más un ámbar sacado de la imagen de ambientación (D-82). Imágenes en `10 - Referencia` (ver [[#Referencias]]). Los porcentajes y los tonos ámbar son valores iniciales, sujetos a revisión en motor.

**Colores principales:**

- **Base (estructura):** `#0f0607` `#2c1f20` `#4b3f3e` `#746a68` `#96928c`. Grises cálidos con matiz ciruela. Estructura de todo el juego: hormigón, metal, ropa, cadáveres y sombras. Ocupa ~70% de la pantalla en cualquier piso. `#0f0607` sustituye al negro puro: la oscuridad conserva matiz.
- **Carmesí (Vitalis, carne y peligro):** `#090205` `#270a16` `#51111f` `#922235` `#452635`. Logotipo y ribetes de Vitalis (incluidos los del overol de Lukas), carne de los asimilados, tinte del piso 3 y de la sala de The Mother, luz roja de las cámaras hostiles (D-28) y avisos de The Mother en el terminal.

**Colores secundarios:**

- **Teal (Lukas y clínica):** `#1a2527` `#355654` `#6e9fa0` (de la paleta Morados). Overol de R-H-07, costuras luminosas y terminal del brazo (ver [[03 - Personajes]]). Las sombras de la clínica usan además `#08241E` (Deep Slate Green).
- **Morado (lo cósmico):** `#080409` `#492153`. Solo para el latido y lo que no es humano: piso que no existe, glitch de los documentos (D-58), tinte de la corrupción CRT por degradación (M-18) y brillo iridiscente de la piel asimilada.
- **Verde fósforo (pantallas de la instalación):** `#08241E` `#054737` `#026D50` `#029061` `#02B574` `#02DF8B`. Vista de vigilancia, paneles de hackeo de las puertas, consolas y contador del revólver de Kessler.
- **Ámbar (luz de sodio):** tonos tomados de la imagen de referencia, aproximadamente `#3a2410` `#8a5a1c` `#d08a2e` `#f0b860`. Luz de emergencia del piso 1.

**Reparto por zona (D-82):**

| Zona | Luz dominante | Acentos |
|---|---|---|
| Introducción (casa y exterior) | Base con luz blanca sucia | Ninguno; Lukas civil sin emisivos |
| Piso 1 — Mantenimiento | Ámbar de sodio | Verde de consolas |
| Piso 2 — Clínica ilegal | Fluorescente blanco frío, sombras `#08241E` | Verde de consolas; carmesí en restos de experimentos |
| Piso 3 — Núcleo y sala de The Mother | Carmesí | Verde de consolas; morado en lo cósmico |
| Piso que no existe | Base casi negra | Morado |

**Colores para información jugable:**

- **Teal:** información de Lukas (terminal, inventario, estados).
- **Verde:** información de la instalación (cámaras, paneles, contador del revólver).
- **Rojo `#922235`:** amenaza, Vitalis o The Mother. Nunca decoración neutra.
- **Morado:** algo no es humano. Nunca aparece en lo cotidiano.
- **Blanco cálido:** solo la linterna.

## Reglas visuales

1. **El color nunca es la única señal (D-82).** Todo estado comunicado con color tiene también forma, texto, parpadeo o sonido; rojo y verde son el par más difícil con deuteranopía (ver [[03 - Accesibilidad]]).
2. **Un tinte de luz por sala y como máximo un acento de información.** Carmesí, morado, verde, teal y ámbar juntos saturan la imagen.
3. **El verde `#02DF8B` solo en emisivos pequeños** (texto y lecturas de pantallas), nunca en superficies grandes.
4. **El rojo saturado significa amenaza o Vitalis;** el morado, lo cósmico. Ninguno se usa como decoración.
5. **La oscuridad es mecánica, no un filtro.** Las salas son legibles con la linterna; sin ella solo se leen siluetas, focos de emergencia y pantallas.
6. **Los recogibles solo destacan bajo la luz (D-84):** un destello especular sutil al iluminarlos con la linterna o un foco; ningún brillo propio visible a oscuras.
7. **La silueta de Lukas siempre se separa del fondo.** Por eso la clínica no usa luz teal (D-82).
8. **Horror corporal sin desmembramiento (D-85).** Cuerpos deformados, carne expuesta, sangre oscura e impactos de bala; sin desmembramiento ni vísceras. A los niños solo se alude con objetos (D-73), nunca con cuerpos. Sin desnudez explícita.
9. **La degradación nunca tapa información (D-29).** La corrupción CRT por degradación es morada y se distingue de la señal de vigilancia degradada por búsqueda (verde con ruido).

## Referencias

Imágenes guardadas en `10 - Referencia`:

- Paleta base: ![[PaletaBase.png]]
- Paleta carmesí: ![[PaletaCarmesi.png]]
- Paleta morados (morado y teal): ![[PaletaMorados.png]]
- Paleta verde (pantallas): ![[PaletaVerde.png]]
- Ambientación (tintes ámbar, verde-teal y rojo por altura; solo como guía de color: es pixel art 2D y su texto en español no sigue D-62): ![[ReferenciaConColoresOscurosFrios.jpg]]
- Lukas civil (introducción): ![[Lukas-Reiner-Hellen.jpg]]
- Lukas R-H-07 (reconstrucción): ![[Lukas-R-H-07.jpg]]


> **Navegación:** [[00 - Índice]] · ← [[05 - Misiones y Desafíos]] · [[02 - Personajes y Animación]] →
