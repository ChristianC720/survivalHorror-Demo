# Requisitos Técnicos

## 1. Descripción de la sección

Define las condiciones técnicas necesarias para ejecutar y desarrollar el videojuego.

Los requisitos son objetivos provisionales y se confirman con el perfilado de la semana de pruebas del [[03 - Roadmap]]. Los valores numéricos de este documento son iniciales y están sujetos a balance.

## 2. Requisitos mínimos

**Sistema operativo:** Windows 10 de 64 bits.

**CPU:** 4 núcleos a ~2.5 GHz, como un Intel Core i5 de 6.ª generación o un AMD Ryzen 3 1200.

**GPU:** Compatible con DirectX 11 y con 2 GB de VRAM, como una NVIDIA GTX 950 o una AMD RX 460.

**RAM:** 8 GB.

**Almacenamiento:** 3 GB libres.

**Resolución:** 1280 × 720 a 30 FPS.

**Conexión:** No requerida.

## 3. Requisitos recomendados

**Sistema operativo:** Windows 10 u 11 de 64 bits.

**CPU:** 6 núcleos a ~3 GHz, como un Intel Core i5 de 9.ª generación o un AMD Ryzen 5 2600.

**GPU:** 4 GB de VRAM, como una NVIDIA GTX 1060 o una AMD RX 580.

**RAM:** 16 GB.

**Almacenamiento:** 3 GB libres en SSD.

## 4. Objetivos de rendimiento

**FPS objetivo:** 60 FPS a 1920 × 1080 en el equipo recomendado ([[03 - Público y Plataforma]]). Nunca baja de 30 FPS en el equipo mínimo.

**Tiempo de carga objetivo:** La generación del piso y la construcción del NavMesh tardan 5 s o menos, para que la transición de piso no se alargue. El arranque hasta el menú principal tarda 15 s o menos.

**Memoria máxima:** 3 GB de RAM del proceso y 1.5 GB de VRAM como máximo.

**Presupuestos por fotograma:**

| Elemento | Presupuesto |
|---|---|
| Salas activas | La sala actual y sus adyacentes. El resto del piso queda desactivado con `SetActive(false)` |
| Enemigos con IA activa | Todos los del piso, que nunca se desactivan y fuera de la vista no se dibujan. Configuración completa: 6 a 9 en el piso 1, 9 a 12 en el piso 2 y 12 a 15 en el piso 3. Showcase: 5 a 7, 7 a 9 y 9 a 11 |
| Salas dibujadas | La actual y las que están a 2 puertas o menos |
| Resolución interna | ~540 líneas, con escalado sin suavizado para el estilo pixelado |
| Luces dinámicas visibles | 16 como máximo, con *Forward+* |
| Luces con sombra | 4 como máximo a la vez, la linterna y 3 más. El resto va sin sombra |
| Triángulos en pantalla | 300 000 como máximo |
| *Draw calls* / *batches* | 1 500 como máximo |
| Texturas de mundo | 256 × 256 como máximo, 512 para personajes, con filtro *Point* |
| Audio simultáneo | 32 voces como máximo |

## 5. Restricciones

- Solo Windows de 64 bits. La demo no tiene versión de consola, macOS ni Linux.
- No hay luz horneada, porque las salas cambian en cada intento. La iluminación es dinámica con *Forward+*.
- El render a ~540p no se desactiva. La interfaz siempre va a resolución nativa.
- Teclado y mouse están completos. El mando tipo Xbox usa el mapa básico fijo de [[04 - Controles]], sin reasignación.
- **NavMesh.** Se construye una vez por piso, durante la transición o la reconstrucción, y nunca durante el juego.
- **Vista de vigilancia.** El retraso de imagen de ~0.3 s se logra guardando unas pocas imágenes de baja resolución. No debe bajar de 60 FPS.
- **Unity 6.6 no es LTS.** El riesgo está en [[05 - Seguridad y Riesgos Técnicos]].
- **Tamaño del build.** El objetivo es 2 GB como máximo, y cierra el punto de almacenamiento de [[03 - Público y Plataforma]].

## 6. Requisitos de desarrollo

- Unity `6000.6.8f1` con *Windows Build Support*. La build de Windows de 64 bits usa el backend Mono.
- Git y Git LFS instalados ([[02 - Motor y Herramientas]]).
- Blender, REAPER y FL Studio para quien produce arte y audio ([[02 - Motor y Herramientas]]).

> **Navegación:** [[00 - Índice]] · ← [[02 - Motor y Herramientas]] · [[04 - Datos y Persistencia]] →
