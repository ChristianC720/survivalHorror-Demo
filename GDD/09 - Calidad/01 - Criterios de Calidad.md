# Criterios de Calidad

## Descripción de la sección

Define qué significa que una característica esté correctamente terminada.

## Calidad funcional

- [ ] Funciona según las reglas documentadas en [[02 - Mecánicas]], [[03 - Sistemas]] y [[07 - Reglas y Estados del Juego]].
- [ ] No produce errores conocidos críticos ni excepciones en la consola durante una partida completa.
- [ ] Maneja casos límite importantes: los casos especiales de [[07 - Reglas y Estados del Juego]] y los criterios de aceptación del sistema S-xx.
- [ ] Funciona en las dos configuraciones (showcase y completa) y en los dos modos (Supervivencia e Historia) cuando aplica.

## Calidad de experiencia

- [ ] El jugador entiende el objetivo sin lista de misiones: entorno, líneas de protocolo y terminal (D-78).
- [ ] Las respuestas del juego son claras: cada estado de alerta y cada aviso tiene su señal (M-10, S-11).
- [ ] La dificultad es coherente con la curva de [[05 - Progresión]].
- [ ] La interfaz es comprensible: etiquetas de texto en el terminal (D-86) y ningún estado comunicado solo con color.
- [ ] Ningún indicador muestra la percepción de los enemigos (D-02).

## Calidad técnica

- [ ] Rendimiento aceptable: 60 FPS en el equipo recomendado y ≥ 30 en el mínimo ([[03 - Requisitos Técnicos]]).
- [ ] Sin errores críticos.
- [ ] Guardado estable: ninguna ranura se corrompe al cerrar el juego en cualquier momento.
- [ ] No existen dependencias rotas: el proyecto abre y compila sin errores en un clon limpio.
- [ ] Todos los valores de balance están en ScriptableObjects, no en el código.

## Definición de terminado

Una característica se considera terminada cuando (D-113):

1. Cumple los criterios de aceptación de su sistema en [[03 - Sistemas]] y los de [[04 - Criterios de Aceptación]].
2. Su código está en `develop` mediante un *pull request* revisado por otra persona del equipo.
3. Tiene prueba EditMode si contiene lógica pura (generación, validación, inventario, resistencia, contador, guardado; [[02 - Pruebas]]).
4. Otra persona la probó en una build o en el editor siguiendo su caso de prueba, sin bugs críticos ni altos abiertos.
5. No baja el rendimiento por debajo del objetivo en la escena de prueba.
6. Si cambió una regla, el GDD está actualizado y el cambio registrado ([[05 - Gestión de Cambios]]).


> **Navegación:** [[00 - Índice]] · ← [[05 - Gestión de Cambios]] · [[02 - Pruebas]] →
