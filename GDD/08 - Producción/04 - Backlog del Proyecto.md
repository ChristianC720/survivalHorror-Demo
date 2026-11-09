# Backlog del Proyecto

## 1. Descripción de la sección

El backlog contiene el trabajo necesario para convertir el diseño en un videojuego funcional.

El seguimiento se lleva en el Excel `CronogramaIdentityDissolution_Objetivos_v3.xlsx`, compartido en el OneDrive de la universidad. Cada actividad tiene su responsable, su objetivo y su hito, de H1 a la entrega ([[03 - Roadmap]]). Esta nota define el formato y las épicas. El estado de cada tarea se consulta en el cronograma, y sus pasos están en la guía de `Desarrollo/` indicada. Los valores numéricos de este documento son iniciales y están sujetos a balance.

**Cronograma:** [CronogramaIdentityDissolution_Objetivos_v3.xlsx](https://utcancunmx-my.sharepoint.com/:x:/r/personal/23393274_utcancun_edu_mx/_layouts/15/Doc.aspx?sourcedoc=%7B7E079720-7475-412B-B81D-BA1CC55BD030%7D&file=CronogramaIdentityDissolution_Objetivos_v3.xlsx&action=default&mobileredirect=true)

## 2. Formato de tarea

### 2.1 Tarea

**Descripción:** Qué se construye, con la mecánica, el sistema o la decisión que lo define.

**Responsable:** Una persona ([[02 - Roles y Responsabilidades]]).

**Prioridad:** Alta, Media o Baja.

**Dependencias:** Tareas que deben estar terminadas antes.

**Criterio de aceptación:** El criterio de [[03 - Sistemas]] o de [[04 - Criterios de Aceptación]] que la cierra.

**Estado:** Pendiente, En progreso, Bloqueada o Terminada.

## 3. Backlog

Las épicas van en orden de prioridad. Cada línea es una épica con su responsable. Alta significa necesaria para el MVP del hito H3. El estado sigue las fechas del [[03 - Roadmap]].

- [x] **Proyecto** (guía 00). Alta: proyecto Unity 6.6 URP, paquetes, carpetas, escenas base, ajustes de calidad y build de prueba. *Christian Coronel Palma.* Terminada.
- [x] **Núcleo** (guía 01). Alta: `Bootstrap`, `GameStateMachine`, `GameEvents`, `SceneLoader` y pausa. *Christian Coronel Palma.* Terminada.
- [x] **Jugador** (guía 02). Alta: Input System por contextos, movimiento a 0.8, 1.5 y 4 m/s, resistencia y salud. Media: mapa fijo de mando. *Christian Coronel Palma.* Terminada.
- [ ] **Cámara** (guía 03). Alta: cámara sobre el hombro y apuntado. Media: vigilancia, señal escalonada, atención y bloqueo de vigilancia. *Christian Coronel Palma.* En progreso.
- [ ] **Combate** (guía 04). Alta: revólver, empujón y agarre con punzón. Media: remate. *Christian Coronel Palma.* En progreso.
- [ ] **IA** (guía 05). Alta: percepción y estados del Alto. Media: Rastrero, Boca y Mudo, y respuesta a la atención de The Mother. *Christian Coronel Palma.* En progreso.
- [x] **Inventario e interacción** (guía 06). Alta: 6 casillas, recoger, usar, descartar, llaves y documentos. Media: linterna, hackeo y sellado. *Christian Coronel Palma.* Terminada.
- [ ] **Generación** (guía 07). Alta: grafo, validador, instanciado, NavMesh, presupuesto y activación por cercanía. Media: tutorial fijo, energía y configuraciones showcase y completa. *Christian Coronel Palma.* En progreso.
- [ ] **Ciclo y persistencia** (guía 08). Alta: muerte, reinicio, reconstrucción, terminal, ranuras y autoguardado. Media: reciclaje y costo de salir. Baja: degradación por muerte, final Asimilación y fallas de la máquina. *Christian Coronel Palma.* En progreso.
- [ ] **Jefe** (guía 09). Media: 3 fases, cámaras hostiles, brazo recolector y decisión final. *Christian Coronel Palma.* Pendiente.
- [ ] **Interfaz** (guía 10). Media: terminal del brazo, menú en el terminal, pausa, confirmaciones, `Opciones`, subtítulos, accesibilidad y localización. *Programación: Christian Coronel Palma. Diseño: Victor Gabriel Alvarado Chay.* En progreso.
- [ ] **Audio** (guía 11). Media: alertas, latidos, pasos, avisos, mezcla y *ducking*. Baja: música y leitmotivs. *Lian Daniel Sierra Vera.* En progreso.
- [ ] **Arte y render** (guía 12). Media: render a ~540p, materiales *Point*, luz por piso, personajes y salas. Baja: cinemáticas. *Bryan Iván Navarrete Canché.* En progreso.
- [ ] **Calidad y build** (guía 13). Alta: pruebas EditMode del generador y del guardado. Media: perfilado, builds intermedias numeradas 0.1, 0.2 y así en adelante, y build de entrega `v1.0-demo`. *Christian Coronel Palma. Playtests y bugs: Lenin Ramón López Córdova.* En progreso.
- [ ] **Niveles.** Alta: arquetipos de sala y tema del piso 1 en *greybox*, como prefabs para el generador. Media: temas de los pisos 2 y 3, tutorial y sala de The Mother ([[02 - Niveles]]). *Victor Gabriel Alvarado Chay y Bryan Iván Navarrete Canché.* En progreso.
- [ ] **Textos.** Media: documentos de Emilia y de contexto, bestiario, carteles y líneas en inglés y español ([[05 - Textos del Juego]]). *Victor Gabriel Alvarado Chay.* En progreso.

## 4. Priorización

Prioriza aquello que desbloquea otras tareas o valida primero las partes más riesgosas del proyecto.

- Primero va lo que todo usa, que es el núcleo, el jugador y los eventos.
- Después va lo más riesgoso, antes del contenido. Son la generación con NavMesh y el ciclo de muerte con guardado.
- El arte entra sobre sistemas ya probados en *greybox* y nunca bloquea a programación.
- Como una sola persona programa, el arte, el audio y los niveles se producen en paralelo. Se integran por prefabs con los nombres acordados en [[02 - Motor y Herramientas]].
- Lo Deseable, que son las fallas de la máquina y la degradación por muerte, solo se empieza con el hito H4 cumplido.

> **Navegación:** [[00 - Índice]] · ← [[03 - Roadmap]] · [[05 - Gestión de Cambios]] →
