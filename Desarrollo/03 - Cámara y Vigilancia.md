# 03 — Cámara y Vigilancia (S-01)

**Objetivo:** cámara OTS por defecto, apuntado y vista de vigilancia isométrica con ruido, retraso, señal escalonada, corte, bloqueo y la atención oculta de The Mother.

**Responsable:** Programación de jugabilidad.

**Referencias:** `GDD/02 - Diseño/03 - Sistemas.md` (S-01), `GDD/02 - Diseño/02 - Mecánicas.md` (M-01, M-02, M-03), decisiones D-23 (señal escalonada), D-24 y D-98 (atención), D-25 (cobertura), D-28 (fase 2 del jefe).

**Depende de:** guía 02 (OTS); guías 05 y 07 (vigilancia).

## Parte A — OTS y apuntado (semana 1)

1. En la escena `Floor`, a la `Main Camera` añade `CinemachineBrain`.
2. Crea `CM_OTS`: `CinemachineCamera` con *Tracking Target* = `CameraTarget` del jugador y *Position Control* = **Third Person Follow** (`CinemachineThirdPersonFollow`): *Shoulder Offset* ~(0.5, 0.1, 0), *Camera Distance* ~2.5, *Camera Collision Filter* = capas de geometría (la cámara nunca atraviesa paredes, S-01).
3. Crea `CM_Aim`: igual, con *Camera Distance* ~1.2, *Shoulder Offset* más cerrado y FOV menor. Prioridad mayor que `CM_OTS` solo mientras se mantiene `Aim`.
4. `CameraLook` rota `CameraTarget` con `Look` (sensibilidad de mirar y de apuntar por separado, eje vertical invertible; guía 10). Limita el *pitch* a ±70°.
5. La retícula de punto (UI) solo aparece mientras `CM_Aim` está activa (D-86). Con asistencia de apuntado (D-104), la sensibilidad baja ~40% cuando un `SphereCast` desde el centro toca la capa `Enemy`.
6. Al recibir daño, `CinemachineImpulseSource` sacude la cámara; desactívalo con "Reducción de movimiento" (D-106).

## Parte B — Vista de vigilancia (semana 4)

### Diseño técnico

La vista de vigilancia **no** mueve la cámara principal. Cada sala con cámara (D-25) tiene un `SecurityCamera` con una `Camera` desactivada en ángulo isométrico (proyección ortográfica, ~45° de rotación y ~30° de inclinación) que renderiza a una `RenderTexture` de baja resolución (~320 × 180). Una UI a pantalla completa muestra esa textura con un shader CRT verde (D-82). Así:

- Solo se ve lo que está dentro del campo de esa cámara (no es omnisciente, M-02).
- Solo existe un piso a la vez porque solo existe el piso actual (D-101).
- El **retraso** se obtiene con un búfer circular de ~8 `RenderTexture`: se captura una cada N fotogramas y se muestra la más antigua.

### Componentes

| Componente | Responsabilidad |
|---|---|
| `SecurityCamera` (en el prefab de sala) | `Camera` isométrica desactivada, `RenderTexture` y bandera `Hostile` (fase 2 del jefe) |
| `SurveillanceSystem` (en el jugador) | Entrar y salir con `Q`; cortes; bloqueo; señal escalonada; búfer de retraso |
| `MotherAttention` (uno por piso) | Medidor oculto: +1/5 por segundo en vigilancia, −1/15 por segundo fuera; al llegar a 1 emite `AttentionFull(posición del jugador)` y vuelve a 0 |
| `SurveillanceView` (UI) | `RawImage` a pantalla completa con el material CRT; parámetros `_Noise`, `_ScanlineIntensity`, `_Glitch` |

### Reglas a implementar

1. **Entrar (`Q`):** solo si la sala actual tiene `SecurityCamera` activa, el estado de alerta más alto dentro del alcance de M-03 no es caza y el jefe no está en fase 2. Cambia el subestado a `Surveillance` y el mapa de entrada a `Surveillance` (solo quieto o caminar muy lento).
2. **Señal escalonada (D-23)** según `GameEvents.HighestAlertChanged`:

| Estado más alto | Señal |
|---|---|
| Patrulla o sospecha | Base: ruido y retraso base |
| Búsqueda | Degradada: más `_Noise` y el doble de retraso |
| Caza | Cortada y bloqueada |

3. **Cortes:** recibir daño, correr, apuntar, empujar, cruzar a una sala sin cámara o un enemigo que entra en caza devuelven a OTS al instante.
4. **Bloqueo (M-03):** mientras haya un enemigo en caza en la sala o en una adyacente con ruta de persecución, `Q` no hace nada. Se levanta al eliminarlo, al salir de la caza o al cruzar una puerta. Si al levantarse hay enemigos en búsqueda, la señal vuelve degradada.
5. **Atención (D-24, D-98):** un `MotherAttention` por piso; no se vacía al cambiar de sala. "Enemigos cercanos" = los de la sala actual y las adyacentes (consulta el grafo del piso, guía 07). **Nunca** se muestra en la interfaz.
6. **Fase 2 del jefe (D-28):** con `SecurityCamera.Hostile = true` en la sala, `Q` no activa la vista.
7. **Accesibilidad (D-106):** "Reducción de efectos CRT y glitch" baja `_Noise` y `_Glitch` pero conserva una diferencia visible entre base y degradada (por ejemplo, el retraso y una etiqueta `SIGNAL: DEGRADED` en la esquina).

### Depuración

En builds de desarrollo, una tecla de depuración muestra el valor de `MotherAttention` en la consola (criterio de S-01 "comprobable en una compilación de depuración").

## Criterios de aceptación

Los de S-01 en `GDD/02 - Diseño/03 - Sistemas.md`, en especial:

- La vigilancia no se activa con un enemigo en caza dentro del alcance de M-03 ni en la fase 2 del jefe.
- Se corta de inmediato al recibir daño o cuando un enemigo entra en caza.
- En sospecha la señal es idéntica a la base; en búsqueda hay más ruido y más retraso sin cortarse.
- Tras ~5 s de vigilancia continua, los enemigos cercanos en patrulla pasan a sospecha y después a búsqueda hacia el jugador.
- Ningún elemento de la interfaz muestra el medidor de atención.
- La cámara OTS no atraviesa la geometría.
