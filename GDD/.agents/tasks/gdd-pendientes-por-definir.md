# Pendientes por definir — GDD v0.2.0

Puntos abiertos que quedaron en el GDD después de la corrección de Mecánicas. Ninguno bloquea la v0.2.0, pero varios afectan el balance o la implementación.

## Decisiones de diseño abiertas

| Tema | Dónde | Pregunta | Impacto |
|---|---|---|---|
| Umbral de "en alerta" | M-10 (afecta M-02 y M-03) | ¿Se corta y bloquea la vigilancia en **búsqueda** o en **caza**? | Búsqueda: más Vulnerabilidad, vigilancia menos útil. Caza: vigilancia más útil |
| Cobertura de cámaras | M-02 | Mínimo de salas con cámara por intento | Disponibilidad real de la vigilancia |
| Distancia de la sala clave | M-15 | Distancia mínima desde la reaparición | Tensión de cada piso |
| Persecución por escaleras | M-15 | ¿Los enemigos siguen al jugador entre pisos? | Seguridad de las salas clave |
| Munición | M-04 | Tamaño del cargador y munición por casilla | Escasez y presión del inventario |
| Velocidad herido | M-05 | % de reducción en Herido y Crítico | Castigo del daño |
| Costo del empujón | M-06 | Resistencia que consume | Evitar que el melee sea la solución por defecto |
| Degradación | M-18 | ¿Se reinicia al llegar a una sala clave? | Lectura narrativa del progreso |
| The Mother, fase 2 | M-19 | Comportamiento y forma de la vista hostil | Diseño del jefe |
| Salir durante una alerta | Reglas y Estados (Bloque 2) | ¿Hay periodo de gracia al reanudar? | Evitar abusar de la suspensión |
| Lore | Visión, Historia | Relación entre la esposa, la instalación y The Mother | Narrativa completa |
| Controles | `04 - Controles.md` | Reasignación y columna de control | Accesibilidad y soporte posterior |

## Observaciones no bloqueantes de la revisión de v0.2.0

1. **Alcance de "en alerta":** M-02 corta la vigilancia con cualquier enemigo en alerta, pero M-03 solo la bloquea si el enemigo está en la sala o en el área cercana con ruta de persecución. Aclarar en M-02 que usa el mismo alcance que M-03, o marcarlo "Por definir".
2. **Mecánicas Demo que dependen de Deseables:** M-19 usa M-17 para acortar la fase 1, y M-15 reinicia el contador de M-18. Agregar en M-19 y M-15 una línea que indique que funcionan sin la Deseable, como ya hacen M-12 y M-14.
3. **Fichas faltantes en el Registro:** D-03, D-05 y D-08 a D-12 solo tienen fila en la tabla. Para uniformidad, agregarles ficha sin contenido nuevo.

## Secciones del GDD sin empezar

- Carpetas 03 a 09 (Historia y Mundo, Contenido, Arte y Audio, Experiencia, Tecnología, Producción, Calidad).
- `03 - Público y Plataforma.md`: "Interfaz legible" desmarcada hasta que exista `06 - Experiencia/02 - UI y Menús.md`.
