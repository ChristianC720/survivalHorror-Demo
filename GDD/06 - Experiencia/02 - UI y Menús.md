# UI y Menús

## Descripción de la sección

Define la estructura funcional de las interfaces y menús.

Estilo visual en [[04 - Interfaz Visual]]; efectos de cada opción en [[07 - Reglas y Estados del Juego]]; opciones de configuración en [[04 - Guardado y Configuración]].

## Mapa de navegación

```text
Calibración de brillo (solo el primer inicio)
Pantalla de título
└── Menú principal (terminal de la sala de cápsulas)
    ├── Continuar (última ranura usada; en gris sin partida)
    ├── Nueva partida → Elegir ranura → Elegir modo → Introducción
    ├── Cargar partida → Elegir ranura
    ├── Opciones
    │   ├── Audio
    │   ├── Video
    │   ├── Controles
    │   ├── Accesibilidad
    │   └── Juego
    ├── Créditos
    └── Salir

Pausa (Esc)
├── Volver al juego
├── Opciones (mismas secciones)
├── Reiniciar (no en la introducción ni en la decisión final)
└── Salir (con el costo de D-31)

Terminal del brazo (Tab)
├── Inventario
├── Estado (salud y linterna)
└── Registros (documentos, expediente de Emilia, bestiario, objetos confiscados, fallas, anillo)
```

## Pantallas

### Calibración de brillo

**Propósito:** Ajustar el brillo para un juego casi a oscuras (D-106).

**Elementos:** 3 símbolos sobre negro (uno debe verse apenas, otro no); barra de brillo.

**Acciones disponibles:** Ajustar, Aceptar.

**Estados:** Solo aparece en el primer inicio del juego; después está en Opciones > Video.

**Transiciones:** → Pantalla de título.

### Pantalla de título

**Propósito:** Entrada al juego (D-90).

**Elementos:** Fondo negro, logotipo y el latido del laboratorio.

**Acciones disponibles:** Cualquier tecla o botón.

**Estados:** Único.

**Transiciones:** → Menú principal.

### Menú principal

**Propósito:** Elegir partida y opciones (D-39, D-90).

**Elementos:** Terminal verde de la instalación en la sala de la fila de cápsulas, junto a la cápsula de Lukas; opciones del mapa de navegación.

**Acciones disponibles:** Continuar, Nueva partida, Cargar partida, Opciones, Créditos, Salir.

**Estados:** Continuar en gris si no hay ninguna partida (D-96). Tras un final, Continuar inicia un ciclo nuevo (D-93).

**Transiciones:** → Carga (arranque del terminal) → Juego; → Introducción; → Opciones; → Créditos; → cierre del juego.

### Selección de ranura

**Propósito:** Elegir una de las 3 ranuras (D-105).

**Elementos:** Por ranura: código de sujeto actual (R-H-xx), modo, piso del autoguardado, documentos de Emilia (x/6) y fecha. Ranura vacía: `EMPTY`.

**Acciones disponibles:** Elegir; borrar (con confirmación).

**Estados:** Desde Nueva partida, una ranura ocupada pide confirmar que se sobrescribirá. Desde Cargar partida, las vacías no se pueden elegir.

**Transiciones:** Nueva partida → Selección de modo; Cargar partida → Carga.

### Selección de modo

**Propósito:** Elegir el modo de la partida (D-103).

**Elementos:** Modo Supervivencia y Modo Historia, cada uno con una línea que lo describe.

**Acciones disponibles:** Elegir, Volver.

**Estados:** El modo queda fijo en la ranura.

**Transiciones:** → Introducción.

### Pausa

**Propósito:** Detener el recorrido (regla global 2).

**Elementos:** Menú sobre la imagen oscurecida.

**Acciones disponibles:** Volver al juego, Opciones, Reiniciar, Salir.

**Estados:** Sin "Reiniciar" en la introducción (D-63) y en la decisión final (D-92).

**Transiciones:** → Juego; → Muerte (Reiniciar); → Menú principal (Salir).

### Terminal del brazo

**Propósito:** Gestionar recursos y consultar estados y registros sin pausar (M-07, M-16, D-86).

**Elementos:** Pestañas Inventario, Estado y Registros; contadores de las secundarias.

**Acciones disponibles:** Usar o descartar objetos; leer registros; cambiar de pestaña.

**Estados:** Abierto o cerrado; el juego sigue en tiempo real.

**Transiciones:** `Tab` abre y cierra.

### Créditos

**Propósito:** Cierre de cada final y consulta desde el menú.

**Elementos:** Créditos del equipo.

**Acciones disponibles:** Saltar (`Espacio`) solo si ya se vieron (D-39).

**Estados:** —

**Transiciones:** → Menú principal.

## Confirmaciones

| Acción | Confirmación | Texto |
|---|---|---|
| Nueva partida en una ranura ocupada | Sí | Avisa que se borra esa ranura, incluido el terminal (D-32) |
| Borrar ranura | Sí | Avisa que no se puede deshacer |
| Reiniciar | Sí | Avisa que cuenta como muerte (D-32, D-96) |
| Salir durante el recorrido | Sí | Avisa del costo según la situación (D-31) |
| Salir desde el menú principal | Sí (D-96) | Confirma el cierre |
| Restablecer opciones | Sí | Indica qué sección vuelve a sus valores predeterminados |

Las confirmaciones usan `Cancelar` como opción por defecto.


> **Navegación:** [[00 - Índice]] · ← [[01 - UX y Flujo del Jugador]] · [[03 - Accesibilidad]] →
