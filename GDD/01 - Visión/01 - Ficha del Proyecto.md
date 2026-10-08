# Ficha del Proyecto

## Descripción de la sección

Esta ficha identifica el proyecto de forma rápida. Debe permitir que alguien que nunca haya visto el videojuego entienda qué proyecto es y cuál es su estado actual.

## Información general

| Campo                        | Información                                                                                                                                    |
| ---------------------------- | ---------------------------------------------------------------------------------------------------------------------------------------------- |
| **Nombre del proyecto**      | Identity Dissolution                                                                                          |
| **Nombre clave**             | Heartbeat                                                                                                                                      |
| **Versión del GDD**          | 0.5.0                                                                                                                                          |
| **Estado del proyecto**      | Concepto <!-- / Preproducción / Producción / Pruebas / Publicado -->                                                                           |
| **Equipo**                   | Victor Gabriel Alvarado Chay<br>Christian Coronel Palma<br>Lenin Ramón López Córdoba<br>Brian Iván Navarrete Canché<br>Lian Daniel Sierra Sosa |
| **Responsable del proyecto** | Christian Coronel Palma                                                                                                                        |
| **Fecha de inicio**          | 2026-09-25                                                                                                                                     |
| **Última actualización**     | 2026-10-08                                                                                                                                     |
| **Repositorio**              | [Repositorio Demo](https://github.com/ChristianC720/survivalHorror-Demo)                                                                       |

## Identidad del videojuego

**Género principal:**  Survival Horror.
**Justificación:**  El género principal del videojuego es la supervivencia, ya que el jugador tiene que gestionar recursos limitados, enfrentarse a amenazas más fuertes que el personaje principal, administrar un inventario 6-slots, y llegar en las mejores condiciones posibles hasta el final.

**Géneros secundarios:**  Terror y Roguelite.
**Justificación:** El Terror se toma como género secundario porque el juego tendrá una ambientación oscura, se enfrentará a monstruos, y la historia estará centrada en el terror y lo que le sucede al personaje dentro de este mundo. También se tomó el Roguelite porqué se implementará una generación procedural de cuartos a partir del punto inicial, y cada que el jugador pierda una partida al morir, el escenario cambiará por completo. Esto como parte de la narrativa y jugabilidad que se tienen contemplados.

**Subgéneros:** Survival Horror.
**¿Por qué?:** Porque combina la supervivencia de los recursos limitados y los enemigos difíciles con el terror de la narrativa y el mundo que lo rodea.

**Plataforma Objetivo:**  PC
**¿Por qué?:** Por su versatilidad y rendimiento


**Modo principal:**  Un jugador
**Jugadores simultáneos:**  No
**Local / En línea:**  Local
**Justificación:** El juego será de un solo jugador, ya que el tipo de juego y la historia que se quiere implementar fue pensado para este modo de juego y que el jugador se pueda enfocar ya sea en la narrativa o simplemente disfrutar de avanzar en el juego a su propio paso.

**Perspectiva principal:**  Tercera Persona
**Tipo de cámara:**  Cámara Dinámica
**¿Puede cambiar durante el juego?:**  Sí. Over the Shoulder (Tercera persona) es la vista por defecto. El jugador puede activar manualmente una vista isométrica de reconocimiento, que corresponde a las cámaras de seguridad de The Mother y solo está disponible en salas con cámara activa (ver [[02 - Mecánicas]]).

**Justificación de diseño:** El videojuego utiliza Over the Shoulder como perspectiva principal para preservar incertidumbre, escala y tensión durante la exploración y el combate. El jugador puede activar manualmente una vista isométrica secundaria de reconocimiento, accediendo desde el terminal de su brazo a las cámaras de seguridad de la instalación, para obtener una mejor comprensión espacial del entorno a cambio de un costo: debe permanecer quieto o caminar muy lento y la imagen se degrada cuando algún enemigo cercano está en búsqueda y la vista se corta al recibir daño o cuando algún enemigo cercano entra en caza (D-23). Esta vista no proporciona información omnisciente: enemigos y elementos relevantes solo son visibles dentro del campo de la cámara de seguridad, con ruido y retraso en la imagen. El modo isométrico no indica que una zona sea segura y las amenazas continúan activas mientras se utiliza.

**Clasificaciones y etiquetas adicionales:**  
- Salas clave fijas
- Procedural
- Generación aleatoria
- Inventario
- Jefes
- Sigilo
- Combate
- Exploración
- Exploración vertical
- Local


**Tecnología principal:**  Unity, C#, FL Studio, Blender, Blockbench, Obsidian
> Motor, lenguaje y herramientas principales.

## Elevator Pitch

**Respuesta:**
Survival Horror con elementos Roguelite en el que el jugador debe explorar escenarios generados proceduralmente, administrar recursos y sobrevivir a criaturas más fuertes que él. Combinando una cámara en tercera persona sobre el hombro con el acceso limitado a las cámaras de vigilancia isométricas de la instalación.

**Premisa:** Un hombre busca la última pista que su esposa desaparecida le dejó, pero queda atrapado en un ciclo sin fin del que ahora debe salir.

## Estado actual

- [x] Concepto definido
- [ ] Alcance inicial definido
- [ ] Mecánica principal definida
- [ ] Prototipo funcional
- [ ] Vertical slice
- [ ] Producción
- [ ] Pruebas
- [ ] Versión final

## Criterio de éxito del proyecto

¿Qué tendría que suceder para considerar que el videojuego cumplió su propósito?

**Respuesta:**
El proyecto se considerará exitoso si se logra ofrecer una experiencia jugable completa y comprensible en la que el jugador pueda explorar, administrar recursos, combatir o evadir amenazas, utilizar correctamente el inventario y completar una partida sin confundirse con las mecánicas que el juego ofrece.

También deberá demostrar correctamente las principales características que diferencian al proyecto: el cambio dinámico entre cámara isométrica y tercera persona, la variación del escenario después de morir, la sensación constante de vulnerabilidad del Survival Horror y la integración de estas mecánicas con la narrativa.

Como criterio mínimo para la demo, el jugador deberá poder:

- Iniciar una partida y recorrer un escenario.
- Encontrar y administrar recursos mediante el inventario de 6 espacios.
- Enfrentarse o escapar de enemigos.
- Experimentar al menos un cambio  entre cámara isométrica y tercera persona.
- Morir, reiniciar la partida y obtener una distribución diferente del escenario.
- Alcanzar un objetivo final claramente definido.
- Comprender, aunque sea parcialmente, la relación entre la muerte, la generación procedural y la narrativa del juego.
- En la configuración de showcase (evento de presentación), completar el recorrido de los 3 pisos hasta The Mother en ~30 minutos, con menos salas por piso y enemigos y recursos escalados en proporción (D-34, valor inicial, sujeto a balance). La configuración completa se estima en ~45–60 minutos (ver [[03 - Público y Plataforma]]).

> **Navegación:** [[00 - Índice]] · ← [[00 - Índice]] · [[02 - Visión del Juego]] →
