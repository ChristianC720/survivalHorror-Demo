# Resumen de pruebas QA

## Alcance

Este documento reúne los resultados obtenidos hasta ahora al probar Identity Dissolution sobre la rama `feature/Heartbeat`, en el commit `a42ec20`.

Las pruebas se realizaron en Debian GNU/Linux 13 con Unity 6000.6.8f1, utilizando una copia aislada del proyecto.

## Pruebas automatizadas

| Modalidad | Total | Aprobadas | Fallidas | Omitidas |
|---|---:|---:|---:|---:|
| EditMode | 202 | 202 | 0 | 0 |
| PlayMode | 16 | 16 | 0 | 0 |
| **Total** | **218** | **218** | **0** | **0** |

Los archivos XML de ambas ejecuciones están incluidos en el repositorio.

Se registró además una incidencia del entorno durante una ejecución anterior de PlayMode sin interfaz gráfica. La ejecución posterior utilizada para este resumen terminó correctamente. Ese problema del entorno no se considera un defecto confirmado del videojuego.

## Pruebas manuales

Se documentaron nueve pruebas manuales:

- Una prueba de interacción y reinicio con resultado favorable.
- Seis pruebas que dieron lugar a incidencias.
- Una prueba de seguimiento para intentar reproducir BUG-006.
- Una prueba de movimiento de enemigos con aprobación parcial.

Las observaciones detalladas están en [Matriz de pruebas](Matriz-de-Pruebas.md).

## Incidencias

Tenemos seis incidencias documentadas, identificadas como BUG-001 a BUG-006.

BUG-006 tiene un seguimiento específico: la anomalía apareció en uno de tres intentos de reproducción. Esto no permite calcular su frecuencia real.

Las incidencias siguen pendientes de validación técnica. No se han confirmado sus causas.

## Limitaciones

- No se han completado todas las pruebas manuales.
- Falta vincular evidencia audiovisual a los reportes.
- Algunas incidencias requieren más intentos de reproducción.
- Los resultados corresponden a la versión evaluada, no necesariamente a versiones posteriores.
- Que todas las pruebas automatizadas hayan aprobado no implica que el videojuego esté libre de defectos.

## Siguiente etapa

Continuar con QA-MAN-010, revisar las animaciones y transiciones de enemigos, y dar seguimiento a las incidencias abiertas.
