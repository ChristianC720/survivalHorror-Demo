using System;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public static class HackTestSix
{
    private const BindingFlags Any = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

    private static Keyboard keyboard;
    private static Heartbeat.Player.PlayerHack hack;
    private static Heartbeat.Player.PlayerMotor motor;
    private static Heartbeat.Config.GameConfig config;
    private static Heartbeat.Level.SealableDoor oeste;
    private static Heartbeat.Level.SealableDoor vigi;
    private static Heartbeat.Level.SealableDoor almacen;
    private static Heartbeat.Level.SealableDoor finalDoor;
    private static Heartbeat.Level.HackPanel panelAlmacen;
    private static Heartbeat.Level.HackPanel panelVigi;
    private static Heartbeat.Level.HackPanel panelOeste;
    private static Heartbeat.Level.HackPanel panelFinal;
    private static GameObject player;
    private static Transform camTransform;
    private static Camera cam;
    private static Heartbeat.Player.ThirdPersonCamera rig;

    public static object Execute()
    {
        var sb = new StringBuilder();

        try
        {
            player = GameObject.Find("Player");
            hack = player.GetComponent<Heartbeat.Player.PlayerHack>();
            motor = player.GetComponent<Heartbeat.Player.PlayerMotor>();
            var health = player.GetComponent<Heartbeat.Player.PlayerHealth>();
            camTransform = player.transform.Find("Camera");
            cam = camTransform.GetComponent<Camera>();
            rig = camTransform.GetComponent<Heartbeat.Player.ThirdPersonCamera>();

            Transform doorsRoot = GameObject.Find("VS_Piso1").transform.Find("Doors");
            oeste = doorsRoot.Find("Puerta_SalaClaveSecundaria_Oeste").GetComponent<Heartbeat.Level.SealableDoor>();
            vigi = doorsRoot.Find("Puerta_SalaVigilancia").GetComponent<Heartbeat.Level.SealableDoor>();
            almacen = doorsRoot.Find("Puerta_Almacen").GetComponent<Heartbeat.Level.SealableDoor>();
            finalDoor = doorsRoot.Find("Puerta_SalaClaveFinal").GetComponent<Heartbeat.Level.SealableDoor>();

            Transform panelsRoot = GameObject.Find("VS_Piso1").transform.Find("Panels");
            panelAlmacen = panelsRoot.Find("Panel_Almacen").GetComponent<Heartbeat.Level.HackPanel>();
            panelVigi = panelsRoot.Find("Panel_SalaVigilancia").GetComponent<Heartbeat.Level.HackPanel>();
            panelOeste = panelsRoot.Find("Panel_SalaClaveSecundaria").GetComponent<Heartbeat.Level.HackPanel>();
            panelFinal = panelsRoot.Find("Panel_SalaClaveFinal").GetComponent<Heartbeat.Level.HackPanel>();
            config = GetField<Heartbeat.Config.GameConfig>(panelVigi, "config");

            sb.AppendLine($"INICIAL Oeste abierta={!oeste.IsClosed} Vigi abierta={!vigi.IsClosed} Almacen cerrada={almacen.IsClosed} Final cerrada={finalDoor.IsClosed} llave={finalDoor.IsLocked} hacking={hack.IsHacking}");

            // ---------------- A) puerta estandar de Almacen ----------------
            sb.AppendLine("== A) panel de Almacen: 4-5 flechas, abre la hoja ==");
            sb.AppendLine(Mira(panelAlmacen.transform.position, new Vector3(15.4f, 0.05f, 0.2f)));

            if (hack.TryStart())
            {
                sb.AppendLine($"A) INICIO flechas={hack.Sequence.Count} [{Secuencia()}] tipo='{hack.ActivePanelName}' motorBloqueado={motor.IsMovementLocked}");
                FeedKey(WrongKey(hack.Sequence[0]));
                Invoke(hack, "Update");
                ReleaseKeys();
                sb.AppendLine($"A) fallo -> index={hack.SequenceIndex} flash={hack.IsWrongFlash} sigue={hack.IsHacking}");
                Completar(sb, "A");
            }
            else sb.AppendLine("A) TryStart=FALSE");

            for (int i = 0; i < 40; i++) Invoke(almacen, "Update");
            sb.AppendLine($"A) FIN hackeando={hack.IsHacking} motorBloqueado={motor.IsMovementLocked} AlmacenCerrada={almacen.IsClosed} hojaRot={almacen.transform.localRotation.eulerAngles.y:F1} panelUsable={panelAlmacen.IsUsable}");

            // ---------------- B) sello ----------------
            sb.AppendLine("== B) panel de sellado de Vigilancia: 2-3 flechas ==");
            sb.AppendLine(Mira(panelVigi.transform.position, new Vector3(51.4f, 0.05f, 3.8f)));

            if (hack.TryStart())
            {
                sb.AppendLine($"B) INICIO flechas={hack.Sequence.Count} [{Secuencia()}] tipo='{hack.ActivePanelName}' motorBloqueado={motor.IsMovementLocked}");
                FeedKey(KeyFor(hack.Sequence[0]));
                Invoke(hack, "Update");
                ReleaseKeys();
                int acierto = hack.SequenceIndex;
                FeedKey(WrongKey(hack.Sequence[0]));
                Invoke(hack, "Update");
                ReleaseKeys();
                sb.AppendLine($"B) acierto->index={acierto}; fallo->index={hack.SequenceIndex} flash={hack.IsWrongFlash}");
                Completar(sb, "B");
            }
            else sb.AppendLine("B) TryStart=FALSE");

            for (int i = 0; i < 40; i++) Invoke(vigi, "Update");
            sb.AppendLine($"B) FIN cerrada={vigi.IsClosed} sellada={vigi.IsSealed} hojaRot={vigi.transform.localRotation.eulerAngles.y:F1} panelUsable={panelVigi.IsUsable} hackeando={hack.IsHacking}");

            // ---------------- C) E sobre la hoja sellada ----------------
            sb.AppendLine("== C) E sobre la hoja sellada ==");
            sb.AppendLine(Mira(vigi.OpeningCenter, new Vector3(53f, 0.05f, 3.8f)));
            hack.TryStart();
            for (int i = 0; i < 40; i++) Invoke(vigi, "Update");
            sb.AppendLine($"C) FIN cerrada={vigi.IsClosed} sellada={vigi.IsSealed} hojaRot={vigi.transform.localRotation.eulerAngles.y:F1}");

            // ---------------- D) brecha ----------------
            sb.AppendLine("== D) la rompen los golpes ==");
            vigi.Seal();
            for (int i = 0; i < 40; i++) Invoke(vigi, "Update");
            sb.AppendLine($"D) sellada={vigi.IsSealed} cerrada={vigi.IsClosed} brecha={vigi.BreachProgress01:F2}");

            GameObject enemy = GameObject.Find("Enemies/Enemy_Alto_Pasillo");
            object enemyAI = enemy.GetComponent<Heartbeat.AI.EnemyAI>();
            Vector3 guardado = enemy.transform.position;
            enemy.transform.position = new Vector3(53f, 0.05f, 6.2f);
            SetField(enemyAI, "state", Heartbeat.AI.EnemyState.Caza);

            float original = config.SealBreakDuration;
            SetField(config, "sealBreakDuration", 0.2f);
            for (int i = 0; i < 40; i++) Invoke(vigi, "Update");
            sb.AppendLine($"D) con enemigo en Caza: sellada={vigi.IsSealed} cerrada={vigi.IsClosed} rot={vigi.transform.localRotation.eulerAngles.y:F1} brecha={vigi.BreachProgress01:F2}");
            SetField(config, "sealBreakDuration", original);
            enemy.transform.position = guardado;
            SetField(enemyAI, "state", Heartbeat.AI.EnemyState.Patrulla);
            sb.AppendLine($"D) duracion restaurada={config.SealBreakDuration:F1}");

            // ---------------- E) golpe cancela ----------------
            sb.AppendLine("== E) un golpe cancela la secuencia ==");
            sb.AppendLine(Mira(panelOeste.transform.position, new Vector3(48.21f, 0.05f, -2.6f)));
            if (hack.TryStart())
            {
                sb.AppendLine($"E) INICIO hackeando={hack.IsHacking} motorBloqueado={motor.IsMovementLocked} flechas={hack.Sequence.Count}");
                health.ApplyHit(1);
                sb.AppendLine($"E) TRAS GOLPE hackeando={hack.IsHacking} motorBloqueado={motor.IsMovementLocked} golpes={health.CurrentHits}/{health.MaxHits}");
            }
            else sb.AppendLine("E) TryStart=FALSE");

            // ---------------- F) puerta final ----------------
            sb.AppendLine("== F) puerta final con llave ==");
            sb.AppendLine($"F) panelUsable={panelFinal.IsUsable} cerrada={finalDoor.IsClosed} llave={finalDoor.IsLocked}");
            finalDoor.Interact();
            finalDoor.Seal();
            sb.AppendLine($"F) FIN cerrada={finalDoor.IsClosed} sellada={finalDoor.IsSealed}");
        }
        catch (Exception e)
        {
            sb.AppendLine($"EXCEPCION: {(e.InnerException != null ? e.InnerException.ToString() : e.ToString())}");
        }

        return sb.ToString();
    }

    private static string Mira(Vector3 destino, Vector3 posicionJugador)
    {
        var controller = player.GetComponent<CharacterController>();
        controller.enabled = false;
        player.transform.SetPositionAndRotation(posicionJugador, Quaternion.identity);
        controller.enabled = true;

        Vector3 dir = destino - posicionJugador;
        SetField(rig, "yaw", Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg);
        SetField(rig, "pitch", -4f);

        for (int i = 0; i < 10; i++) Invoke(rig, "LateUpdate");

        Invoke(hack, "Update");

        Vector3 viewport = cam.WorldToViewportPoint(destino);
        return $"  mira: jugador={player.transform.position} destino={destino} dist={Vector3.Distance(player.transform.position, destino):F2} cam={camTransform.position} viewport=({viewport.x:F2},{viewport.y:F2},{viewport.z:F2}) target={hack.HasTarget} score={hack.CenterScore:F3} prompt='{hack.Prompt}'";
    }

    private static void Completar(StringBuilder sb, string etiqueta)
    {
        int pulsaciones = 0;
        while (hack.IsHacking && hack.Sequence.Count > 0 && hack.SequenceIndex < hack.Sequence.Count && pulsaciones < 20)
        {
            FeedKey(KeyFor(hack.Sequence[hack.SequenceIndex]));
            Invoke(hack, "Update");
            ReleaseKeys();
            pulsaciones++;
        }

        sb.AppendLine($"{etiqueta}) secuencia completa: hackeando={hack.IsHacking} pulsaciones={pulsaciones}");
    }

    private static string Secuencia()
    {
        var sb = new StringBuilder();
        for (int i = 0; i < hack.Sequence.Count; i++) sb.Append(hack.Sequence[i]).Append(' ');
        return sb.ToString();
    }

    private static Key KeyFor(Heartbeat.Player.HackArrow arrow)
    {
        switch (arrow)
        {
            case Heartbeat.Player.HackArrow.Abajo: return Key.S;
            case Heartbeat.Player.HackArrow.Izquierda: return Key.A;
            case Heartbeat.Player.HackArrow.Derecha: return Key.D;
            default: return Key.W;
        }
    }

    private static Key WrongKey(Heartbeat.Player.HackArrow arrow)
    {
        return arrow == Heartbeat.Player.HackArrow.Abajo ? Key.W : Key.S;
    }

    private static void FeedKey(Key key)
    {
        keyboard = InputSystem.GetDevice<Keyboard>();
        if (keyboard == null) throw new Exception("No hay teclado.");
        InputSystem.QueueStateEvent(keyboard, new KeyboardState(key));
        InputSystem.Update();
    }

    private static void ReleaseKeys()
    {
        if (keyboard == null) return;
        InputSystem.QueueStateEvent(keyboard, new KeyboardState());
        InputSystem.Update();
    }

    private static void Invoke(object target, string methodName)
    {
        MethodInfo method = target.GetType().GetMethod(methodName, Any);
        if (method == null) throw new Exception($"Metodo '{methodName}' no encontrado en {target.GetType().Name}.");
        method.Invoke(target, null);
    }

    private static T GetField<T>(object target, string fieldName)
    {
        FieldInfo field = target.GetType().GetField(fieldName, Any);
        if (field == null) throw new Exception($"Campo '{fieldName}' no encontrado en {target.GetType().Name}.");
        return (T)field.GetValue(target);
    }

    private static void SetField(object target, string fieldName, object value)
    {
        FieldInfo field = target.GetType().GetField(fieldName, Any);
        if (field == null) throw new Exception($"Campo '{fieldName}' no encontrado en {target.GetType().Name}.");
        field.SetValue(target, value);
    }
}
