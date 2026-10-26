namespace Heartbeat.Items
{
    /// <summary>Tipo de objeto que ocupa casilla en el inventario.</summary>
    public enum ItemType
    {
        None = 0,

        /// <summary>Cura: 1 por casilla.</summary>
        Heal = 1,

        /// <summary>Munición: hasta 12 balas por casilla; cada recogida da 3.</summary>
        Ammo = 2,

        /// <summary>Punzón: 1 por casilla.</summary>
        Spike = 3,

        /// <summary>Llave: 1 por casilla. Objeto clave: puede reemplazar otro con el inventario lleno.</summary>
        Key = 4
    }

    public static class ItemTypeUtil
    {
        public static string DisplayName(ItemType type)
        {
            switch (type)
            {
                case ItemType.Heal: return "Cura";
                case ItemType.Ammo: return "Munición";
                case ItemType.Spike: return "Punzón";
                case ItemType.Key: return "Llave";
                default: return "—";
            }
        }

        /// <summary>Los objetos clave pueden reemplazar otra casilla cuando el inventario está lleno.</summary>
        public static bool IsKeyItem(ItemType type)
        {
            return type == ItemType.Key;
        }
    }
}
