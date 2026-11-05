# Guía del equipo: cómo funciona el juego en Unity

Para quien necesite modificar el proyecto sin haber escrito el código. Explica qué hace cada parte, dónde está y qué pasos seguir para los cambios más comunes.

**Unity 6000.6.6f1 · URP 17 (Forward+) · C# · demo para PC de un jugador.**

---

## 1. Antes de empezar

### Qué leer si hay dudas
| Tema | Dónde |
|---|---|
| Diseño: reglas y números | `GDD/` (en español) |
| Decisiones y sus motivos | `GDD/10 - Referencia/03 - Registro de Decisiones.md`; busca el código `D-xx` |
| Pasos técnicos por sistema | `Desarrollo/00-project.md` a `15-texts.md` (en inglés) |
| Medidas del kit y de las salas | `Desarrollo/KIT_SPEC.md` |
| Eventos entre sistemas | `Desarrollo/EVENTS.md` |
| Sonidos y modelos provisionales | `Desarrollo/AUDIO_CUES.md`, `Desarrollo/PLACEHOLDERS.md` |
| Todos los textos del juego | `GDD/03 - Historia y Mundo/05 - Textos del Juego.md` |

**Si el código y el GDD no coinciden, manda el GDD.** Si cambias una regla de diseño, actualiza también el GDD y anota la decisión en el registro.

### Primer uso
1. Instala Git LFS **antes de clonar**: `git lfs install`. Sin LFS, las texturas, modelos y audio llegan como pequeños archivos de texto y el proyecto se ve roto.
2. Abre el proyecto con **Unity 6000.6.6f1**, exactamente esa versión.
3. La primera apertura tarda porque Unity importa todo y descarga los paquetes de `Packages/manifest.json`. Hay dos paquetes que se instalan desde Git, así que hace falta conexión a internet.
4. Abre `Assets/_Project/Scenes/Bootstrap.unity` y pulsa Play.

### Probar el juego
- **Play desde cualquier escena:** el proyecto carga `Bootstrap` primero (`BootstrapLoader` en `Core/Bootstrap.cs`).
- **Play dentro de `Floor`:** genera un piso directamente, con los valores del Inspector de `FloorSession`. Con semilla 0 usa una al azar.
- **Galería de assets:** `Sandbox_Gallery` muestra todos los modelos por tipo y piso, sin ejecutar el juego. Su panel tiene un botón para iniciar el juego real.
- **`Heartbeat ▸ Sandbox ▸ Play Starts in Gallery`:** hace que Play abra siempre la galería. Se guarda por equipo y viene apagado.
- **Partidas guardadas:** están en `C:\Users\<usuario>\AppData\LocalLow\Heartbeat\Identity Dissolution\saves\` (`global.json` y `slot_0.json` a `slot_2.json`). Borra esa carpeta para probar como jugador nuevo, incluida la calibración de brillo.

---

## 2. Cómo está organizado

```
Assets/_Project/
  Scripts/<Sistema>/      Código del juego, por sistema
  Scripts/Editor/         Herramientas del menú "Heartbeat" (no van en el build)
  Sandbox/                Galería de assets (solo editor)
  Tests/EditMode/         Pruebas automáticas
  Prefabs/                Kit, Rooms, Props, Items, Characters, Level, PH, UI
  ScriptableObjects/      Datos editables: Balance, Enemies, Rooms, Floors, Docs, Audio
  Models/                 Modelos exportados desde Blender
  Materials/              Materiales por piso (F1, F2, F3, Shared)
  Localization/Source/    Textos editables (.tsv)
  Input/                  Heartbeat.inputactions
  Scenes/                 Escenas
Blender3DAssets/          .blend y scripts de Blender (fuente de los modelos)
GDD/                      Diseño
Desarrollo/               Guías técnicas
```

**Pisos:** `F1` Mantenimiento (ámbar de sodio), `F2` Clínica ilegal (fluorescente frío), `F3` Núcleo (carmesí). `Shared` se usa en todos.

**Nombres:** `KIT_F1_Wall_2m` (kit), `ROOM_F1_Corridor_A` (sala), `KEY_F1_Final` (sala clave), `PROP_F1_Boiler` (prop), `PH_` (provisional), `CUE_` (sonido), `RD_` (definición de sala), `FC_` (configuración de piso), `ED_` (definición de enemigo).

### Escenas
| Escena | Función | En el build |
|---|---|---|
| `Bootstrap` | Crea los servicios que duran toda la sesión | Sí, la primera |
| `MainMenu` | Calibración, título, menú, ranuras, opciones | Sí |
| `Intro` | Introducción jugable | Sí |
| `Floor` | Escena casi vacía: las salas las crea el generador | Sí |
| `Cinematics` | Cinemáticas, cargada encima de otra escena | Sí |
| `Sandbox_*` | Pruebas y construcción | **No** |

No coloques salas a mano en `Floor`: el generador las crea en cada partida.

---

## 3. Reglas del proyecto

Rompen el juego o complican el trabajo de los demás si no se cumplen.

1. **Cada número de balance vive en un ScriptableObject,** nunca fijo en el código. Están en `ScriptableObjects/Balance/`.
2. **Los sistemas se comunican por `GameEvents`.** Quien emite no sabe quién escucha. Un sistema puede leer el estado de otro, pero solo lo cambia con un método público o un evento.
3. **Sin singletons:** los servicios se piden con `Services.Get<T>()`. No uses `FindObjectOfType` dentro de `Update`.
4. **La lógica pura va en clases normales de C#,** sin `MonoBehaviour`, para poder probarla. Por ejemplo `StaminaModel`, `InventoryModel` o `LayoutGenerator`.
5. **Solo `PauseService` cambia `Time.timeScale`.** El tiempo se detiene únicamente en el menú de pausa y al leer un documento o póster (D-159). Inventario, listas del terminal y hackeo siguen en tiempo real.
6. **La entrada se lee con `InputRouter`,** nunca con `Keyboard.current` o `Mouse.current` en el código de juego.
7. **Todo texto visible usa claves de localización** con `Loc.Get(clave)`. El alemán no se traduce: queda igual en ambos idiomas.
8. **Ningún `AudioClip` en el código:** todo sonido es un asset `AudioCue`.
9. **Interfaz diegética (D-86), sin HUD permanente.** En pantalla solo van retícula, indicador `E`, subtítulos, "SYNCHRONIZING…" y el aviso breve `REGISTERED`. La salud se lee en la postura de Lukas y el terminal del brazo.
10. **La generación es determinista:** misma semilla y misma configuración dan el mismo piso. La semilla se escribe en el log, así que anótala al reportar un fallo.
11. **Solo paquetes oficiales de Unity** para el juego, nada de Asset Store.
12. **Código, comentarios y logs en inglés.** El juego sale en inglés (principal) y español.

---

## 4. Cómo arranca y fluye una partida

```
Bootstrap → Calibración de brillo (solo la primera vez) → Título → Menú principal
  → Intro (partida nueva) → Loading → Playing ⇄ Paused
  Playing → FloorTransition → siguiente piso
  Playing → Death → Reconstruction → Loading → Playing
  Piso 3 → The Mother → FinalDecision → final → Credits
```

- **`Bootstrap` (`Core/Bootstrap.cs`):** crea y registra `GameStateMachine`, `SceneLoader`, `PauseService`, `InputRouter`, `RegistryModel`, `FaultModel`, `SaveService`, `CycleController` y `AudioService`, y aplica idioma, controles y video guardados.
- **`GameStateMachine`:** sus estados están en `Core/GameStateId.cs`. Cambian con `TryTransition`, que rechaza los cambios no permitidos.
- **`CycleController` (`Cycle/`):** lleva el bucle de muerte y reconstrucción, el paso entre pisos, el contador de muertes y el autoguardado.
- **`FloorSession` y `FloorBuilder` (`Generation/`):** construyen el piso pedido al entrar en `Floor`.

---

## 5. Los sistemas, uno por uno

| Carpeta en `Scripts/` | Qué hace | Clases clave |
|---|---|---|
| `Core` | Arranque, estados, eventos, escenas, pausa, logs | `Bootstrap`, `GameEvents`, `Services`, `PauseService`, `HbLog` |
| `Input` | Contextos de control | `InputRouter`, `HeartbeatInput` (autogenerado) |
| `Player` | Movimiento, resistencia, salud, linterna | `PlayerController`, `PlayerHealth`, `Flashlight` |
| `Camera` | Cámara al hombro y vigilancia | `OtsCameraRig`, `SurveillanceSystem`, `AttentionMeter` |
| `Combat` | Revólver, empujón, remate, punto débil | `Revolver`, `Shove`, `PlayerFinisher`, `WeakPoint` |
| `AI` | Percepción, alerta y conducta de enemigos | `EnemyBrain`, `AlertDirector`, `EnemyDefinition` |
| `Inventory` | 6 ranuras | `InventoryModel`, `PlayerInventory` |
| `Interaction` | Lo que se usa con `E` | `Interactor`, `IInteractable`, `WorldItem`, `DocumentPickup` |
| `Level` | Puertas, hackeo, energía, salida | `DoorLeaf`, `PlayerHack`, `PowerSwitch`, `FloorExit`, `BrokenElevator` |
| `Generation` | Generación de pisos | `LayoutGenerator`, `LayoutValidator`, `FloorBuilder`, `RoomActivation` |
| `Cycle` | Muerte, progreso y finales | `CycleController`, `DeathCounter`, `EndingDirector` |
| `Save` | Guardado y opciones | `SaveService`, `SaveData`, `OptionsDefaults` |
| `Recycle` | Cadáveres que vuelven a levantarse | `RecycleSystem`, `Corpse` |
| `Boss` | The Mother | `MotherDirector`, `MotherFight`, `BossNode` |
| `Audio` | Sonido, mezcla, latido | `AudioService`, `AudioCue`, `LabHeartbeat`, `ThreatAudio` |
| `UI` | Terminal, menús, opciones, lectura | `TerminalView`, `MenuView`, `OptionsView`, `ReaderView`, `UiKit` |
| `Narrative` | Registro, documentos, intro | `RegistryModel`, `DocumentDefinition`, `IntroDirector` |
| `Render` | Aspecto pixelado, brillo, degradación | `PixelationFeature`, `Brightness`, `Degradation`, `VideoSettings` |

### Generación de pisos
1. `FloorConfig` (`ScriptableObjects/Floors/FC_F#_Full` y `_Showcase`) define cuántas salas, objetos, enemigos y documentos tiene el piso.
2. `LayoutGenerator` crea el grafo de salas con la semilla y `LayoutValidator` lo comprueba. Ambos son C# puro y tienen pruebas.
3. `FloorBuilder` coloca los prefabs en la cuadrícula de 2 m, monta paredes y suelos con `RoomShell`, pone puertas y cerraduras, llena los `SpawnPoint`, hornea el NavMesh y coloca a Lukas.
4. `RoomActivation` activa solo lo cercano y limita las luces: 14 luces por sala activa y 3 con sombra en calidad alta.

### Controles por contexto
`InputRouter.SetContext` activa solo las acciones de cada situación, para que una tecla nunca tenga dos significados a la vez.

| Contexto | Se usa en |
|---|---|
| `Exploration`, `Aiming`, `OverBody` | Juego normal, apuntando, sobre un cuerpo |
| `Surveillance` | Vista de cámara |
| `Inventory` | Terminal del brazo abierto |
| `Hacking` | Secuencia de hackeo |
| `Reading` | Leyendo un documento o póster |
| `Menu`, `Cinematic` | Menús y cinemáticas |
| `IntroPlayable` | Introducción: mover, mirar e interactuar |

### Interfaz
Casi toda se **construye por código** con `UI/UiKit.cs`: `Prefabs/UI/HUD.prefab` solo tiene las raíces y las referencias. Para cambiar un color, tamaño o botón, edita el script de esa pantalla (`TerminalView`, `MenuView`…), no el prefab.

- `UiKit` guarda los colores y crea textos, botones y paneles.
- `UiFocusStyle` da el aspecto de foco: fila iluminada, texto oscuro y marcador `>`.
- Fuentes: VT323 para datos e IBM Plex Sans para títulos y menús. Teal es Lukas, verde son las pantallas de la instalación y cian es la marca Vitalis.
- El diseño de referencia está en Figma: [Identity Dissolution — UI/UX](https://www.figma.com/design/EWpLs3vJvGYN1uqbobt2yH/Identity-Dissolution-%E2%80%94-UI-UX).

---

## 6. Qué cambiar y cómo

Después de cualquier cambio, usa la lista de la sección 8.

### Ajustar el balance
Selecciona el asset y edita el valor en el Inspector. No hace falta tocar código.

| Quiero cambiar | Asset en `ScriptableObjects/` |
|---|---|
| Velocidad, resistencia, salud, linterna | `Balance/PlayerBalance` |
| Revólver, empujón, punzón | `Balance/CombatBalance` |
| Cámara | `Balance/CameraBalance` |
| Fallas del sistema | `Balance/FaultBalance` |
| The Mother | `Balance/BossBalance` |
| Mezcla de audio y latido | `Balance/AudioBalance` |
| Vida, velocidad, vista y ataques de un enemigo | `Enemies/ED_Alto`, `ED_Boca`, `ED_Mudo`, `ED_Rastrero` |
| Cantidad de salas, objetos y enemigos por piso | `Floors/FC_F#_Full` (o `_Showcase`) |

### Cambiar o añadir un texto
1. Edita el `.tsv` que corresponda en `Localization/Source/`. Las columnas son `key`, `en`, `es` y `note`, separadas por **tabulador**.
2. Ejecuta `Heartbeat ▸ Import Localization`.
3. En código, usa `Loc.Get("UI.MI.CLAVE")`. Con parámetros: `{0}` en el texto y `Loc.Get("CLAVE", valor)`.

| Archivo | Contenido |
|---|---|
| `UI.tsv` | Menús, terminal, avisos |
| `SUB.tsv`, `CAP.tsv` | Subtítulos y subtítulos de sonido |
| `DOC.tsv`, `CONF.tsv`, `BEST.tsv` | Documentos, objetos confiscados, bestiario |
| `POST.tsv` | Pósters |
| `END.tsv` | Finales y créditos |

No edites las tablas de `Localization/Tables/`: el importador las sobrescribe.

### Añadir un documento u objeto legible
1. Añade título y cuerpo en `DOC.tsv`, `CONF.tsv` o `BEST.tsv` e importa.
2. Crea un `DocumentDefinition` en `ScriptableObjects/Docs/` con `id` estable (se guarda en las partidas, no lo cambies después), `kind`, `floor`, `titleKey` y `bodyKey`. En `model` puedes poner un modelo propio.
3. Añádelo a la lista `documents` de `TerminalView` en `HUD.prefab`. Si falta, no se podrá abrir desde LOGS.
4. Para que aparezca en los pisos, añádelo a las listas de `FloorBuilder` en la escena `Floor`: `emiliaDocuments`, `contextDocuments` o `confiscatedObjects`.

### Añadir o reemplazar un sonido
1. Crea o abre el `AudioCue` en `ScriptableObjects/Audio/` (`CUE_Nombre`) y asigna clips, bus, prioridad y volumen. Sin clip no hay error: solo no suena.
2. Si es nuevo, añade su nombre como constante en `Core/Sfx.cs` y llámalo con `Sfx.Play(Sfx.MiCue, posicion)` o `Sfx.Play2D(Sfx.MiCue)`.
3. Regístralo en `Desarrollo/AUDIO_CUES.md`.

### Añadir una sala
La regla: el **kit y la distribución de cada sala se hacen a mano**. Respeta las medidas de `KIT_SPEC.md`: celdas de 2 m, paredes de 3 m × 0,3 m y puertas de 2 m × 2,2 m.

1. Parte de una sala parecida y crea el prefab en `Prefabs/Rooms/F#/`. **El nombre importa,** porque de él se leen los metadatos: `ROOM_F1_Storage_A`, `KEY_F1_Secondary_A`, `KEY_F1_Final`, `KEY_F1_CapsuleRow`.
2. Tipos válidos: `Corridor`, `Storage`, `Open`, `Surveillance`, `Hack`, `Vertical`.
3. Configura `RoomShell` (tamaño en celdas, kit del piso).
4. Coloca los `SOCKET` para las puertas y los `SP_Enemy`, `SP_Item` y `SP_Player` de `Prefabs/Level/`. No tapes un socket ni un spawn con props.
5. Ejecuta `Heartbeat ▸ Rooms ▸ Refresh Definitions`. Valida la sala, crea su `RD_` y actualiza la biblioteca. **Lee la consola:** ahí avisa de lo que está mal.
6. Para crear las versiones de F2 y F3, ejecuta `Heartbeat ▸ Rooms ▸ Re-dress Missing F2-F3 Variants` y luego otra vez `Refresh Definitions`. Solo crea las que faltan y no pisa las existentes.
7. Corre las pruebas: comprueban generación y solapes entre prefabs.

> **Cuidado:** `Heartbeat ▸ Rooms ▸ Dress Rooms` reconstruye el hijo `Dress_F#` de cada sala. Lo que coloques a mano dentro de ese hijo se pierde; pon tus cambios fuera de él.

### Añadir un prop
- Con forma simple: añádelo a `Desarrollo/OBJECT_CATALOG.md` y ejecuta `Heartbeat ▸ Props ▸ Build Missing Catalog Props`. No sobrescribe lo existente.
- Con modelo propio: sigue el flujo siguiente y crea el prefab en `Prefabs/Props/F#/`.

### Cambiar un modelo
1. Edita el `.blend` en `Blender3DAssets/Blends/`. Hay uno por modelo.
2. Exporta el FBX a `Assets/_Project/Models/`. Los scripts están en `Blender3DAssets/Scripts/`, y `export_fbx.py` tiene los ajustes correctos.
3. `ModelImportRules` aplica solo las reglas de importación: filtro point, sin mipmaps, clips a 30 fps.
4. Para el entorno, ejecuta `Heartbeat ▸ Models ▸ Wire Environment Models`. Para Lukas, `Heartbeat ▸ Models ▸ Build Lukas Animator`.

**Los scripts de juego van en el prefab padre, nunca en la malla,** para poder cambiar el modelo sin perder nada. Los provisionales se llaman `PH_<Nombre>` y se registran en `PLACEHOLDERS.md`.

### Añadir un enemigo o cambiar su conducta
- Los números van en su `ED_`. La lógica está en `AI/EnemyBrain.cs` y las animaciones en `EnemyAnimator`.
- El tipo se elige en `FloorConfig.enemyTypes`, y `FloorBuilder` tiene un campo de prefab por tipo.
- Un tipo nuevo también pide un valor en `EnemyType`, una definición, un prefab en `Prefabs/Characters/` y un campo nuevo en `FloorBuilder`.

### Añadir algo interactuable
1. Crea un script que implemente `IInteractable`: `PromptKey`, `InteractionPoint`, `CanInteract` e `Interact`.
2. Dale un collider en la capa **`Interactable`**. Sin esa capa, `Interactor` no lo detecta.

### Añadir una acción de control
1. Edita `Input/Heartbeat.inputactions`, con enlaces de teclado y de mando.
2. Unity regenera `Scripts/Input/HeartbeatInput.cs`. **No lo edites a mano.**
3. Úsala con `router.Actions.Mapa.Accion`. Si es de `Gameplay`, comprueba la lista `GameplayWhitelist` de `InputRouter.cs`.

### Añadir una opción
1. Añade el campo con su valor por defecto a `GlobalData` en `Save/SaveData.cs`.
2. Añade el reinicio de ese campo a `Save/OptionsDefaults.cs` y su prueba.
3. Añade la fila en `UI/OptionsView.cs` con `SliderRow`, `ToggleRow` o `CycleRow`.
4. Aplica el efecto. Quien deba reaccionar en vivo escucha `GameEvents.OptionsChanged`.

### Añadir un dato guardado en la partida
Añade el campo a `SlotData` con un valor por defecto que no rompa partidas antiguas. Los pisos no se guardan: al continuar se genera una semilla nueva (D-101).

### Comunicar dos sistemas
1. En `Core/GameEvents.cs`, añade el evento, su método `Raise…` y su línea `= null` en el reinicio. Si olvidas esa línea, quedan suscriptores colgados.
2. Suscríbete en `OnEnable` y desuscríbete en `OnDisable`.
3. Documéntalo en `Desarrollo/EVENTS.md`.

### Añadir un sistema nuevo
Crea la carpeta `Scripts/<Sistema>/` con namespace `Heartbeat.<Sistema>`. Separa la lógica pura de los `MonoBehaviour`, añade pruebas, y pon los números en un ScriptableObject de `Balance/`.

---

## 7. Menú `Heartbeat` del editor

Son seguros de repetir y no sobrescriben assets existentes, salvo donde se indica.

| Menú | Qué hace |
|---|---|
| `Import Localization` | Pasa los `.tsv` a las tablas |
| `Rooms ▸ Refresh Definitions` | Valida las salas y actualiza `RD_` y la biblioteca |
| `Rooms ▸ Re-dress Missing F2-F3 Variants` | Crea las variantes de piso que faltan |
| `Rooms ▸ Re-dress F2 B Layouts as F1` | Crea versiones F1 de las distribuciones B de F2 |
| `Rooms ▸ Dress Rooms` | **Reconstruye** el hijo `Dress_F#` de cada sala |
| `Props ▸ Build Missing Catalog Props` | Crea los props del catálogo que faltan |
| `Props ▸ Finish Props In Scene` | Termina los props de la escena abierta |
| `Kit ▸ Finish Kit Pieces In Scene` | Termina las piezas de kit de la escena abierta |
| `Items ▸ Finish Items In Scene` | Termina los objetos de la escena abierta |
| `Models ▸ Wire Environment Models` | Pone los modelos de Blender en los prefabs |
| `Models ▸ Build Lukas Animator` | Genera el Animator de Lukas |
| `Placeholders ▸ Strip Mesh Colliders In Scene` | Quita colliders de malla en la escena abierta |
| `Sandbox ▸ Create Gallery Scene` | Crea la escena de la galería |
| `Sandbox ▸ Play Starts in Gallery` | Hace que Play abra la galería (por equipo) |

---

## 8. Comprobar un cambio

1. **Compila** sin errores y revisa la consola, también los avisos.
2. **Guarda** escena y prefabs. Unity no guarda solo.
3. **Corre las pruebas:** `Window ▸ General ▸ Test Runner ▸ EditMode ▸ Run All`. Todas deben pasar.
4. **Juégalo:** prueba el caso cambiado y sus vecinos. Si tocaste una sala, genera varias semillas. Si tocaste un sistema con textos, míralo en los dos idiomas.
5. **Lee los logs:** cada sistema escribe con el prefijo `[Heartbeat][Sistema]`. Algunas comprobaciones dependen de esas líneas, así que no las cambies sin motivo. Lo que solo sirva para depurar va con `HbLog.Dev(...)`, que no llega a la versión final.

---

## 9. Trabajo con Git

Usamos **Git Flow**: `main` para versiones, `develop` para integrar, y **una rama por tarea**.

```bash
git flow feature start E07-floor-validator
# ...trabajo y commits...
git flow feature finish E07-floor-validator
```

- **Mensajes:** en imperativo y con la épica delante, por ejemplo `E07: add LayoutValidator with 1000-seed test`.
- **Revisa antes de cada commit:** usa `git status` y añade solo lo tuyo. Unity suele modificar assets por su cuenta, como `OpenDyslexic SDF.asset` o `Assets/AddressableAssetsData`; no subas esos cambios si no son tuyos.
- **Los `.meta` siempre van con su archivo.** Si faltan, las referencias se rompen.
- **Escenas y prefabs:** avisa si vas a tocarlos. Son archivos difíciles de fusionar, así que lo mejor es que no los edite más de una persona a la vez.
- **Archivos grandes:** LFS se encarga solo de imágenes, audio, fuentes y modelos (ver `.gitattributes`).

---

## 10. Problemas frecuentes

| Síntoma | Causa probable | Solución |
|---|---|---|
| Texturas moradas o rotas tras clonar | LFS no descargó los archivos | `git lfs install` y `git lfs pull` |
| Pulso Play y sale la galería | Opción activada | Apaga `Sandbox ▸ Play Starts in Gallery` |
| Mi sala nueva nunca aparece | Definición sin actualizar o nombre incorrecto | `Rooms ▸ Refresh Definitions` y lee la consola |
| Triángulos negros en paredes de F2 o F3 | Modelo de F1 duplicado bajo el del piso | `Models ▸ Wire Environment Models` |
| Un material emisivo deja de brillar | URP limpia la emisión | Pon `Global Illumination` en Realtime en el material |
| `E` no detecta mi objeto | Falta la capa o `IInteractable` | Collider en capa `Interactable` y revisa `CanInteract` |
| Un texto sale como clave (`UI.ALGO`) | No se importó o la clave está mal | `Import Localization` y revisa el `.tsv` |
| Un documento no se abre desde LOGS | No está en `TerminalView.documents` | Añádelo en `HUD.prefab` |
| Un enemigo no camina | Sin NavMesh o spawn fuera de él | Revisa la posición de `SP_Enemy` y que el piso hornee bien |
| El juego queda congelado | Lectura o pausa sin cerrar | Revisa los logs y `PauseService`; solo él toca `Time.timeScale` |
| El editor se cuelga | Scripts editados durante Play | Sal de Play antes de guardar scripts |
| Un fallo de generación no se repite | Semilla distinta | Copia la semilla del log y úsala en `FloorSession` |
