using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public static class HackTestTwo
{
    private const BindingFlags Any = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

    public static object Execute()
    {
        var sb = new StringBuilder();

        GameObject player = GameObject.Find("Player");
        var hack = player.GetComponent<Heartbeat.Player.PlayerHack>();
        var motor = player.GetComponent<Heartbeat.Player.PlayerMotor>();
        var health = player.GetComponent<Heartbeat.Player.PlayerHealth>();

        Transform doorsRoot = GameObject.Find("VS_Piso1").transform.Find("Doors");
        var oeste = doorsRoot.Find("Puerta_SalaClaveSecundaria_Oeste").GetComponent<Heartbeat.Level.SealableDoor>();
        var vigi = doorsRoot.Find("Puerta_SalaVigilancia").GetComponent<Heartbeat.Level.SealableDoor>();
        var almacen = doorsRoot.Find("Puerta_Almacen").GetComponent<Heartbeat.Level.SealableDoor>();

        sb.AppendLine($"== sesion en curso (Almacen) flechas={hack.Sequence.Count} index={hack.SequenceIndex} hacking={hack.IsHacking} ==");

        // --- A) fallo deliberado: manda la flecha contraria a la que toca y comprueba el reinicio parcial ---
        Key wrongKey = WrongKey(hack.Sequence[0]);
        FeedKey(wrongKey);
        Invoke(hack, "Update");
        sb.AppendLine($"A) fallo con {wrongKey}: index={hack.SequenceIndex} flash={hack.IsWrongFlash} hacking={hack.IsHacking}");
        ReleaseKeys();

        // --- B) secuencia correcta completa ---
        for (int i = 0; i < hack.Sequence.Count; i++)
        {
            Heartbeat.Player.HackArrow expected = hack.Sequence[i];
            FeedKey(KeyFor(expected));
            Invoke(hack, "Update");
            sb.AppendLine($"B) {expected} -> index={hack.SequenceIndex} hacking={hack.IsHacking}");
            ReleaseKeys();
            if (!hack.IsHacking) break;
        }

        sb.AppendLine($"B) tras completar: hacking={hack.IsHacking} motorBloqueado={motor.IsMovementLocked} AlmacenCerrada={almacen.IsClosed} AlmacenSellada={almacen.IsSealed}");

        // --- C) panel de sellado: fallo y reinicio desde el indice 0 ---
        Teleport(player, new Vector3(51.4f, 0.05f, 3.7f), Quaternion.Euler(0f, 0f, 0f));
        RefreshCamera(player);
        Invoke(hack, "Update");
        sb.AppendLine($"C) delante del panel de Vigilancia: HasTarget={hack.HasTarget} Prompt='{hack.Prompt}' score={hack.CenterScore:F3}");

        bool started = hack.TryStart();
        int total = hack.Sequence.Count;
        sb.AppendLine($"C) TryStart={started} flechas={total} tipo='{hack.ActivePanelName}' motorBloqueado={motor.IsMovementLocked}");

        // acierto, luego dos fallos para ver que vuelve a 0
        FeedKey(KeyFor(hack.Sequence[0]));
        Invoke(hack, "Update");
        ReleaseKeys();
        int afterFirst = hack.SequenceIndex;

        FeedKey(WrongKey(hack.Sequence[0]));
        Invoke(hack, "Update");
        ReleaseKeys();
        int afterWrong = hack.SequenceIndex;

        sb.AppendLine($"C) acierto -> index={afterFirst}; fallo -> index={afterWrong} flash={hack.IsWrongFlash}");

        // completar
        for (int i = 0; i < hack.Sequence.Count; i++)
        {
            FeedKey(KeyFor(hack.Sequence[i]));
            Invoke(hack, "Update");
            ReleaseKeys();
            if (!hack.IsHacking) break;
        }

        sb.AppendLine($"C) tras completar: hacking={hack.IsHacking} VigilanciaCerrada={vigi.IsClosed} VigilanciaSellada={vigi.IsSealed} hojaRot={vigi.transform.localRotation.eulerAngles.y:F1}");

        // --- D) E sobre la hoja sellada: levanta el sello ---
        vigi.Interact();
        sb.AppendLine($"D) ligar sello: cerrada={vigi.IsClosed} sellada={vigi.IsSealed} brecha={vigi.BreachProgress01:F2}");

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

    private static Keyboard keyboard;

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
}
