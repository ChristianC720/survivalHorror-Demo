using Heartbeat.Config;
using UnityEngine;

namespace Heartbeat.Items
{
    /// <summary>
    /// Objeto recogible en el mundo. Se detecta con el rayo de <see cref="Heartbeat.Player.PlayerPickup"/>
    /// y se guarda en el inventario. Puede dejarse caer de nuevo (descartar).
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class WorldItem : MonoBehaviour
    {
        [SerializeField] private GameConfig config;
        [SerializeField] private ItemType type = ItemType.Heal;
        [SerializeField] private int amount = 1;

        private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");
        private static readonly int ColorId = Shader.PropertyToID("_Color");
        private static readonly int EmissionColorId = Shader.PropertyToID("_EmissionColor");

        private MeshRenderer meshRenderer;

        public ItemType Type => type;
        public int Amount => amount;
        public string DisplayName => ItemTypeUtil.DisplayName(type);

        private void Awake()
        {
            Collider own = GetComponent<Collider>();
            if (own != null) own.isTrigger = true;
        }

        private void Start()
        {
            ApplyVisuals();
        }

        public void Configure(ItemType newType, int newAmount)
        {
            type = newType;
            amount = Mathf.Max(1, newAmount);
            ApplyVisuals();
        }

        /// <summary>El objeto ha sido recogido: desaparece del mundo.</summary>
        public void Consume()
        {
            Destroy(gameObject);
        }

        private void ApplyVisuals()
        {
            if (config == null) return;

            if (meshRenderer == null) meshRenderer = GetComponent<MeshRenderer>();
            if (meshRenderer == null) return;

            Color color = config.GetItemColor(type);

            // Instancia propia del material: no toca el material compartido del proyecto.
            Material material = meshRenderer.material;
            if (material.HasProperty(BaseColorId)) material.SetColor(BaseColorId, color);
            if (material.HasProperty(ColorId)) material.SetColor(ColorId, color);
            if (material.HasProperty(EmissionColorId))
            {
                material.EnableKeyword("_EMISSION");
                material.SetColor(EmissionColorId, color * 0.9f);
            }

            meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            meshRenderer.receiveShadows = false;
        }
    }
}
