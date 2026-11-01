using TMPro;
using UnityEngine;

namespace Heartbeat.Combat
{
    /// <summary>Green round counter on the revolver frame (D-74, D-86): the only ammo readout, no HUD.</summary>
    public sealed class RevolverCounter : MonoBehaviour
    {
        [SerializeField] private Revolver revolver;
        [SerializeField] private TMP_Text label;

        private void Start()
        {
            revolver.Model.LoadedChanged += Show;
            Show(revolver.Model.Loaded);
        }

        private void OnDestroy()
        {
            if (revolver != null && revolver.Model != null) revolver.Model.LoadedChanged -= Show;
        }

        private void Show(int loaded) => label.text = loaded.ToString();
    }
}
