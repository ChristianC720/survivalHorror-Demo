# Objetos y Coleccionables

## 1. Descripción de la sección

Define objetos que el jugador puede recoger, utilizar, equipar, vender, intercambiar o descubrir.

## 2. Plantilla

En la demo no se vende ni se intercambia nada. El inventario tiene 6 casillas y 5 tipos de objeto. El arma y la linterna no ocupan casilla. Los presupuestos por piso están en [[06 - Economía y Recursos]] y las reglas de uso en [[02 - Mecánicas]]. Todos los valores son iniciales y sujetos a balance. Los objetos de escenario de cada piso están en la sección 3.

| Objeto | Tipo | Casilla | Persiste al morir |
| --- | --- | --- | --- |
| Revólver de Kessler | Arma | No ocupa | Sí, The Mother lo devuelve descargado |
| Linterna | Equipo | No ocupa | Sí |
| Cura | Consumible | 1 por casilla | No |
| Munición | Consumible | 12 balas por casilla | No |
| Punzón de descarga | Consumible | 1 por casilla | No |
| Llave obligatoria u opcional | Objeto clave | 1 por casilla | No |
| Anillo | Registro | No ocupa | Sí, como registro del terminal |
| Documentos | Registro | No ocupa | Sí |
| Objetos confiscados | Coleccionable | No ocupa | Sí |

![Cura, blíster, punzón y las dos llaves sobre una mesa de trabajo|520](../Media/Capturas/ObjetosInventario.png)

### 2.1 Revólver de Kessler

**Tipo:** Arma, la única de la demo.

**Descripción:** Revólver de servicio del Werkschutz de Kessler Sicherheit AG, de gama alta y diseño de 2080. Tiene armazón cerámico y de polímero mate, cañón alineado con la recámara inferior y contador de balas en el armazón.

**Función:** Eliminar o interrumpir enemigos y destruir los transformadores de The Mother.

**Cómo se obtiene:** En el primer intento está en el cadáver asimilado de un guardia de Kessler, en la sala 2 del tutorial. Después, The Mother lo deja en la bandeja de efectos personales en cada reconstrucción, aunque Lukas haya muerto antes de recogerlo.

**Cómo se utiliza:** Se mantiene `Clic derecho` para apuntar y `Clic izquierdo` para disparar. `R` recarga bala a bala.

**Cantidad máxima:** 1.

**Valor:** No aplica, porque no hay comercio.

**Rareza:** Único.

**Restricciones:** Tambor de 6 balas y ~0,8 s entre disparos. Solo dispara con una identidad autorizada, y el terminal del brazo identifica a Lukas como activo de Vitalis. Siempre vuelve descargado.

**Interacciones:** Los disparos alertan a ~20 m y llenan un poco la atención de The Mother. Acertar en los puntos débiles interrumpe ataques.

**Representación visual:** Logotipo `KESSLER WERKSCHUTZ` y número de inventario de Vitalis. El contador del armazón sustituye al indicador de munición del HUD.

### 2.2 Linterna

**Tipo:** Equipo.

**Descripción:** Linterna integrada en el equipo de Lukas.

**Función:** Ver en la oscuridad a cambio de delatarse.

**Cómo se obtiene:** Está siempre disponible.

**Cómo se utiliza:** `F` la enciende y la apaga.

**Cantidad máxima:** 1.

**Valor:** No aplica.

**Rareza:** No aplica.

**Restricciones:** Su barra de carga dura ~120 s de uso continuo. Tras ~3 s apagada se recarga sola y va de 0 a 100% en ~30 s. En 0 parpadea, se apaga y no enciende durante ~10 s.

**Interacciones:** La luz duplica la velocidad de alerta de los enemigos que ilumina y activa al Mudo. La Boca es ciega.

**Representación visual:** Un indicador de carga en el terminal del brazo.

### 2.3 Cura

**Tipo:** Consumible.

**Descripción:** Kit médico de Vitalis.

**Función:** Recuperar un estado de salud. En Bien con daño devuelve la salud completa.

**Cómo se obtiene:** Con el presupuesto del piso.

**Cómo se utiliza:** Desde el inventario, en ~1,5 s. No se usa con la salud completa.

**Cantidad máxima:** 1 por casilla.

**Valor:** No aplica.

**Rareza:** Escasa.

**Restricciones:** Un golpe durante la animación la cancela sin gastarla.

**Interacciones:** Salud y estados, e Inventario.

**Representación visual:** Estuche rectangular de plástico rígido blanco sucio con asa, ribete cian y logotipo de Vitalis, con un autoinyector dentro. Lleva la etiqueta en alemán *Regenerationsmittel*. Se reconoce por la silueta del asa.

### 2.4 Munición

**Tipo:** Consumible.

**Descripción:** Blíster de 4 cartuchos sin vaina, de cebado eléctrico, con código de inventario de Vitalis.

**Función:** Cargar el revólver.

**Cómo se obtiene:** Con el presupuesto del piso. La sala de The Mother da 20 balas fijas, 12 para la fase 2 y 8 para la fase 3.

**Cómo se utiliza:** `R` carga bala a bala desde el inventario, ~0,7 s por bala. Se puede cancelar y se conservan las balas ya cargadas.

**Cantidad máxima:** 12 balas por casilla, que son 3 blísteres. En el Modo Historia cada blíster da 6 balas en lugar de 4.

**Valor:** No aplica.

**Rareza:** Escasa.

**Restricciones:** Se pierde al morir.

**Interacciones:** Combate a distancia e Inventario.

**Representación visual:** Blíster transparente con 4 cartuchos cobrizos sin vaina y tapa de aluminio con código de inventario de Vitalis.

### 2.5 Punzón de descarga

**Tipo:** Consumible de un solo uso con 2 funciones.

**Descripción:** Equipo de contención de sujetos de Kessler. Es un punzón que suelta una descarga al clavarse.

**Función:** Defensa contra agarres o remate silencioso.

**Cómo se obtiene:** Con el presupuesto del piso.

**Cómo se utiliza:** Se activa solo al ser agarrado y aturde al enemigo 2 s. Con `E` sobre un cuerpo quema su último rastro de identidad en ~1,5 s, y el laboratorio ya no puede reciclarlo.

**Cantidad máxima:** 1 por casilla.

**Valor:** No aplica.

**Rareza:** Escasa.

**Restricciones:** Gastarlo en un remate deja a Lukas sin defensa ante el próximo agarre. `E` no se activa sobre cuerpos que no pueden reciclarse.

**Interacciones:** Inventario, Punzón de descarga y Reciclaje de enemigos y remate.

**Representación visual:** Cilindro negro mate del largo de un antebrazo, con empuñadura de goma, punta metálica y franja ámbar de aviso. Lleva el logotipo `KESSLER WERKSCHUTZ` y un LED verde fósforo que indica carga.

### 2.6 Llave obligatoria y llave opcional

**Tipo:** Objeto clave.

**Descripción:** Llave de acceso de Vitalis.

**Función:** La obligatoria abre la sala clave final del piso, que en el piso 3 es la sala de The Mother. Las opcionales abren atajos o salas con recursos.

**Cómo se obtiene:** Hay 1 obligatoria por piso, alcanzable antes de su puerta. Las opcionales dependen del presupuesto.

**Cómo se utiliza:** `E` en su puerta, y se gasta.

**Cantidad máxima:** 1 por casilla.

**Valor:** No aplica.

**Rareza:** Fija la obligatoria y escasa la opcional.

**Restricciones:** Con el inventario lleno puede reemplazar otro objeto. El mínimo garantizado de cada recurso nunca queda detrás de una llave opcional.

**Interacciones:** Inventario, Interacción y documentos, y Exploración vertical por pisos.

**Representación visual:** Tarjeta magnética de Vitalis con número de sala. La obligatoria es una placa más grande con banda cian y un icono de escalera. La opcional es una tarjeta estándar con banda gris. Se distinguen por forma e icono, además del color.

### 2.7 Anillo

**Tipo:** Registro.

**Descripción:** Anillo que Lukas sabe importante. Nunca se sabe si es el suyo o el de Emilia.

**Función:** Narrativa.

**Cómo se obtiene:** Del suelo de la fila de cápsulas, solo tras la cinemática completa de reconstrucción.

**Cómo se utiliza:** Se examina en el terminal del brazo.

**Cantidad máxima:** 1.

**Valor:** No aplica.

**Rareza:** Único.

**Restricciones:** No ocupa casilla. Se recoge una sola vez y queda como registro del terminal, sin presencia física después.

**Interacciones:** Reaparición por reconstrucción y terminal del brazo.

**Representación visual:** Alianza de acero sencilla y gastada. La inscripción interior está raspada e ilegible para que nunca se sepa de quién es.

### 2.8 Documentos

**Tipo:** Registro.

**Descripción:** Son 6 documentos de Emilia y 12 más. En el piso 1 hay memorandos de Vitalis y partes de guardia de Kessler, en el piso 2 informes clínicos de The Mother y en el piso 3 registros del incidente.

**Función:** Contar la historia. Los 6 de Emilia desbloquean la decisión final.

**Cómo se obtiene:** Los de Emilia están 2 por piso en salas clave secundarias. Los demás son 6 por piso y aparecen 1 o 2 por intento en salas procedurales, con prioridad para los no registrados.

**Cómo se utiliza:** `E` para leer. Se registran en el terminal.

**Cantidad máxima:** 18 en total.

**Valor:** No aplica.

**Rareza:** Los de Emilia están garantizados. Los demás aparecen 1 o 2 por intento.

**Restricciones:** Si el documento de Emilia de una sala ya está registrado, la sala da en ese intento 1 recurso extra del tipo más escaso del piso, por encima del presupuesto.

**Interacciones:** Interacción y documentos, Reaparición por reconstrucción y Jefe final.

**Representación visual:** Estilo *Signalis*. Los textos están en [[05 - Textos del Juego]].

### 2.9 Objetos confiscados

**Tipo:** Coleccionable de narrativa escénica.

**Descripción:** Pertenencias olvidadas del personal de Vitalis y de Kessler, objetos confiscados a los sujetos de los experimentos y restos del incidente. Incluyen objetos de niños, que muestran la frialdad de la corporación.

**Función:** Narrativa escénica que premia la exploración.

**Cómo se obtiene:** Son 12, 4 por piso. El piso 1 tiene los del personal, el piso 2 los de los sujetos y el piso 3 los del incidente. El generador pone 1 o 2 por intento en salas procedurales, con prioridad para los no registrados.

**Cómo se utiliza:** `E` para examinar. Muestra una etiqueta de inventario de Vitalis con código, fecha y propietario, más 1 o 2 líneas frías. Se registran en la sección `Objetos confiscados` del terminal.

**Cantidad máxima:** 12.

**Valor:** No aplica.

**Rareza:** No están garantizados en cada intento.

**Restricciones:** No se recogen ni ocupan casilla. No cuentan para ningún final. Solo `Nueva partida` los borra.

**Interacciones:** Interacción y documentos, y Reaparición por reconstrucción.

**Representación visual:** Objetos cotidianos como juguetes, carteras, fotos y gafas, con una etiqueta de inventario. La lista y los textos están en [[05 - Textos del Juego]].

## 3. Objetos de escenario por piso

Estos objetos visten las salas y las escenas. Todos se modelan en Blender. La lista sale de las 46 salas de `Prefabs/Rooms/`, de `ROOM_Mother` y de las escenas `Intro`, `Floor`, `Cinematics` y `MainMenu`. Cada objeto figura con su nombre real de Unity. Una cobertura bloquea la línea de visión de los enemigos y permite agacharse detrás. Las medidas y pivotes están en la guía `Desarrollo/OBJECT_CATALOG.md`.

### 3.1 Objetos recogibles y registros

| Objeto | Prefab o modelo | Dónde aparece |
| --- | --- | --- |
| Revólver de Kessler | `ITEM_Revolver` | En el cadáver del guardia de `ROOM_F1_Storage_A` en el primer intento y en `KEY_F1_CapsuleRow` en las reconstrucciones siguientes |
| Cura | `ITEM_Heal` | Puntos de objeto `SP_Item` de cualquier sala, según el presupuesto |
| Blíster de munición | `ITEM_AmmoBlister` | Puntos de objeto, según el presupuesto |
| Punzón de descarga | `ITEM_Spike` | Puntos de objeto, según el presupuesto |
| Llave obligatoria | `ITEM_KeyMandatory` | Una por piso, antes de la puerta de la sala final |
| Llave opcional | `ITEM_KeyOptional` | Puntos de objeto, según el presupuesto |
| Documento | `ITEM_Document` | Salas clave secundarias para los de Emilia y salas procedurales para los demás |
| Objeto confiscado | `ITEM_Document` con el modelo `CONF_WATCH`, `CONF_THERMOS`, `CONF_GLASSES`, `CONF_LUNCHBOX_NOTE`, `CONF_PLUSH_RABBIT`, `CONF_WALLET`, `CONF_ROSARY`, `CONF_CHILD_DRAWING`, `CONF_EARPLUGS`, `CONF_BADGE_KS`, `CONF_CASSETTE` o `CONF_HEADPHONES` | Salas procedurales, 1 o 2 por intento |
| Anillo | `DOC_RING` | Registro del terminal tras la cinemática completa |
| Interruptor de energía | `INT_PowerSwitch` | Una sala clave secundaria de los pisos 1 y 2 cuando la sala final empieza sin energía |
| Panel de hackeo | `PROP_Shared_HackPanel` | Salas de hackeo de los 3 pisos, 2 por piso |
| Terminal del brazo | `PROP_ArmTerminal` | En el antebrazo izquierdo de `Player` |

### 3.2 Comunes a los 3 pisos

| Objeto | Prefab | Cobertura | Salas que lo usan |
| --- | --- | --- | --- |
| Bocina de megafonía | `PROP_Shared_Speaker` | No | 9 del piso 1, 11 del piso 2, 10 del piso 3 y la sala de The Mother. También emite las líneas de The Mother |
| Haz de cables | `PROP_Shared_CableBundle` | No | 3 del piso 1, 5 del piso 2 y 5 del piso 3 |
| Tubo recto | `PROP_Shared_Pipe_2m` | No | 1 por piso |
| Codo de tubo | `PROP_Shared_Pipe_Elbow` | No | 1 por piso |
| Montón de basura | `PROP_Shared_TrashPile` | No, es bajo | 11 del piso 1, 13 del piso 2 y 10 del piso 3 |
| Rejilla de ventilación o desagüe | `PROP_Shared_Grate` | No | 3 del piso 1, 5 del piso 2 y 8 del piso 3 |
| Charco | `PROP_Shared_Puddle` | No | 11 del piso 1, 11 del piso 2 y 13 del piso 3. Superficie mojada para los pasos |
| Cartel de seguridad de Vitalis | `PROP_Shared_VitalisPoster` | No | 5 del piso 1, 1 del piso 2 y 1 del piso 3. En el tutorial muestra la tecla asignada |
| Rampa | `KIT_Shared_Ramp_2x4` | No | 2 salas del piso 1 y 1 sala vertical del piso 3 |

### 3.3 Piso 1, mantenimiento

| Objeto | Prefab o modelo | Cobertura | Salas que lo usan |
| --- | --- | --- | --- |
| Generador de energía | `PH_F1_Generator` | Sí | 3 |
| Caldera | `PH_F1_Boiler` | Sí | 1, la sala de calderas del tutorial |
| Bomba de agua | `PH_F1_WaterPump` | Sí | 1 |
| Tanque de agua | `PROP_F1_WaterTank` | Sí | 1 |
| Válvula de presión | `PROP_F1_PressureValve` | No | 1 |
| Tubería gruesa | `PROP_F1_ThickPipe` | Baja | 6 |
| Mesa de trabajo | `PROP_F1_Workbench` | Agachado | 8, también como mesa de efectos personales de la fila de cápsulas |
| Herramientas | `PROP_F1_Tools` | No | 2 |
| Medidor de agua, presión o energía | `PROP_F1_Gauge` | No | 4 |
| Contenedor metálico gigante | `PROP_F1_BigContainer` | Sí | 1 |
| Estante de herramientas | `PROP_F1_ToolShelf` | Sí | 5 |
| Monitor CRT de acero | `PROP_F1_CRT` | No | 6 |
| Caja metálica y media caja | `PROP_Shared_MetalCrate`, `PROP_Shared_MetalCrate_Half` | Sí | 7 y 5 |
| Cápsula de reconstrucción | `PH_Capsule_Reconstruction` | Sí | La fila de cápsulas |
| Cadáver de un guardia de Kessler | `PH_KesslerGuard_Corpse` | No | `ROOM_F1_Storage_A` |
| Entreplanta | `CH_Env_Mezzanine` | No | 1 sala vertical |
| Escalera de la sala final | `CH_Stairs_F1` | No | `KEY_F1_Final` |
| Salida de la sala final | `CH_Exit_F1` | No | `KEY_F1_Final` |
| Lámpara | `CH_Lamp_F1` con `Light_F1_Sodium` | No | 5 salas con lámpara visible y 10 con luz de sodio |

### 3.4 Piso 2, clínica ilegal

| Objeto | Prefab o modelo | Cobertura | Salas que lo usan |
| --- | --- | --- | --- |
| Silla de laboratorio | `PROP_F2_LabChair` | No | 3 |
| Cápsula para persona | `PH_F2_HumanCapsule` | Sí | 1 y la sala de The Mother |
| Escritorio | `PROP_F2_Desk` | Agachado | 5 |
| Consola | `PROP_F2_Console` | Agachado | 4 y la sala de The Mother |
| Cama de investigación | `PROP_F2_ResearchBed` | No | 2 |
| Perchero con batas | `PROP_F2_LabCoatRack` | No | 1 |
| Mesa de cirugía | `PROP_F2_SurgeryTable` | No | 2 |
| Herramientas de cirugía | `PROP_F2_SurgicalTools` | No | 2 |
| Lámpara quirúrgica | `PROP_F2_SurgicalLamp` | No | 1 |
| Biombo | `PROP_F2_Screen` | Solo de la vista | 6 |
| Carrito de suero | `PROP_F2_IVCart` | No | 4 |
| Archivador | `PROP_F2_FilingCabinet` | Sí | 4 |
| Nevera de muestras | `PROP_F2_SampleFridge` | Sí | 3 |
| Monitor CRT de plástico amarillento | `PROP_F2_CRT` | No | 6 |
| Mesa con la foto imposible | `CH_Env_PhotoTable` y `CH_Env_PhotoFrame` | No | `KEY_F2_Final` |
| Elevador averiado de llegada | `CH_Elevator_F2` | No | `KEY_F2_Arrival` |
| Escalera de la sala final | `CH_Stairs_F2` | No | `KEY_F2_Final` |
| Salida de la sala final | `CH_Exit_F2` | No | `KEY_F2_Final` |
| Lámpara | `CH_Lamp_F2` con `Light_F2_Fluorescent` | No | 10 salas con lámpara visible y 5 con fluorescente |

### 3.5 Piso 3, núcleo

| Objeto | Prefab o modelo | Cobertura | Salas que lo usan |
| --- | --- | --- | --- |
| Tanque criogénico | `PH_F3_CryoTank` | Sí | 3 y la sala de The Mother |
| Transformador | `PH_F3_Transformer` | Sí | 3 y la sala de The Mother |
| Columna de refrigeración | `PROP_F3_CoolingColumn` | Sí | 2 |
| Consola de contención | `PROP_F3_ContainmentConsole` | Agachado | 9 y la sala de The Mother |
| Cápsula rota | `PH_F3_BrokenCapsule` | Sí | 3 |
| Bocina de megafonía | `PROP_F3_PAHorn` | No | 7 y la sala de The Mother |
| Cableado grueso | `PROP_F3_HeavyCabling` | No | 6 |
| Banco de 6 monitores CRT, uno rojo | `PROP_F3_CRTBank` | No | 3 |
| Pasarela | `CH_Env_Catwalk` | No | 1 sala vertical |
| Elevador averiado de llegada | `CH_Elevator_F3` | No | `KEY_F3_Arrival` |
| Escalera de la sala final | `CH_Stairs_F3` | No | `KEY_F3_Final` |
| Salida de la sala final | `CH_Exit_F3` | No | `KEY_F3_Final` |
| Lámpara | `CH_Lamp_F3` con `Light_F3_Crimson` | No | 10 salas con lámpara visible y 6 con luz carmesí |

### 3.6 Sala de The Mother

`ROOM_Mother` reúne la central de megafonía. The Mother es el modelo `CH_TheMother`, con su brazo `CH_TheMother_Arm`, su pinza `CH_TheMother_Claw` y sus cámaras `CH_TheMother_Camera`. Las 3 mesas de mezcla de los nodos son `PROP_F3_MixingTable` con el modelo `CH_F3_MixingTable`. Los 3 transformadores del jefe reutilizan `PH_F3_Transformer` con un punto débil. La salida de la decisión final es `CH_Env_MotherExit`. La sala suma `PH_F2_HumanCapsule`, `PH_F3_CryoTank`, `PROP_F2_Console`, `PROP_F3_ContainmentConsole`, `PROP_F3_PAHorn`, `PROP_Shared_Speaker` y `Light_F3_Crimson`.

### 3.7 Piezas del kit por piso

Cada piso arma sus salas con su `KitSet`. El piso 1 usa `KIT_F1_Floor_2x2`, `KIT_F1_Wall_2m`, `KIT_F1_Wall_2m_High`, `KIT_F1_WallWindow_2m`, `KIT_F1_Doorway_2m`, `KIT_F1_Ceiling_2x2` y la puerta `Door_F1`. Los pisos 2 y 3 usan las mismas piezas con el prefijo `KIT_F2_` y `KIT_F3_`, y las puertas `Door_F2` y `Door_F3`.

### 3.8 Escenas fuera de los pisos

| Escena | Objetos |
| --- | --- |
| `Intro` | Mesa `CH_Intro_Table`, silla `CH_Intro_Chair`, foto `CH_Intro_Photo`, compuerta `CH_Intro_Hatch`, panel `CH_Intro_Panel`, puerta del elevador `CH_Intro_LiftDoor`, el Mudo señuelo `CH_Mudo_Still`, Lukas `CH_Lukas_RH07`, una cápsula `PH_Capsule_Reconstruction` y luces de los 3 pisos |
| `Cinematics` | The Mother `CH_TheMother`, Lukas `CH_Lukas_RH07`, Lukas convertido en Mudo `CH_Lukas_Mudo`, la salida `CH_Env_MotherExit`, una cápsula, un transformador, `PROP_F3_CRTBank`, `PROP_F3_ContainmentConsole`, `PROP_F3_HeavyCabling` y `KIT_F1_Floor_2x2` |
| `Floor` | Los 4 enemigos `Enemy_Alto`, `Enemy_Boca`, `Enemy_Rastrero` y `Enemy_Mudo`, los objetos `ITEM_*`, `INT_PowerSwitch` y `ROOM_Mother` como plantillas del generador |

> **Navegación:** [[00 - Índice]] · ← [[03 - Enemigos y NPC]] · [[05 - Misiones y Desafíos]] →
