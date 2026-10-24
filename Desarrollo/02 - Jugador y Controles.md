# 02 — Jugador y Controles

**Objetivo:** Lukas se mueve, corre, se agacha e interactúa con las teclas de Controles, con resistencia (S-04) y salud por estados (M-05).

**Responsable:** Christian Coronel Palma.

**Referencias:** `GDD/02 - Diseño/04 - Controles.md` (mapa y contextos), `GDD/02 - Diseño/03 - Sistemas.md` (S-03, S-04), `GDD/02 - Diseño/02 - Mecánicas.md` (M-05, M-06, M-09), `GDD/04 - Contenido/03 - Enemigos y NPC.md` (velocidades de Lukas, D-76).

**Depende de:** guía 01.

## Pasos

### 1. Asset de entrada

Crea `Input/IdentityDissolution.inputactions` con **un mapa por contexto** de Controles. Así ninguna tecla tiene dos funciones en el mismo contexto:

| Mapa | Acciones |
|---|---|
| `Exploration` | Move (WASD), Look (ratón), Run (Shift izq., mantener), Crouch (Ctrl izq.), Aim (clic der., mantener), Attack (clic izq.), Reload (R), Interact (E), Flashlight (F), Surveillance (Q), Terminal (Tab), Pause (Esc) |
| `Inventory` | Point, UseSlot (clic izq.), DiscardSlot (clic der.), Terminal (Tab), Pause (Esc) — Move y Look siguen activos |
| `Hacking` | Up/Down/Left/Right (WASD y flechas), Abort (E), Pause (Esc) |
| `Surveillance` | MoveSlow (WASD), Exit (Q), Pause (Esc) |
| `Cinematic` | Skip (Espacio: pulsar y mantener) |
| `Menu` | Navigate, Submit, Cancel (Esc) |

- Mando (D-120): crea los *control schemes* `KeyboardMouse` y `Gamepad` y añade a cada acción el enlace de la columna Control de Controles. El mando no se reasigna.
- Activa *Generate C# Class* para tener `IdentityDissolutionInput` tipado.
- `InputRouter` (Core) habilita un solo mapa de "acciones" a la vez según `GameState` y `PlaySubstate`; `Move` y `Look` pueden estar en varios mapas.
- El contexto **Sobre un cuerpo** no es un mapa: `Attack` e `Interact` del mapa `Exploration` consultan a `InteractionProbe` (guía 06) qué hay al centro de la vista.

### 2. Controlador de movimiento

`PlayerController` con `CharacterController` (no Rigidbody: el movimiento es preciso y no necesita física).

- Velocidades de `SO_Balance` (D-76): agachado ~0.8 m/s, caminando ~1.5 m/s, corriendo ~4 m/s.
- El movimiento es relativo a la cámara (guía 03); el personaje gira hacia la dirección de movimiento y, al apuntar, hacia la mira.
- Multiplicadores de velocidad: Herido ×0.85, Crítico ×0.7 (M-05); apuntando, velocidad de caminar reducida (M-01); vigilancia, "muy lento" (M-02).
- Agacharse: alternar por defecto; modo mantener desde Opciones (guía 10). Baja la altura del `CharacterController`.
- Ruido emitido (S-02): agachado 0, caminar bajo, correr alto. Publica el nivel con `NoiseEmitter.Emit(position, radius)`; la IA lo escucha (guía 05).

### 3. Resistencia (S-04)

Clase pura `StaminaModel` + componente `Stamina`:

```csharp
public sealed class StaminaModel
{
    public float Value { get; private set; } = 1f;          // 0..1
    public bool Exhausted => exhaustedTimer > 0f;
    float sinceLastUse, exhaustedTimer;
    readonly StaminaSettings s;                              // costos y tiempos de SO_Balance (D-37)

    public StaminaModel(StaminaSettings settings) => s = settings;

    public bool TrySpend(float cost)                         // empujón 0.35, golpe en el suelo 0.25
    {
        if (Exhausted || Value < cost) return false;         // el llamador reproduce el jadeo
        Value -= cost; sinceLastUse = 0f;
        if (Value <= 0f) { Value = 0f; exhaustedTimer = s.exhaustDuration; } // 2 s
        return true;
    }

    public bool TickRun(float dt) { /* ~0.10 por segundo; false si no alcanza */ }

    public void Tick(float dt)
    {
        exhaustedTimer = Mathf.Max(0f, exhaustedTimer - dt);
        sinceLastUse += dt;
        if (sinceLastUse >= s.regenDelay) Value = Mathf.Min(1f, Value + s.regenPerSecond * dt); // tras 3 s
    }
}
```

- Regeneración: tras 3 s sin correr, empujar ni rematar, +25% por segundo (~4 s de 0 a 100%, D-123).
- Disparar **no** consume resistencia; caminar no interrumpe la regeneración (S-04).
- Sin barra en pantalla: el jadeo es la señal (D-86). `Stamina` emite `GameEvents.Panted` para S-11.
- Pruebas EditMode: costo insuficiente, agotamiento de 2 s, regeneración tras 3 s.

### 4. Salud (M-05)

Clase pura `HealthModel` con `enum HealthState { Fine, Injured, Critical, Dead }`:

- Los enemigos matan en 2-4 golpes según su tipo (D-66): el Alto y el Mudo en 2, la Boca en 3, el Rastrero en 4. Propuesta de implementación: **60 puntos ocultos** (divisible entre 2, 3, 4 y 5) con umbrales Fine 41–60, Injured 21–40, Critical 1–20; cada golpe quita `60 / golpesParaMatar`. Ejemplo: el Rastrero lleva a Lukas por Fine → Injured → Critical → Dead.
- Sin regeneración. La cura sube al máximo del estado siguiente; en Fine con daño sube a 60 y en Fine a 60 no se permite (D-119). Un agarre sin punzón baja al máximo del estado inferior, o mata en Critical (M-11).
- Modo Historia (D-103, D-116): daño recibido ×0.5, así que cada enemigo necesita el doble de golpes (Alto y Mudo 4, Boca 6, Rastrero 8). Usa 120 puntos en lugar de 60 para que todo sea entero.
- Al llegar a `Dead`, `GameEvents.RaisePlayerDied(cause)`.
- La postura y el electrocardiograma del terminal leen `HealthState` (guías 10 y 12).

### 5. Prefab del jugador

`PF_Player`: `CharacterController`, `PlayerController`, `Stamina`, `Health`, `InteractionProbe` (guía 06), `NoiseEmitter`, `Revolver` (guía 04), la linterna como `Light` *Spot* hija de la mano (guía 06) y un `Transform` `CameraTarget` a la altura del hombro (guía 03). Capa `Player`.

## Criterios de aceptación

- Las velocidades medidas en el editor coinciden con D-76 (±0.1 m/s) y bajan un 15% y un 30% en Herido y Crítico.
- No se puede correr mientras se apunta ni durante el agotamiento.
- Una acción sin resistencia suficiente no se ejecuta y suena el jadeo (S-04).
- La salud nunca sube sola; una cura con la salud completa no se consume.
- Con el inventario abierto (mapa `Inventory`), `Clic izquierdo` nunca dispara ni empuja.
