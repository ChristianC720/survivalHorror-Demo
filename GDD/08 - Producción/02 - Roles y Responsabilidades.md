# Roles y Responsabilidades

## Descripción de la sección

Define quién es responsable de cada área y evita que tareas importantes queden sin dueño.

## Equipo

El equipo tiene 5 integrantes ([[01 - Ficha del Proyecto]]). Las guías de `Desarrollo/` de cada área se indican entre paréntesis.

| Integrante | Rol principal | Responsabilidades |
|---|---|---|
| Christian Coronel Palma | Líder del proyecto, programación, narrativa y gestión | Responsable del proyecto y del GDD; programa los sistemas S-01..S-11 (guías 00–10 y 13); mantiene el backlog en Trello, decide los recortes ([[01 - Alcance]]) y aprueba los cambios ([[05 - Gestión de Cambios]]); revisa los *pull requests* de `develop` a `main` |
| Victor Gabriel Alvarado Chay | Diseño de niveles y narrativa | Arquetipos y temas de sala, tutorial, salas clave y sala de The Mother ([[02 - Niveles]]); documentos, bestiario y textos ([[04 - Diálogos y Narrativa]], [[05 - Textos del Juego]]); coordina la interfaz visual y escribe sus contenidos: textos del terminal, carteles y menús (D-137) |
| Bryan Iván Navarrete Canché | Arte 3D y animación | Personajes, enemigos, The Mother, kit modular por tema, objetos, materiales, luz por piso, rig, animación y render pixelado ([[01 - Dirección de Arte]], [[02 - Personajes y Animación]], [[03 - Entornos]]; guía 12) |
| Lian Daniel Sierra Vera | Audio y música | Leitmotivs, ambientes, latido, efectos y mezcla ([[05 - Audio y Música]]; S-11, guía 11) |
| Lenin Ramón López Córdova | QA y audio | Plan de pruebas, playtests y seguimiento de bugs ([[02 - Pruebas]], [[03 - Bugs y Seguimiento]]); apoyo en efectos de sonido |
| Todos | QA | Probar cada *pull request*, ejecutar los casos de [[02 - Pruebas]] y reportar en [[03 - Bugs y Seguimiento]]; en la semana de pruebas todo el equipo prueba |

## Áreas

| Área | Responsable | Apoyo |
|---|---|---|
| Diseño | Christian Coronel Palma (sistemas y GDD) | Victor Gabriel Alvarado Chay (niveles) |
| Programación | Christian Coronel Palma | — |
| Arte | Bryan Iván Navarrete Canché | — |
| Interfaz visual | Victor Gabriel Alvarado Chay (coordina; contenido y textos) | Bryan Iván Navarrete Canché (aspecto CRT, iconos y post-proceso); Christian Coronel Palma (lógica e integración) (D-137) |
| Animación | Bryan Iván Navarrete Canché | — |
| Audio | Lian Daniel Sierra Vera | Lenin Ramón López Córdova |
| Narrativa | Victor Gabriel Alvarado Chay | Christian Coronel Palma |
| QA | Lenin Ramón López Córdova | Todo el equipo |
| Gestión | Christian Coronel Palma | — |

**Riesgos de reparto:**

- **Una sola persona programa** todo el juego. Es el cuello de botella del proyecto: el resto del equipo entrega contenido listo para integrar (prefabs de sala, clips, audio con los nombres de [[02 - Motor y Herramientas]]) y los [[01 - Alcance#Criterios de recorte|criterios de recorte]] se aplican pensando en su carga.
- **Interfaz visual repartida** ([[04 - Interfaz Visual]]): terminal del brazo, menús y post-proceso CRT (guía 10). La coordina Victor Gabriel Alvarado Chay; Bryan Iván Navarrete Canché hace el aspecto visual y Christian Coronel Palma la lógica (D-137). Al repartirse entre tres personas, Victor valida la coherencia con [[01 - Dirección de Arte]] antes de cada *pull request*.

## Responsabilidad

Cada entregable importante debe tener una persona responsable de coordinarlo, aunque varias personas participen en su elaboración.

- Cada tarea del backlog tiene **un** responsable ([[04 - Backlog del Proyecto]]).
- Cada escena y cada prefab de sala tiene un dueño mientras se edita (evita conflictos, [[05 - Seguridad y Riesgos Técnicos]]).
- Quien cambia una regla del GDD la registra como decisión D-xx y avisa a los responsables de los sistemas afectados.


> **Navegación:** [[00 - Índice]] · ← [[01 - Alcance]] · [[03 - Roadmap]] →
