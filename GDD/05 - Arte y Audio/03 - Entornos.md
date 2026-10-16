# Entornos

## Descripción de la sección

Define la estética y necesidades visuales de escenarios, fondos, objetos y elementos ambientales.

Estructura jugable de cada nivel en [[02 - Niveles]]; lugares y su lectura narrativa en [[02 - Mundo]]; paleta en [[01 - Dirección de Arte]].

## Zonas

| Zona | Estilo | Elementos | Función |
|---|---|---|---|
| Casa de Lukas | Doméstica, sencilla y descuidada; luz blanca sucia | Foto familiar, muebles gastados, teléfono | Solo cinemática de la introducción (D-53) |
| Exterior, compuerta y elevador | Sektor V-7 de noche; industrial y corporativo | Compuerta sellada con panel, logotipo de Vitalis, cabina del elevador | Primer hackeo con el teléfono; el elevador se distorsiona y cae (D-63) |
| Piso que no existe | Pasillo único casi negro; morado en lo que no es humano | Pasillo sin señalización, figura a lo lejos | Muerte guionizada; vuelve solo en la Asimilación (D-61) |
| Piso 1 — Mantenimiento | Ámbar de sodio sobre la base; calderas, tuberías y talleres | Túnel de tuberías, depósito de herramientas, sala de calderas, puesto de guardia de Kessler, cuadro eléctrico, conductos | Tutorial y aprendizaje del ciclo |
| Fila de cápsulas | Clínica dentro del piso 1; luz fría sobre las cápsulas | 7 o más cápsulas, algunas con restos; bandeja de efectos personales | Reaparición tras cada reconstrucción (M-16, D-74) |
| Piso 2 — Clínica ilegal | Fluorescente blanco frío, sombras `#08241E`; clínica improvisada en espacios industriales | Corredor de pacientes, farmacia y archivo, sala de espera, control de enfermería, quirófano, pozo de montacargas | Experimentos inhumanos; foto imposible en la sala final (D-56) |
| Piso 3 — Núcleo | Carmesí; maquinaria de contención | Galería de cableado, almacén de viales, sala de contención, sala de monitores, nodo de datos, pasarelas sobre el núcleo | Máxima presión antes del jefe |
| Sala de The Mother | Central de megafonía; carmesí y máquina, sin carne | Bobinas de cinta, mesas de mezcla, altavoces del tamaño de una pared, grúa y archivo criogénico, terminal central, salida | Jefe de 3 fases y finales (D-65) |

Los nombres de sala por arquetipo y tema salen de la tabla de [[02 - Niveles]] (D-69).

## Modularidad

- **Kit modular por piso sobre una rejilla común** (módulos de 2 m de lado, valor inicial). Los 6 arquetipos comparten geometría base (muros, suelos, puertas, marcos, techos) y cada piso cambia texturas, props y luces. Así el generador combina salas sin piezas a medida (S-06).
- **Reutilizables entre pisos:** puertas y marcos, paneles de hackeo, cámaras de seguridad, rejillas, tuberías, cajas, estanterías, camillas, terminales y consolas, señalización de Vitalis (con variantes por idioma: inglés y alemán sin traducir, D-58, D-62).
- **Props por tema:** herramientas y calderas (piso 1), equipo médico y archivadores (piso 2), viales y cableado (piso 3).
- **Salas clave:** fijas pero recolocables; usan el mismo kit con un decorado propio para que se reconozcan.
- **Objetos confiscados:** objetos cotidianos con una etiqueta de inventario común (D-73).

## Iluminación

- **Global:** estado de emergencia (D-82). Luz ambiental casi nula; la escena se lee por focos aislados y por la linterna.
- **Fuentes de luz:**
  - Focos de emergencia con el tinte del piso (ámbar, blanco frío o carmesí).
  - Pantallas verdes de la instalación y terminal teal de Lukas (emisivos débiles).
  - **Linterna:** la única luz blanca (cálida neutra) y la más fuerte; proyecta sombras y concentra el haz al apuntar (M-12).
  - Luz roja de las cámaras hostiles en la fase 2 del jefe (D-28).
- **Cambios durante el juego:**
  - Sala clave final sin energía: oscura hasta restaurar el interruptor (D-72).
  - El latido no cambia la luz; la degradación solo afecta a la capa CRT (M-18).
  - Fase 3 del jefe: el núcleo abierto ilumina la sala.
- **Rendimiento:** pocas luces dinámicas con sombra por sala (la linterna siempre); el resto, luces sin sombra o precalculadas en los módulos (ver [[03 - Público y Plataforma]]).

## Efectos visuales

- **Partículas:** polvo en el haz de la linterna, chispas de cables y transformadores, partículas del reciclaje (M-20).
- **Clima:** ninguno; todo el juego es interior salvo la introducción.
- **Humo:** vapor de calderas (piso 1) y niebla criogénica en la fase 2 del jefe (D-65).
- **Fuego:** ninguno.
- **Agua:** charcos y goteras en el piso 1; coinciden con los pasos sobre agua (M-10).
- **Magia / energía:** no hay magia. Hay energía: la descarga del punzón (destello blanco-teal), la distorsión morada de lo cósmico y la corrupción CRT por degradación.
- **Impactos:** impactos de bala en superficies y sangre oscura en la carne, sin desmembramiento (D-85).

## Recursos requeridos

- **Modelos:**
  - Kit modular de 3 temas (piso 1, 2 y 3).
  - Fila de cápsulas.
  - Sala de The Mother.
  - Casa, exterior, compuerta y elevador.
  - Piso que no existe.
  - Props por tema.
  - Revólver de Kessler, punzón de descarga, cura, blíster, llaves y anillo.
  - 12 objetos confiscados.
  - Puertas, paneles de hackeo, interruptor de energía y cámaras.
- **Texturas:** hojas de textura por tema (256–512 px, filtro *point*), señalización y logotipos de Vitalis y Kessler, documentos.
- **Materiales:**
  - Base sin PBR complejo.
  - Máscara de destello para los recogibles (D-84).
  - Emisivos (pantallas, costuras, terminal).
  - Carne asimilada con brillo morado.
- **Efectos:** los de la lista anterior, más el post-proceso global (reescalado, *dithering*, CRT de la vigilancia y de la degradación).


> **Navegación:** [[00 - Índice]] · ← [[02 - Personajes y Animación]] · [[04 - Interfaz Visual]] →
