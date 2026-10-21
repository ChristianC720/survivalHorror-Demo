# Bugs y Seguimiento

## Descripción de la sección

Registra problemas encontrados durante las pruebas y permite darles seguimiento.

Los bugs se registran como **GitHub Issues** con la etiqueta `bug` y una etiqueta de severidad (`sev:critica`, `sev:alta`, `sev:media`, `sev:baja`) (D-113). El ID es el número de la *issue* (`#123`). La tabla de esta nota solo resume los críticos y altos abiertos en cada hito.

**Severidad:**

- **Crítica:** cierre del juego, pérdida o corrupción del guardado, piso imposible, progreso bloqueado.
- **Alta:** una regla del GDD no se cumple de forma visible (por ejemplo, la vigilancia no se corta en caza) o el rendimiento cae por debajo del mínimo.
- **Media:** error con rodeo fácil, animación o sonido incorrecto.
- **Baja:** detalle visual o de texto.

**Regla de hitos:** no se cierra un hito con bugs críticos abiertos; la entrega (H6) no tiene críticos ni altos abiertos ([[03 - Roadmap]]).

## Reporte de bug

**Título:** [Sistema S-xx] Qué falla, en una frase.

**Versión:** commit o build (`git rev-parse --short HEAD`).

**Nivel / escena:** piso, sala o escena, y semilla del piso (visible en la consola de depuración).

**Pasos para reproducir:**

1.
2.
3.

**Resultado esperado:** (cita la regla del GDD: M-xx, S-xx o D-xx)

**Resultado actual:**

**Frecuencia:** Siempre / A veces / Rara vez

**Severidad:** Crítica / Alta / Media / Baja

**Evidencia:** captura, video o `Player.log` (`%USERPROFILE%\AppData\LocalLow\<Compañía>\Identity Dissolution\Player.log`).

**Estado:** Abierto / En progreso / Resuelto / Cerrado

## Registro

| ID | Bug | Severidad | Responsable | Estado |
|---|---|---|---|---|
| — | Sin bugs registrados todavía | — | — | — |


> **Navegación:** [[00 - Índice]] · ← [[02 - Pruebas]] · [[04 - Criterios de Aceptación]] →
