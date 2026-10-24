# Roadmap

## Descripción de la sección

El roadmap organiza el desarrollo en etapas y entregables.

El GDD se escribió del **2026-09-26** al **2026-10-22**. El desarrollo va del **2026-10-23** a la entrega del **2026-11-20** (4 semanas, D-134). La guía de `Desarrollo/` de cada tarea se indica entre paréntesis.

## Etapas

### Concepto

**Objetivo:** Definir qué juego se hace y por qué.

**Entregables:** GDD 0.1.0–0.10.0 (Visión, Diseño, Historia y Mundo, Contenido, Arte y Audio, Experiencia). **Terminado** (2026-09-26 a 2026-10-16).

### Preproducción

**Objetivo:** Cerrar la base técnica y de producción antes de abrir Unity.

**Entregables:** GDD 0.11.0–0.13.0 (Tecnología, Producción, Calidad, Textos del Juego y revisión de coherencia); guías de `Desarrollo/`; roles asignados. **Terminado** (2026-10-17 a 2026-10-22).

### Producción

**Objetivo:** Construir el juego completo en orden de riesgo: primero el bucle, después el contenido.

**Entregables:**

- **Semana 1 (2026-10-23 a 2026-11-01):** proyecto Unity 6.6 URP con paquetes, carpetas y escenas base (00); núcleo con `GameStateMachine` y `GameEvents` (01); jugador con movimiento, correr, agacharse y cámara OTS en una sala *greybox* (02, 03). **Base técnica el 2026-10-26.** Después: combate, salud, resistencia (04); inventario, interacción, linterna y hackeo (06); el Alto con percepción y estados de alerta (05); importación de un personaje con Humanoid (12). **Prototipo de combate.**
- **Semana 2 (2026-11-02 a 2026-11-08):** generación procedural con validador y NavMesh en tiempo de ejecución (07); muerte, reconstrucción, terminal y guardado por ranuras (08). **MVP.**
- **Semana 3 (2026-11-09 a 2026-11-15):** hasta el 2026-11-11, vigilancia y atención de The Mother (03), reciclaje y remate (08), tutorial, audio de alertas y latidos (11), terminal del brazo (10), render pixelado y arte del piso 1 (12): **vertical slice**. Del 2026-11-12 al 2026-11-15, los 4 tipos de enemigo (05); pisos 2 y 3; The Mother y decisión final (09); introducción y cinemáticas; menús, Opciones, accesibilidad y localización (10); arte de los pisos 2 y 3; música. **Contenido completo y congelación de funciones el 2026-11-15.**

### Pruebas

**Objetivo:** Encontrar y corregir errores, balancear y alcanzar el rendimiento objetivo.

**Entregables (semana 4, 2026-11-16 a 2026-11-19):** pruebas de [[02 - Pruebas]] aprobadas; bugs críticos y altos cerrados ([[03 - Bugs y Seguimiento]]); balance de recursos y enemigos de la showcase; perfilado contra [[03 - Requisitos Técnicos]] (13). Solo se corrigen errores: no entran funciones nuevas.

### Publicación / Entrega

**Objetivo:** Entregar la demo.

**Entregables (2026-11-20):** build de Windows 64 bits en `main` con etiqueta `v1.0-demo`, GDD actualizado con la versión entregada y [[04 - Criterios de Aceptación]] marcados.

## Hitos

| Hito | Entregable | Criterio de finalización | Estado |
| ---- | ---------- | ------------------------ | ------ |
| H1 — Base técnica | Proyecto Unity, núcleo, jugador y cámara | El jugador se mueve, corre, se agacha y mira en OTS en una sala *greybox*; el proyecto abre sin errores en los 5 equipos | Pendiente (2026-10-26) |
| H2 — Prototipo de combate | Combate, inventario, interacción y el Alto | En una sala de prueba se puede esconder, disparar, empujar, curar y morir contra un Alto que patrulla, sospecha, busca y caza | Pendiente (2026-11-01) |
| H3 — MVP | Piso 1 procedural y ciclo de muerte | Se cumple el MVP de [[01 - Alcance]]; el validador pasa 1 000 semillas | Pendiente (2026-11-08) |
| H4 — Vertical slice | Piso 1 con calidad final | Se cumple el vertical slice de [[01 - Alcance]] y una persona ajena al equipo completa el piso 1 sin ayuda | Pendiente (2026-11-11) |
| H5 — Contenido completo | 3 pisos, The Mother, finales, menús | Se puede jugar de Nueva partida a los créditos en la showcase; congelación de funciones | Pendiente (2026-11-15) |
| H6 — Entrega | Build final | Todos los criterios del proyecto de [[04 - Criterios de Aceptación]] marcados; sin bugs críticos ni altos abiertos | Pendiente (2026-11-20) |


> **Navegación:** [[00 - Índice]] · ← [[02 - Roles y Responsabilidades]] · [[04 - Backlog del Proyecto]] →
