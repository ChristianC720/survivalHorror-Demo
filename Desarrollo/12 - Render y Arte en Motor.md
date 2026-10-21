# 12 — Render y Arte en Motor

**Objetivo:** el aspecto low-poly con luz moderna a ~540p, la luz por piso, la importación de modelos y animaciones desde Blender y Mixamo, y los efectos CRT y de degradación.

**Responsable:** Arte 3D y animación (con Programación de jugabilidad para los scripts).

**Referencias:** `GDD/05 - Arte y Audio/01 - Dirección de Arte.md` (D-81, D-82, paleta), `GDD/05 - Arte y Audio/02 - Personajes y Animación.md` (D-88, convenciones), `GDD/05 - Arte y Audio/03 - Entornos.md` (kit modular, iluminación), `GDD/07 - Tecnología/03 - Requisitos Técnicos.md` (presupuestos), D-84, D-85, D-106.

**Depende de:** guía 00.

## Pasos

### 1. Render a ~540p (D-81)

En los assets URP (guía 00) el *Upscaling Filter* ya es **Nearest-Neighbor**. Un script en `Bootstrap` fija la *Render Scale* para que la altura interna sea ~540 px en cualquier resolución:

```csharp
public sealed class PixelRenderScale : MonoBehaviour
{
    const float InternalHeight = 540f;   // D-81
    int lastHeight;

    void Update()
    {
        if (Screen.height == lastHeight) return;
        lastHeight = Screen.height;
        var urp = (UniversalRenderPipelineAsset)GraphicsSettings.currentRenderPipeline;
        urp.renderScale = Mathf.Clamp(InternalHeight / Screen.height, 0.1f, 1f);
    }
}
```

- La UI en *Screen Space – Overlay* no se ve afectada: se dibuja a resolución nativa (guía 10).
- En la cámara principal: *Post Processing* activado y **Dithering** activado (D-81).
- El pixelado **no** se puede desactivar (D-81).
- *shortcut:* `renderScale` modifica el asset en el editor; si molesta en Git, restáuralo a 1 en `OnDisable` del editor.

### 2. Texturas y materiales

- Texturas de mundo ≤ 256 px (personajes 512, The Mother 1024), **Filter Mode = Point**, sin mipmaps en el mundo, compresión normal.
- Shader *URP/Lit* simple o un ShaderGraph propio `SG_LowPolyLit`: color base, una máscara de brillo para el **destello especular** de los recogibles (solo bajo luz, D-84) y emisivos (pantallas, costuras teal de Lukas).
- Sin PBR complejo ni mapas de normales (Dirección de Arte).
- Sin temblor de vértices ni texturas afines (no es PSX puro).

### 3. Luz por piso (D-82)

- Ambiente casi negro (`#0f0607`) y niebla de URP del color de sombra del piso.
- Un *Volume* global por piso con *Color Adjustments* y *Tonemapping*; perfiles `VP_Floor1_Amber`, `VP_Floor2_ColdWhite` (sombras `#08241E`), `VP_Floor3_Crimson`, `VP_NonExistent_Purple`.
- Luces de emergencia sin sombra con el tinte del piso; la **linterna** es la única luz blanca y una de las ≤ 4 con sombra.
- El brillo de Opciones mueve la exposición del *Volume* global (guía 10).
- Sala final sin energía (D-72): sus luces apagadas hasta usar el interruptor.

### 4. Kit modular y salas

- Módulos de 2 m sobre la rejilla; salas de 12 × 12 m (6 × 6 módulos) con una abertura centrada por lado (guía 07).
- Cada sala es un prefab con su geometría, sus luces, sus `SpawnPoint` y su `SecurityCamera` opcional. Colliders simples (cajas) en la capa `Room` y coberturas en `Occluder`.
- Variantes por piso: mismo prefab base, distintos materiales y props (`PF_Room_<Arquetipo>_<Piso>_<NN>`).
- *Greybox* primero: los programadores trabajan con cubos; el arte sustituye la geometría sin mover los marcadores.

### 5. Personajes y animación (D-88)

1. Exporta desde Blender en FBX: escala 1 = 1 m, *Apply Transform*, Y arriba al importar en Unity, sin cámaras ni luces.
2. **Humanoid compartido** para Lukas, el Alto, la Boca y el Mudo: *Rig → Animation Type = Humanoid*, *Avatar* creado desde Lukas y reutilizado. Valida un clip de Mixamo por personaje en la semana 2.
3. Rastrero y The Mother: *Generic* con esqueletos propios.
4. Clips a 30 fps; locomoción de Mixamo retocada en Blender; agarres y remates como clips sincronizados propios.
5. `Animator` de Lukas: capa base (locomoción con *Blend Tree* por velocidad y estado de salud), capa superior con máscara (apuntar, recargar, terminal), capa aditiva (respiración, golpe recibido).
6. **Animation Rigging:** *Multi-Aim Constraint* del torso y la cabeza hacia la mira; *Two Bone IK* para pies y mano en puertas y paneles; IK procedural de las patas del Rastrero; brazo recolector de The Mother.
7. Ruido procedural en huesos de los asimilados (espasmos) con un script que suma rotaciones pequeñas en `LateUpdate`.
8. Ragdoll en enemigos al morir (`Corpse`, guía 08) con el asistente de Unity.
9. Nombres: `CH_<Personaje>_<Variante>` y `AN_<Personaje>_<Acción>`.

### 6. Efectos de pantalla

- **CRT de la vigilancia:** material de la `RawImage` (guía 03), verde fósforo con líneas de escaneo, ruido y *glitch*.
- **Terminal del brazo:** mismo shader en teal (guía 10).
- **Corrupción por degradación (M-18):** un *Volume* con un efecto de pantalla propio (*Full Screen Pass Renderer Feature* de URP con un ShaderGraph *Fullscreen*) que añade ruido y *glitch* morado según el nivel; **tope** que nunca tapa la interfaz ni se confunde con la señal degradada por búsqueda (D-29).
- **Niebla criogénica** de la fase 2 del jefe: *Volume* local + partículas, solo visual.
- Opciones de reducción (D-106) bajan la intensidad de estos efectos y de los parpadeos.
- Sangre oscura e impactos sin desmembramiento (D-85).

### 7. Cinemáticas

Timeline con *Cinemachine Track* y *Animation Track* en tiempo real (D-39). Si se aplica el recorte 5 de `GDD/08 - Producción/01 - Alcance.md`, las cinemáticas narrativas pasan a imágenes fijas con texto y sonido en el mismo Timeline.

## Criterios de aceptación

- A 1080p y a 1440p la imagen del mundo se ve a ~540 px de alto, con píxeles nítidos; la interfaz se ve nítida a resolución nativa.
- Los recogibles solo destellan bajo la linterna o una luz (D-84).
- Cada piso tiene su luz dominante de la tabla de Dirección de Arte.
- Un clip de locomoción funciona en Lukas, el Alto, la Boca y el Mudo con el mismo *Avatar*.
- La escena del piso 3 con la linterna y 2 enemigos cumple los presupuestos de `GDD/07 - Tecnología/03 - Requisitos Técnicos.md`.
