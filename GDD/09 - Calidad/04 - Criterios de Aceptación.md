# Criterios de Aceptación

## 1. Descripción de la sección

Los criterios de aceptación permiten decidir objetivamente si una característica está lista.

## 2. Plantilla

Cada sistema tiene su lista detallada de criterios en [[03 - Sistemas]]. Esta nota agrega un criterio *Dado que, Cuando, Entonces* por mecánica, que sirve como prueba rápida de la característica completa. Las reglas de cada mecánica están en [[02 - Mecánicas]]. Los valores numéricos de este documento son iniciales y están sujetos a balance.

### 2.1 Característica

| Característica | Dado que | Cuando | Entonces |
|---|---|---|---|
| Cámara sobre el hombro y apuntado | Lukas explora | Mantiene `Clic derecho` | La cámara se acerca al hombro, aparece la retícula, la velocidad baja y no puede correr |
| Vista de vigilancia | Lukas está en una sala con cámara y sin enemigos en caza | Pulsa `Q` | Ve la sala en isométrica, con ruido y retraso y solo de ese piso. Solo puede quedarse quieto o caminar muy lento |
| Bloqueo de vigilancia por detección | Lukas usa la vigilancia | Un enemigo cercano entra en caza o Lukas recibe daño | La vista se corta al instante. `Q` no la reactiva hasta que termina la caza o el perseguidor queda fuera de su sala y de las conectadas |
| Combate a distancia | Lukas apunta a un Alto con 6 balas cargadas | Le acierta 4 disparos | El Alto muere y el contador del armazón muestra 2. El disparo alertó a los enemigos a ~20 m |
| Salud y estados | Lukas está en Bien | Recibe un golpe de un Alto y espera 60 s | Pasa a Herido, camina un 15% más lento y no recupera salud sola |
| Resistencia y empujón | La resistencia está al 30% | Intenta empujar, que cuesta 35% | El empujón no sale y suena un jadeo |
| Inventario de 6 casillas en tiempo real | El inventario está abierto con un enemigo en caza | Usa una cura de ~1,5 s | El enemigo sigue moviéndose y atacando. Si golpea durante la animación, la cura no se gasta |
| Interacción y documentos | Lukas está a ~1,5 m de un documento de Emilia | Pulsa `E` | Se abre la vista de lectura y el juego se detiene hasta cerrarla. Al cerrar aparece `REGISTERED` y el expediente sube a n/6 en el terminal |
| Cono de interacción | Una puerta está a ~1 m detrás de Lukas y la cámara mira al frente | Pulsa `E` | No aparece el aviso y la puerta no se abre. Al girar la cámara hacia ella aparece `[E]` y se abre |
| Tutorial sin muerte | Es el primer intento y Lukas está en Crítico frente al Rastrero de la sala de calderas | El Rastrero lo golpea o lo agarra sin punzón | Lukas sigue vivo en Crítico. No hay cinemática de muerte ni suma al contador |
| Sigilo y ruido | Lukas va agachado tras una cobertura sólida con la linterna apagada | Un Alto pasa a 5 m | El Alto no lo detecta |
| Señales diegéticas | Un Rastrero pasa de patrulla a sospecha, búsqueda y caza | Lukas escucha | Cada estado tiene un sonido distinto y no aparece ningún indicador en pantalla |
| Punzón de descarga | Lukas tiene un punzón y un Alto lo agarra | Se resuelve el agarre | El punzón se gasta solo, el Alto queda aturdido 2 s y Lukas no pierde salud |
| Linterna | La linterna lleva ~120 s encendida | Se agota la carga | Parpadea y se apaga, y `F` no la enciende durante ~10 s. Se recarga sola tras ~3 s apagada y se llena en ~30 s |
| Hackeo por secuencias | Lukas está ante un panel de puerta | Introduce una entrada incorrecta | Suena el pitido de error y la secuencia se reinicia. Lukas sigue inmóvil y el juego no se pausa |
| Escape de combate y retirada | Un enemigo en caza persigue a Lukas | Lukas cruza una puerta y la sella con el hackeo rápido de 2 a 3 entradas | La persecución se corta. Un enemigo que sigue ahí rompe el sellado en ~30 s, y el sellado también se rompe si Lukas vuelve a cruzar |
| Exploración vertical por pisos | Lukas llega a la sala final del piso 1 con la llave y sin enemigos en caza | Usa la escalera | Se crea el autoguardado, la transición dura al menos 5 s sin pantalla de carga y Lukas aparece en el piso 2 |
| Reaparición por reconstrucción y terminal del brazo | Lukas llega al piso 2 con 1 objeto, recoge 2 más y tiene 2 documentos | Muere y termina la cinemática corta | Reaparece al inicio del piso 2 con el objeto con el que llegó, los 2 documentos y el código R-H siguiente |
| Fallas de la máquina, deseable | Lukas descubrió una falla | La usa 2 veces | The Mother la parchea y el registro sigue en el terminal |
| Degradación por muerte y final de 5 muertes, deseable | El contador va en 4 después del piso 2 | Lukas muere otra vez | Se reproduce el final Asimilación y después los créditos |
| Jefe final: The Mother | Lukas entra en la sala sin munición | Hackea 3 nodos, elimina a los 4 asimilados con las 12 balas de la sala y llega al núcleo | Empieza la fase 3 con 8 balas junto al núcleo y 3 transformadores de 2 disparos. Al destruirlos cae The Mother |
| Reciclaje de enemigos y remate | Un Rastrero muerto queda en la sala | Lukas lo remata con el punzón usando `E` | El cuerpo no se recicla y ningún enemigo cercano se alerta |

## 3. Criterios del proyecto

Se comprueban en la configuración completa, que es la predeterminada. La showcase se comprueba con el recorrido corto, lanzando el juego con `-showcase` ([[01 - Alcance]]).

- [ ] El bucle principal funciona. El jugador explora, gestiona, combate o evade, muere y se reconstruye en un piso distinto.
- [ ] Las condiciones de victoria y derrota funcionan. Derrotar a The Mother da el final que corresponde y la muerte reconstruye a Lukas.
- [ ] Si la degradación entra en la demo, la quinta muerte contada activa la Asimilación.
- [ ] El progreso puede guardarse con el autoguardado, las 3 ranuras, el terminal persistente y la tabla de salida de [[07 - Reglas y Estados del Juego]].
- [ ] Los 11 sistemas principales se comunican según [[03 - Sistemas]].
- [ ] Los casos críticos de [[02 - Pruebas]] están aprobados.
- [ ] El rendimiento cumple el objetivo de [[03 - Requisitos Técnicos]].
- [ ] No existen errores bloqueantes conocidos ([[03 - Bugs y Seguimiento]]).
- [ ] Una partida completa sin muertes dura ~45 a 60 min. Con `-showcase` dura ~30 min.
- [ ] Un jugador que no conoce el GDD cumple el criterio mínimo de la demo de [[01 - Ficha del Proyecto]].


> **Navegación:** [[00 - Índice]] · ← [[03 - Bugs y Seguimiento]] · [[01 - Glosario]] →
