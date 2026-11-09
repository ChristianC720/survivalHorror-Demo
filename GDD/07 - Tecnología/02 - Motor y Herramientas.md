# Motor y Herramientas

## 1. Descripción de la sección

Documenta las herramientas utilizadas para crear el videojuego y por qué fueron seleccionadas.

## 2. Motor

**Motor:** Unity.

**Versión:** Unity 6.6, `6000.6.8f1`, *Supported release* y no LTS. Todo el equipo usa la misma versión exacta, registrada en `ProjectSettings/ProjectVersion.txt`. Cambiarla es una solicitud de cambio ([[05 - Gestión de Cambios]]).

**Renderizado:** Universal Render Pipeline (URP) 17 con la ruta *Forward+*. Admite muchas luces dinámicas sin luz horneada, porque las salas son procedurales. La resolución interna es de ~540p, con escalado *Nearest-Neighbor* y *dithering* de cámara. La interfaz se dibuja a resolución nativa ([[03 - Accesibilidad]]).

**Física:** PhysX integrado, para colisiones, *raycasts* de disparo, percepción y ragdoll. El jugador usa `CharacterController`, sin Rigidbody.

**Sistema de entrada:** Input System 1.20.1, con un asset `.inputactions` y un mapa de acción por contexto de [[04 - Controles]]. La reasignación en tiempo de ejecución usa `PerformInteractiveRebinding`.

## 3. Lenguajes

| Lenguaje | Uso |
|---|---|
| C# | Toda la lógica del juego, las herramientas de editor y las pruebas |
| ShaderGraph / HLSL | Efectos CRT, glitch, destello de recogibles y degradación |
| JSON | Archivos de guardado y configuración |
| Markdown | GDD en Obsidian y guías de `Desarrollo/` |

## 4. Herramientas

| Herramienta | Propósito | Versión |
|---|---|---|
| Unity Editor | Motor | 6000.6.8f1 |
| Visual Studio 2022 o Rider | IDE de C#, con el `.vsconfig` del repositorio | Última estable |
| Blender | Modelado, rig y animación, incluidos los modelos low-poly de objetos y utilería | 5.x |
| REAPER y FL Studio | Música y diseño sonoro | La del equipo |
| Obsidian | GDD | Última estable |
| Git, GitHub y Git LFS | Control de versiones. Los binarios van por LFS, según `.gitattributes` | Última estable |

## 5. Dependencias

El proyecto usa solo paquetes oficiales de Unity.

| Paquete | Uso |
|---|---|
| Universal RP 17 | Render ([[01 - Dirección de Arte]]) |
| Input System 1.20.1 | Controles y reasignación |
| Cinemachine 6.6.0 | Cámara sobre el hombro, apuntado, vigilancia y cinemáticas |
| AI Navigation | `NavMeshSurface` construido en tiempo de ejecución por piso |
| Timeline | Cinemáticas |
| Animation Rigging | IK de apuntado, pies y manos |
| Localization | Inglés y español |
| TextMeshPro (uGUI) | Texto de la interfaz y del terminal |
| Test Framework | Pruebas EditMode y PlayMode ([[02 - Pruebas]]) |

No hay plugins de la Asset Store ni servicios en línea. Las fuentes tipográficas son OFL o propias ([[04 - Interfaz Visual]]).

## 6. Convenciones del proyecto

- **Estructura de carpetas.** El proyecto Unity vive en la raíz del repositorio, junto a `GDD/` y `Desarrollo/`. Los paquetes de terceros quedan fuera de `_Project`. Los assets de URP están en `Assets/Settings/`, con `PC_RPAsset`, `PC_Renderer` y los perfiles de volumen.

| Carpeta de `Assets/_Project/` | Contenido |
|---|---|
| `Audio/` | `Mixer/` con `Heartbeat.mixer`, `Music/` y `SFX/` |
| `Input/` | `Heartbeat.inputactions` y los ajustes de Input System |
| `Localization/` | `Locales/` en inglés y español, `Source/` con las tablas `.tsv` y `Tables/` |
| `Materials/` | Materiales de entorno por piso, en `F1/`, `F2/`, `F3/` y `Shared/` |
| `Models/` | Modelos `.fbx` de Blender, en `Characters/`, `Enemies/`, `Env/`, `Props/`, `Statics/` y `ModelsNew/` |
| `Prefabs/` | `Characters/`, `Items/`, `Kit/`, `Level/`, `PH/`, `Props/`, `Rooms/` y `UI/`. Kit, utilería y salas van por piso |
| `Render/` y `Resources/` | Shaders del render pixelado, del contorno de interacción y de las luces de estado |
| `Scenes/` | Las 5 escenas del juego, y `Tests/` con una escena de prueba por sistema |
| `ScriptableObjects/` | `Audio/`, `Balance/`, `Docs/`, `Enemies/`, `Floors/`, `Items/` y `Rooms/` |
| `Scripts/` | Una subcarpeta por sistema: `AI`, `Audio`, `Boss`, `Camera`, `Combat`, `Core`, `Cycle`, `Debug`, `Editor`, `Generation`, `Input`, `Interaction`, `Inventory`, `Level`, `Narrative`, `Player`, `Recycle`, `Render`, `Save`, `Tutorial` y `UI` |
| `Tests/` | Pruebas `EditMode/` y `PlayMode/` |
| `Timelines/` y `UI/` | Cinemáticas, fuentes y recursos de la interfaz |

- **Nomenclatura.** Clases y archivos C# van en PascalCase y en inglés, como `FloorBuilder.cs`. Las salas procedurales siguen el patrón `ROOM_<Piso>_<Arquetipo>_<Variante>`, como `ROOM_F1_Storage_A`. Las salas clave siguen `KEY_<Piso>_<Función>`, como `KEY_F1_Final`.

| Prefijo | Asset | Ejemplo |
|---|---|---|
| `ROOM_` | Prefab de sala procedural | `ROOM_F1_Corridor_A` |
| `KEY_` | Prefab de sala clave | `KEY_F1_CapsuleRow` |
| `KIT_` | Pieza del kit modular | `KIT_F1_Wall_2m` |
| `RD_` | Definición de sala | `RD_KEY_F1_Final` |
| `FC_` | Configuración de piso | `FC_F1_Full`, `FC_F1_Showcase` |
| `ED_` | Definición de enemigo | `ED_Alto` |
| `*Balance` | Valores de balance de un sistema | `CombatBalance.asset` |
| `DOC_` | Documento | `DOC_EMILIA_01` |
| `BEST_` | Entrada del bestiario | `BEST_ALTO` |
| `CONF_` | Objeto confiscado | `CONF_WALLET` |
| `CUE_` | Señal de audio | `CUE_Alert_Hunt` |
| `ITEM_` | Objeto recogible | `ITEM_Heal` |
| `INT_` | Elemento interactuable | `INT_PowerSwitch` |
| `PROP_` | Utilería | `PROP_ArmTerminal` |
| `PH_` | Utilería provisional | `PH_F1_Boiler` |
| `CH_` | Modelo `.fbx` | `CH_Lukas_RH07` |
| `AC_` | Controlador de animación | `AC_Lukas` |
| `M_` | Material de modelo | `M_Alto` |
| `MAT_` | Material de entorno | `MAT_F1_Concrete` |
| `T_` | Textura | `T_Env_F1` |
| `SFX_` | Efecto de sonido | `SFX_Alert_Hunt` |
| `MUS_` | Música | `MUS_Emilia` |
| `SP_` | Punto de aparición | `SP_Enemy` |
| `Enemy_` | Prefab de enemigo | `Enemy_Alto` |
| `Door_` y `Light_` | Puerta y luminaria por piso | `Door_F1`, `Light_F1_Sodium` |
| `KitSet_` | Conjunto del kit por piso | `KitSet_F1` |

- **Control de versiones.** Hay tres tipos de rama: `main` para entregas, `develop` para integración y `feature/<sistema>-<tema>`.
    - Cada *pull request* va a `develop`, y otra persona lo prueba antes de fusionar. Es la definición de terminado de [[01 - Criterios de Calidad]].
    - Una escena o un prefab lo edita una sola persona a la vez, y se avisa en el canal del equipo.
    - La serialización usa *Force Text* y *Visible Meta Files*.
- **Formato de assets.** Los modelos salen de Blender en `.fbx`, con escala 1 = 1 m y Y arriba al importar.
    - Las texturas son `.png` de potencia de 2, con filtro *Point* y sin mipmaps en las texturas de mundo.
    - El audio es `.wav` a 48 kHz, y Unity lo comprime al compilar.
- **Convenciones de código.** Hay un `MonoBehaviour` por archivo, y los campos serializados son `[SerializeField] private`.
    - No va lógica en `Update` si puede ir en eventos.
    - Los comentarios explican solo el porqué.
    - Cada valor de balance vive en un ScriptableObject, con la decisión que lo define en su `[Tooltip]`.

> **Navegación:** [[00 - Índice]] · ← [[01 - Arquitectura Técnica]] · [[03 - Requisitos Técnicos]] →
