# Criterios de Calidad

## 1. Descripción de la sección

Define qué significa que una característica esté correctamente terminada.

## 2. Calidad funcional

Los valores numéricos de este documento son iniciales y están sujetos a balance.

- [ ] Funciona según las reglas de [[02 - Mecánicas]], [[03 - Sistemas]] y [[07 - Reglas y Estados del Juego]].
- [ ] Una partida completa termina sin errores críticos conocidos ni excepciones en la consola.
- [ ] Cumple los criterios de aceptación de su sistema en [[03 - Sistemas]] y los casos especiales de [[07 - Reglas y Estados del Juego]].
- [ ] Resiste los casos límite del proyecto. Son morir durante una recarga, una cura, un hackeo o un remate, salir del juego durante una caza, el inventario lleno y los pisos generados con semillas extremas.
- [ ] Funciona en la configuración completa y en la showcase, y en el Modo Supervivencia y el Modo Historia cuando aplica.

## 3. Calidad de experiencia

- [ ] El jugador entiende el objetivo sin lista de misiones. Lo guían el entorno, las líneas de protocolo y el terminal del brazo.
- [ ] Las respuestas del juego son claras. Cada estado de alerta y cada aviso del sistema tiene su señal, según Señales diegéticas y el sistema de audio dinámico.
- [ ] La dificultad sigue la curva de [[05 - Progresión]]. El jugador percibe cada muerte como consecuencia de una decisión y no como azar.
- [ ] Se respeta la vulnerabilidad de [[04 - Pilares de Diseño]]. Enfrentar a 2 enemigos a la vez es casi siempre perder.
- [ ] La interfaz es comprensible. El terminal usa etiquetas de texto y ningún estado se comunica solo con color.
- [ ] Ningún indicador muestra la percepción de los enemigos.

## 4. Calidad técnica

- [ ] El rendimiento alcanza 60 FPS en el equipo recomendado y no baja de 30 FPS en el mínimo. Ver [[03 - Requisitos Técnicos]].
- [ ] No hay errores críticos.
- [ ] El guardado es estable. Ninguna ranura se corrompe al cerrar el juego en cualquier momento.
- [ ] No hay dependencias rotas. El proyecto abre y compila sin errores en un clon limpio, y la compilación de Windows arranca en una máquina sin Unity.
- [ ] Los valores de balance viven en ScriptableObjects y no en el código. Los de los sistemas creados en tiempo de ejecución están en `SystemsBalance`.

## 5. Definición de terminado

Una característica se considera terminada cuando:

1. Cumple los criterios de aceptación de su sistema en [[03 - Sistemas]] y los de [[04 - Criterios de Aceptación]].
2. Su código entra en `develop` por un *pull request*. Antes de fusionarlo, otra persona del equipo lo prueba en el editor o en una build con su caso de prueba.
3. No se exige revisión de código, porque una sola persona programa. Ver [[02 - Roles y Responsabilidades]].
4. Tiene prueba EditMode si contiene lógica pura, como generación, validación, inventario, resistencia, contador o guardado. Ver [[02 - Pruebas]].
5. No tiene bugs críticos ni altos abiertos. Ver [[03 - Bugs y Seguimiento]].
6. No baja el rendimiento por debajo del objetivo en la escena de prueba.
7. Si cambió una regla, el GDD está actualizado y el cambio registrado en [[05 - Gestión de Cambios]].
8. Funciona integrada en una partida real, con un piso generado. Una escena de prueba no basta.
9. Usa assets finales o tiene una tarea de assets pendientes en el Excel de seguimiento `CronogramaIdentityDissolution_Objetivos_v3.xlsx`. Ver [[04 - Backlog del Proyecto]].


> **Navegación:** [[00 - Índice]] · ← [[05 - Gestión de Cambios]] · [[02 - Pruebas]] →
