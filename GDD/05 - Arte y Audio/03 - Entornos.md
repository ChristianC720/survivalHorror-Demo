# Entornos

## 1. Descripción de la sección

Define la estética y necesidades visuales de escenarios, fondos, objetos y elementos ambientales.

La estructura jugable de cada nivel está en [[02 - Niveles]], los lugares y su lectura narrativa en [[02 - Mundo]] y la paleta en [[01 - Dirección de Arte]]. Los valores numéricos de este documento son iniciales y están sujetos a balance.

## 2. Zonas

| Zona | Estilo | Elementos | Función |
|---|---|---|---|
| Casa de Lukas | Doméstica, sencilla y descuidada, con luz blanca sucia | Foto familiar, muebles gastados y teléfono | Solo cinemática de la introducción |
| Exterior, compuerta y elevador | Sektor V-7 de noche, industrial y corporativo | Compuerta sellada con panel, logotipo de Vitalis y cabina del elevador | Primer hackeo con el teléfono. El elevador se distorsiona y cae |
| Piso que no existe | Pasillo único casi negro, con morado en lo que no es humano | Pasillo sin señalización y una figura a lo lejos | Muerte guionizada. Solo vuelve en la Asimilación |
| Piso 1, Mantenimiento | Ámbar de sodio sobre la base, con calderas, tuberías y talleres | Túnel de tuberías, depósito de herramientas, sala de calderas, puesto de guardia de Kessler, cuadro eléctrico y conductos | Tutorial y aprendizaje del ciclo |
| Fila de cápsulas | Clínica dentro del piso 1, con luz fría sobre las cápsulas | 7 o más cápsulas, algunas con restos, y la bandeja de efectos personales | Reaparición tras cada reconstrucción |
| Piso 2, Clínica ilegal | Fluorescente blanco frío `#C7E6FF`. Es una clínica improvisada en espacios industriales | Corredor de pacientes, farmacia y archivo, sala de espera, control de enfermería, quirófano y pozo de montacargas | Experimentos inhumanos. La foto imposible está en la sala final |
| Piso 3, Núcleo | Carmesí, con maquinaria de contención | Galería de cableado, almacén de viales, sala de contención, sala de monitores, nodo de datos y pasarelas sobre el núcleo | Máxima presión antes del jefe |
| Sala de The Mother | Central de megafonía, carmesí y máquina, sin carne | Bobinas de cinta, mesas de mezcla, altavoces del tamaño de una pared, grúa y archivo criogénico, terminal central y salida | Jefe de 3 fases y finales |

Los nombres de sala por arquetipo y tema salen de la tabla de [[02 - Niveles]].

| Piso 1, Mantenimiento | Piso 2, Clínica ilegal | Piso 3, Núcleo |
| --- | --- | --- |
| ![Sala del piso 1 con luz de sodio|240](../Media/Capturas/Piso1Mantenimiento.png) | ![Sala del piso 2 con fluorescente frío|240](../Media/Capturas/Piso2Clinica.png) | ![Sala del piso 3 con tinte carmesí|240](../Media/Capturas/Piso3Nucleo.png) |

## 3. Modularidad

- **Kit modular por piso sobre una rejilla común.** Los módulos miden 2 m de lado. Los 6 arquetipos comparten la geometría base de muros, suelos, puertas, marcos y techos. Cada piso cambia texturas, utilería y luces. Así el generador combina salas sin piezas a medida. Ver Generación procedural por pisos en [[03 - Sistemas]].
- **Reutilizables entre pisos.** Puertas y marcos, paneles de hackeo, cámaras de seguridad, rejillas, tuberías, cajas, estanterías, camillas, terminales y consolas. También la señalización de Vitalis, con variantes en inglés y en alemán sin traducir.
- **Utilería por tema.** Herramientas y calderas en el piso 1, equipo médico y archivadores en el piso 2, y viales y cableado en el piso 3.
- **Salas clave.** Son fijas pero cambian de posición. Usan el mismo kit con un decorado propio para que se reconozcan.
- **Objetos confiscados.** Son objetos cotidianos con una etiqueta de inventario común.

## 4. Iluminación

**Global.** La instalación está en estado de emergencia. La luz ambiental es casi nula y la escena se lee con focos aislados y con la linterna.

**Fuentes de luz:**

- Focos de emergencia con el tinte del piso, que es ámbar, blanco frío o carmesí.
- Pantallas verdes de la instalación y terminal teal de Lukas, como emisivos débiles.
- La linterna, única luz blanca, cálida neutra y la más fuerte. Proyecta sombras y concentra el haz al apuntar. Ver Linterna en [[02 - Mecánicas]].
- Luz roja de las cámaras hostiles en la fase 2 del jefe.

**Cambios durante el juego:**

- La sala clave final sin energía queda oscura hasta que Lukas restaura el interruptor.
- El latido no cambia la luz. La degradación solo afecta a la capa CRT.
- En la fase 3 del jefe, el núcleo abierto ilumina la sala.

**Rendimiento.** Cada sala tiene pocas luces dinámicas con sombra, y la linterna siempre la tiene. El resto son luces sin sombra o precalculadas en los módulos. Ver [[03 - Público y Plataforma]].

## 5. Efectos visuales

| Efecto | Uso en el juego |
|---|---|
| Partículas | Polvo en el haz de la linterna, chispas de cables y transformadores, y partículas del reciclaje |
| Clima | Ninguno. Todo el juego ocurre en interiores, salvo la introducción |
| Humo | Vapor de calderas en el piso 1 y niebla criogénica en la fase 2 del jefe |
| Fuego | Ninguno |
| Agua | Charcos y goteras en el piso 1, que coinciden con los pasos sobre agua |
| Magia / energía | No hay magia. Hay energía en la descarga del punzón, con destello blanco teal, en la distorsión morada de lo cósmico y en la corrupción CRT por degradación |
| Impactos | Impactos de bala en superficies y sangre oscura en la carne, sin desmembramiento |

## 6. Recursos requeridos

**Modelos:**

- Kit modular de 3 temas, uno por piso, con piezas de hasta 400 triángulos.
- Fila de cápsulas.
- Sala de The Mother.
- Casa, exterior, compuerta y elevador.
- Piso que no existe.
- Utilería por tema.
- Revólver de Kessler, punzón de descarga, cura, blíster, llaves y anillo.
- 12 objetos confiscados.
- Puertas, paneles de hackeo, interruptor de energía y cámaras.

**Texturas.** Un atlas de 2048 px por piso, `T_Env_F1`, `T_Env_F2` y `T_Env_F3`, más uno compartido, `T_Env_Shared`, cada uno con su mapa emisivo y filtro *point*. Señalización y logotipos de Vitalis y Kessler. Documentos.

**Materiales:**

- Base sin PBR complejo.
- Máscara de destello para los recogibles.
- Emisivos para pantallas, costuras y terminal.
- Carne asimilada.

**Efectos.** Los de la sección 5, más el posproceso global de reescalado, *dithering* y CRT de la vigilancia y de la degradación.

Todos los modelos de escenario, objetos y utilería se hacen en Blender.

> **Navegación:** [[00 - Índice]] · ← [[02 - Personajes y Animación]] · [[04 - Interfaz Visual]] →
