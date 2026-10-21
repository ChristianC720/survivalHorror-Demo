# 01 — Arquitectura y Núcleo

**Objetivo:** tener el esqueleto que todos los sistemas usan: escena `Bootstrap` con servicios persistentes, máquina de estados del juego, eventos entre sistemas, carga de escenas y pausa.

**Responsable:** Programación de sistemas.

**Referencias:** `GDD/07 - Tecnología/01 - Arquitectura Técnica.md` (D-108), `GDD/02 - Diseño/07 - Reglas y Estados del Juego.md` (estados, transiciones y reglas globales), `GDD/02 - Diseño/03 - Sistemas.md` (tabla "Relaciones entre sistemas").

**Depende de:** guía 00.

## Pasos

### 1. Escena `Bootstrap`

1. Crea un GameObject `Services` con los componentes `GameStateMachine`, `SceneLoader`, `SaveService` (guía 08), `SettingsService` (guía 10) y `AudioService` (guía 11). Al principio basta con los dos primeros.
2. Añade un script `Bootstrap` que en `Awake` haga `DontDestroyOnLoad(gameObject)` y cargue `MainMenu`.
3. Añade el `EventSystem` de UI con *Input System UI Input Module* aquí, una sola vez.
4. **Atajo de desarrollo:** un script `[RuntimeInitializeOnLoadMethod]` que, si la escena abierta no es `Bootstrap`, cargue `Bootstrap` de forma aditiva primero. Así se puede pulsar Play desde cualquier escena de prueba.

### 2. Estados del juego

Copia los estados de "Estados principales" del GDD en un `enum` y una máquina que valide las transiciones de la tabla "Transiciones":

```csharp
public enum GameState
{
    MainMenu, Intro, Loading, Playing, Paused, FloorTransition,
    Death, Reconstruction, Victory, FinalDecision,
    RelayBasic, RelayComplete, Ambiguous, Assimilation, Credits
}

public enum PlaySubstate { Exploration, Surveillance, AlertCombat, Hacking }

public sealed class GameStateMachine : MonoBehaviour
{
    public GameState State { get; private set; } = GameState.MainMenu;
    public PlaySubstate Substate { get; private set; }

    public bool TryChange(GameState next)
    {
        if (!GameStateRules.IsAllowed(State, next)) { Debug.LogError($"Transición inválida {State} → {next}"); return false; }
        var previous = State;
        State = next;
        GameEvents.RaiseStateChanged(previous, next);
        return true;
    }
}
```

- `GameStateRules` es una clase **estática pura** con un diccionario `estado → estados permitidos`, copiado de la tabla del GDD. Pruébala en EditMode (guía 13).
- Los subestados de Jugando (Exploración, Vigilancia, Alerta/Combate, Hackeo) los cambia el sistema dueño (S-01, S-02, M-13) con `SetSubstate`.
- Inventario y terminal **no** son estados (GDD: "Abrir el inventario o el terminal del brazo no es un subestado").

### 3. Eventos entre sistemas

Una clase estática `GameEvents` con un evento C# por fila de "Relaciones entre sistemas" (D-108). Ejemplo de arranque:

```csharp
public static class GameEvents
{
    public static event Action<GameState, GameState> StateChanged;
    public static event Action<Vector3> AttentionFull;                 // S-01 → S-02 (D-24)
    public static event Action<AlertState> HighestAlertChanged;        // S-02 → S-01, S-11 (D-23)
    public static event Action<EnemyBrain> EnemyKilled;                // S-03 → S-09
    public static event Action<DeathCause> PlayerDied;                 // S-03 → S-08
    public static event Action<int> Reconstructed;                     // S-08 → S-07 (código R-H)
    public static event Action<int> FinalRoomReached;                  // S-06 → S-07, S-08

    public static void RaiseStateChanged(GameState a, GameState b) => StateChanged?.Invoke(a, b);
    // Un Raise… por evento.

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetAll() { StateChanged = null; AttentionFull = null; /* … todos */ }
}
```

- `ResetAll` evita suscripciones fantasma con *Enter Play Mode Options* (recarga de dominio desactivada).
- Suscríbete en `OnEnable` y desuscríbete en `OnDisable`, siempre.
- Añade eventos solo cuando un sistema los necesite; el nombre sigue la tabla del GDD.

### 4. Carga de escenas

`SceneLoader` con `LoadSceneAsync` (modo *Single* para `MainMenu`, `Intro` y `Floor`; *Additive* para `Cinematics`). Expone `IsLoading` y un evento `LoadFinished`. La **Transición de piso** (mínimo 5 s, D-39) espera a que la carga **y** la generación (guía 07) terminen:

```csharp
IEnumerator FloorTransition(int nextFloor)
{
    float start = Time.unscaledTime;
    yield return generator.GenerateAsync(nextFloor);      // grafo + prefabs + NavMesh (guía 07)
    while (Time.unscaledTime - start < 5f) yield return null; // D-39: nunca menos de 5 s
    stateMachine.TryChange(GameState.Playing);
}
```

### 5. Pausa

- `Esc` (mapa `UI` de la guía 02) llama a `PauseController.Toggle()`.
- Pausar: `Time.timeScale = 0`, `AudioListener.pause = true` salvo la música del menú, cambia a `GameState.Paused` y activa el mapa de entrada `Menu`.
- **Solo la pausa** toca `Time.timeScale` (regla global 1). Para animaciones de UI durante la pausa usa `Time.unscaledDeltaTime`.
- En la introducción y en la decisión final la pausa no ofrece "Reiniciar" (D-63, D-92).

### 6. Datos de configuración

Crea los ScriptableObjects raíz en `Data/`:

- `SO_GameConfig`: configuración activa (Showcase / Completa, D-34) y referencias a los demás.
- `SO_Balance`: valores de D-37, D-66, D-76 (velocidades, costos de resistencia, tiempos). Cada campo con `[Tooltip("D-xx …")]`.
- `SO_ModeSurvival`, `SO_ModeStory`: multiplicadores del Modo Historia (D-103: daño recibido ×0.5, detección más lenta, +1 al mínimo de cada recurso salvo llaves, 1 blíster en munición, D-131).

Los sistemas reciben estos assets por `[SerializeField]`; nunca valores escritos en el código.

## Criterios de aceptación

- Pulsar Play en cualquier escena carga `Bootstrap` primero y no duplica servicios.
- `GameStateRules` rechaza toda transición que no esté en la tabla del GDD (prueba EditMode).
- `Esc` pausa y despausa; con la pausa, la IA, los temporizadores y las animaciones se detienen; con el inventario abierto, no.
- Una transición de piso simulada dura ≥ 5 s aunque la carga tarde 0 s, y espera si tarda más.
- Ningún sistema guarda una referencia directa a otro sistema de otra carpeta salvo a servicios de `Bootstrap`.
