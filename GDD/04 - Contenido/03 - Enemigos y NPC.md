# Enemigos y NPC

## 1. Descripción de la sección

Documenta las entidades controladas por el juego y su comportamiento.

## 2. Plantilla de enemigo

Los enemigos comunes son personas asimiladas por el laboratorio, entre personal de Vitalis Konzern, guardias de Kessler Sicherheit AG y sujetos de prueba. Su aspecto y su lugar en el mundo están en [[03 - Personajes]] y [[02 - Mundo]]. Aquí se documenta su contenido jugable. Todos los valores son iniciales y sujetos a balance. Las reglas comunes y los valores de IA están en las secciones 4 y 5.

Como referencia, Lukas se mueve a ~0,8 m/s agachado, ~1,5 m/s caminando y ~4 m/s corriendo.

![Los 4 enemigos comunes lado a lado|520](../Media/Capturas/EnemigosComunes.png)

### 2.1 El Alto

**Rol de combate:** Centinela de pasillos.

**Comportamiento:** Patrulla pasillos largos. Ve lejos y oye poco, así que castiga cruzar su línea de visión con la linterna encendida.

**Vida:** 4 disparos.

**Daño:** Mata en 2 golpes.

**Defensas:** Ninguna.

**Velocidad:** 1,2 m/s en patrulla y 3 m/s en caza. Lukas corriendo lo deja atrás.

**Rango de detección:** Vista de ~15 m y oído de ~6 m.

**Ataques:** Golpe de alcance largo y agarre, según Punzón de descarga en [[02 - Mecánicas]].

**Debilidades:** Su lentitud. Su punto débil es la cabeza encorvada.

**Recompensa:** Ninguna.

**Estados de IA:**

```text
Patrullar → Sospecha → Búsqueda → Caza → Atacar / Agarrar
                          ↓
              Perder objetivo → Buscar → Patrullar
```

### 2.2 La Boca

**Rol de combate:** Alarma.

**Comportamiento:** Deambula despacio. Es ciega, así que la linterna no la afecta, pero oye mucho. Al detectar a Lukas grita y manda a los enemigos cercanos a buscar hacia ella.

**Vida:** 3 disparos.

**Daño:** Mata en 3 golpes.

**Defensas:** No reacciona a la luz.

**Velocidad:** 1 m/s en patrulla y 2,5 m/s en caza.

**Rango de detección:** No tiene vista. Su oído llega a ~20 m y oye cualquier ruido al doble de su radio normal, así que oye correr a ~20 m y caminar a ~6 m.

**Ataques:** Mordida frontal y grito de alerta. No agarra.

**Debilidades:** Su grito delata su posición. Su punto débil es la boca abierta, y acertarle durante el grito lo interrumpe.

**Recompensa:** Ninguna.

**Estados de IA:**

```text
Deambular → Oír ruido → Gritar (alerta a otros) → Caza → Atacar
                ↓
      Perder sonido → Buscar → Deambular
```

### 2.3 El Rastrero

**Rol de combate:** Flanqueador rápido y frágil.

**Comportamiento:** Va a cuatro patas y rodea para atacar por los flancos. Junto con el Mudo, es el único enemigo común que alcanza a Lukas corriendo.

**Vida:** 2 disparos.

**Daño:** Mata en 4 golpes.

**Defensas:** Ninguna.

**Velocidad:** 1,8 m/s en patrulla y 4,2 m/s en caza.

**Rango de detección:** Vista y oído de ~8 m.

**Ataques:** Zarpazos rápidos. No agarra.

**Debilidades:** Su fragilidad. Su punto débil es la cabeza con la mandíbula colgante.

**Recompensa:** Ninguna.

**Estados de IA:**

```text
Patrullar → Detectar → Flanquear → Atacar
               ↓
     Perder objetivo → Buscar
```

### 2.4 El Mudo

**Rol de combate:** Emboscada.

**Comportamiento:** No patrulla. Queda inmóvil y parece un cadáver o una persona quieta hasta activarse.

**Vida:** 3 disparos.

**Daño:** Mata en 2 golpes.

**Defensas:** Mientras está inmóvil no se distingue de un cuerpo.

**Velocidad:** Inmóvil. Al activarse avanza en estallidos de 4,5 m/s de ~0,9 s con pausas de ~0,7 s. Lukas corriendo no lo deja atrás.

**Rango de detección:** Se activa a ~3 m o al ser iluminado. Una vez activo ve hasta 10 m y oye hasta 4 m.

**Ataques:** Agarre de emboscada. Si el agarre no sale, golpea.

**Debilidades:** Queda inmóvil hasta activarse, así que se puede rodear a oscuras y a distancia. Su punto débil es el torso.

**Recompensa:** Ninguna.

**Estados de IA:**

```text
Inmóvil → (Lukas a ~3 m o luz) → Estallido → Agarrar → Pausa → Estallido
                                     ↓
                         Perder objetivo → Inmóvil
```

### 2.5 The Mother, jefe

**Rol de combate:** Jefe final de 3 fases.

**Comportamiento:** Archivo de muestras criogénicas en una grúa que trata a Lukas como un espécimen no conforme. En la fase 2 usa 3 cámaras hostiles.

**Vida:** 3 transformadores de 2 disparos cada uno.

**Daño:** El brazo recolector agarra con las reglas del punzón. En Crítico, el primer agarre sin punzón deja a Lukas al borde de la muerte y solo el siguiente lo mata.

**Defensas:** Solo es vulnerable en la fase 3, con el núcleo abierto.

**Velocidad:** Los valores de la grúa y del brazo están en Jefe final, en [[02 - Mecánicas]].

**Rango de detección:** Las cámaras de la sala en la fase 2.

**Ataques:** El brazo recolector en las 3 fases y los 4 asimilados liberados en la fase 2.

**Debilidades:** Los transformadores expuestos.

**Recompensa:** La victoria y un final.

**Estados de IA:**

```text
Fase 1: patrulla de nodos → Fase 2: cámaras + liberación → Fase 3: núcleo abierto → Caída
```

## 3. Plantilla de NPC

La demo no tiene NPC vivos ni personajes con diálogo. La soledad refuerza el pilar Vulnerabilidad Constante. Los cadáveres de guardias y personal ya están asimilados y son narrativa escénica.

### 3.1 The Mother, voz de protocolo

**Nombre:** The Mother.

**Función:** Antagonista que anuncia, registra y bloquea.

**Ubicación:** Altavoces y terminales de toda la instalación, más el terminal del brazo.

**Interacción:** Ninguna directa. Sus líneas aparecen como texto.

**Diálogos:** Líneas de protocolo en inglés con humor negro corporativo. Ver [[04 - Diálogos y Narrativa]].

**Condiciones:** Reacciona a eventos como la reconstrucción, la falta de llave o energía y las fases del jefe.

**Recompensas / información:** Registros en el terminal, como el bestiario, los efectos personales y los objetivos.

### 3.2 Emilia Reiner-Hellen

**Nombre:** Emilia Reiner-Hellen.

**Función:** Motivación del protagonista.

**Ubicación:** Solo en la nota de voz de la introducción y en sus 6 documentos.

**Interacción:** Leer sus documentos.

**Diálogos:** La transcripción de la nota de voz y el diario. Ver [[04 - Diálogos y Narrativa]].

**Condiciones:** Los 6 documentos desbloquean la decisión final.

**Recompensas / información:** La historia, y la recuperación de los nombres con los 6 documentos.

## 4. Reglas comunes

- **Estados de IA.** Patrulla, sospecha, búsqueda y caza, con un sonido distinto por estado.
- Ningún enemigo cambia de piso. Durante una caza, la escalera y el elevador quedan en cierre.
- **Recompensa.** Ninguna. Los enemigos no sueltan objetos, y saquear cadáveres queda para después de la demo.
- **Reciclaje.** Sigue Reciclaje de enemigos y remate, en [[02 - Mecánicas]].
- **Puntos débiles.** Cada tipo tiene uno. Acertarle interrumpe el ataque o el agarre en curso, pero no reduce los disparos necesarios.
- **Densidad.** Máximo 2 enemigos activos por sala y ~1 de cada 3 salas vacía. El total del piso nunca supera 2 por sala ocupada.
- **Aparición por piso.** El piso 1 tiene el Alto y el Rastrero, el piso 2 suma la Boca y el piso 3 suma el Mudo. El Mudo de la introducción es un señuelo guionizado.

## 5. Valores de IA

Estos valores ya los usa el juego. Viven en los `EnemyDefinition` (`ED_*`) y en `BossBalance`.

| Valor | Alto | Boca | Rastrero | Mudo |
| --- | --- | --- | --- | --- |
| Cono de visión | 100° | Ninguno, es ciega | 140° | 100° |
| Probabilidad de agarre en un ataque a ~1,6 m | 40% | No agarra | No agarra | 40% |
| Preparación del ataque | 0,7 s | 0,7 s | 0,35 s | 0,7 s |
| Descanso entre ataques | 1,8 s | 1,8 s | 1,1 s | 1,8 s |
| Alcance de la vista | 15 m | 0 m | 8 m | 10 m |
| Alcance del oído | 6 m | 20 m | 8 m | 4 m |
| Multiplicador del radio de ruido que oye | 1 | 2 | 1 | 1 |
| Reacciona a la linterna | Sí | No | Sí | Sí |
| Subida de alerta por segundo a la vista | 1,2 | 1,2 | 1,6 | 1,2 |
| Alcance del ataque | 2,4 m | 1,8 m | 1,5 m | 2,4 m |
| Alcance del agarre | 1,6 m | No agarra | No agarra | 1,6 m |
| Radio de patrulla | 6 m | 6 m | 7 m | No patrulla |
| Aturdimiento por acierto en el punto débil | 0,7 s | 0,7 s | 0,5 s | 0,7 s |
| Velocidad del reciclado | ×1,25 | ×1,25 | ×1,25 | ×1,25 |
| Disparos para eliminar al reciclado | 2 | 2 | 1 | 2 |

- **Alerta.** La caza dura 5 s sin ver a Lukas antes de bajar a búsqueda. La búsqueda se mantiene 6 s antes de decaer.
- La alerta baja 0,12 por segundo al decaer. En el límite de su alcance, la vista sube la alerta al 35% de su ritmo.
- **El Rastrero.** Flanquea abriéndose ~3 m a un lado de la ruta directa.
- **La Boca.** Su grito alcanza 18 m y tiene 8 s de recarga. Como es ciega, si ya está en búsqueda y oye un ruido a menos del 60% de su alcance de oído, unos 12 m, pasa directo a caza. Los gritos no cuentan para esto.
- **El Mudo.** Se activa a 3 m.
- En búsqueda, el enemigo se mueve a 1,4 veces su velocidad de patrulla.
- **Jefe.** Cada cámara hostil descansa 5 s tras liberar un asimilado.
- El brazo recolector deja de patrullar los nodos y persigue a Lukas cuando está a menos de ~6 m.
- **Sala de The Mother.** Se construye después del piso 3 como un paso propio, con su propia transición de 5 s o más desde la sala final del piso 3.

> **Navegación:** [[00 - Índice]] · ← [[02 - Niveles]] · [[04 - Objetos y Coleccionables]] →
