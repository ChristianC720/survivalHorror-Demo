# Mecánicas

## Descripción de la sección

Una mecánica es una regla o interacción mediante la cual el jugador puede actuar dentro del sistema del juego.

Documenta cada mecánica con suficiente detalle para que otro integrante pueda implementarla.

## Plantilla de mecánica

### Nombre de la mecánica

**Propósito:**  
¿Qué aporta a la experiencia?

**Activación:**  
¿Qué debe hacer el jugador?

**Reglas:**  
¿Qué puede y qué no puede ocurrir?

**Entrada:**  
¿Qué botón, gesto o acción la activa?

**Estado inicial:**

**Estado final:**

**Recursos utilizados:**

**Recompensa:**

**Penalización:**

**Interacciones con otras mecánicas:**

**Casos límite:**  
¿Qué ocurre en situaciones poco comunes?

## Mecánicas principales

### Mecánica 1: Transición Dinámica de Cámara e Iluminación 

- **Propósito:** Ofrecer lectura táctica del entorno durante la exploración y generar claustrofobia, tensión y enfoque táctico durante el combate.
    
- **Activación:** El jugador presiona y mantiene el comando de apuntar o enfocar.
    
- **Reglas:**
    
    - En **vista isométrica**, el jugador tiene una visión amplia del mapa y de la red de vigilancia. No se muestra retícula de disparo.
        
    - Al pasar a **vista al hombro**, el campo de visión  se reduce, la linterna intensifica su cono de luz frontal y se pierde la visión periférica y trasera.
        
    - La velocidad de movimiento del personaje se reduce en un 30% mientras está en vista al hombro.
        
    - El cambio de cámara se realiza con una interpolación suave y rápida.
        
- **Entrada:** Mantener `Gatillo Izquierdo (L2/LT)` o `Clic Derecho del Mouse`.
    
- **Estado inicial:** Vista isométrica superior fija (Exploración).
    
- **Estado final:** Vista sobre el hombro con retícula en pantalla.
    
- **Recursos utilizados:** Energía de atención del jugador y batería de la linterna.
    
- **Recompensa:** Capacidad de apuntar a puntos débiles de los enemigos y mayor visibilidad en pasillos oscuros.
    
- **Penalización:** Vulnerabilidad a ataques por la espalda o flancos no detectados por falta de visión periférica.
    
- **Interacciones con otras mecánicas:** Interactúa con el _Disparo/Combate_, _Gestión de Resistencia_ (impide esprintar) y _Visibilidad de Enemigos_.
    
- **Casos límite:**
    
    - Si el jugador activa la vista al hombro pegado a una pared, la cámara debe colisionar y acortar su distancia sin atravesar la geometría.
        
    - Si el jugador es golpeado mientras está en vista al hombro, la cámara vibra pero no rompe la transición a menos que caiga derribado.
        

### Mecánica 02: Persistencia del Terminal e Información entre Ciclos

- **Propósito:** Permitir que el fracaso del jugador se convierta en una herramienta de progreso narrativo y mecánico contra la IA .
    
- **Activación:** Interactuar con un terminal de control en la sala o reaparecer tras morir.
    
- **Reglas:**
    
    - La muerte elimina todos los objetos físicos del inventario.
        
    - Todos los archivos de texto, códigos de seguridad de puertas y pistas de acertijos leídos en partidas anteriores permanecen guardados en la memoria persistente del **Terminal**.
        
    - El mapa cambia de distribución entre partidas, pero los códigos almacenados en el terminal siguen siendo válidos para las puertas clave.
        
    - La interfaz del terminal **no pausa la partida**.
        
- **Entrada:** Pulsar `Botón A / X` o tecla `E` frente a un terminal interactivo.
    
- **Estado inicial:** Datos del laboratorio bloqueados / inaccesibles.
    
- **Estado final:** Registro de la base de datos actualizado de forma permanente en el guardado global del juego.
    
- **Recursos utilizados:** Archivo de guardado en disco.
    
- **Recompensa:** Desbloqueo de atajos, contraseñas de puertas blindadas y avance en la historia sin repetir acertijos.
    
- **Penalización:** Ninguna directa; el costo previo fue la muerte de la unidad/clon anterior.
    
- **Interacciones con otras mecánicas:** Interactúa con la _Reaparición por Clonación_, _Generación Procedural de Salas_ y _Apertura de Puertas de Seguridad_.
    
- **Casos límite:**
    
    - Si un jugador muere justo en el instante en que interactúa con un terminal, la información leída se registra en la memoria global antes de procesar la pantalla de muerte.
        

### Mecánica 3: Gestión de Inventario en Tiempo Real 

- **Propósito:** Forzar al jugador a priorizar recursos y buscar zonas seguras antes de tomar decisiones tácticas bajo presión.
    
- **Activación:** El jugador abre el menú de inventario durante el recorrido.
    
- **Reglas:**
    
    - El inventario tiene un límite de **6 casillas fijas**.
        
    - Abrir el inventario **no pausa el juego** ni detiene la IA enemiga.
        
    - Los objetos no se pueden apilar infinitamente.
        
    - Equipar, usar o descartar un objeto consume un tiempo de animación de 1.5 segundos.
        
- **Entrada:** Pulsar `Botón Y / Triángulo` o tecla `TAB` / `I`.
    
- **Estado inicial:** Interfaz de inventario oculta.
    
- **Estado final:** Interfaz desplegada superpuesta en pantalla mientras el juego continúa en tiempo real.
    
- **Recursos utilizados:** Espacio físico en la retícula de 6 ranuras.
    
- **Recompensa:** Organización de equipamiento, curación y recarga de suministros.
    
- **Penalización:** Quedar indefenso a un ataque si se administra el inventario en un pasillo no despejado.
    
- **Interacciones con otras mecánicas:** Interactúa con el _Combate_, _Recolección de Loot / Cadáveres_ y _Uso de Objetos Clave_.
    
- **Casos límite:**
    
    - Si el inventario está lleno e intentas recoger un objeto de misión clave, el sistema permite reemplazar un objeto del inventario soltándolo al suelo en lugar de perder el objeto clave.
        

### Mecánica 04: Sistema Unificado de Resistencia

- **Propósito:** Crear una gestión táctica del esfuerzo físico del protagonista, obligando a medir cada movimiento para no quedar exhausto.
    
- **Activación:**
    
    - **Esprintar:** Mantener el botón de correr mientras se mueve.
        
    - **Evasión Corta:** Pulsar el botón de esquivar en una dirección.
        
    - **Ataque Cuerpo a Cuerpo:** Pulsar el botón de ataque melee.
        
- **Reglas:**
    
    - Una sola barra de resistencia se consume con cualquiera de las tres acciones físicas y se regenera automáticamente tras 1 segundo de inactividad física.
        
    - **Evasión Corta:** Es un desplazamiento rápido y corto para evadir un golpe por milímetros.
        
    - **Ataque Cuerpo a Cuerpo:** Golpear con la culata o tubo industrial desgasta resistencia para apartar enemigos o ahorrar balas.
        
    - **Estado de Agotamiento:** Si la resistencia llega a 0, el jugador no puede esprintar, atacar ni esquivar durante 2.5 segundos.
        
- **Entrada:**
    
    - **Esprintar:** Mantener `L3 / Shift Izquierdo` + Dirección.
        
    - **Evasión Corta:** Pulsar `Botón B / Círculo` o `Barra Espaciadora` + Dirección.
        
    - **Ataque Melee:** Pulsar `R3 / Clic Central` o tecla dedicada.
        
- **Estado inicial:** Barra de resistencia al 100%.
    
- **Estado final:** Barra de resistencia consumida proporcionalmente (o en estado de agotamiento si llegó a 0).
    
- **Recursos utilizados:** Barra de Resistencia.
    
- **Recompensa:** Capacidad de esquivar golpes fatales por milímetros y conservar munición.
    
- **Penalización:** Quedar indefenso y exhausto si se administra mal la barra.
    
- **Interacciones con otras mecánicas:**
    
    - **Disparo:** Las armas de fuego **no consumen resistencia**, lo que permite disparar cuando la resistencia física está agotada.
        
    - **Escape de Agarre:** Si el jugador falla la evasión corta y es atrapado, se activa la mecánica de escape con flechas.
        
- **Casos límite:**
    
    - Hacer una evasión corta con menos del 15% de resistencia provoca una animación de esquiva más lenta y sin ventana de invulnerabilidad.
        

## Mecánicas Secundarias

### Mecánica 1: Sigilo y Campo de Visión Enemigo

- **Propósito:** Brindar la opción de evitar el combate directo, ahorrar munición y maniobrar en las salas usando las coberturas de la instalación.
    
- **Activación:** Pulsar el botón de agacharse/sigilo o moverse en vista isométrica fuera del cono de visión enemigo.
    
- **Reglas:**
    
    - **Cono de Visión Enemigo:** Los enemigos proyectan un cono de visión visible sobre el suelo.
        
    - **Oclusión por Obstáculos:** El cono de visión **no atraviesa objetos sólidos**. Si una caja tapa al jugador, el enemigo no lo detectará.
        
    - **Nivel de Ruido:** Ir agachado genera 0% ruido. Caminar produce ruido bajo. Esprintar emite un pulso de sonido en área que alerta a los enemigos cercanos.
        
    - **Uso de Linterna:** Apuntar con la linterna encendida hacia un enemigo incrementa rápidamente su nivel de alerta.
        
- **Entrada:** Pulsar `Botón C / Ctrl` para agacharse o permanecer oculto tras coberturas.
    
- **Estado inicial:** Estado "Infiltración".
    
- **Estado final:** Oculto o Detectado.
    
- **Recursos utilizados:** Coberturas del mapa, oscuridad, resistencia.
    
- **Recompensa:** Evitar el gasto de munición y curas atravesando salas peligrosas sin entrar en combate.
    
- **Penalización:** Avance significativamente más lento; riesgo de quedar acorralado en esquinas sin salida.
    
- **Interacciones con otras mecánicas:**
    
    - **Manipulación de Cajas:** El jugador puede empujar/mover cajas para crear sus propias coberturas de sigilo. El ruido al arrastrarlas puede atraer enemigos.
        
    - **Cámara:** La vista isométrica permite identificar los conos de visión y trazar rutas de infiltración.
        
- **Casos límite:**
    
    - Si el jugador está oculto tras una caja pero la luz de su linterna ilumina el suelo frente al enemigo, la IA detectará el reflejo.
        

### Mecánica 2: Liberation Táctica de Agarre 

- **Propósito:** Ofrecer una última oportunidad de defensa cuando un enemigo atrapa al personaje, evitando daño fatal a cambio de consumir un recurso estricto.
    
- **Activación:** Automática e inmediata cuando un enemigo ejecuta su animación de "agarre" sobre el personaje.
    
- **Reglas:**
    
    - Aparece en pantalla una secuencia rápida de entradas direccionales.
        
    - **Límite de 3 usos por vida:** Solo se puede completar con éxito **un máximo de 3 veces por cada ciclo/clon** del jugador.
        
    - Introducir la secuencia correctamente a tiempo hace que el personaje empuje al enemigo y lo aturda por 2 segundos.
        
    - Si se falla, expira el tiempo o **ya se gastaron los 3 usos de esa vida**, el enemigo inflige su ataque de daño masivo o muerte directa.
        
- **Entrada:** Pulsar en orden la secuencia de direcciones con la **Cruceta / D-Pad**, **WASD / Flechas** o el **Stick Analógico**.
    
- **Estado inicial:** Jugador atrapado por el enemigo; breve desaceleración visual de tiempo.
    
- **Estado final:**
    
    - **Éxito:** Enemigo aturdido y el jugador recupera el control de movimiento.
        
    - **Fallo / Sin usos:** El personaje recibe daño crítico o muere.
        
- **Recursos utilizados:** Contador de Cargas de Escape (3/3 por vida).
    
- **Recompensa:** Evitar daño fatal o muerte instantánea y ganar 2 segundos para reposicionarse.
    
- **Penalización:** Consumir una de las 3 cargas limitadas de la vida actual.
    
- **Interacciones con otras mecánicas:**
    
    - **Clonación:** Al morir y reaparecer, el contador se restablece a 3/3 cargas.
        
    - **Cámara:** La vista se aleja ligeramente para mostrar con claridad los iconos de las flechas sin tapar la acción.
        
- **Casos límite:**
    
    - Al intentar usarlo sin cargas (0/3 restantes), la interfaz muestra los iconos de las flechas tachados, ejecutando el daño completo sin opción a escape.
        

### Mecánica 3: Hackeo Dinámico y Calibración de Terminales (Estilo Stratagem / Helldivers)

- **Propósito:** Añadir estrés controlado y habilidad manual al interactuar con consolas, puertas blindadas y sistemas de _The Mother_ en tiempo real.
    
- **Activación:** Acercarse a un panel interactivo o terminal de datos e iniciar la interacción.
    
- **Reglas:**
    
    - Se muestra en la interfaz una cadena de comandos direccionales (flechas/vectores).
        
    - Longitud según el tipo de acción: Puerta estándar, Anulación de seguridad, Terminal Principal.
        
    - **El juego no se pausa** durante la secuencia.
        
    - Errar una dirección cancela la cadena actual y obliga a reiniciar la secuencia de esa fase.
        
- **Entrada:** Pulsar la secuencia con la **Cruceta / D-Pad** o **WASD / Flechas de Dirección**.
    
- **Estado inicial:** Panel bloqueado en pantalla.
    
- **Estado final:**
    
    - **Éxito:** Sistema activado, puerta abierta o datos registrados en la memoria persistente.
        
    - **Fallo:** Pitido de error y reinicio de la combinación.
        
- **Recursos utilizados:** Tiempo y atención del jugador.
    
- **Recompensa:** Apertura de atajos, activación de trampas ambientales, aislamiento de zonas y registro de datos narrativos.
    
- **Penalización:** Vulnerabilidad total a ataques mientras se introduce la combinación.
    
- **Interacciones con otras mecánicas:**
    
    - **Cámara:** La interfaz se despliega sobre la vista isométrica permitiendo monitorear el entorno.
        
    - **Escape de Combate:** Permite sellar puertas de seguridad rápidamente frente a perseguidores.
        
- **Casos límite:**
    
    - Recibir un golpe interrumpe el hackeo de inmediato cancelando la interfaz.
        

### Mecánica 4: Bloqueo Ambiental de Cámara por Detección Enemiga

- - **Propósito:** Intensificar la vulnerabilidad y la claustrofobia en el momento exacto en que el jugador es descubierto, obligándolo a reaccionar bajo la vista de combate.
    
- **Activación:** Automática e inmediata cuando la barra de alerta de **al menos un enemigo** se llena al 100%.
    
- **Reglas:**
    
    - Mientras el jugador esté en **estado de sigilo / no detectado**, conserva la **vista isométrica libre** para analizar el mapa, calcular conos de visión y planificar su ruta.
        
    - En el instante exacto en que un enemigo detecta al personaje (emite el sonido de alerta), la cámara se fija forzosamente en **vista sobre el hombro (_Over-The-Shoulder_)**.
        
    - La entrada manual para volver a la vista isométrica queda inhabilitada mientras exista **al menos un enemigo en estado de alerta/caza** dentro de la sala o área cercana.
        
    - El bloqueo se desactiva y regresa automáticamente a la vista isométrica cuando:
        
        1. El jugador elimina a todos los enemigos que lo habían detectado.
            
        2. El jugador logra escapar, esconderse tras coberturas y romper la línea de visión el tiempo suficiente para que los enemigos pierdan la alerta (vuelvan a estado de patrulla).
            
        3. El jugador cruza una puerta y se retira a una sala adyacente segura.
            
- **Entrada:** N/A.
    
- **Estado inicial:** Vista isométrica de exploración / sigilo.
    
- **Estado final:** Vista sobre el hombro forzada con iluminación centrada de linterna.
    
- **Recursos utilizados:** Eventos del sistema de IA.
    
- **Recompensa:** El jugador puede explorar y pasar salas habitadas en vista panorámica siempre que mantenga el sigilo.
    
- **Penalización:** La pérdida del sigilo se castiga quitando de golpe la visión periférica del entorno justo cuando los enemigos comienzan a acorralarlo.
    
- **Interacciones con otras mecánicas:**
    
    - **Sigilo y Coberturas:** Permite usar cajas y sombras en vista isométrica sin que la cámara interfiera antes de tiempo.
        
    - **Escape de Combate:** Salir de la sala o romper la línea de visión restaura la vista isométrica automáticamente.
        
- **Casos límite:**
    
    - Si un enemigo detecta al jugador a través de una ventana o un pasillo desde otra sala, la cámara solo debe bloquearse si ese enemigo tiene una ruta clara para perseguir al personaje.
        
    - Si el jugador usa la mecánica de _Escape de Agarre con éxito, la cámara se mantiene en vista sobre el hombro mientras el enemigo siga en alerta tras el aturdimiento.
        

### Mecánica 5: Escape de Combate y Retirada Táctica

- **Propósito:** Permitir al jugador romper el combate cuando la situación se vuelve insostenible y retirarse hacia zonas seguras.
    
- **Activación:** Correr hacia la puerta de salida de la sala durante un enfrentamiento.
    
- **Reglas:**
    
    - El jugador puede romper el combate cruzando el marco de una puerta o cerrándola detrás de sí.
        
    - Los enemigos persiguen brevemente o golpean la puerta sellada antes de regresar a su patrulla.
        
    - Sellar una puerta con fusibles o tarjetas corta inmediatamente la persecución.
        
- **Entrada:** Moverse hacia la salida / interactuar con la puerta.
    
- **Estado inicial:** Estado de Alerta/Combate activo.
    
- **Estado final:** Estado de Reagrupación/Sigilo en la sala adyacente.
    
- **Recursos utilizados:** Resistencia para huir; consumibles para sellar puertas.
    
- **Recompensa:** Sobrevivir a combates desfavorables sin perder la vida ni gastar cargas de escape.
    
- **Penalización:** La sala de la que se huyó permanecerá infestada al regresar.
    
- **Interacciones con otras mecánicas:**
    
    - **Cámara:** Al cruzar la puerta y salir del combate, la cámara regresa a la **vista isométrica**.
        
    - **Inventario:** Permite ganar tiempo para abrir el inventario de 6 casillas sin riesgo inmediato.
        
- **Casos límite:**
    
    - Recibir un golpe mientras se interactúa con la puerta cancela la acción de transición.

### Control de Iluminación y Foco de Linterna

**Propósito: 

Permite al jugador inspeccionar zonas oscuras y apuntar con precisión en vista sobre el hombro, creando un dilema táctico entre visibilidad y riesgo de ser detectado.  

**Activación: 

Pulsar el botón dedicado para encender/apagar la linterna, o de forma automática/intensificada al activar el modo de apuntado al hombro.  

**Reglas: 

- La linterna tiene dos estados: Apagada (visibilidad reducida para el jugador, 0% detección por luz) y Encendida (visibilidad clara en el cono de luz, riesgo alto de alerta).  
    

- En vista isométrica, la linterna emite una luz ambiental tenue a 360° o un cono corto frontal.  
    

- En vista al hombro, la luz se concentra en un cono estrecho de largo alcance en la dirección hacia donde apunta el arma/jugador.  
    

- Si el haz de luz ilumina directamente a un enemigo mientras el jugador está en sigilo, la barra de alerta del enemigo subirá el doble de rápido. 
    

**Entrada: 

Pulsar D-Pad Arriba / Teclas de Cruceta o tecla F (para Toggle Encendido/Apagado manual). 

**Estado inicial: 

Linterna en modo estándar o apagada según la zona del laboratorio.  

**Estado final: 

Linterna encendida proyectando luz / apagada para ocultarse en las sombras. 

**Recursos utilizados: 

- Visibilidad del jugador y nivel de exposición ante la IA enemiga.  
    

- Baterías / Cargas de energía (opcional si decides incluir la recolección de baterías por el laboratorio).  
    

**Recompensa: 

Revelar pistas en paredes, códigos de terminales ocultos en la oscuridad y enemigos al acecho antes de que se acerquen.  

**Penalización: 

Revelar tu posición exacta a enemigos distantes si apuntas la luz hacia ellos por error.  

**Interacciones con otras mecánicas: 

- Sigilo: Apagar la linterna permite pasar al lado de enemigos sin llamar su atención mediante el cono visual.  
    

- Bloqueo de Cámara: Si la linterna revela a un enemigo y este te detecta por la luz, se activa automáticamente el Bloqueo de Cámara en Vista al Hombro.  
    

- Transición de Cámara: La linterna cambia automáticamente de ángulo y alcance según la perspectiva activa.  
    

Casos límite: 

- Si la linterna proyecta luz a través de una ranura o rejilla de ventilación, los enemigos al otro lado de la pared pueden detectar el destello y acudir a investigar ese punto.

Lista las mecánicas que complementan la experiencia pero no constituyen el núcleo.


> **Navegación:** [[00 - Índice]] · ← [[01 - Jugabilidad]] · [[03 - Sistemas]] →
