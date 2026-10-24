# 00 — Configuración del Proyecto

**Objetivo:** que los 5 integrantes abran el mismo proyecto Unity, con los mismos paquetes y ajustes, desde el repositorio.

**Responsable:** Christian Coronel Palma.

**Referencias:** `GDD/07 - Tecnología/02 - Motor y Herramientas.md` (D-107), `GDD/07 - Tecnología/03 - Requisitos Técnicos.md` (D-112).

## Prerrequisitos (cada integrante)

1. Unity Hub con **Unity 6.6 (`6000.6.6f1`)** (la misma versión exacta para todos) y el módulo *Windows Build Support (IL2CPP)*.
2. Visual Studio 2022 con la carga "Desarrollo de juegos con Unity" (el `.vsconfig` del repositorio la instala) o Rider.
3. Git y **Git LFS**: ejecutar `git lfs install` una vez por equipo. El `.gitattributes` del repositorio ya envía modelos, imágenes y audio a LFS.

## Pasos

### 1. Crear el proyecto (una sola persona)

1. En Unity Hub: *New project* → plantilla **Universal 3D** → nombre `IdentityDissolution` → ubicación temporal.
2. Mueve el contenido del proyecto creado (`Assets/`, `Packages/`, `ProjectSettings/`) a la **raíz del repositorio**, junto a `GDD/` y `Desarrollo/`. El `.gitignore` de la raíz ya ignora `Library/`, `Temp/`, `Logs/`, `UserSettings/` y `Build/`.
3. Abre el proyecto desde la raíz del repositorio con Unity Hub (*Add project from disk*).
4. Borra el contenido de ejemplo de la plantilla (`Assets/TutorialInfo`, escena de ejemplo, `Readme.asset`).

### 2. Ajustes del editor

*Edit → Project Settings*:

- **Editor:** *Asset Serialization* = **Force Text**; *Version Control* = **Visible Meta Files**.
- **Player:** *Company Name* y *Product Name* = `Identity Dissolution`; *Active Input Handling* = **Input System Package (New)**; *Api Compatibility Level* = .NET Standard 2.1; *Scripting Backend* = IL2CPP (solo para la build final; Mono durante el desarrollo es más rápido).
- **Quality:** deja dos niveles, `Low` y `High`, cada uno con su asset de URP (paso 4).
- **Time:** *Fixed Timestep* 0.02 (por defecto).
- **Tags and Layers:** crea las capas `Player`, `Enemy`, `Interactable`, `Corpse`, `Occluder` (bloquea la visión enemiga y del disparo), `CameraCollision`, `Room`.

### 3. Paquetes

*Window → Package Manager → Unity Registry*, instala:

| Paquete | Para qué |
|---|---|
| Input System | Controles por contexto y reasignación (guía 02) |
| Cinemachine | Cámara OTS, vigilancia y cinemáticas (guía 03) |
| AI Navigation | NavMesh en tiempo de ejecución (guías 05 y 07) |
| Timeline | Cinemáticas (guía 08) |
| Animation Rigging | IK de apuntado y pies (guía 12) |
| Localization | Inglés y español (guía 10) |
| Test Framework | Pruebas (guía 13) |

TextMeshPro ya viene con uGUI; importa sus *Essential Resources* cuando Unity lo pida. Ningún paquete de la Asset Store (D-107).

### 4. URP

1. En `Assets/_Project/Settings/` crea dos *URP Assets* (`URP_High`, `URP_Low`) con su *Renderer*.
2. En cada *Renderer*: *Rendering Path* = **Forward+**.
3. En cada *URP Asset*: *Upscaling Filter* = **Nearest-Neighbor**, *HDR* activado, sombras de luces adicionales activadas con *Max Additional Lights* según `GDD/07 - Tecnología/03 - Requisitos Técnicos.md` (High: 4 con sombra; Low: 2). La *Render Scale* la fija un script en tiempo de ejecución (guía 12).
4. *Lighting → Environment*: sin *Skybox*, luz ambiental casi negra (`#0f0607`); desactiva *Baked Global Illumination* en todas las escenas (D-110: las salas son procedurales).

### 5. Estructura de carpetas

Crea esta estructura (D-107):

```text
Assets/
└── _Project/
    ├── Art/            (Characters, Enemies, Environment/<Piso>, Props, Materials, Shaders, UI)
    ├── Audio/          (SFX, Music, Ambience, Mixers)
    ├── Code/
    │   ├── IdentityDissolution.asmdef
    │   ├── Core/  Player/  Camera/  Combat/  AI/  Inventory/
    │   ├── Generation/  Persistence/  DeathCycle/  Recycle/  Boss/  Audio/  UI/
    ├── Data/           (ScriptableObjects: Balance, Enemies, Items, Rooms, Documents, Configs)
    ├── Input/          (IdentityDissolution.inputactions)
    ├── Localization/
    ├── Prefabs/        (Player, Enemies, Rooms/<Piso>, Props, UI)
    ├── Scenes/         (Bootstrap, MainMenu, Intro, Floor, Cinematics, Tests/)
    ├── Settings/       (URP, Volume profiles)
    ├── Timelines/
    └── Tests/
        ├── EditMode/IdentityDissolution.Tests.EditMode.asmdef
        └── PlayMode/IdentityDissolution.Tests.PlayMode.asmdef
```

- El `asmdef` de `Code/` referencia `Unity.InputSystem`, `Unity.Cinemachine`, `Unity.AI.Navigation`, `Unity.Timeline`, `Unity.Localization`, `Unity.TextMeshPro` y `Unity.RenderPipelines.Universal.Runtime`.
- El `asmdef` de EditMode referencia `IdentityDissolution`, marca *Editor* como única plataforma y activa *Test Assemblies*.

### 6. Escenas base

Crea las escenas vacías `Bootstrap`, `MainMenu`, `Intro`, `Floor` y `Cinematics` en `Scenes/` y añádelas a *File → Build Profiles → Scene List* con `Bootstrap` en el índice 0. Su contenido se arma en la guía 01.

### 7. Primer commit

1. Comprueba que `git status` no muestra `Library/` ni `Temp/`.
2. Comprueba con `git lfs ls-files` que los binarios van por LFS.
3. Rama `feature/proyecto-base` → *pull request* a `develop`.
4. Los otros 4 integrantes clonan, abren el proyecto y confirman que abre sin errores en la consola.

## Criterios de aceptación

- El proyecto abre sin errores ni advertencias de paquetes en los 5 equipos con la misma versión de Unity.
- `ProjectSettings/ProjectVersion.txt` es igual en todos los clones.
- *Force Text* y *Visible Meta Files* activos; `Library/` no está en el repositorio.
- Una build vacía de Windows arranca y muestra la escena `Bootstrap`.

## Problemas frecuentes

- **"Package not found" al abrir:** alguien abrió con otra versión de Unity; vuelve a la versión de `ProjectVersion.txt`.
- **Archivos `.fbx` de pocos bytes:** falta `git lfs install`; ejecútalo y haz `git lfs pull`.
- **Conflictos en `.unity`:** una persona por escena a la vez (D-108); las salas viven en prefabs, no en la escena `Floor`.
