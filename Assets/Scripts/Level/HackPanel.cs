using System.Collections.Generic;
using Heartbeat.Config;
using UnityEngine;

namespace Heartbeat.Level
{
    /// <summary>Qué hace un panel cuando la secuencia de flechas se completa.</summary>
    public enum HackPanelKind
    {
        /// <summary>Sella la puerta: los enemigos no pasan hasta que la cruces o la rompan.</summary>
        Sellado = 0,

        /// <summary>Puerta estándar: abre la hoja hasta el final del piso.</summary>
        PuertaEstandar = 1,

        /// <summary>Escalera de la sala final (7-8 flechas).</summary>
        Escalera = 2,
    }

    /// <summary>
    /// Panel de hackeo: cubo en la pared junto a una puerta. La secuencia de flechas la genera
    /// el panel con la longitud que marca <see cref="GameConfig"/> según su tipo, y al completarse
    /// aplica su efecto sobre <see cref="SealableDoor"/>. Todas las cifras vienen de
    /// <see cref="GameConfig"/>.
    /// </summary>
    public class HackPanel : MonoBehaviour
    {
        [Header("Referencias")]
        [SerializeField] private GameConfig config;
        [SerializeField] private HackPanelKind kind = HackPanelKind.Sellado;

        [Tooltip("Puerta que sella o abre este panel.")]
        [SerializeField] private SealableDoor door;

        [Header("Depuración")]
        [SerializeField] private bool logEvents = true;

        private static readonly List<HackPanel> Registry = new List<HackPanel>();

        /// <summary>Paneles de la escena (registro estático, purgado en OnEnable).</summary>
        public static IReadOnlyList<HackPanel> All => Registry;

        public HackPanelKind Kind => kind;
        public SealableDoor Door => door;

        /// <summary>Nombre de la acción, para el aviso "[E] ..." en pantalla.</summary>
        public string ActionName
        {
            get
            {
                switch (kind)
                {
                    case HackPanelKind.PuertaEstandar: return "Abrir puerta";
                    case HackPanelKind.Escalera: return "Abrir escalera";
                    default: return "Sellar puerta";
                }
            }
        }

        /// <summary>Queda algo que hacer con este panel.</summary>
        public bool IsUsable
        {
            get
            {
                if (door == null) return kind == HackPanelKind.Escalera;
                if (door.IsLocked) return false;

                return kind == HackPanelKind.PuertaEstandar ? door.IsClosed : !door.IsSealed;
            }
        }

        private void OnEnable()
        {
            Registry.RemoveAll(item => item == null);
            if (!Registry.Contains(this)) Registry.Add(this);
        }

        private void OnDisable()
        {
            Registry.Remove(this);
        }

        /// <summary>Longitud de la secuencia para este tipo de panel (sorteada en cada intento).</summary>
        public int RollSequenceLength()
        {
            if (config == null) return 1;

            switch (kind)
            {
                case HackPanelKind.PuertaEstandar:
                    return Roll(config.HackPuertaMin, config.HackPuertaMax);
                case HackPanelKind.Escalera:
                    return Roll(config.HackEscaleraMin, config.HackEscaleraMax);
                default:
                    return Roll(config.HackSelladoMin, config.HackSelladoMax);
            }
        }

        /// <summary>Efecto de la secuencia completada.</summary>
        public void ApplyEffect()
        {
            if (logEvents) Debug.Log($"[Panel] Secuencia completada en {name}.");

            if (door == null)
            {
                if (logEvents) Debug.Log($"[Panel] {name} todavía no tiene puerta cableada.");
                return;
            }

            if (kind == HackPanelKind.PuertaEstandar)
            {
                door.OpenByHack();
                return;
            }

            if (kind == HackPanelKind.Sellado)
            {
                door.Seal();
                return;
            }

            if (logEvents) Debug.Log($"[Panel] {name} es de escalera: sin efecto cableado todavía.");
        }

        private static int Roll(int min, int max)
        {
            int low = Mathf.Min(min, max);
            int high = Mathf.Max(min, max);
            return Random.Range(low, high + 1);
        }
    }
}
