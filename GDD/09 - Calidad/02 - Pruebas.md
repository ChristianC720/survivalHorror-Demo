# Pruebas

## 1. Descripción de la sección

Define cómo se comprobará que el videojuego funciona como fue diseñado.

## 2. Tipos de prueba

Lenin Ramón López Córdova es el responsable de QA ([[02 - Roles y Responsabilidades]]). Las pruebas y el balance general de la demo van del 9 al 12 de noviembre de 2026, y la entrega es el 13 de noviembre ([[03 - Roadmap]]). Los valores numéricos de este documento son iniciales y están sujetos a balance.

### 2.1 Pruebas funcionales

Comprueban que una característica haga lo que debe.

- **Automáticas.** Usan Unity Test Framework en EditMode y prueban lógica pura sin escena.
- Son obligatorias para el validador del generador, las reglas del inventario, la resistencia, el contador de muertes y el código de sujeto. También para la serialización y migración del guardado y la tabla de costo de salida.
- **Manuales.** Ejecutan los casos de esta nota y los criterios de aceptación de [[03 - Sistemas]], en una escena de prueba por sistema dentro de `Assets/_Project/Scenes/Tests/`.
- Esa carpeta también guarda las escenas `Sandbox_AI`, `Sandbox_Gallery`, `Sandbox_Kit`, `Sandbox_Movement` y `Sandbox_Render`.

### 2.2 Pruebas de integración

Comprueban que varios sistemas funcionen correctamente juntos.

- **PlayMode.** Se genera un piso, se coloca al jugador y se comprueba que existe un camino en el NavMesh hasta la llave y la sala final.
- **Cadenas entre sistemas.** Se recorren las relaciones de [[03 - Sistemas]]. Por ejemplo, morir durante una caza suma al contador, devuelve el inventario de llegada, regenera el piso, descarta el autoguardado y sube el código de sujeto.
- **Recorrido completo.** Va de `Nueva partida` a los créditos en la configuración showcase, desde el hito H5 ([[03 - Roadmap]]).

### 2.3 Pruebas de usabilidad

Comprueban si los jugadores entienden y pueden utilizar el sistema.

- Desde el hito H4, una persona ajena al equipo juega el piso 1 sin ayuda mientras alguien anota dónde se confunde.
- Después se le pregunta si entendió el uso y el costo de la vigilancia y si supo que la muerte cambia el piso. También si entendió los estados de alerta por el sonido.
- **Playtests con métricas.** Se hacen en cada hito desde H2, con personas que no conocen el GDD. Se les observa sin ayudar y luego se les pregunta.
- Los datos salen del modo de telemetría local, que se activa al lanzar el juego con `-telemetry`, igual que `-showcase` ([[05 - Seguridad y Riesgos Técnicos]]).

| Pregunta | Cómo se mide |
|---|---|
| ¿La vista de vigilancia es útil y divertida? | Usos y segundos por piso. Si la usa sin que se le diga |
| ¿Se percibe la atención de The Mother? | Veces que se llena. Si siente que los enemigos lo encuentran *sin razón* |
| ¿Cuánto dura un piso y la partida? | Tiempo por piso y total, con ~30 min como objetivo en la showcase |
| ¿Morir es justo o frustrante? | Muertes por piso, sala y causa. Si abandona antes de terminar |
| ¿Se entienden las reglas? | Si explica el reciclaje, el remate y el autoguardado sin haberlos leído |
| ¿Rematar es una decisión o una tarea? | Remates frente a enemigos reciclados |

### 2.4 Pruebas de rendimiento

Comprueban FPS, memoria, tiempos de carga y estabilidad.

- Se usan Profiler y Memory Profiler en una build de desarrollo, en el piso 3, con 2 enemigos en caza y la linterna encendida.
- Se mide el tiempo de generación y NavMesh por piso, con 5 s como máximo, y el tamaño de la build ([[03 - Requisitos Técnicos]]).
- Otros escenarios son la vista de vigilancia con señal degradada y la fase 2 de The Mother.
- Una partida de 60 min termina sin cierre inesperado ni crecimiento de memoria.
- Cerrar el proceso a la fuerza durante un autoguardado no corrompe la ranura ([[04 - Datos y Persistencia]]).

## 3. Caso de prueba

Estos son los casos críticos del proyecto. Se ejecutan en la semana de pruebas, del 9 al 12 de noviembre de 2026.

### 3.1 Generación válida

**Objetivo:** Comprobar que ningún piso es imposible, según Generación procedural por pisos en [[03 - Sistemas]]. Es una prueba automática.

**Precondiciones:** `LayoutValidator` con los parámetros de las configuraciones showcase y completa.

**Pasos:**

1. Generar 1 000 grafos por piso y configuración con las semillas 0 a 999.
2. Validar cada grafo.

**Resultado esperado:** Todos pasan.

- Hay ruta de la entrada a la llave obligatoria y de ahí a la sala final, sin cruzar su puerta ni puertas opcionales.
- La sala final queda al menos a 4 salas, o a 3 en la showcase.
- Ninguna sala tiene más de 2 enemigos y cada recurso queda entre su mínimo y su máximo.
- Hay 2 documentos de Emilia en salas secundarias.

**Resultado obtenido:** Sin ejecutar.

**Estado:** Pendiente.

### 3.2 Muerte y reconstrucción

**Objetivo:** Comprobar el Ciclo de muerte y reconstrucción de [[03 - Sistemas]].

**Precondiciones:** Partida en el piso 1 después del primer intento, con objetos y un documento registrado.

**Pasos:**

1. Dejarse matar por un enemigo.
2. Ver la cinemática corta.
3. Repetir la muerte y saltar la cinemática con `Espacio`.
4. Repetir con `Reiniciar`.

**Resultado esperado:**

- Lukas aparece al inicio del piso 1 con el inventario de llegada, vacío si llegó sin nada.
- El terminal conserva el documento y la distribución del piso es distinta.
- El código de sujeto y el contador suben 1.
- `Reiniciar` y la cinemática saltada dejan el mismo resultado.

**Resultado obtenido:** Sin ejecutar.

**Estado:** Pendiente.

### 3.3 Autoguardado y salida

**Objetivo:** Comprobar que guardar y continuar siguen la tabla de salida de Persistencia en [[03 - Sistemas]].

**Precondiciones:** Partida con autoguardado en la sala final del piso 1.

**Pasos:**

1. Salir con calma a mitad del piso 2 y elegir `Continuar`.
2. Salir durante una caza y elegir `Continuar`.
3. Cerrar el proceso desde el administrador de tareas y elegir `Continuar`.

**Resultado esperado:**

1. Lukas aparece en la sala final del piso 1 con el inventario de llegada y el contador sube 1.
2. Lukas aparece al inicio del piso 2 con el inventario de llegada y cuenta como muerte completa.
3. Lukas vuelve al último autoguardado sin costo.

**Resultado obtenido:** Sin ejecutar.

**Estado:** Pendiente.

### 3.4 Vigilancia con costo

**Objetivo:** Comprobar que la Vista de vigilancia respeta la señal escalonada de [[02 - Mecánicas]].

**Precondiciones:** Sala con cámara y un Alto en la sala adyacente.

**Pasos:**

1. Activar `Q` con el Alto en patrulla durante 6 s.
2. Repetir con el Alto en búsqueda.
3. Provocar que el Alto entre en caza.

**Resultado esperado:**

1. Tras ~5 s, el Alto pasa a sospecha y a búsqueda.
2. La señal llega con más ruido y más retraso.
3. La vista se corta y `Q` no la reactiva.

**Resultado obtenido:** Sin ejecutar.

**Estado:** Pendiente.

### 3.5 The Mother y decisión final

**Objetivo:** Comprobar que el jefe y los finales se resuelven según Jefe The Mother y Ciclo de muerte y reconstrucción, en [[03 - Sistemas]].

**Precondiciones:** Ranura de depuración en el piso 3, una con 0 documentos de Emilia y otra con 6.

**Pasos:**

1. Completar las 3 fases entrando sin munición.
2. Con 0 documentos, ver la victoria.
3. Con 6 documentos, elegir la terminal central.
4. Cargar de nuevo y elegir la salida.

**Resultado esperado:**

- La sala da 12 balas en la fase 2 y 8 en la fase 3.
- La fase 3 solo empieza con los 4 asimilados eliminados y Lukas en el núcleo.
- Si Lukas llega al núcleo sin ser enfocado, salen los asimilados que falten.
- Con 0 documentos se reproduce el Relevo básico sin elección.
- Con 6, la terminal lleva al Relevo completo y la salida al Ambiguo.
- Todos los finales terminan en el gancho y los créditos.

**Resultado obtenido:** Sin ejecutar.

**Estado:** Pendiente.

### 3.6 Rendimiento

**Objetivo:** Cumplir [[03 - Requisitos Técnicos]].

**Precondiciones:** Build de desarrollo en el equipo de menor gama del equipo.

**Pasos:**

1. Jugar 10 min en el piso 3 con la linterna encendida y enemigos en caza.
2. Medir 3 transiciones de piso.

**Resultado esperado:** Al menos 30 FPS en el equipo mínimo y 60 en el recomendado. La generación y el NavMesh tardan 5 s o menos.

**Resultado obtenido:** Sin ejecutar.

**Estado:** Pendiente.

### 3.7 Autoguardado pendiente durante una caza

**Objetivo:** Comprobar que llegar perseguido a la sala clave final no guarda hasta que termina la caza, según Persistencia en [[03 - Sistemas]].

**Precondiciones:** Piso 1 sin autoguardado previo, con un Alto en caza detrás del jugador.

**Pasos:**

1. Entrar en la sala clave final con el Alto en caza.
2. Comprobar que no aparece `sincronizando…` y que la escalera y el elevador muestran `LOCKDOWN`.
3. Perder al Alto o eliminarlo hasta que salga de la caza.
4. Salir del juego con calma y volver a cargar la ranura.

**Resultado esperado:** `sincronizando…` aparece solo al terminar la caza. Al cargar, el jugador aparece en la sala clave final con el mismo inventario y la sala empieza sin enemigos dentro.

**Resultado obtenido:** Sin ejecutar.

**Estado:** Pendiente.


> **Navegación:** [[00 - Índice]] · ← [[01 - Criterios de Calidad]] · [[03 - Bugs y Seguimiento]] →
