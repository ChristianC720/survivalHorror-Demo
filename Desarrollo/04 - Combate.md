# 04 — Combate (S-03)

**Objetivo:** revólver de Kessler, empujón, agarres con punzón de descarga, puntos débiles y remate de cuerpos.

**Responsable:** Christian Coronel Palma.

**Referencias:** `GDD/02 - Diseño/03 - Sistemas.md` (S-03, S-09), `GDD/02 - Diseño/02 - Mecánicas.md` (M-04, M-06, M-11, M-20), `GDD/04 - Contenido/04 - Objetos y Coleccionables.md` (revólver, punzón), decisiones D-66 (disparos por tipo), D-71, D-74 (revólver), D-27 (contexto sobre un cuerpo).

**Depende de:** guías 02 y 03; guía 06 para la munición y el punzón en el inventario.

## Pasos

### 1. Revólver (M-04, D-74)

`Revolver` en el jugador, datos en `SO_Revolver`:

| Dato | Valor inicial |
|---|---|
| Tambor | 6 balas |
| Recarga | Bala a bala desde la munición del inventario |
| Cadencia | ~0.8 s entre disparos |
| Ruido | Alerta a ~20 m (`NoiseEmitter.Emit`) y suma un poco a la atención de The Mother (D-24) |
| Dispersión | Se cierra si Lukas está quieto apuntando |

- **Disparo:** `Physics.Raycast` desde el centro de la cámara de apuntado contra `Enemy`, `Occluder` y geometría; aplica la dispersión como desviación aleatoria del rayo. Solo dispara en el mapa `Exploration` con `Aim` mantenido.
- **Daño por tipo:** cada enemigo muere tras N impactos (`EnemyDefinition.shotsToKill`: Alto 4, Boca 3, Rastrero 2, Mudo 3; reciclado 2, Rastrero reciclado 1; D-66). Cuenta impactos, no puntos de vida.
- **Punto débil:** un `Collider` hijo con `WeakPoint`; acertarle **interrumpe** el ataque o agarre en curso pero cuenta como un impacto normal (D-74).
- **Recarga (`R`):** corrutina bala a bala; impide disparar; un golpe la cancela sin perder la munición ya cargada (S-03).
- **Cargador vacío:** chasquido (S-11).
- **Siempre vuelve descargado:** en cada reconstrucción el tambor queda en 0 (D-74).
- El contador verde del armazón (modelo del revólver) muestra las balas cargadas al apuntar; es la única indicación de munición fuera del terminal (D-86).

### 2. Empujón (M-06)

- `Attack` sin apuntar y sin cuerpo al centro de la vista → `Stamina.TrySpend(0.35)`; si falla, jadeo y nada más.
- `OverlapSphere` frente al jugador (~1.5 m, ángulo ~60°): el primer enemigo recibe `Stun(duración corta)` y un empuje hacia atrás. No hace daño ni da invulnerabilidad (D-05).
- Corta la vigilancia (guía 03).

### 3. Agarre (M-11)

`GrabResolver` en el jugador; los enemigos que agarran (el Alto, el Mudo y el brazo recolector de The Mother) llaman a `GrabResolver.Resolve(enemy)`:

1. Si hay un hackeo en curso, se cancela antes de resolver (caso especial de Reglas y Estados).
2. Si el inventario tiene punzón: se consume, el enemigo queda aturdido 2 s, sin daño a Lukas.
3. Si no: Lukas baja un estado de salud; en Critical, muere.

El punzón **no** se activa con una tecla en la defensa: es automático (Controles).

### 4. Contexto "sobre un cuerpo" y remate (M-20, D-27)

`InteractionProbe` (guía 06) decide qué hay **al centro de la vista** a ~1.5 m. Si es un `Corpse` que puede reciclarse (no es reciclado ni está en la sala del jefe):

| Entrada | Acción | Costo | Ruido |
|---|---|---|---|
| `Attack` sin apuntar | Golpe en el suelo (~2 s) | 25% de resistencia | Alerta a enemigos cercanos |
| `Interact` (`E`) | Punzón de descarga (~1.5 s) | 1 punzón | Ninguno |

- Recibir un golpe durante el golpe en el suelo lo cancela.
- Sin punzón, `E` no hace nada; con el cuerpo ya rematado, `Attack` vuelve a empujar y `E` a interactuar.
- Si otro interactuable está más cerca del centro de la vista que el cuerpo, gana ese (D-27).
- Al terminar: `Corpse.MarkFinished()` → S-09 no lo recicla (guía 08).

### 5. Eventos que emite

- `EnemyKilled(enemy)` al último impacto → S-09 crea el `Corpse` y S-11 deja de contar al enemigo.
- `PlayerDamaged(state)` → corte de vigilancia, sacudida de cámara, sonido.
- `PlayerDied(cause)` → S-08.

## Criterios de aceptación

- Con los valores iniciales, cada tipo cae con sus disparos de D-66 y un reciclado con 2 (1 si es un Rastrero).
- Recargar con `R` añade balas de una en una; un golpe la cancela sin perder balas.
- El punzón se consume al ser agarrado (uno por agarre) o al rematar un cuerpo (D-71).
- El golpe en el suelo alerta a los enemigos cercanos y el punzón no (S-09).
- Ninguna tecla tiene dos funciones en el contexto "sobre un cuerpo".
- El empujón sin resistencia suficiente no ocurre y suena un jadeo.
