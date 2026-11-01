# VS_Piso1 — Escena graybox (7 salas)

## Objective
Construir la escena `VS_Piso1` con 7 salas grises hechas a mano, conectadas por vanos de 2 m, con ambiente casi a 0, un foco ámbar por sala y un trigger `RoomVolume` por sala con nombre, zona segura y adyacencias para la IA.

## Changes
- `Assets/Scripts/Config/GameConfig.cs` - (crear) ScriptableObject con las métricas de nivel e iluminación.
- `Assets/ScriptableObjects/GameConfig.asset` - (crear) instancia editable en el Inspector.
- `Assets/Scripts/Level/RoomVolume.cs` - (crear) trigger con `roomId`, `isSafeZone`, `adjacentRoomIds` + refs `RoomVolume[]`, `Contains(Vector3)` y gizmo.
- `Assets/Scripts/Level/Door.cs` - (crear) hoja de puerta con `requiresLevelKey`/`isLocked` y `TryOpen()`.
- `Assets/Editor/GrayboxLevelBuilder.cs` - (crear) tabla fija de coordenadas + menú `Heartbeat/Graybox/Build VS_Piso1`. Solo edición; no se ejecuta en runtime.
- `Assets/Scenes/VS_Piso1.unity` - (crear) escena resultante (geometría, props, luces, triggers).

## Relevant Assets and Quirks
- Unity **6000.6.6f1**, URP **17.6.0**. Sin tags ni layers personalizados: todo en `Default`. Los `RoomVolume` son `isTrigger`, así que los raycasts futuros de interacción deben usar `QueryTriggerInteraction.Ignore`.
- Primitivas: cubo 1×1×1, cápsula 1 m de diámetro × 2 m de alto, plano 10×10 a escala 1 → suelo: `scale = tamaño/10`.
- `CharacterController` (default `slopeLimit` 45°, `stepOffset` 0.3): la rampa al 20° es caminable y el vano de 2.2 m supera la cápsula de 1.8 m.
- La escena se genera **una vez** desde una tabla de coordenadas fija y sin aleatoriedad; después la escena es la fuente de verdad (nada de generación en runtime). Las medidas quedan editables en el Inspector de cada objeto.
- `Assets/Scenes/SampleScene.unity`, `Assets/Prefabs/Player/Player.prefab` y `InputSystem_Actions.inputactions` no se tocan en este paso.
- URP toma la luz ambiental de Environment Lighting: `ambientMode = Flat`, color casi negro, y se elimina la Directional Light por defecto (si no, no hay oscuridad).

## Layout (1 unidad = 1 m; X este, Z norte; suelo en y=0; pared 0.3 de grosor × 3.0 de alto)

| Sala | Centro (x,z) | Interior X×Z | Segura | Adyacentes |
|---|---|---|---|---|
| FilaCapsulas | (0, 0) | 12×8 | **sí** | Pasillo |
| Pasillo | (19, 0) | 26×3 | no | FilaCapsulas, Almacen, SalaAbierta |
| Almacen | (14, 7.5) | 12×12 | no | Pasillo |
| SalaAbierta | (40, 0) | 16×16 | no | Pasillo, SalaClaveSecundaria |
| SalaVigilancia | (53, 10) | 10×10 | no | SalaClaveSecundaria |
| SalaClaveSecundaria | (53, 0) | 10×10 | no | SalaAbierta, SalaVigilancia, SalaClaveFinal |
| SalaClaveFinal | (65, 0) | 14×12 | no | SalaClaveSecundaria |

**Vanos (todos 2.0 m de ancho × 2.2 m de alto, muro de 0.3):** FilaCapsulas↔Pasillo en `x=6`; Pasillo↔Almacen en `z=1.5`; Pasillo↔SalaAbierta en `x=32`; SalaAbierta↔SalaClaveSecundaria en `x=48`; SalaClaveSecundaria↔SalaVigilancia en `z=5`; **SalaClaveSecundaria↔SalaClaveFinal en `x=58`, con hoja `Door` bloqueada (llave obligatoria)**. Cada muro se compone de jamba + dintel + jamba alrededor del vano.

**Props:** FilaCapsulas: 7 cápsulas (escala 0.8/1.0/0.8) en fila al norte (paso 1.2 m), pedestal + bandeja (cubo 0.5×0.1×0.35) y `PlayerSpawn` vacío. Pasillo: 2 coberturas 1.2×1.2×2.0. Almacen: 3 estanterías 0.6×2.2×9 que cortan la línea de visión. SalaVigilancia / SalaClaveSecundaria / SalaClaveFinal: cámara (cubo 0.4) en esquina alta (y=2.7) orientada al centro. SalaClaveSecundaria: pedestal + llave (cubo 0.15×0.05×0.3). SalaClaveFinal: rampa 4.6×0.3×4.0 a 20° al fondo (+X) con rellano.

**Ruta corta:** FilaCapsulas → Pasillo → SalaAbierta → SalaClaveSecundaria → SalaClaveFinal = **5 salas** (Almacen y SalaVigilancia quedan como ramas opcionales).

## Steps
- [x] 1. Crear `GameConfig.cs` + `Assets/ScriptableObjects/GameConfig.asset` con `[Header("Nivel")]` (alto de muro 3, grosor 0.3, ancho de vano 2, alto de vano 2.2, separación de cápsulas 1.2) y `[Header("Iluminación")]` (color ambiente, color ámbar, intensidad, rango, spotAngle, altura de foco, sombras). [depends: none]
- [x] 2. Crear `RoomVolume.cs` y `Door.cs` con `Debug.Log` de entrada/salida de sala y de intento de apertura bloqueada. [depends: 1]
- [x] 3. Crear `GrayboxLevelBuilder.cs` en `Assets/Editor/` con la tabla de las 7 salas, los 6 vanos, los muros, luces y props; lee todas las medidas de `GameConfig`. [depends: 1,2]
- [x] 4. Ejecutar el menú `Heartbeat/Graybox/Build VS_Piso1`: crear `Assets/Scenes/VS_Piso1.unity`, geometría, props, `RenderSettings` (ambiente casi 0, sin Directional Light) y los focos ámbar. [depends: 3]
- [x] 5. Cablear `roomId`, `isSafeZone` y `adjacentRooms` en cada `RoomVolume` (lista simétrica) y guardar escena + assets. [depends: 4]
- [x] 6. Verificar con `execute_code`: contar `RoomVolume` (7), comprobar simetría de adyacencias, BFS FilaCapsulas→SalaClaveFinal = 5 salas, y capturar vista cenital y vista a nivel de jugador. [depends: 5]

## Verify
- La escena `VS_Piso1` abre sin errores de compilación y sin warnings en consola.
- Existen 7 triggers con nombre exacto y adyacencias simétricas; `FilaCapsulas.isSafeZone == true`.
- BFS desde `FilaCapsulas` a `SalaClaveFinal` devuelve 5 salas (≥ 4).
- Todos los vanos miden 2.0 m y el jugador (cápsula de 1.8 m) pasa por ellos; la rampa de `SalaClaveFinal` es ascendible.
- Ambiente casi negro: sin Directional Light, 1 foco ámbar por sala (2 en Pasillo y SalaAbierta) y sin superficies iluminadas fuera de su cono.
- La puerta de `SalaClaveFinal` está bloqueada y su collider impide el paso; `TryOpen()` registra `ACCESS DENIED` en consola.
