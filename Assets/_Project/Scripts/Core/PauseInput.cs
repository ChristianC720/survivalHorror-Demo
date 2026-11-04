using Heartbeat.InputRouting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Heartbeat.Core
{
    /// <summary>
    /// Esc / Menu toggles pause (Controles). Lives on the Bootstrap object. The pause menu view (E-10) listens to
    /// GameEvents.PauseChanged; Restart and Quit go through <see cref="PauseService.TryLeavePauseTo"/>.
    /// </summary>
    public sealed class PauseInput : MonoBehaviour
    {
        private InputRouter router;
        private PauseService pause;

        private void Start()
        {
            router = Services.Get<InputRouter>();
            pause = Services.Get<PauseService>();
            router.Actions.System.Pause.performed += OnPause;
        }

        private void OnDestroy()
        {
            if (router != null) router.Actions.System.Pause.performed -= OnPause;
        }

        private void OnPause(InputAction.CallbackContext _)
        {
            if (pause.IsReading || pause.ReadingEndedFrame == Time.frameCount) return;   // Esc closes the reading, not opens the menu
            if (UI.ConfirmDialog.AnyOpen) return;   // Esc cancels the dialog, it does not resume behind it
            if (pause.IsPaused) pause.TryResume();
            else pause.TryPause();
        }
    }
}
