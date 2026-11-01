using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public static class HackTestOne
{
    private const BindingFlags Any = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

    public static object Execute()
    {
        var sb = new StringBuilder();

        GameObject player = GameObject.Find("Player");
        if (player == null) return "ERROR: no hay Player.";

        var hack = player.GetComponent<Heartbeat.Player.PlayerHack>();
        var motor = player.GetComponent<Heartbeat.Player.PlayerMotor>();
        if (hack == null || motor == null) return "ERROR: faltan componentes en Player.";

        Transform doorsRoot = GameObject.Find("VS_Piso1").transform.Find("Doors");
        var oeste = doorsRoot.Find("Puerta_SalaClaveSecundaria_Oeste").GetComponent<Heartbeat.Level.SealableDoor>();
        var vigi = doorsRoot.Find("Puerta_SalaVigilancia").GetComponent<Heartbeat.Level.SealableDoor>();
        var almacen = doorsRoot.Find("Puerta_Almacen").GetComponent<Heartbeat.Level.SealableDoor>();
        var finalDoor = doorsRoot.Find("Puerta_SalaClaveFinal").GetComponent<Heartbeat.Level.SealableDoor>();

        sb.AppendLine("== estado inicial ==");
        sb.AppendLine($"Oeste abierta={!oeste.IsClosed} | Vigilancia abierta={!vigi.IsClosed} | Almacen cerrada={almacen.IsClosed} | Final cerrada={finalDoor.IsClosed} conLlave={finalDoor.IsLocked}");
        sb.AppendLine($"Final centro vano={finalDoor.OpeningCenter} | Almacen centro vano={almacen.OpeningCenter} | Vigilancia centro vano={vigi.OpeningCenter}");
        sb.AppendLine($"hoja Oeste rot={oeste.transform.localRotation.eulerAngles} | hoja Almacen rot={almacen.transform.localRotation.eulerAngles}");

        // Colocar al jugador delante del panel de Almacén (Pasillo, z=0.1) y refrescar la cámara.
        Teleport(player, new Vector3(15.4f, 0.05f, 0.1f), Quaternion.identity);
        RefreshCamera(player);

        Invoke(hack, "Update");

        sb.AppendLine("== deteccion ==");
        sb.AppendLine($"HasTarget={hack.HasTarget} Prompt='{hack.Prompt}' CenterScore={hack.CenterScore:F3}");

        bool started = hack.TryStart();
        sb.AppendLine($"TryStart={started} IsHacking={hack.IsHacking} motorBloqueado={motor.IsMovementLocked} flechas={hack.Sequence.Count} tipo='{hack.ActivePanelName}'");

        var sb2 = new StringBuilder();
        for (int i = 0; i < hack.Sequence.Count; i++) sb2.Append(hack.Sequence[i]).Append(' ');
        sb.AppendLine($"secuencia=[ {sb2}] index={hack.SequenceIndex}");

        return sb.ToString();
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
