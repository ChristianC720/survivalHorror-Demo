# 08 — Muerte, Reconstrucción y Persistencia (S-07, S-08, S-09)

**Objetivo:** el ciclo de muerte y reconstrucción, el terminal del brazo persistente, las 3 ranuras con autoguardado y costo de salida, y el reciclaje de cuerpos.

**Responsable:** Programación de sistemas.

**Referencias:** `GDD/02 - Diseño/03 - Sistemas.md` (S-07, S-08, S-09), `GDD/02 - Diseño/02 - Mecánicas.md` (M-16, M-17, M-18, M-20), `GDD/02 - Diseño/07 - Reglas y Estados del Juego.md` (reglas globales 3–13 y casos especiales), `GDD/06 - Experiencia/04 - Guardado y Configuración.md`, `GDD/07 - Tecnología/04 - Datos y Persistencia.md` (D-109), D-30, D-31, D-32, D-35, D-43, D-93, D-94, D-96, D-99.

**Depende de:** guías 01, 04, 06 y 07.

## Parte A — Guardado (S-07)

### 1. Clases de datos

Copia el modelo de `GDD/07 - Tecnología/04 - Datos y Persistencia.md` en clases `[Serializable]`: `GlobalData`, `SlotData`, `TerminalData`, `AttemptSave`, `InventorySlotData`.

- `JsonUtility` no serializa `null` en clases anidadas ni diccionarios: usa `bool hasAttemptSave` + `AttemptSave attemptSave` y listas en lugar de diccionarios.
- Los objetos se guardan por **id** (nombre del ScriptableObject), nunca por índice.

### 2. Escritura atómica

```csharp
public static void WriteAtomic(string path, string json)
{
    string tmp = path + ".tmp", bak = path + ".bak";
    File.WriteAllText(tmp, json);
    if (File.Exists(path)) File.Replace(tmp, path, bak);   // conserva el anterior como .bak
    else File.Move(tmp, path);
}
```

- Envuelve la llamada en `try/catch`: si falla, el terminal muestra `SYNC FAILED`, se conserva el archivo anterior y se reintenta en el siguiente cambio (`GDD/06 - Experiencia/04 - Guardado y Configuración.md`).
- Al cargar: lee, comprueba `saveVersion`, aplica `SaveMigrator`; si no se puede leer, la ranura aparece dañada (ofrece `.bak` o borrar).

### 3. Cuándo se guarda

| Momento | Qué | Regla |
|---|---|---|
| Cambio en el terminal (documento, bestiario, objeto confiscado, falla, anillo) | `SlotData` | Inmediato; antes de procesar una muerte si coinciden |
| Llegada a la sala final de los pisos 1 y 2 | `attemptSave` (piso, inventario) | Solo sin enemigos en caza; si los hay, **pendiente** hasta que `AlertDirector.AnyHunting` sea falso (D-30) |
| Inicio de la decisión final | `attemptSave` especial | D-92 |
| Salida confirmada | `confirmedExit`, costo | Antes de cerrar (S-07) |
| Salir de Opciones | `GlobalData` | Guía 10 |

Muestra "sincronizando…" en el terminal sin quitar el control (D-30). La sala secundaria y la sala de The Mother nunca guardan.

### 4. Costo de salir (lógica pura)

```csharp
public static ExitCost GetExitCost(bool anyHunting, bool inSavedFinalRoom)
    => anyHunting ? ExitCost.FullDeath
     : inSavedFinalRoom ? ExitCost.None
     : ExitCost.Calm;               // suma 1 al contador y a la degradación, sube el código (D-94)
```

- Un cierre inesperado (no hay `confirmedExit` al arrancar) no cuesta nada (D-96).
- En el primer intento las salidas con costo no suman al contador, sí a la degradación (regla global 8).
- Pruebas EditMode con las 4 filas de la tabla de salida.

## Parte B — Ciclo de muerte (S-08)

### 5. `DeathCycle`

Escucha `PlayerDied` y la opción "Reiniciar". Ambos siguen **el mismo proceso** (D-32), en este orden:

1. `SaveService` guarda cualquier registro pendiente.
2. Si no es el primer intento: `deathCounter++` (si M-18 entra). Siempre: `degradation++` (si M-18 entra).
3. Si `deathCounter == 5` (M-18): estado `Assimilation` → cinemática → créditos. Fin.
4. `Inventory.Clear()`, descartar `attemptSave`.
5. Estado `Reconstruction`: reproducir la cinemática corta (saltable con `Espacio`) **mientras** se regenera el piso 1 (guía 07). Si se salta antes de que termine la generación, pantalla de arranque del terminal (D-96).
6. `subjectCode++` y `Reconstructed` (R-H-07 en la primera, D-43); variante de cuerpo ajeno +1 hasta la última (3–5 variantes).
7. Revólver descargado en la bandeja junto a la cápsula (D-74); salud en Fine, resistencia al 100%.
8. Estado `Playing`.

Reglas extra:

- Llegar a la sala final del piso 2 (sin caza) pone `deathCounter = 0`; la primera vez reproduce el hallazgo, no saltable (D-56, D-67).
- La muerte guionizada de la introducción no suma nada (D-63).
- Ciclo nuevo tras un final: contador y degradación a 0, terminal y código conservados (D-93).
- `ShouldTriggerAssimilation`, `OnDeath`, `OnFloor2Final` como lógica pura en `DeathCounterModel`, probada en EditMode.

### 6. Cinemáticas

Timelines en la escena `Cinematics` (aditiva). `CinematicPlayer` aplica D-91: la corta siempre se salta con `Espacio`; las narrativas solo si están en `GlobalData.seenCinematics`, manteniendo `Espacio` ~1 s; el hallazgo y la transición nunca.

### 7. Degradación (M-18, Deseable)

Un valor entero que alimenta: intensidad del *Volume* de corrupción CRT morada (guía 12), distorsión del sonido vocal y nivel del latido del laboratorio (guía 11). **Tope:** nunca tapa la interfaz crítica ni las señales de M-10 (D-29). Si se recorta M-18, deja el campo a 0.

## Parte C — Reciclaje (S-09)

### 8. `Corpse` y `RecycleSystem`

- Al morir un enemigo (`EnemyKilled`), su modelo pasa a *ragdoll* o pose de muerte con un `Corpse` (capa `Corpse`) con `canRecycle` = no es reciclado **y** no está en la sala de The Mother (D-28).
- `RecycleSystem` (uno por piso, fuera de las salas) lleva un temporizador por cuerpo: a los ~40 s, aviso orgánico de ~3 s (convulsión + crujidos, S-11) (D-99).
- Tras el aviso, el cuerpo espera a que **el jugador no esté en esa sala**; entonces desaparece y reaparece como reciclado en **otra sala del mismo piso**, nunca en la sala del jugador ni en su campo de visión. El reciclado es un 25% más rápido, muere con 2 disparos (1 el Rastrero) y no vuelve a reciclarse.
- Rematar (`Corpse.MarkFinished()`, guía 04) cancela su temporizador.
- La pausa congela los temporizadores (regla global 2): usa `Time.deltaTime`, no tiempo real.

## Criterios de aceptación

Los de S-07, S-08 y S-09, en especial:

- Tras la reconstrucción: inventario vacío, terminal intacto, código R-H siguiente, inicio del ciclo en el piso 1.
- "Reiniciar" produce el mismo resultado que una muerte.
- Llegar perseguido a la sala final no guarda hasta que termina la caza.
- Cada fila de la tabla de salida aplica exactamente su costo; un cierre inesperado no suma nada (CP-03).
- Matar el proceso durante una escritura nunca deja una ranura ilegible.
- Ningún enemigo se recicla más de una vez ni en la sala de The Mother; un reciclado nunca aparece en la sala del jugador.
