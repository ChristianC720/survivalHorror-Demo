# UX y Flujo del Jugador

## Descripción de la sección

UX (experiencia de usuario) describe qué tan comprensible, cómoda y coherente resulta la interacción con el videojuego.

Estados y transiciones completos en [[07 - Reglas y Estados del Juego]]; interfaz en [[04 - Interfaz Visual]].

## Flujo principal

```text
Primer inicio: calibración de brillo (D-106)
Título (negro, logotipo y latido) → Menú principal (terminal de la sala de cápsulas, D-90)
  → Nueva partida (ranura y modo, D-103, D-105) → Introducción → Reconstrucción completa
  → Piso 1 (tutorial, primer intento) → Piso 2 → Piso 3 → Sala de The Mother → Final → Créditos
  → Continuar: ciclo nuevo con el terminal conservado (D-93)

Durante el recorrido:
  Pausa ⇄ Juego
  Muerte → Reconstrucción corta → Piso 1 (semilla nueva, D-101)
  5.ª muerte → Final: Asimilación
```

## Primeros minutos

| Momento | Qué descubre el jugador | Cómo |
|---|---|---|
| Primer inicio | Que el juego es oscuro y necesita ver los negros | Calibración de brillo (D-106) |
| Casa (cinemática) | Lukas, la foto y la nota de voz de Emilia; un latido de fondo que no sabe explicar | Cinemática con subtítulos (D-53, D-95) |
| Compuerta | Moverse, mirar, interactuar y hackear | HUD mínimo; hackeo en el teléfono (D-63) |
| Piso que no existe | Que algo está mal; muerte inevitable | Pasillo único y muerte guionizada (D-53) |
| Reconstrucción completa | Que Lukas olvidó y vuelve como R-H-07 | Cinemática (D-39, D-45) |
| Fila de cápsulas | Terminal, anillo, bandeja; único lugar seguro | Carteles de Vitalis (D-89); el anillo en el suelo (D-54) |
| Depósito a oscuras | Linterna, recoger, inventario, revólver | Carteles; destello bajo la linterna (D-84) |
| Túnel | Correr y resistencia | Carteles; jadeo |
| Sala de calderas | Sigilo, empujón, disparo, que la muerte es real | 1 Rastrero real (D-70) |
| Primera muerte | El ciclo: inventario perdido, terminal conservado, piso distinto | Reconstrucción corta y semilla nueva |

## Retroalimentación

**Entrada → Respuesta visual → Respuesta sonora → Resultado**

| Acción | Entrada | Respuesta visual | Respuesta sonora | Resultado |
|---|---|---|---|---|
| Moverse / correr | `WASD` / `Shift` | Animación y postura según salud | Pasos según superficie; jadeo sin resistencia | Ruido para los enemigos (M-06) |
| Linterna | `F` | Haz blanco; destellos en recogibles | Clic | Ver a cambio de delatarse (M-12) |
| Interactuar | `E` | Aviso `E` a ~1.5 m | Sonido del objeto | Recoger, leer, abrir (M-08) |
| Apuntar y disparar | `Clic derecho` + `Clic izquierdo` | Retícula de punto; contador verde del revólver | Disparo; tambor vacío | Daño; ruido a ~20 m (M-04) |
| Recargar | `R` | Animación bala a bala | Cartucho por bala | Tambor cargado (M-04) |
| Empujón | `Clic izquierdo` sin apuntar | Animación de empujón | Golpe | Aleja al enemigo (M-14) |
| Remate | `Clic izquierdo` / `E` sobre un cuerpo | Golpe en el suelo o punzón (~1.5 s) | Golpe ruidoso / descarga silenciosa | El cuerpo no se recicla (M-20) |
| Curarse | Inventario | Animación ~1.5 s; postura mejora | Kit médico | +1 estado de salud (M-05) |
| Recibir daño | — | Vibración de cámara, cambio de postura | Impacto y quejido | Baja un estado de salud (M-05) |
| Hackear | `E` y secuencia | Secuencia en el terminal; panel verde | Pitido por entrada / error | Abrir, sellar o activar (M-13) |
| Vigilancia | `Q` | Imagen verde con ruido CRT | Interferencia | Información parcial; atención de The Mother (M-02) |
| Registrar | Leer o examinar | `REGISTERED` en el terminal | Pitido de registro | Persiste entre muertes (M-16) |
| Cambiar de piso | `E` en escalera o elevador | Cinemática ≥ 5 s o `LOCKDOWN` en caza | Ascensor / alarma de cierre | Piso siguiente y autoguardado (D-96) |

## Fricción intencional

**Forma parte del diseño (no se suaviza):**

- Oscuridad: buscar exige linterna, y la linterna delata (Pilar 1).
- Inventario de 6 casillas en tiempo real y munición escasa.
- Recarga bala a bala y dispersión al moverse.
- Sin mapa; la vigilancia da información parcial y llena la atención de The Mother.
- Pérdida del inventario al morir, piso nuevo en cada entrada (D-101) y contador de 5 muertes.
- Cierre de tránsito durante la caza (D-96): no se huye cambiando de piso.

**Es mala experiencia (se evita):**

- Golpes "invisibles" por la cámara OTS: se compensan con las señales de M-10 y los subtítulos de sonido (D-102).
- Perder un objeto en la oscuridad sin poder encontrarlo: los recogibles destellan bajo cualquier luz (D-84).
- No saber por qué se murió: cada ataque tiene animación y sonido de aviso propios.
- Perder progreso por un cierre inesperado: el terminal se autoguarda en cada cambio (S-07).
- Saltar sin querer una cinemática: las narrativas se saltan manteniendo `Espacio` (D-91).
- Confundir la señal degradada con la degradación estética: tope de D-29.
- Bloquearse sin ruta: la llave obligatoria siempre es alcanzable (D-36).

## Descubrimiento

**Qué hacer:** entorno y líneas de protocolo de The Mother (puerta cerrada, `NO POWER`, "Restricted access: level key required.") y contadores de las secundarias en el terminal (D-78).

**Dónde ir:** sin mapa ni marcadores. La lectura del entorno (señalización de Vitalis, luz de emergencia, puertas) y la vista de vigilancia; la sala final siempre está a ≥ 4 salas (D-100).

**Qué puede interactuar:** destello bajo la luz y aviso `E` a ~1.5 m (D-84); carteles del tutorial (D-89).

**Qué ocurrió:** registros del terminal (documentos, bestiario, fallas) y señales de M-10; con subtítulos de sonido para quien los active (D-102).

**Por qué ganó o perdió:** la muerte muestra el ataque que la causó; la cinemática corta refleja la degradación; la Asimilación llega tras 5 muertes consecutivas y la degradación acumulada la anticipa; la victoria depende de derrotar a The Mother y el final, de los 6 documentos de Emilia (D-51).


> **Navegación:** [[00 - Índice]] · ← [[05 - Audio y Música]] · [[02 - UI y Menús]] →
