# Identity Dissolution — GDD Estructurado

> [!info] Qué es este documento
> Vista consolidada del GDD 0.13.1 con la estructura clásica de un GDD (Basics → Gameplay → Narrativa → Arte → Audio → Tecnología → Plan). No reemplaza a las secciones detalladas: cada apartado enlaza a su fuente. Si hay conflicto, **manda la sección detallada** y el [[03 - Registro de Decisiones]].
>
> Valores numéricos: valores iniciales, sujetos a balance, salvo que se indique otra cosa.

---

## 1. The Basics / Elevator Pitch

### Título y tagline

| Campo | Valor |
|---|---|
| **Título** | Identity Dissolution |
| **Nombre clave** | Heartbeat (D-41) |
| **Tagline** | ***Die. Rebuild. Forget.*** (D-136) |
| **Versión del GDD** | 0.13.1 · Preproducción |

### Quick Pitch

Survival Horror con elementos Roguelite en tercera persona. Lukas, técnico de mantenimiento de Vitalis Konzern, entra a una instalación sellada siguiendo la última nota de voz de su esposa desaparecida, Emilia. Muere, y The Mother —la IA médica de la instalación— lo reconstruye una y otra vez mientras el laboratorio cambia y lo asimila un poco más en cada ciclo.

El jugador desciende 3 pisos generados proceduralmente con un inventario de 6 casillas, recursos escasos y enemigos más fuertes que él. Puede "ver por los ojos de la máquina" accediendo a las cámaras de seguridad desde el terminal de su brazo, pero esa información tiene costo: atrae la atención de The Mother y comienza a localizarlo a través de ondas de sonido, lo cual atrae a los enemigos.

**Premisa oficial (D-40, D-49):** "Un hombre busca la última pista que su esposa desaparecida le dejó, pero queda atrapado en un ciclo sin fin del que ahora debe salir."

**Qué lo hace distinto:**

1. **Cámara con intención narrativa:** OTS por defecto; vista isométrica de vigilancia manual, con ruido, retraso y costo.
2. **Survival Horror con escenarios variables:** cada muerte reacomoda el laboratorio (generación procedural con salas clave fijas que cambian de posición).
3. **Inventario de 6 casillas en tiempo real:** administrar es decidir, no almacenar.
4. **La muerte es parte del ciclo:** reconstrucción, pieza ajena en el cuerpo, código de sujeto que sube (R-H-07, R-H-08…), conocimiento persistente.

Fuente: [[01 - Ficha del Proyecto]], [[02 - Visión del Juego]].

### Género

| Nivel       | Valor                                                                                   | Justificación                                                                 |
| ----------- | --------------------------------------------------------------------------------------- | ----------------------------------------------------------------------------- |
| Principal   | Supervivencia                                                                           | Recursos limitados, amenazas superiores, inventario de 6 casillas             |
| Secundarios | Terror, Roguelite                                                                       | Ambientación y narrativa de terror; generación procedural que cambia al morir |
| Subgénero   | **Survival Horror**                                                                     | Une ambos                                                                     |
| Etiquetas   | Salas clave fijas, procedural, inventario, jefes, sigilo, combate, exploración vertical | —                                                                             |

**Perspectiva:** tercera persona Over the Shoulder (OTS) + vista isométrica de vigilancia (secundaria, manual, con costo).
**Modo:** un jugador, local, sin conexión.

### Who's It For? (Público objetivo)

- **Edad:** 20–30 años. **Experiencia:** intermedia; no requiere haber jugado el género.
- **Intereses:** Survival Horror, terror psicológico y ambiental, exploración, gestión de recursos, generación procedural, narrativas de misterio, juegos donde combatir no siempre es la mejor opción, rejugabilidad y descubrimiento progresivo de la historia.
- **Contenido sensible (D-85):** horror corporal sin desmembramiento ni vísceras, sin desnudez explícita; a los niños solo se alude con objetos.
- **Clasificación objetivo (D-138):** PEGI 16 / ESRB M.

Fuente: [[03 - Público y Plataforma]].

### Where Will People Play? (Plataforma)

| Elemento         | Decisión                                                                                      |
| ---------------- | --------------------------------------------------------------------------------------------- |
| Plataforma       | PC (Windows 64 bits). Sin consolas, macOS ni Linux en la demo                                 |
| Entrada          | Teclado y mouse (principal, reasignable); mando tipo Xbox con mapa fijo (D-120)               |
| Resolución / FPS | 1920×1080 a 60 FPS (recomendado); 1280×720 a 30 FPS (mínimo)                                  |
| Conexión         | No requerida                                                                                  |
| Sesión           | ~10 min mínimo por sesión; autoguardado en salas clave finales de pisos 1 y 2                 |
| Duración total   | ~45–60 min (configuración completa) · ~30 min (configuración **showcase** para eventos, D-34) |

### Inspirations

| Referencia | Qué tomamos | Qué NO tomamos |
|---|---|---|
| *Signalis* (referencia principal) | Retrofuturismo low-poly, texturas crudas, documentos en dos voces, glitch en texto, idioma sin traducir, inventario de 6 casillas, interfaz diegética | Lore, estética comunista, paleta, cámara cenital |
| *Silent Hill* | Protagonista civil roto; monstruos grotescos; terror por atmósfera y sonido | Pueblo, simbolismo religioso, criaturas concretas, niebla como recurso principal |
| *Resident Evil* | Corporación que experimenta con humanos; munición escasa; laboratorios subterráneos | El virus como explicación; acción y abundancia de armas |
| *Alan Wake* | Hombre corriente busca a su esposa; narrativa por fragmentos; ambigüedad | Combate con luz como mecánica central; estructura episódica |
| *The King in Yellow* (Chambers) | Un texto que destruye a quien lo escucha completo → el latido como texto, con primer y segundo acto | Carcosa, Hastur, el Signo Amarillo |
| H. P. Lovecraft | Horror cósmico: lo incognoscible, la indiferencia, la insignificancia | Entidades concretas, tentáculos como cliché |
| Música: Lustmord, Carpenter, Reznor & Ross, Guðnadóttir, Yamaoka, Alanko | Industrial, oscuro, melancólico, minimalista | Temas, melodías, samples |

Fuente: [[02 - Referencias e Inspiración]].

---

## 2. How It Plays (Gameplay & Mechanics)

### Pilares de diseño

| Pilar | Idea | Se refleja en |
|---|---|---|
| **V — Vulnerabilidad Constante** | El jugador siempre en desventaja; combatir es posible, no la solución más segura | Enemigos letales en solitario, sin regeneración de salud, sin mejoras de personaje |
| **A — Adaptación** | Nunca hay conocimiento completo del escenario | Generación procedural por intento; el conocimiento ayuda, pero no permite memorizar |
| **DC — Decisiones con Consecuencias** | Cada recurso, ruta o combate tiene ventaja y costo | 6 casillas, presupuesto decreciente, reciclaje de enemigos, vigilancia con costo |

Principios complementarios: la cámara debe tener intención; la muerte forma parte de la experiencia; el terror surge de los sistemas (no solo de sustos). Fuente: [[04 - Pilares de Diseño]].

### The Core Loop

```text
Explorar → Identificar amenazas y recursos → Combatir / Evadir / Interactuar
→ Descubrir información → Morir → Aplicar conocimiento adquirido → Explorar
```

**Segmento típico (minuto a minuto):** entrar a una zona → observar → explorar → encontrar recursos o información → detectar amenaza → evaluar opciones → actuar → consumir o conservar → continuar.

**Largo plazo — por qué seguir:** supervivencia (llegar más lejos), aprendizaje (enemigos, sistemas, reglas de generación), descubrimiento (qué es el laboratorio, quién es Emilia) y variación entre intentos.

**Ritmo:** alterna exploración (tensión moderada) → acción (breve y significativa) → descanso (nunca seguridad absoluta) → desafío → recompensa → historia (cuando el jugador puede prestar atención).

Fuente: [[01 - Jugabilidad]].

### Key Actions y controles

| Acción | Teclado / Mouse | Mando (fijo) |
|---|---|---|
| Mover / mirar | `WASD` / mouse | Stick izq. / der. |
| Correr | Mantener `Shift` | Mantener `LB` |
| Agacharse (sigilo) | `Ctrl` (alternar) | `B` |
| Apuntar / disparar | Mantener clic der. / clic izq. | `LT` / `RT` |
| Empujón / golpe de culata | Clic izq. sin apuntar | `RT` sin apuntar |
| Recargar | `R` | `X` |
| Interactuar (recoger, leer, abrir, hackear, escalera) | `E` | `A` |
| Linterna | `F` | Cruceta arriba |
| Vista de vigilancia | `Q` | `Y` |
| Terminal del brazo (inventario y registros) | `Tab` | `View` |
| Hackeo (personaje inmóvil) | `WASD` / flechas | Cruceta / stick |
| Remate (cerca de un cuerpo) | Clic izq. = golpe en el suelo · `E` = punzón | `RT` · `A` |
| Pausa | `Esc` | `Menu` |

- Las teclas cambian por **contexto** (exploración, apuntando, inventario, hackeo, vigilancia, sobre un cuerpo, menús, cinemática), sin duplicados dentro de un contexto.
- **No hay salto ni esquiva.** El inventario y el hackeo **no pausan** el juego.
- Reasignación completa de teclado/mouse, sensibilidad, eje invertido, agacharse alternar/mantener y asistencia de apuntado (D-104).

Fuente: [[04 - Controles]].

### Game Rules & Systems

#### Mecánicas (20)

| ID | Mecánica | Resumen | Prioridad |
|---|---|---|---|
| M-01 | Cámara OTS y apuntado | Vista por defecto; apuntar acerca la cámara y concentra la linterna | Demo |
| M-02 | Vista de vigilancia | Isométrica, con ruido CRT y ~0.3 s de retraso; solo en salas con cámara (todas las clave + ~1/3 de las procedurales). Llena un medidor **oculto** de atención de The Mother (~5 s) que atrae enemigos | Demo |
| M-03 | Bloqueo por detección | Con un enemigo en **caza**, la vista se corta y queda bloqueada; en **búsqueda**, la señal se degrada | Demo |
| M-04 | Combate a distancia | 1 arma: revólver de Kessler, tambor de 6, recarga bala a bala | Demo |
| M-05 | Salud y estados | Bien → Herido → Crítico; sin regeneración; se lee en la postura y el terminal | Demo |
| M-06 | Resistencia y empujón | Una barra para correr, empujar y rematar; agotamiento de 2 s en 0 | Demo |
| M-07 | Inventario de 6 casillas | En tiempo real; sin apilado infinito | Demo |
| M-08 | Interacción y documentos | Recolección, lectura e investigación; los documentos van al terminal | Demo |
| M-09 | Sigilo y ruido | Agachado = 0 ruido; correr y disparar alertan; la visión enemiga no atraviesa coberturas | Demo |
| M-10 | Señales diegéticas | Percepción enemiga solo por sonido (sospecha / búsqueda / caza) y latido del protagonista | Demo |
| M-11 | Punzón de descarga | Un uso: se activa solo al ser agarrado (aturde 2 s) **o** remata un cuerpo en silencio | Demo |
| M-12 | Linterna | Barra de ~120 s que se recarga sola; iluminar a un enemigo duplica su alerta | Demo |
| M-13 | Hackeo por secuencias | Cadenas direccionales (2–3 / 4–5 / 7–8 entradas); un error reinicia; personaje inmóvil | Demo |
| M-14 | Escape y retirada | Romper contacto; sellado rápido de puertas | Demo |
| M-15 | Exploración vertical por pisos | 3 pisos; salas procedurales + clave secundarias + clave final; llave obligatoria por piso | Demo |
| M-16 | Reconstrucción y terminal del brazo | Al morir, reaparece en la fila de cápsulas del piso 1; conserva el conocimiento | Demo |
| M-17 | Fallas de la máquina | Atajos descubribles que The Mother parchea tras 1–2 usos | Deseable |
| M-18 | Degradación y final de 5 muertes | Corrupción CRT y distorsión vocal acumulativas (solo estética); 5 muertes seguidas → final Asimilación | Deseable |
| M-19 | Jefe: The Mother | 3 fases que reutilizan mecánicas existentes | Demo |
| M-20 | Reciclaje y remate | Todo enemigo muerto se levanta **una vez** (~40 s, con aviso) salvo que se remate | Demo |

**Descartadas:** conos de visión visibles, esquiva con invulnerabilidad, regeneración de salud, fijar siempre una perspectiva.
**Post-demo:** manipular cajas, saqueo de cadáveres, pasarelas entre salas, techos rotos, detección entre salas, cámaras que "mienten", reasignación del mando.

Fuente: [[02 - Mecánicas]].

#### Sistemas (11)

S-01 Cámara y vigilancia · S-02 IA y percepción · S-03 Salud y combate · S-04 Resistencia · S-05 Inventario · S-06 Generación procedural por pisos · S-07 Persistencia · S-08 Ciclo de muerte y reconstrucción · S-09 Reciclaje de enemigos · S-10 Jefe The Mother · S-11 Audio dinámico. Fuente: [[03 - Sistemas]].

#### Cómo se hace más fuerte el jugador (progresión)

- **El personaje no mejora** (Pilar V): sin niveles, habilidades, armas nuevas ni mejoras. Siempre un revólver, que vuelve **descargado** en cada reconstrucción.
- **Progresa el jugador:** conocimiento persistente en el terminal del brazo (bestiario, documentos, reglas del sistema, fallas). Lo hace "más informado, no más fuerte".
- **Al morir o "Reiniciar":** se pierde todo el inventario; se conservan los registros del terminal. Solo "Nueva partida" los borra.
- **Desbloqueo clave:** reunir los **6 documentos de Emilia** (2 por piso, en salas clave secundarias, en cualquier intento) desbloquea la elección final.

Fuente: [[05 - Progresión]].

#### Economía de recursos

| Recurso | Límite por casilla | Notas |
|---|---|---|
| Curas | 1 | Devuelven un estado de salud; un golpe cancela la animación sin gastarla |
| Munición | 12 balas | Se recoge en blísteres de 4. **Nunca alcanza para matar a todos** (criterio de munición) |
| Punzón de descarga | 1 | Presupuesto bajo; defensa o remate |
| Llaves | 1 | 1 obligatoria por piso + opcionales (atajos o recursos) |
| Documentos, objetos confiscados | — | No ocupan casilla; persisten |

**Presupuesto por piso (completa):**

| Recurso | Piso 1 | Piso 2 | Piso 3 | Sala de The Mother |
|---|---|---|---|---|
| Curas | 2–3 | 1–2 | 1 | 0–1 |
| Munición | 12–16 | 8–12 | 4–8 | 20 fijas |
| Punzones | 2–3 | 1–2 | 0–1 | 0 |
| Enemigos | 6–9 | 9–12 | 12–15 | Jefe + 4 asimilados |

El presupuesto baja mientras los enemigos suben. La configuración showcase escala todo por ~0.75. El **Modo Historia** sube en +1 el mínimo de cada recurso por piso; el **Modo Supervivencia** usa los valores base.

Fuente: [[06 - Economía y Recursos]].

#### Enemigos (IA)

Personas asimiladas por el laboratorio. Estados de alerta: patrulla → **sospecha** → **búsqueda** → **caza**. Máximo 2 enemigos activos por sala; ~1/3 de las salas vacías a propósito.

| Tipo | Rasgo | Disparos para matarlo | Golpes para matar a Lukas | Pisos |
|---|---|---|---|---|
| El Alto | ~2.5 m, golpe de alcance largo, agarra | 4 | 2 | 1–3 |
| El Rastrero | Rápido, flanquea | 2 | 4 | 1–3 |
| La Boca | Ciega; su grito manda a los cercanos en búsqueda hacia ella | 3 | 3 | 2–3 |
| El Mudo | Inmóvil, señuelo; se activa a ~3 m o al iluminarlo; agarra | 3 | 2 | 3 |

**Reciclados:** más rápidos y frágiles (2 disparos; 1 si es Rastrero); solo una vez por enemigo.

Fuente: [[03 - Personajes]], [[03 - Enemigos y NPC]].

#### Condiciones de victoria y derrota

- **Victoria:** derrotar a The Mother al final del piso 3.
- **Derrota:** muerte → reconstrucción al inicio del ciclo con un piso nuevo. "Reiniciar" cuenta como muerte; salir durante una alerta o caza cuenta como muerte completa.
- **Final de derrota "Asimilación"** (Deseable): 5 muertes consecutivas. Solo llegar a la sala clave final del piso 2 reinicia el contador. Las muertes del primer intento (tutorial) no cuentan.

Estados y transiciones: [[07 - Reglas y Estados del Juego]].

### Level Design Ideas

**Estructura:** introducción guionizada → 3 pisos procedurales → sala de The Mother.

| Nivel | Tema y luz | Duración | Enemigos | Clave |
|---|---|---|---|---|
| Introducción | Casa → compuerta → elevador → piso que no existe | ~5 min | Guionizados | Primer hackeo con el teléfono; muerte guionizada |
| Piso 1 — Mantenimiento | Calderas y tuberías; ámbar de sodio | ~12–15 min (+5 de tutorial) | Alto, Rastrero | Tutorial de 4 salas (primer intento); autoguardado |
| Piso 2 — Clínica ilegal | Clínica improvisada; fluorescente frío | ~12–15 min | + la Boca | La foto imposible; reinicio del contador de muertes |
| Piso 3 — Núcleo | Maquinaria de contención; carmesí | ~12–15 min | + el Mudo | Sin guardado; puerta al jefe |
| Sala de The Mother | Central de megafonía; solo máquina | ~3–6 min | Jefe | 3 fases, decisión final |

**6 arquetipos de sala** (vestidos con el tema de cada piso): Pasillo, Almacén, Sala abierta, Sala de vigilancia, Sala de hackeo, Sala vertical.

**Composición por piso:** completa = 8–12 procedurales + 2–3 clave secundarias + 1 clave final · showcase = 6–8 + 2 + 1.

**Reglas del generador:** ruta válida garantizada; llave obligatoria alcanzable antes de la puerta final; ningún recurso mínimo detrás de una llave opcional; la sala final puede empezar sin energía (~1/3; el interruptor está en una sala secundaria); el validador debe pasar 1 000 semillas.

**The Mother — fases:**

1. **Hackeo** de 3 nodos en las mesas de mezcla mientras el brazo recolector patrulla.
2. **Cámaras hostiles:** cada enfoque libera un asimilado (4 en total, máx. 2 activos); niebla criogénica; moverse entre puntos ciegos.
3. **Disparos** a 3 transformadores expuestos (2 balas cada uno).

Fuente: [[02 - Niveles]], [[01 - Estructura del Contenido]].

---

## 3. The Story & World (Narrative)

### Story Overview

**2083, Sektor V-7.** Vitalis Konzern explotaba algo que no entendía y lo vendía como medicina de regeneración. El laboratorio tiene un latido, y ese latido es un **texto**: quien lo escucha completo pierde por completo su identidad y es asimilado. El incidente fue su transmisión por la megafonía. La instalación se selló; solo The Mother sigue operando.

Emilia, científica de Vitalis, oyó solo el **primer acto**. The Mother la reconstruyó (R-H-01). La despidieron y volvió a escondidas por el **segundo acto**, dejando programada una nota de voz para Lukas.

Lukas la recibe y entra pese a que ella le pide que no lo haga. Muere al final de la introducción en un piso que no existe. Al reconstruirse como **R-H-07**, la olvida: solo sabe que busca a alguien y tiene un anillo que encontró en el suelo.

**Estructura en 3 actos (= 3 pisos):**

| Acto | Piso | Contenido |
|---|---|---|
| 1 — La entrada y el ciclo | Introducción + Piso 1 | Nota de voz, muerte, el olvido, tutorial, primeros documentos y bestiario |
| 2 — Vitalis, el incidente, Emilia | Piso 2 | Documentos corporativos; registros R-H-01 a R-H-06; la foto imposible |
| 3 — Reconstrucción y asimilación | Piso 3 + The Mother | La regla del mundo; el jefe; el desenlace |

**Finales (4):**

| Condición | Final | Resultado |
|---|---|---|
| 5 muertes consecutivas | **Asimilación** | Lukas se convierte en un Mudo, el señuelo para el próximo que llegue |
| Victoria sin los 6 documentos | **Relevo básico** | Lukas reemplaza a The Mother sin recordar ningún nombre |
| Victoria con 6, elige la terminal | **Relevo completo** | La reemplaza sabiendo más; oye y entiende el segundo acto (ilegible para el jugador) |
| Victoria con 6, elige la salida | **Ambiguo** | Su latido y el del laboratorio se sincronizan; no se sabe qué fue de él |

Toda victoria pasa por el **Despertar** (el latido del laboratorio se acelera) y cierra con un gancho abierto. **Ningún final ofrece salvación.**

Fuente: [[01 - Premisa y Trama]].

### Characters

| Personaje | Rol | Clave |
|---|---|---|
| **Lukas Reiner-Hellen** | Protagonista | ~30 años, técnico de mantenimiento, obstinado y cansado, no es combatiente. Dos estados visuales: civil (introducción) y R-H-07 (overol teal, terminal en el antebrazo izquierdo). Cada reconstrucción suma una pieza ajena visible |
| **The Mother** | Antagonista y jefe | IA médica de Vitalis. Protocolo clínico, burocrático e indiferente. Físicamente: archivo de muestras criogénicas en una grúa con brazo recolector. Sin cara: su mirada son las cámaras. **Reconstruye**, no asimila |
| **Emilia Reiner-Hellen** | Secundario ausente | Científica brillante y fría, obsesionada con el segundo acto. Solo aparece en la foto, la nota de voz y 6 documentos. Su destino no se confirma |
| **Asimilados** | Enemigos | Personal de Vitalis, guardias de Kessler y sujetos de prueba. Sin diálogo ni identidad |
| **El laboratorio** | Fuerza antagonista | La criatura lovecraftiana. Nunca se explica qué es |

No hay NPC con diálogo. Fuente: [[03 - Personajes]].

### The Game World

- **Ambientación:** distopía retrofuturista con estética de los 90 y tecnología de ~2080, gobernada por megacorporaciones. Instalación subterránea cruda e industrial.
- **Facciones:** Vitalis Konzern (biotecnología, indiferente y burocrática), Kessler Sicherheit AG / Werkschutz (seguridad privada cómplice), The Mother, el laboratorio, los asimilados.
- **Reglas del mundo:**
  - **The Mother reconstruye; el laboratorio asimila.** Nunca se invierten.
  - Solo se reconstruye a quien conserva identidad.
  - La generación procedural es el laboratorio reacomodándose.
  - El latido de fondo es el primer acto: inquieta, pero no asimila.
  - La corrupción de los documentos es un glitch del laboratorio y nunca oculta información necesaria para avanzar.
  - Los textos en alemán nunca se traducen.

Fuente: [[02 - Mundo]].

### Themes

- **Tema central (D-44, D-133):** buscar sentido dentro de algo que no puede darlo.
- **Postura:** nihilismo frente al horror cósmico (lo incognoscible, la indiferencia, la insignificancia). El anillo es el ancla del personaje, no del universo.
- **Temas secundarios:** pérdida de identidad y memoria; obstinación frente a esperanza; horror corporativo y burocrático; el cuerpo como algo ajeno.
- **Motivación del jugador:** el misterio y el descubrimiento, no la esperanza.

### Presentación narrativa

Sin narrador ni actuación de voz. Se cuenta con el entorno, los documentos (Emilia, Vitalis, Kessler, The Mother), el bestiario (informes clínicos), 12 objetos confiscados, las líneas cortas de The Mother y los murmullos de Lukas, todo con texto en pantalla. Frase clave compartida: **"Escucha el latido"**. Fuente: [[04 - Diálogos y Narrativa]], [[05 - Textos del Juego]].

---

## 4. The Look & Feel (Art & Aesthetics)

### Visual Style

- **Low-poly con luz moderna (D-81):** pocos polígonos, texturas de baja resolución con filtro *point* y render interno a ~540p reescalado con *dithering*. Iluminación dinámica por píxel (no PSX puro), porque la linterna y la oscuridad son mecánicas.
- **Sensación:** vulnerabilidad y frío. Instalación en **estado de emergencia**, casi a oscuras.
- **Formas:** industriales y rectas (instalación); orgánicas y rotas (asimilados); mecánicas y verticales (The Mother, nunca carne).

**Paleta:**

| Color | Significado |
|---|---|
| Base (grises cálidos con matiz ciruela) | Estructura, ~70% de la pantalla |
| Carmesí `#922235` | Vitalis, carne, peligro, The Mother |
| Teal | Lukas y su terminal |
| Morado | Lo cósmico y no humano |
| Verde fósforo | Pantallas de la instalación |
| Ámbar | Luz de sodio del piso 1 |
| Blanco cálido | Solo la linterna |

**Reglas clave:**

- El color nunca es la única señal.
- Un tinte de luz por sala.
- Los recogibles solo destacan bajo la luz.
- La silueta de Lukas siempre se separa del fondo.
- La degradación nunca tapa información.

Referencias en `10 - Referencia/`: 4 paletas, una imagen de ambientación y 2 diseños de Lukas.

Fuente: [[01 - Dirección de Arte]].

### Character Appearance

- **Lukas:** proporciones realistas, 1.78 m, ~5–8k triángulos, 3–5 variantes de cuerpo ajeno. Su postura cambia con la salud: erguido en Bien, cojea en Herido, arrastra el paso en Crítico.
- **Asimilados:** figuras grotescas con restos de uniformes y brillo morado iridiscente. Los reciclados añaden piezas metálicas: el intento fallido de The Mother de preservarlos.
- **The Mother:** "estantería-catedral" criogénica colgada de una grúa, con viales escarchados y luz carmesí interna.
- **Emilia:** sin modelo 3D; solo la foto familiar.
- **Animación:** Blender (más locomoción base de Mixamo, retocada), con IK, *Animation Rigging*, ruido procedural y ragdoll en Unity.

Fuente: [[02 - Personajes y Animación]].

### Environments

Kit modular por piso sobre una rejilla de 2 m; los 6 arquetipos comparten geometría y cada piso cambia texturas, props y luces. Interiores con vapor, goteras, chispas y niebla criogénica. Sin clima, fuego ni magia. Iluminación dinámica sin luz horneada (las salas son procedurales).

Fuente: [[03 - Entornos]].

### Menus & Interface (UI/UX)

- **Interfaz diegética (D-86), como en *Signalis*:** sin HUD permanente.
  - La salud se lee en la postura de Lukas y en el terminal (electrocardiograma + `HEALTH: FINE / INJURED / CRITICAL`).
  - La munición cargada se ve en el contador verde del revólver.
  - En pantalla solo aparecen la retícula, el aviso `E`, los subtítulos y "sincronizando…".
- **Sin mapa:** la vigilancia es la única ayuda espacial.
- **Tutorial:** carteles de seguridad de Vitalis en las paredes que muestran la tecla asignada (`IN CASE OF DARKNESS: PRESS [F]`).
- **Terminal del brazo (`Tab`):** pestañas Inventario, Estado y Registros; en tiempo real.
- **Menús:**
  - Calibración de brillo → pantalla de título → menú principal (un terminal en la sala de cápsulas) → 3 ranuras → Modo Supervivencia / Historia.
  - Pausa con Reiniciar (cuenta como muerte).
- **Tipografía:** VT323 (datos) + IBM Plex Sans (menús); OpenDyslexic como opción.
- **Accesibilidad:** brillo, subtítulos (≥ 32 px), escala de texto, contraste ≥ 4.5:1, fuente para dislexia, nunca solo color.

Fuente: [[04 - Interfaz Visual]], [[02 - UI y Menús]], [[03 - Accesibilidad]], [[04 - Guardado y Configuración]].

---

## 5. What It Sounds Like (Audio)

### Music

Cruda, oscura, industrial, melancólica y minimalista: frágil (piano, guitarra acústica) frente a mecánico (sintetizadores, bajos, estática, máquinas). Producida en FL Studio.

- **Leitmotivs:**
  - **Emilia:** piano y guitarra.
  - **The Mother:** industrial, en loops por fase.
  - **El latido:** subgraves que se cuelan en los otros dos.
- **Sin música en exploración:** solo ambiente, para no tapar las señales. Hay música en menús, cinemáticas, el jefe y los finales.

### Sound Effects

El audio es **información jugable** (M-10, S-11):

- Un sonido distinto por estado de alerta enemigo.
- Latido de Lukas (proximidad de amenazas) frente al latido del laboratorio (capa ambiental; crece con la degradación).
- Pasos por superficie; grito de la Boca; crujidos de reciclaje (~40 s después de la muerte).
- Revólver: disparo, recarga bala a bala, chasquido en vacío.
- Punzón, pitidos de hackeo, arranque de maquinaria, motor de las cámaras hostiles, cápsula de reconstrucción.

**Audio 3D** posicional con oclusión por puertas; se recomiendan auriculares.

**Prioridad de mezcla:** señales M-10 > Lukas y combate > ambiente > música, con *ducking*.

### Voices

**No hay actuación de voz ni narrador.** Emilia, The Mother y Lukas se representan con sonidos vocales + texto en pantalla; subtítulos obligatorios. La voz de Lukas se distorsiona con la degradación.

**Idiomas de texto:** inglés (principal) y español. El alemán no se traduce.

Fuente: [[05 - Audio y Música]].

---

## 6. Tech Stuff

### Engine & Tools

| Elemento     | Decisión                                                                                                                                                 |
| ------------ | -------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Motor        | **Unity 6.6** (`6000.6.6f1`, no LTS), URP *Forward+*                                                                                                     |
| Lenguaje     | C#; ShaderGraph/HLSL; JSON para guardado                                                                                                                 |
| Paquetes     | Solo oficiales: URP, Input System, Cinemachine 3, AI Navigation, Timeline, Animation Rigging, Localization, TextMeshPro, Test Framework. Sin Asset Store |
| Herramientas | Blender 4.x, Mixamo, Blockbench, FL Studio, Obsidian, Git + GitHub + Git LFS                                                                             |
| Ramas        | `main` (entregas), `develop` (integración), `feature/<sistema>-<tema>` con PR probado por otra persona                                                   |

**Requisitos (provisionales):**

- **Mínimo:** i5 6.ª gen / Ryzen 3 1200, GTX 950 (2 GB), 8 GB RAM, 720p a 30 FPS.
- **Recomendado:** i5 9.ª gen / Ryzen 5 2600, GTX 1060 (4 GB), 16 GB RAM, 1080p a 60 FPS.

**Objetivos técnicos:**

- Build ≤ 2 GB.
- Generación de piso + NavMesh ≤ 5 s.
- ≤ 6 enemigos con IA activa; ≤ 16 luces dinámicas y ≤ 4 con sombra.
- Solo la sala actual y sus adyacentes activas.

**Riesgos técnicos:**

- Unity 6.6 no es LTS.
- Una sola persona programa todo.
- La validez de la generación procedural.

Fuente: [[01 - Arquitectura Técnica]], [[02 - Motor y Herramientas]], [[03 - Requisitos Técnicos]], [[05 - Seguridad y Riesgos Técnicos]].

### Monetization

**Ninguna (D-139).** Demo académica gratuita: sin venta, compras dentro del juego ni anuncios. No se define un modelo comercial para una hipotética versión completa.

---

## 7. The Plan

### Rough Timeline

| Fase                        | Fechas             | Entregable                                                        |
| --------------------------- | ------------------ | ----------------------------------------------------------------- |
| Concepto                    | 2026-09-26 → 10-16 | GDD 0.1–0.10 ✅                                                    |
| Preproducción               | 10-17 → 10-22      | GDD 0.11–0.13, guías de `Desarrollo/` ✅                           |
| **H1** Base técnica         | 10-26              | Unity + núcleo + jugador + cámara OTS en *greybox*. Reutiliza partes del prototipo desechable `feature/heartbeatPrueba`; fechas sin cambio (D-140) |
| **H2** Prototipo de combate | 11-01              | Combate, inventario, interacción y el Alto con IA                 |
| **H3** MVP                  | 11-08              | Piso 1 procedural + ciclo de muerte; validador con 1 000 semillas |
| **H4** Vertical slice       | 11-11              | Piso 1 con calidad final; una persona ajena lo completa sin ayuda |
| **H5** Contenido completo   | 11-15              | 3 pisos, The Mother, finales, menús; congelación de funciones     |
| Pruebas                     | 11-16 → 11-19      | Solo corrección de errores, balance y perfilado                   |
| **H6** Entrega              | **2026-11-20**     | Build Win64 `v1.0-demo` en `main`                                 |

**Objetivo de entrega:** la configuración **showcase** (~30 min). La configuración completa solo si el balance llega a tiempo.

**Orden de recorte si falta tiempo:**

1. Fallas de la máquina.
2. Degradación y Asimilación.
3. Configuración completa.
4. Documentos de contexto (12 → 4).
5. Cinemáticas como imágenes fijas.
6. Modo Historia simplificado.
7. Español.
8. Introducción reducida.
9. IA de la Boca y el Mudo reutilizada.
10. Fase 2 de The Mother sin cámaras hostiles.

**Nunca se recorta:**

- Ciclo muerte → reconstrucción → piso nuevo.
- Generación válida.
- Vigilancia con costo.
- Inventario en tiempo real.
- The Mother.
- Los 6 documentos y la decisión final.
- Brillo y subtítulos.

Fuente: [[01 - Alcance]], [[03 - Roadmap]].

### Team Roles

| Integrante | Rol |
|---|---|
| Christian Coronel Palma | Líder, **toda la programación** (S-01..S-11), narrativa, gestión, GDD, backlog y recortes |
| Victor Gabriel Alvarado Chay | Diseño de niveles y narrativa (salas, tutorial, documentos, bestiario); **coordina la interfaz visual** |
| Bryan Iván Navarrete Canché | Arte 3D, animación, entornos, luz y render pixelado |
| Lian Daniel Sierra Vera | Audio y música |
| Lenin Ramón López Córdova | QA (plan de pruebas, playtests, bugs) y apoyo en SFX |
| Todos | QA de cada PR y semana de pruebas |

**Interfaz visual repartida (D-137):** Victor coordina y escribe los contenidos (terminal, carteles, menús); Bryan hace el aspecto CRT, los iconos y el post-proceso; Christian, la lógica.

Fuente: [[02 - Roles y Responsabilidades]].

---

## Decisiones de la revisión (0.13.1)

Preguntas abiertas resueltas el 2026-10-29:

| # | Tema | Resolución |
|---|---|---|
| 1 | Tagline | *Die. Rebuild. Forget.* (D-136) |
| 2 | Interfaz visual | Repartida; la coordina Victor (D-137) |
| 3 | Hitos | La rama `feature/heartbeatPrueba` es un prototipo desechable; se reutiliza código y las fechas no cambian (D-140) |
| 4 | Corte de la vigilancia | En **caza** (D-23); corregido en [[04 - Pilares de Diseño]] |
| 5 | Fase 3 del jefe | Los transformadores se exponen al abrirse el **núcleo criogénico** (D-65); corregido en M-19 |
| 6 | Mando | Mapa básico fijo en la demo; reasignación post-demo (D-120); corregido en [[03 - Público y Plataforma]] |
| 7 | Clasificación | PEGI 16 / ESRB M (D-138) |
| 8 | Monetización | Demo gratuita, sin modelo comercial (D-139) |
