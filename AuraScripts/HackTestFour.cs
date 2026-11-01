using System;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public static class HackTestFour
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
    private static Heartbeat.Level.HackPanel panelVigi;
    private static Heartbeat.Level.HackPanel panelOeste;
    private static Heartbeat.Level.HackPanel panelFinal;
    private static Heartbeat.Level.HackPanel panelAlmacen;
    private static GameObject player;

    public static object Execute()
    {
        var sb = new StringBuilder();

        try
        {
            player = GameObject.Find("Player");
            hack = player.GetComponent<Heartbeat.Player.PlayerHack>();
            motor = player.GetComponent<Heartbeat.Player.PlayerMotor>();
            var health = player.GetComponent<Heartbeat.Player.PlayerHealth>();

            Transform doorsRoot = GameObject.Find("VS_Piso1").transform.Find("Doors");
            oeste = doorsRoot.Find("Puerta_SalaClaveSecundaria_Oeste").GetComponent<Heartbeat.Level.SealableDoor>();
            vigi = doorsRoot.Find("Puerta_SalaVigilancia").GetComponent<Heartbeat.Level.SealableDoor>();
            almacen = doorsRoot.Find("Puerta_Almacen").GetComponent<Heartbeat.Level.SealableDoor>();
            finalDoor = doorsRoot.Find("Puerta_SalaClaveFinal").GetComponent<Heartbeat.Level.SealableDoor>();

            Transform panelsRoot = GameObject.Find("VS_Piso1").transform.Find("Panels");
            panelVigi = panelsRoot.Find("Panel_SalaVigilancia").GetComponent<Heartbeat.Level.HackPanel>();
            panelOeste = panelsRoot.Find("Panel_SalaClaveSecundaria").GetComponent<Heartbeat.Level.HackPanel>();
            panelFinal = panelsRoot.Find("Panel_SalaClaveFinal").GetComponent<Heartbeat.Level.HackPanel>();
            panelAlmacen = panelsRoot.Find("Panel_Almacen").GetComponent<Heartbeat.Level.HackPanel>();
            config = GetField<Heartbeat.Config.GameConfig>(panelVigi, "config");

            sb.AppendLine($"ESTADO INICIAL hacking={hack.IsHacking} motorBloqueado={motor.IsMovementLocked} Oeste={oeste.IsClosed}/{oeste.IsSealed} Vigi={vigi.IsClosed}/{vigi.IsSealed} Almacen={almacen.IsClosed}/{almacen.IsSealed} Final={finalDoor.IsClosed}/{finalDoor.IsLocked}");

            // ---------------- A) panel de Almacen ----------------
            sb.AppendLine("== A) panel de Almacen (puerta estandar) ==");
            Posicionate(new Vector3(15.4f, 0.05f, 0.1f));
            sb.AppendLine($"A) detectado={hack.HasTarget} prompt='{hack.Prompt}' score={hack.CenterScore:F3} usable={panelAlmacen.IsUsable}");

            if (hack.TryStart())
            {
                sb.AppendLine($"A) iniciar OK flechas={hack.Sequence.Count} tipo='{hack.ActivePanelName}' motorBloqueado={motor.IsMovementLocked}");
                sb.AppendLine($"A) secuencia=[{Secuencia()}]");

                if (hack.Sequence.Count > 0)
                {
                    FeedKey(WrongKey(hack.Sequence[0]));
                    Invoke(hack, "Update");
                    ReleaseKeys();
                    sb.AppendLine($"A) fallo -> index={hack.SequenceIndex} flash={hack.IsWrongFlash} sigue={hack.IsHacking}");

                    Completar(sb, "A");
                }
            }
            else sb.AppendLine("A) TryStart=FALSE");

            for (int i = 0; i < 30; i++) Invoke(almacen, "Update");
            sb.AppendLine($"A) resultado: hackeando={hack.IsHacking} motorBloqueado={motor.IsMovementLocked} AlmacenCerrada={almacen.IsClosed} hojaRot={almacen.transform.localRotation.eulerAngles.y:F1} usableAhora={panelAlmacen.IsUsable}");

            // ---------------- B) panel de sellado ----------------
            sb.AppendLine("== B) panel de sellado (Vigilancia) ==");
            Posicionate(new Vector3(51.4f, 0.05f, 3.7f));
            sb.AppendLine($"B) detectado={hack.HasTarget} prompt='{hack.Prompt}' score={hack.CenterScore:F3} usable={panelVigi.IsUsable}");

            if (hack.TryStart())
            {
                sb.AppendLine($"B) iniciar OK flechas={hack.Sequence.Count} tipo='{hack.ActivePanelName}' motorBloqueado={motor.IsMovementLocked} secuencia=[{Secuencia()}]");

                if (hack.Sequence.Count > 0)
                {
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
            }
            else sb.AppendLine("B) TryStart=FALSE");

            for (int i = 0; i < 60; i++) Invoke(vigi, "Update");
            sb.AppendLine($"B) resultado: hackeando={hack.IsHacking} cerrada={vigi.IsClosed} sellada={vigi.IsSealed} hojaRot={vigi.transform.localRotation.eulerAngles.y:F1} usableAhora={panelVigi.IsUsable} motorBloqueado={motor.IsMovementLocked}");

            // ---------------- C) E sobre la hoja sellada ----------------
            sb.AppendLine("== C) E sobre la hoja sellada ==");
            vigi.Interact();
            for (int i = 0; i < 60; i++) Invoke(vigi, "Update");
            sb.AppendLine($"C) cerrada={vigi.IsClosed} sellada={vigi.IsSealed} hojaRot={vigi.transform.localRotation.eulerAngles.y:F1}");

            // ---------------- D) brecha por golpes ----------------
            sb.AppendLine("== D) brecha por golpes de enemigo ==");
            vigi.Seal();
            for (int i = 0; i < 60; i++) Invoke(vigi, "Update");
            sb.AppendLine($"D) sellada={vigi.IsSealed} cerrada={vigi.IsClosed} brecha={vigi.BreachProgress01:F2}");

            GameObject enemy = GameObject.Find("Enemies/Enemy_Alto_Pasillo");
            object enemyAI = enemy.GetComponent<Heartbeat.AI.EnemyAI>();
            Vector3 guardado = enemy.transform.position;
            enemy.transform.position = new Vector3(53f, 0.05f, 6.2f);
            SetField(enemyAI, "state", Heartbeat.AI.EnemyState.Caza);

            float original = config.SealBreakDuration;
            SetField(config, "sealBreakDuration", 0.2f);

            Invoke(vigi, "Update");
            sb.AppendLine($"D) enemigo en Caza pegado al vano: brecha={vigi.BreachProgress01:F3}");
            for (int i = 0; i < 40; i++) Invoke(vigi, "Update");
            sb.AppendLine($"D) tras insistir: sellada={vigi.IsSealed} cerrada={vigi.IsClosed} rot={vigi.transform.localRotation.eulerAngles.y:F1} brecha={vigi.BreachProgress01:F2}");

            SetField(config, "sealBreakDuration", original);
            enemy.transform.position = guardado;
            SetField(enemyAI, "state", Heartbeat.AI.EnemyState.Patrulla);
            sb.AppendLine($"D) restaurado duracion={config.SealBreakDuration:F1}");

            // ---------------- E) cancelar por golpe ----------------
            sb.AppendLine("== E) cancelar por golpe ==");
            Posicionate(new Vector3(48.21f, 0.05f, -2.9f));
            sb.AppendLine($"E) detectado={hack.HasTarget} prompt='{hack.Prompt}' usable={panelOeste.IsUsable}");
            if (hack.TryStart())
            {
                sb.AppendLine($"E) iniciar OK hackeando={hack.IsHacking} motorBloqueado={motor.IsMovementLocked} flechas={hack.Sequence.Count}");
                health.ApplyHit(1);
                sb.AppendLine($"E) tras golpe: hackeando={hack.IsHacking} motorBloqueado={motor.IsMovementLocked}");
            }
            else sb.AppendLine("E) TryStart=FALSE");

            // ---------------- F) puerta final con llave ----------------
            sb.AppendLine("== F) puerta final con llave ==");
            sb.AppendLine($"F) panel usable={panelFinal.IsUsable} cerrada={finalDoor.IsClosed} conLlave={finalDoor.IsLocked}");
            finalDoor.Interact();
            finalDoor.Seal();
            sb.AppendLine($"F) tras E y sello: cerrada={finalDoor.IsClosed} sellada={finalDoor.IsSealed}");
        }
        catch (Exception e)
        {
            sb.AppendLine($"EXCEPCION: {(e.InnerException != null ? e.InnerException.ToString() : e.ToString())}");
        }

        return sb.ToString();
    }

    private static void Completar(StringBuilder sb, string etiqueta)
    {
        int guardas = 0;
        while (hack.IsHacking && hack.Sequence.Count > 0 && hack.SequenceIndex < hack.Sequence.Count && guardas < 20)
        {
            Heartbeat.Player.HackArrow arrow = hack.Sequence[hack.SequenceIndex];
            FeedKey(KeyFor(arrow));
            Invoke(hack, "Update");
            ReleaseKeys();
            guardas++;
        }

        sb.AppendLine($"{etiqueta}) secuencia completa: hackeando={hack.IsHacking} pulsaciones={guardas}");
    }

    private static string Secuencia()
    {
        var sb = new StringBuilder();
        for (int i = 0; i < hack.Sequence.Count; i++) sb.Append(hack.Sequence[i]).Append(' ');
        return sb.ToString();
    }

    private static void Posicionate(Vector3 position)
    {
        var controller = player.GetComponent<CharacterController>();
        if (controller != null) controller.enabled = false;
        player.transform.SetPositionAndRotation(position, Quaternion.identity);
        if (controller != null) controller.enabled = true;

        Transform camTransform = player.transform.Find("Camera");
        if (camTransform != null)
        {
            var rig = camTransform.GetComponent<Heartbeat.Player.ThirdPersonCamera>();
            if (rig != null)
            {
                Invoke(rig, "Update");
                Invoke(rig, "LateUpdate");
            }
        }

        Invoke(hack, "Update");
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
