# Seguridad y Riesgos Técnicos

## 1. Descripción de la sección

Documenta riesgos técnicos y medidas para reducir fallos, pérdida de datos o abuso de sistemas.

## 2. Riesgos

Los valores numéricos de este documento son iniciales y están sujetos a balance. Las fechas salen del [[03 - Roadmap]].

| Riesgo | Probabilidad | Impacto | Mitigación |
|---|---|---|---|
| El generador produce pisos imposibles, sin ruta, con una llave inaccesible o con una distancia corta | Media | Crítico | Generar primero el grafo abstracto y validarlo con `LayoutValidator` antes de instanciar. Si falla, `LayoutGenerator` reintenta con la semilla+1, hasta 20 intentos (`MaxAttempts = 20`). Una prueba EditMode recorre 1 000 semillas por configuración ([[02 - Pruebas]]) |
| Los 20 intentos fallan y el piso no se construye | Baja | Crítico | `FloorBuilder` registra el error con el piso y la semilla y devuelve `false`. No hay piso de respaldo. La prueba de 1 000 semillas debe pasar sin fallos antes de cada build, y la semilla del log permite reproducir el caso |
| El NavMesh en tiempo de ejecución tarda más de 5 s o deja huecos en las puertas | Media | Alto | Un solo `NavMeshSurface` por piso, construido durante la transición. Las puertas usan `NavMeshObstacle` con *carve*. Se mide durante la generación procedural del hito H3, del 1 al 7 de noviembre de 2026 |
| Rendimiento bajo por luces dinámicas y sombras | Media | Alto | *Forward+*, 4 luces con sombra como máximo, salas lejanas desactivadas y render a ~540p ([[03 - Requisitos Técnicos]]) |
| Plazo de desarrollo insuficiente, con una sola persona programando | Alta | Crítico | MVP al cierre del hito H3, el 7 de noviembre de 2026. Recortes en orden ([[01 - Alcance]]). Congelación de funciones al cierre de contenido, el 12 de noviembre de 2026 |
| El hito H5 queda en 2 días, del 11 al 12 de noviembre, y concentra tres pisos, The Mother, cinemáticas, menús y música | Alta | Crítico | Aplicar los criterios de recorte de [[01 - Alcance]] en orden y priorizar el recorrido completo |
| Las pruebas pierden 3 de sus 7 días y coinciden con el cierre del hito H5 | Alta | Alto | Las pruebas corren del 9 al 12 de noviembre en paralelo con el contenido. El 13 de noviembre queda libre para la build y la entrega |
| Corrupción o pérdida del guardado | Baja | Alto | Escritura atómica con `.tmp` y `.bak`, `saveVersion` y migraciones ([[04 - Datos y Persistencia]]) |
| Conflictos de *merge* en escenas y prefabs | Alta | Medio | Una persona por escena o prefab, salas como prefabs independientes, *Force Text* y ramas cortas |
| Binarios grandes en Git | Media | Medio | Git LFS ya configurado en `.gitattributes`. Nunca se sube `Library/` |
| Estados combinados no previstos, como morir en un hackeo, salir en una caza o una quinta muerte por `Reiniciar` | Media | Medio | Una única `GameStateMachine` con la tabla de transiciones de [[07 - Reglas y Estados del Juego]] y casos de prueba por caso especial |
| Unity 6.6 no es LTS y deja de recibir parches cuando sale la siguiente versión | Alta | Bajo | Fijar `6000.6.8f1` hasta terminar la demo. Cambiar de versión solo al inicio de un hito y como solicitud de cambio ([[05 - Gestión de Cambios]]) |
| Comportamiento de la IA difícil de reproducir | Media | Medio | Semilla del piso visible y consola de depuración con el estado de alerta de cada enemigo y la atención de The Mother ([[03 - Bugs y Seguimiento]]) |

## 3. Datos sensibles

El juego no tiene cuentas, red ni datos personales. Solo guarda progreso y configuración en el equipo local ([[04 - Datos y Persistencia]]). Los archivos JSON no se cifran, así que modificarlos solo afecta la partida de quien lo hace.

El juego tiene un modo de telemetría local y anónima para los *playtests*. Se activa al lanzar el juego con el argumento `-telemetry`, igual que `-showcase`. Escribe un archivo en `Application.persistentDataPath` con los datos para las métricas de [[02 - Pruebas]]. Sin el argumento, el juego no escribe telemetría.

## 4. Multijugador

No aplica, porque el juego es de un solo jugador y sin conexión ([[03 - Público y Plataforma]]).

- **Autenticación:** No aplica.
- **Validación:** El juego valida sus propios archivos al cargar, por versión y formato.
- **Autoridad del servidor:** No aplica.
- **Comunicación:** No aplica.
- **Protección contra manipulación:** No se busca, porque es un juego local de un jugador.

## 5. Recuperación

- **Corrupción de datos.** La ranura aparece dañada, y se ofrece restaurar el `.bak` o borrarla. Una configuración global dañada vuelve a los valores por defecto y repite la calibración de brillo.
- **Pérdida de conexión.** No aplica.
- **Fallo del programa.** Al volver se aplica la regla de cierre inesperado de [[07 - Reglas y Estados del Juego]], sin costo y desde el último autoguardado. El terminal del brazo no pierde entradas, porque se guarda en cada cambio.
- **Piso inválido tras 20 intentos.** `FloorBuilder` registra el error en el log con el piso y la semilla y devuelve `false`. No se carga ningún piso de respaldo.

> **Navegación:** [[00 - Índice]] · ← [[04 - Datos y Persistencia]] · [[01 - Alcance]] →
