# Bugs y Seguimiento

## 1. Descripción de la sección

Registra problemas encontrados durante las pruebas y permite darles seguimiento.

## 2. Reporte de bug

Cada bug se registra en el Excel de seguimiento `CronogramaIdentityDissolution_Objetivos_v3.xlsx`, con su severidad. Lenin Ramón López Córdova los clasifica y les asigna responsable ([[02 - Roles y Responsabilidades]]). Cada registro lleva estos campos.

**Título:** El sistema afectado entre corchetes y qué falla, en una frase.

**Versión:** Commit o build, con el resultado de `git rev-parse --short HEAD`.

**Nivel / escena:** Piso, sala o escena, y la semilla del piso que muestra la consola de depuración.

**Pasos para reproducir:** Lista numerada con las acciones mínimas que provocan el fallo desde la ranura o escena indicada.

**Resultado esperado:** La regla del GDD que no se cumple, citada por el nombre de su mecánica o sistema y su nota.

**Resultado actual:** Lo que ocurre en el juego.

**Frecuencia:** Siempre, A veces o Rara vez.

**Severidad:**

- **Crítica.** Cierre del juego, pérdida o corrupción del guardado, piso imposible o progreso bloqueado.
- **Alta.** Una regla del GDD falla de forma visible, como una vigilancia que no se corta en caza, o el rendimiento cae por debajo del mínimo.
- **Media.** Error con rodeo fácil, o una animación o un sonido incorrecto.
- **Baja.** Detalle visual o de texto.

**Evidencia:** Captura, video o el archivo `%USERPROFILE%\AppData\LocalLow\Heartbeat\Identity Dissolution\Player.log`.

**Estado:** Abierto, En progreso, Resuelto o Cerrado.

## 3. Registro

El ID de cada bug es su número en el Excel de seguimiento. Esta tabla solo resume los bugs críticos y altos abiertos en cada hito.

Los estados avanzan en este orden:

1. **Abierto.** Sin asignar.
2. **En progreso.** Con responsable trabajando en él.
3. **Resuelto.** Corregido en `develop`.
4. **Cerrado.** QA lo verificó en una build.

Ningún hito se cierra con bugs críticos abiertos. La entrega del 13 de noviembre de 2026 no puede tener bugs críticos ni altos abiertos ([[03 - Roadmap]]).

| ID | Bug | Severidad | Responsable | Estado |
|---|---|---|---|---|
| Ninguno | Sin bugs registrados todavía | No aplica | No aplica | No aplica |


> **Navegación:** [[00 - Índice]] · ← [[02 - Pruebas]] · [[04 - Criterios de Aceptación]] →
