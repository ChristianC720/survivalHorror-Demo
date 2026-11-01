# Pruebas

## Descripción de la sección

Define cómo se comprobará que el videojuego funciona como fue diseñado.

Responsable de QA: Lenin Ramón López Córdova ([[02 - Roles y Responsabilidades]]). Calendario de hitos en [[03 - Roadmap]].

## Tipos de prueba

### Pruebas funcionales

Comprueban que una característica haga lo que debe.

- **Automáticas (Unity Test Framework, EditMode):** lógica pura sin escena. Obligatorias para: validador del generador, reglas de inventario, resistencia, contador de muertes, código de sujeto, serialización y migración del guardado, tabla de costo de salida.
- **Manuales:** casos de prueba de esta nota y criterios de aceptación de [[03 - Sistemas]], en una escena de prueba por sistema (`Scenes/Tests/`).

### Pruebas de integración

Comprueban que varios sistemas funcionen correctamente juntos.

- **PlayMode:** generar un piso, colocar al jugador y comprobar que existe un camino en el NavMesh hasta la llave y la sala final.
- **Cadenas de la tabla de relaciones** de [[03 - Sistemas]]: por ejemplo, morir durante una caza → contador, inventario vacío, pisos regenerados, autoguardado descartado y código de sujeto +1.
- **Recorrido completo:** de Nueva partida a los créditos en la showcase, una vez por semana desde H5 ([[03 - Roadmap]]).

### Pruebas de usabilidad

Comprueban si los jugadores entienden y pueden utilizar el sistema.

- Desde H4, una persona ajena al equipo juega el piso 1 sin ayuda mientras alguien anota dónde se confunde. Preguntas: ¿entendió cómo se usa la vigilancia y su costo?, ¿supo que la muerte cambia el piso?, ¿entendió los estados de alerta por el sonido?
- **Playtests con métricas** en cada hito desde H2, con personas que no conocen el GDD. Se les observa sin ayudar y luego se les pregunta. Los datos salen de la telemetría local de las compilaciones de prueba ([[05 - Seguridad y Riesgos Técnicos]]).

| Pregunta | Cómo se mide |
|---|---|
| ¿La vista de vigilancia es útil y divertida? | Usos y segundos por piso; ¿la usa sin que se le diga? |
| ¿Se percibe la atención de The Mother? | Veces que se llena; ¿siente que los enemigos lo encuentran "sin razón"? |
| ¿Cuánto dura un piso y la partida? | Tiempo por piso y total (objetivo ~30 min en la showcase) |
| ¿Morir es justo o frustrante? | Muertes por piso, sala y causa; ¿abandona antes de terminar? |
| ¿Se entienden las reglas? | Que explique el reciclaje, el remate y el autoguardado sin haberlos leído |
| ¿Rematar es una decisión o una tarea? | Remates frente a enemigos reciclados |

### Pruebas de rendimiento

Comprueban FPS, memoria, tiempos de carga y estabilidad.

- Profiler y Memory Profiler en una build de desarrollo, en el piso 3 con 2 enemigos en caza y la linterna encendida.
- Medir el tiempo de generación + NavMesh por piso (objetivo ≤ 5 s) y el tamaño del build ([[03 - Requisitos Técnicos]]).
- Escenarios adicionales: vista de vigilancia con señal degradada y fase 2 de The Mother.
- Partida de 60 min sin cierre inesperado ni crecimiento de memoria.
- Cerrar el proceso a la fuerza durante un autoguardado: la ranura no se corrompe ([[04 - Datos y Persistencia]]).

## Caso de prueba

Plantilla, seguida de los casos críticos del proyecto.

### Nombre

**Objetivo:**

**Precondiciones:**

**Pasos:**

1.
2.
3.

**Resultado esperado:**

**Resultado obtenido:**

**Estado:** Correcto / Incorrecto

### CP-01 Generación válida (automática)

**Objetivo:** Ningún piso es imposible (S-06).

**Precondiciones:** `LayoutValidator` y parámetros de showcase y completa.

**Pasos:**

1. Generar 1 000 grafos por piso y configuración con semillas 0..999.
2. Validar cada uno.

**Resultado esperado:** Todos pasan: ruta entrada → llave obligatoria → sala final sin cruzar su puerta ni puertas opcionales; distancia ≥ 4 salas (≥ 3 en showcase); máximo 2 enemigos por sala; presupuesto dentro de mínimo y máximo; 2 documentos de Emilia en salas secundarias.

**Resultado obtenido:**

**Estado:**

### CP-02 Muerte y reconstrucción

**Objetivo:** El ciclo funciona (S-08).

**Precondiciones:** Partida en el piso 1 tras el primer intento, con objetos y un documento registrado.

**Pasos:**

1. Dejarse matar por un enemigo.
2. Ver la cinemática corta; repetir saltándola con `Espacio`.

**Resultado esperado:** Inventario vacío; terminal con el documento; aparición en la cápsula del piso 1; distribución del piso distinta; código de sujeto +1; contador +1. Igual resultado con "Reiniciar".

**Resultado obtenido:**

**Estado:**

### CP-03 Autoguardado y salida

**Objetivo:** Guardar y continuar según la tabla de salida (S-07).

**Precondiciones:** Partida con autoguardado en la sala final del piso 1.

**Pasos:**

1. Salir con calma a mitad del piso 2; Continuar.
2. Salir durante una caza; Continuar.
3. Cerrar el proceso desde el administrador de tareas; Continuar.

**Resultado esperado:** 1: aparece en la sala final del piso 1 con el inventario de llegada, contador +1. 2: inicio del ciclo con muerte completa. 3: último autoguardado sin costo.

**Resultado obtenido:**

**Estado:**

### CP-04 Vigilancia con costo

**Objetivo:** La vista de vigilancia respeta la señal escalonada (S-01).

**Precondiciones:** Sala con cámara y un Alto en la sala adyacente.

**Pasos:**

1. Activar `Q` con el Alto en patrulla durante 6 s.
2. Repetir con el Alto en búsqueda.
3. Provocar que entre en caza.

**Resultado esperado:** 1: tras ~5 s el Alto pasa a sospecha y búsqueda. 2: señal con más ruido y retraso. 3: la vista se corta y `Q` no la reactiva.

**Resultado obtenido:**

**Estado:**

### CP-05 The Mother y decisión final

**Objetivo:** El jefe y los finales se resuelven según S-10 y S-08.

**Precondiciones:** Ranura de depuración en el piso 3 con 0 y con 6 documentos de Emilia.

**Pasos:**

1. Completar las 3 fases sin munición al entrar.
2. Con 0 documentos, ver la victoria; con 6, elegir la terminal y luego (cargando) la salida.

**Resultado esperado:** La sala da 12 balas en la fase 2 y 8 en la fase 3; la fase 3 solo empieza con los 4 asimilados eliminados y Lukas en el núcleo; si llega al núcleo sin ser enfocado, salen los asimilados que falten (D-117); 0 documentos → Relevo básico sin elección; 6 → Relevo completo con la terminal y Ambiguo con la salida; todas terminan en el gancho y los créditos.

**Resultado obtenido:**

**Estado:**

### CP-06 Rendimiento

**Objetivo:** Cumplir [[03 - Requisitos Técnicos]].

**Precondiciones:** Build de desarrollo en el equipo de menor gama del equipo.

**Pasos:**

1. Jugar 10 min en el piso 3 con la linterna encendida y enemigos en caza.
2. Medir 3 transiciones de piso.

**Resultado esperado:** ≥ 30 FPS en el mínimo y 60 en el recomendado; generación + NavMesh ≤ 5 s.

**Resultado obtenido:**

**Estado:**

### CP-07 Autoguardado pendiente durante una caza

**Objetivo:** Llegar perseguido a la sala clave final no guarda hasta que termina la caza (S-07, D-96).

**Precondiciones:** Piso 1, sin autoguardado previo; un Alto en caza persigue al jugador.

**Pasos:**

1. Entrar en la sala clave final con el Alto en caza.
2. Comprobar que no aparece "sincronizando…" y que la escalera y el elevador muestran `LOCKDOWN`.
3. Perder al Alto o eliminarlo hasta que salga de la caza.
4. Salir del juego con calma y volver a cargar la ranura.

**Resultado esperado:** "sincronizando…" aparece solo al terminar la caza; al cargar, el jugador aparece en la sala clave final con el mismo inventario y la sala sin enemigos dentro (D-97).

**Resultado obtenido:**

**Estado:**


> **Navegación:** [[00 - Índice]] · ← [[01 - Criterios de Calidad]] · [[03 - Bugs y Seguimiento]] →
