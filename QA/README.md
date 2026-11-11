# Pruebas QA - Identity Dissolution

En esta carpeta guardamos las pruebas que hemos hecho al videojuego **Identity Dissolution**, los resultados y los errores que encontramos.

Trabajamos sobre la rama `feature/Heartbeat`, usando una copia del proyecto para no modificar el código original.

## Entorno

- Sistema operativo: Debian GNU/Linux 13
- Unity: 6000.6.8f1
- Rama evaluada: `feature/Heartbeat`
- Commit base: `a42ec20`

## Resultados hasta ahora

Las pruebas automatizadas de Unity terminaron con estos resultados:

| Tipo | Aprobadas | Fallidas | Omitidas |
|---|---:|---:|---:|
| EditMode | 202 | 0 | 0 |
| PlayMode | 16 | 0 | 0 |
| **Total** | **218** | **0** | **0** |

También llevamos nueve pruebas manuales. Revisamos controles, cámara, enemigos, sonido y algunos comportamientos durante la partida.

Por ahora tenemos seis incidencias registradas. Algunas se pudieron repetir y otras todavía necesitan más pruebas.

## Archivos

- [Pruebas manuales](Matriz-de-Pruebas.md): qué revisamos y qué pasó.
- [Incidencias](Incidencias.md): problemas encontrados y su seguimiento.
- [Resultados generales](Resumen-Ejecutivo.md): resumen de esta etapa.
- [Evidencias](Evidencias/README.md): qué archivos tenemos y qué falta documentar.

Los resultados automatizados están en `Pruebas-Automatizadas/EditMode` y `Pruebas-Automatizadas/PlayMode`.

## Pendientes

Nos falta revisar las animaciones de enemigos, repetir algunas pruebas y relacionar las capturas o grabaciones con los reportes.

Esta documentación corresponde a un avance. No significa que el juego ya esté libre de errores ni que las incidencias tengan una causa técnica confirmada.
