# 13 — Pruebas y Build

**Objetivo:** pruebas automáticas de la lógica crítica, perfilado contra los objetivos de rendimiento y la build de entrega.

**Responsable:** Christian Coronel Palma (pruebas automáticas y build); Lenin Ramón López Córdova y todo el equipo (pruebas manuales).

**Referencias:** `GDD/09 - Calidad/01 - Criterios de Calidad.md` (definición de terminado, D-113), `GDD/09 - Calidad/02 - Pruebas.md` (CP-01..CP-06), `GDD/09 - Calidad/03 - Bugs y Seguimiento.md`, `GDD/09 - Calidad/04 - Criterios de Aceptación.md`, `GDD/07 - Tecnología/03 - Requisitos Técnicos.md` (D-112).

**Depende de:** guía 00 (asmdefs de pruebas).

## Pasos

### 1. Pruebas EditMode (lógica pura)

*Window → General → Test Runner → EditMode*. Una clase de prueba por modelo, en `Tests/EditMode/`:

| Prueba | Qué comprueba | Guía |
|---|---|---|
| `GameStateRulesTests` | Solo se permiten las transiciones de la tabla del GDD | 01 |
| `StaminaModelTests` | Costos, agotamiento de 2 s, regeneración tras 3 s | 02 |
| `HealthModelTests` | Golpes para matar por tipo, cura no permitida en Fine, sin regeneración | 02 |
| `InventoryModelTests` | 6 casillas, máximos por casilla, reemplazo con inventario lleno, `Clear` | 06 |
| `LayoutValidatorTests` | **CP-01:** 1 000 semillas por piso y configuración sin errores | 07 |
| `SaveRoundTripTests` | `SlotData` → JSON → `SlotData` sin pérdida; migración v1 → v2 | 08 |
| `ExitCostTests` | Las 4 filas de la tabla de salida, incluido el primer intento | 08 |
| `DeathCounterTests` | Primer intento no suma; reinicio en la sala final del piso 2; quinta muerte = Asimilación | 08 |

Ejemplo de CP-01:

```csharp
[Test]
public void EverySeedProducesAValidFloor([Values(1, 2, 3)] int floor, [Values(false, true)] bool showcase)
{
    var config = TestConfigs.Floor(floor, showcase);
    for (int seed = 0; seed < 1000; seed++)
    {
        var layout = FloorGraphBuilder.Build(seed, config, firstAttempt: seed % 2 == 0, registeredDocs: TestDocs.None);
        var errors = LayoutValidator.Validate(layout, config);
        Assert.IsEmpty(errors, $"Piso {floor}, semilla {seed}: {string.Join("; ", errors)}");
    }
}
```

Corre las pruebas EditMode antes de cada *pull request* (definición de terminado).

### 2. Pruebas PlayMode (integración)

En `Tests/PlayMode/`, pocas y lentas:

- Generar un piso real, construir el NavMesh y comprobar con `NavMesh.CalculatePath` que existe camino desde la entrada hasta la llave y la sala final.
- Medir el tiempo de generación + NavMesh (objetivo ≤ 5 s).

### 3. Escenas de prueba

`Scenes/Tests/`: `Test_Player`, `Test_Combat` (un Alto y munición), `Test_Surveillance` (sala con cámara y un enemigo adyacente), `Test_Generation` (botón para regenerar con semilla), `Test_Boss`. Una escena de prueba por sistema; nadie edita la de otro sin avisar.

### 4. Herramientas de depuración (solo en *Development Build*)

Envueltas en `#if DEVELOPMENT_BUILD || UNITY_EDITOR`:

- Mostrar la semilla del piso, el estado de alerta de cada enemigo y el medidor de atención de The Mother en la consola.
- Teclas para: regenerar el piso, dar munición y curas, ir al piso 2 o 3, dar los 6 documentos de Emilia, matar al jugador.
- Nunca se incluyen en la build de entrega.

### 5. Perfilado (semana de pruebas)

1. *Build Profiles → Windows* con *Development Build* y *Autoconnect Profiler*.
2. Jugar el piso 3 con la linterna y 2 enemigos en caza (CP-06); revisar CPU, GPU, *batches* y memoria en el Profiler y el Memory Profiler.
3. Comparar con los presupuestos de `GDD/07 - Tecnología/03 - Requisitos Técnicos.md` y actualizar los requisitos con los valores reales (D-112).

### 6. Build de entrega

1. Congelación de funciones el 2026-11-15; desde entonces solo correcciones (`GDD/08 - Producción/05 - Gestión de Cambios.md`).
2. *Player Settings:* IL2CPP, x86_64, sin *Development Build*, versión `1.0.0`.
3. Escenas en orden: `Bootstrap` (0), `MainMenu`, `Intro`, `Floor`, `Cinematics`.
4. Compila en `Build/IdentityDissolution_v1.0/` (ignorado por Git) y comprueba tamaño ≤ 2 GB.
5. Prueba la build en un equipo limpio sin Unity: Nueva partida → créditos en la showcase (CP-02..CP-05).
6. Merge de `develop` a `main`, etiqueta `v1.0-demo`, comprime la carpeta de la build y súbela como *GitHub Release*.
7. Actualiza el GDD: estado del proyecto, versión del GDD e `GDD/10 - Referencia/04 - Historial de Versiones.md`.

## Criterios de aceptación

- Todas las pruebas EditMode pasan en `develop`.
- CP-01..CP-06 marcados como correctos en `GDD/09 - Calidad/02 - Pruebas.md`.
- Sin bugs críticos ni altos abiertos.
- La build de entrega arranca en un equipo sin Unity y no incluye herramientas de depuración.
