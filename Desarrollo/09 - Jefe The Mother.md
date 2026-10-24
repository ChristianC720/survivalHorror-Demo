# 09 — Jefe The Mother (S-10)

**Objetivo:** el encuentro final de 3 fases en la sala de The Mother, la victoria, la decisión final y las cinemáticas de victoria.

**Responsable:** Christian Coronel Palma (con Bryan Iván Navarrete Canché para la sala y Lian Daniel Sierra Vera para la música por fases).

**Referencias:** `GDD/02 - Diseño/03 - Sistemas.md` (S-10 y cinemáticas de S-08), `GDD/02 - Diseño/02 - Mecánicas.md` (M-19), `GDD/04 - Contenido/02 - Niveles.md` ("Sala de The Mother"), `GDD/04 - Contenido/03 - Enemigos y NPC.md` (The Mother), D-28, D-51, D-55, D-61, D-65, D-80, D-92, D-98.

**Depende de:** guías 03, 04, 05, 06, 07 y 08. Solo se empieza con H4 cumplido.

## Pasos

### 1. Sala

`PF_MotherRoom`: sala clave fija del piso 3 (guía 07), sin carne (D-59). Contiene:

- La grúa con rieles y el **brazo recolector** (`CollectorArm`).
- **Nodos de hackeo** (`HackPanel` con longitud de terminal principal) en mesas de mezcla y racks de cinta (fase 1).
- **3 cámaras hostiles** (`SecurityCamera` con `Hostile = true`) con luz roja y motor (fase 2).
- **Cápsulas del archivo** como puntos de liberación de asimilados (fase 2).
- El núcleo criogénico con **3 transformadores** (`WeakPoint` de 2 disparos cada uno) (fase 3).
- **20 balas** fijas: 12 (3 blísteres) en los puntos ciegos para la fase 2 (D-115) y 8 junto al núcleo para la fase 3 (6 + 2 de margen, D-80); 0–1 curas.
- Terminal central y salida como `IInteractable` desactivados hasta la decisión final.
- Sin `RecycleSystem` (D-28) y sin autoguardado al entrar (D-30).

### 2. `MotherBoss`

Máquina de estados `Inactive → Phase1 → Phase2 → Phase3 → Defeated`. Al empezar cada fase, una línea de protocolo en texto (D-62) y cambio de capa de la música (guía 11).

**Brazo recolector (todas las fases, D-65, D-98):** recorre los rieles a ~2.5 m/s hacia el jugador o los nodos; si el jugador está a ≤ ~3 m, aviso de ~1 s (sonido y luz) y agarre con `GrabResolver.Resolve` (guía 04).

**Fase 1 — Hackeo bajo presión:** el jugador hackea N nodos (M-13) mientras el brazo patrulla entre ellos. N = **3** nodos (D-114), en `SO_MotherBoss`. Con fallas de la máquina registradas (M-17), las secuencias se acortan; sin M-17 usa la longitud original. Al completar todos → fase 2.

**Fase 2 — Cámaras hostiles (D-28, D-98):**

- 3 cámaras barren a ~20°/s con un cono de ~12 m; luz roja y sonido de motor al girar.
- Si una cámara enfoca al jugador ~1 s, libera el siguiente asimilado (2 Altos y 2 Rastreros en total) desde una cápsula y envía ese punto a S-02; el asimilado va hacia él.
- 4 asimilados en total: 2 Altos y 2 Rastreros; nunca más de 2 activos (D-80, D-115).
- Niebla criogénica solo visual (un *Volume* local); no ralentiza ni tapa señales.
- `Q` no activa la vigilancia (guía 03).
- **Fin de la fase (D-115):** `Phase2.IsComplete()` = los 4 asimilados eliminados **y** Lukas dentro del *trigger* `CoreZone` junto al núcleo. Si Lukas entra en `CoreZone` con asimilados sin liberar, se liberan los que falten respetando el máximo de 2 activos (D-117).

**Fase 3 — Puntos débiles:** el núcleo se abre y expone los 3 transformadores; 2 disparos cada uno. Al destruir el último: `Defeated`.

Duración objetivo: 1–2 min por fase (M-19).

### 3. Victoria y decisión final

Al llegar a `Defeated`, `MotherBoss` emite `BossDefeated`; `DeathCycle`/`GameStateMachine` reproducen:

1. **Secuencia común** (estado `Victory`): caída de The Mother; si `emiliaDocuments == 6`, los documentos corruptos y el mensaje se completan en el terminal y Lukas recupera los nombres (D-55); **Despertar** (el latido del laboratorio se acelera).
2. Con menos de 6 documentos → `RelayBasic` (examen automático de la terminal central, sin elección).
3. Con 6 → `FinalDecision`: se crea un autoguardado (D-92), se activan la terminal central y la salida. `E` sobre la que esté al centro de la vista (D-27). Presión sin muerte: altavoces, latido, temblor y luces que fallan, sin daño ni enemigos (D-61). Pausa disponible sin "Reiniciar".
4. Terminal central → `RelayComplete` (con el expediente R-H-01..R-H-06 "estado desconocido"); salida → `Ambiguous`.
5. Toda cinemática de victoria termina con el **gancho abierto** y después los **créditos**.
6. Tras los créditos, Continuar inicia un ciclo nuevo (D-93, guía 08).

### 4. Muerte en la sala

Regresa al inicio del ciclo del piso 1 (guía 08). El progreso de fases no persiste.

## Criterios de aceptación

Los de S-10 y los de victoria de S-08, en especial:

- Las 3 fases se completan solo con mecánicas existentes.
- Entrar en la sala no crea un autoguardado.
- En la fase 2 cada cámara muestra luz roja y su motor suena al girar; cada enfoque libera un asimilado hacia ese punto; nunca hay más de 2 activos.
- Al empezar la fase 3 hay munición para los 3 transformadores más 2 de margen aunque el jugador llegue sin munición.
- Ningún enemigo eliminado en la sala se recicla.
- Con 0–5 documentos: Relevo básico sin elección. Con 6: `E` en la terminal → Relevo completo; `E` en la salida → Ambiguo; sin menú (CP-05).
