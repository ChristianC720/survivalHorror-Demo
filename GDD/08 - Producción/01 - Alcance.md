# Alcance

## 1. Descripción de la sección

Define qué se va a construir y, sobre todo, qué queda fuera del proyecto.

**Versión objetivo:** Demo jugable para PC, con entrega el 13 de noviembre de 2026. El plan está en [[03 - Roadmap]]. Los valores numéricos de este documento son iniciales y están sujetos a balance.

## 2. Alcance de la versión objetivo

### 2.1 Incluido

- **Configuración completa** como predeterminada, con 3 pisos y ~45 a 60 min de juego.
- **Configuración showcase** para ferias y presentaciones, con ~30 min de juego. Se activa al lanzar el juego con `-showcase`. Las salas y los recursos de ambas configuraciones están en Generación procedural por pisos, en [[03 - Sistemas]].
- Todas las mecánicas de prioridad Demo de [[02 - Mecánicas]].
- Los 11 sistemas de [[03 - Sistemas]]. El ciclo de muerte va sin degradación si se recorta la degradación por muerte.
- 4 tipos de enemigo, que son el Alto, la Boca, el Rastrero y el Mudo, y The Mother con 3 fases.
- 6 arquetipos de sala con el tema de cada piso, salas clave secundarias, salas finales y la sala de The Mother ([[02 - Niveles]]).
- Tutorial de 4 salas con carteles e introducción jugable.
- Los 6 documentos de Emilia, la decisión final y las 3 cinemáticas de victoria: Relevo básico, Relevo completo y Ambiguo.
- Terminal del brazo, menús, 3 ranuras, `Opciones` y las opciones de [[03 - Accesibilidad]].
- Modo Supervivencia y Modo Historia.
- Textos en inglés y español.
- Mando tipo Xbox con mapa básico fijo.

### 2.2 No incluido

- Reasignación del mando.
- Todo lo marcado como Post-demo en [[02 - Mecánicas]], como el saqueo de cadáveres, las pasarelas entre salas y los techos rotos.
- El reflejo del haz de la linterna en el suelo, el modelo propio de cada enemigo reciclado y la opción de salida estéreo o auriculares.
- Los finales *Salida* y *Encuentro con Emilia*.
- Actuación de voz, otras plataformas, logros y estadísticas en línea.
- NPC vivos.

## 3. MVP

El **Producto Mínimo Viable** debe contener únicamente lo necesario para demostrar que la experiencia principal funciona.

**MVP del proyecto:** Se cierra con el hito H3, del 1 al 7 de noviembre de 2026. Es un piso 1 generado por procedimiento en *greybox* que prueba el bucle de [[01 - Jugabilidad]] sin arte final. En él, Lukas cumple estos pasos.

- Se mueve con la cámara sobre el hombro.
- Encuentra recursos y los guarda en el inventario de 6 casillas.
- Evita o combate a un enemigo con IA, el Alto, con revólver y empujón.
- Consigue la llave obligatoria y llega a la sala clave final.
- Si muere, se reconstruye en la cápsula con un piso distinto y el terminal conservado.

## 4. Vertical Slice

**Vertical slice:** Se cierra con el hito H4, del 8 al 10 de noviembre de 2026. Es el piso 1 completo con calidad final, y demuestra el tono, el render a ~540p y los tres pilares. Incluye estos elementos.

- Tutorial de 4 salas con carteles.
- Arte y luz ámbar de mantenimiento.
- El Alto y el Rastrero animados.
- Vista de vigilancia con señal escalonada.
- Sigilo con linterna, hackeo, reciclaje y remate.
- Audio de alertas y latidos.
- Terminal del brazo y autoguardado en la sala final.

## 5. Criterios de recorte

Si falta tiempo o recursos, se recorta en este orden. Cada recorte se registra en [[05 - Gestión de Cambios]].

1. **Fallas de la máquina**, que es Deseable. La fase 1 de The Mother usa la longitud original.
2. **Degradación por muerte y final Asimilación**, que es Deseable. No hay contador de 5 muertes ni capas de degradación, y el estado Final: Asimilación no existe.
3. **Configuración showcase.** Se deja de mantener y solo se entrega la completa.
4. **Documentos de contexto y objetos confiscados.** Pasan de 12 y 12 a 4 y 4, uno por tipo y piso.
5. **Cinemáticas.** Las narrativas pasan a imágenes fijas con texto y sonido sobre Timeline. Se mantienen la cinemática corta de reconstrucción y la transición de piso.
6. **Modo Historia.** Se mantiene la selección, pero solo cambia el daño recibido.
7. **Español.** Se entrega solo en inglés, el idioma principal.
8. **Introducción.** Se reduce a la compuerta y el elevador.
9. **Enemigos de los pisos 2 y 3.** La Boca y el Mudo reutilizan la IA del Alto o del Rastrero, con otro modelo y otros valores.
10. **Fase 2 de The Mother.** Se juega sin cámaras hostiles.

**Nunca se recorta:**

- El ciclo de muerte, reconstrucción y piso nuevo.
- La generación válida.
- La vista de vigilancia con costo.
- El inventario en tiempo real.
- The Mother, salvo lo que permite el recorte 10.
- Los 6 documentos de Emilia y la decisión final.
- Las opciones de accesibilidad de brillo y subtítulos.

> **Navegación:** [[00 - Índice]] · ← [[05 - Seguridad y Riesgos Técnicos]] · [[02 - Roles y Responsabilidades]] →
