# Criterios de Calidad

## Descripción de la sección

Define qué significa que una característica esté correctamente terminada.

## Calidad funcional

- [ ] Funciona según las reglas documentadas en [[02 - Mecánicas]], [[03 - Sistemas]] y [[07 - Reglas y Estados del Juego]].
- [ ] No produce errores conocidos críticos ni excepciones en la consola durante una partida completa.
- [ ] Maneja casos límite importantes: los casos especiales de [[07 - Reglas y Estados del Juego]] y los criterios de aceptación del sistema S-xx; en particular, morir durante una acción (recarga, curación, hackeo, remate), salir del juego durante una caza, el inventario lleno y pisos generados con semillas extremas.
- [ ] Funciona en las dos configuraciones (showcase y completa) y en los dos modos (Supervivencia e Historia) cuando aplica.

## Calidad de experiencia

- [ ] El jugador entiende el objetivo sin lista de misiones: entorno, líneas de protocolo y terminal (D-78).
- [ ] Las respuestas del juego son claras: cada estado de alerta y cada aviso tiene su señal (M-10, S-11).
- [ ] La dificultad es coherente con la curva de [[05 - Progresión]]; las muertes se perciben como consecuencia de una decisión, no como azar.
- [ ] Se respeta la vulnerabilidad: enfrentar a 2 enemigos a la vez es casi siempre perder ([[04 - Pilares de Diseño]]).
- [ ] La interfaz es comprensible: etiquetas de texto en el terminal (D-86) y ningún estado comunicado solo con color.
- [ ] Ningún indicador muestra la percepción de los enemigos (D-02).

## Calidad técnica

- [ ] Rendimiento aceptable: 60 FPS en el equipo recomendado y ≥ 30 en el mínimo ([[03 - Requisitos Técnicos]]).
- [ ] Sin errores críticos.
- [ ] Guardado estable: ninguna ranura se corrompe al cerrar el juego en cualquier momento.
- [ ] No existen dependencias rotas: el proyecto abre y compila sin errores en un clon limpio, y la compilación de Windows arranca en una máquina sin Unity.
- [ ] Todos los valores de balance están en ScriptableObjects, no en el código.

## Definición de terminado

Una característica se considera terminada cuando (D-113):

1. Cumple los criterios de aceptación de su sistema en [[03 - Sistemas]] y los de [[04 - Criterios de Aceptación]].
2. Su código está en `develop` mediante un *pull request* que otra persona del equipo probó en el editor o en una build con su caso de prueba antes de fusionarlo. No se exige revisión de código: una sola persona programa ([[02 - Roles y Responsabilidades]]).
3. Tiene prueba EditMode si contiene lógica pura (generación, validación, inventario, resistencia, contador, guardado; [[02 - Pruebas]]).
4. No tiene bugs críticos ni altos abiertos ([[03 - Bugs y Seguimiento]]).
5. No baja el rendimiento por debajo del objetivo en la escena de prueba.
6. Si cambió una regla, el GDD está actualizado y el cambio registrado ([[05 - Gestión de Cambios]]).
7. Funciona integrada en una partida real (piso generado), no solo en una escena de prueba.
8. Usa assets finales o tiene una tarjeta de assets pendiente en Trello ([[04 - Backlog del Proyecto]]).


> **Navegación:** [[00 - Índice]] · ← [[05 - Gestión de Cambios]] · [[02 - Pruebas]] →
