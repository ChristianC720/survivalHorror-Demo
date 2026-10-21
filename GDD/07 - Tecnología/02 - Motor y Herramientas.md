# Motor y Herramientas

## Descripción de la sección

Documenta las herramientas utilizadas para crear el videojuego y por qué fueron seleccionadas.

## Motor

**Motor:** Unity (D-107).

**Versión:** Unity 6.6 (*Supported release*). Todo el equipo usa la misma versión exacta, registrada en `ProjectSettings/ProjectVersion.txt`; cambiarla es una solicitud de cambio ([[05 - Gestión de Cambios]]).

**Renderizado:** Universal Render Pipeline (URP), ruta *Forward+* para muchas luces dinámicas sin luz horneada (las salas son procedurales). Resolución interna ~540p con escalado *Nearest-Neighbor* y *dithering* de cámara (D-81); la interfaz se dibuja a resolución nativa ([[03 - Accesibilidad]]).

**Física:** PhysX integrado (colisiones, *raycasts* de disparo, percepción y ragdoll, D-88).

**Sistema de entrada:** Input System con un asset `.inputactions` y un mapa de acción por contexto de [[04 - Controles]]; reasignación en tiempo de ejecución con `PerformInteractiveRebinding` (D-104).

## Lenguajes

| Lenguaje | Uso |
|---|---|
| C# | Toda la lógica del juego, herramientas de editor y pruebas |
| ShaderGraph / HLSL | Efectos CRT, glitch, destello de recogibles (D-84) y degradación (M-18) |
| JSON | Archivos de guardado y configuración (D-109) |
| Markdown | GDD (Obsidian) y guías de `Desarrollo/` |

## Herramientas

| Herramienta | Propósito | Versión |
|---|---|---|
| Unity Editor | Motor | 6.6 |
| Visual Studio 2022 / Rider | IDE de C# (`.vsconfig` del repositorio) | Última estable |
| Blender | Modelado, rig y animación (D-88) | 4.x |
| Mixamo | Locomoción base, retocada en Blender (D-88) | Web |
| Blockbench | Modelos low-poly de objetos y utilería | Última estable |
| FL Studio | Música y diseño sonoro (D-87) | La del equipo |
| Obsidian | GDD | Última estable |
| Git + GitHub + Git LFS | Control de versiones; binarios por LFS (`.gitattributes`) | Última estable |

## Dependencias

Solo paquetes oficiales de Unity (D-107):

| Paquete | Uso |
|---|---|
| Universal RP | Render ([[01 - Dirección de Arte]]) |
| Input System | Controles y reasignación |
| Cinemachine 3 | Cámara OTS, apuntado, vigilancia y cinemáticas |
| AI Navigation | `NavMeshSurface` construido en tiempo de ejecución por piso (S-06) |
| Timeline | Cinemáticas (D-39) |
| Animation Rigging | IK de apuntado, pies y manos (D-88) |
| Localization | Inglés y español (D-62) |
| TextMeshPro (uGUI) | Texto de la interfaz y del terminal |
| Test Framework | Pruebas EditMode y PlayMode ([[02 - Pruebas]]) |

Sin plugins de la Asset Store ni servicios en línea. Las fuentes tipográficas son OFL o propias ([[04 - Interfaz Visual]]).

## Convenciones del proyecto

- **Estructura de carpetas:** el proyecto Unity vive en la raíz del repositorio, junto a `GDD/` y `Desarrollo/`. Dentro de `Assets/_Project/`: `Art/`, `Audio/`, `Code/` (una subcarpeta por sistema: `Core`, `Player`, `Camera`, `Combat`, `AI`, `Inventory`, `Generation`, `Persistence`, `DeathCycle`, `Recycle`, `Boss`, `Audio`, `UI`), `Data/` (ScriptableObjects), `Prefabs/`, `Scenes/`, `Timelines/`, `Settings/`, `Tests/`. Paquetes de terceros fuera de `_Project`.
- **Nomenclatura:** clases y archivos C# en PascalCase e inglés (`FloorGenerator.cs`); prefijos de assets `SM_` (malla estática), `SK_` (malla con esqueleto), `M_` (material), `T_` (textura), `A_` (animación), `SFX_`, `MUS_`, `PF_` (prefab), `SO_` (ScriptableObject). Salas: `PF_Room_<Arquetipo>_<Piso>_<NN>`.
- **Control de versiones:** ramas `main` (entregas), `develop` (integración) y `feature/<sistema>-<tema>`; *pull request* a `develop` con revisión de otra persona. Una escena o prefab lo edita una sola persona a la vez (se avisa en el canal del equipo). Serialización *Force Text* y *Visible Meta Files*.
- **Formato de assets:** modelos `.fbx` desde Blender (escala 1 = 1 m, Y arriba al importar), texturas `.png` de potencia de 2 con filtro *Point* y sin mipmaps en texturas de mundo, audio `.wav` 48 kHz (Unity comprime al compilar).
- **Convenciones de código:** un `MonoBehaviour` por archivo; campos serializados `[SerializeField] private`; sin lógica en `Update` que pueda ir en eventos; comentarios solo para el porqué; cada valor de balance en un ScriptableObject con el D-xx que lo define en su `[Tooltip]`.


> **Navegación:** [[00 - Índice]] · ← [[01 - Arquitectura Técnica]] · [[03 - Requisitos Técnicos]] →
