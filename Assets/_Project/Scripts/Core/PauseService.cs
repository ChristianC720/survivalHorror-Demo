using UnityEngine;

namespace Heartbeat.Core
{
    /// <summary>
    /// The only code allowed to write Time.timeScale (GDD global rule 1: only Pause and reading a document or poster
    /// stop time, D-159; inventory, the terminal's lists and hacking never do).
    /// </summary>
    public sealed class PauseService
    {
        private readonly GameStateMachine states;

        public PauseService(GameStateMachine states)
        {
            this.states = states;
        }

        public bool IsPaused => states.Current == GameStateId.Paused;

        public bool TryPause()
        {
            if (!states.TryTransition(GameStateId.Paused)) return false;
            Time.timeScale = 0f;
            AudioListener.pause = true;
            GameEvents.RaisePauseChanged(true);
            HbLog.Info("Pause", "Paused");
            return true;
        }

        public bool TryResume()
        {
            if (!states.TryResume()) return false;
            Time.timeScale = 1f;
            AudioListener.pause = false;
            GameEvents.RaisePauseChanged(false);
            HbLog.Info("Pause", "Resumed");
            return true;
        }

        public bool IsReading { get; private set; }
        /// <summary>The key that closes a reading also fires interact and pause in that frame: they must ignore it.</summary>
        public int ReadingEndedFrame { get; private set; } = -1;

        /// <summary>D-159: stops the world while a document or poster is on screen, without the pause menu or a state change.</summary>
        public void BeginReading()
        {
            if (IsReading) return;
            IsReading = true;
            Time.timeScale = 0f;
            AudioListener.pause = true;
        }

        public void EndReading()
        {
            if (!IsReading) return;
            IsReading = false;
            ReadingEndedFrame = Time.frameCount;
            Time.timeScale = 1f;
            AudioListener.pause = false;
        }

        /// <summary>Leaves pause towards another state (Restart → Death, Quit → MainMenu) and restores time.</summary>
        public bool TryLeavePauseTo(GameStateId target)
        {
            if (!IsPaused || !states.TryTransition(target)) return false;
            Time.timeScale = 1f;
            AudioListener.pause = false;
            GameEvents.RaisePauseChanged(false);
            return true;
        }

        /// <summary>Safety net when Bootstrap starts or a scene is reloaded.</summary>
        public static void ResetTime()
        {
            Time.timeScale = 1f;
            AudioListener.pause = false;
        }
    }
}
