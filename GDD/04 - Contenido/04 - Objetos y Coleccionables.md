# Objetos y Coleccionables

## Descripción de la sección

Define objetos que el jugador puede recoger, utilizar, equipar, vender, intercambiar o descubrir.

En la demo no se vende ni se intercambia nada. El inventario tiene 6 casillas y **5 tipos de objeto** (D-71); el arma y la linterna no ocupan casilla. Los presupuestos por piso están en [[06 - Economía y Recursos]]. Todos los valores son iniciales, sujetos a balance.

| Objeto | Tipo | Casilla | Persiste al morir |
|---|---|---|---|
| Revólver de Kessler | Arma | No | Sí: The Mother lo devuelve descargado (D-74) |
| Linterna | Equipo | No | Sí |
| Cura | Consumible | 1 por casilla | No |
| Munición | Consumible | 12 balas por casilla | No |
| Punzón de descarga | Consumible | 1 por casilla | No |
| Llave obligatoria / opcional | Objeto clave | 1 por casilla | No |
| Anillo | Registro | No | Sí, como registro del terminal |
| Documentos | Registro | No | Sí |
| Objetos confiscados | Coleccionable | No | Sí |

## Plantilla

### Revólver de Kessler

**Tipo:** Arma (única de la demo).

**Descripción:** Revólver de servicio del Werkschutz de Kessler Sicherheit AG, de gama alta y diseño de 2080: armazón cerámico y polímero mate, cañón alineado con la recámara inferior y contador de balas en el armazón (D-74, D-75).

**Función:** Eliminar o interrumpir enemigos y destruir los transformadores de The Mother (M-04, M-19).

**Cómo se obtiene:** En el primer intento, en el cadáver asimilado de un guardia de Kessler (tutorial, sala 2). Desde entonces, The Mother lo deja en la bandeja de efectos personales junto a la cápsula en cada reconstrucción, también si Lukas murió antes de recogerlo (D-74).

**Cómo se utiliza:** Mantener `Clic derecho` para apuntar y `Clic izquierdo` para disparar; `R` recarga bala a bala (M-04).

**Cantidad máxima:** 1.

**Valor:** —

**Rareza:** Único.

**Restricciones:** Tambor de 6 balas; ~0.8 s entre disparos. Solo dispara con una identidad autorizada: el terminal del brazo identifica a Lukas como activo de Vitalis. Siempre vuelve descargado.

**Interacciones:** Los disparos alertan a ~20 m y llenan un poco la atención de The Mother (M-09, D-24); los puntos débiles interrumpen ataques (D-74).

**Representación visual:** Logotipo "KESSLER WERKSCHUTZ" y número de inventario de Vitalis. El contador del armazón sustituye al indicador de munición del HUD.

### Linterna

**Tipo:** Equipo.

**Descripción:** Linterna integrada en el equipo de Lukas.

**Función:** Ver en la oscuridad a cambio de delatarse (M-12).

**Cómo se obtiene:** Siempre disponible.

**Cómo se utiliza:** `F` para encender y apagar.

**Cantidad máxima:** 1.

**Valor:** —

**Rareza:** —

**Restricciones:** Barra de carga (D-71): ~120 s de uso continuo; tras ~3 s apagada se recarga sola y se llena de 0 a 100% en ~30 s. En 0 parpadea, se apaga y no se puede encender durante ~10 s (D-124).

**Interacciones:** La luz duplica la velocidad de alerta de los enemigos que ilumina y activa al Mudo; la Boca es ciega (M-09).

**Representación visual:** Indicador de carga en el terminal del brazo.

### Cura

**Tipo:** Consumible.

**Descripción:** Kit médico de Vitalis.

**Función:** Recuperar un estado de salud; en Bien con daño, devuelve la salud completa (M-05, D-119).

**Cómo se obtiene:** Presupuesto del piso (D-17).

**Cómo se utiliza:** Desde el inventario (~1.5 s, M-07); no se usa con la salud completa.

**Cantidad máxima:** 1 por casilla.

**Valor:** —

**Rareza:** Escasa.

**Restricciones:** Un golpe durante la animación la cancela sin consumirla.

**Interacciones:** M-05, M-07.

**Representación visual:** estuche rectangular de plástico rígido blanco sucio con asa, ribete carmesí y logotipo de Vitalis; dentro, un autoinyector. Etiqueta en alemán *Regenerationsmittel*. Silueta reconocible por el asa.

### Munición

**Tipo:** Consumible.

**Descripción:** Blíster de **4 cartuchos** sin vaina, de cebado eléctrico, con código de inventario de Vitalis (D-74).

**Función:** Cargar el revólver.

**Cómo se obtiene:** Presupuesto del piso (D-17); la sala de The Mother da 20 balas fijas: 12 para la fase 2 y 8 para la fase 3 (D-80, D-115).

**Cómo se utiliza:** `R` carga bala a bala (~0.7 s por bala) desde el inventario; se puede cancelar conservando las balas ya cargadas.

**Cantidad máxima:** 12 balas por casilla (3 blísteres, D-122).

**Valor:** —

**Rareza:** Escasa.

**Restricciones:** Se pierde al morir.

**Interacciones:** M-04, M-07.

**Representación visual:** blíster transparente con 4 cartuchos cobrizos sin vaina y tapa de aluminio con código de inventario de Vitalis (D-122).

### Punzón de descarga

**Tipo:** Consumible de un solo uso con 2 funciones (D-71).

**Descripción:** Equipo de contención de sujetos de Kessler: un punzón que libera una descarga al clavarse.

**Función:** Defensa contra agarres **o** remate silencioso (M-11, M-20).

**Cómo se obtiene:** Presupuesto del piso (D-17).

**Cómo se utiliza:** Se activa solo al ser agarrado y aturde al enemigo 2 s. Con `E` sobre un cuerpo, quema su último rastro de identidad y el laboratorio ya no puede reciclarlo (~1.5 s).

**Cantidad máxima:** 1 por casilla.

**Valor:** —

**Rareza:** Escasa.

**Restricciones:** Gastarlo en un remate deja a Lukas sin defensa ante el próximo agarre. `E` no se activa sobre cuerpos que no pueden reciclarse (M-20).

**Interacciones:** M-07, M-11, M-20.

**Representación visual:** cilindro negro mate del largo de un antebrazo con empuñadura de goma, punta metálica y franja ámbar de aviso; logotipo "KESSLER WERKSCHUTZ" y un LED verde fósforo que indica carga.

### Llave obligatoria / Llave opcional

**Tipo:** Objeto clave (D-36).

**Descripción:** Llave de acceso de Vitalis.

**Función:** La obligatoria abre la sala clave final del piso (en el piso 3, la sala de The Mother); las opcionales abren atajos o salas con recursos.

**Cómo se obtiene:** 1 obligatoria por piso, alcanzable antes de la puerta; opcionales según el presupuesto.

**Cómo se utiliza:** `E` en su puerta; se consume.

**Cantidad máxima:** 1 por casilla.

**Valor:** —

**Rareza:** Fija (obligatoria) / escasa (opcional).

**Restricciones:** Con el inventario lleno puede reemplazar otro objeto (M-07). El mínimo garantizado de cada recurso nunca queda detrás de una llave opcional (D-79).

**Interacciones:** M-07, M-08, M-15.

**Representación visual:** tarjeta magnética de Vitalis con número de sala. La obligatoria es una placa más grande con banda carmesí y un icono de escalera; la opcional es una tarjeta estándar con banda gris. Se distinguen por forma e icono, no solo por color.

### Anillo

**Tipo:** Registro.

**Descripción:** Anillo que Lukas sabe importante; nunca se sabe si es el suyo o el de Emilia (D-54).

**Función:** Narrativa.

**Cómo se obtiene:** Del suelo de la fila de cápsulas, solo tras la cinemática completa de reconstrucción.

**Cómo se utiliza:** Se examina en el terminal del brazo.

**Cantidad máxima:** 1.

**Valor:** —

**Rareza:** Único.

**Restricciones:** No ocupa casilla. Se recoge una sola vez y queda como registro del terminal, sin presencia física después.

**Interacciones:** M-16.

**Representación visual:** alianza de acero sencilla y gastada; la inscripción interior está raspada y es ilegible, para que nunca se sepa de quién es (D-54).

### Documentos

**Tipo:** Registro.

**Descripción:** 6 documentos de Emilia (D-52) y **12 documentos más** (D-79): piso 1, memorandos de Vitalis y partes de guardia de Kessler; piso 2, informes clínicos de The Mother; piso 3, registros del incidente.

**Función:** Historia; los 6 de Emilia desbloquean la decisión final (D-51).

**Cómo se obtiene:** Los de Emilia, 2 por piso en salas clave secundarias. Los demás, 4 por piso; aparecen 1–2 por intento en salas procedurales, dando prioridad a los no registrados.

**Cómo se utiliza:** `E` para leer; se registran en el terminal (M-08).

**Cantidad máxima:** —

**Valor:** —

**Rareza:** —

**Restricciones:** Si el documento de Emilia de una sala ya está registrado, la sala da 1 recurso extra del tipo más escaso del piso en ese intento, por encima del presupuesto (D-79).

**Interacciones:** M-08, M-16, M-19.

**Representación visual:** Estilo *Signalis* (D-58).

### Objetos confiscados

**Tipo:** Coleccionable (narrativa escénica, D-73).

**Descripción:** Pertenencias olvidadas por el personal de Vitalis y de Kessler, objetos confiscados a los sujetos de los experimentos (incluidos objetos de niños, que muestran la frialdad de la corporación) y restos del incidente.

**Función:** Narrativa escénica; premian la exploración.

**Cómo se obtiene:** **12**, 4 por piso: piso 1, personal; piso 2, sujetos; piso 3, incidente. El generador coloca 1–2 por intento en salas procedurales, dando prioridad a los no registrados.

**Cómo se utiliza:** `E` para examinar: muestra una etiqueta de inventario de Vitalis (código, fecha y propietario) y 1–2 líneas frías. Se registran en la sección "Objetos confiscados" del terminal.

**Cantidad máxima:** —

**Valor:** —

**Rareza:** No garantizados por intento.

**Restricciones:** No se recogen ni ocupan casilla. No cuentan para ningún final. Solo "Nueva partida" los borra (D-32).

**Interacciones:** M-08, M-16.

**Representación visual:** Objetos cotidianos (juguetes, carteras, fotos, gafas) con una etiqueta de inventario. Lista y textos en borrador en [[05 - Textos del Juego]].


> **Navegación:** [[00 - Índice]] · ← [[03 - Enemigos y NPC]] · [[05 - Misiones y Desafíos]] →
