# Textos del Juego

## Descripción de la sección

Borradores de todos los textos del juego: nota de voz de Emilia, documentos, bestiario, líneas de The Mother, objetos confiscados y murmullos de Lukas. El contexto, el tono y las reglas de cada voz están en [[04 - Diálogos y Narrativa]]; esta nota contiene solo el texto.

> [!warning] Borrador
> Todos los textos de esta nota son **borradores** pendientes de revisión. Se redactan en inglés (idioma original, D-62) con su traducción al español. Las fechas concretas también son borrador, dentro de la línea temporal de D-125.

## Convenciones

- **Identificador:** cada texto tiene un `id` que coincide con el nombre de su ScriptableObject y con la clave de la tabla de Localization ([[04 - Datos y Persistencia]]).
- **Corrupción (D-127):** todo pasaje que nombra a Lukas, se dirige a él o habla de su vida con Emilia se registra corrupto, en cualquier documento. Se marca con `{c}…{/c}`: el terminal muestra ese tramo con símbolos y letras corruptas (glitch morado, D-58) y lo muestra limpio en el desenlace si se reunieron los 6 documentos de Emilia (D-55). La corrupción nunca toca rutas ni reglas (D-45).
- **Alemán (D-58):** va en *cursiva* y no se traduce en ningún idioma ni contiene información necesaria.
- **Variables:** `{n}` es el número del código de sujeto actual de Lukas (R-H-07, R-H-08…).
- **Fechas:** formato `YYYY-MM-DD`, como en el terminal.

## Línea temporal de referencia (D-125)

| Fecha | Hecho | Registro de The Mother |
|---|---|---|
| 2080-11-02 | Emilia empieza a transcribir el latido | — |
| 2081-03-14 | El incidente: el latido se transmite por megafonía | R-H-01 |
| 2081-04-02 | Vitalis despide a Emilia y sella la instalación | — |
| 2082-10-19 | Primera visita de Emilia a la instalación sellada | R-H-02 |
| 2083-01-27 | Visita | R-H-03 |
| 2083-05-09 | Visita | R-H-04 |
| 2083-08-30 | Visita | R-H-05 |
| 2083-11-18 | Última visita | R-H-06 |
| 2083-12-02 | El teléfono de Lukas recibe la nota de voz programada | — |
| 2083-12-03 | Primera reconstrucción de Lukas | R-H-07 |

## Nota de voz de Emilia

`id: voice_note_emilia` · Transcripción con sonido vocal y forma de onda (D-52, D-53). En el terminal del brazo se guarda corrupta hasta el desenlace.

**EN**

> {c}Lukas. If you're hearing this, I didn't come back.{/c}
> I've been looking into what happened at Vitalis. The incident. I found something in the sound and I went back for the rest of it.
> Something went wrong. I don't remember… I don't remember what I'm forgetting.
> Don't come looking for me. {c}Please. Not you. Stay home. Keep the photo.{/c}
> *(breathing)* …it's in the walls. I can hear it from here.
> *(footsteps, running)*

**ES**

> {c}Lukas. Si estás oyendo esto, no volví.{/c}
> He estado investigando lo que pasó en Vitalis. El incidente. Encontré algo en el sonido y volví por el resto.
> Algo salió mal. No recuerdo… no recuerdo qué estoy olvidando.
> No vengas a buscarme. {c}Por favor. Tú no. Quédate en casa. Guarda la foto.{/c}
> *(respiración)* …está en las paredes. Lo oigo desde aquí.
> *(pasos, corre)*

## Documentos de Emilia

Diario íntimo y fragmentado: empieza técnico y se quiebra (D-57). Nunca usa el código R-H (D-58); sus fechas coinciden con los registros de The Mother. Se contradice sobre quién abrió el canal (D-49). Reparto por piso según D-121. La investigación es una **partitura** del latido (D-128).

### Piso 1

`id: doc_emilia_01` · **Notebook — 2080-11-02**

**EN**

> Sublevel C, acoustic lab. The pulse under the plant is not a heartbeat. A heartbeat repeats. This develops.
> I am transcribing it as a score: phrases, rests, returns. Tempo is stable at 41 bpm. Every 3 hours the material comes back slightly changed, like a theme after its variation.
> It has the shape of an opening. Openings are followed by something.
> {c}Lukas asked why I come home smelling of the cold rooms. I said "work". He laughed and made tea anyway.{/c}

**ES**

> Subnivel C, laboratorio acústico. El pulso bajo la planta no es un latido. Un latido se repite. Esto se desarrolla.
> Lo estoy transcribiendo como una partitura: frases, silencios, retornos. El tempo es estable a 41 ppm. Cada 3 horas el material vuelve un poco cambiado, como un tema después de su variación.
> Tiene forma de apertura. Después de una apertura viene algo.
> {c}Lukas me preguntó por qué llego a casa oliendo a las salas frías. Le dije "trabajo". Se rio y preparó té de todos modos.{/c}

`id: doc_emilia_02` · **Notebook — 2081-03-13**

**EN**

> The score is complete up to bar 212. Then a double bar line. The first act ends there.
> The second act exists. I can hear where it should start: the rests change length, as if something were waiting for a cue.
> Requested access to the PA central to compare the room's resonance with the transcription. Denied. Kessler posted a guard at the door.
> I left a copy of the score on the mixing desk. I shouldn't have.
> {c}Tomorrow is our anniversary. I'll be home early. I promised.{/c}

**ES**

> La partitura está completa hasta el compás 212. Luego, una doble barra. Ahí termina el primer acto.
> El segundo acto existe. Oigo dónde debería empezar: los silencios cambian de duración, como si algo esperara una señal.
> Pedí acceso a la central de megafonía para comparar la resonancia de la sala con la transcripción. Denegado. Kessler puso un guardia en la puerta.
> Dejé una copia de la partitura en la mesa de mezclas. No debí hacerlo.
> {c}Mañana es nuestro aniversario. Llegaré temprano. Lo prometí.{/c}

### Piso 2

`id: doc_emilia_03` · **Notebook — 2081-03-20**

**EN**

> I heard it. Only the first act, through the walls of Sublevel C. Then nothing.
> I woke up in a capsule. My hands were cold for two days. The machine said my preservation was "successful".
> I did not open the channel. I was in the acoustic lab. I am almost sure I was in the acoustic lab.
> Vitalis calls me an "unstable survivor". Termination effective 2081-04-02.
> {c}I didn't tell Lukas about the capsule. I told him I was let go. He held me all night and said it was their loss.{/c}

**ES**

> Lo oí. Solo el primer acto, a través de las paredes del subnivel C. Después, nada.
> Desperté en una cápsula. Tuve las manos frías dos días. La máquina dijo que mi preservación fue "exitosa".
> Yo no abrí el canal. Estaba en el laboratorio acústico. Estoy casi segura de que estaba en el laboratorio acústico.
> Vitalis me llama "superviviente inestable". Despido efectivo el 2081-04-02.
> {c}No le conté a Lukas lo de la cápsula. Le dije que me despidieron. Me abrazó toda la noche y dijo que ellos se lo perdían.{/c}

`id: doc_emilia_04` · **Notebook — 2082-10-19**

**EN**

> Went back. My old badge still opens the maintenance hatch; nobody revoked it. Nobody revokes anything here.
> The pulse is louder without people. The building rearranges itself between visits; I mark doors with chalk and the marks are in the wrong rooms.
> The machine greeted me with a number. I will not write it down.
> I died on the stairs of Sublevel C. I woke up in the capsule again. I lost something. I checked the score and it is all there. So it was something else.
> Listen to the heartbeat. The second act starts at the core.

**ES**

> Volví. Mi vieja tarjeta todavía abre la escotilla de mantenimiento; nadie la revocó. Aquí nadie revoca nada.
> El pulso suena más fuerte sin gente. El edificio se reacomoda entre visitas; marco las puertas con tiza y las marcas aparecen en las salas equivocadas.
> La máquina me saludó con un número. No lo voy a escribir.
> Morí en las escaleras del subnivel C. Desperté otra vez en la cápsula. Perdí algo. Revisé la partitura y está completa. Entonces fue otra cosa.
> Escucha el latido. El segundo acto empieza en el núcleo.

### Piso 3

`id: doc_emilia_05` · **Notebook — 2083-05-09**

**EN**

> Third time this year. Or fourth. The dates are in the margins in my handwriting, so it must be true.
> There is a photo in my wallet. A man is holding me and I am laughing. {c}His name is Lukas. His name is Lukas. His name is{/c}
> I know I love him. I wrote it here so the machine can't take it.
> The score now has notes I don't remember writing. Bars 213 to 240. I must have heard them. I must have been closer.
> Listen to the heartbeat. Listen to the heartbeat. Listen.

**ES**

> Tercera vez este año. O cuarta. Las fechas están en los márgenes con mi letra, así que debe ser cierto.
> Hay una foto en mi cartera. Un hombre me abraza y yo me río. {c}Se llama Lukas. Se llama Lukas. Se llama{/c}
> Sé que lo amo. Lo escribo aquí para que la máquina no pueda quitármelo.
> La partitura tiene ahora notas que no recuerdo haber escrito. Compases 213 a 240. Debo haberlos oído. Debo haber estado más cerca.
> Escucha el latido. Escucha el latido. Escucha.

`id: doc_emilia_06` · **Notebook — 2083-11-17**

**EN**

> I opened the channel.
> I did not open the channel.
> Both sentences are in my handwriting, on the same page, with the same pen.
> Tomorrow I go to the core. If the second act plays, I want to be in the room. If I don't come back, the phone will send the recording. {c}He will come anyway. He always comes anyway. Lukas, don't.{/c}
> The score ends at bar 240. The rest is not written. The rest has to be heard.

**ES**

> Yo abrí el canal.
> Yo no abrí el canal.
> Las dos frases tienen mi letra, en la misma página, con la misma pluma.
> Mañana voy al núcleo. Si suena el segundo acto, quiero estar en la sala. Si no vuelvo, el teléfono enviará la grabación. {c}Él vendrá igual. Siempre viene igual. Lukas, no.{/c}
> La partitura termina en el compás 240. El resto no está escrito. El resto hay que oírlo.

## Documentos de Vitalis Konzern, Kessler Sicherheit AG y The Mother

12 documentos, 4 por piso, en salas procedurales; aparecen 1–2 por intento (D-79). No cuentan para la decisión final. Voz burocrática con humor negro corporativo (D-62).

### Piso 1 — Mantenimiento

`id: doc_vk_maintenance_notice` · **Vitalis Konzern — Aviso al personal de mantenimiento**

**EN**

> TO: Maintenance, Sublevels A–C
> RE: Low-frequency sound
> The rhythmic sound reported in the boiler rooms is plant resonance and poses no risk. Staff experiencing anxiety are reminded that listening to the heartbeat has a documented calming effect.
> Do not record the sound. Do not hum along. Overtime will not be paid for "listening breaks".
> *Ordnung ist das halbe Leben.*

**ES**

> PARA: Mantenimiento, subniveles A–C
> ASUNTO: Sonido de baja frecuencia
> El sonido rítmico reportado en las salas de calderas es resonancia de la planta y no supone ningún riesgo. Se recuerda al personal con ansiedad que escuchar el latido tiene un efecto calmante documentado.
> No grabe el sonido. No lo tararee. No se pagarán horas extra por "pausas de escucha".
> *Ordnung ist das halbe Leben.*

`id: doc_ks_patrol_log` · **Kessler Sicherheit AG — Werkschutz, libro de rondas, puesto 3**

**EN**

> 02:00 Round complete. Nothing to report.
> 02:40 Technician asks what is behind Door C-11. Told him it is not his concern. It is not ours either.
> 03:00 Round complete. Nothing to report. Floor feels warm.
> 03:12 Music on the PA. Nothing to report.
> 03:13

**ES**

> 02:00 Ronda completa. Sin novedad.
> 02:40 Un técnico pregunta qué hay detrás de la puerta C-11. Le digo que no es asunto suyo. Tampoco es nuestro.
> 03:00 Ronda completa. Sin novedad. El suelo se siente tibio.
> 03:12 Música por la megafonía. Sin novedad.
> 03:13

`id: doc_vk_regen_poster` · **Vitalis Konzern — Cartel publicitario**

**EN**

> VITALIS REGEN™
> Your body. Renewed. Again.
> Clinically proven regeneration for the modern workforce. Ask your supervisor about early enrollment.
> *Gesundheit ist Pflicht.*
> Side effects may include fatigue, tinnitus and mild loss of personal history.

**ES**

> VITALIS REGEN™
> Tu cuerpo. Renovado. Otra vez.
> Regeneración clínicamente probada para la plantilla moderna. Pregunta a tu supervisor por la inscripción anticipada.
> *Gesundheit ist Pflicht.*
> Los efectos secundarios pueden incluir fatiga, acúfenos y una leve pérdida de la historia personal.

`id: doc_mother_status_report` · **The Mother — Informe automático de estado**

**EN**

> FACILITY STATUS: DECOMMISSIONED
> Personnel on site: 0
> Personnel preserved: {n}
> Reports sent to Vitalis servers: 4,112
> Reports read: 0
> Recommendation: continue reporting.

**ES**

> ESTADO DE LA INSTALACIÓN: DADA DE BAJA
> Personal en el lugar: 0
> Personal preservado: {n}
> Informes enviados a los servidores de Vitalis: 4112
> Informes leídos: 0
> Recomendación: continuar informando.

### Piso 2 — Clínica ilegal

`id: doc_mother_rh_records` · **The Mother — Registro de preservación, serie R-H**

Sin nombre ni estado (D-58). El jugador cruza las fechas con el diario de Emilia.

**EN**

> SERIES R-H — PRESERVATION LOG
> R-H-01 · 2081-03-14 · Cardiac arrest. Acoustic exposure.
> R-H-02 · 2082-10-19 · Fall. Sublevel C stairwell.
> R-H-03 · 2083-01-27 · Hypothermia.
> R-H-04 · 2083-05-09 · Blunt trauma.
> R-H-05 · 2083-08-30 · Asphyxia.
> R-H-06 · 2083-11-18 · Acoustic exposure.
> Preservation: successful. Subject retains identity.

**ES**

> SERIE R-H — REGISTRO DE PRESERVACIÓN
> R-H-01 · 2081-03-14 · Paro cardíaco. Exposición acústica.
> R-H-02 · 2082-10-19 · Caída. Escalera del subnivel C.
> R-H-03 · 2083-01-27 · Hipotermia.
> R-H-04 · 2083-05-09 · Traumatismo contuso.
> R-H-05 · 2083-08-30 · Asfixia.
> R-H-06 · 2083-11-18 · Exposición acústica.
> Preservación: exitosa. El sujeto conserva su identidad.

`id: doc_vk_termination` · **Vitalis Konzern — Recursos Humanos, notificación de despido**

**EN**

> Dr. E. Reiner-Hellen, Acoustic Research
> Following the event of 2081-03-14, you have been classified as an unstable survivor. Your employment ends on 2081-04-02.
> Vitalis Konzern thanks you for your contribution and reminds you that all research, notes and recollections remain company property.
> Please return your badge.

**ES**

> Dra. E. Reiner-Hellen, Investigación Acústica
> Tras el evento del 2081-03-14, ha sido clasificada como superviviente inestable. Su relación laboral termina el 2081-04-02.
> Vitalis Konzern agradece su contribución y le recuerda que toda investigación, nota y recuerdo sigue siendo propiedad de la empresa.
> Por favor, devuelva su tarjeta.

`id: doc_vk_incident_memo` · **Vitalis Konzern — Dirección, memorando interno**

Contradice al registro de Kessler sobre el origen de la transmisión (D-49).

**EN**

> CONFIDENTIAL
> The event of 2081-03-14 was caused by an unauthorized researcher who accessed the PA central and broadcast unapproved material.
> The facility will be sealed and decommissioned. No retrieval of personnel is planned; retrieval is not cost-effective.
> Regen production continues from existing stock.
> *Vertraulich. Nicht kopieren.*

**ES**

> CONFIDENCIAL
> El evento del 2081-03-14 fue causado por una investigadora no autorizada que accedió a la central de megafonía y emitió material no aprobado.
> La instalación será sellada y dada de baja. No se prevé recuperar al personal; la recuperación no es rentable.
> La producción de Regen continúa con las existencias actuales.
> *Vertraulich. Nicht kopieren.*

`id: doc_ks_incident_report` · **Kessler Sicherheit AG — Informe de incidente, puesto 7**

Contradice al memorando de Vitalis (D-49).

**EN**

> Broadcast initiated 03:12 from the PA central console.
> Console log: authenticated by facility system. No badge used.
> Guard on Post 7 (PA central door) reports no one entered or left.
> Guard on Post 7 stopped reporting at 03:14.
> Contract status: active. Invoice attached.

**ES**

> Emisión iniciada a las 03:12 desde la consola de la central de megafonía.
> Registro de la consola: autenticado por el sistema de la instalación. No se usó ninguna tarjeta.
> El guardia del puesto 7 (puerta de la central de megafonía) informa que nadie entró ni salió.
> El guardia del puesto 7 dejó de informar a las 03:14.
> Estado del contrato: activo. Se adjunta factura.

### Piso 3 — Núcleo

`id: doc_mother_preservation_protocol` · **The Mother — Protocolo de preservación del personal (revisión 9)**

Expone la regla del mundo: The Mother reconstruye; el laboratorio asimila (D-47).

**EN**

> 1. Personnel who cease to function are preserved.
> 2. Preservation requires identity. Identity is confirmed by response to the subject code.
> 3. Personnel without identity cannot be preserved. They remain on site. They are not my responsibility.
> 4. Each preservation is successful.
> 5. The number of personnel without identity is increasing. Cause: outside the scope of this protocol.

**ES**

> 1. El personal que deja de funcionar es preservado.
> 2. La preservación requiere identidad. La identidad se confirma por la respuesta al código de sujeto.
> 3. El personal sin identidad no puede ser preservado. Permanece en el lugar. No es mi responsabilidad.
> 4. Cada preservación es exitosa.
> 5. El número de personal sin identidad aumenta. Causa: fuera del alcance de este protocolo.

`id: doc_mother_archive_index` · **The Mother — Índice del archivo criogénico**

**EN**

> CRYOGENIC ARCHIVE — INDEX
> Samples stored: 1,204
> Origin: personnel (all)
> Purpose: Vitalis Regen™ reference material
> Non-conforming specimens pending collection: 1
> Note: the archive sings at night. Logged as compressor noise.

**ES**

> ARCHIVO CRIOGÉNICO — ÍNDICE
> Muestras almacenadas: 1204
> Origen: personal (todo)
> Finalidad: material de referencia de Vitalis Regen™
> Especímenes no conformes pendientes de recolección: 1
> Nota: el archivo canta de noche. Registrado como ruido del compresor.

`id: doc_vk_rnd_minutes` · **Vitalis Konzern — Acta de I+D**

Describe la versión comercial, nunca la naturaleza de la criatura (regla 8 de [[02 - Mundo]]).

**EN**

> Attendees: R&D Board.
> Item 1: Source of the regenerative compound. Discussion postponed indefinitely.
> Item 2: Yield is up 14% since the night shift was moved closer to the core.
> Item 3: A board member asked what the compound is. The question was struck from the minutes.
> Item 4: Coffee machine on Sublevel B replaced.

**ES**

> Asistentes: Junta de I+D.
> Punto 1: Origen del compuesto regenerativo. Discusión aplazada indefinidamente.
> Punto 2: El rendimiento sube un 14% desde que el turno de noche se trasladó más cerca del núcleo.
> Punto 3: Un miembro de la junta preguntó qué es el compuesto. La pregunta se eliminó del acta.
> Punto 4: Se reemplazó la máquina de café del subnivel B.

`id: doc_mother_listening_note` · **The Mother — Nota clínica: exposición acústica**

**EN**

> Subjects exposed to the first act: preservable.
> Subjects exposed to the complete heartbeat: no longer subjects.
> Recommendation to personnel: listen to the heartbeat. Do not listen to all of it.

**ES**

> Sujetos expuestos al primer acto: preservables.
> Sujetos expuestos al latido completo: dejan de ser sujetos.
> Recomendación al personal: escuchen el latido. No lo escuchen entero.

## Bestiario

Informes clínicos de The Mother, uno por tipo; se desbloquean en el primer encuentro (D-48). Cada tipo tiene un código de sujeto propio, distinto de la serie R-H.

`id: bestiary_alto` · **KS-W-0193 — "El Alto"**

**EN**

> Origin: Werkschutz personnel, Kessler Sicherheit AG.
> Status: non-conforming. Identity: not detected.
> Observations: continues patrol routes. Sight: long range. Hearing: poor. Reach exceeds standard uniform sleeve length by 1.1 m.
> Recommendation: avoid its line of sight. Uniform is still under contract.

**ES**

> Origen: personal del Werkschutz, Kessler Sicherheit AG.
> Estado: no conforme. Identidad: no detectada.
> Observaciones: mantiene sus rutas de ronda. Vista: largo alcance. Oído: deficiente. Su alcance supera en 1,1 m el largo de manga del uniforme estándar.
> Recomendación: evitar su línea de visión. El uniforme sigue bajo contrato.

`id: bestiary_boca` · **VK-P-0441 — "La Boca"**

**EN**

> Origin: clinical test subject, Vitalis Konzern.
> Status: non-conforming. Identity: not detected.
> Observations: blind. Hearing: exceptional. On detection it screams; nearby subjects respond to the scream.
> Recommendation: reduce noise. Consent form on file.

**ES**

> Origen: sujeto de prueba clínico, Vitalis Konzern.
> Estado: no conforme. Identidad: no detectada.
> Observaciones: ciega. Oído: excepcional. Al detectar, grita; los sujetos cercanos responden al grito.
> Recomendación: reducir el ruido. Formulario de consentimiento archivado.

`id: bestiary_rastrero` · **VK-T-0072 — "El Rastrero"**

**EN**

> Origin: laboratory technician, Vitalis Konzern.
> Status: non-conforming. Identity: not detected.
> Observations: quadrupedal. Fast. Approaches from the flanks. Fragile.
> Recommendation: do not let it circle. Productivity has improved.

**ES**

> Origen: técnico de laboratorio, Vitalis Konzern.
> Estado: no conforme. Identidad: no detectada.
> Observaciones: cuadrúpedo. Rápido. Se acerca por los flancos. Frágil.
> Recomendación: no permitir que rodee. Su productividad ha mejorado.

`id: bestiary_mudo` · **VK-S-0009 — "El Mudo"**

**EN**

> Origin: unknown. File incomplete.
> Status: non-conforming. Identity: not detected.
> Observations: motionless until approached or illuminated. Often mistaken for a corpse or for a colleague.
> Recommendation: do not greet colleagues.

**ES**

> Origen: desconocido. Expediente incompleto.
> Estado: no conforme. Identidad: no detectada.
> Observaciones: inmóvil hasta que alguien se acerca o lo ilumina. Suele confundirse con un cadáver o con un compañero.
> Recomendación: no saludar a los compañeros.

`id: bestiary_recycled` · **Anexo — Sujetos reciclados**

**EN**

> Some non-conforming subjects rise again after ceasing to function. Faster. More fragile. Once only.
> Cause: their last trace of identity is consumed. I attempted preservation. The attempt is logged as successful.

**ES**

> Algunos sujetos no conformes vuelven a levantarse tras dejar de funcionar. Más rápidos. Más frágiles. Solo una vez.
> Causa: se consume su último rastro de identidad. Intenté la preservación. El intento consta como exitoso.

## Líneas de The Mother

| `id` | Momento | EN | ES |
|---|---|---|---|
| `mother_subject_register` | Primera reconstrucción y cada reconstrucción (terminal) | `SUBJECT R-H-{n} PRESERVED. IDENTITY: RETAINED.` | `SUBJECT R-H-{n} PRESERVED. IDENTITY: RETAINED.` (etiqueta del terminal, en inglés, D-62) |
| `mother_phone_copy` | Copia del teléfono (terminal) | `PERSONAL EFFECTS OF SUBJECT R-H-{n}: 1 VOICE RECORDING (DAMAGED).` | Igual (etiqueta del terminal) |
| `mother_protocol_phase1` | Inicio de la fase 1 | Specimen R-H-{n}: non-conforming. Initiating collection. | Espécimen R-H-{n}: no conforme. Iniciando recolección. |
| `mother_protocol_phase2` | Inicio de la fase 2 | Visual containment active. Releasing archived personnel to assist. | Contención visual activa. Liberando personal archivado para colaborar. |
| `mother_protocol_phase3` | Inicio de la fase 3 | Core exposed. Preservation of R-H-{n} is the priority. Please hold still. | Núcleo expuesto. La preservación de R-H-{n} es prioritaria. Por favor, no se mueva. |
| `mother_fall` | Cae The Mother (secuencia común, D-51) | There is something else. I cannot describe it. It is not in my protocol. | Hay algo más. No puedo describirlo. No está en mi protocolo. |
| `mother_assimilation_report` | Cinemática de Asimilación (D-61) | Subject R-H-{n}: identity not detected. Reclassified: decoy. Assigned to level −0. | Sujeto R-H-{n}: identidad no detectada. Reclasificado: señuelo. Asignado al nivel −0. |
| `mother_rh_status_unknown` | Relevo completo (D-61) | `R-H-01 … R-H-06 — STATUS: UNKNOWN` | Igual (etiqueta del terminal) |
| `mother_hook` | Gancho final (D-51) | New arrival detected. Initiating preservation protocol. | Nueva llegada detectada. Iniciando protocolo de preservación. |

## Objetos confiscados

12 objetos, 4 por piso (D-73). Etiqueta de inventario de Vitalis (código, fecha, propietario) y 1–2 líneas frías.

| `id` | Piso | Objeto | Etiqueta | Línea (EN) | Línea (ES) |
|---|---|---|---|---|---|
| `conf_watch` | 1 | Reloj de pulsera parado | `VK-INV-1031 · 2081-03-14 · Maintenance, Sublevel B` | Stopped at 03:12. Battery functional. | Se detuvo a las 03:12. La pila funciona. |
| `conf_thermos` | 1 | Termo con nombre rayado | `VK-INV-1047 · 2081-03-14 · Maintenance, Sublevel A` | Contents: coffee, cold. Owner's name removed by owner. | Contenido: café, frío. El dueño borró su propio nombre. |
| `conf_glasses` | 1 | Gafas de lectura con un cristal roto | `VK-INV-1052 · 2081-03-14 · Boiler room 2` | Prescription: strong. Owner will not need them. | Graduación: alta. El dueño ya no las necesitará. |
| `conf_lunchbox_note` | 1 | Fiambrera con una nota infantil | `VK-INV-1060 · 2081-03-14 · Workshop C` | Note reads "come home soon". Request not processed. | La nota dice "vuelve pronto". Solicitud no procesada. |
| `conf_plush_rabbit` | 2 | Conejo de peluche con pulsera de hospital | `VK-INV-2004 · 2080-09-01 · Subject, minor` | Admitted with subject. Not required for treatment. | Ingresó con el sujeto. No es necesario para el tratamiento. |
| `conf_wallet` | 2 | Cartera con documento de identidad | `VK-INV-2018 · 2080-12-11 · Subject, adult` | Identity document retained. Identity not retained. | Documento de identidad conservado. Identidad no conservada. |
| `conf_rosary` | 2 | Rosario de plástico | `VK-INV-2023 · 2081-01-30 · Subject, adult` | Used during procedures. No measurable effect. | Usado durante los procedimientos. Sin efecto medible. |
| `conf_child_drawing` | 2 | Dibujo de una casa con un sol | `VK-INV-2031 · 2081-02-17 · Subject, minor` | Drawing of a house. Subject was not discharged. | Dibujo de una casa. El sujeto no recibió el alta. |
| `conf_earplugs` | 3 | Tapones para los oídos ensangrentados | `VK-INV-3002 · 2081-03-14 · PA central` | Insufficient protection. | Protección insuficiente. |
| `conf_badge_ks` | 3 | Placa del Werkschutz, puesto 7 | `VK-INV-3009 · 2081-03-14 · Kessler Werkschutz` | Returned to Kessler. Kessler did not collect it. | Devuelta a Kessler. Kessler no vino a recogerla. |
| `conf_cassette` | 3 | Casete etiquetada a mano "AKT I" | `VK-INV-3015 · 2081-03-14 · PA central` | Do not play. | No reproducir. |
| `conf_headphones` | 3 | Auriculares de estudio derretidos | `VK-INV-3021 · 2081-03-14 · PA central` | Owner kept listening. | El dueño siguió escuchando. |

## Murmullos de Lukas

Sonido vocal más texto en pantalla (D-52). El texto se mantiene limpio y solo se degrada en la Asimilación (D-62).

| `id` | Disparador | EN | ES |
|---|---|---|---|
| `lukas_ring` | Recoger el anillo (cinemática completa) | …This matters. I don't know why. | …Esto importa. No sé por qué. |
| `lukas_ring_examine` | Examinar el anillo en el terminal | Mine? Hers? …Whose? | ¿Mío? ¿De ella? …¿De quién? |
| `lukas_first_emilia_doc` | Registrar el primer documento de Emilia | I know this handwriting. | Conozco esta letra. |
| `lukas_emilia_doc` | Registrar otro documento de Emilia | …Keep going. | …Sigue. |
| `lukas_rh_records` | Registrar la serie R-H | Six before me. | Seis antes que yo. |
| `lukas_voice_note` | Abrir la nota de voz corrupta | Say my name. Say it again. | Di mi nombre. Dilo otra vez. |
| `lukas_photo_floor2` | Hallazgo del piso 2 (D-56) | You're not supposed to be here. …Neither am I. | No deberías estar aquí. …Yo tampoco. |
| `lukas_capsule_return` | Salir de la cápsula tras morir (1 de cada 3 veces, aleatorio) | Again. | Otra vez. |
| `lukas_name_emilia` | Desenlace con los 6 documentos (D-55) | …Emilia. | …Emilia. |
| `lukas_name_own` | Justo después | …Lukas. I'm Lukas. | …Lukas. Soy Lukas. |


> **Navegación:** [[00 - Índice]] · ← [[04 - Diálogos y Narrativa]] · [[01 - Estructura del Contenido]] →
