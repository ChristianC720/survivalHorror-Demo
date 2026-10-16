# Estructura del Contenido

## Descripción de la sección

Organiza el contenido del videojuego en unidades manejables: mundos, capítulos, zonas, niveles, misiones y desafíos.

## Estructura

La demo no tiene mundos ni capítulos: cada **piso es un nivel**, y la introducción y la sala de The Mother son niveles propios (D-69). Cada intento recorre los mismos niveles en el mismo orden con una distribución distinta de salas y recursos (M-15, M-16). Los niveles se documentan en [[02 - Niveles]].

```text
Identity Dissolution (demo)
├── Introducción (casa, compuerta, elevador, piso que no existe) — una sola vez
│   └── Cinemática completa de reconstrucción (el olvido)
├── Piso 1 — Mantenimiento
│   ├── Tutorial: 4 salas fijas (solo en el primer intento)
│   ├── Salas procedurales (6 arquetipos con tema de mantenimiento)
│   ├── Salas clave secundarias (2 documentos de Emilia, interruptor de energía si falta)
│   └── Sala clave final (autoguardado)
├── Piso 2 — Clínica ilegal
│   ├── Salas procedurales (tema clínico)
│   ├── Salas clave secundarias (2 documentos de Emilia)
│   └── Sala clave final (autoguardado + reinicio del contador + hallazgo de la foto)
├── Piso 3 — Núcleo
│   ├── Salas procedurales (tema de núcleo)
│   ├── Salas clave secundarias (2 documentos de Emilia)
│   └── Sala de The Mother (jefe, 3 fases)
└── Finales
    ├── Victoria: Relevo básico / Relevo completo / Ambiguo
    └── Derrota: Asimilación (5 muertes consecutivas)
```

## Cantidad estimada

Valores de la configuración completa; el showcase escala salas, enemigos y recursos (D-34).

| Tipo                            |                                                                         Cantidad |
| ------------------------------- | -------------------------------------------------------------------------------: |
| Mundos                          |                                                                   1 (Sektor V-7) |
| Niveles                         |                    5 (Introducción, Piso 1, Piso 2, Piso 3 y Sala de The Mother) |
| Arquetipos de sala procedural   |                                                            6, con 3 temas (D-69) |
| Salas por piso                  |                                             ~10–15 (completa) / ~9–11 (showcase) |
| Misiones principales            |                                                                         5 (D-78) |
| Misiones secundarias            |                                                                         2 (D-78) |
| Enemigos (tipos)                |                                4: el Alto, la Boca, el Rastrero y el Mudo (D-60) |
| Enemigos por intento            |                                27–36 + 4 en la fase 2 de The Mother (D-37, D-80) |
| Jefes                           |                                                             1 (The Mother, D-65) |
| NPC                             |                                                                   0 vivos (D-77) |
| Armas                           |                                                    1 (revólver de Kessler, D-74) |
| Objetos de inventario (tipos)   | 5: cura, munición, punzón de descarga, llave obligatoria y llave opcional (D-71) |
| Documentos de Emilia            |                                                                         6 (D-52) |
| Otros documentos                |                                                                        12 (D-79) |
| Objetos confiscados             |                                                                        12 (D-73) |
| Entradas de bestiario           |                                                   5 (4 tipos + The Mother, D-48) |
| Fallas de la máquina (Deseable) |                                                                       1–2 (M-17) |
| Finales                         |                        4 (Relevo básico, Relevo completo, Ambiguo y Asimilación) |

## Dependencias

¿Qué contenido debe existir antes de otro?

| Contenido | Depende de | Motivo |
|---|---|---|
| Salas procedurales de los 3 pisos | Los 6 arquetipos y los 3 temas (D-69) | El generador combina arquetipo y tema |
| Tutorial | Arquetipos almacén, pasillo y sala abierta; el Rastrero; el revólver | Las 4 salas fijas reutilizan arquetipos (D-70) |
| Piso 1 | Alto, Rastrero, revólver, punzón de descarga, llaves e interruptor de energía | Son los primeros elementos que encuentra el jugador (D-76) |
| Piso 2 | La Boca y la cinemática del hallazgo de la foto | Tipo nuevo del piso (D-76) y reinicio del contador (D-56) |
| Piso 3 | El Mudo | Tipo nuevo del piso (D-76) |
| Sala de The Mother | Hackeo (M-13), cámaras (M-02), agarre (M-11), revólver (M-04), Alto y Rastrero | Las 3 fases reutilizan mecánicas existentes (M-19, D-80) |
| Decisión final y Relevo completo | Los 6 documentos de Emilia | Solo con los 6 hay elección (D-51) |
| Final Asimilación | Contador de 5 muertes y piso que no existe | Reutiliza el piso de la introducción (D-61) |
| Documentos, objetos confiscados y bestiario | Textos en [[04 - Diálogos y Narrativa]] | Se registran en el terminal del brazo (M-16) |


> **Navegación:** [[00 - Índice]] · ← [[04 - Diálogos y Narrativa]] · [[02 - Niveles]] →
