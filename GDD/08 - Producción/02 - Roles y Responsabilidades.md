# Roles y Responsabilidades

## 1. Descripción de la sección

Define quién es responsable de cada área y evita que tareas importantes queden sin dueño.

## 2. Equipo

El equipo tiene 5 integrantes ([[01 - Ficha del Proyecto]]). Las actividades y sus fechas están en [[03 - Roadmap]]. Las guías de `Desarrollo/` de cada área van entre paréntesis. Los valores numéricos de este documento son iniciales y están sujetos a balance.

| Integrante | Rol principal | Responsabilidades |
|---|---|---|
| Christian Coronel Palma | Líder del proyecto, programación, narrativa y gestión | Responsable del proyecto y del GDD. Codirige el concepto y los pilares, y define el alcance inicial y la plataforma. Programa todos los sistemas del juego (guías 00 a 10 y 13). Produce la narrativa de documentos y eventos. Mantiene el seguimiento en el cronograma del proyecto, decide los recortes ([[01 - Alcance]]) y aprueba los cambios ([[05 - Gestión de Cambios]]). Revisa los *pull requests* de `develop` a `main` y hace la entrega |
| Victor Gabriel Alvarado Chay | Diseño de niveles, sistemas y narrativa | Codirige el concepto y los pilares. Diseña los sistemas y la economía de recursos por piso. Hace los arquetipos y temas de sala, el tutorial, las salas clave y la sala de The Mother ([[02 - Niveles]]). Escribe los documentos de Emilia, el bestiario, los textos, la introducción, las cinemáticas y los finales ([[04 - Diálogos y Narrativa]], [[05 - Textos del Juego]]). Coordina y diseña la interfaz visual, con los textos del terminal, los carteles y los menús |
| Bryan Iván Navarrete Canché | Arte 3D y animación | Personajes, enemigos, The Mother, kit modular por tema, objetos, materiales, luz por piso, rig, animación y render pixelado ([[01 - Dirección de Arte]], [[02 - Personajes y Animación]], [[03 - Entornos]], guía 12). Apoya los arquetipos de sala y los pisos 2 y 3. Hace el arte de la introducción, las cinemáticas y los finales |
| Lian Daniel Sierra Vera | Audio y música | Leitmotivs, ambientes, latido, efectos y mezcla ([[05 - Audio y Música]], guía 11). Hace las señales de alerta, los latidos y los pasos, y la música |
| Lenin Ramón López Córdova | QA y audio | Plan de pruebas, *playtests*, balance general de la demo y seguimiento de bugs ([[02 - Pruebas]], [[03 - Bugs y Seguimiento]]). Apoya la producción de audio y las señales de alerta, latidos y pasos |
| Todos | QA | Prueban cada *pull request*, ejecutan los casos de [[02 - Pruebas]] y reportan en [[03 - Bugs y Seguimiento]]. En la semana de pruebas todo el equipo prueba |

## 3. Áreas

| Área | Responsable | Apoyo |
|---|---|---|
| Dirección de proyecto | Christian Coronel Palma | Victor Gabriel Alvarado Chay, en el concepto y los pilares |
| Diseño | Christian Coronel Palma, en el GDD | Victor Gabriel Alvarado Chay, en los sistemas, la economía de recursos y los niveles |
| Programación | Christian Coronel Palma | Ninguno |
| Arte | Bryan Iván Navarrete Canché | Ninguno |
| Diseño de niveles | Victor Gabriel Alvarado Chay | Bryan Iván Navarrete Canché |
| Interfaz visual | Victor Gabriel Alvarado Chay, que coordina, diseña y escribe los contenidos | Bryan Iván Navarrete Canché, en el aspecto CRT, los iconos y el posproceso. Christian Coronel Palma, en la lógica y la integración |
| Animación | Bryan Iván Navarrete Canché | Ninguno |
| Audio | Lian Daniel Sierra Vera | Lenin Ramón López Córdova |
| Narrativa | Christian Coronel Palma, en los documentos y eventos | Victor Gabriel Alvarado Chay, que además es responsable de los documentos de Emilia, el bestiario, los textos, la introducción, las cinemáticas y los finales |
| QA | Lenin Ramón López Córdova | Todo el equipo |
| Gestión | Christian Coronel Palma | Ninguno |

**Riesgos de reparto:**

- **Una sola persona programa todo el juego.** Es el cuello de botella del proyecto. El resto del equipo entrega contenido listo para integrar, como prefabs de sala, clips y audio con los nombres de [[02 - Motor y Herramientas]].
- Los criterios de recorte de [[01 - Alcance]] se aplican pensando en la carga de programación.
- **La interfaz visual se reparte entre tres personas** ([[04 - Interfaz Visual]]). Cubre el terminal del brazo, los menús y el posproceso CRT (guía 10).
- Por ese reparto, Victor Gabriel Alvarado Chay valida la coherencia con [[01 - Dirección de Arte]] antes de cada *pull request*.

## 4. Responsabilidad

Cada entregable importante debe tener una persona responsable de coordinarlo, aunque varias personas participen en su elaboración.

- Cada tarea del backlog tiene un solo responsable ([[04 - Backlog del Proyecto]]).
- Cada escena y cada prefab de sala tiene un dueño mientras se edita. Así se evitan conflictos ([[05 - Seguridad y Riesgos Técnicos]]).
- Quien cambia una regla del GDD la registra como decisión y avisa a los responsables de los sistemas afectados ([[05 - Gestión de Cambios]]).

> **Navegación:** [[00 - Índice]] · ← [[01 - Alcance]] · [[03 - Roadmap]] →
