# Datos y Persistencia

## Descripción de la sección

Define qué información necesita conservar el juego y cómo se estructura.

Las reglas de qué se guarda y cuándo están en S-07 ([[03 - Sistemas]]), [[07 - Reglas y Estados del Juego]] y [[04 - Guardado y Configuración]]. Esta nota define el formato (D-109).

## Datos persistentes

- **Progreso (por ranura):** código de sujeto, contador de muertes, degradación, marca de primer intento, modo (D-103), autoguardado del intento y marca de salida confirmada.
- **Configuración (global):** audio, video, controles reasignados, accesibilidad, idioma y calibración de brillo.
- **Inventario:** solo dentro del autoguardado del intento (S-05); no persiste al morir.
- **Estadísticas:** ninguna en la demo.
- **Desbloqueos:** terminal del brazo (documentos, bestiario, objetos confiscados, fallas, anillo, mensaje de Emilia) por ranura; cinemáticas y créditos vistos (global, D-91).
- **Otros:** nada en línea; ningún dato personal.

## Modelo de datos

```text
GlobalData (global.json)
├── saveVersion
├── lastUsedSlot (0–2)
├── settings (audio, video, controles, accesibilidad, idioma, brillo)
└── seenCinematics [ids]  ·  creditsSeen

SlotData (slot_0.json … slot_2.json)
├── saveVersion · createdAt · updatedAt
├── mode (Survival | Story)
├── subjectCode (7 = R-H-07 …)
├── deathCounter · degradationLevel · firstAttempt · bodyVariant
├── cycleCompleted (para el ciclo nuevo, D-93)
├── terminal
│   ├── documents [ {id, type: Emilia|Vitalis|Kessler|Mother, corrupted, completed} ]
│   ├── bestiary [enemyType]
│   ├── confiscated [id]
│   ├── faults [ {id, discovered, patched} ]
│   └── ringFound · voiceNoteCopied
├── attemptSave (nullable)
│   ├── floor (1 | 2 | decisión final)
│   └── inventory [ {itemType, amount} ×6 ]
└── confirmedExit (bool) · exitCost (None | Calm | Hunt)
```

- La distribución del piso **no** se guarda: al continuar se genera con una semilla nueva (D-101).
- Los identificadores (`id`) de documentos, objetos y enemigos son los nombres de sus ScriptableObjects, nunca índices, para que reordenar el contenido no rompa partidas.

## Formato

**Formato de guardado:** JSON legible con `JsonUtility`, sin cifrado (D-109). Un archivo por ranura y uno global.

**Ubicación:** `Application.persistentDataPath` (en Windows, `%USERPROFILE%\AppData\LocalLow\<Compañía>\Identity Dissolution\`).

**Escritura atómica:** se escribe `slot_N.json.tmp`, se reemplaza el archivo con `File.Replace` y se conserva `slot_N.json.bak`. Un fallo nunca deja la ranura corrupta ([[04 - Guardado y Configuración]]). La escritura ocurre fuera del hilo principal solo si dura más de 1 fotograma en el perfilado.

**Versionado de datos:** entero `saveVersion` en cada archivo, empezando en `1`.

## Migraciones

- Al cargar, si `saveVersion` es menor que la actual, `SaveMigrator` aplica en orden las migraciones `vN → vN+1` y vuelve a guardar.
- Un campo nuevo se inicializa con su valor por defecto; un campo eliminado se ignora.
- Si `saveVersion` es mayor que la del juego, o el JSON no se puede leer, la ranura aparece **dañada** y se ofrece borrarla; nunca se sobrescribe sin confirmación. Si existe `.bak` válido, se ofrece restaurarlo.
- Cada cambio de estructura se registra como solicitud de cambio ([[05 - Gestión de Cambios]]) y añade una prueba de migración ([[02 - Pruebas]]).


> **Navegación:** [[00 - Índice]] · ← [[03 - Requisitos Técnicos]] · [[05 - Seguridad y Riesgos Técnicos]] →
