# Dudas sobre los modelos (2026-11-04)

Responde por el chat con el número y lo que prefieras. Lo que no contestes se queda como está. Las cinco primeras dudas ya las contestaste y están aplicadas (D-157).

## Para decidir

1. **Alto: versión mejorada o anterior.** El juego usa la mejorada (`CH_Alto`). La anterior sigue en `CH_Alto_V1`, y las dos se comparan en `Sandbox_Gallery`, en Modelos sueltos y Enemigos. ¿Con cuál me quedo?
2. **Ribetes de Lukas.** Ahora que Vitalis es cian, el GDD deja los ribetes rojos de su overol como un detalle propio de Lukas, porque pediste no tocar su modelo. ¿Los paso a cian o se quedan rojos?
3. **Texto de The Mother.** Su modelo aún dice "VITALIS KONZERN" en rojo, porque usa el atlas antiguo y al regenerarlo también se reharían las piezas del equipo. ¿Lo cambio a cian a mano en la textura?
4. **Triángulos del Mudo.** Sin ropa tiene 3192 triángulos, por debajo de los 5–8k que pediste para enemigos. Subirlo cambiaría el modelo original. ¿Se queda así?
5. **Luces y rendimiento.** Para que ninguna luz atraviese paredes, solo se encienden 3 luces de sala con sombra por vez. Las salas vecinas sin presupuesto se ven a oscuras desde una puerta abierta, hasta que entras. Dar sombra a más luces subiría el coste y pasaría el límite del GDD (4 con la linterna). ¿Lo dejo así?
6. **`CLAUDE.md`.** Sigue diciendo que el kit y las salas se hacen a mano con Aura, y ya no es lo que hacemos en el kit (D-156). ¿Lo actualizo?
7. **Guardado.** `slot_0.json` da un aviso de archivo ilegible al entrar en Play y tiene fecha del 3 de noviembre. No lo toqué porque es tu guardado de pruebas. ¿Lo borro?

## Pendiente, sin duda

- **Enemigos reciclados:** sin modelo. Un enemigo reciclado se ve igual que el normal, y la lógica de juego sigue activa.
