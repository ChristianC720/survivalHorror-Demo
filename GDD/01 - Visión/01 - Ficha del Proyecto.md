# Ficha del Proyecto

## 1. Descripción de la sección

Esta ficha identifica el proyecto de forma rápida. Debe permitir que alguien que nunca haya visto el videojuego entienda qué proyecto es y cuál es su estado actual.

## 2. Información general

| Campo | Información |
|---|---|
| **Nombre del proyecto** | Identity Dissolution |
| **Nombre clave** | Heartbeat |
| **Lema** | *Die. Rebuild. Forget.* |
| **Versión del GDD** | 0.24.0 |
| **Estado del proyecto** | Producción |
| **Equipo** | Victor Gabriel Alvarado Chay<br>Christian Coronel Palma<br>Lenin Ramón López Córdova<br>Bryan Iván Navarrete Canché<br>Lian Daniel Sierra Vera |
| **Responsable del proyecto** | Christian Coronel Palma |
| **Fecha de inicio** | 2026-09-26 |
| **Última actualización** | 2026-11-09 |
| **Repositorio** | [Repositorio de la demo](https://github.com/ChristianC720/survivalHorror-Demo) |

## 3. Identidad del videojuego

**Género principal:** Supervivencia. Lukas administra recursos limitados y un inventario de 6 casillas frente a amenazas más fuertes que él, y debe llegar al final en las mejores condiciones posibles. Ver [[01 - Géneros]].

**Géneros secundarios:** Terror y roguelite. La ambientación, los monstruos y la historia son de terror. Al morir, las salas procedurales se regeneran y las salas clave cambian de posición como parte de la narrativa.

**Subgéneros:** Survival horror. Une la supervivencia con recursos escasos y enemigos difíciles con el terror del mundo y de la historia. Ver [[02 - Subgéneros y Combinaciones]].

**Biblioteca de clasificación:** [[00 - Biblioteca de Clasificación]]

**Plataforma objetivo:** PC, por versatilidad y rendimiento. Ver [[03 - Plataformas y Consolas]].

**Modo de juego:** Un jugador, local y sin jugadores simultáneos. La historia está pensada para una persona que avanza a su propio ritmo, ya sea por la narrativa o por el recorrido. Ver [[05 - Modos de Juego]].

**Perspectiva:** Tercera persona con cámara dinámica. La vista sobre el hombro es la predeterminada y conserva la incertidumbre, la escala y la tensión. Desde el terminal del brazo, Lukas puede activar una vista isométrica a través de las cámaras de seguridad de The Mother, solo en salas con cámara activa. Esa vista cuesta. Lukas debe quedarse quieto o caminar muy lento, la imagen llega con ruido y retraso y solo muestra lo que cubre la cámara. Se degrada si un enemigo cercano está en búsqueda y se corta con el daño o cuando un enemigo cercano entra en caza. Nunca indica que una sala sea segura. Ver [[04 - Perspectivas y Cámaras]] y [[02 - Mecánicas]].

**Clasificaciones y etiquetas adicionales:** Salas clave fijas, procedural, generación aleatoria, inventario, jefes, sigilo, combate, exploración, exploración vertical y local. Ver [[06 - Clasificaciones y Etiquetas]].

**Tecnología principal:** Unity y C# para el juego. Blender para modelado, rig y animación. REAPER y FL Studio para audio. Obsidian para la documentación.

## 4. Elevator Pitch

**Respuesta:** Un técnico de mantenimiento busca la última pista que le dejó su esposa desaparecida y queda atrapado en un ciclo sin fin dentro de un laboratorio sellado. Es un survival horror con elementos de roguelite en el que el jugador explora pisos generados por procedimiento, administra seis casillas de inventario y sobrevive a criaturas más fuertes que él. La cámara sobre el hombro se alterna con un acceso limitado a las cámaras de vigilancia isométricas de la instalación.

![Pantalla de título de Identity Dissolution|640](../Media/Capturas/MenuPrincipal.png)

## 5. Estado actual

- [x] Concepto definido
- [x] Alcance inicial definido
- [x] Mecánica principal definida
- [x] Prototipo funcional (hitos H2 y H3)
- [ ] Vertical slice, en curso hasta el 10 de noviembre de 2026
- [ ] Producción
- [ ] Pruebas, en curso del 9 al 12 de noviembre de 2026
- [ ] Versión final, con entrega el 13 de noviembre de 2026

## 6. Criterio de éxito del proyecto

El proyecto cumple su propósito si la demo es completa y se entiende sin confusión. El jugador explora, administra recursos, pelea o esquiva amenazas, usa bien el inventario y termina una partida sin perderse entre las mecánicas. La demo también debe mostrar lo que distingue al juego. Eso incluye el cambio entre cámara isométrica y tercera persona, el escenario que cambia al morir y la vulnerabilidad constante, unidos a la narrativa.

Como mínimo, el jugador debe poder:

- Iniciar una partida y recorrer un escenario.
- Encontrar recursos y administrarlos en el inventario de 6 casillas.
- Enfrentar o esquivar enemigos.
- Pasar al menos una vez entre la cámara isométrica y la tercera persona.
- Morir, reiniciar y encontrar una distribución distinta del escenario.
- Alcanzar un objetivo final claro.
- Entender, aunque sea en parte, la relación entre la muerte, la generación procedural y la historia.
- Recorrer los 3 pisos hasta The Mother en ~30 minutos en la configuración showcase para eventos. Esta versión tiene menos salas por piso y escala enemigos y recursos en proporción. La configuración completa dura ~45 a 60 minutos. Ambas duraciones son valores iniciales sujetos a balance ([[03 - Público y Plataforma]]).

> **Navegación:** [[00 - Índice]] · ← [[00 - Índice]] · [[02 - Visión del Juego]] →
