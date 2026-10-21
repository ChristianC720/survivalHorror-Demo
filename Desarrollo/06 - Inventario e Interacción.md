# 06 — Inventario e Interacción (S-05, M-07, M-08, M-12, M-13)

**Objetivo:** inventario de 6 casillas en tiempo real, interacción con `E` (recoger, leer, abrir, interruptores, escalera y elevador), linterna con carga y hackeo por secuencias.

**Responsable:** Programación de jugabilidad.

**Referencias:** `GDD/02 - Diseño/03 - Sistemas.md` (S-05), `GDD/02 - Diseño/02 - Mecánicas.md` (M-07, M-08, M-12, M-13), `GDD/04 - Contenido/04 - Objetos y Coleccionables.md`, `GDD/02 - Diseño/06 - Economía y Recursos.md`, D-36 (llaves), D-71 (5 tipos), D-72 (energía), D-84 (destello), D-97 (sellado).

**Depende de:** guías 02 y 04.

## Pasos

### 1. Datos de objeto

`ItemDefinition` (ScriptableObject) con `ItemType { Cure, Ammo, Stake, MandatoryKey, OptionalKey }`, `maxPerSlot` (1, 12 balas, 1, 1, 1; D-37), `useDuration` (~1.5 s) e icono. Revólver, linterna y anillo **no** son `ItemDefinition`: no ocupan casilla (D-71).

### 2. Modelo de inventario (lógica pura)

`InventoryModel` sin `MonoBehaviour`, probado en EditMode:

- 6 casillas fijas; `TryAdd(item, amount)` llena casillas del mismo tipo hasta su máximo y luego una vacía; devuelve lo que no cupo.
- Inventario lleno + objeto clave (llave) → `TryReplace(slot, item)`: el objeto reemplazado cae al suelo (M-07).
- `Consume(type, amount)`, `Has(type)`, `Clear()` (muerte o reinicio, S-08), `ToSave()` / `FromSave()` (autoguardado, guía 08).
- Una llave se consume al abrir su puerta (D-36).

### 3. Uso en tiempo real

`Inventory` (componente) envuelve el modelo:

- `Tab` abre el terminal del brazo (guía 10) y cambia al mapa `Inventory`. **Nunca** toca `Time.timeScale` (regla global 1).
- Usar, equipar o descartar es una corrutina de ~1.5 s con animación; un golpe la cancela sin consumir (la cura no se usa con la salud completa).
- Descartar instancia el `Pickup` del objeto en el suelo.

### 4. Interacción

`InteractionProbe` en el jugador:

1. Cada fotograma, `OverlapSphere` de ~1.5 m sobre la capa `Interactable` y `Corpse`.
2. Elige el candidato **más cercano al centro de la vista** (menor ángulo con el *forward* de la cámara) con línea de visión (D-27).
3. Muestra el aviso `E` (guía 10) y expone `Current` para `Interact` y para el contexto de remate (guía 04).

Interfaz común:

```csharp
public interface IInteractable
{
    string PromptKey { get; }                 // clave de localización del aviso
    bool CanInteract(PlayerContext ctx);
    void Interact(PlayerContext ctx);
}
```

Implementaciones: `Pickup` (objeto del presupuesto), `DocumentPickup` (registra en el terminal; guía 08), `ConfiscatedItem`, `Door` (con llave, `LOCKED`, o con panel de hackeo), `PowerSwitch` (D-72: arranque ruidoso que alerta), `Stairs` / `Elevator` (transición de piso; `LOCKDOWN` durante caza, `NO POWER` sin energía), `HackPanel`.

### 5. Destello de recogibles (D-84)

Los `Pickup` usan un material con máscara de brillo especular que **solo** se ve bajo la linterna o una luz; sin brillo propio a oscuras. No añadas contornos ni iconos flotantes.

### 6. Linterna (M-12, D-71)

`Flashlight`: `Light` *Spot* con sombras (cuenta como una de las ≤ 4 luces con sombra).

- `F` alterna. Carga 0..1: ~120 s de uso continuo; tras ~3 s apagada se recarga a ~1/30 por segundo (D-124).
- En 0: parpadea (reducible con "Reducción de destellos", D-106) y se apaga; `F` no la enciende durante ~10 s (D-124).
- Al apuntar, el haz se concentra (ángulo menor) (Controles).
- `IsIlluminating(collider)`: el collider está dentro del cono y un `Raycast` lo alcanza; lo usa la percepción (guía 05).
- La carga se lee en el terminal (barra), no en pantalla (D-86).

### 7. Hackeo (M-13)

`HackPanel` + `HackingController`:

- `E` inicia: subestado `Hacking`, mapa `Hacking`, personaje inmóvil en OTS. El juego **no** se pausa.
- Secuencia aleatoria de direcciones mostrada en el terminal del brazo (en la introducción, en la pantalla del teléfono, D-63). Longitudes: sellado rápido 2-3, puerta estándar 4-5, elevador o terminal principal 7-8.
- Entrada correcta: tono. Error: pitido de error y reinicio de la secuencia. `E` abandona. Un golpe la cancela.
- Si un enemigo entra en caza durante la secuencia, continúa (M-13).
- Éxito: `ACCESS GRANTED`; la puerta se abre, se sella (sellado ~30 s o hasta que Lukas cruza, D-97) o el elevador se habilita.
- Fallas de la máquina (M-17, Deseable): `HackPanel.lengthModifier` acorta la secuencia si hay una falla activa.

## Criterios de aceptación

Los de S-05 y además:

- Ninguna acción del inventario detiene a los enemigos.
- Nunca hay más de 6 casillas ocupadas ni una casilla por encima de su máximo (prueba EditMode).
- Usar una llave en su puerta la elimina del inventario.
- Con un cuerpo y una puerta al alcance, `E` actúa sobre el que está al centro de la vista.
- La linterna se agota tras ~120 s encendida, queda ~10 s sin poder encenderse y se llena en ~30 s tras ~3 s apagada.
- Un error de hackeo reinicia la secuencia; un golpe la cancela; el juego nunca se pausa.
