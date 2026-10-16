# Misiones y Desafíos

## Descripción de la sección

Las misiones y desafíos convierten las mecánicas en objetivos concretos para el jugador.

La demo tiene **5 misiones principales** y **2 secundarias persistentes**; no hay desafíos tipo logro (D-78).

**Cómo se comunican (D-78):** no hay lista de misiones ni marcadores. Las principales se comunican con el entorno (la puerta de la sala final cerrada, un panel "SIN ENERGÍA") y con líneas de protocolo de The Mother en el terminal del brazo, por ejemplo "Restricted access: level key required." El terminal muestra el progreso de las secundarias en sus secciones (por ejemplo, Expediente 3/6 y Objetos confiscados 5/12).

## Plantilla de misión

### MP-01 Introducción: la nota de voz

**Tipo:** Principal

**Objetivo:** Llegar al laboratorio siguiendo la nota de voz de Emilia.

**Contexto:** La nota de voz programada de Emilia llega al teléfono personal de Lukas (D-53).

**Requisitos:** Nueva partida.

**Pasos:**

1. Escuchar la nota de voz en la casa.
2. Hackear la compuerta con el teléfono (D-63).
3. Bajar en el elevador (cinemática) y acercarse a la figura en el piso que no existe.

**Condición de éxito:** Guionizada: Lukas muere y empieza la cinemática completa de reconstrucción.

**Condición de fracaso:** Ninguna.

**Recompensa:** Ninguna.

**Consecuencias:** Lukas olvida a Emilia y despierta como R-H-07 (D-45).

**Personajes relacionados:** Lukas, Emilia (nota de voz), The Mother.

**Nivel relacionado:** Introducción.

### MP-02 Piso 1: salir de mantenimiento

**Tipo:** Principal

**Objetivo:** Llegar al piso 2.

**Contexto:** Lukas despierta en la fila de cápsulas buscando algo o a alguien sin recordar a quién.

**Requisitos:** Reconstrucción (inicio del ciclo).

**Pasos:**

1. Conseguir la llave obligatoria del piso.
2. Restaurar la energía en una sala clave secundaria si la sala final no tiene (D-72).
3. Abrir la sala clave final y usar la escalera o el elevador (hackeo si lo exige).

**Condición de éxito:** Transición al piso 2.

**Condición de fracaso:** Muerte o "Reiniciar": regreso al inicio del ciclo (M-16).

**Recompensa:** Autoguardado (D-30).

**Consecuencias:** Por la escalera siguen los enemigos en caza (D-26).

**Personajes relacionados:** The Mother.

**Nivel relacionado:** Piso 1 — Mantenimiento.

### MP-03 Piso 2: atravesar la clínica

**Tipo:** Principal

**Objetivo:** Llegar al piso 3.

**Contexto:** La clínica ilegal donde Vitalis experimentaba con sujetos.

**Requisitos:** Completar MP-02 en el intento.

**Pasos:**

1. Conseguir la llave obligatoria del piso.
2. Restaurar la energía si falta (D-72).
3. Abrir la sala clave final y usar la escalera o el elevador.

**Condición de éxito:** Transición al piso 3.

**Condición de fracaso:** Muerte o "Reiniciar".

**Recompensa:** Autoguardado y reinicio del contador de 5 muertes (D-35).

**Consecuencias:** La primera vez, hallazgo de la foto imposible (D-56).

**Personajes relacionados:** The Mother, Emilia (foto).

**Nivel relacionado:** Piso 2 — Clínica ilegal.

### MP-04 Piso 3: llegar al núcleo

**Tipo:** Principal

**Objetivo:** Abrir la sala de The Mother.

**Contexto:** El núcleo de la instalación, desde donde se emitió el latido en el incidente.

**Requisitos:** Completar MP-03 en el intento.

**Pasos:**

1. Conseguir la llave obligatoria del piso.
2. Abrir la puerta de la sala de The Mother.

**Condición de éxito:** Entrar a la sala de The Mother.

**Condición de fracaso:** Muerte o "Reiniciar".

**Recompensa:** Ninguna (sin autoguardado, D-30).

**Consecuencias:** Empieza el encuentro con el jefe.

**Personajes relacionados:** The Mother.

**Nivel relacionado:** Piso 3 — Núcleo.

### MP-05 The Mother

**Tipo:** Principal

**Objetivo:** Derrotar a The Mother.

**Contexto:** The Mother trata a Lukas como un espécimen no conforme (D-65).

**Requisitos:** Completar MP-04 en el intento.

**Pasos:**

1. Fase 1: hackear los nodos mientras patrulla el brazo recolector.
2. Fase 2: sobrevivir a las cámaras hostiles y a los 4 asimilados (D-80).
3. Fase 3: destruir los 3 transformadores.

**Condición de éxito:** The Mother cae; Despertar y final de victoria (D-51).

**Condición de fracaso:** Muerte: regreso al inicio del ciclo.

**Recompensa:** Final de victoria; con los 6 documentos de Emilia, la elección entre Relevo completo y Ambiguo.

**Consecuencias:** Fin de la demo con el gancho abierto (M-19).

**Personajes relacionados:** The Mother, Emilia (documentos).

**Nivel relacionado:** Sala de The Mother.

### MS-01 Expediente de Emilia

**Tipo:** Secundaria (persistente entre intentos)

**Objetivo:** Reunir los 6 documentos de Emilia.

**Contexto:** Emilia investigó Vitalis desde dentro y fue reconstruida como R-H-01 a R-H-06 (D-57).

**Requisitos:** Ninguno; se acumula en cualquier intento.

**Pasos:**

1. Encontrar los 2 documentos de Emilia del piso 1.
2. Encontrar los 2 del piso 2.
3. Encontrar los 2 del piso 3.

**Condición de éxito:** 6/6 registrados en el terminal del brazo.

**Condición de fracaso:** Ninguna; solo "Nueva partida" los borra (D-32).

**Recompensa:** Decisión final y recuperación de los nombres (D-51, D-55).

**Consecuencias:** Sin los 6, la victoria siempre reproduce el Relevo básico.

**Personajes relacionados:** Emilia.

**Nivel relacionado:** Pisos 1 a 3 (salas clave secundarias).

### MS-02 Objetos confiscados

**Tipo:** Secundaria (persistente entre intentos)

**Objetivo:** Examinar los 12 objetos confiscados.

**Contexto:** Vitalis archiva todo lo que pertenece a sus sujetos y a su personal (D-73).

**Requisitos:** Ninguno.

**Pasos:**

1. Examinar los 4 objetos del piso 1.
2. Examinar los 4 del piso 2.
3. Examinar los 4 del piso 3.

**Condición de éxito:** 12/12 registrados en el terminal del brazo.

**Condición de fracaso:** Ninguna.

**Recompensa:** Solo narrativa; no cuenta para ningún final.

**Consecuencias:** Ninguna mecánica.

**Personajes relacionados:** Personal de Vitalis y de Kessler, sujetos de los experimentos.

**Nivel relacionado:** Pisos 1 a 3 (salas procedurales).


> **Navegación:** [[00 - Índice]] · ← [[04 - Objetos y Coleccionables]] · [[01 - Dirección de Arte]] →
