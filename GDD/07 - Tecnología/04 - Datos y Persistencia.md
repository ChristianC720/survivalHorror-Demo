# Datos y Persistencia

## 1. Descripción de la sección

Define qué información necesita conservar el juego y cómo se estructura.

Las reglas de qué se guarda y cuándo están en el sistema de persistencia de [[03 - Sistemas]] y en [[07 - Reglas y Estados del Juego]]. También están en [[04 - Guardado y Configuración]]. Esta nota define el formato. Los valores numéricos de este documento son iniciales y están sujetos a balance.

## 2. Datos persistentes

- **Progreso, por ranura.** Código de sujeto, contador de muertes, muertes totales, degradación, marca de primer intento, modo y configuración showcase. También guarda el autoguardado del intento, el inicio del piso actual y la marca de salida confirmada con su costo.
- **Marcas de la historia, por ranura.** Introducción vista, hallazgo del piso 2 visto, autoguardado de la decisión final, ciclo completado y ciclo nuevo pendiente tras un final.
- **Configuración, global.** Audio, video, controles reasignados, accesibilidad, idioma y calibración de brillo.
- **Inventario.** Solo vive dentro del autoguardado del intento y del inicio del piso. No persiste lo recogido al morir.
- **Estadísticas.** La demo solo guarda el contador de muertes totales de la ranura.
- **Desbloqueos.** El terminal del brazo se guarda por ranura, con documentos, bestiario, objetos confiscados, reglas, fallas y anillo. Las fallas parcheadas también se guardan. Las cinemáticas y los créditos vistos se guardan de forma global.
- **Otros.** No hay nada en línea ni ningún dato personal.

## 3. Modelo de datos

El modelo está en `Assets/_Project/Scripts/Save/SaveData.cs`.

```text
GlobalData (global.json)
├── saveVersion · lastUsedSlot (0-2)
├── brightnessCalibrated · brightness · language
├── bindingOverrides
├── volMaster · volMusic · volSfx · volAmbience · volVoices
├── subtitles · subtitleOpaque · soundCaptions · textScale · dyslexiaFont
├── crouchToggle · invertY · lookSensitivity · aimSensitivity · aimAssist
├── resolutionWidth · resolutionHeight · screenMode · vsync · fpsLimit · shadowQuality
├── terminalScale · reduceMotion · reduceFlashes · reduceCrt
└── seenCinematics [ids] · creditsSeen

SlotData (slot_0.json … slot_2.json)
├── saveVersion · createdAt · updatedAt
├── mode (Survival | Story) · showcase
├── subjectCode (7 = R-H-07 …)
├── deathCounter · totalDeaths · degradationLevel · firstAttempt
├── introDone · floor2PhotoSeen · finalDecisionSave · cycleCompleted · newCyclePending
├── terminal [ {id, kind} ]
│   kind: EmiliaDocument | ContextDocument | Confiscated | Bestiary | Rule | Fault | Ring
├── patchedFaults [id]
├── hasAttemptSave · attemptSave
├── hasFloorStart · floorStart
│   AttemptSave: floor · inventory [ {Kind, Count} ×6 ] · revolverLoaded · revolverOwned · health
└── confirmedExit (bool) · exitCost (None | Calm | Hunt)
```

- `JsonUtility` no serializa objetos nulos. Por eso `attemptSave` y `floorStart` siempre existen, y `hasAttemptSave` y `hasFloorStart` indican si son válidos.
- `floorStart` guarda el inventario con el que Lukas llegó al piso actual. Es el punto al que regresan la muerte, `Reiniciar` y la salida con costo.
- En `AttemptSave`, `health` vale `-1` cuando la salud está completa.
- La distribución del piso no se guarda. Al continuar, el piso se genera con una semilla nueva.
- Los identificadores `id` del terminal son los nombres de los ScriptableObjects de documentos, objetos y enemigos, nunca índices. Así, reordenar el contenido no rompe partidas.

## 4. Formato

**Formato de guardado:** JSON legible con `JsonUtility`, sin cifrado. Hay un archivo por ranura y uno global. Lo escribe `SaveService`, en `Assets/_Project/Scripts/Save/SaveService.cs`.

**Ubicación:** `Application.persistentDataPath/saves`. En Windows es `%USERPROFILE%\AppData\LocalLow\Heartbeat\Identity Dissolution\saves\`.

**Escritura atómica:** El juego escribe `slot_N.json.tmp`, reemplaza el archivo con `File.Replace` y conserva `slot_N.json.bak`. El archivo global sigue el mismo proceso. Un fallo nunca deja la ranura corrupta ([[04 - Guardado y Configuración]]). La escritura sale del hilo principal solo si el perfilado muestra que dura más de 1 fotograma.

**Versionado de datos:** Cada archivo lleva un entero `saveVersion`, que empieza en `1`. La versión actual es `SaveService.CurrentVersion = 1`.

## 5. Migraciones

- Al cargar, si `saveVersion` es menor que la actual, `Migrate()` aplica en orden las migraciones `vN → vN+1` y vuelve a guardar. Con la versión 1 todavía no hay ninguna migración.
- Un campo nuevo se inicializa con su valor por defecto. Un campo eliminado se ignora.
- Si `saveVersion` es mayor que la del juego, o el JSON no se puede leer, la ranura aparece dañada y se ofrece borrarla. Nunca se sobrescribe sin confirmación.
- Si existe un `.bak` válido, se ofrece restaurarlo.
- Si el archivo global no se puede leer, la configuración vuelve a los valores por defecto.
- Cada cambio de estructura se registra como solicitud de cambio ([[05 - Gestión de Cambios]]) y añade una prueba de migración ([[02 - Pruebas]]).

> **Navegación:** [[00 - Índice]] · ← [[03 - Requisitos Técnicos]] · [[05 - Seguridad y Riesgos Técnicos]] →
