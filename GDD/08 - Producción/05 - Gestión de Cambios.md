# Gestión de Cambios

## Descripción de la sección

El diseño de un videojuego cambia. Esta sección evita que las modificaciones importantes se pierdan o aparezcan sin evaluar sus consecuencias.

**Qué es un cambio:** cualquier modificación de una regla, un valor de balance que no esté marcado como "valor inicial", el alcance, la versión del motor, un paquete o la estructura del guardado. Ajustar un valor marcado "valor inicial, sujeto a balance" dentro de su ScriptableObject **no** requiere solicitud; se anota en la descripción del *pull request*.

**Flujo:**

1. Quien propone abre una tarjeta de Trello con la etiqueta "Cambio" y la plantilla de abajo.
2. El responsable del área afectada estima el costo y el impacto; Christian Coronel Palma decide ([[02 - Roles y Responsabilidades]]).
3. Si se aprueba, se registra como decisión D-xx en [[03 - Registro de Decisiones]], se actualizan las notas del GDD afectadas y se sube la versión en [[04 - Historial de Versiones]].
4. Desde la congelación de funciones (2026-11-15, H5) solo se aprueban cambios que corrigen errores o recortan alcance ([[01 - Alcance]]).

## Solicitud de cambio

**Fecha:**

**Cambio propuesto:**

**Motivo:**

**Beneficio esperado:**

**Costo estimado:** (horas o días de trabajo)

**Impacto en otros sistemas:** (S-xx, M-xx y notas del GDD afectadas)

**Decisión:** Aprobado / Rechazado / Pospuesto (Post-demo)

**Responsable:**

## Registro

| Fecha | Cambio | Motivo | Impacto | Estado |
|---|---|---|---|---|
| 2026-10-21 | Cierre de Tecnología, Producción y Calidad (D-107..D-113) | Base para empezar el desarrollo en Unity | GDD 0.11.0; guías de `Desarrollo/` | Aprobado |
| 2026-10-21 | Nodos y fin de fase de The Mother; daño del Modo Historia (D-114..D-116) | Puntos pendientes del jefe y contradicción del Modo Historia | M-19, S-10, Contenido, Economía, Accesibilidad, Calidad; guías 02 y 09 | Aprobado |
| 2026-10-21 | Revisión de huecos e incoherencias (D-117..D-122) | Bloqueo en la fase 2, piso 3 sin capacidad, cura ambigua, mando contradictorio, documentos sin piso, blísteres | M-05, M-15, M-19, S-03, S-06, S-10, Controles, Economía, Contenido, Alcance, Calidad; guías 02, 06, 07 y 09 | Aprobado |
| 2026-10-21 | Cierre de "Por definir" de reglas, mundo, tipografía, textos y diseño visual (D-123..D-130) | Valores sin definir y textos pendientes | M-06, M-12, Mundo, Interfaz Visual, Accesibilidad, Diálogos, Textos del Juego, Objetos; guías 02, 06 y 10 | Aprobado |
| 2026-10-21 | Modo Historia y avisos con mando (D-131..D-132) | Mínimo por encima del máximo; avisos solo de teclado | Economía, Accesibilidad, Interfaz Visual, Controles; guías 01, 07 y 10 | Aprobado |
| 2026-10-22 | Fusión de la revisión local con el remoto; calendario, equipo y seguimiento (D-133..D-135) | Fecha real de entrega (2026-11-20), una sola persona programa, textos que contradecían decisiones registradas | Diseño, Historia, Contenido, Tecnología, Producción, Calidad, Referencia; guías README, 00, 03, 12 y 13 | Aprobado |


> **Navegación:** [[00 - Índice]] · ← [[04 - Backlog del Proyecto]] · [[01 - Criterios de Calidad]] →
