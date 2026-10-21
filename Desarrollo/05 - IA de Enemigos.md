# 05 — IA de Enemigos (S-02)

**Objetivo:** enemigos que patrullan, perciben por vista, luz y ruido, pasan por sospecha, búsqueda y caza, y se comportan según su tipo, sin mostrar nunca su percepción en pantalla.

**Responsable:** Programación de sistemas.

**Referencias:** `GDD/02 - Diseño/03 - Sistemas.md` (S-02), `GDD/04 - Contenido/03 - Enemigos y NPC.md` (fichas y valores, D-66, D-76), `GDD/02 - Diseño/02 - Mecánicas.md` (M-09, M-10, M-12, M-14), D-23, D-24, D-96 (cierre de tránsito), D-97 (salas de aparición).

**Depende de:** guías 01, 02 y 04. El NavMesh lo construye la guía 07; mientras tanto usa una sala de prueba con `NavMeshSurface` horneado a mano.

## Pasos

### 1. Datos

`EnemyDefinition` (ScriptableObject), uno por tipo:

| Campo | Alto | Boca | Rastrero | Mudo |
|---|---|---|---|---|
| Velocidad patrulla / caza (m/s) | 1.2 / 3 | 1 / 2.5 | 1.8 / 4.2 | 0 / estallidos 4.5 (~2 s) |
| Vista | ~15 m | Ninguna | ~8 m | Activación a ~3 m |
| Oído | ~6 m | ~20 m | ~8 m | Activación a ~3 m |
| Reacciona a la luz | Sí | No (ciega) | Sí | Sí: se activa |
| Golpes para matar a Lukas | 2 | 3 | 4 | 2 |
| Disparos para morir | 4 | 3 | 2 | 3 |
| Agarra | Sí | No | No | Sí |
| Pisos | 1, 2, 3 | 2, 3 | 1, 2, 3 | 3 |

Todos son valores iniciales (D-66, D-76). Añade el multiplicador de detección del Modo Historia (D-103).

### 2. Prefab base

`PF_Enemy_<Tipo>`: `NavMeshAgent`, `EnemyBrain`, `Perception`, `EnemyHealth` (cuenta impactos), `WeakPoint` hijo, `Animator` (guía 12), `AudioSource` 3D (guía 11). Capa `Enemy`.

### 3. Percepción

`Perception` calcula cada ~0.2 s (no cada fotograma) una **alerta acumulada** 0..1:

- **Vista:** el jugador está dentro del radio y del ángulo de visión, y un `Raycast` a pecho y cabeza no choca con la capa `Occluder` ni con geometría (la visión no atraviesa sólidos, M-09). Agachado detrás de una cobertura sólida = no visto.
- **Luz:** si el haz de la linterna toca al enemigo (`Linterna.IsIlluminating(collider)`, guía 06), la alerta sube **el doble de rápido** (M-12). La Boca lo ignora; el Mudo se activa.
- **Ruido:** `NoiseEmitter.Emit(pos, radio)` publica un evento; cada enemigo con oído suficiente y sin puerta sellada en medio recibe un punto de interés.
- Umbrales de la alerta: sospecha → búsqueda → caza. La alerta baja sola sin estímulos.

### 4. Máquina de estados por enemigo

`EnemyBrain` con estados: `Patrol`, `Suspicious`, `Search`, `Hunt`, `Attack`, `Stunned`, `Dead` (+ `Dormant` del Mudo).

```text
Patrol → Suspicious (mira hacia el estímulo) → Search (va al último punto conocido)
      → Hunt (persigue con la velocidad de caza) → Attack / Grab
Hunt → pierde el objetivo → Search → Patrol
```

- **Huida (M-14):** al perder al jugador tras una puerta, persigue unos segundos o golpea la puerta y vuelve a patrulla. Una puerta sellada (hackeo) tiene `NavMeshObstacle` *carve* y un temporizador de ~30 s para romperse (D-97).
- **Boca:** al detectar, `Scream()`: todos los enemigos de la sala y las adyacentes pasan a `Search` hacia la posición de la Boca (D-64). Acertarle en la boca durante el grito lo interrumpe.
- **Rastrero:** en `Hunt` elige un punto lateral al jugador (flanqueo) antes de atacar.
- **Mudo:** `Dormant` (parece un cadáver) hasta que el jugador está a ~3 m o lo ilumina; después alterna estallidos de ~2 s y pausas.
- **Aturdido:** empujón, punzón (2 s) o punto débil interrumpen el estado actual.
- Cada cambio de estado emite `EnemyAlertChanged(enemy, state)`; S-11 reproduce el sonido de ese estado (M-10).

### 5. Director de alerta

`AlertDirector` (uno por piso):

- Calcula el **estado más alto** de los enemigos de la sala actual y de las adyacentes con ruta de persecución (alcance de M-03) y lo publica con `HighestAlertChanged` para S-01 y S-11 (D-23).
- Lleva la lista de enemigos en caza: `AnyHunting` lo usan el autoguardado (S-07) y el **cierre de tránsito** (`LOCKDOWN` en escalera y elevador mientras haya caza, D-96).
- Al recibir `AttentionFull(pos)` de S-01: los enemigos cercanos en `Patrol` pasan a `Suspicious` y luego a `Search` hacia `pos`; nunca baja a los que ya buscan o cazan (D-24).
- Activa la IA solo de las salas activas (guía 07); ≤ 6 enemigos con IA a la vez (`GDD/07 - Tecnología/03 - Requisitos Técnicos.md`).

### 6. Ataques

Cada tipo ataca con una animación con evento (*Animation Event*) que llama a `PlayerHealth.TakeHit(definition)` o a `GrabResolver.Resolve(this)` (guía 04). El brazo recolector de The Mother usa el mismo `GrabResolver` (guía 09).

## Criterios de aceptación

- Un jugador agachado detrás de una cobertura sólida no es detectado por la vista.
- Todo cambio de estado de alerta produce una señal audible distinta (con la guía 11).
- La interfaz no muestra la percepción de los enemigos (D-02).
- Al llenarse la atención de The Mother, ningún enemigo cercano en patrulla permanece en patrulla.
- Con un enemigo en caza, la escalera y el elevador muestran `LOCKDOWN` y no se pueden usar; al terminar la caza se abren.
- Iluminar a un Alto lo lleva a caza en la mitad de tiempo que sin luz; iluminar a la Boca no cambia nada.
- Ninguna sala tiene más de 2 enemigos activos (con la guía 07).
