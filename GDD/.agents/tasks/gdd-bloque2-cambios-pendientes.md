# Cambios pendientes — Bloque 2 (GDD v0.3.0)

**Estado:** Aprobado por el equipo, sin aplicar.
**Base:** GDD v0.2.0 (aprobado por revisión, workflow `gdd-heartbeat-correccion-mecanicas`).
**Responsable:** Christian Coronel Palma.

## Alcance

- Completar las plantillas vacías de `02 - Diseño`: Sistemas, Progresión, Economía y Recursos, Reglas y Estados.
- Corregir la terminología de "clonación" por "reconstrucción".
- Agregar la mecánica M-20 Reciclaje de enemigos y remate.

**Fuera de alcance:** `Christian Decisions.md` (solo lectura) y las carpetas 03 a 09 y 11.

## Estándar del GDD

- Conservar en cada nota el título, "## Descripción de la sección", los encabezados de la plantilla y la línea final de navegación.
- Rellenar las plantillas; no reemplazarlas. Lo no decidido se marca "Por definir".
- Wikilinks `[[Nota]]`, tablas Markdown, negritas para etiquetas de campo, mermaid permitido.
- Cifras nuevas marcadas "(valor inicial, sujeto a balance)".
- Enlazar a [[02 - Mecánicas]] en lugar de repetir reglas.
- No inventar lore. La relación entre la esposa, la instalación y The Mother sigue "Por definir".

## Decisiones nuevas

| ID | Tema | Decisión | Pilar |
|---|---|---|---|
| D-17 | Recursos | Presupuesto garantizado por piso (mínimo y máximo de cada recurso). El generador decide la ubicación | V, principio 9 de Jugabilidad |
| D-18 | Muerte | El personaje se **reconstruye**, no se clona: siempre es el mismo hombre, recompuesto con piezas ajenas. Se pierde todo el inventario. Modifica D-12 | A |
| D-19 | Etiquetas | Se elimina "Permadeath". Se usa "Ciclo de muerte y reconstrucción" | A |
| D-20 | Enemigos | Los enemigos eliminados siguen muertos durante el intento, pero The Mother puede reciclar cada uno **una sola vez** | DC, V |
| D-21 | Remate | Dos formas de evitar el reciclaje: golpe en el suelo o consumible de remate | DC |
| D-22 | Guardado | Suspensión al salir y reanudación en el mismo punto, sin guardado manual. El terminal y las fallas se autoguardan al cambiar | A |

### Detalle de D-20 (reciclaje)

- **Aviso:** el cuerpo vibra y emite un sonido de maquinaria antes de desaparecer.
- **Reaparición:** cuando el jugador sale de la sala, en otra sala del mismo piso, nunca frente al jugador.
- **Enemigo reciclado:** más rápido, pero muere con menos golpes.

### Detalle de D-21 (remate)

| Opción | Costo | Tiempo | Ruido | Entrada |
|---|---|---|---|---|
| Golpe en el suelo | Resistencia | ~2 s (valor inicial, sujeto a balance) | Alerta a enemigos cercanos | `Clic izquierdo` sobre un cuerpo |
| Consumible de remate (p. ej. combustible para incinerar) | 1 casilla del inventario | Rápido | No alerta a enemigos | `E` sobre un cuerpo |

## Tareas

### Task 1: Terminología de reconstrucción

Ocurrencias detectadas en v0.2.0:

| Archivo | Ubicación | Texto actual | Cambio |
|---|---|---|---|
| `02 - Diseño/02 - Mecánicas.md` | M-16, Reglas | "reaparece mediante clonación (permadeath individual del clon)" | "se reconstruye en el punto de reaparición (ciclo de muerte y reconstrucción)" |
| `02 - Diseño/02 - Mecánicas.md` | M-16, Estado final | "Nuevo clon en el punto de reaparición…" | "Personaje reconstruido en el punto de reaparición…" |
| `02 - Diseño/02 - Mecánicas.md` | M-16, título | "Ciclo de muerte y terminal del brazo" | "Reaparición por reconstrucción y terminal del brazo" (también en catálogo y matriz) |
| `10 - Referencia/03 - Registro de Decisiones.md` | Fila D-12 | "Permadeath individual del clon" | Conservar como histórico y marcar "Modificada por D-18" |

- Registrar D-17 a D-22 en la tabla del Registro. Fichas completas para D-18, D-20 y D-21.
- Las menciones a "clones" en D-13 y en `Decisiones a Implementar.md` se refieren al lore descartado del documento viejo y se conservan.
- **Verificación:** "clon" y "Permadeath" solo aparecen en registros históricos, en D-13 y en `11 - Referencia de Diseño`.

### Task 2: M-20 Reciclaje de enemigos y remate

- Crear M-20 en `02 - Mecánicas.md` con la plantilla completa e **Impacto en pilares**.
  - Refuerza DC (tiempo y ruido frente a una casilla) y V.
  - Riesgo de desgaste sin fin: mitigado con reciclaje único.
  - Riesgo de saturar el inventario: mitigado con presupuesto bajo.
- Catálogo: M-20, Demo, V y DC.
- Matriz: relación con M-06 Resistencia, M-07 Inventario, M-09 Sigilo, M-10 Señales (aviso de maquinaria), M-14 Escape y M-19 Jefe.
- M-14: aclarar que la sala de la que se huyó sigue infestada, incluidos los reciclados.
- `04 - Controles.md`: agregar el remate como acción contextual sobre un cuerpo.
- **Verificación:** no contradice D-05, D-07 ni D-14; ninguna tecla tiene dos funciones en el mismo contexto.

### Task 3: `06 - Economía y Recursos.md`

- Tabla Recurso | Obtención | Uso | Límite | Pérdida:
  - Salud, Curas, Munición, Resistencia, Objeto defensivo, Consumible de remate, Objetos clave, Documentos y conocimiento, Fallas de la máquina, Baterías (Deseable), Fusibles o tarjetas (Deseable).
- Fuentes y sumideros.
- Reglas: presupuesto por piso (D-17) con valores iniciales para 2 pisos + sala del jefe, pérdida total al morir (D-18), 6 casillas, sin apilado infinito.
- Balance:
  - **Incentivar:** decidir con lo disponible, explorar con riesgo, rematar o huir.
  - **Evitar:** acumular, eliminar a todos los enemigos, desgaste por reciclaje, saturar el inventario.

### Task 4: `07 - Reglas y Estados del Juego.md`

- Estados: Menú principal, Carga y Generación, Jugando (Exploración, Vigilancia, Alerta/Combate, Hackeo), Pausa, Muerte, Reconstrucción, Victoria, Final de disolución, Créditos.
- Mapear los estados de la plantilla: Derrota → Muerte → Reconstrucción; Game Over → Final de disolución.
- Tabla de transiciones y diagrama `stateDiagram-v2`.
- Reglas globales: inventario y terminal en tiempo real; solo la pausa detiene el juego; guardado D-22; la sala clave reinicia el contador de muertes.
- Casos especiales: morir durante el hackeo, morir al registrar información, morir en la sala del jefe, salir durante una alerta, reciclaje durante la suspensión.

### Task 5: `03 - Sistemas.md`

- Tabla de inventario de sistemas y plantilla completa para cada uno:
  - Cámara y vigilancia
  - IA y percepción
  - Salud y combate
  - Resistencia
  - Inventario
  - Generación procedural por pisos (presupuesto D-17, cobertura mínima de cámaras, sala clave con escalera o elevador, ruta garantizada)
  - Persistencia (terminal, fallas, guardado D-22)
  - Ciclo de muerte y reconstrucción
  - Reciclaje de enemigos
  - Jefe The Mother
  - Audio dinámico (señales y latido)
- Diagrama mermaid de relaciones entre sistemas.

### Task 6: `05 - Progresión.md`

- Etapas: inicio/tutorial → piso 1 → sala clave → piso 2 → The Mother → final (revelación).
- Orden de aprendizaje: movimiento, interacción, inventario, linterna, sigilo y señales, combate y empujón, vigilancia, hackeo, muerte y reconstrucción, reciclaje y remate, fallas, jefe.
- El personaje no mejora (Pilar 1). Solo hay degradación estética (D-08).
- Desbloqueos: entradas del terminal, documentos de la esposa, fallas de la máquina (con parche).
- Curva de dificultad: reciclaje con aviso en el piso 1; piso 2 con más enemigos y menos presupuesto; jefe.

### Task 7: Integración y versión

- Enlaces: Jugabilidad ↔ Reglas y Estados, Economía ↔ Mecánicas, Sistemas ↔ Mecánicas.
- `03 - Público y Plataforma.md`: volver a marcar "Guardado compatible". "Interfaz legible" sigue desmarcada.
- Historial de versiones: fila 0.3.0. Ficha: versión 0.3.0 y fecha de actualización.
- Verificación global: "clon", "Permadeath", "cargas", "3/3", "QTE", "esquiv", "invulnerab", "cono" (solo linterna), "Presión Constante"; navegación intacta; wikilinks válidos; sin negritas abiertas.
