# Roadmap

## 1. Descripción de la sección

El roadmap organiza el desarrollo en etapas y entregables.

Las actividades, sus responsables y sus fechas salen del cronograma de entrega. El día 1 es el 24 de septiembre de 2026, y la entrega es el 13 de noviembre de 2026. El 12 de noviembre cierra el contenido y las pruebas, y deja el 13 libre para la build y la entrega. La guía de `Desarrollo/` de cada tarea va entre paréntesis. Los valores numéricos de este documento son iniciales y están sujetos a balance.

## 2. Etapas

### 2.1 Concepto

**Objetivo:** Definir qué juego se hace y por qué.

**Entregables:** GDD 0.1.0 a 0.10.0, con Visión, Diseño, Historia y Mundo, Contenido, Arte y Audio y Experiencia. Terminado.

| Actividad | Responsable | Inicio | Fin |
|---|---|---|---|
| Definición del concepto y los pilares de diseño | Christian Coronel Palma y Victor Gabriel Alvarado Chay | 24 sep | 7 oct |
| Definición del alcance inicial y la plataforma objetivo | Christian Coronel Palma | 8 oct | 10 oct |
| Diseño de los sistemas y la economía de recursos por piso | Victor Gabriel Alvarado Chay | 9 oct | 28 oct |

### 2.2 Preproducción

**Objetivo:** Cerrar la base técnica y de producción antes de abrir Unity.

**Entregables:** GDD 0.11.0 a 0.13.0, con Tecnología, Producción, Calidad, Textos del Juego y la revisión de coherencia. También las guías de `Desarrollo/` y los roles asignados. Terminado.

| Actividad | Responsable | Inicio | Fin |
|---|---|---|---|
| Preproducción: tecnología, producción, calidad y textos del GDD | Christian Coronel Palma | 16 oct | 21 oct |

### 2.3 Producción

**Objetivo:** Construir el juego completo en orden de riesgo, primero el bucle y después el contenido.

**Entregables:** Los hitos H1 a H5 de la sección 3, con las actividades de esta tabla.

La rama de prototipo `feature/heartbeatPrueba` es desechable y no se fusiona. Partes de su código se reutilizan, adaptadas a las convenciones de [[02 - Motor y Herramientas]], para cumplir H1 y H2 en sus fechas.

| Actividad | Responsable | Inicio | Fin | Hito |
|---|---|---|---|---|
| Producción de arte | Bryan Iván Navarrete Canché | 30 sep | 31 oct | Ninguno |
| Producción de audio | Lian Daniel Sierra Vera, con apoyo de Lenin Ramón López Córdova | 30 sep | 31 oct | Ninguno |
| Producción de narrativa, documentos y eventos | Christian Coronel Palma, con apoyo de Victor Gabriel Alvarado Chay | 30 sep | 31 oct | Ninguno |
| Base técnica: proyecto Unity 6.6 URP, núcleo y jugador (guías 00, 01 y 02) | Christian Coronel Palma | 22 oct | 25 oct | H1 |
| Cámara sobre el hombro y apuntado (guía 03) | Christian Coronel Palma | 22 oct | 25 oct | H1 |
| Inventario de 6 casillas, interacción, linterna y hackeo (guía 06) | Christian Coronel Palma | 26 oct | 31 oct | H2 |
| Combate, salud y resistencia: revólver, empujón y punzón (guía 04) | Christian Coronel Palma | 26 oct | 31 oct | H2 |
| El Alto con percepción y estados de alerta (guía 05) | Christian Coronel Palma | 26 oct | 31 oct | H2 |
| Generación procedural con validador y NavMesh en tiempo de ejecución (guía 07) | Christian Coronel Palma | 1 nov | 7 nov | H3 |
| Muerte, reconstrucción, terminal y guardado por ranuras (guía 08) | Christian Coronel Palma | 1 nov | 7 nov | H3 |
| Arquetipos de sala y tema del piso 1 en *greybox* para el generador | Victor Gabriel Alvarado Chay, con apoyo de Bryan Iván Navarrete Canché | 1 nov | 7 nov | Ninguno |
| Tutorial integrado en las primeras salas del piso 1 | Victor Gabriel Alvarado Chay | 8 nov | 10 nov | H4 |
| Render pixelado a ~540p y arte del piso 1 (guía 12) | Bryan Iván Navarrete Canché | 8 nov | 10 nov | H4 |
| Vista de vigilancia y atención de The Mother (guía 03) | Christian Coronel Palma | 8 nov | 10 nov | H4 |
| Terminal del brazo, diseño | Victor Gabriel Alvarado Chay | 8 nov | 10 nov | H4 |
| Terminal del brazo, programación (guía 10) | Christian Coronel Palma | 8 nov | 10 nov | H4 |
| Reciclaje y remate de enemigos (guía 08) | Christian Coronel Palma | 8 nov | 10 nov | H4 |
| Señales de alerta, latidos y pasos (guía 11) | Lian Daniel Sierra Vera, con apoyo de Lenin Ramón López Córdova | 8 nov | 10 nov | Ninguno |
| Los 4 tipos de enemigo: Alto, Rastrero, Boca y Mudo (guía 05) | Christian Coronel Palma | 11 nov | 12 nov | H5 |
| Pisos 2 y 3: temas de sala y salas clave | Victor Gabriel Alvarado Chay, con apoyo de Bryan Iván Navarrete Canché | 11 nov | 12 nov | H5 |
| Documentos de Emilia, bestiario y textos del juego | Victor Gabriel Alvarado Chay | 11 nov | 12 nov | H5 |
| Arte de los pisos 2 y 3 | Bryan Iván Navarrete Canché | 11 nov | 12 nov | H5 |
| Menús, opciones, accesibilidad y localización, diseño | Victor Gabriel Alvarado Chay | 11 nov | 12 nov | Ninguno |
| Menús, opciones, accesibilidad y localización, programación (guía 10) | Christian Coronel Palma | 11 nov | 12 nov | Ninguno |
| The Mother: 3 fases, cámaras hostiles y decisión final (guía 09) | Christian Coronel Palma | 11 nov | 12 nov | H5 |
| Sala de The Mother | Victor Gabriel Alvarado Chay | 11 nov | 12 nov | H5 |
| Introducción, cinemáticas y finales, arte | Bryan Iván Navarrete Canché | 11 nov | 12 nov | H5 |
| Introducción, cinemáticas y finales, narrativa | Victor Gabriel Alvarado Chay | 11 nov | 12 nov | H5 |
| Música y leitmotivs (guía 11) | Lian Daniel Sierra Vera | 11 nov | 12 nov | H5 |

### 2.4 Pruebas

**Objetivo:** Encontrar y corregir errores, balancear y alcanzar el rendimiento objetivo.

**Entregables:** Pruebas de juego y balance general de la demo, a cargo de Lenin Ramón López Córdova, del 9 al 12 de noviembre de 2026. Incluyen estos resultados.

- Pruebas de [[02 - Pruebas]] aprobadas.
- Bugs críticos y altos cerrados ([[03 - Bugs y Seguimiento]]).
- Balance de recursos y enemigos de la showcase.
- Perfilado contra [[03 - Requisitos Técnicos]] (guía 13).

En esta etapa solo se corrigen errores y no entran funciones nuevas.

### 2.5 Publicación / Entrega

**Objetivo:** Entregar la demo.

**Entregables:** Entrega de la demo jugable el 13 de noviembre de 2026, a cargo de Christian Coronel Palma. Incluye estos elementos.

- Build de Windows de 64 bits en `main` con etiqueta `v1.0-demo`.
- GDD actualizado con la versión entregada.
- [[04 - Criterios de Aceptación]] marcados.

Las builds intermedias se numeran 0.1, 0.2 y así en adelante. Solo la build de entrega lleva la etiqueta `v1.0-demo`.

## 3. Hitos

| Hito | Entregable | Criterio de finalización | Estado |
|---|---|---|---|
| H1 Base técnica, del 22 al 25 oct | Proyecto Unity, núcleo, jugador y cámara | El jugador se mueve, corre, se agacha y mira con la cámara sobre el hombro en una sala *greybox*. El proyecto abre sin errores en los 5 equipos | Cumplido |
| H2 Prototipo de combate, del 26 al 31 oct | Combate, inventario, interacción y el Alto | En una sala de prueba se puede esconder, disparar, empujar, curar y morir contra un Alto que patrulla, sospecha, busca y caza | Cumplido |
| H3 MVP, del 1 al 7 nov | Piso 1 procedural y ciclo de muerte | Se cumple el MVP de [[01 - Alcance]] y el validador pasa 1 000 semillas | Cumplido |
| H4 Vertical slice, del 8 al 10 nov | Piso 1 con calidad final | Se cumple el vertical slice de [[01 - Alcance]], y una persona ajena al equipo completa el piso 1 sin ayuda | En curso |
| H5 Contenido completo, del 11 al 12 nov | 3 pisos, The Mother, finales y menús | Se puede jugar de `Nueva partida` a los créditos en la showcase. Congelación de funciones | Pendiente |
| Entrega, 13 nov | Build final | Todos los criterios del proyecto de [[04 - Criterios de Aceptación]] marcados, sin bugs críticos ni altos abiertos | Pendiente |

> **Navegación:** [[00 - Índice]] · ← [[02 - Roles y Responsabilidades]] · [[04 - Backlog del Proyecto]] →
