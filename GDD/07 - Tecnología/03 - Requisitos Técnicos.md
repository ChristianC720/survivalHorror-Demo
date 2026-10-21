# Requisitos Técnicos

## Descripción de la sección

Define las condiciones técnicas necesarias para ejecutar y desarrollar el videojuego.

Los requisitos son **objetivos provisionales** (D-112) y se confirman con el perfilado de la semana 6 del [[03 - Roadmap]].

## Requisitos mínimos

**Sistema operativo:** Windows 10 de 64 bits.

**CPU:** 4 núcleos a ~2.5 GHz (Intel Core i5 de 6.ª generación o AMD Ryzen 3 1200).

**GPU:** compatible con DirectX 11 y 2 GB de VRAM (NVIDIA GTX 950 o AMD RX 460).

**RAM:** 8 GB.

**Almacenamiento:** 3 GB libres.

**Resolución:** 1280 × 720 a 30 FPS.

**Conexión:** No requerida.

## Requisitos recomendados

**Sistema operativo:** Windows 10 u 11 de 64 bits.

**CPU:** 6 núcleos a ~3 GHz (Intel Core i5 de 9.ª generación o AMD Ryzen 5 2600).

**GPU:** 4 GB de VRAM (NVIDIA GTX 1060 o AMD RX 580).

**RAM:** 16 GB.

**Almacenamiento:** 3 GB libres en SSD.

## Objetivos de rendimiento

**FPS objetivo:** 60 FPS a 1920 × 1080 en el equipo recomendado ([[03 - Público y Plataforma]]); nunca por debajo de 30 FPS en el mínimo.

**Tiempo de carga objetivo:** generación del piso y construcción del NavMesh en **≤ 5 s**, para que la transición de piso no se alargue (D-39). Arranque hasta el menú principal en ≤ 15 s.

**Memoria máxima:** ≤ 3 GB de RAM del proceso y ≤ 1.5 GB de VRAM.

**Presupuestos por fotograma (D-112):**

| Elemento | Presupuesto |
|---|---|
| Salas activas | La sala actual y sus adyacentes; el resto del piso desactivado (`SetActive(false)`) |
| Enemigos con IA activa | ≤ 6 (2 por sala, D-37, en 3 salas activas) |
| Luces dinámicas visibles | ≤ 16 (Forward+) |
| Luces con sombra | ≤ 4 a la vez (linterna + 3); el resto sin sombra |
| Triángulos en pantalla | ≤ 300 000 |
| *Draw calls* / *batches* | ≤ 1 500 |
| Texturas de mundo | ≤ 256 × 256 (512 para personajes) con filtro *Point* (D-81) |
| Audio simultáneo | ≤ 32 voces |

## Restricciones

- Solo Windows de 64 bits; no hay versión de consola, macOS ni Linux en la demo.
- Sin luz horneada: las salas cambian en cada intento (S-06); la iluminación es dinámica con *Forward+*.
- El render a ~540p no se desactiva (D-81); la interfaz siempre a resolución nativa.
- Teclado y ratón completos; mando tipo Xbox con el mapa básico fijo de [[04 - Controles]], sin reasignación (D-120).
- **Tamaño del build:** objetivo ≤ 2 GB; cierra el punto de almacenamiento de [[03 - Público y Plataforma]].


> **Navegación:** [[00 - Índice]] · ← [[02 - Motor y Herramientas]] · [[04 - Datos y Persistencia]] →
