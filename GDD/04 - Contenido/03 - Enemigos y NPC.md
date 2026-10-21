# Enemigos y NPC

## Descripción de la sección

Documenta las entidades controladas por el juego y su comportamiento.

Los enemigos comunes son personas asimiladas por el laboratorio: personal de Vitalis Konzern, guardias de Kessler Sicherheit AG y sujetos de prueba (D-48, D-60, D-75). Su aspecto y su lugar en el mundo están en [[03 - Personajes]] y [[02 - Mundo]]; aquí se documenta su contenido jugable. Todos los valores son iniciales, sujetos a balance.

**Referencia de velocidad de Lukas (M-06):** agachado ~0.8 m/s, caminando ~1.5 m/s, corriendo ~4 m/s.

## Reglas comunes

- **Estados de IA:** patrulla, sospecha, búsqueda y caza, con un sonido distinto por estado (M-10, D-23). Ningún enemigo cambia de piso: durante una caza, la escalera y el elevador quedan en cierre (D-96).
- **Recompensa:** ninguna. Los enemigos no sueltan objetos; el saqueo de cadáveres es Post-demo (M-20).
- **Reciclaje:** el laboratorio recicla cada cuerpo no rematado una sola vez, con un aviso orgánico ~40 s después de la muerte (D-99); el reciclado es un 25% más rápido y muere con 2 disparos, o con 1 si es un Rastrero (M-20, D-66).
- **Puntos débiles:** cada tipo tiene uno. Acertarle interrumpe el ataque o el agarre en curso, pero no reduce los disparos necesarios (D-74).
- **Densidad:** máximo 2 enemigos activos por sala; ~1/3 de las salas vacías (D-37). El total del piso nunca supera 2 por sala ocupada (D-118).
- **Aparición por piso (D-76):** piso 1, el Alto y el Rastrero; piso 2, se suma la Boca; piso 3, se suma el Mudo. El Mudo de la introducción es un señuelo guionizado (D-53).

## Plantilla de enemigo

### El Alto

**Rol de combate:** Centinela de pasillos.

**Comportamiento:** Patrulla pasillos largos. Ve lejos y oye poco: castiga cruzar su línea de visión con la linterna encendida.

**Vida:** 4 disparos (D-66).

**Daño:** Mata en 2 golpes (D-66).

**Defensas:** Ninguna.

**Velocidad:** 1.2 m/s en patrulla; 3 m/s en caza. Lukas corriendo lo deja atrás.

**Rango de detección:** Vista ~15 m; oído ~6 m.

**Ataques:** Golpe de alcance largo; agarre (M-11).

**Debilidades:** Lentitud. Punto débil: la cabeza encorvada.

**Recompensa:** Ninguna.

**Estados de IA:**

```text
Patrullar → Sospecha → Búsqueda → Caza → Atacar / Agarrar
                ↓
          Perder objetivo → Buscar → Patrullar
```

### La Boca

**Rol de combate:** Alarma.

**Comportamiento:** Deambula despacio. Es ciega: la linterna no la afecta, pero oye mucho. Al detectar a Lukas grita y pone a los enemigos cercanos en búsqueda hacia ella (M-09, D-64).

**Vida:** 3 disparos (D-66).

**Daño:** Mata en 3 golpes (D-66).

**Defensas:** No reacciona a la luz.

**Velocidad:** 1 m/s en patrulla; 2.5 m/s en caza.

**Rango de detección:** Sin vista; oído ~20 m.

**Ataques:** Mordida frontal; grito de alerta. No agarra.

**Debilidades:** Su grito delata su posición. Punto débil: la boca abierta; acertarle durante el grito lo interrumpe.

**Recompensa:** Ninguna.

**Estados de IA:**

```text
Deambular → Oír ruido → Gritar (alerta a otros) → Caza → Atacar
                                ↓
                       Perder sonido → Buscar → Deambular
```

### El Rastrero

**Rol de combate:** Flanqueador rápido y frágil.

**Comportamiento:** Se mueve a cuatro patas y rodea para atacar por los flancos. Es el único enemigo común, además del Mudo, que alcanza a Lukas corriendo.

**Vida:** 2 disparos (D-66).

**Daño:** Mata en 4 golpes (D-66).

**Defensas:** Ninguna.

**Velocidad:** 1.8 m/s en patrulla; 4.2 m/s en caza.

**Rango de detección:** Vista y oído ~8 m.

**Ataques:** Zarpazos rápidos. No agarra.

**Debilidades:** Fragilidad. Punto débil: la cara invertida.

**Recompensa:** Ninguna.

**Estados de IA:**

```text
Patrullar → Detectar → Flanquear → Atacar
                ↓
          Perder objetivo → Buscar
```

### El Mudo

**Rol de combate:** Emboscada.

**Comportamiento:** No patrulla. Permanece inmóvil y parece un cadáver o una persona quieta hasta activarse (M-09, D-66).

**Vida:** 3 disparos (D-66).

**Daño:** Mata en 2 golpes (D-66).

**Defensas:** Indistinguible de un cuerpo mientras está inmóvil.

**Velocidad:** Inmóvil; al activarse, estallidos de 4.5 m/s de ~2 s con pausas. Lukas corriendo no lo deja atrás.

**Rango de detección:** Se activa a ~3 m o al ser iluminado.

**Ataques:** Agarre de emboscada (M-11).

**Debilidades:** Inmóvil hasta activarse: se puede rodear a oscuras y a distancia. Punto débil: el torso.

**Recompensa:** Ninguna.

**Estados de IA:**

```text
Inmóvil → (Lukas a ~3 m o luz) → Estallido → Agarrar → Pausa → Estallido
                                                ↓
                                     Perder objetivo → Inmóvil
```

### The Mother (jefe)

**Rol de combate:** Jefe final de 3 fases (M-19).

**Comportamiento:** Archivo de muestras criogénicas en una grúa; trata a Lukas como un espécimen no conforme que debe recolectar (D-65). 3 cámaras hostiles en la fase 2 (D-98).

**Vida:** 3 transformadores de 2 disparos cada uno (D-80).

**Daño:** El agarre del brazo recolector sigue las reglas de M-11.

**Defensas:** Solo es vulnerable en la fase 3, con el núcleo abierto.

**Velocidad:** La grúa recorre sus rieles a ~2.5 m/s; el brazo recolector alcanza ~3 m y avisa ~1 s antes de agarrar (D-98).

**Rango de detección:** Las cámaras de la sala (fase 2, D-28).

**Ataques:** Brazo recolector (las 3 fases); 4 asimilados liberados en la fase 2 (2 Altos y 2 Rastreros), máximo 2 activos (D-80, D-115).

**Debilidades:** Los transformadores expuestos.

**Recompensa:** Victoria y final (D-51).

**Estados de IA:**

```text
Fase 1: patrulla de nodos → Fase 2: cámaras + liberación → Fase 3: núcleo abierto → Caída
```

## Plantilla de NPC

La demo no tiene NPC vivos ni personajes con diálogo (D-77). La soledad refuerza la Vulnerabilidad Constante. Los cadáveres de guardias y personal ya están asimilados y son narrativa escénica.

**Nombre:** The Mother (voz de protocolo)

**Función:** Antagonista; anuncia, registra y bloquea.

**Ubicación:** Altavoces y terminales de toda la instalación; terminal del brazo.

**Interacción:** Ninguna directa. Sus líneas aparecen como texto (D-62).

**Diálogos:** Líneas de protocolo en inglés con humor negro corporativo (ver [[04 - Diálogos y Narrativa]]).

**Condiciones:** Reacciona a eventos: reconstrucción, falta de llave o energía, fases del jefe.

**Recompensas / información:** Registros en el terminal: bestiario, efectos personales y objetivos (D-78).

---

**Nombre:** Emilia Reiner-Hellen

**Función:** Motivación del protagonista.

**Ubicación:** Solo en la nota de voz de la introducción y en sus 6 documentos (D-52, D-53).

**Interacción:** Leer sus documentos (M-08).

**Diálogos:** Transcripción de la nota de voz y diario (ver [[04 - Diálogos y Narrativa]]).

**Condiciones:** Los 6 documentos desbloquean la decisión final (D-51).

**Recompensas / información:** Historia; recuperación de los nombres con los 6 documentos (D-55).


> **Navegación:** [[00 - Índice]] · ← [[02 - Niveles]] · [[04 - Objetos y Coleccionables]] →
