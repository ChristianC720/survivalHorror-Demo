public class VigInput
{
    public static string ExecuteDamage()
    {
        UnityEngine.GameObject player = UnityEngine.GameObject.Find("Player");
        Heartbeat.Player.SurveillanceView view = player.GetComponent<Heartbeat.Player.SurveillanceView>();
        Heartbeat.Player.PlayerHealth health = player.GetComponent<Heartbeat.Player.PlayerHealth>();

        bool antes = view.IsActive;
        health.ApplyHit(1);

        return "[Aura] Daño aplicado. Vista antes=" + antes + " ahora=" + view.IsActive
            + " | golpes=" + health.CurrentHits + "/" + health.MaxHits + " estado=" + health.State;
    }

    public static string ExecuteAim()
    {
        UnityEngine.InputSystem.Mouse mouse = UnityEngine.InputSystem.Mouse.current;
        if (mouse == null) return "[Aura] No hay ratón.";

        UnityEngine.InputSystem.LowLevel.MouseState state = new UnityEngine.InputSystem.LowLevel.MouseState();
        state.WithButton(UnityEngine.InputSystem.LowLevel.MouseButton.Right);
        UnityEngine.InputSystem.InputSystem.QueueStateEvent(mouse, state);

        return "[Aura] Clic derecho encolado (apuntar).";
    }

    public static string ExecuteRun()
    {
        UnityEngine.InputSystem.Keyboard keyboard = UnityEngine.InputSystem.Keyboard.current;
        if (keyboard == null) return "[Aura] No hay teclado.";

        UnityEngine.InputSystem.LowLevel.KeyboardState state = new UnityEngine.InputSystem.LowLevel.KeyboardState(
            UnityEngine.InputSystem.Key.LeftShift, UnityEngine.InputSystem.Key.W);
        UnityEngine.InputSystem.InputSystem.QueueStateEvent(keyboard, state);

        return "[Aura] Shift+W encolado (correr).";
    }

    public static string ExecuteWalk()
    {
        UnityEngine.InputSystem.Keyboard keyboard = UnityEngine.InputSystem.Keyboard.current;
        if (keyboard == null) return "[Aura] No hay teclado.";

        UnityEngine.InputSystem.InputSystem.QueueStateEvent(keyboard,
            new UnityEngine.InputSystem.LowLevel.KeyboardState(UnityEngine.InputSystem.Key.W));

        return "[Aura] W encolado (caminar).";
    }

    public static string ExecuteProbe()
    {
        UnityEngine.GameObject player = UnityEngine.GameObject.Find("Player");
        Heartbeat.Player.PlayerMotor motor = player.GetComponent<Heartbeat.Player.PlayerMotor>();
        Heartbeat.Player.SurveillanceView view = player.GetComponent<Heartbeat.Player.SurveillanceView>();
        Heartbeat.Player.PlayerAim aim = player.GetComponent<Heartbeat.Player.PlayerAim>();

        return "[Aura] vista=" + view.IsActive + " | corriendo=" + motor.IsRunning + " quiereCorrer=" + motor.WantsSprint
            + " velocidad=" + motor.CurrentSpeed.ToString("F3") + " | apuntando=" + aim.IsAiming;
    }

    public static string ExecuteRelease()
    {
        UnityEngine.InputSystem.Mouse mouse = UnityEngine.InputSystem.Mouse.current;
        if (mouse != null)
        {
            UnityEngine.InputSystem.InputSystem.QueueStateEvent(mouse, new UnityEngine.InputSystem.LowLevel.MouseState());
        }

        UnityEngine.InputSystem.Keyboard keyboard = UnityEngine.InputSystem.Keyboard.current;
        if (keyboard != null)
        {
            UnityEngine.InputSystem.InputSystem.QueueStateEvent(keyboard, new UnityEngine.InputSystem.LowLevel.KeyboardState());
        }

        UnityEngine.GameObject player = UnityEngine.GameObject.Find("Player");
        Heartbeat.Player.PlayerMotor motor = player.GetComponent<Heartbeat.Player.PlayerMotor>();
        Heartbeat.Player.PlayerAim aim = player.GetComponent<Heartbeat.Player.PlayerAim>();

        return "[Aura] Entradas liberadas. motor.IsRunning=" + motor.IsRunning
            + " WantsSprint=" + motor.WantsSprint + " aim.IsAiming=" + aim.IsAiming;
    }
}
