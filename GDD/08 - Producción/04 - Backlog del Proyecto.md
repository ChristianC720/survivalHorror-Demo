# Backlog del Proyecto

## Descripción de la sección

El backlog contiene el trabajo necesario para convertir el diseño en un videojuego funcional.

El backlog vivo se lleva en **Trello** (D-113): tablero **Por definir** (enlace), con una tarjeta por tarea, la etiqueta de su épica y el hito H1..H6 ([[03 - Roadmap]]). Esta nota define el formato y las épicas; el estado de cada tarea se consulta en el tablero y los pasos de cada tarea están en la guía de `Desarrollo/` indicada.

## Formato de tarea

### Tarea

**Descripción:** Qué se construye, con el M-xx / S-xx / D-xx que la define.

**Responsable:** Una persona ([[02 - Roles y Responsabilidades]]).

**Prioridad:** Alta / Media / Baja

**Dependencias:** Tareas que deben estar terminadas antes.

**Criterio de aceptación:** El criterio de [[03 - Sistemas]] o de [[04 - Criterios de Aceptación]] que la cierra.

**Estado:** Pendiente / En progreso / Bloqueada / Terminada (columnas del tablero).

## Backlog

Épicas en orden de prioridad. Cada línea es una tarjeta inicial con su responsable ([[02 - Roles y Responsabilidades]]); Alta = necesaria para el MVP (H3).

- [ ] **E-00 Proyecto** (guía 00) — Alta: crear proyecto Unity 6.6 URP, paquetes, carpetas, escenas base, ajustes de calidad y build de prueba. *Christian Coronel Palma.*
- [ ] **E-01 Núcleo** (guía 01) — Alta: `Bootstrap`, `GameStateMachine`, `GameEvents`, `SceneLoader`, pausa. *Christian Coronel Palma.*
- [ ] **E-02 Jugador** (guía 02) — Alta: Input System por contextos, movimiento (0.8 / 1.5 / 4 m/s), resistencia (S-04), salud (M-05, D-119). Media: mapa fijo de mando (D-120). *Christian Coronel Palma.*
- [ ] **E-03 Cámara** (guía 03) — Alta: OTS y apuntado (M-01). Media: vigilancia, señal escalonada, atención y bloqueo (M-02, M-03). *Christian Coronel Palma.*
- [ ] **E-04 Combate** (guía 04) — Alta: revólver, empujón, agarre con punzón (M-04, M-06, M-11). Media: remate (M-20). *Christian Coronel Palma.*
- [ ] **E-05 IA** (guía 05) — Alta: percepción y estados del Alto (S-02). Media: Rastrero, Boca, Mudo; respuesta a la atención. *Christian Coronel Palma.*
- [ ] **E-06 Inventario e interacción** (guía 06) — Alta: 6 casillas, recoger, usar, descartar, llaves, documentos (M-07, M-08). Media: linterna (M-12), hackeo y sellado (M-13). *Christian Coronel Palma.*
- [ ] **E-07 Generación** (guía 07) — Alta: grafo, validador, instanciado, NavMesh, presupuesto, activación por cercanía (S-06). Media: tutorial fijo, energía (D-72), showcase y completa. *Christian Coronel Palma.*
- [ ] **E-08 Ciclo y persistencia** (guía 08) — Alta: muerte, reinicio, reconstrucción, terminal, ranuras, autoguardado (S-07, S-08). Media: reciclaje (S-09), costo de salir. Baja: degradación y Asimilación (M-18), fallas (M-17). *Christian Coronel Palma.*
- [ ] **E-09 Jefe** (guía 09) — Media: 3 fases, cámaras hostiles, brazo recolector, decisión final (S-10). *Christian Coronel Palma.*
- [ ] **E-10 UI** (guía 10) — Media: terminal del brazo, menú en el terminal, pausa, confirmaciones, Opciones, subtítulos, accesibilidad, localización. *Programación: Christian Coronel Palma; diseño visual: por asignar.*
- [ ] **E-11 Audio** (guía 11) — Media: alertas, latidos, pasos, avisos, mezcla y *ducking*. Baja: música y leitmotivs. *Lian Daniel Sierra Vera.*
- [ ] **E-12 Arte y render** (guía 12) — Media: render ~540p, materiales *Point*, luz por piso, personajes y salas. Baja: cinemáticas. *Bryan Iván Navarrete Canché.*
- [ ] **E-13 Calidad y build** (guía 13) — Alta: pruebas EditMode del generador y del guardado. Media: perfilado, build final. *Christian Coronel Palma; playtests y bugs: Lenin Ramón López Córdova.*
- [ ] **E-14 Niveles** — Alta: arquetipos de sala y tema del piso 1 en *greybox* (prefabs para S-06). Media: temas de los pisos 2 y 3, tutorial y sala de The Mother ([[02 - Niveles]]). *Victor Gabriel Alvarado Chay y Bryan Iván Navarrete Canché.*
- [ ] **E-15 Textos** — Media: documentos de Emilia y de contexto, bestiario, carteles y líneas en inglés y español ([[05 - Textos del Juego]]). *Victor Gabriel Alvarado Chay.*

## Priorización

Prioriza aquello que desbloquea otras tareas o valida primero las partes más riesgosas del proyecto.

- Primero lo que **todo** usa: núcleo, jugador y eventos (E-01, E-02).
- Después lo más **riesgoso**: generación con NavMesh (E-07) y ciclo de muerte con guardado (E-08), antes del contenido.
- El arte entra sobre sistemas ya probados en *greybox*; nunca bloquea a programación. Como una sola persona programa, el arte, el audio y los niveles se producen en paralelo y se integran por prefabs con los nombres acordados ([[02 - Motor y Herramientas]]).
- Lo Deseable (M-17, M-18) solo se empieza con H4 cumplido.


> **Navegación:** [[00 - Índice]] · ← [[03 - Roadmap]] · [[05 - Gestión de Cambios]] →
