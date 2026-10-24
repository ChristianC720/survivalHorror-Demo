# 07 — Generación Procedural (S-06)

**Objetivo:** construir cada piso con salas procedurales, salas clave, llaves, cámaras, enemigos, recursos y documentos según las reglas de S-06, sin producir nunca un piso imposible, y darle navegación a la IA.

**Responsable:** Christian Coronel Palma.

**Referencias:** `GDD/02 - Diseño/03 - Sistemas.md` (S-06, tabla de parámetros y criterios), `GDD/02 - Diseño/02 - Mecánicas.md` (M-15), `GDD/04 - Contenido/02 - Niveles.md` (arquetipos, tutorial, energía), `GDD/02 - Diseño/06 - Economía y Recursos.md` (presupuesto por piso), D-33, D-34, D-36, D-37, D-38, D-72, D-79, D-100, D-101, D-110.

**Depende de:** guías 01, 05 y 06.

## Enfoque (D-110)

```text
semilla → FloorGraphBuilder (C# puro) → FloorLayout → LayoutValidator ─(falla)→ otra semilla (máx. 20) → piso de respaldo
                                                          └─(válido)→ FloorInstantiator (prefabs) → NavMeshSurface.BuildNavMesh → RoomActivator
```

Todo lo que decide **qué** hay en el piso es lógica pura con `System.Random(seed)` (nunca `UnityEngine.Random`), así se prueba en EditMode con 1 000 semillas (CP-01). Unity solo **instancia** el resultado.

## Pasos

### 1. Salas como prefabs

- **Cuadrícula:** cada sala ocupa una celda de tamaño fijo (por ejemplo 12 × 12 m) para simplificar la conexión. Las salas grandes (sala abierta, sala de The Mother) pueden ocupar 2 × 2 celdas.
- **4 aberturas por sala:** cada prefab tiene una abertura centrada en cada lado (N, E, S, O) con un `DoorSocket`. Al instanciar, las aberturas sin conexión se cierran con un muro (`PF_WallPlug`) y las conectadas reciben un `PF_Door`. Así cualquier arquetipo encaja con cualquier conexión.
- **Marcadores dentro del prefab:** `SpawnPoint` con tipo `Enemy`, `Resource`, `Document`, `Confiscated`, `PatrolPoint`, `Cover` y un `SecurityCamera` opcional (guía 03).
- `RoomDefinition` (ScriptableObject): arquetipo (6 de Niveles), piso (tema), prefab, tamaño en celdas, si admite cámara.
- Salas clave como prefabs propios: `KeyRoom_Secondary`, `KeyRoom_Final` (con escalera o elevador), `CapsuleRow` (piso 1, refugio) y `MotherRoom` (piso 3).
- Nomenclatura: `PF_Room_<Arquetipo>_<Piso>_<NN>`.

### 2. Datos de configuración

`FloorConfig` (ScriptableObject) por piso y configuración (showcase / completa), con los valores de S-06 y Economía:

| Campo | Showcase | Completa |
|---|---|---|
| Salas procedurales | 6–8 | 8–12 |
| Salas clave secundarias | 2 | 2–3 |
| Enemigos | Escalados por sala (D-34) | Piso 1: 6–9 · piso 2: 9–12 · piso 3: 12–15 |
| Distancia mínima a la sala final | ≥ 3 salas | ≥ 4 salas |
| Cámaras en procedurales | ~1/3 | ~1/3 |
| Salas vacías | ~1/3 | ~1/3 |
| Recursos | Mín.–máx. por piso de Economía, escalados (D-34) | Tabla de Economía |

### 3. Construcción del grafo (`FloorGraphBuilder`)

En este orden, todo con el mismo `System.Random`:

1. **Celdas:** crecimiento aleatorio desde la celda de entrada hasta el número total de salas (procedurales + secundarias + final). En el piso 1 del primer intento, las 4 salas del tutorial van primero en cadena fija (D-38, D-70) y cuentan como procedurales.
2. **Conexiones:** árbol de expansión entre celdas vecinas + ~20% de conexiones extra para formar bucles (rutas de huida, M-14).
3. **Sala final:** una **hoja** (una sola puerta) a distancia ≥ 4 salas de la entrada (≥ 3 en showcase, D-100). Su puerta exige la llave obligatoria. En el piso 3 es la sala de The Mother.
4. **Salas clave secundarias:** 2–3 nodos que no son la entrada ni la final.
5. **Llave obligatoria:** en una sala secundaria. Como la final es una hoja, la ruta a la llave nunca la atraviesa (D-36).
6. **Llaves opcionales:** solo cierran puertas que llevan a hojas sin salas clave (atajos o salas con recursos). La llave opcional nunca está detrás de su propia puerta.
7. **Energía (D-72):** en los pisos 1 y 2, ~1/3 de probabilidad de sala final sin energía (nunca en el primer intento del piso 1 ni en el piso 3); el interruptor va en una sala secundaria.
8. **Cámaras (D-25):** todas las salas clave + ~1/3 de las procedurales.
9. **Enemigos:** cantidad dentro del rango del piso, con tope de 2 × salas ocupadas (D-118); tipos permitidos por piso (D-76); máximo 2 por sala; ~1/3 de salas vacías; **nunca** en la sala de aparición ni en la fila de cápsulas (D-97). En el tutorial, 1 Rastrero fijo en la sala de calderas.
10. **Recursos:** reparte el presupuesto (entre mínimo y máximo de cada recurso) en salas procedurales y en salas de llave opcional. El **mínimo garantizado** nunca queda detrás de una llave opcional (D-79). Modo Historia: +1 al mínimo (1 blíster en munición); si supera al máximo, el máximo sube igual; las llaves no cambian (D-103, D-131).
11. **Documentos de Emilia:** 2 por piso, en salas secundarias. Si el documento ya está registrado en el terminal, la sala recibe 1 recurso extra del tipo más escaso (D-79). Cada piso tiene sus 2 documentos fijos (1–2, 3–4, 5–6) y cada sala secundaria recibe como máximo 1 (D-121).
12. **Documentos de contexto y objetos confiscados:** 1–2 de cada uno en salas procedurales, de los 4 del piso (D-73, D-79).

```csharp
public sealed class FloorLayout
{
    public int Seed, Floor;
    public List<RoomNode> Rooms;          // celda, tipo (Procedural, Secondary, Final, Capsule, Tutorial, Mother), arquetipo
    public List<RoomEdge> Edges;          // a, b, lock (None, MandatoryKey, OptionalKey, Power)
    public int EntranceId, FinalId;
    public bool FinalHasPower;
}
```

### 4. Validación (`LayoutValidator`)

`List<string> Validate(FloorLayout, FloorConfig)` devuelve los errores (lista vacía = válido). Comprueba cada criterio de S-06:

- Exactamente 1 llave obligatoria y 1 sala final (la de The Mother en el piso 3).
- Existe una ruta entrada → llave obligatoria → sala final que no cruza la puerta de la final ni puertas de llave opcional (BFS que trata esas aristas como cerradas).
- Distancia mínima a la final respetada; ninguna escalera o elevador fuera de la final.
- Todas las salas clave con cámara; procedurales con cámara ~1/3 (±1).
- Ninguna sala con más de 2 enemigos; enemigos dentro del rango; ninguno en la sala de aparición.
- Recursos dentro de mínimo y máximo; mínimo garantizado alcanzable sin llaves opcionales.
- 2 documentos de Emilia en salas secundarias (o su recurso extra); ninguno de contexto en salas clave.
- Si falta energía, el interruptor es alcanzable sin la llave obligatoria.

Si falla: `Debug.LogWarning` con la semilla y los errores, nueva semilla. Tras 20 intentos, `FloorLayout` de respaldo hecho a mano por piso.

### 5. Instanciado

`FloorInstantiator`:

1. Instancia cada sala en `celda × tamaño` bajo `FloorRoot`.
2. Coloca puertas y muros según las aristas; configura cerraduras, paneles de hackeo y la escalera o el elevador.
3. Rellena los `SpawnPoint` según el layout (enemigos, recursos, documentos, cámaras).
4. **NavMesh:** un `NavMeshSurface` en `FloorRoot` (*Collect Objects: Children*). `BuildNavMesh()` durante la transición de piso o la reconstrucción, que lo ocultan (D-39). Puertas cerradas con llave y selladas: `NavMeshObstacle` con *Carve*. Fila de cápsulas: `NavMeshModifierVolume` con un área que los enemigos no pueden usar (refugio, D-97).
   - *shortcut:* `BuildNavMesh()` es síncrono y puede congelar la animación de la transición; si se nota, pasar a `UpdateNavMesh(navMeshData)` asíncrono.
5. Mide el tiempo total; objetivo ≤ 5 s (`GDD/07 - Tecnología/03 - Requisitos Técnicos.md`).

### 6. Activación por cercanía

`RoomActivator`: un `BoxCollider` *trigger* por sala. Al entrar, activa la sala actual y sus vecinas del grafo y desactiva el resto (`SetActive(false)`). El NavMesh no se pierde al desactivar. Los temporizadores de reciclaje viven en `RecycleSystem`, no en la sala, para que sigan contando (guía 08).

### 7. Semilla y regeneración

- Semilla nueva cada vez que se entra en un piso, también al continuar (D-101); se escribe en el log y en la consola de depuración para reportar bugs.
- Morir o reiniciar regenera el piso 1 desde la fila de cápsulas; no se vuelve a pisos anteriores.
- Al continuar desde un autoguardado, la sala final guardada es la sala de aparición (sin enemigos dentro) y el resto del piso es nuevo.

## Criterios de aceptación

- Todos los criterios de S-06 (tabla "Criterios de aceptación") para la showcase; para la completa si se activa.
- CP-01: 1 000 semillas por piso y configuración pasan el validador (prueba EditMode, guía 13).
- Generación + NavMesh ≤ 5 s en el equipo mínimo.
- En el primer intento las 4 salas del tutorial son idénticas en todas las partidas; desde el segundo, procedurales.
- Un enemigo con `NavMeshAgent` puede llegar desde cualquier sala a cualquier otra sin puertas cerradas en medio.
