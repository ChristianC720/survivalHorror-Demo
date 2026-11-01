# Alcance

## Descripción de la sección

Define qué se va a construir y, sobre todo, qué queda fuera del proyecto.

**Versión objetivo:** demo jugable para PC, entrega el **2026-11-20** (D-111, D-134). Plan semanal en [[03 - Roadmap]].

## Alcance de la versión objetivo

### Incluido

- Configuración **showcase** completa (3 pisos de 6–8 salas procedurales + 2 secundarias + 1 final, ~30 min; D-34) como objetivo principal. La configuración completa es un ajuste de parámetros de S-06 y se activa si el balance llega a tiempo.
- Todas las mecánicas de prioridad Demo: M-01..M-16, M-19 y M-20 ([[02 - Mecánicas]]).
- Los 11 sistemas S-01..S-11 ([[03 - Sistemas]]); S-08 sin degradación si se recorta M-18.
- 4 tipos de enemigo (el Alto, la Boca, el Rastrero, el Mudo) y The Mother con 3 fases.
- 6 arquetipos de sala con el tema de cada piso, salas clave secundarias, salas finales y la sala de The Mother ([[02 - Niveles]]).
- Tutorial de 4 salas con carteles (D-70, D-89) e introducción jugable (D-53, D-63).
- Los 6 documentos de Emilia, la decisión final y las 3 cinemáticas de victoria (Relevo básico, Relevo completo, Ambiguo).
- Terminal del brazo, menús, 3 ranuras, Opciones y las opciones de [[03 - Accesibilidad]].
- Modo Supervivencia y Modo Historia (D-103).
- Textos en inglés y español (D-62).
- Mando tipo Xbox con mapa básico fijo (D-120).

### No incluido

- Reasignación del mando (D-120).
- Todo lo marcado Post-demo en [[02 - Mecánicas]]: saqueo de cadáveres, pasarelas entre salas, techos rotos.
- Finales "Salida" y "Encuentro con Emilia" (D-51).
- Actuación de voz (D-52), otras plataformas, logros, estadísticas en línea.
- NPC vivos (D-77).

## MVP

El **Producto Mínimo Viable** debe contener únicamente lo necesario para demostrar que la experiencia principal funciona.

**MVP del proyecto (hito H3, 2026-11-08):** un **piso 1 generado proceduralmente** en *greybox* donde Lukas se mueve en OTS, encuentra recursos en el inventario de 6 casillas, evita o combate a un enemigo con IA (el Alto) con revólver y empujón, consigue la llave obligatoria, llega a la sala clave final y, si muere, se reconstruye en la cápsula con un piso distinto y el terminal conservado. Prueba el bucle de [[01 - Jugabilidad]] sin arte final.

## Vertical Slice

**Vertical slice (hito H4, 2026-11-11):** el **piso 1 completo con calidad final**: tutorial de 4 salas con carteles, arte y luz ámbar de mantenimiento, el Alto y el Rastrero animados, vista de vigilancia con señal escalonada, sigilo con linterna, hackeo, reciclaje y remate, audio de alertas y latidos, terminal del brazo y autoguardado en la sala final. Demuestra el tono, el render ~540p y los tres pilares.

## Criterios de recorte

Si el tiempo o los recursos son insuficientes, se recorta en este orden (D-111). Cada recorte se registra en [[05 - Gestión de Cambios]].

1. **M-17 Fallas de la máquina** (Deseable): la fase 1 de The Mother usa la longitud original.
2. **M-18 Degradación y final Asimilación** (Deseable): sin contador de 5 muertes ni capas de degradación; el estado Final: Asimilación no existe.
3. **Configuración completa:** solo se entrega la showcase.
4. **Documentos de contexto y objetos confiscados:** de 12 + 12 a 4 + 4 (uno por tipo y piso).
5. **Cinemáticas:** las narrativas pasan a imágenes fijas con texto y sonido sobre Timeline; se mantienen la corta de reconstrucción y la transición de piso.
6. **Modo Historia:** se mantiene la selección, pero solo cambia el daño recibido.
7. **Español:** se entrega solo en inglés (idioma principal, D-62).
8. **Introducción:** se reduce a la compuerta y el elevador.
9. **Enemigos de los pisos 2 y 3:** la Boca y el Mudo reutilizan la IA del Alto o del Rastrero con otro modelo y otros valores.
10. **Fase 2 de The Mother:** sin cámaras hostiles.

**Nunca se recorta:** el ciclo muerte → reconstrucción → piso nuevo, la generación válida, la vista de vigilancia con costo, el inventario en tiempo real, The Mother (salvo lo que permite el recorte 10), los 6 documentos de Emilia y la decisión final, ni las opciones de accesibilidad de brillo y subtítulos.


> **Navegación:** [[00 - Índice]] · ← [[05 - Seguridad y Riesgos Técnicos]] · [[02 - Roles y Responsabilidades]] →
