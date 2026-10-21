# Roadmap

## Descripción de la sección

El roadmap organiza el desarrollo en etapas y entregables.

Plazo total: del **2026-10-21** a la entrega del **2026-11-30** (6 semanas, D-111). Las fechas son de fin de semana de trabajo (domingo). La guía de `Desarrollo/` de cada tarea se indica entre paréntesis.

## Etapas

### Concepto

**Objetivo:** Definir qué juego se hace y por qué.

**Entregables:** GDD 0.1.0–0.10.0 (Visión, Diseño, Historia y Mundo, Contenido, Arte y Audio, Experiencia). **Terminado** (2026-09-25 a 2026-10-16).

### Preproducción

**Objetivo:** Cerrar la base técnica y de producción y tener el proyecto Unity funcionando.

**Entregables (semana 1, 2026-10-21 a 2026-10-26):** GDD 0.11.0 (Tecnología, Producción, Calidad); guías de `Desarrollo/`; proyecto Unity 6.6 URP con paquetes, carpetas y escenas base (00); núcleo con `GameStateMachine` y `GameEvents` (01); jugador con movimiento, correr, agacharse y cámara OTS en una sala *greybox* (02, 03). Roles asignados.

### Producción

**Objetivo:** Construir el juego completo en orden de riesgo: primero el bucle, después el contenido.

**Entregables:**

- **Semana 2 (2026-10-27 a 2026-11-02):** combate, salud, resistencia (04); inventario, interacción, linterna y hackeo (06); el Alto con percepción y estados de alerta (05); importación de un personaje con Humanoid (12).
- **Semana 3 (2026-11-03 a 2026-11-09):** generación procedural con validador y NavMesh en tiempo de ejecución (07); muerte, reconstrucción, terminal y guardado por ranuras (08). **MVP.**
- **Semana 4 (2026-11-10 a 2026-11-16):** vigilancia y atención de The Mother (03); reciclaje y remate (08); los 4 tipos de enemigo (05); pisos 2 y 3; tutorial; audio de alertas y latidos (11); terminal del brazo (10); render pixelado y arte del piso 1 (12). **Vertical slice.**
- **Semana 5 (2026-11-17 a 2026-11-23):** The Mother y decisión final (09); introducción y cinemáticas; menús, Opciones, accesibilidad y localización (10); arte de los pisos 2 y 3; música. **Contenido completo y congelación de funciones el 2026-11-23.**

### Pruebas

**Objetivo:** Encontrar y corregir errores, balancear y alcanzar el rendimiento objetivo.

**Entregables (semana 6, 2026-11-24 a 2026-11-29):** pruebas de [[02 - Pruebas]] aprobadas; bugs críticos y altos cerrados ([[03 - Bugs y Seguimiento]]); balance de recursos y enemigos de la showcase; perfilado contra [[03 - Requisitos Técnicos]] (13). Solo se corrigen errores: no entran funciones nuevas.

### Publicación / Entrega

**Objetivo:** Entregar la demo.

**Entregables (2026-11-30):** build de Windows 64 bits en `main` con etiqueta `v1.0-demo`, GDD actualizado con la versión entregada y [[04 - Criterios de Aceptación]] marcados.

## Hitos

| Hito | Entregable | Criterio de finalización | Estado |
| ---- | ---------- | ------------------------ | ------ |
| H1 — Base técnica | GDD 0.11.0, guías, proyecto Unity, jugador y cámara | El jugador se mueve, corre, se agacha y mira en OTS en una sala *greybox*; el proyecto abre sin errores en los 5 equipos | Pendiente (2026-10-26) |
| H2 — Prototipo de combate | Combate, inventario, interacción y el Alto | En una sala de prueba se puede esconder, disparar, empujar, curar y morir contra un Alto que patrulla, sospecha, busca y caza | Pendiente (2026-11-02) |
| H3 — MVP | Piso 1 procedural y ciclo de muerte | Se cumple el MVP de [[01 - Alcance]]; el validador pasa 1 000 semillas | Pendiente (2026-11-09) |
| H4 — Vertical slice | Piso 1 con calidad final | Se cumple el vertical slice de [[01 - Alcance]] y una persona ajena al equipo completa el piso 1 sin ayuda | Pendiente (2026-11-16) |
| H5 — Contenido completo | 3 pisos, The Mother, finales, menús | Se puede jugar de Nueva partida a los créditos en la showcase; congelación de funciones | Pendiente (2026-11-23) |
| H6 — Entrega | Build final | Todos los criterios del proyecto de [[04 - Criterios de Aceptación]] marcados; sin bugs críticos ni altos abiertos | Pendiente (2026-11-30) |


> **Navegación:** [[00 - Índice]] · ← [[02 - Roles y Responsabilidades]] · [[04 - Backlog del Proyecto]] →
