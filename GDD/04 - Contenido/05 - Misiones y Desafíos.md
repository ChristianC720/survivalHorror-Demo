# Misiones y Desafíos

## 1. Descripción de la sección

Las misiones y desafíos convierten las mecánicas en objetivos concretos para el jugador.

## 2. Plantilla de misión

La demo tiene 5 misiones principales y 2 secundarias persistentes. No hay desafíos de tipo logro. Las cantidades de esta nota son valores iniciales sujetos a balance.

No hay lista de misiones ni marcadores. Las principales se comunican con el entorno, como la puerta cerrada de la sala final o un panel sin energía. También se comunican con líneas de protocolo de The Mother en el terminal del brazo, como `Restricted access: level key required.`. El terminal muestra el progreso de las secundarias en sus secciones, por ejemplo `Expediente 3/6` y `Objetos confiscados 5/12`.

### 2.1 Introducción: la nota de voz

**Tipo:** Principal.

**Objetivo:** Llegar al laboratorio siguiendo la nota de voz de Emilia.

**Contexto:** La nota de voz programada de Emilia llega al teléfono personal de Lukas.

**Requisitos:** `Nueva partida`.

**Pasos:**

1. Escuchar la nota de voz en la casa.
2. Hackear la compuerta con el teléfono.
3. Bajar en el elevador, en cinemática, y acercarse a la figura del piso que no existe.

**Condición de éxito:** Guionizada. Lukas muere y empieza la cinemática completa de reconstrucción.

**Condición de fracaso:** Ninguna.

**Recompensa:** Ninguna.

**Consecuencias:** Lukas olvida a Emilia y despierta como R-H-07.

**Personajes relacionados:** Lukas, Emilia por su nota de voz y The Mother.

**Nivel relacionado:** Introducción, en [[02 - Niveles]].

### 2.2 Piso 1: salir de mantenimiento

**Tipo:** Principal.

**Objetivo:** Llegar al piso 2.

**Contexto:** Lukas despierta en la fila de cápsulas buscando algo o a alguien, sin recordar a quién.

**Requisitos:** Una reconstrucción, al inicio del ciclo o tras morir en el piso 1.

**Pasos:**

1. Conseguir la llave obligatoria del piso.
2. Restaurar la energía en una sala clave secundaria si la sala final no tiene.
3. Abrir la sala clave final y usar la escalera o el elevador, con hackeo si lo exige.

**Condición de éxito:** La transición al piso 2.

**Condición de fracaso:** Morir o elegir `Reiniciar`, que devuelven al inicio del piso actual.

**Recompensa:** El autoguardado.

**Consecuencias:** Durante una caza, la escalera y el elevador quedan en cierre hasta que termina.

**Personajes relacionados:** The Mother.

**Nivel relacionado:** Piso 1, mantenimiento.

### 2.3 Piso 2: atravesar la clínica

**Tipo:** Principal.

**Objetivo:** Llegar al piso 3.

**Contexto:** La clínica ilegal donde Vitalis experimentaba con sujetos.

**Requisitos:** Completar el piso 1 en el intento.

**Pasos:**

1. Conseguir la llave obligatoria del piso.
2. Restaurar la energía si falta.
3. Abrir la sala clave final y usar la escalera o el elevador.

**Condición de éxito:** La transición al piso 3.

**Condición de fracaso:** Morir o elegir `Reiniciar`.

**Recompensa:** El autoguardado y el reinicio del contador de 5 muertes.

**Consecuencias:** La primera vez, el hallazgo de la foto imposible.

**Personajes relacionados:** The Mother y Emilia por la foto.

**Nivel relacionado:** Piso 2, clínica ilegal.

### 2.4 Piso 3: llegar al núcleo

**Tipo:** Principal.

**Objetivo:** Abrir la sala de The Mother.

**Contexto:** El núcleo de la instalación, desde donde se emitió el latido en el incidente.

**Requisitos:** Completar el piso 2 en el intento.

**Pasos:**

1. Conseguir la llave obligatoria del piso.
2. Abrir la puerta de la sala de The Mother.

**Condición de éxito:** Entrar a la sala de The Mother.

**Condición de fracaso:** Morir o elegir `Reiniciar`.

**Recompensa:** Ninguna, porque el piso 3 no tiene autoguardado.

**Consecuencias:** Empieza el encuentro con el jefe.

**Personajes relacionados:** The Mother.

**Nivel relacionado:** Piso 3, núcleo.

### 2.5 The Mother

**Tipo:** Principal.

**Objetivo:** Derrotar a The Mother.

**Contexto:** The Mother trata a Lukas como un espécimen no conforme.

**Requisitos:** Completar el piso 3 en el intento.

**Pasos:**

1. En la fase 1, hackear los 3 nodos mientras patrulla el brazo recolector.
2. En la fase 2, eliminar a los 4 asimilados bajo las cámaras hostiles y llegar al núcleo.
3. En la fase 3, destruir los 3 transformadores.

**Condición de éxito:** The Mother cae, llega el Despertar y se ve un final de victoria.

**Condición de fracaso:** Morir, que devuelve al inicio del piso 3.

**Recompensa:** Un final de victoria. Con los 6 documentos de Emilia, la elección entre Relevo completo y Ambiguo.

**Consecuencias:** La demo termina con el gancho abierto.

**Personajes relacionados:** The Mother y Emilia por sus documentos.

**Nivel relacionado:** Sala de The Mother.

### 2.6 Expediente de Emilia

**Tipo:** Secundaria, persistente entre intentos.

**Objetivo:** Reunir los 6 documentos de Emilia.

**Contexto:** Emilia investigó Vitalis desde dentro y fue reconstruida como R-H-01 a R-H-06.

**Requisitos:** Ninguno. Se acumula en cualquier intento.

**Pasos:**

1. Encontrar los 2 documentos de Emilia del piso 1.
2. Encontrar los 2 del piso 2.
3. Encontrar los 2 del piso 3.

**Condición de éxito:** 6 de 6 registrados en el terminal del brazo.

**Condición de fracaso:** Ninguna. Solo `Nueva partida` los borra.

**Recompensa:** La decisión final y la recuperación de los nombres.

**Consecuencias:** Sin los 6, la victoria siempre lleva al Relevo básico.

**Personajes relacionados:** Emilia.

**Nivel relacionado:** Pisos 1 a 3, en las salas clave secundarias.

### 2.7 Objetos confiscados

**Tipo:** Secundaria, persistente entre intentos.

**Objetivo:** Examinar los 12 objetos confiscados.

**Contexto:** Vitalis archiva todo lo que pertenece a sus sujetos y a su personal.

**Requisitos:** Ninguno.

**Pasos:**

1. Examinar los 4 objetos del piso 1.
2. Examinar los 4 del piso 2.
3. Examinar los 4 del piso 3.

**Condición de éxito:** 12 de 12 registrados en el terminal del brazo.

**Condición de fracaso:** Ninguna.

**Recompensa:** Solo narrativa. No cuenta para ningún final.

**Consecuencias:** Ninguna mecánica.

**Personajes relacionados:** Personal de Vitalis y de Kessler, y sujetos de los experimentos.

**Nivel relacionado:** Pisos 1 a 3, en las salas procedurales.

> **Navegación:** [[00 - Índice]] · ← [[04 - Objetos y Coleccionables]] · [[01 - Dirección de Arte]] →
