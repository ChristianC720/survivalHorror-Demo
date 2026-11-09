# Gestión de Cambios

## 1. Descripción de la sección

El diseño de un videojuego cambia. Esta sección evita que las modificaciones importantes se pierdan o aparezcan sin evaluar sus consecuencias.

**Qué es un cambio.** Es cualquier modificación de una regla, del alcance, de la versión del motor, de un paquete o de la estructura del guardado. También lo es un valor de balance que no esté marcado como valor inicial.

Ajustar un valor marcado como inicial y sujeto a balance dentro de su ScriptableObject no requiere solicitud. Se anota en la descripción del *pull request*. Los valores numéricos de este documento son iniciales y están sujetos a balance.

**Flujo:**

1. Quien propone presenta la solicitud con la plantilla de la sección 2 y la anota en el cronograma `CronogramaIdentityDissolution_Objetivos_v3.xlsx` ([[04 - Backlog del Proyecto]]).
2. El responsable del área afectada estima el costo y el impacto. Christian Coronel Palma decide ([[02 - Roles y Responsabilidades]]).
3. Si se aprueba, se registra la decisión en [[03 - Registro de Decisiones]], se actualizan las notas del GDD afectadas y se sube la versión en [[04 - Historial de Versiones]].
4. Desde la congelación de funciones, al cierre del hito H5 el 12 de noviembre de 2026, solo se aprueban cambios que corrigen errores o recortan alcance ([[01 - Alcance]]).

## 2. Solicitud de cambio

**Fecha:** Día en que se presenta la solicitud.

**Cambio propuesto:** Qué regla, valor, alcance, paquete o estructura cambia.

**Motivo:** Por qué hace falta el cambio.

**Beneficio esperado:** Qué mejora en el juego o en la producción.

**Costo estimado:** Horas o días de trabajo.

**Impacto en otros sistemas:** Mecánicas, sistemas y notas del GDD afectadas.

**Decisión:** Aprobado, Rechazado o Pospuesto para después de la demo.

**Responsable:** La persona que aplica el cambio.

## 3. Registro

| Fecha | Cambio | Motivo | Impacto | Estado |
|---|---|---|---|---|
| 2026-10-21 | Cierre de Tecnología, Producción y Calidad | Base para empezar el desarrollo en Unity | GDD 0.11.0 y guías de `Desarrollo/` | Aprobado |
| 2026-10-21 | Nodos y fin de fase de The Mother, y daño del Modo Historia | Puntos pendientes del jefe y contradicción del Modo Historia | Jefe final The Mother, sistema del jefe, Contenido, Economía, Accesibilidad y Calidad. Guías 02 y 09 | Aprobado |
| 2026-10-21 | Revisión de huecos e incoherencias | Bloqueo en la fase 2, piso 3 sin capacidad, cura ambigua, mando contradictorio, documentos sin piso y blísteres | Salud y estados, exploración vertical, jefe final, salud y combate, generación procedural, Controles, Economía, Contenido, Alcance y Calidad. Guías 02, 06, 07 y 09 | Aprobado |
| 2026-10-21 | Cierre de los puntos por definir de reglas, mundo, tipografía, textos y diseño visual | Valores sin definir y textos pendientes | Empujón, linterna, Mundo, Interfaz Visual, Accesibilidad, Diálogos, Textos del Juego y Objetos. Guías 02, 06 y 10 | Aprobado |
| 2026-10-21 | Modo Historia y avisos con mando | Mínimo por encima del máximo y avisos solo de teclado | Economía, Accesibilidad, Interfaz Visual y Controles. Guías 01, 07 y 10 | Aprobado |
| 2026-10-22 | Fusión de la revisión local con el remoto, calendario, equipo y seguimiento | Fecha real de entrega, una sola persona programa y textos que contradecían decisiones registradas | Diseño, Historia, Contenido, Tecnología, Producción, Calidad y Referencia. Guías README, 00, 03, 12 y 13 | Aprobado |
| 2026-10-29 | Interfaz visual repartida entre tres personas | La interfaz visual no tenía responsable | Roles y Responsabilidades e Interfaz Visual | Aprobado |
| 2026-10-29 | Prototipo de mecánicas desechable en `feature/heartbeatPrueba`, que no se fusiona | Acelerar H1 y H2 sin heredar una estructura distinta | Roadmap y Motor y Herramientas | Aprobado |
| 2026-11-02 | Toda la animación, incluida la locomoción base, se hace en Blender | Se elimina la animación externa. Los clips se ajustan a las velocidades y posturas del GDD | Personajes y Animación y Motor y Herramientas | Aprobado |
| 2026-11-05 | Lo que no se ve no se dibuja. Los enemigos nunca se desactivan | Rendimiento | Requisitos Técnicos | Aprobado |
| 2026-11-08 | Recortes de la demo: reflejo del haz en el suelo, modelo propio de cada reciclado y salida estéreo o auriculares | Alcance de la demo | Alcance, Sigilo y ruido, Personajes y Animación | Aprobado |
| 2026-11-09 | Parche del motor de `6000.6.6f1` a `6000.6.8f1` | Misma versión Unity 6.6, solo parche | Motor y Herramientas, Requisitos Técnicos y Seguridad y Riesgos Técnicos | Aprobado |

> **Navegación:** [[00 - Índice]] · ← [[04 - Backlog del Proyecto]] · [[01 - Criterios de Calidad]] →
