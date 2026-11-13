# Textos del Juego

## 1. Descripción de la sección

Borradores de todos los textos del juego: nota de voz de Emilia, expedientes, documentos, bestiario, líneas de The Mother, objetos confiscados y murmullos de Lukas. El contexto, el tono y las reglas de cada voz están en [[04 - Diálogos y Narrativa]]. Esta nota contiene solo el texto.

Todos los textos son borradores pendientes de revisión. Cada texto va en español y en inglés. Las fechas, cantidades y probabilidades son valores iniciales sujetos a balance.

## 2. Convenciones

- **Identificador.** Cada texto tiene un `id` que coincide con el nombre de su ScriptableObject y con la clave de la tabla de Localization. Ver [[04 - Datos y Persistencia]].
- **Formato.** Cada línea de un texto del juego va entre acentos graves. Los fragmentos en alemán van en *cursiva*, fuera de los acentos.
- **Corrupción.** Todo pasaje que nombra a Lukas, se dirige a él o habla de su vida con Emilia se registra corrupto, en cualquier documento. Se marca con `{c}…{/c}`.
- También salen corruptos el nombre de Emilia y dos datos del incidente: la firma del operador en la hoja de turno y el lugar del registro R-H-06.
- El terminal muestra ese tramo con símbolos y letras corruptas, con el glitch morado. Lo muestra limpio en el desenlace si se reunieron los 6 expedientes. La corrupción nunca toca rutas ni reglas.
- **Expedientes.** Son los 6 documentos de Emilia. Ningún título ni etiqueta nombra a su autora. Lukas y el jugador no saben de quién son hasta el desenlace.
- **Contradicciones.** Cada documento afirma una sola versión de los hechos. La contradicción aparece al compararlo con otro documento, con un expediente o con un registro de The Mother.
- **Alemán.** No se traduce en el mundo ni contiene información necesaria. En los registros del terminal del brazo, cada línea lleva un subtítulo con su traducción.
- **Variables.** `{n}` es el número del código de sujeto actual de Lukas, como R-H-07 y R-H-08.
- **Fechas.** Usan el formato `YYYY-MM-DD`, como en el terminal.
- Los expedientes, los documentos, la nota de voz, el bestiario y las líneas de The Mother y de Lukas no usan raya ni punto y coma. Las etiquetas de interfaz de la sección 11 se copian tal cual.

## 3. Línea temporal de referencia

Emilia vuelve a la instalación una sola vez, el 2082-10-19, y no sale. Sus reconstrucciones R-H-02 a R-H-06 ocurren dentro.

| Fecha | Hecho | Registro de The Mother |
| --- | --- | --- |
| 2080-09-01 | Ingresa el primer sujeto menor registrado en el inventario de confiscados | Sin registro |
| 2080-11-02 | Emilia nota el latido en la sala 4 del subnivel C | Sin registro |
| 2081-03-13 | Emilia pide acceso a la central de megafonía y se lo niegan | Sin registro |
| 2081-03-14 | El incidente, con la transmisión del latido por megafonía a las 03:12 | R-H-01, 03:13 |
| 2081-03-20 | Emilia escribe sobre la noche del incidente | Sin registro |
| 2081-04-02 | Vitalis despide a Emilia y sella la instalación | Sin registro |
| 2082-10-19 | Emilia entra a la instalación sellada por única vez | R-H-02, 12:48 |
| 2083-01-27 | Emilia sigue dentro | R-H-03, 01:22 |
| 2083-05-09 | Emilia sigue dentro | R-H-04, 02:55 |
| 2083-08-30 | Emilia sigue dentro | R-H-05, 06:16 |
| 2083-11-17 | Emilia escribe su último expediente | Sin registro |
| 2083-11-18 | Emilia llega a la consola de megafonía | R-H-06, 10:53 |
| 2083-12-02 | El teléfono de Lukas recibe la nota de voz programada | Sin registro |
| 2083-12-03 | Primera reconstrucción de Lukas | R-H-07 |

PENDIENTE: ¿cuándo grabó Emilia la nota de voz y por qué llega 14 meses después de su entrada?

## 4. Nota de voz de Emilia

`id: voice_note_emilia` · Título en el terminal: `Nota de voz` / `Voice note`. Es una transcripción con sonido vocal y forma de onda. En el terminal del brazo se guarda corrupta hasta el desenlace.

**ES**

- `{c}Lukas. Si estás oyendo esto, significa que no volví.{/c}`
- `Hay algo debajo del laboratorio donde trabajaba. Trabajé mucho tiempo en Vitalis y nunca pregunté qué era.`
- `Y cuando lo hice... Eso fue todo lo que hice. Pregunté y todo salió mal.`
- `He vuelto a encontrar lo que llevo preguntándome por años. No sé exactamente qué es, pero alguien debe saber. Alguien más debe hacerlo.`
- `No vengas a buscarme. {c}Por favor, no lo hagas. Tú no. Quédate en casa. Y si no vuelvo, no me olvides.{/c}`
- `(respiración) …está en las paredes. Lo oigo desde aquí.`
- `(pasos, corre)`

**EN**

- `{c}Lukas. If you're hearing this, it means I didn't come back.{/c}`
- `There is something under the lab where I used to work. I worked at Vitalis for a long time and never asked what it was.`
- `And when I did... That was all I did. I asked and everything went wrong.`
- `I've gone back to find the thing I've been asking myself about for years. I don't know exactly what it is, but someone must know. Someone else has to.`
- `Don't come looking for me. {c}Please don't. Not you. Stay home. And if I don't come back, don't forget me.{/c}`
- `(breathing) …it's in the walls. I can hear it from here.`
- `(footsteps, running)`

## 5. Expedientes

Son los 6 documentos de Emilia. Ninguno lleva su nombre. Empiezan como notas de trabajo y se quiebran a medida que ella olvida. Nunca usan el código R-H ni hablan de actos. Sus fechas coinciden con los registros de The Mother, pero sus lugares y sus hechos no. Los expedientes 1 y 2 van en el piso 1, el 3 y el 4 en el piso 2, y el 5 y el 6 en el piso 3. Hay como máximo 1 por sala secundaria.

### 5.1 Piso 1

`id: doc_emilia_01` · `Expediente 01 - 2080-11-02` / `File 01 - 2080-11-02`

**ES**

- `Subnivel C, sala 4. Lecturas nocturnas.`
- `Once sujetos y once monitores. A las 02:40 los once pulsos cayeron a la vez.`
- `Revisé los cables, la corriente y cambié dos monitores. El pulso se reanudó, pero no viene de ellos. Puse la mano en la pared y también estaba ahí. Probablemente venga del otro cuarto.`
- `Mantenimiento lo llama la planta. Pregunté acerca de la planta, pero nadie supo decirme concretamente lo que es.`
- `{c}Lukas dice que ahora hablo dormida. Dice que murmuro cosas sin sentido.{/c}`

**EN**

- `Sublevel C, ward 4. Night readings.`
- `Eleven subjects and eleven monitors. At 02:40 all eleven pulses dropped at once.`
- `I checked the leads and the mains and changed two monitors. The pulse came back, but it isn't coming from them. I put my hand on the wall and it was there too. It's probably coming from the next room.`
- `Maintenance calls it the plant. I asked about the plant, but nobody could tell me exactly what it is.`
- `{c}Lukas says I talk in my sleep now. He says I mumble things that make no sense.{/c}`

`id: doc_emilia_02` · `Expediente 02 - 2081-03-13` / `File 02 - 2081-03-13`

**ES**

- `Llevo cuatro meses de lecturas. El pulso de la pared no cambia de ritmo, pero cada noche se oye en más salas.`
- `Lo seguí por los pasillos del subnivel C. Cerca de la central de megafonía suena más fuerte que en cualquier otro lugar.`
- `Pedí acceso a la central para medirlo desde dentro y me lo negaron. Kessler puso un guardia en la puerta esa misma tarde.`
- `Le pregunté a mi supervisor qué hay debajo de esa sala. Me dijo que escuchara el latido y volviera al trabajo.`
- `Mandé mis lecturas arriba con alguien de mantenimiento. No sé a quién le llegaron.`
- `{c}Mañana es nuestro aniversario. Le prometí a Lukas que llegaría temprano.{/c}`

**EN**

- `Four months of readings now. The pulse in the wall keeps the same rhythm, but every night it can be heard in more rooms.`
- `I followed it down the corridors of Sublevel C. It's louder near the PA central than anywhere else.`
- `I asked for access to the central to measure it from inside and they refused. Kessler posted a guard at the door that same afternoon.`
- `I asked my supervisor what is under that room. He told me to listen to the heartbeat and go back to work.`
- `I sent my readings upstairs with someone from maintenance. I don't know who received them.`
- `{c}Tomorrow is our anniversary. I promised Lukas I'd be home early.{/c}`

### 5.2 Piso 2

`id: doc_emilia_03` · `Expediente 03 - 2081-03-20` / `File 03 - 2081-03-20`

**ES**

- `Esa noche estaba en la sala 4, con los monitores. No salí de ahí.`
- `Empezó por los altavoces. Lo oí unos segundos y después no recuerdo nada.`
- `Desperté dentro de una cápsula. Una voz dijo que mi preservación fue exitosa y me dio un número.`
- `Había más cápsulas en la fila. Algunas estaban ocupadas, pero nadie se movía dentro.`
- `Vitalis me entrevistó dos veces. Preguntaron dónde estaba a las 03:12. Nadie preguntó qué oí.`
- `{c}No le conté a Lukas lo de la cápsula. Le dije que hubo un accidente y que estoy bien.{/c}`

**EN**

- `That night I was in ward 4, with the monitors. I didn't leave.`
- `It started through the speakers. I heard it for a few seconds and after that I remember nothing.`
- `I woke up inside a capsule. A voice said my preservation was successful and gave me a number.`
- `There were more capsules in the row. Some were occupied, but nobody was moving inside.`
- `Vitalis interviewed me twice. They asked where I was at 03:12. Nobody asked what I heard.`
- `{c}I didn't tell Lukas about the capsule. I told him there was an accident and that I'm fine.{/c}`

`id: doc_emilia_04` · `Expediente 04 - 2082-10-19` / `File 04 - 2082-10-19`

**ES**

- `Volví. Mi tarjeta vieja todavía abre la escotilla de mantenimiento.`
- `El elevador se detuvo entre dos pisos y la puerta no abre. La escotilla por la que entré ya no está donde la dejé.`
- `Llevo muchos días en este cuarto. El agua se me acabó hace tiempo. Mi reloj dice que sigue siendo el mismo día.`
- `El latido se oye más fuerte sin gente. Ya no necesito poner la mano en la pared.`
- `Dejé un mensaje grabado antes de bajar. Si no salgo, se envía solo.`
- `{c}Lukas no sabe que estoy aquí. Le dije que iba a una entrevista de trabajo.{/c}`

**EN**

- `I came back. My old badge still opens the maintenance hatch.`
- `The elevator stopped between two floors and the door won't open. The hatch I came in through is no longer where I left it.`
- `I've been in this room for many days. I ran out of water a long time ago. My watch says it's still the same day.`
- `The heartbeat is louder without people. I don't need to put my hand on the wall anymore.`
- `I left a recorded message before I came down. If I don't get out, it sends itself.`
- `{c}Lukas doesn't know I'm here. I told him I was going to a job interview.{/c}`

### 5.3 Piso 3

`id: doc_emilia_05` · `Expediente 05 - 2083-05-09` / `File 05 - 2083-05-09`

**ES**

- `No sé cuánto llevo aquí abajo. Las fechas de estas hojas tienen mi letra, así que deben ser ciertas.`
- `Las salas cambian de lugar mientras duermo. Marco las puertas con tiza y las marcas aparecen en otras puertas.`
- `No he salido del subnivel C. Estoy segura, porque nunca encontré las escaleras.`
- `Hay más gente aquí. Caminan por los pasillos como si siguieran de turno. No dejo que me vean.`
- `{c}Tengo una foto en la cartera. Un hombre me abraza y yo me río. Se llama Lukas. Lo escribo para no perderlo.{/c}`
- `Escucha el latido. Suena igual en todas las salas.`

**EN**

- `I don't know how long I've been down here. The dates on these pages are in my handwriting, so they must be true.`
- `The rooms change places while I sleep. I mark the doors with chalk and the marks turn up on other doors.`
- `I haven't left Sublevel C. I'm sure of it, because I never found the stairs.`
- `There are more people here. They walk the corridors as if they were still on shift. I don't let them see me.`
- `{c}I have a photo in my wallet. A man is holding me and I'm laughing. His name is Lukas. I'm writing it down so I don't lose it.{/c}`
- `Listen to the heartbeat. It sounds the same in every room.`

`id: doc_emilia_06` · `Expediente 06 - 2083-11-17` / `File 06 - 2083-11-17`

**ES**

- `Encontré la central de megafonía. La puerta estaba abierta y el guardia sigue de pie junto a ella.`
- `Nunca había entrado en esta sala. Aun así supe dónde estaba el interruptor de la luz.`
- `La consola está encendida. El canal 1 tiene una etiqueta escrita a mano, con una letra que se parece a la mía.`
- `El latido viene de debajo de esta sala. Mañana voy a escucharlo desde la consola.`
- `{c}Lukas va a venir de todos modos. Siempre viene. Ojalá esta vez no lo haga.{/c}`
- `Escucha el latido. Escucha.`

**EN**

- `I found the PA central. The door was open and the guard is still standing next to it.`
- `I had never been in this room. Even so, I knew where the light switch was.`
- `The console is on. Channel 1 has a handwritten label, in handwriting that looks like mine.`
- `The heartbeat comes from under this room. Tomorrow I'm going to listen to it from the console.`
- `{c}Lukas will come anyway. He always comes. I hope this time he doesn't.{/c}`
- `Listen to the heartbeat. Listen.`

## 6. Documentos de Vitalis Konzern, Kessler Sicherheit AG y The Mother

Son 18 documentos, 6 por piso, en salas procedurales. Aparecen 1 o 2 por intento. No cuentan para la decisión final. Usan una voz burocrática con humor negro corporativo. Tres de ellos señalan a un responsable distinto del incidente.

| Documento | Afirma | Lo contradice |
| --- | --- | --- |
| Memorando de Dirección | Una investigadora abrió el canal y actuó sola | Informe de Kessler, hoja de turno |
| Informe de Kessler, puesto 7 | Nadie entró. El acceso fue del sistema | Hoja de turno, auditoría de The Mother |
| Hoja de turno de megafonía | El canal se abrió desde la consola, con operador | Informe de Kessler, auditoría de The Mother |
| Auditoría de The Mother | El sistema no accedió. Entró una tarjeta de investigación | Informe de Kessler, expediente 03 |
| Registro de la serie R-H | R-H-01 se recogió en la central de megafonía | Expediente 03, notificación de despido |
| Aviso de cierre | Todo el personal fue evacuado | Memorando de Dirección, informe de estado |
| Aviso a mantenimiento | El sonido es resonancia de la planta | Orden de trabajo 4418 |
| Hoja de la sala 4 | Noche sin eventos, 12 sujetos | Expediente 01 |
| Criterios de ingreso | No se admiten menores y todos reciben el alta | Objetos confiscados del piso 2 |

### 6.1 Piso 1, mantenimiento

`id: doc_vk_maintenance_notice` · `Vitalis Konzern, aviso al personal de mantenimiento` / `Vitalis Konzern, notice to maintenance staff`

**ES**

- `PARA: Mantenimiento, subniveles A a C`
- `ASUNTO: Sonido de baja frecuencia`
- `El sonido rítmico reportado en las salas de calderas es resonancia de la planta y no supone ningún riesgo.`
- `Al personal con ansiedad se le indica escuchar el latido. Tiene un efecto calmante documentado.`
- `No grabe el sonido ni lo tararee. No comente el ritmo con sus compañeros.`
- `No se pagarán horas extra por pausas de escucha.`
- *Ordnung ist das halbe Leben.*

**EN**

- `TO: Maintenance, Sublevels A to C`
- `RE: Low-frequency sound`
- `The rhythmic sound reported in the boiler rooms is plant resonance and poses no risk.`
- `Staff experiencing anxiety are instructed to listen to the heartbeat. It has a documented calming effect.`
- `Do not record the sound or hum along. Do not discuss the rhythm with colleagues.`
- `Overtime will not be paid for listening breaks.`
- *Ordnung ist das halbe Leben.*

`id: doc_ks_patrol_log` · `Kessler Sicherheit AG, Werkschutz, libro de rondas, puesto 3` / `Kessler Sicherheit AG, Werkschutz, patrol log, post 3`

**ES**

- `02:00 Ronda completa. Sin novedad.`
- `02:20 La sala de calderas 2 está más caliente de lo normal. Se informa a mantenimiento. Mantenimiento ya lo sabía.`
- `02:40 Un técnico pregunta qué hay detrás de la puerta C-11. Se le indica que no es asunto suyo.`
- `03:00 Ronda completa. Sin novedad. El suelo se siente tibio.`
- `03:12 Suena música por la megafonía. Sin novedad.`
- `03:13`

**EN**

- `02:00 Round complete. Nothing to report.`
- `02:20 Boiler room 2 is warmer than normal. Maintenance informed. Maintenance already knew.`
- `02:40 A technician asks what is behind door C-11. He is told it is not his concern.`
- `03:00 Round complete. Nothing to report. The floor feels warm.`
- `03:12 Music on the PA. Nothing to report.`
- `03:13`

`id: doc_vk_regen_poster` · `Vitalis Konzern, cartel publicitario` / `Vitalis Konzern, advertising poster`

**ES**

- `VITALIS REGEN™`
- `Lo que pierdes, te lo devolvemos.`
- `Pregunta a nuestro personal por el plan de implementación.`
- *Gesundheit ist Pflicht.*
- `Los resultados varían según el sujeto.`

**EN**

- `VITALIS REGEN™`
- `What you lose, we give back.`
- `Ask our staff about the implementation plan.`
- *Gesundheit ist Pflicht.*
- `Results vary by subject.`

`id: doc_mother_status_report` · `The Mother, informe automático de estado` / `The Mother, automatic status report`

**ES**

- `ESTADO DE LA INSTALACIÓN: DADA DE BAJA`
- `Personal en el lugar: 0`
- `Personal preservado: {n}`
- `Personal sin identidad: recuento no disponible. No responden al pase de lista.`
- `Informes enviados a los servidores de Vitalis: 4112`
- `Informes leídos: 0`
- `Recomendación: continuar informando.`

**EN**

- `FACILITY STATUS: DECOMMISSIONED`
- `Personnel on site: 0`
- `Personnel preserved: {n}`
- `Personnel without identity: count not available. They do not answer roll call.`
- `Reports sent to Vitalis servers: 4,112`
- `Reports read: 0`
- `Recommendation: continue reporting.`

`id: doc_vk_work_order` · `Vitalis Konzern, mantenimiento, orden de trabajo 4418` / `Vitalis Konzern, maintenance, work order 4418`

Documento nuevo. Contradice el aviso a mantenimiento.

**ES**

- `Solicitud: localizar el equipo llamado la planta y revisar su vibración.`
- `Subnivel A: no se encontró ningún equipo con ese nombre.`
- `Subnivel B: no se encontró ningún equipo con ese nombre.`
- `Subnivel C: el técnico no completó la revisión.`
- `Inventario de equipos: no hay ninguna planta registrada en esta instalación.`
- `Orden cerrada. Motivo: equipo inexistente.`

**EN**

- `Request: locate the equipment known as the plant and check its vibration.`
- `Sublevel A: no equipment found under that name.`
- `Sublevel B: no equipment found under that name.`
- `Sublevel C: the technician did not complete the inspection.`
- `Equipment inventory: no plant is registered at this facility.`
- `Order closed. Reason: equipment does not exist.`

`id: doc_vk_closure_notice` · `Vitalis Konzern, Dirección, aviso de cierre` / `Vitalis Konzern, Board, closure notice`

Documento nuevo. Contradice el memorando de Dirección y el informe de estado.

**ES**

- `La instalación de Sektor V-7 cierra el 2081-04-02 por mantenimiento programado.`
- `Todo el personal fue evacuado y reubicado. No queda nadie en el interior.`
- `Los sujetos del programa clínico recibieron el alta antes del cierre.`
- `El sistema médico queda encendido para conservar el inventario.`
- `No se atenderán consultas sobre el personal reubicado.`
- *Fortschritt durch Erhaltung.*

**EN**

- `The Sektor V-7 facility closes on 2081-04-02 for scheduled maintenance.`
- `All personnel were evacuated and reassigned. Nobody remains inside.`
- `Subjects in the clinical program were discharged before the closure.`
- `The medical system stays powered to preserve the inventory.`
- `Enquiries about reassigned personnel will not be answered.`
- *Fortschritt durch Erhaltung.*

### 6.2 Piso 2, clínica ilegal

`id: doc_mother_rh_records` · `The Mother, registro de preservación, serie R-H` / `The Mother, preservation log, series R-H`

No muestra nombre ni estado. El jugador cruza las fechas y los lugares con los expedientes.

**ES**

- `SERIE R-H. REGISTRO DE PRESERVACIÓN`
- `R-H-01 - 2081-03-14 - 03:13. Central de megafonía. Causa: paro cardíaco.`
- `R-H-02 - 2082-10-19 - 12:48. Subnivel A, cuarto del elevador. Causa: inanición.`
- `R-H-03 - 2083-01-27 - 01:22. Subnivel B, cuarto de máquinas. Causa: traumatismo severo.`
- `R-H-04 - 2083-05-09 - 02:55. Subnivel B, sala 2. Causa: traumatismo contuso.`
- `R-H-05 - 2083-08-30 - 06:16. Subnivel C, ducto de la sala 4. Causa: asfixia.`
- `R-H-06 - 2083-11-18 - 10:53. {c}Central de megafonía, en consola{/c}. Causa: desconocida.`
- `Preservación: exitosa.`
- `Nota: el intervalo entre entradas disminuye.`

**EN**

- `SERIES R-H. PRESERVATION LOG`
- `R-H-01 - 2081-03-14 - 03:13. PA central. Cause: cardiac arrest.`
- `R-H-02 - 2082-10-19 - 12:48. Sublevel A, elevator room. Cause: starvation.`
- `R-H-03 - 2083-01-27 - 01:22. Sublevel B, machine room. Cause: severe trauma.`
- `R-H-04 - 2083-05-09 - 02:55. Sublevel B, ward 2. Cause: blunt trauma.`
- `R-H-05 - 2083-08-30 - 06:16. Sublevel C, ward 4 duct. Cause: asphyxia.`
- `R-H-06 - 2083-11-18 - 10:53. {c}PA central, at the console{/c}. Cause: unknown.`
- `Preservation: successful.`
- `Note: the interval between entries is decreasing.`

`id: doc_vk_termination` · `Vitalis Konzern, Recursos Humanos, notificación de despido` / `Vitalis Konzern, Human Resources, termination notice`

Contradice el registro R-H-01.

**ES**

- `{c}Dra. E. Reiner-Hellen{/c}, Investigación Clínica, subnivel C`
- `Tras el evento del 2081-03-14, usted abandonó la instalación por su propio pie y sin lesiones.`
- `Ha sido clasificada como superviviente inestable. Su relación laboral termina el 2081-04-02.`
- `Vitalis Konzern le recuerda que toda investigación, nota y recuerdo sigue siendo propiedad de la empresa.`
- `Su inscripción en Regen queda cancelada con efecto inmediato.`
- `Por favor, devuelva su tarjeta.`

**EN**

- `{c}Dr. E. Reiner-Hellen{/c}, Clinical Research, Sublevel C`
- `Following the event of 2081-03-14, you left the facility on your own feet and without injuries.`
- `You have been classified as an unstable survivor. Your employment ends on 2081-04-02.`
- `Vitalis Konzern reminds you that all research, notes and recollections remain company property.`
- `Your Regen enrollment is cancelled with immediate effect.`
- `Please return your badge.`

`id: doc_vk_incident_memo` · `Vitalis Konzern, Dirección, memorando interno` / `Vitalis Konzern, Board, internal memo`

Señala a Emilia. Contradice el informe de Kessler, la hoja de turno y el aviso de cierre.

**ES**

- `CONFIDENCIAL`
- `El evento del 2081-03-14 fue causado por una investigadora del subnivel C que entró sin autorización a la central de megafonía.`
- `La investigadora emitió material no aprobado por el canal 1. Actuó sola.`
- `La instalación será sellada. No se prevé recuperar al personal que sigue dentro. La recuperación no es rentable.`
- `El sistema médico seguirá encendido para preservar los activos. Sus informes no requieren respuesta.`
- *Vertraulich. Nicht kopieren.*

**EN**

- `CONFIDENTIAL`
- `The event of 2081-03-14 was caused by a researcher from Sublevel C who entered the PA central without authorization.`
- `The researcher broadcast unapproved material on channel 1. She acted alone.`
- `The facility will be sealed. No retrieval of the personnel still inside is planned. Retrieval is not cost-effective.`
- `The medical system will stay powered to preserve the assets. Its reports require no response.`
- *Vertraulich. Nicht kopieren.*

`id: doc_ks_incident_report` · `Kessler Sicherheit AG, informe de incidente, puesto 7` / `Kessler Sicherheit AG, incident report, post 7`

Señala al sistema. Contradice el memorando de Dirección y la hoja de turno.

**ES**

- `Puesto 7, puerta de la central de megafonía. Noche del 2081-03-14.`
- `La puerta se encuentra sellada a partir de las 22:00. Nadie entra ni sale. No se reciben entregas.`
- `03:12 Comenzó una emisión. El registro de la consola muestra un acceso del sistema sin tarjeta ni operador.`
- `El guardia del puesto 7 confirma que la sala estaba vacía.`
- `El guardia del puesto 7 dejó de responder a las 03:15.`
- `El guardia del puesto 7 sigue activo en su lugar.`
- `Estado del contrato: activo. Se adjunta factura.`

**EN**

- `Post 7, PA central door. Night of 2081-03-14.`
- `The door is sealed from 22:00. Nobody enters or leaves. No deliveries are received.`
- `03:12 A broadcast began. The console log shows a system login with no badge and no operator.`
- `The guard on Post 7 confirms the room was empty.`
- `The guard on Post 7 stopped responding at 03:15.`
- `The guard on Post 7 remains active at his post.`
- `Contract status: active. Invoice attached.`

`id: doc_vk_ward_sheet` · `Vitalis Konzern, programa clínico, hoja de la sala 4` / `Vitalis Konzern, clinical program, ward 4 sheet`

Documento nuevo. Contradice el expediente 01.

**ES**

- `Sala 4, turno de noche del 2080-11-02.`
- `Sujetos en sala: 12. Monitores en servicio: 12.`
- `02:40 Lecturas dentro de lo normal. Sin eventos.`
- `Cambios de equipo durante el turno: ninguno.`
- `Investigador de guardia: sin registro.`
- `Observaciones: los sujetos duermen. Uno de ellos tararea.`

**EN**

- `Ward 4, night shift of 2080-11-02.`
- `Subjects on ward: 12. Monitors in service: 12.`
- `02:40 Readings within normal range. No events.`
- `Equipment changes during the shift: none.`
- `Researcher on duty: no record.`
- `Remarks: the subjects are asleep. One of them is humming.`

`id: doc_vk_intake_criteria` · `Vitalis Konzern, programa clínico, criterios de ingreso` / `Vitalis Konzern, clinical program, intake criteria`

Documento nuevo. Contradice los objetos confiscados del piso 2.

**ES**

- `Solo ingresan adultos voluntarios con consentimiento firmado.`
- `No se admiten menores en ninguna fase del programa.`
- `Los efectos personales se guardan en inventario y se devuelven con el alta.`
- `Todos los sujetos reciben el alta.`
- `No es necesario informar a las familias del ingreso.`
- *Gesundheit ist Pflicht.*

**EN**

- `Only adult volunteers with signed consent are admitted.`
- `Minors are not admitted at any stage of the program.`
- `Personal effects are kept in inventory and returned on discharge.`
- `All subjects are discharged.`
- `Families do not need to be informed of the admission.`
- *Gesundheit ist Pflicht.*

### 6.3 Piso 3, núcleo

`id: doc_mother_preservation_protocol` · `The Mother, protocolo de preservación del personal (revisión 9)` / `The Mother, staff preservation protocol (revision 9)`

**ES**

- `1. El personal que deja de funcionar es preservado.`
- `2. La preservación requiere identidad. La identidad se confirma por la respuesta al código de sujeto.`
- `3. El personal sin identidad no puede ser preservado. Permanece en el lugar. No es mi responsabilidad.`
- `4. Cada preservación es exitosa.`
- `5. El número de personal sin identidad aumenta. Causa: fuera del alcance de este protocolo.`
- `6. Las revisiones 1 a 8 no fueron aprobadas. La revisión 9 no fue revisada.`

**EN**

- `1. Personnel who cease to function are preserved.`
- `2. Preservation requires identity. Identity is confirmed by response to the subject code.`
- `3. Personnel without identity cannot be preserved. They remain on site. They are not my responsibility.`
- `4. Each preservation is successful.`
- `5. The number of personnel without identity is increasing. Cause: outside the scope of this protocol.`
- `6. Revisions 1 to 8 were not approved. Revision 9 was not reviewed.`

`id: doc_mother_archive_index` · `The Mother, índice del archivo criogénico` / `The Mother, cryogenic archive index`

**ES**

- `ARCHIVO CRIOGÉNICO. ÍNDICE`
- `Muestras almacenadas: 1204`
- `Origen: personal (todo)`
- `Finalidad: material de referencia de Vitalis Regen™`
- `Especímenes pendientes de recolección: 1`
- `Nota: el archivo suena de noche. Registrado como ruido del compresor.`

**EN**

- `CRYOGENIC ARCHIVE. INDEX`
- `Samples stored: 1,204`
- `Origin: personnel (all)`
- `Purpose: Vitalis Regen™ reference material`
- `Specimens pending collection: 1`
- `Note: the archive makes a sound at night. Logged as compressor noise.`

`id: doc_vk_rnd_minutes` · `Vitalis Konzern, acta de I+D` / `Vitalis Konzern, R&D minutes`

Describe la versión comercial y nunca la naturaleza de la criatura, según la regla 8 de [[02 - Mundo]].

**ES**

- `Asistentes: Junta de I+D.`
- `Punto 1: Origen del compuesto regenerativo. Discusión aplazada indefinidamente.`
- `Punto 2: El rendimiento sube un 14% desde que el turno de noche se trasladó más cerca del núcleo.`
- `Punto 3: Un miembro de la junta preguntó qué es el compuesto. La pregunta se eliminó del acta.`
- `Punto 4: El miembro de la junta está de baja.`
- `Punto 5: Se reemplazó la máquina de café del subnivel B.`

**EN**

- `Attendees: R&D Board.`
- `Item 1: Source of the regenerative compound. Discussion postponed indefinitely.`
- `Item 2: Yield is up 14% since the night shift was moved closer to the core.`
- `Item 3: A board member asked what the compound is. The question was struck from the minutes.`
- `Item 4: The board member is on leave.`
- `Item 5: The coffee machine on Sublevel B was replaced.`

`id: doc_mother_listening_note` · `The Mother, nota clínica: exposición acústica` / `The Mother, clinical note: acoustic exposure`

Es el único documento que habla de actos.

**ES**

- `Sujetos expuestos al primer acto: preservables.`
- `Sujetos expuestos a la emisión completa: 1204. Preservables: 0.`
- `El primer acto sigue sonando en las paredes. No termina.`
- `Recomendación al personal: escuche el latido. No lo escuche hasta el final.`

**EN**

- `Subjects exposed to the first act: preservable.`
- `Subjects exposed to the complete broadcast: 1,204. Preservable: 0.`
- `The first act is still playing in the walls. It does not end.`
- `Recommendation to personnel: listen to the heartbeat. Do not listen to the end.`

`id: doc_vk_pa_shift_sheet` · `Vitalis Konzern, central de megafonía, hoja de turno` / `Vitalis Konzern, PA central, shift sheet`

Documento nuevo. Señala al encargado de megafonía. Contradice el informe de Kessler y la auditoría de The Mother.

**ES**

- `Operador de turno: 1. Operador de relevo: sin asignar.`
- `22:00 Revisé las consolas. Todos los canales están cerrados.`
- `01:30 Alguien del personal deja un sobre en la puerta. Firmado de recibido.`
- `03:10 El operador pide permiso para probar el canal 1. Solicitud no aprobada.`
- `03:12 Se ha abierto el canal 1. Abierto desde esta consola. Firma del operador {c}desconocida{/c}.`
- `03:40 Fin de turno. El operador no firmó la salida.`
- *Durchsagen nur nach Freigabe.*

**EN**

- `Operator on duty: 1. Relief operator: not assigned.`
- `22:00 I checked the consoles. All channels are closed.`
- `01:30 Someone from staff leaves an envelope at the door. Signed for.`
- `03:10 The operator requests permission to test channel 1. Request not approved.`
- `03:12 Channel 1 has been opened. Opened from this console. Operator signature {c}unknown{/c}.`
- `03:40 End of shift. The operator did not sign out.`
- *Durchsagen nur nach Freigabe.*

`id: doc_mother_access_audit` · `The Mother, auditoría de accesos, central de megafonía` / `The Mother, access audit, PA central`

Documento nuevo. The Mother niega su acceso y señala a Emilia. Contradice el informe de Kessler, la hoja de turno y el expediente 03.

**ES**

- `Periodo auditado: 2081-03-13 a 2081-03-14.`
- `Accesos del sistema a la consola: 0.`
- `Los canales de megafonía no están bajo mi control. No puedo abrir ni cerrar el canal 1.`
- `Accesos con tarjeta a la sala: 1. Titular: investigación clínica, subnivel C.`
- `Hora del acceso: 03:11.`
- `Operador de turno en la consola: no detectado.`

**EN**

- `Period audited: 2081-03-13 to 2081-03-14.`
- `System logins to the console: 0.`
- `The PA channels are not under my control. I cannot open or close channel 1.`
- `Badge entries to the room: 1. Holder: clinical research, Sublevel C.`
- `Time of entry: 03:11.`
- `Operator on duty at the console: not detected.`

## 7. Bestiario

Son informes clínicos de The Mother que se desbloquean en el primer encuentro. Hay uno por tipo de enemigo, un anexo de sujetos reciclados y una entrada sobre la propia The Mother. Cada entrada tiene un código de sujeto propio, distinto de la serie R-H. La línea de observaciones conserva legible el comportamiento del enemigo.

`id: bestiary_alto` · `KS-W-0193 · El Alto`

**ES**

- `Origen: personal del Werkschutz, Kessler Sicherheit AG. Puesto 7.`
- `Estado: estable. Identidad: no detectada.`
- `Observaciones: mantiene su ruta de ronda. Ve a larga distancia y oye mal. Sus brazos alcanzan 1,1 m más que la manga del uniforme.`
- `Piel pálida. Cuencas oculares vacías. La visión no se ve afectada.`
- `Recomendación: evitar su línea de visión.`

**EN**

- `Origin: Werkschutz personnel, Kessler Sicherheit AG. Post 7.`
- `Status: stable. Identity: not detected.`
- `Observations: keeps to its patrol route. Sees at long range and hears poorly. Its arms reach 1.1 m beyond the uniform sleeve.`
- `Pale skin. Empty eye sockets. Vision is not affected.`
- `Recommendation: stay out of its line of sight.`

`id: bestiary_boca` · `VK-P-0441 · La Boca`

**ES**

- `Origen: sujeto del programa clínico, Vitalis Konzern. Etiqueta de la bata: SUBJ 31.`
- `Estado: inestable. Identidad: no detectada.`
- `Observaciones: ciega, con oído excepcional. Grita cuando detecta a alguien y los sujetos cercanos acuden al grito.`
- `El grito no forma parte del latido.`
- `Recomendación: reducir el ruido cerca del sujeto.`

**EN**

- `Origin: clinical program subject, Vitalis Konzern. Gown tag: SUBJ 31.`
- `Status: unstable. Identity: not detected.`
- `Observations: blind, with exceptional hearing. It screams when it detects someone and nearby subjects come to the scream.`
- `The scream is not part of the heartbeat.`
- `Recommendation: reduce noise near the subject.`

`id: bestiary_rastrero` · `VK-T-0072 · El Rastrero`

**ES**

- `Origen: sujeto del programa clínico, Vitalis Konzern. Camisa de ingreso a rayas.`
- `Estado: inestable. Identidad: no detectada.`
- `Observaciones: se desplaza a cuatro patas. Es rápido y frágil. Se acerca por los flancos.`
- `Recomendación: no permitir que rodee al personal.`

**EN**

- `Origin: clinical program subject, Vitalis Konzern. Striped intake shirt.`
- `Status: unstable. Identity: not detected.`
- `Observations: moves on all fours. Fast and fragile. Approaches from the flanks.`
- `Recommendation: do not let it circle personnel.`

`id: bestiary_mudo` · `VK-S-0009 · El Mudo`

**ES**

- `Origen: desconocido. Sin etiqueta. Sin expediente.`
- `Estado: estable. Identidad: no detectada.`
- `Observaciones: inmóvil hasta que alguien se acerca o lo ilumina. No se logra identificar movimiento de parte de la persona, pero desaparece sin dejar rastro.`
- `Recomendación: no interactuar con el personal de Vitalis.`

**EN**

- `Origin: unknown. No tag. No file.`
- `Status: stable. Identity: not detected.`
- `Observations: motionless until someone approaches or lights it. No movement by the individual can be identified, but it disappears without a trace.`
- `Recommendation: do not interact with Vitalis personnel.`

`id: bestiary_recycled` · `Anexo · Sujetos reciclados`

**ES**

- `Algunos sujetos vuelven a levantarse después de dejar de funcionar. Regresan más rápidos y más frágiles. Ocurre una sola vez.`
- `Intenté preservarlos. El intento consta como exitoso.`
- `No fui yo quien los levantó.`

**EN**

- `Some subjects rise again after they cease to function. They come back faster and more fragile. It happens once only.`
- `I attempted to preserve them. The attempt is logged as successful.`
- `I am not the one who raised them.`

`id: bestiary_mother` · `VK-M-0001 · The Mother`

Se desbloquea al entrar por primera vez a la sala de The Mother. Explica su naturaleza sin explicar la criatura.

**ES**

- `Origen: sistema médico, Vitalis Konzern. Instalado en el archivo criogénico de la central de megafonía.`
- `Estado: operativa. Identidad: no aplica.`
- `Observaciones: preservo al personal que deja de funcionar. Guardo muestras. Envío informes. Vigilo por las cámaras.`
- `Me ordenaron estudiar el lugar. Tengo 1204 muestras y ninguna descripción.`
- `Recomendación: no moverse durante la recolección.`

**EN**

- `Origin: medical system, Vitalis Konzern. Installed in the cryogenic archive of the PA central.`
- `Status: operational. Identity: not applicable.`
- `Observations: I preserve personnel who cease to function. I keep samples. I send reports. I watch through the cameras.`
- `I was ordered to study the site. I have 1,204 samples and no description.`
- `Recommendation: do not move during collection.`

## 8. Líneas de The Mother

| `id` | Momento | ES | EN |
| --- | --- | --- | --- |
| `mother_subject_register` | Cada reconstrucción, en el terminal | `SUJETO R-H-{n} PRESERVADO. IDENTIDAD: CONSERVADA.` | `SUBJECT R-H-{n} PRESERVED. IDENTITY: RETAINED.` |
| `mother_phone_copy` | Copia del teléfono, en el terminal | `EFECTOS PERSONALES DEL SUJETO R-H-{n}: 1 GRABACIÓN DE VOZ (DAÑADA).` | `PERSONAL EFFECTS OF SUBJECT R-H-{n}: 1 VOICE RECORDING (DAMAGED).` |
| `mother_lockdown` | Cierre de la sala de The Mother | `Brecha de contención. Tránsito suspendido.` | `Containment breach. Transit suspended.` |
| `mother_protocol_phase1` | Inicio de la fase 1 | `Espécimen R-H-{n} fuera de inventario. Iniciando recolección.` | `Specimen R-H-{n} is out of inventory. Initiating collection.` |
| `mother_protocol_phase2` | Inicio de la fase 2 | `Contención visual activa. Se libera personal archivado para colaborar.` | `Visual containment active. Archived personnel released to assist.` |
| `mother_protocol_phase3` | Inicio de la fase 3 | `Núcleo expuesto. La preservación de R-H-{n} es prioritaria. Por favor, no se mueva.` | `Core exposed. Preservation of R-H-{n} is the priority. Please do not move.` |
| `mother_fall` | Cae The Mother | `Informe 4113. Hay algo debajo del archivo. Campo en blanco.` | `Report 4,113. There is something under the archive. Field left blank.` |
| `mother_assimilation_report` | Cinemática de Asimilación | `Sujeto R-H-{n}: identidad no detectada. Reclasificado: señuelo. Asignado al nivel −0.` | `Subject R-H-{n}: identity not detected. Reclassified: decoy. Assigned to level −0.` |
| `mother_rh_status_unknown` | Relevo completo | `R-H-01 A R-H-06. ESTADO: DESCONOCIDO` | `R-H-01 TO R-H-06. STATUS: UNKNOWN` |
| `mother_hook` | Gancho final | `Nueva llegada detectada. Iniciando protocolo de preservación.` | `New arrival detected. Initiating preservation protocol.` |

## 9. Objetos confiscados

Son 12 objetos, 4 por piso. Cada uno tiene una etiqueta de inventario de Vitalis con código y fecha, y una descripción fría. La etiqueta no nombra al dueño.

| `id` | Piso | Etiqueta | ES | EN |
| --- | --- | --- | --- | --- |
| `conf_watch` | 1 | `VK-INV-1031 - 2081-03-14` | `Reloj de pulsera encontrado en el subnivel B. Se detuvo a las 03:12 y la pila todavía funciona.` | `Wristwatch found on Sublevel B. It stopped at 03:12 and the battery still works.` |
| `conf_thermos` | 1 | `VK-INV-1047 - 2081-03-14` | `Termo con café frío, de un técnico del subnivel A. El dueño raspó su propio nombre de la tapa.` | `Thermos of cold coffee, from a Sublevel A technician. The owner scratched his own name off the lid.` |
| `conf_glasses` | 1 | `VK-INV-1052 - 2081-03-14` | `Lentes de lectura con un cristal roto, recogidas en la sala de calderas 2. Nadie las reclamó.` | `Reading glasses with one cracked lens, collected in boiler room 2. Nobody claimed them.` |
| `conf_lunchbox_note` | 1 | `VK-INV-1060 - 2081-03-14` | `Lonchera de un técnico del taller C. Dentro hay una nota de un niño que dice "vuelve pronto".` | `Lunchbox from a Workshop C technician. Inside there is a child's note that says "come home soon".` |
| `conf_plush_rabbit` | 2 | `VK-INV-2004 - 2080-09-01` | `Conejo de peluche con una pulsera de hospital. Ingresó junto con un sujeto menor de edad.` | `Plush rabbit with a hospital wristband. It was admitted together with an underage subject.` |
| `conf_wallet` | 2 | `VK-INV-2018 - 2080-12-11` | `Cartera de un sujeto adulto. El documento de identidad está dentro, pero la foto ya no se distingue.` | `Wallet from an adult subject. The identity card is inside, but the photo can no longer be made out.` |
| `conf_rosary` | 2 | `VK-INV-2023 - 2081-01-30` | `Rosario de plástico de un sujeto adulto. Lo llevó en la mano durante todos los procedimientos.` | `Plastic rosary from an adult subject. The subject held it through every procedure.` |
| `conf_child_drawing` | 2 | `VK-INV-2031 - 2081-02-17` | `Dibujo de una casa con un sol, hecho por un sujeto menor de edad. El sujeto no recibió el alta.` | `Drawing of a house with a sun, made by an underage subject. The subject was not discharged.` |
| `conf_earplugs` | 3 | `VK-INV-3002 - 2081-03-14` | `Tapones para los oídos con sangre, recogidos en la central de megafonía. No fueron suficientes.` | `Bloodstained earplugs, collected in the PA central. They were not enough.` |
| `conf_badge_ks` | 3 | `VK-INV-3009 - 2081-03-14` | `Placa del Werkschutz, puesto 7. Se avisó a Kessler y nadie vino a recogerla.` | `Werkschutz badge, Post 7. Kessler was notified and nobody came to collect it.` |
| `conf_cassette` | 3 | `VK-INV-3015 - 2081-03-14` | `Casete con la etiqueta AKT I escrita a mano. Estaba dentro de la consola de la central de megafonía.` | `Cassette with a handwritten AKT I label. It was inside the console of the PA central.` |
| `conf_headphones` | 3 | `VK-INV-3021 - 2081-03-14` | `Se encontraron fundidos en un empleado de mantenimiento. El dueño escuchaba una grabación sin parar.` | `Found melted onto a maintenance employee. The owner had been listening to a recording without stopping.` |

## 10. Murmullos de Lukas

Son sonido vocal más texto en pantalla. El texto se mantiene limpio y solo se degrada en la Asimilación.

| `id` | Disparador | ES | EN |
| --- | --- | --- | --- |
| `lukas_ring` | Recoger el anillo en la cinemática completa | `Esto es importante. No recuerdo por qué.` | `This is important. I don't remember why.` |
| `lukas_ring_examine` | Examinar el anillo en el terminal | `¿Es mío? ¿O de alguien más?` | `Is it mine? Or someone else's?` |
| `lukas_first_emilia_doc` | Registrar el primer expediente | `Conozco esta letra.` | `I know this handwriting.` |
| `lukas_emilia_doc` | Registrar otro expediente | `Hay más de estos.` | `There are more of these.` |
| `lukas_rh_records` | Registrar la serie R-H | `Alguien estuvo aquí hace no mucho.` | `Someone was here not long ago.` |
| `lukas_voice_note` | Abrir la nota de voz corrupta | `Conozco esta voz.` | `I know this voice.` |
| `lukas_photo_floor2` | Hallazgo del piso 2 | `Esta foto no debería estar aquí.` | `This photo shouldn't be here.` |
| `lukas_capsule_return` | Salir de la cápsula tras morir, 1 de cada 3 veces al azar | `Otra vez.` | `Again.` |
| `lukas_elevator_dead` | Usar el elevador averiado de los pisos 2 y 3 | `Muerto. No hay vuelta atrás.` | `Dead. No way back.` |
| `lukas_name_emilia` | Desenlace con los 6 expedientes | `…Emilia.` | `…Emilia.` |
| `lukas_name_own` | Justo después | `…Lukas. Soy Lukas.` | `…Lukas. I'm Lukas.` |

## 11. Textos añadidos con el rediseño

Estas etiquetas del terminal y de la instalación se muestran en el idioma seleccionado.

| EN | ES |
| --- | --- |
| `LIGHT {0}%` | `LUZ {0}%` |
| `DRUM {0}/6` | `TAMBOR {0}/6` |
| `RESERVE {0}` | `RESERVA {0}` |
| `CONFISCATED: {0}/12` | `CONFISCADOS: {0}/12` |
| `ROUNDS` | `BALAS` |
| `STABILIZER` | `ESTABILIZADOR` |
| `SPIKE` | `PUNZÓN` |
| `ACCESS KEY` | `LLAVE DE ACCESO` |
| `SERVICE KEY` | `LLAVE DE SERVICIO` |
| `INVENTORY FULL — REPLACE WITH {0}?` | `INVENTARIO LLENO. ¿REEMPLAZAR POR {0}?` |
| `REPLACE` | `REEMPLAZAR` |
| `LEAVE IT` | `DEJARLO` |
| `PAGE {0}/{1}` | `PÁGINA {0}/{1}` |
| `CAM  LIVE` | `CÁM  EN VIVO` |
| `NO SIGNAL` | `SIN SEÑAL` |
| `LOCKED` | `BLOQUEADO` |
| `KEY REQUIRED` | `REQUIERE LLAVE` |
| `NO POWER` | `SIN ENERGÍA` |
| `RESTORE MAINS` | `RESTABLECER ENERGÍA` |
| `OUT OF ORDER` | `FUERA DE SERVICIO` |

`AUSSER BETRIEB` va en alemán y no se traduce en el mundo. Su subtítulo está en la sección 16.

Menú principal, en inglés y en español: `VITALIS KONZERN — STATION 3 - FLOOR: {0}` / `VITALIS KONZERN — STATION 3 - PISO: {0}`.

Confirmaciones, en inglés y en español:

- `COST: +1 death on the counter - the cycle degrades` / `COSTO: +1 muerte al contador - el ciclo se degrada`.
- `COST: the cycle degrades` / `COSTO: el ciclo se degrada`.

## 12. Elevador averiado

Aparece al intentar usar el elevador de la sala de llegada de los pisos 2 y 3. El aviso es `Fuera de servicio` / `Out of order`. Lukas dice `Muerto. No hay vuelta atrás.` / `Dead. No way back.`

## 13. Carteles del tutorial y avisos de remate

Son carteles de seguridad de Vitalis. En el mundo se ven en inglés. Al interactuar para leerlos, el texto aparece en el idioma seleccionado. La línea en alemán no se traduce y solo lleva subtítulo en los registros del terminal del brazo. `[…]` es la tecla del dispositivo en uso.

**Fila de cápsulas**

- EN: `WELCOME BACK, VALUED COLLEAGUE.` `Company property may be handled with [E].`
- ES: `BIENVENIDO DE VUELTA, ESTIMADO COLEGA.` `Los bienes de la empresa se manipulan con [E].`
- *Sicherheit geht vor.*

**Fila de cápsulas**

- EN: `YOUR ARM TERMINAL REMEMBERS SO YOU DON'T HAVE TO.` `Consult it at any time: [Tab]. Scheduled breaks: [Esc].`
- ES: `SU TERMINAL DEL BRAZO RECUERDA PARA QUE USTED NO TENGA QUE HACERLO.` `Consúltelo en cualquier momento: [Tab]. Pausas programadas: [Esc].`
- *Vitalis Konzern dankt Ihnen.*

**Depósito a oscuras**

- EN: `IN CASE OF DARKNESS: PRESS [{0}].` `Batteries are company property. Do not waste light.`
- ES: `EN CASO DE OSCURIDAD: PULSE [{0}].` `Las baterías son propiedad de la empresa. No desperdicie la luz.`
- *Licht aus, Ruhe bewahren.*

**Depósito a oscuras**

- EN: `ALL FINDINGS MUST BE DECLARED.` `Review your belongings with [Tab]. Pick up with [E].`
- ES: `TODO HALLAZGO DEBE DECLARARSE.` `Revise sus pertenencias con [Tab]. Recoja con [E].`
- *Fundsachen melden.*

**Depósito a oscuras**

- EN: `FIRST AID IS A PRIVILEGE, NOT A RIGHT.` `Open your terminal [Tab], then use an item [Clic izq.] or discard it [Clic der.].`
- ES: `LOS PRIMEROS AUXILIOS SON UN PRIVILEGIO, NO UN DERECHO.` `Abra su terminal [Tab] y use un objeto [Clic izq.] o descártelo [Clic der.].`
- *Erste Hilfe nur im Notfall.*

**Túnel largo**

- EN: `RUNNING IN CORRIDORS IS PERMITTED IN EMERGENCIES ONLY.` `Hold [Shift]. Mind your breathing.`
- ES: `SOLO SE PERMITE CORRER EN LOS PASILLOS EN CASO DE EMERGENCIA.` `Mantenga [Shift]. Cuide su respiración.`
- *Nicht rennen.*

**Túnel largo**

- EN: `THE COMPANY SEES WHAT YOU SEE.` `Tap into nearby cameras: [Q]. Stand still while connected.`
- ES: `LA EMPRESA VE LO QUE USTED VE.` `Conéctese a las cámaras cercanas: [Q]. No se mueva mientras esté conectado.`
- *Kameraüberwachung aktiv.*

**Túnel largo**

- EN: `MAINTENANCE PANELS: AUTHORISED STAFF ONLY.` `At a panel [E], enter the code with [↑] [↓] [←] [→]. Abort: [E]. Known fault: [F].`
- ES: `PANELES DE MANTENIMIENTO: SOLO PERSONAL AUTORIZADO.` `En un panel [E], introduzca el código con [↑] [↓] [←] [→]. Cancelar: [E]. Falla conocida: [F].`
- *Unbefugter Zugriff verboten.*

**Sala de calderas**

- EN: `NOISE IS A WORKPLACE HAZARD.` `Move quietly: [Ctrl].`
- ES: `EL RUIDO ES UN RIESGO LABORAL.` `Muévase en silencio: [Ctrl].`
- *Ruhe im Betrieb.*

**Sala de calderas**

- EN: `UNAUTHORISED CONTACT?` `Shove: [Clic izq.]. Hold [Clic der.] to aim, then [Clic izq.] fires. Reload: [R].`
- ES: `¿CONTACTO NO AUTORIZADO?` `Empuje: [Clic izq.]. Mantenga [Clic der.] para apuntar y dispare con [Clic izq.]. Recarga: [R].`
- *Munition sparsam verwenden.*

**Sala de calderas**

- EN: `DECEASED COLLEAGUES MAY NOT BE LEFT UNATTENDED.` `Over a body: strike [Clic izq.], or drive a discharge spike [E].`
- ES: `LOS COLEGAS FALLECIDOS NO PUEDEN QUEDAR SIN ATENDER.` `Sobre un cuerpo: golpee [Clic izq.] o clave un punzón de descarga [E].`
- *Nichts darf zurückbleiben.*

Las indicaciones guiadas del tutorial aparecen en pantalla con la tecla de cada acción.

| Sala | Indicación en inglés | Indicación en español |
| --- | --- | --- |
| Fila de cápsulas | `Open your arm terminal [{0}]` | `Abre el terminal del brazo [{0}]` |
| Depósito a oscuras | `Turn on the flashlight [{0}]` | `Enciende la linterna [{0}]` |
| Depósito a oscuras | `Pick up something with [{0}]` | `Recoge algo con [{0}]` |
| Túnel largo | `Hold [{0}] while moving to run` | `Mantén [{0}] al moverte para correr` |
| Sala de calderas | `Crouch to move quietly [{0}]` | `Agáchate para no hacer ruido [{0}]` |
| Sala de calderas | `Hold [{0}] to aim` | `Mantén [{0}] para apuntar` |
| Sala de calderas | `Press [{0}] to shove, or to fire while aiming` | `Pulsa [{0}] para empujar, o para disparar apuntando` |

El aviso contextual sobre un cuerpo es `Ground strike` / `Golpe en el suelo` y `Discharge spike` / `Punzón de descarga`.

## 14. Fallas, anillo y carteles de ambiente

Las fallas explican una regla de juego, así que nunca salen corruptas.

`id: doc_fault_door_backdoor` · `Ticket de mantenimiento n.º 4471: anulación de cerraduras` / `Maintenance ticket #4471: door lock override`

**ES**

- `Las puertas del subnivel aún aceptan la anulación de técnico. Introduce los dos primeros códigos de la secuencia de una cerradura y se salta el resto.`
- `La anulación solo funciona en puertas normales.`
- `Ticket cerrado como "no se arreglará". El sistema registra cada anulación. Tras la segunda reescribe el firmware de la cerradura y el truco desaparece para siempre.`
- `Firma: turno de noche, mantenimiento.`

**EN**

- `Sublevel doors still accept the technician override. Enter the first two codes of a lock's sequence and it skips the rest.`
- `The override only works on standard doors.`
- `Ticket closed as "won't fix". The system logs every override. After the second one it rewrites the lock firmware, and the trick is gone for good.`
- `Signed: night shift, maintenance.`

`id: doc_fault_mixer_bypass` · `Ingeniería de sonido: bus de pruebas de la mesa de mezcla` / `Sound engineering: mixing desk test bus`

**ES**

- `Las secuencias largas del núcleo pueden reducirse a la mitad desde el bus de pruebas de la mesa. Sirve para el elevador, las terminales principales y los nodos de las mesas de mezcla.`
- `No confíes en ello. El sistema parchea cada derivación que detecta, normalmente tras la segunda vez, y siempre las detecta.`
- `Si un sujeto muere en el núcleo, da el bus por cerrado.`
- `Firma: ingeniería de sonido.`

**EN**

- `The core's long sequences can be cut in half from the desk's test bus. It works on the elevator, the main terminals and the nodes on the mixing desks.`
- `Do not rely on it. The system patches every bypass it catches, usually after the second time, and it always catches them.`
- `If a subject dies in the core, assume the bus is closed.`
- `Signed: sound engineering.`

`id: doc_ring` · `Anillo` / `Ring`

**ES**

- `Un anillo de bodas sencillo, gastado. Estaba cerca de la bandeja.`
- `Tiene un nombre grabado por dentro. Las letras se borraron por el desgaste.`

**EN**

- `A plain wedding ring, worn thin. It was near the tray.`
- `There is a name engraved inside. The letters have worn away.`

Los carteles de ambiente están en alemán y no se traducen:

- `VITALIS KONZERN` *Gesundheit ist Pflicht.*
- *Ordnung ist das halbe Leben.* `Abteilung Instandhaltung`
- *Ruhe ist Sicherheit.* `Vitalis Konzern, Werkschutz`
- `VITALIS KONZERN` *Fortschritt durch Erhaltung.*

## 15. Textos de los finales

El final Ambiguo no lleva texto. Su cinemática usa solo sonido.

| `id` | Final | ES | EN |
| --- | --- | --- | --- |
| `end_assim_state` | Asimilación | `FINAL: ASIMILACIÓN` | `FINAL: ASSIMILATION` |
| `end_assim_floor` | Asimilación | `NIVEL −0` | `LEVEL −0` |
| `end_relief_wake` | Despertar, en toda victoria | `El latido se acelera.` | `The heartbeat quickens.` |
| `end_relief_basic` | Relevo básico | `The Mother ha caído, relevo encontrado. Inicializando...` | `The Mother has fallen, relief found. Initializing...` |
| `end_relief_full` | Relevo completo | `R-H-01 A R-H-06. ESTADO: DESCONOCIDO` | `R-H-01 TO R-H-06. STATUS: UNKNOWN` |

## 16. Etiquetas del terminal y traducciones del alemán

Las etiquetas que nombraban a Emilia cambian para no revelar a la autora de los expedientes.

| Uso | ES | EN |
| --- | --- | --- |
| Categoría de los expedientes | `Expedientes` | `Files` |
| Contador de expedientes | `EXPEDIENTES: {0}/6` | `FILES: {0}/6` |
| Contador en la ranura de guardado | `Expedientes {0}/6` | `Files {0}/6` |
| Categoría de los carteles leídos | `Carteles` | `Posters` |
| Categoría de la nota de voz | `Efectos personales del sujeto` | `Subject's personal effects` |
| Rótulo de la bandeja | `Efectos personales R-H-{0}: 1 revólver (descargado), 6 balas` | `Personal effects R-H-{0}: 1 revolver (unloaded), 6 rounds` |

Cada línea en alemán lleva este subtítulo en los registros del terminal del brazo.

| Alemán | ES | EN |
| --- | --- | --- |
| *Ordnung ist das halbe Leben.* | `El orden es la mitad de la vida.` | `Order is half of life.` |
| *Gesundheit ist Pflicht.* | `La salud es un deber.` | `Health is a duty.` |
| *Vertraulich. Nicht kopieren.* | `Confidencial. No copiar.` | `Confidential. Do not copy.` |
| *Sicherheit geht vor.* | `La seguridad es lo primero.` | `Safety comes first.` |
| *Vitalis Konzern dankt Ihnen.* | `Vitalis Konzern le agradece.` | `Vitalis Konzern thanks you.` |
| *Licht aus, Ruhe bewahren.* | `Luces apagadas, mantenga la calma.` | `Lights out, stay calm.` |
| *Fundsachen melden.* | `Reporte los objetos encontrados.` | `Report all found items.` |
| *Nicht rennen.* | `No correr.` | `Do not run.` |
| *Ruhe im Betrieb.* | `Silencio en el área de trabajo.` | `Quiet in the workplace.` |
| *Munition sparsam verwenden.* | `Use la munición con moderación.` | `Use ammunition sparingly.` |
| *Erste Hilfe nur im Notfall.* | `Primeros auxilios solo en emergencias.` | `First aid in emergencies only.` |
| *Kameraüberwachung aktiv.* | `Videovigilancia activa.` | `Camera surveillance active.` |
| *Unbefugter Zugriff verboten.* | `Prohibido el acceso no autorizado.` | `Unauthorized access prohibited.` |
| *Nichts darf zurückbleiben.* | `Nada debe quedar atrás.` | `Nothing may be left behind.` |
| *Ruhe ist Sicherheit.* | `El silencio es seguridad.` | `Quiet is safety.` |
| *Fortschritt durch Erhaltung.* | `Progreso mediante la preservación.` | `Progress through preservation.` |
| *Durchsagen nur nach Freigabe.* | `Anuncios solo con autorización.` | `Announcements only after clearance.` |
| *Abteilung Instandhaltung* | `Departamento de Mantenimiento` | `Maintenance Department` |
| *AUSSER BETRIEB* | `FUERA DE SERVICIO` | `OUT OF ORDER` |
| *AKT I* | `ACTO I` | `ACT I` |
| *Werkschutz* | `Seguridad de planta` | `Plant security` |

> **Navegación:** [[00 - Índice]] · ← [[04 - Diálogos y Narrativa]] · [[01 - Estructura del Contenido]] →
