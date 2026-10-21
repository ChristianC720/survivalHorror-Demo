# 11 — Audio (S-11)

**Objetivo:** que el sonido comunique la percepción y la cercanía de las amenazas sin indicadores en pantalla, con una mezcla por prioridades.

**Responsable:** Audio, narrativa y UI.

**Referencias:** `GDD/02 - Diseño/03 - Sistemas.md` (S-11), `GDD/05 - Arte y Audio/05 - Audio y Música.md` (efectos, música, mezcla), `GDD/02 - Diseño/02 - Mecánicas.md` (M-10), D-23, D-29, D-46, D-64, D-87, D-95, D-102.

**Depende de:** guías 01 y 05.

## Pasos

### 1. Mezclador

`Audio/Mixers/Main.mixer` con grupos y parámetros expuestos para Opciones (guía 10):

```text
Master
├── Signals     (prioridad 1: alertas, latido del protagonista, grito de la Boca, avisos, motor de cámaras)
├── Player      (prioridad 2: pasos, disparos, golpes, jadeo)
├── Ambience    (prioridad 3: drones, maquinaria, latido del laboratorio)
├── Music       (prioridad 4)
└── Voice       (sonidos vocales de Lukas, Emilia y The Mother)
```

- **Ducking:** efecto *Duck Volume* en `Ambience` y `Music` con *send* desde `Signals` (D-87: la música y el ambiente bajan con una señal de prioridad 1).
- Valores concretos de mezcla: **Por definir** en pruebas; empieza con −8 dB de *ducking*.

### 2. Audio 3D

- `AudioSource` con *Spatial Blend* 1, atenuación logarítmica, en enemigos, pasos y objetos.
- **Oclusión simple por puertas:** si entre la fuente y el oyente hay una puerta sellada o cerrada (`Raycast` a la capa de puertas), baja el volumen y aplica un *Low Pass*. El latido del protagonista **no** cuenta enemigos tras una puerta sellada (S-11).
- El latido del laboratorio y la música no son posicionales (*Spatial Blend* 0).

### 3. Señales de alerta (M-10, D-23)

`ThreatAudio` escucha `HighestAlertChanged` (guía 05) — el **mismo** valor que usa la señal de vigilancia (guía 03), así sonido y señal nunca se contradicen:

| Estado más alto | Sonido |
|---|---|
| Sospecha | Sonido A (corto, grave) |
| Búsqueda | Sonido B (distinto, más insistente) |
| Caza | Sonido C (agresivo) |

Con varios enemigos, suena solo el del estado más alto.

### 4. Latidos

- **Latido del protagonista:** volumen y ritmo según la distancia a la amenaza más cercana sin puerta sellada en medio, **cuantizada** en 3–4 niveles para que no funcione como radar.
- **Latido del laboratorio (D-46, D-95):** capa de `Ambience` con timbre grave distinto. Su nivel = `base + degradación × k + (salud == Critical ? extra : 0)`; se acelera en el Despertar. **Nunca** depende de enemigos ni de alertas. Tope: nunca supera las señales de `Signals` (D-29).
- Suena de fondo en la nota de voz de Emilia y en todas las salas desde la primera reconstrucción.

### 5. Efectos

Implementa la tabla "Efectos de sonido" de Audio y Música. Cada evento del juego llama a `AudioService.Play(SoundId, posición)`; `SoundId` es un ScriptableObject (`SoundDefinition`) con clips, grupo, volumen, variación de tono y la marca `captioned` + texto para los subtítulos de sonido (D-102, guía 10).

- Pasos por superficie: `PhysicMaterial` o una etiqueta del suelo (metal, agua, escombros) → conjunto de clips.
- Aviso de reciclaje (~3 s), jadeo, chasquido del tambor vacío, pitidos de hackeo, arranque del interruptor, cierre `LOCKDOWN`, encendido del terminal, "sincronizando…".

### 6. Música (D-87)

- Exploración **sin música**: solo ambiente.
- Música en menú, cinemáticas, sala de The Mother y finales. Leitmotivs: Emilia, The Mother, el latido.
- **Jefe:** un loop por fase; el cambio de fase espera al siguiente compás (`AudioSource.PlayScheduled` con `AudioSettings.dspTime`). La música nunca tapa el motor de las cámaras.

### 7. Degradación (M-18, Deseable)

La distorsión del sonido vocal de Lukas sube con la degradación (efecto *Distortion* o *Pitch Shifter* en el grupo `Voice` por nivel), con el tope de D-29.

## Criterios de aceptación

Los de S-11, en especial:

- Cada estado de alerta tiene un sonido distinguible y coincide siempre con la calidad de la señal de vigilancia.
- El latido no responde a enemigos detrás de una puerta sellada.
- Con la misma degradación, el latido del laboratorio suena igual con o sin enemigos cerca.
- Con la degradación máxima, todas las señales de M-10 siguen audibles.
- Todo cuerpo reproduce el aviso orgánico antes de desaparecer.
