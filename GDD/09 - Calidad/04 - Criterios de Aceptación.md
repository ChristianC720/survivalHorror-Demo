# Criterios de Aceptación

## Descripción de la sección

Los criterios de aceptación permiten decidir objetivamente si una característica está lista.

Cada sistema tiene además su lista detallada en [[03 - Sistemas]]; aquí va un criterio Dado / Cuando / Entonces por mecánica, que sirve como prueba rápida de la característica completa. Los valores son iniciales, sujetos a balance.

## Plantilla

### Característica

**Dado que:** <!-- contexto inicial -->

**Cuando:** <!-- acción -->

**Entonces:** <!-- resultado esperado -->

## Criterios por mecánica

| Mecánica | Dado que | Cuando | Entonces |
|---|---|---|---|
| M-01 Cámara OTS y apuntado | Lukas explora | Mantiene `Clic derecho` | La cámara se acerca al hombro, aparece la retícula, la velocidad baja y no puede correr |
| M-02 Vista de vigilancia | Lukas está en una sala con cámara y sin enemigos en caza | Pulsa `Q` | Ve la sala en isométrica con ruido y retraso, solo de ese piso, y solo puede quedarse quieto o caminar muy lento |
| M-03 Bloqueo de vigilancia | Lukas usa la vigilancia | Un enemigo cercano entra en caza o recibe daño | La vista se corta al instante y `Q` no la reactiva hasta que termina la caza o cruza una puerta |
| M-04 Combate a distancia | Lukas apunta con 6 balas cargadas a un Alto | Le acierta 4 disparos | El Alto muere; el contador del armazón muestra 2; el disparo alertó a enemigos a ~20 m |
| M-05 Salud y estados | Lukas está en Bien | Recibe un golpe de un Alto y espera 60 s | Pasa a Herido, camina un 15% más lento y no recupera salud sola |
| M-06 Resistencia y empujón | La resistencia está al 30% | Intenta empujar (35%) | El empujón no ocurre y suena un jadeo |
| M-07 Inventario | El inventario está abierto con un enemigo en caza | Usa una cura (~1.5 s) | El enemigo sigue moviéndose y atacando; si golpea durante la animación, la cura no se consume |
| M-08 Interacción y documentos | Lukas está a ~1.5 m de un documento de Emilia | Pulsa `E` | Aparece `REGISTERED`, el expediente sube a n/6 en el terminal y el juego nunca se detiene |
| M-09 Sigilo y ruido | Lukas va agachado tras una cobertura sólida con la linterna apagada | Un Alto pasa a 5 m | El Alto no lo detecta |
| M-10 Señales diegéticas | Un Rastrero pasa de patrulla a sospecha, búsqueda y caza | Lukas escucha | Suena un sonido distinto en cada estado y no aparece ningún indicador en pantalla |
| M-11 Punzón de descarga | Lukas tiene un punzón y un Alto lo agarra | Se resuelve el agarre | El punzón se consume solo, el Alto queda aturdido 2 s y Lukas no pierde salud |
| M-12 Linterna | La linterna lleva ~120 s encendida | Se agota la carga | Parpadea y se apaga; `F` no la enciende durante ~10 s; se recarga sola tras ~3 s apagada y se llena en ~30 s |
| M-13 Hackeo | Lukas está ante un panel de puerta | Introduce una entrada incorrecta de la secuencia | Suena el pitido de error y la secuencia se reinicia; Lukas sigue inmóvil y el juego no se pausa |
| M-14 Escape y retirada | Un enemigo en caza persigue a Lukas | Lukas cruza una puerta y la sella con el hackeo rápido (2-3 entradas) | La persecución se corta; un enemigo que sigue ahí rompe el sellado en ~30 s, y el sellado también se rompe si Lukas vuelve a cruzar la puerta |
| M-15 Exploración vertical | Lukas llega a la sala final del piso 1 con la llave y sin enemigos en caza | Usa la escalera | Se crea el autoguardado, la transición dura ≥ 5 s sin pantalla de carga y aparece en el piso 2 |
| M-16 Reconstrucción y terminal | Lukas muere con 3 objetos y 2 documentos | Termina la cinemática corta | Reaparece en la cápsula del piso 1 sin objetos, con los 2 documentos y el código R-H siguiente |
| M-17 Fallas de la máquina (Deseable) | Lukas descubrió una falla | La usa 2 veces | The Mother la parchea; el registro sigue en el terminal |
| M-18 Degradación y Asimilación (Deseable) | El contador va en 4 tras el piso 2 | Lukas muere otra vez | Se reproduce el final Asimilación y después los créditos |
| M-19 The Mother | Lukas entra en la sala sin munición | Hackea 3 nodos, elimina a los 4 asimilados con las 12 balas de la sala y llega al núcleo | Empieza la fase 3 con 8 balas junto al núcleo y 3 transformadores de 2 disparos; al destruirlos cae The Mother |
| M-20 Reciclaje y remate | Un Rastrero muerto queda en la sala | Lukas lo remata con el punzón (`E`) | El cuerpo no se recicla y ningún enemigo cercano se alerta |

## Criterios del proyecto

- [ ] El bucle principal funciona: explorar, gestionar, combatir o evadir, morir y reconstruirse con un piso distinto.
- [ ] Las condiciones de victoria y derrota funcionan: victoria al derrotar a The Mother con el final que corresponde; la muerte reconstruye (y, si M-18 entra, la quinta activa la Asimilación).
- [ ] El progreso puede guardarse: autoguardado, 3 ranuras, terminal persistente y la tabla de salida de [[07 - Reglas y Estados del Juego]].
- [ ] Los sistemas principales están integrados: S-01..S-11 se comunican según [[03 - Sistemas]].
- [ ] Las pruebas críticas están aprobadas: CP-01..CP-06 de [[02 - Pruebas]].
- [ ] El rendimiento cumple el objetivo definido en [[03 - Requisitos Técnicos]].
- [ ] No existen errores bloqueantes conocidos ([[03 - Bugs y Seguimiento]]).
- [ ] Una partida showcase completa dura ~30 min sin muertes (D-34).


> **Navegación:** [[00 - Índice]] · ← [[03 - Bugs y Seguimiento]] · [[01 - Glosario]] →
