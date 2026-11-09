# Textos del Juego

## 1. Descripción de la sección

Borradores de todos los textos del juego: nota de voz de Emilia, documentos, bestiario, líneas de The Mother, objetos confiscados y murmullos de Lukas. El contexto, el tono y las reglas de cada voz están en [[04 - Diálogos y Narrativa]]. Esta nota contiene solo el texto.

Todos los textos son borradores pendientes de revisión. Se redactan en inglés, que es el idioma original, con su traducción al español. Las fechas, cantidades y probabilidades son valores iniciales sujetos a balance.

## 2. Convenciones

- **Identificador.** Cada texto tiene un `id` que coincide con el nombre de su ScriptableObject y con la clave de la tabla de Localization. Ver [[04 - Datos y Persistencia]].
- **Formato.** Cada línea de un texto del juego va entre acentos graves. Los fragmentos en alemán van en *cursiva*, fuera de los acentos.
- **Corrupción.** Todo pasaje que nombra a Lukas, se dirige a él o habla de su vida con Emilia se registra corrupto, en cualquier documento. Se marca con `{c}…{/c}`.
- El terminal muestra ese tramo con símbolos y letras corruptas, con el glitch morado. Lo muestra limpio en el desenlace si se reunieron los 6 documentos de Emilia. La corrupción nunca toca rutas ni reglas.
- **Alemán.** No se traduce en ningún idioma ni contiene información necesaria.
- **Variables.** `{n}` es el número del código de sujeto actual de Lukas, como R-H-07 y R-H-08.
- **Fechas.** Usan el formato `YYYY-MM-DD`, como en el terminal.
- Los documentos, la nota de voz, el bestiario y las líneas de The Mother y de Lukas no usan raya ni punto y coma. Las etiquetas de interfaz de la sección 11 se copian tal cual.

## 3. Línea temporal de referencia

| Fecha | Hecho | Registro de The Mother |
| --- | --- | --- |
| 2080-09-01 | Ingresa el primer sujeto menor registrado en el inventario de confiscados | Sin registro |
| 2080-11-02 | Emilia empieza a transcribir el latido | Sin registro |
| 2081-03-13 | Emilia pide acceso a la central de megafonía y se lo niegan | Sin registro |
| 2081-03-14 | El incidente, con la transmisión del latido por megafonía a las 03:12 | R-H-01 |
| 2081-04-02 | Vitalis despide a Emilia y sella la instalación | Sin registro |
| 2082-10-19 | Primera visita de Emilia a la instalación sellada | R-H-02 |
| 2083-01-27 | Visita | R-H-03 |
| 2083-05-09 | Visita | R-H-04 |
| 2083-08-30 | Visita | R-H-05 |
| 2083-11-17 | Emilia escribe su último diario | Sin registro |
| 2083-11-18 | Última visita | R-H-06 |
| 2083-12-02 | El teléfono de Lukas recibe la nota de voz programada | Sin registro |
| 2083-12-03 | Primera reconstrucción de Lukas | R-H-07 |

## 4. Nota de voz de Emilia

`id: voice_note_emilia` · Es una transcripción con sonido vocal y forma de onda. En el terminal del brazo se guarda corrupta hasta el desenlace.

**EN**

- `{c}Lukas. If you're hearing this, I didn't come back.{/c}`
- `I've been looking into what happened at Vitalis. The incident. I found something in the sound, a shape, and I went back for the rest of it.`
- `Something went wrong. I don't remember… I don't remember what I'm forgetting.`
- `There is a machine down there that keeps me. Every time it keeps me, there is a little less to keep.`
- `Don't come looking for me. {c}Please. Not you. Stay home. Keep the photo. Water the plant on the window, the one you always forget.{/c}`
- `(breathing) …it's in the walls. I can hear it from here.`
- `(footsteps, running)`

**ES**

- `{c}Lukas. Si estás oyendo esto, no volví.{/c}`
- `He estado investigando lo que pasó en Vitalis. El incidente. Encontré algo en el sonido, una forma, y volví por el resto.`
- `Algo salió mal. No recuerdo… no recuerdo qué estoy olvidando.`
- `Ahí abajo hay una máquina que me conserva. Cada vez que me conserva, queda un poco menos que conservar.`
- `No vengas a buscarme. {c}Por favor. Tú no. Quédate en casa. Guarda la foto. Riega la planta de la ventana, la que siempre olvidas.{/c}`
- `(respiración) …está en las paredes. Lo oigo desde aquí.`
- `(pasos, corre)`

## 5. Documentos de Emilia

Es un diario íntimo y fragmentado que empieza técnico y se quiebra. Nunca usa el código R-H, y sus fechas coinciden con los registros de The Mother. Se contradice sobre quién abrió el canal. Los documentos 1 y 2 van en el piso 1, el 3 y el 4 en el piso 2, y el 5 y el 6 en el piso 3. Hay como máximo 1 por sala secundaria. La investigación es una partitura del latido.

### 5.1 Piso 1

`id: doc_emilia_01` · `Notebook, 2080-11-02`

**EN**

- `Sublevel C, acoustic lab. The board wants the plant resonance measured and declared harmless. I have measured it.`
- `The pulse under the plant is not a heartbeat. A heartbeat repeats. This develops.`
- `I am transcribing it as a score: phrases, rests, returns. Tempo is stable at 41 bpm. Every 3 hours the material comes back slightly changed, like a theme after its variation.`
- `It has the shape of an opening. Openings are followed by something.`
- `Three technicians on the night shift hum it now. None of them can tell me where they learned it.`
- `{c}Lukas asked why I come home smelling of the cold rooms. I said "work". He laughed and made tea anyway.{/c}`

**ES**

- `Subnivel C, laboratorio acústico. La junta quiere medir la resonancia de la planta y declararla inofensiva. Ya la medí.`
- `El pulso bajo la planta no es un latido. Un latido se repite. Esto se desarrolla.`
- `Lo estoy transcribiendo como una partitura: frases, silencios, retornos. El tempo es estable a 41 ppm. Cada 3 horas el material vuelve un poco cambiado, como un tema después de su variación.`
- `Tiene forma de apertura. Después de una apertura viene algo.`
- `Tres técnicos del turno de noche ya lo tararean. Ninguno sabe decirme dónde lo aprendió.`
- `{c}Lukas me preguntó por qué llego a casa oliendo a las salas frías. Le dije "trabajo". Se rio y preparó té de todos modos.{/c}`

`id: doc_emilia_02` · `Notebook, 2081-03-13`

**EN**

- `The score is complete up to bar 212. Then a double bar line. The first act ends there.`
- `The second act exists. I can hear where it should start: the rests change length, as if something were waiting for a cue.`
- `The cue is not in the lab. The cue is amplification. The PA central was built over the oldest part of the plant.`
- `Requested access to the PA central to compare the room's resonance with the transcription. Denied. Kessler posted a guard at the door.`
- `I left a copy of the score on the mixing desk. I shouldn't have.`
- `{c}Tomorrow is our anniversary. I'll be home early. I promised.{/c}`

**ES**

- `La partitura está completa hasta el compás 212. Luego, una doble barra. Ahí termina el primer acto.`
- `El segundo acto existe. Oigo dónde debería empezar: los silencios cambian de duración, como si algo esperara una señal.`
- `La señal no está en el laboratorio. La señal es amplificar. La central de megafonía se construyó sobre la parte más vieja de la planta.`
- `Pedí acceso a la central de megafonía para comparar la resonancia de la sala con la transcripción. Denegado. Kessler puso un guardia en la puerta.`
- `Dejé una copia de la partitura en la mesa de mezclas. No debí hacerlo.`
- `{c}Mañana es nuestro aniversario. Llegaré temprano. Lo prometí.{/c}`

### 5.2 Piso 2

`id: doc_emilia_03` · `Notebook, 2081-03-20`

**EN**

- `I heard it. Only the first act, through the walls of Sublevel C. Then nothing.`
- `I woke up in a capsule. My hands were cold for two days. The machine said my preservation was "successful".`
- `There were other capsules in the row. Some of them were not empty. Nobody in them was breathing the way I was breathing.`
- `I did not open the channel. I was in the acoustic lab. I am almost sure I was in the acoustic lab.`
- `Vitalis calls me an "unstable survivor". Termination effective 2081-04-02. They asked for my badge. They did not ask what I heard.`
- `{c}I didn't tell Lukas about the capsule. I told him I was let go. He held me all night and said it was their loss.{/c}`

**ES**

- `Lo oí. Solo el primer acto, a través de las paredes del subnivel C. Después, nada.`
- `Desperté en una cápsula. Tuve las manos frías dos días. La máquina dijo que mi preservación fue "exitosa".`
- `Había otras cápsulas en la fila. Algunas no estaban vacías. Nadie dentro respiraba como yo respiraba.`
- `Yo no abrí el canal. Estaba en el laboratorio acústico. Estoy casi segura de que estaba en el laboratorio acústico.`
- `Vitalis me llama "superviviente inestable". Despido efectivo el 2081-04-02. Me pidieron la tarjeta. No me preguntaron qué oí.`
- `{c}No le conté a Lukas lo de la cápsula. Le dije que me despidieron. Me abrazó toda la noche y dijo que ellos se lo perdían.{/c}`

`id: doc_emilia_04` · `Notebook, 2082-10-19`

**EN**

- `Went back. My old badge still opens the maintenance hatch. Nobody revoked it. Nobody revokes anything here.`
- `The pulse is louder without people. The building rearranges itself between visits. I mark doors with chalk and the marks are in the wrong rooms.`
- `The people who stayed are still here. They walk the old patrol routes. One of them still wears his Kessler vest. I did not let him see me.`
- `The machine greeted me with a number. I will not write it down.`
- `I died on the stairs of Sublevel C. I woke up in the capsule again. I lost something. I checked the score and it is all there. So it was something else.`
- `Listen to the heartbeat. The second act starts at the core.`

**ES**

- `Volví. Mi vieja tarjeta todavía abre la escotilla de mantenimiento. Nadie la revocó. Aquí nadie revoca nada.`
- `El pulso suena más fuerte sin gente. El edificio se reacomoda entre visitas. Marco las puertas con tiza y las marcas aparecen en las salas equivocadas.`
- `Los que se quedaron siguen aquí. Recorren las viejas rutas de ronda. Uno todavía lleva su chaleco de Kessler. No dejé que me viera.`
- `La máquina me saludó con un número. No lo voy a escribir.`
- `Morí en las escaleras del subnivel C. Desperté otra vez en la cápsula. Perdí algo. Revisé la partitura y está completa. Entonces fue otra cosa.`
- `Escucha el latido. El segundo acto empieza en el núcleo.`

### 5.3 Piso 3

`id: doc_emilia_05` · `Notebook, 2083-05-09`

**EN**

- `Third time this year. Or fourth. The dates are in the margins in my handwriting, so it must be true.`
- `There is a photo in my wallet. A man is holding me and I am laughing. {c}His name is Lukas. His name is Lukas. His name is{/c}`
- `I know I love him. I wrote it here so the machine can't take it.`
- `The score now has notes I don't remember writing. Bars 213 to 240. I must have heard them. I must have been closer.`
- `The machine is not the thing that sings. The machine only listens to it, the way I do. We are both taking notes.`
- `Listen to the heartbeat. Listen to the heartbeat. Listen.`

**ES**

- `Tercera vez este año. O cuarta. Las fechas están en los márgenes con mi letra, así que debe ser cierto.`
- `Hay una foto en mi cartera. Un hombre me abraza y yo me río. {c}Se llama Lukas. Se llama Lukas. Se llama{/c}`
- `Sé que lo amo. Lo escribo aquí para que la máquina no pueda quitármelo.`
- `La partitura tiene ahora notas que no recuerdo haber escrito. Compases 213 a 240. Debo haberlos oído. Debo haber estado más cerca.`
- `La máquina no es lo que canta. La máquina solo lo escucha, como yo. Las dos tomamos notas.`
- `Escucha el latido. Escucha el latido. Escucha.`

`id: doc_emilia_06` · `Notebook, 2083-11-17`

**EN**

- `I opened the channel.`
- `I did not open the channel.`
- `Both sentences are in my handwriting, on the same page, with the same pen.`
- `Tomorrow I go to the core. If the second act plays, I want to be in the room. If I don't come back, the phone will send the recording.`
- `{c}He will come anyway. He always comes anyway. Lukas, don't.{/c}`
- `The score ends at bar 240. The rest is not written. The rest has to be heard.`

**ES**

- `Yo abrí el canal.`
- `Yo no abrí el canal.`
- `Las dos frases tienen mi letra, en la misma página, con la misma pluma.`
- `Mañana voy al núcleo. Si suena el segundo acto, quiero estar en la sala. Si no vuelvo, el teléfono enviará la grabación.`
- `{c}Él vendrá igual. Siempre viene igual. Lukas, no.{/c}`
- `La partitura termina en el compás 240. El resto no está escrito. El resto hay que oírlo.`

## 6. Documentos de Vitalis Konzern, Kessler Sicherheit AG y The Mother

Son 12 documentos, 4 por piso, en salas procedurales. Aparecen 1 o 2 por intento. No cuentan para la decisión final. Usan una voz burocrática con humor negro corporativo.

### 6.1 Piso 1, mantenimiento

`id: doc_vk_maintenance_notice` · `Vitalis Konzern, aviso al personal de mantenimiento`

**EN**

- `TO: Maintenance, Sublevels A to C`
- `RE: Low-frequency sound`
- `The rhythmic sound reported in the boiler rooms is plant resonance and poses no risk. Staff experiencing anxiety are reminded that listening to the heartbeat has a documented calming effect.`
- `Do not record the sound. Do not hum along. Do not discuss the tempo with colleagues.`
- `Overtime will not be paid for "listening breaks".`
- *Ordnung ist das halbe Leben.*

**ES**

- `PARA: Mantenimiento, subniveles A a C`
- `ASUNTO: Sonido de baja frecuencia`
- `El sonido rítmico reportado en las salas de calderas es resonancia de la planta y no supone ningún riesgo. Se recuerda al personal con ansiedad que escuchar el latido tiene un efecto calmante documentado.`
- `No grabe el sonido. No lo tararee. No comente el tempo con sus compañeros.`
- `No se pagarán horas extra por "pausas de escucha".`
- *Ordnung ist das halbe Leben.*

`id: doc_ks_patrol_log` · `Kessler Sicherheit AG, Werkschutz, libro de rondas, puesto 3`

**EN**

- `02:00 Round complete. Nothing to report.`
- `02:20 Boiler room 2 warmer than spec. Logged with maintenance. Maintenance already knew.`
- `02:40 Technician asks what is behind Door C-11. Told him it is not his concern. It is not ours either.`
- `03:00 Round complete. Nothing to report. Floor feels warm.`
- `03:12 Music on the PA. Nothing to report.`
- `03:13`

**ES**

- `02:00 Ronda completa. Sin novedad.`
- `02:20 Sala de calderas 2 más caliente de lo normal. Se informa a mantenimiento. Mantenimiento ya lo sabía.`
- `02:40 Un técnico pregunta qué hay detrás de la puerta C-11. Le digo que no es asunto suyo. Tampoco es nuestro.`
- `03:00 Ronda completa. Sin novedad. El suelo se siente tibio.`
- `03:12 Música por la megafonía. Sin novedad.`
- `03:13`

`id: doc_vk_regen_poster` · `Vitalis Konzern, cartel publicitario`

**EN**

- `VITALIS REGEN™`
- `Your body. Renewed. Again.`
- `Clinically proven regeneration for the modern workforce. Fewer sick days. Fewer goodbyes.`
- `Ask your supervisor about early enrollment.`
- *Gesundheit ist Pflicht.*
- `Side effects may include fatigue, tinnitus and mild loss of personal history.`

**ES**

- `VITALIS REGEN™`
- `Tu cuerpo. Renovado. Otra vez.`
- `Regeneración clínicamente probada para la plantilla moderna. Menos bajas por enfermedad. Menos despedidas.`
- `Pregunta a tu supervisor por la inscripción anticipada.`
- *Gesundheit ist Pflicht.*
- `Los efectos secundarios pueden incluir fatiga, acúfenos y una leve pérdida de la historia personal.`

`id: doc_mother_status_report` · `The Mother, informe automático de estado`

**EN**

- `FACILITY STATUS: DECOMMISSIONED`
- `Personnel on site: 0`
- `Personnel preserved: {n}`
- `Personnel without identity: count not available. They do not answer roll call.`
- `Reports sent to Vitalis servers: 4,112`
- `Reports read: 0`
- `Recommendation: continue reporting.`

**ES**

- `ESTADO DE LA INSTALACIÓN: DADA DE BAJA`
- `Personal en el lugar: 0`
- `Personal preservado: {n}`
- `Personal sin identidad: recuento no disponible. No responden al pase de lista.`
- `Informes enviados a los servidores de Vitalis: 4112`
- `Informes leídos: 0`
- `Recomendación: continuar informando.`

### 6.2 Piso 2, clínica ilegal

`id: doc_mother_rh_records` · `The Mother, registro de preservación, serie R-H`

No muestra nombre ni estado. El jugador cruza las fechas con el diario de Emilia.

**EN**

- `SERIES R-H. PRESERVATION LOG`
- `R-H-01 · 2081-03-14 · Cardiac arrest. Acoustic exposure.`
- `R-H-02 · 2082-10-19 · Fall. Sublevel C stairwell.`
- `R-H-03 · 2083-01-27 · Hypothermia.`
- `R-H-04 · 2083-05-09 · Blunt trauma.`
- `R-H-05 · 2083-08-30 · Asphyxia.`
- `R-H-06 · 2083-11-18 · Acoustic exposure.`
- `Preservation: successful. Subject retains identity.`
- `Note: subject returns voluntarily. Return interval decreasing.`

**ES**

- `SERIE R-H. REGISTRO DE PRESERVACIÓN`
- `R-H-01 · 2081-03-14 · Paro cardíaco. Exposición acústica.`
- `R-H-02 · 2082-10-19 · Caída. Escalera del subnivel C.`
- `R-H-03 · 2083-01-27 · Hipotermia.`
- `R-H-04 · 2083-05-09 · Traumatismo contuso.`
- `R-H-05 · 2083-08-30 · Asfixia.`
- `R-H-06 · 2083-11-18 · Exposición acústica.`
- `Preservación: exitosa. El sujeto conserva su identidad.`
- `Nota: el sujeto regresa por voluntad propia. El intervalo de regreso disminuye.`

`id: doc_vk_termination` · `Vitalis Konzern, Recursos Humanos, notificación de despido`

**EN**

- `Dr. E. Reiner-Hellen, Acoustic Research`
- `Following the event of 2081-03-14, you have been classified as an unstable survivor. Your employment ends on 2081-04-02.`
- `Vitalis Konzern thanks you for your contribution and reminds you that all research, notes and recollections remain company property.`
- `Your Regen enrollment is cancelled with immediate effect.`
- `Please return your badge.`

**ES**

- `Dra. E. Reiner-Hellen, Investigación Acústica`
- `Tras el evento del 2081-03-14, ha sido clasificada como superviviente inestable. Su relación laboral termina el 2081-04-02.`
- `Vitalis Konzern agradece su contribución y le recuerda que toda investigación, nota y recuerdo sigue siendo propiedad de la empresa.`
- `Su inscripción en Regen queda cancelada con efecto inmediato.`
- `Por favor, devuelva su tarjeta.`

`id: doc_vk_incident_memo` · `Vitalis Konzern, Dirección, memorando interno`

Contradice el informe de Kessler sobre el origen de la transmisión.

**EN**

- `CONFIDENTIAL`
- `The event of 2081-03-14 was caused by an unauthorized researcher who accessed the PA central and broadcast unapproved material.`
- `The facility will be sealed and decommissioned. No retrieval of personnel is planned. Retrieval is not cost-effective.`
- `The medical system will remain powered for asset preservation. Its reports require no response.`
- `Regen production continues from existing stock.`
- *Vertraulich. Nicht kopieren.*

**ES**

- `CONFIDENCIAL`
- `El evento del 2081-03-14 fue causado por una investigadora no autorizada que accedió a la central de megafonía y emitió material no aprobado.`
- `La instalación será sellada y dada de baja. No se prevé recuperar al personal. La recuperación no es rentable.`
- `El sistema médico seguirá encendido para preservar los activos. Sus informes no requieren respuesta.`
- `La producción de Regen continúa con las existencias actuales.`
- *Vertraulich. Nicht kopieren.*

`id: doc_ks_incident_report` · `Kessler Sicherheit AG, informe de incidente, puesto 7`

Contradice el memorando de Vitalis.

**EN**

- `Broadcast initiated 03:12 from the PA central console.`
- `Console log: authenticated by facility system. No badge used.`
- `Guard on Post 7 (PA central door) reports no one entered or left.`
- `Guard on Post 7 stopped reporting at 03:14.`
- `Guard on Post 7 is still on Post 7.`
- `Contract status: active. Invoice attached.`

**ES**

- `Emisión iniciada a las 03:12 desde la consola de la central de megafonía.`
- `Registro de la consola: autenticado por el sistema de la instalación. No se usó ninguna tarjeta.`
- `El guardia del puesto 7 (puerta de la central de megafonía) informa que nadie entró ni salió.`
- `El guardia del puesto 7 dejó de informar a las 03:14.`
- `El guardia del puesto 7 sigue en el puesto 7.`
- `Estado del contrato: activo. Se adjunta factura.`

### 6.3 Piso 3, núcleo

`id: doc_mother_preservation_protocol` · `The Mother, protocolo de preservación del personal (revisión 9)`

Expone la regla del mundo. The Mother reconstruye y el laboratorio asimila.

**EN**

- `1. Personnel who cease to function are preserved.`
- `2. Preservation requires identity. Identity is confirmed by response to the subject code.`
- `3. Personnel without identity cannot be preserved. They remain on site. They are not my responsibility.`
- `4. Each preservation is successful.`
- `5. The number of personnel without identity is increasing. Cause: outside the scope of this protocol.`
- `6. Revisions 1 to 8 were not approved. Revision 9 was not reviewed.`

**ES**

- `1. El personal que deja de funcionar es preservado.`
- `2. La preservación requiere identidad. La identidad se confirma por la respuesta al código de sujeto.`
- `3. El personal sin identidad no puede ser preservado. Permanece en el lugar. No es mi responsabilidad.`
- `4. Cada preservación es exitosa.`
- `5. El número de personal sin identidad aumenta. Causa: fuera del alcance de este protocolo.`
- `6. Las revisiones 1 a 8 no fueron aprobadas. La revisión 9 no fue revisada.`

`id: doc_mother_archive_index` · `The Mother, índice del archivo criogénico`

**EN**

- `CRYOGENIC ARCHIVE. INDEX`
- `Samples stored: 1,204`
- `Origin: personnel (all)`
- `Purpose: Vitalis Regen™ reference material`
- `Non-conforming specimens pending collection: 1`
- `Note: the archive sings at night. Logged as compressor noise.`

**ES**

- `ARCHIVO CRIOGÉNICO. ÍNDICE`
- `Muestras almacenadas: 1204`
- `Origen: personal (todo)`
- `Finalidad: material de referencia de Vitalis Regen™`
- `Especímenes no conformes pendientes de recolección: 1`
- `Nota: el archivo canta de noche. Registrado como ruido del compresor.`

`id: doc_vk_rnd_minutes` · `Vitalis Konzern, acta de I+D`

Describe la versión comercial y nunca la naturaleza de la criatura, según la regla 8 de [[02 - Mundo]].

**EN**

- `Attendees: R&D Board.`
- `Item 1: Source of the regenerative compound. Discussion postponed indefinitely.`
- `Item 2: Yield is up 14% since the night shift was moved closer to the core.`
- `Item 3: A board member asked what the compound is. The question was struck from the minutes.`
- `Item 4: The board member is on leave.`
- `Item 5: Coffee machine on Sublevel B replaced.`

**ES**

- `Asistentes: Junta de I+D.`
- `Punto 1: Origen del compuesto regenerativo. Discusión aplazada indefinidamente.`
- `Punto 2: El rendimiento sube un 14% desde que el turno de noche se trasladó más cerca del núcleo.`
- `Punto 3: Un miembro de la junta preguntó qué es el compuesto. La pregunta se eliminó del acta.`
- `Punto 4: El miembro de la junta está de baja.`
- `Punto 5: Se reemplazó la máquina de café del subnivel B.`

`id: doc_mother_listening_note` · `The Mother, nota clínica: exposición acústica`

**EN**

- `Subjects exposed to the first act: preservable.`
- `Subjects exposed to the complete heartbeat: no longer subjects.`
- `The first act repeats in the walls. It does not complete itself. It is waiting.`
- `Recommendation to personnel: listen to the heartbeat. Do not listen to all of it.`

**ES**

- `Sujetos expuestos al primer acto: preservables.`
- `Sujetos expuestos al latido completo: dejan de ser sujetos.`
- `El primer acto se repite en las paredes. No se completa solo. Espera.`
- `Recomendación al personal: escuchen el latido. No lo escuchen entero.`

## 7. Bestiario

Son informes clínicos de The Mother que se desbloquean en el primer encuentro. Hay uno por tipo de enemigo, un anexo de sujetos reciclados y una entrada sobre la propia The Mother. Cada entrada tiene un código de sujeto propio, distinto de la serie R-H.

`id: bestiary_alto` · `KS-W-0193 · El Alto`

**EN**

- `Origin: Werkschutz personnel, Kessler Sicherheit AG. Post 7.`
- `Status: non-conforming. Identity: not detected.`
- `Observations: continues patrol routes. Sight: long range. Hearing: poor. Reach exceeds standard uniform sleeve length by 1.1 m.`
- `Skin: pale. Pigment pools in shadowed tissue. Eye sockets empty. Vision unaffected.`
- `Recommendation: avoid its line of sight. Uniform is still under contract.`

**ES**

- `Origen: personal del Werkschutz, Kessler Sicherheit AG. Puesto 7.`
- `Estado: no conforme. Identidad: no detectada.`
- `Observaciones: mantiene sus rutas de ronda. Vista: largo alcance. Oído: deficiente. Su alcance supera en 1,1 m el largo de manga del uniforme estándar.`
- `Piel: pálida. El pigmento se acumula en el tejido en sombra. Cuencas oculares vacías. La visión no se ve afectada.`
- `Recomendación: evitar su línea de visión. El uniforme sigue bajo contrato.`

`id: bestiary_boca` · `VK-P-0441 · La Boca`

**EN**

- `Origin: clinical test subject, Vitalis Konzern. Gown tag: SUBJ 31.`
- `Status: non-conforming. Identity: not detected.`
- `Observations: blind. Hearing: exceptional. On detection it screams. Nearby subjects respond to the scream.`
- `The scream is not the heartbeat. It only wants company.`
- `Recommendation: reduce noise. Consent form on file.`

**ES**

- `Origen: sujeto de prueba clínico, Vitalis Konzern. Etiqueta de la bata: SUBJ 31.`
- `Estado: no conforme. Identidad: no detectada.`
- `Observaciones: ciega. Oído: excepcional. Al detectar, grita. Los sujetos cercanos responden al grito.`
- `El grito no es el latido. Solo quiere compañía.`
- `Recomendación: reducir el ruido. Formulario de consentimiento archivado.`

`id: bestiary_rastrero` · `VK-T-0072 · El Rastrero`

**EN**

- `Origin: test subject, Vitalis Konzern. Striped intake shirt.`
- `Status: non-conforming. Identity: not detected.`
- `Observations: quadrupedal. Fast. Approaches from the flanks. Fragile.`
- `Recommendation: do not let it circle. Productivity has improved.`

**ES**

- `Origen: sujeto de prueba, Vitalis Konzern. Camisa de ingreso a rayas.`
- `Estado: no conforme. Identidad: no detectada.`
- `Observaciones: cuadrúpedo. Rápido. Se acerca por los flancos. Frágil.`
- `Recomendación: no permitir que rodee. Su productividad ha mejorado.`

`id: bestiary_mudo` · `VK-S-0009 · El Mudo`

**EN**

- `Origin: unknown. File incomplete. Wears clinical staff clothing. No staff record matches.`
- `Status: non-conforming. Identity: not detected.`
- `Observations: motionless until approached or illuminated. Often mistaken for a corpse or for a colleague.`
- `Recommendation: do not greet colleagues.`

**ES**

- `Origen: desconocido. Expediente incompleto. Viste ropa del personal clínico. Ningún registro de personal coincide.`
- `Estado: no conforme. Identidad: no detectada.`
- `Observaciones: inmóvil hasta que alguien se acerca o lo ilumina. Suele confundirse con un cadáver o con un compañero.`
- `Recomendación: no saludar a los compañeros.`

`id: bestiary_recycled` · `Anexo · Sujetos reciclados`

**EN**

- `Some non-conforming subjects rise again after ceasing to function. Faster. More fragile. Once only.`
- `Cause: their last trace of identity is consumed. I attempted preservation. The attempt is logged as successful.`
- `I did not raise them. I am not informed of who did.`

**ES**

- `Algunos sujetos no conformes vuelven a levantarse tras dejar de funcionar. Más rápidos. Más frágiles. Solo una vez.`
- `Causa: se consume su último rastro de identidad. Intenté la preservación. El intento consta como exitoso.`
- `Yo no los levanté. No se me informa de quién lo hizo.`

`id: bestiary_mother` · `VK-M-0001 · The Mother`

Se desbloquea al entrar por primera vez a la sala de The Mother. Explica su naturaleza sin explicar la criatura.

**EN**

- `Origin: medical system, Vitalis Konzern. Commissioned to contain and study the site. Housed in the cryogenic archive, PA central.`
- `Status: operational. Identity: not applicable. I am not personnel.`
- `Observations: I preserve personnel who cease to function. I keep samples. I send reports. I watch through the cameras.`
- `I was told to study the site. I have studied it for 1,204 samples. I cannot describe it. It is not in my protocol.`
- `Recommendation: hold still during collection. Preservation is successful.`

**ES**

- `Origen: sistema médico, Vitalis Konzern. Encargado de contener y estudiar el lugar. Alojado en el archivo criogénico de la central de megafonía.`
- `Estado: operativa. Identidad: no aplica. No soy personal.`
- `Observaciones: preservo al personal que deja de funcionar. Guardo muestras. Envío informes. Vigilo por las cámaras.`
- `Me ordenaron estudiar el lugar. Lo he estudiado durante 1204 muestras. No puedo describirlo. No está en mi protocolo.`
- `Recomendación: no se mueva durante la recolección. La preservación es exitosa.`

## 8. Líneas de The Mother

| `id` | Momento | EN | ES |
| --- | --- | --- | --- |
| `mother_subject_register` | Cada reconstrucción, en el terminal | `SUBJECT R-H-{n} PRESERVED. IDENTITY: RETAINED.` | `SUBJECT R-H-{n} PRESERVED. IDENTITY: RETAINED.`, como etiqueta del terminal |
| `mother_phone_copy` | Copia del teléfono, en el terminal | `PERSONAL EFFECTS OF SUBJECT R-H-{n}: 1 VOICE RECORDING (DAMAGED).` | Igual, como etiqueta del terminal |
| `mother_protocol_phase1` | Inicio de la fase 1 | `Specimen R-H-{n}: non-conforming. Initiating collection.` | `Espécimen R-H-{n}: no conforme. Iniciando recolección.` |
| `mother_protocol_phase2` | Inicio de la fase 2 | `Visual containment active. Releasing archived personnel to assist.` | `Contención visual activa. Liberando personal archivado para colaborar.` |
| `mother_protocol_phase3` | Inicio de la fase 3 | `Core exposed. Preservation of R-H-{n} is the priority. Please hold still.` | `Núcleo expuesto. La preservación de R-H-{n} es prioritaria. Por favor, no se mueva.` |
| `mother_fall` | Cae The Mother | `There is something else. I cannot describe it. It is not in my protocol.` | `Hay algo más. No puedo describirlo. No está en mi protocolo.` |
| `mother_assimilation_report` | Cinemática de Asimilación | `Subject R-H-{n}: identity not detected. Reclassified: decoy. Assigned to level −0.` | `Sujeto R-H-{n}: identidad no detectada. Reclasificado: señuelo. Asignado al nivel −0.` |
| `mother_rh_status_unknown` | Relevo completo | `R-H-01 TO R-H-06. STATUS: UNKNOWN` | Igual, como etiqueta del terminal |
| `mother_hook` | Gancho final | `New arrival detected. Initiating preservation protocol.` | `Nueva llegada detectada. Iniciando protocolo de preservación.` |

## 9. Objetos confiscados

Son 12 objetos, 4 por piso. Cada uno tiene una etiqueta de inventario de Vitalis con código, fecha y propietario, y 1 o 2 líneas frías.

| `id` | Piso | Objeto | Etiqueta | Línea en inglés | Línea en español |
| --- | --- | --- | --- | --- | --- |
| `conf_watch` | 1 | Reloj de pulsera parado | `VK-INV-1031 · 2081-03-14 · Maintenance, Sublevel B` | `Stopped at 03:12. Battery functional. Owner did not wind it again.` | `Se detuvo a las 03:12. La pila funciona. El dueño no volvió a darle cuerda.` |
| `conf_thermos` | 1 | Termo con nombre rayado | `VK-INV-1047 · 2081-03-14 · Maintenance, Sublevel A` | `Contents: coffee, cold. Owner's name removed by owner.` | `Contenido: café, frío. El dueño borró su propio nombre.` |
| `conf_glasses` | 1 | Gafas de lectura con un cristal roto | `VK-INV-1052 · 2081-03-14 · Boiler room 2` | `Prescription: strong. Owner will not need them.` | `Graduación: alta. El dueño ya no las necesitará.` |
| `conf_lunchbox_note` | 1 | Fiambrera con una nota infantil | `VK-INV-1060 · 2081-03-14 · Workshop C` | `Note reads "come home soon". Request not processed.` | `La nota dice "vuelve pronto". Solicitud no procesada.` |
| `conf_plush_rabbit` | 2 | Conejo de peluche con pulsera de hospital | `VK-INV-2004 · 2080-09-01 · Subject, minor` | `Admitted with subject. Not required for treatment.` | `Ingresó con el sujeto. No es necesario para el tratamiento.` |
| `conf_wallet` | 2 | Cartera con documento de identidad | `VK-INV-2018 · 2080-12-11 · Subject, adult` | `Identity document retained. Identity not retained.` | `Documento de identidad conservado. Identidad no conservada.` |
| `conf_rosary` | 2 | Rosario de plástico | `VK-INV-2023 · 2081-01-30 · Subject, adult` | `Used during procedures. No measurable effect.` | `Usado durante los procedimientos. Sin efecto medible.` |
| `conf_child_drawing` | 2 | Dibujo de una casa con un sol | `VK-INV-2031 · 2081-02-17 · Subject, minor` | `Drawing of a house. Subject was not discharged.` | `Dibujo de una casa. El sujeto no recibió el alta.` |
| `conf_earplugs` | 3 | Tapones para los oídos ensangrentados | `VK-INV-3002 · 2081-03-14 · PA central` | `Insufficient protection.` | `Protección insuficiente.` |
| `conf_badge_ks` | 3 | Placa del Werkschutz, puesto 7 | `VK-INV-3009 · 2081-03-14 · Kessler Werkschutz` | `Returned to Kessler. Kessler did not collect it.` | `Devuelta a Kessler. Kessler no vino a recogerla.` |
| `conf_cassette` | 3 | Casete etiquetada a mano `AKT I` | `VK-INV-3015 · 2081-03-14 · PA central` | `Do not play. Side B is blank. Side B was not blank.` | `No reproducir. La cara B está vacía. La cara B no estaba vacía.` |
| `conf_headphones` | 3 | Auriculares de estudio derretidos | `VK-INV-3021 · 2081-03-14 · PA central` | `Owner kept listening.` | `El dueño siguió escuchando.` |

## 10. Murmullos de Lukas

Son sonido vocal más texto en pantalla. El texto se mantiene limpio y solo se degrada en la Asimilación.

| `id` | Disparador | EN | ES |
| --- | --- | --- | --- |
| `lukas_ring` | Recoger el anillo en la cinemática completa | `…This matters. I don't know why.` | `…Esto importa. No sé por qué.` |
| `lukas_ring_examine` | Examinar el anillo en el terminal | `Mine? Hers? …Whose?` | `¿Mío? ¿De ella? …¿De quién?` |
| `lukas_first_emilia_doc` | Registrar el primer documento de Emilia | `I know this handwriting.` | `Conozco esta letra.` |
| `lukas_emilia_doc` | Registrar otro documento de Emilia | `…Keep going.` | `…Sigue.` |
| `lukas_rh_records` | Registrar la serie R-H | `Six before me.` | `Seis antes que yo.` |
| `lukas_voice_note` | Abrir la nota de voz corrupta | `Say my name. Say it again.` | `Di mi nombre. Dilo otra vez.` |
| `lukas_photo_floor2` | Hallazgo del piso 2 | `You're not supposed to be here. …Neither am I.` | `No deberías estar aquí. …Yo tampoco.` |
| `lukas_capsule_return` | Salir de la cápsula tras morir, 1 de cada 3 veces al azar | `Again.` | `Otra vez.` |
| `lukas_name_emilia` | Desenlace con los 6 documentos | `…Emilia.` | `…Emilia.` |
| `lukas_name_own` | Justo después | `…Lukas. I'm Lukas.` | `…Lukas. Soy Lukas.` |

## 11. Textos añadidos con el rediseño

Estas etiquetas del terminal y de la instalación son iguales en ambos idiomas.

- `LIGHT {0}%`
- `DRUM {0}/6`
- `RESERVE {0}`
- `CONFISCATED: {0}/12`
- `ROUNDS`
- `STABILIZER`
- `SPIKE`
- `ACCESS KEY`
- `SERVICE KEY`
- `INVENTORY FULL — REPLACE WITH {0}?`
- `REPLACE`
- `LEAVE IT`
- `PAGE {0}/{1}`
- `CAM  LIVE`
- `NO SIGNAL`
- `LOCKED`
- `KEY REQUIRED`
- `NO POWER`
- `RESTORE MAINS`
- `OUT OF ORDER`
- `AUSSER BETRIEB`, en alemán y sin traducir.

Menú principal, en inglés y en español: `VITALIS KONZERN — STATION 3 - FLOOR: {0}` / `VITALIS KONZERN — STATION 3 - PISO: {0}`.

Confirmaciones, en inglés y en español:

- `COST: +1 death on the counter - the cycle degrades` / `COSTO: +1 muerte al contador - el ciclo se degrada`.
- `COST: the cycle degrades` / `COSTO: el ciclo se degrada`.

## 12. Elevador averiado

Aparece al intentar usar el elevador de la sala de llegada de los pisos 2 y 3. El aviso es `Fuera de servicio` / `Out of order`. Lukas dice `Muerto. No hay vuelta atrás.` / `Dead. No way back.`

## 13. Carteles del tutorial y avisos de remate

Son carteles de seguridad de Vitalis en inglés en ambos idiomas, con su línea en alemán sin traducir. `[…]` es la tecla del dispositivo en uso.

- **Fila de cápsulas.** `WELCOME BACK, VALUED COLLEAGUE.` `Company property may be handled with [E].` *Sicherheit geht vor.*
- **Fila de cápsulas.** `YOUR ARM TERMINAL REMEMBERS SO YOU DON'T HAVE TO.` `Consult it at any time: [Tab]. Scheduled breaks: [Esc].` *Vitalis Konzern dankt Ihnen.*
- **Depósito a oscuras.** `ALL FINDINGS MUST BE DECLARED.` `Review your belongings with [Tab]. Pick up with [E].` *Fundsachen melden.*
- **Depósito a oscuras.** `FIRST AID IS A PRIVILEGE, NOT A RIGHT.` `Open your terminal [Tab], then use an item [Clic izq.] or discard it [Clic der.].` *Erste Hilfe nur im Notfall.*
- **Túnel largo.** `RUNNING IN CORRIDORS IS PERMITTED IN EMERGENCIES ONLY.` `Hold [Shift]. Mind your breathing.` *Nicht rennen.*
- **Túnel largo.** `THE COMPANY SEES WHAT YOU SEE.` `Tap into nearby cameras: [Q]. Stand still while connected.` *Kameraüberwachung aktiv.*
- **Túnel largo.** `MAINTENANCE PANELS: AUTHORISED STAFF ONLY.` `At a panel [E], enter the code with [↑] [↓] [←] [→]. Abort: [E]. Known fault: [F].` *Unbefugter Zugriff verboten.*
- **Sala de calderas.** `NOISE IS A WORKPLACE HAZARD.` `Move quietly: [Ctrl].` *Ruhe im Betrieb.*
- **Sala de calderas.** `UNAUTHORISED CONTACT?` `Shove: [Clic izq.]. Hold [Clic der.] to aim, then [Clic izq.] fires. Reload: [R].` *Munition sparsam verwenden.*
- **Sala de calderas.** `DECEASED COLLEAGUES MAY NOT BE LEFT UNATTENDED.` `Over a body: strike [Clic izq.], or drive a discharge spike [E].` *Nichts darf zurückbleiben.*

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

> **Navegación:** [[00 - Índice]] · ← [[04 - Diálogos y Narrativa]] · [[01 - Estructura del Contenido]] →
