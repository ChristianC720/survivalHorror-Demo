# Dudas sobre los modelos (2026-11-04)

Decisiones que tomé a mi criterio mientras no estabas. Responde por el chat con el número y lo que prefieras. Lo que no contestes se queda como está.

## Para decidir

1. **Puertas de F2 y F3.** Las hojas son distintas por piso (estanca, hospital, blindada), pero todas siguen subiendo en vertical, porque es lo que hace `DoorLeaf`. Una puerta de hospital subiendo se ve rara. ¿La dejo así o hago que la de F2 se abra hacia los lados? Lo segundo requiere cambiar código de juego.
2. **Props nuevos sin colocar.** Modelé lo que faltaba del catálogo: altavoz, cables, tuberías, basura, rejilla, charco, panel de hackeo, depósito de agua, válvula, herramientas, manómetro, contenedor, bandeja de efectos y perchero. Se ven en `Sandbox_Gallery`, pero ninguna sala los usa todavía. ¿Los coloco en las salas? Cambiaría coberturas y recorridos, por eso no lo hice sin preguntar.
3. **Rampa y plataforma compartidas.** `KIT_Shared_Ramp_2x4` y `KIT_Shared_Platform_2x2` se ven iguales en todos los pisos. Las escaleras de la salida sí tienen versión por piso. ¿Hago también versión por piso de rampa y plataforma?
4. **Objetos del equipo y GDD.** El GDD describe la cura como un estuche con asa y las llaves como tarjeta o placa, pero se usan el autoinyector y la llave entregados. `Aturdidor` no tiene uso en el GDD. ¿Cambio el texto del GDD o pido otro modelo?
5. **Ropa del Alto.** Quedó como pediste dejar a los enemigos: el chaleco es una tira y el pantalón va muy ajustado. Tenía una versión mejor preparada y la retiré para respetar tu mensaje. ¿La aplico?

## Pendiente, sin duda

- **Enemigos reciclados:** sin modelo. Un enemigo reciclado se ve igual que el normal, y la lógica de juego (más rápido, menos disparos) sigue activa.
- **Enemigos y GDD:** D-154 y D-155 reflejan el estado actual, pero la ropa añadida contradice en parte D-155, que dejaba al Mudo desnudo. Lo corregí en D-156.
