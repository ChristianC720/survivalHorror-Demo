# UI y Menús

## 1. Descripción de la sección

Define la estructura funcional de las interfaces y menús.

El estilo visual está en [[04 - Interfaz Visual]] y el efecto de cada opción en [[07 - Reglas y Estados del Juego]]. Las opciones de configuración están en [[04 - Guardado y Configuración]]. Los valores numéricos de este documento son iniciales y están sujetos a balance.

## 2. Mapa de navegación

```text
Calibración de brillo (solo el primer inicio)
Pantalla de título
└── Menú principal (terminal de la sala de cápsulas)
    ├── Continuar (última ranura usada, en gris sin partida)
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
└── Salir (con el costo de salir del juego)

Terminal del brazo (Tab)
├── Inventario
├── Estado (salud y linterna)
└── Registros (documentos, expediente de Emilia, bestiario, objetos confiscados, fallas y anillo)
```

## 3. Pantallas

### 3.1 Calibración de brillo

**Propósito:** Ajustar el brillo para un juego casi a oscuras.

**Elementos:** 3 símbolos sobre negro, uno que apenas debe verse y otro que no. Una barra de brillo.

**Acciones disponibles:** Ajustar y Aceptar.

**Estados:** Solo aparece en el primer inicio del juego. Después está en `Opciones > Video`.

**Transiciones:** Lleva a la pantalla de título.

![Calibración de brillo con los 3 símbolos sobre negro|520](../Media/Capturas/CalibracionBrillo.png)

### 3.2 Pantalla de título

**Propósito:** Entrada al juego.

**Elementos:** Fondo negro, logotipo y el latido del laboratorio.

**Acciones disponibles:** Cualquier tecla o botón.

**Estados:** Tiene un único estado.

**Transiciones:** Lleva al menú principal.

### 3.3 Menú principal

**Propósito:** Elegir partida y opciones.

**Elementos:** Un terminal verde de la instalación en la sala de la fila de cápsulas, junto a la cápsula de Lukas, con las opciones del mapa de navegación.

**Acciones disponibles:** `Continuar`, `Nueva partida`, `Cargar partida`, `Opciones`, `Créditos` y `Salir`.

**Estados:** `Continuar` aparece en gris si no hay ninguna partida. Tras un final, `Continuar` inicia un ciclo nuevo.

**Transiciones:** Lleva a la carga con el arranque del terminal y luego al juego. También a la introducción, a Opciones, a los créditos o al cierre del juego.

![Menú principal en el terminal verde junto a la cápsula de Lukas|520](../Media/Capturas/MenuPrincipalTerminal.png)

### 3.4 Selección de ranura

**Propósito:** Elegir una de las 3 ranuras.

**Elementos:** Cada ranura muestra el código de sujeto actual, `R-H-xx`, el modo, el piso del autoguardado, los documentos de Emilia en formato x/6 y la fecha. Una ranura vacía muestra `EMPTY`.

**Acciones disponibles:** Elegir y borrar, con confirmación.

**Estados:** Desde `Nueva partida`, una ranura ocupada pide confirmar que se sobrescribirá. Desde `Cargar partida`, las ranuras vacías no se pueden elegir.

**Transiciones:** `Nueva partida` lleva a la selección de modo y `Cargar partida` a la carga.

![Selección de ranura con una ranura ocupada y dos vacías|520](../Media/Capturas/SeleccionRanura.png)

### 3.5 Selección de modo

**Propósito:** Elegir el modo de la partida.

**Elementos:** Modo Supervivencia y Modo Historia, cada uno con una línea que lo describe.

**Acciones disponibles:** Elegir y Volver.

**Estados:** El modo queda fijo en la ranura.

**Transiciones:** Lleva a la introducción.

### 3.6 Pausa

**Propósito:** Detener el recorrido, según la regla global 2 de [[07 - Reglas y Estados del Juego]].

**Elementos:** Menú sobre la imagen oscurecida.

**Acciones disponibles:** Volver al juego, `Opciones`, `Reiniciar` y `Salir`.

**Estados:** No ofrece `Reiniciar` en la introducción ni en la decisión final.

**Transiciones:** Vuelve al juego, lleva a Muerte con `Reiniciar` o al menú principal con `Salir`.

### 3.7 Terminal del brazo

**Propósito:** Gestionar recursos y consultar estados y registros sin pausar. Abrir una entrada lleva a la vista de lectura, que sí detiene el juego.

**Elementos:** Pestañas Inventario, Estado y Registros, con los contadores de las secundarias. Una tira con pulso, salud, luz y tambor queda visible en todas las pestañas.

**Acciones disponibles:** Usar o descartar objetos, leer registros y cambiar de pestaña.

**Estados:** Abierto o cerrado. El juego sigue en tiempo real.

**Transiciones:** `Tab` lo abre y lo cierra.

### 3.8 Créditos

**Propósito:** Cerrar cada final y permitir la consulta desde el menú.

**Elementos:** Créditos del equipo.

**Acciones disponibles:** Saltar con `Espacio`, solo si ya se vieron.

**Estados:** Tiene un único estado.

**Transiciones:** Llevan al menú principal.

## 4. Confirmaciones

| Acción | Confirmación | Texto |
|---|---|---|
| `Nueva partida` en una ranura ocupada | Sí | Avisa que se borra esa ranura, incluido el terminal |
| Borrar ranura | Sí | Avisa que no se puede deshacer |
| `Reiniciar` | Sí | Avisa que cuenta como muerte |
| `Salir` durante el recorrido | Sí | Avisa del costo según la situación |
| `Salir` desde el menú principal | Sí | Confirma el cierre |
| Restablecer opciones | Sí | Indica qué sección vuelve a sus valores predeterminados |

Las confirmaciones usan `Cancelar` como opción por defecto. El costo de cada salida está en la regla global 4 de [[07 - Reglas y Estados del Juego]].

> **Navegación:** [[00 - Índice]] · ← [[01 - UX y Flujo del Jugador]] · [[03 - Accesibilidad]] →
