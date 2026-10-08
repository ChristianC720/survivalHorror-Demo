# Cambios pendientes — Bloque 4 (GDD v0.5.0): Historia y Mundo y nombre final

**Estado:** Aprobado por el equipo, sin aplicar.
**Base:** GDD v0.4.1 (aprobado; workflow `gdd-heartbeat-v041-esposa-personajes`).
**Fecha de los cambios:** 2026-10-08.
**Responsable:** Christian Coronel Palma.

## Alcance

- Completar `03 - Historia y Mundo` (Premisa y Trama, Mundo, Personajes, Diálogos y Narrativa).
- Registrar el nombre final "Identity Dissolution".
- Renombrar el final de 5 muertes a "Asimilación" y "distorsión de voz" a "distorsión del sonido vocal".
- Alinear `02 - Diseño`, Visión y `Decisiones a Implementar.md` con las decisiones nuevas.
- **No editar:** `Christian Decisions.md` (solo lectura), `README.md` (es la autoría de la plantilla, no del juego), carpetas 04 a 09 y 11.

## Estándar del GDD

- Conservar en cada nota el título, "## Descripción de la sección" con su texto, los encabezados de la plantilla y la línea final `> **Navegación:** ...`.
- Rellenar las plantillas; no reemplazarlas. Lo no decidido se marca "Por definir".
- Wikilinks `[[Nota]]`, tablas Markdown, negritas para etiquetas de campo, mermaid permitido. Español, tono técnico-académico.
- Cifras nuevas marcadas "(valor inicial, sujeto a balance)".
- Enlazar a [[02 - Mecánicas]] y a las notas de Historia en lugar de repetir reglas.
- En el Registro, las decisiones modificadas conservan su fila y se marcan "Modificada por D-xx" o "Precisada por D-xx".
- Pilares: V = Vulnerabilidad Constante, A = Adaptación, DC = Decisiones con Consecuencias.

### Regla especial para `03 - Historia y Mundo/03 - Personajes.md`

- El equipo confirmó que la ficha de Lukas es correcta. **Solo se cambia texto de valores**, sin tocar la estructura visual del bloque de Lukas (mismos encabezados, viñetas, etiquetas en negrita, orden y líneas estructurales). Se conservan los nombres propios (Lukas Reiner-Hellen, Emilia Reiner-Hellen), el anillo, la descripción física y la vestimenta.
- Los encabezados de plantilla vacíos (`### Antagonista`, `### Personajes secundarios`, `## NPC`) se rellenan siguiendo la "Plantilla de personaje" de la propia nota.
- Guardar una copia original en `.agents/tasks/gdd-v050-personajes-original.md` antes de editar y reportar diff del bloque de Lukas.

## Decisiones nuevas

| ID | Tema | Decisión | Pilar | Modifica |
|---|---|---|---|---|
| D-41 | Nombre | "Identity Dissolution" es el nombre del proyecto. "Heartbeat" se mantiene como nombre clave | — | — |
| D-42 | Final de derrota | El final de 5 muertes consecutivas se llama **"Asimilación"**: el laboratorio termina de asimilar a Lukas y este se convierte en uno de los enemigos comunes que enfrentaba | A | D-08, D-35 (solo el nombre del final) |
| D-43 | Representación del tema | Solo en la presentación (sin efecto mecánico). (a) Cuerpo ajeno visible: en cada reconstrucción se nota una pieza ajena más, que proviene del laboratorio (3–5 variantes del modelo, valor inicial, sujeto a balance; la mano del anillo puede acabar asimilada). (b) Código de sujeto: The Mother identifica a Lukas con un código que sube en cada reconstrucción (R-H-07, R-H-08…). (c) Los enemigos comunes son personas asimiladas. Se descarta la "memoria que se deshilacha" para pistas y reglas | A | — |
| D-44 | Tema y postura | Postura: **nihilismo puro** frente al horror cósmico (lo incognoscible, la indiferencia, la insignificancia). Tema central: *Identity Dissolution trata sobre buscar sentido dentro de algo que no puede darlo. Lukas lucha por recordar a quien ama, y el laboratorio le demuestra, intento tras intento, que eso nunca importó.* El anillo es el ancla del personaje, no del universo. La motivación del jugador proviene del misterio y del descubrimiento, no de la esperanza | A | — |
| D-45 | Memoria | Lukas recuerda a Emilia en la introducción y la olvida en la primera reconstrucción (la cinemática completa de D-39 es el momento del olvido). Sabe que busca algo o a alguien, se frustra por no recordar a quién, y solo conserva el anillo, que sabe importante. El jugador y Lukas se informan juntos con los documentos. Algunos documentos son **corruptos** (término distinto de las "fallas de la máquina" de M-17), sobre todo los relacionados con la memoria de Lukas; la corrupción nunca oculta información necesaria para avanzar (rutas, reglas) y solo se completa en el desenlace. Al final, Lukas recupera primero el nombre de Emilia y después el suyo | A | — |
| D-46 | Mundo | Distopía retrofuturista con estética de los años 90 y tecnología de ~2080, gobernada por megacorporaciones. **Vitalis Konzern**: corporación de biotecnología y salud. El laboratorio es una instalación subterránea de Vitalis (ubicación geográfica **Por definir**), sellada y dada de baja tras el incidente. The Mother sigue enviando reportes a servidores de la empresa que nadie lee (burocracia automática). Los 3 pisos son un descenso hacia el núcleo. El "laboratorio" **es** la criatura lovecraftiana: la generación procedural es el laboratorio reacomodándose y el laboratorio tiene su propio latido. Vitalis explotaba algo que no entendía y lo vendía como medicina de regeneración (no es un virus ni un arma biológica) | A | — |
| D-47 | The Mother | IA médica de Vitalis creada para contener y estudiar a la criatura. Su protocolo original de preservación del personal está corrupto: reconstruye a todo aquel que llega y todavía conserva identidad, sin entender que cada reconstrucción alimenta la asimilación ni que así mantenía contenido al laboratorio. **The Mother reconstruye; el laboratorio asimila.** Para The Mother, Lukas es un sujeto (código R-H). Justificación del reciclaje (M-20): un enemigo eliminado conserva un último rastro de identidad y The Mother intenta reconstruirlo una vez más; regresa más asimilado (más rápido, más frágil) y después ya no le queda identidad, por eso solo ocurre una vez. Al ser derrotada no puede explicar qué es la criatura (D-16) | V, A | — |
| D-48 | Enemigos y bestiario | Los enemigos comunes son personas que llegaron antes (personal de Vitalis, sujetos de prueba) y fueron asimiladas por el laboratorio. Lukas no es el primero ni es especial. El bestiario del terminal del brazo son informes clínicos de The Mother sobre sujetos asimilados, con código de sujeto; cada entrada se desbloquea en el primer encuentro | A | — |
| D-49 | Emilia | Científica de Vitalis, despedida antes del incidente. Siguió investigando a escondidas de la corporación y se adentró en el laboratorio. Su investigación pudo provocar el incidente, pero los documentos se contradicen y nunca se confirma. The Mother la reconstruyó como sujetos R-H-01 a R-H-06. **No fue asimilada**; su estado en el expediente es "estado desconocido" y ni siquiera The Mother sabe qué ocurrió. Su destino queda **indefinido para la post-demo**; nunca se afirma que esté bien | A | Precisa D-40 |
| D-50 | Lukas y la última pista | Lukas es técnico de mantenimiento de Vitalis Konzern en otra instalación y entra a la instalación sellada con su acceso de técnico. Es R-H-07 porque The Mother lo registra en el mismo expediente de Emilia (empleado y familiar). La última pista es un **mensaje de audio** de Emilia en una tablet o teléfono corporativo de Vitalis, presentado como transcripción con sonido vocal y forma de onda (sin actuación de voz). El dispositivo se pierde en la primera reconstrucción; el mensaje queda copiado en el terminal del brazo, corrupto, y se completa en el desenlace. Cómo muere Lukas al final de la introducción: **Por definir** | A | — |
| D-51 | Finales | Ver tabla "Finales" abajo. Sin final de Renuncia. "Salida" y "Encuentro con Emilia" quedan fuera de la demo. Gancho totalmente abierto: al final solo se escucha una nueva llegada y la voz (sonido) de The Mother iniciando el protocolo. Intención post-demo registrada: en la continuación el protagonista cambia y, si hubo Relevo, Lukas es el antagonista | A | Precisa D-16 |
| D-52 | Narrativa | Sin actuación de voz: todas las voces se representan con sonidos más texto; subtítulos o texto en pantalla obligatorios. Lukas casi no habla: frases cortas y murmullos (sonidos) al encontrar documentos o el anillo; su sonido vocal se distorsiona con la degradación. "Distorsión de voz" pasa a "distorsión del sonido vocal". Tono: clínico y corporativo en documentos de Vitalis y The Mother; fragmentado y personal en los de Emilia. Documentos de Emilia: **6**, 2 por piso, siempre en salas clave secundarias (coincide con el showcase); los documentos de Vitalis y The Mother van en salas procedurales como contexto y no cuentan para la elección final. Idiomas del juego: **inglés principal, español secundario** (el GDD sigue en español) | A | D-08, M-18 (texto) |

### Decisiones por defecto incluidas

- Latido del laboratorio (sonido ambiental): timbre grave distinto del latido de Lukas (M-10), nunca varía con la cercanía de enemigos; solo crece con la degradación acumulada y en el final. Así no compite con la señal de proximidad.
- El Despertar ocurre en todos los finales de victoria (el latido del laboratorio se acelera al caer The Mother).
- La elección final se presenta con dos elementos interactivos en la sala de The Mother después de la fase 3: la terminal central (`E`) y la salida. Sin menú de elección.
- "Todos los documentos" = los 6 documentos de Emilia acumulados en el terminal del brazo, en cualquier intento.
- En complexión y vestimenta manda la ficha de Personajes sobre `Decisiones a Implementar.md`.

## Finales (D-42, D-51)

| Condición | Momento final | Cinemática | Resultado |
|---|---|---|---|
| 5 muertes consecutivas (D-35) | — | **Asimilación** | Lukas se convierte en uno de los enemigos comunes |
| Victoria sin los 6 documentos | Lukas examina la terminal central de forma automática | **Relevo básico** | Despertar; algo ocurre y Lukas reemplaza automáticamente a The Mother (el terminal del brazo lo conecta: es el último con identidad) |
| Victoria con los 6 documentos, examinar la terminal | El jugador elige examinar la terminal central | **Relevo completo** (misma base que el básico, con un par de detalles distintos, entre ellos el expediente R-H-01 a R-H-06 "estado desconocido"; demás detalles **Por definir**) | Despertar; Lukas reemplaza a The Mother sabiendo más |
| Victoria con los 6 documentos, irse | El jugador elige la salida | **Ambiguo** | Despertar; al final solo se escucha el latido. No se sabe si Lukas fue asimilado, si se convirtió en The Mother o si ocurrió otra cosa |

Secuencia común de victoria: 1) cae The Mother; 2) los documentos se completan y Lukas recupera el nombre de Emilia y luego el suyo; 3) Despertar; 4) según la tabla; 5) gancho abierto.

## Estructura narrativa propuesta

- **Acto 1 — Introducción y piso 1:** Lukas entra siguiendo el mensaje de Emilia con su acceso de técnico, muere al final de la introducción, cinemática completa de reconstrucción (olvido), despierta como R-H-07. Aprende el ciclo.
- **Acto 2 — Piso 2:** descubre Vitalis Konzern, el incidente, a Emilia (documentos) y el expediente R-H; documentos corruptos sobre su memoria.
- **Acto 3 — Piso 3 y The Mother:** la verdad sobre reconstrucción y asimilación; jefe; finales.

## Reglas de coherencia del mundo (para `02 - Mundo.md`)

- Qué **es** la criatura nunca se explica; ni The Mother puede explicarlo.
- The Mother reconstruye; el laboratorio asimila. Nunca se invierten.
- Solo se reconstruye a quien conserva identidad.
- La corrupción de documentos nunca oculta información necesaria para avanzar.
- La degradación y la Asimilación son solo presentación; nunca ocultan señales (D-29).
- Vitalis no interviene directamente; su presencia es indiferente y burocrática.
- El destino de Emilia no se confirma en la demo.

## Tareas

### Fase A (un agente; archivos compartidos)

**Task 1: Registro y renombres**
- `10 - Referencia/03 - Registro de Decisiones.md`: agregar D-41 a D-52 a la tabla; fichas completas para D-42, D-44, D-45, D-47, D-49 y D-51; marcar D-08, D-16, D-35 y D-40 según la columna "Modifica".
- `01 - Visión/01 - Ficha del Proyecto.md`: "Nombre del proyecto" = Identity Dissolution (nombre clave Heartbeat se mantiene).
- Reemplazar "Disolución de identidad" **como nombre del final** por "Asimilación" en `02 - Diseño` (Mecánicas M-18, Reglas y Estados "Final de disolución" → "Final: Asimilación", Jugabilidad, Sistemas, Progresión) y en el Registro (conservando lo histórico). Donde la expresión se refiera al tema, se conserva.
- Reemplazar "distorsión de voz" por "distorsión del sonido vocal" donde aplique.

**Task 2: Mecánicas** (`02 - Diseño/02 - Mecánicas.md`)
- M-08: documentos corruptos y los 6 documentos de Emilia en salas clave secundarias.
- M-10: latido del laboratorio distinto del de Lukas.
- M-16: código R-H por reconstrucción, mensaje copiado al terminal, cinemática del olvido.
- M-18: Asimilación y variantes del cuerpo ajeno.
- M-19: decisión final con interactuables y las 3 cinemáticas de victoria.
- M-20: justificación del rastro de identidad.
- Catálogo y matriz si cambian dependencias. Ninguna tecla con dos funciones en el mismo contexto.

**Task 3: `Decisiones a Implementar.md`**
- Marcar como confirmados (con su D-xx) la premisa, el horror cósmico, la criatura y The Mother; mantener el formato del archivo. Indicar que en complexión y vestimenta manda la ficha de Personajes.

### Fase B (5 agentes en paralelo; un dueño exclusivo por archivo o grupo)

- **Task 4:** `03 - Historia y Mundo/01 - Premisa y Trama.md` — premisa (D-40), sinopsis, estructura en 3 actos, tabla de eventos, finales; conservar el callout de spoilers.
- **Task 5:** `03 - Historia y Mundo/02 - Mundo.md` — concepto (época, lugar, contexto, reglas especiales), historia antes/durante/después, lugares (3 pisos, tipos de sala, sala de The Mother), facciones (Vitalis Konzern, The Mother, el laboratorio), coherencia.
- **Task 6:** `03 - Historia y Mundo/03 - Personajes.md` — ficha de Lukas solo texto (rol: técnico de mantenimiento de Vitalis Konzern; motivaciones: el anillo y el olvido; conflicto identitario: el anillo, no el recuerdo; relaciones: Emilia científica de Vitalis, R-H-01 a R-H-06; objetivos y arco definidos según D-44/D-45/D-51; frases o tono sin actuación de voz). Rellenar Antagonista (The Mother), Personajes secundarios (Emilia), NPC (no hay NPC con diálogo; los enemigos comunes son personas asimiladas; bestiario).
- **Task 7:** `03 - Historia y Mundo/04 - Diálogos y Narrativa.md` — métodos narrativos, estructura de diálogo (mensaje de Emilia, textos de The Mother, murmullos de Lukas), tono, decisiones narrativas (elección final), localización (inglés principal, español secundario).
- **Task 8:** resto de `02 - Diseño` — Reglas y Estados (estado Decisión final y las 3 cinemáticas de victoria), Sistemas (S-06 documentos de Emilia en salas secundarias; S-07 contador del código R-H y los 6 documentos; S-08 cinemáticas finales; S-11 latido del laboratorio), Progresión (desbloqueo de la elección), Economía (fila de documentos), Jugabilidad (condición de victoria con los finales).

### Fase C (integración y revisión)

**Task 9: Integración y versión**
- `01 - Visión/02 - Visión del Juego.md`: concepto central con el tema (D-44) y fantasía del jugador actualizada.
- Ficha: versión 0.5.0, última actualización 2026-10-08.
- `10 - Referencia/04 - Historial de Versiones.md`: fila 0.5.0.
- `.agents/tasks/gdd-pendientes-por-definir.md`: actualizada.
- Verificación global: "Final de disolución" y "distorsión de voz" no vigentes; "fallas" no usado para documentos; ninguna aparición de "GesundHealth"; ficha de Lukas con estructura intacta (diff contra la copia); navegación y wikilinks; sin negritas abiertas; archivos fuera de alcance sin cambios.
