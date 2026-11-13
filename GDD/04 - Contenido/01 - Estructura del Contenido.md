# Estructura del Contenido

## 1. Descripción de la sección

Organiza el contenido del videojuego en unidades manejables: mundos, capítulos, zonas, niveles, misiones y desafíos.

## 2. Estructura

La demo no tiene mundos ni capítulos. Cada piso es un nivel, y la introducción y la sala de The Mother son niveles propios. Cada intento recorre los mismos niveles en el mismo orden, con otra distribución de salas y recursos. Los niveles están en [[02 - Niveles]].

```text
Identity Dissolution (demo)
├── Introducción: casa, compuerta, elevador y piso que no existe, una sola vez
│   └── Cinemática completa de reconstrucción, el olvido
├── Piso 1, mantenimiento
│   ├── Fila de cápsulas: sala clave de inicio y primera sala del tutorial
│   ├── Tutorial: 3 salas procedurales fijas, solo en el primer intento
│   ├── Salas procedurales: 6 arquetipos con tema de mantenimiento
│   ├── Salas clave secundarias: 2 documentos de Emilia e interruptor de energía si falta
│   └── Sala clave final: autoguardado
├── Piso 2, clínica ilegal
│   ├── Salas procedurales: tema clínico
│   ├── Salas clave secundarias: 2 documentos de Emilia
│   └── Sala clave final: autoguardado, reinicio del contador y hallazgo de la foto
├── Piso 3, núcleo
│   ├── Salas procedurales: tema de núcleo
│   ├── Salas clave secundarias: 2 documentos de Emilia
│   └── Sala de The Mother: jefe de 3 fases
└── Finales
    ├── Victoria: Relevo básico, Relevo completo o Ambiguo
    └── Derrota: Asimilación, tras 5 muertes consecutivas
```

## 3. Cantidad estimada

Los valores son los de la configuración completa y son iniciales, sujetos a balance. El showcase escala salas, enemigos y recursos, según [[02 - Mecánicas]].

| Tipo | Cantidad |
| --- | ---: |
| Mundos | 1, Sektor V-7 |
| Niveles | 5: introducción, pisos 1 a 3 y sala de The Mother |
| Arquetipos de sala procedural | 6, con 3 temas |
| Salas por piso | ~10 a 15 en la completa y ~9 a 11 en el showcase |
| Misiones principales | 5 |
| Misiones secundarias | 2 |
| Tipos de enemigo | 4: el Alto, la Boca, el Rastrero y el Mudo |
| Enemigos por intento | 27 a 36, más 4 en la fase 2 de The Mother |
| Jefes | 1, The Mother |
| NPC | 0 vivos |
| Armas | 1, el revólver de Kessler |
| Tipos de objeto de inventario | 5: cura, munición, punzón de descarga, llave obligatoria y llave opcional |
| Documentos de Emilia | 6 |
| Otros documentos | 12 |
| Objetos confiscados | 12 |
| Entradas de bestiario | 6: los 4 tipos, el anexo de reciclados y The Mother |
| Fallas de la máquina | 1 o 2 |
| Finales | 4: Relevo básico, Relevo completo, Ambiguo y Asimilación |

## 4. Dependencias

| Contenido | Depende de | Motivo |
| --- | --- | --- |
| Salas procedurales de los 3 pisos | Los 6 arquetipos y los 3 temas | El generador combina arquetipo y tema |
| Tutorial | La fila de cápsulas, las salas `ROOM_F1_Storage_A`, `ROOM_F1_Corridor_A` y `ROOM_F1_Open_A`, el Rastrero y el revólver | Las 3 salas procedurales del tutorial son variantes fijas de los arquetipos almacén, pasillo y sala abierta |
| Piso 1 | Alto, Rastrero, revólver, punzón de descarga, llaves e interruptor de energía | Son lo primero que encuentra el jugador |
| Piso 2 | La Boca y la cinemática del hallazgo de la foto | La Boca es el tipo nuevo del piso y la foto acompaña el reinicio del contador |
| Piso 3 | El Mudo | Es el tipo nuevo del piso |
| Sala de The Mother | Hackeo por secuencias, Vista de vigilancia, el agarre de Punzón de descarga, Combate a distancia, Alto y Rastrero | Las 3 fases reutilizan mecánicas existentes |
| Decisión final y Relevo completo | Los 6 documentos de Emilia | Solo con los 6 hay elección |
| Final Asimilación | El contador de 5 muertes y el piso que no existe | Reutiliza el piso de la introducción |
| Documentos, objetos confiscados y bestiario | Los textos de [[05 - Textos del Juego]] | Se registran en el terminal del brazo |

> **Navegación:** [[00 - Índice]] · ← [[05 - Textos del Juego]] · [[02 - Niveles]] →
