# Seguridad y Riesgos Técnicos

## Descripción de la sección

Documenta riesgos técnicos y medidas para reducir fallos, pérdida de datos o abuso de sistemas.

## Riesgos

| Riesgo | Probabilidad | Impacto | Mitigación |
|---|---|---|---|
| El generador produce pisos imposibles (sin ruta, llave inaccesible, distancia corta) | Media | Crítico | Generar primero el grafo abstracto y validarlo con `LayoutValidator` antes de instanciar; si falla, nueva semilla (máximo 20 reintentos y después un piso de respaldo fijo). Prueba EditMode con 1 000 semillas por configuración ([[02 - Pruebas]]) |
| El NavMesh en tiempo de ejecución tarda más de 5 s o deja huecos en las puertas | Media | Alto | Un solo `NavMeshSurface` por piso construido durante la transición; puertas con `NavMeshObstacle` *carve*; medir en la semana 2 |
| Rendimiento bajo por luces dinámicas y sombras | Media | Alto | *Forward+*, ≤ 4 luces con sombra, salas lejanas desactivadas, render a ~540p ([[03 - Requisitos Técnicos]]) |
| Plazo de 4 semanas de desarrollo insuficiente, con una sola persona programando | Alta | Crítico | MVP el 2026-11-08, recortes en orden ([[01 - Alcance]]), congelación de funciones el 2026-11-15 |
| Corrupción o pérdida del guardado | Baja | Alto | Escritura atómica con `.tmp` y `.bak`, `saveVersion` y migraciones ([[04 - Datos y Persistencia]]) |
| Conflictos de merge en escenas y prefabs | Alta | Medio | Una persona por escena o prefab; salas como prefabs independientes; *Force Text*; ramas cortas |
| Binarios grandes en Git | Media | Medio | Git LFS ya configurado en `.gitattributes`; nunca subir `Library/` |
| Animaciones de Mixamo incompatibles con el esqueleto compartido | Baja | Medio | Validar un clip por personaje en la semana 1 con el *Avatar* Humanoid (D-88) |
| Estados combinados no previstos (morir en hackeo, salir en caza, quinta muerte por Reiniciar) | Media | Medio | Una única `GameStateMachine` con la tabla de transiciones de [[07 - Reglas y Estados del Juego]] y casos de prueba por caso especial |
| Unity 6.6 no es LTS: deja de recibir parches cuando sale la siguiente versión | Alta | Bajo | Fijar `6000.6.6f1` hasta terminar la demo; cambiar de versión solo al inicio de un hito y como solicitud de cambio ([[05 - Gestión de Cambios]]) |
| Comportamiento de la IA difícil de reproducir | Media | Medio | Semilla del piso visible y consola de depuración con el estado de alerta de cada enemigo y la atención de The Mother ([[03 - Bugs y Seguimiento]]) |

## Datos sensibles

No. El juego no tiene cuentas, red ni datos personales. Solo guarda progreso y configuración en el equipo local ([[04 - Datos y Persistencia]]). Los archivos JSON no se cifran: modificarlos solo afecta a la partida de quien lo hace.

Las compilaciones de *playtest* pueden escribir una telemetría local y anónima (un archivo en `Application.persistentDataPath`) para las métricas de [[02 - Pruebas]]; la compilación de entrega no la incluye.

## Multijugador

No aplica: un solo jugador, sin conexión ([[03 - Público y Plataforma]]).

- Autenticación: no aplica.
- Validación: el juego valida sus propios archivos al cargar (versión y formato).
- Autoridad del servidor: no aplica.
- Comunicación: no aplica.
- Protección contra manipulación: no se busca; es un juego local de un jugador.

## Recuperación

- **Corrupción de datos:** la ranura aparece dañada; se ofrece restaurar `.bak` o borrarla. La configuración global dañada vuelve a los valores por defecto y repite la calibración de brillo.
- **Pérdida de conexión:** no aplica.
- **Fallo del programa:** al volver se aplica la regla de cierre inesperado (sin costo, último autoguardado; D-31, D-96). El terminal del brazo no pierde entradas porque se guarda en cada cambio (S-07).
- **Piso inválido tras 20 reintentos:** se carga el piso de respaldo fijo y se registra un error en el log para depuración.


> **Navegación:** [[00 - Índice]] · ← [[04 - Datos y Persistencia]] · [[01 - Alcance]] →
