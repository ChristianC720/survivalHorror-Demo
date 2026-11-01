using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public static class HackTestThree
{
    private const BindingFlags Any = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

    private static Keyboard keyboard;
    private static Heartbeat.Player.PlayerHack hack;
    private static Heartbeat.Player.PlayerMotor motor;
    private static Heartbeat.Config.GameConfig config;

    public static object Execute()
    {
        var sb = new StringBuilder();

        GameObject player = GameObject.Find("Player");
        hack = player.GetComponent<Heartbeat.Player.PlayerHack>();
        motor = player.GetComponent<Heartbeat.Player.PlayerMotor>();
        var health = player.GetComponent<Heartbeat.Player.PlayerHealth>();

        Transform doorsRoot = GameObject.Find("VS_Piso1").transform.Find("Doors");
        var oeste = doorsRoot.Find("Puerta_SalaClaveSecundaria_Oeste").GetComponent<Heartbeat.Level.SealableDoor>();
        var vigi = doorsRoot.Find("Puerta_SalaVigilancia").GetComponent<Heartbeat.Level.SealableDoor>();
        var almacen = doorsRoot.Find("Puerta_Almacen").GetComponent<Heartbeat.Level.SealableDoor>();
        var finalDoor = doorsRoot.Find("Puerta_SalaClaveFinal").GetComponent<Heartbeat.Level.SealableDoor>();

        var panelsRoot = GameObject.Find("VS_Piso1").transform.Find("Panels");
        var panelVigi = panelsRoot.Find("Panel_SalaVigilancia").GetComponent<Heartbeat.Level.HackPanel>();
        var panelOeste = panelsRoot.Find("Panel_SalaClaveSecundaria").GetComponent<Heartbeat.Level.HackPanel>();
        var panelFinal = panelsRoot.Find("Panel_SalaClaveFinal").GetComponent<Heartbeat.Level.HackPanel>();
        config = panelVigi.GetComponent<Heartbeat.Level.HackPanel>() != null
            ? GetField<Heartbeat.Config.GameConfig>(panelVigi, "config")
            : null;

        // ---------------------------------------------------------------
        sb.AppendLine("== A) panel de Almacen: iniciar, fallar y completar ==");
        Teleport(player, new Vector3(15.4f, 0.05f, 0.1f), Quaternion.identity);
        RefreshCamera(player);
        Invoke(hack, "Update");
        sb.AppendLine($"A) detectado={hack.HasTarget} prompt='{hack.Prompt}' score={hack.CenterScore:F3}");

        bool started = hack.TryStart();
        int total = hack.Sequence.Count;
        var seq = new StringBuilder();
        for (int i = 0; i < total; i++) seq.Append(hack.Sequence[i]).Append(' ');
        sb.AppendLine($"A) iniciar={started} flechas={total} [{seq}] motorBloqueado={motor.IsMovementLocked} tipo='{hack.ActivePanelName}'");

        // fallo deliberado en la primera flecha
        FeedKey(WrongKey(hack.Sequence[0]));
        Invoke(hack, "Update");
        ReleaseKeys();
        sb.AppendLine($"A) fallo -> index={hack.SequenceIndex} flash={hack.IsWrongFlash} sigueHackeando={hack.IsHacking}");

        // secuencia correcta
        for (int i = 0; i < hack.Sequence.Count; i++)
        {
            FeedKey(KeyFor(hack.Sequence[i]));
            Invoke(hack, "Update");
            ReleaseKeys();
            if (!hack.IsHacking) break;
        }
        sb.AppendLine($"A) completada: hackeando={hack.IsHacking} motorBloqueado={motor.IsMovementLocked} AlmacenCerrada={almacen.IsClosed} hojaRot={almacen.transform.localRotation.eulerAngles.y:F1}");

        // ---------------------------------------------------------------
        sb.AppendLine("== B) panel de sellado (Vigilancia) ==");
        Teleport(player, new Vector3(51.4f, 0.05f, 3.7f), Quaternion.identity);
        RefreshCamera(player);
        Invoke(hack, "Update");
        sb.AppendLine($"B) detectado={hack.HasTarget} prompt='{hack.Prompt}' score={hack.CenterScore:F3} usable={panelVigi.IsUsable}");

        started = hack.TryStart();
        total = hack.Sequence.Count;
        sb.AppendLine($"B) iniciar={started} flechas={total} tipo='{hack.ActivePanelName}' motorBloqueado={motor.IsMovementLocked}");

        // un acierto, un fallo, y la secuencia completa
        FeedKey(KeyFor(hack.Sequence[0]));
        Invoke(hack, "Update");
        ReleaseKeys();
        int trasAcierto = hack.SequenceIndex;

        FeedKey(WrongKey(hack.Sequence[0]));
        Invoke(hack, "Update");
        ReleaseKeys();
        sb.AppendLine($"B) acierto->index={trasAcierto}; fallo->index={hack.SequenceIndex} flash={hack.IsWrongFlash}");

        for (int i = 0; i < hack.Sequence.Count; i++)
        {
            FeedKey(KeyFor(hack.Sequence[i]));
            Invoke(hack, "Update");
            ReleaseKeys();
            if (!hack.IsHacking) break;
        }

        for (int i = 0; i < 30; i++) Invoke(vigi, "Update");
        sb.AppendLine($"B) completada: hackeando={hack.IsHacking} cerrada={vigi.IsClosed} sellada={vigi.IsSealed} hojaRot={vigi.transform.localRotation.eulerAngles.y:F1} usableAhora={panelVigi.IsUsable}");

        // ---------------------------------------------------------------
        sb.AppendLine("== C) E sobre la hoja sellada: levanta el sello ==");
        vigi.Interact();
        for (int i = 0; i < 30; i++) Invoke(vigi, "Update");
        sb.AppendLine($"C) cerrada={vigi.IsClosed} sellada={vigi.IsSealed} hojaRot={vigi.transform.localRotation.eulerAngles.y:F1}");

        // ---------------------------------------------------------------
        sb.AppendLine("== D) brecha por golpes de enemigo ==");
        vigi.Seal();
        for (int i = 0; i < 30; i++) Invoke(vigi, "Update");
        sb.AppendLine($"D) sellada={vigi.IsSealed} cerrada={vigi.IsClosed} brecha={vigi.BreachProgress01:F2}");

        GameObject enemy = GameObject.Find("Enemies/Enemy_Alto_Pasillo");
        object enemyAI = enemy.GetComponent<Heartbeat.AI.EnemyAI>();
        Vector3 guardado = enemy.transform.position;
        enemy.transform.position = new Vector3(53f, 0.05f, 6.2f);
        SetField(enemyAI, "state", Heartbeat.AI.EnemyState.Caza);

        float originalDuration = config.SealBreakDuration;
        SetField(config, "sealBreakDuration", 0.2f);

        Invoke(vigi, "Update");
        sb.AppendLine($"D) un tick con enemigo en Caza: brecha={vigi.BreachProgress01:F3}");
        for (int i = 0; i < 40; i++) Invoke(vigi, "Update");
        sb.AppendLine($"D) tras insistir: sellada={vigi.IsSealed} cerrada={vigi.IsClosed} rot={vigi.transform.localRotation.eulerAngles.y:F1} brecha={vigi.BreachProgress01:F2}");

        SetField(config, "sealBreakDuration", originalDuration);
        enemy.transform.position = guardado;
        SetField(enemyAI, "state", Heartbeat.AI.EnemyState.Patrulla);
        sb.AppendLine($"D) restaurado: duracion={config.SealBreakDuration:F1}");

        // ---------------------------------------------------------------
        sb.AppendLine("== E) cancelar por golpe ==");
        Teleport(player, new Vector3(48.21f, 0.05f, -2.9f), Quaternion.identity);
        RefreshCamera(player);
        Invoke(hack, "Update");
        sb.AppendLine($"E) detectado={hack.HasTarget} prompt='{hack.Prompt}' usable={panelOeste.IsUsable}");
        started = hack.TryStart();
        sb.AppendLine($"E) iniciar={started} hackeando={hack.IsHacking} motorBloqueado={motor.IsMovementLocked}");
        health.ApplyHit(1);
        sb.AppendLine($"E) tras golpe: hackeando={hack.IsHacking} motorBloqueado={motor.IsMovementLocked}");

        // ---------------------------------------------------------------
        sb.AppendLine("== F) puerta final con llave ==");
        sb.AppendLine($"F) panel usable={panelFinal.IsUsable} cerrada={finalDoor.IsClosed} conLlave={finalDoor.IsLocked}");
        finalDoor.Interact();
        finalDoor.Seal();
        sb.AppendLine($"F) tras E y sello: cerrada={finalDoor.IsClosed} sellada={finalDoor.IsSealed}");

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

    private static void Teleport(GameObject player, Vector3 position, Quaternion rotation)
    {
        var controller = player.GetComponent<CharacterController>();
        if (controller != null) controller.enabled = false;
        player.transform.SetPositionAndRotation(position, rotation);
        if (controller != null) controller.enabled = true;
    }

    private static void RefreshCamera(GameObject player)
    {
        Transform camTransform = player.transform.Find("Camera");
        if (camTransform == null) return;
        var rig = camTransform.GetComponent<Heartbeat.Player.ThirdPersonCamera>();
        if (rig == null) return;
        Invoke(rig, "Update");
        Invoke(rig, "LateUpdate");
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
