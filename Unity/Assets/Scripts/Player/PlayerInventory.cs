using System;
using Heartbeat.Config;
using Heartbeat.Items;
using Heartbeat.Level;
using UnityEngine;

namespace Heartbeat.Player
{
    /// <summary>Casilla del inventario.</summary>
    public struct InventorySlot
    {
        public ItemType Type;
        public int Amount;

        public bool IsEmpty => Type == ItemType.None || Amount <= 0;
    }

    /// <summary>
    /// Inventario de 6 casillas en tiempo real. Usar y descartar tardan
    /// <see cref="GameConfig.ItemActionDuration"/>; descartar deja el objeto en el suelo.
    /// Con el inventario lleno, un objeto clave puede reemplazar la casilla señalada.
    /// Todas las cifras vienen de <see cref="GameConfig"/>.
    /// </summary>
    public class PlayerInventory : MonoBehaviour
    {
        private enum PendingAction
        {
            None = 0,
            Use = 1,
            Discard = 2
        }

        [Header("Referencias")]
        [SerializeField] private GameConfig config;
        [SerializeField] private PlayerHealth health;

        [Tooltip("Punto de origen de los objetos descartados (CameraTarget).")]
        [SerializeField] private Transform dropOrigin;

        [Tooltip("Prefab del objeto que se deja caer al descartar.")]
        [SerializeField] private WorldItem worldItemPrefab;

        [Header("Depuración")]
        [SerializeField] private bool logEvents = true;

        private InventorySlot[] slots;
        private PendingAction pending;
        private int busySlot = -1;
        private float actionTimer;

        private const float GroundProbeHeight = 2f;

        public int SlotCount => slots != null ? slots.Length : 0;
        public bool IsBusy => busySlot >= 0;

        public float ActionProgress01
        {
            get
            {
                if (!IsBusy || config == null || config.ItemActionDuration <= 0f) return 0f;
                return Mathf.Clamp01(actionTimer / config.ItemActionDuration);
            }
        }

        /// <summary>Casilla bajo el ratón; la usa el reemplazo de objetos clave.</summary>
        public int HoveredSlot { get; set; } = -1;

        /// <summary>Cambió el contenido del inventario.</summary>
        public event Action Changed;

        /// <summary>Mensaje para la UI de depuración.</summary>
        public event Action<string> Status;

        private void Awake()
        {
            if (health == null) health = GetComponent<PlayerHealth>();
            if (dropOrigin == null && transform.parent != null) dropOrigin = transform.Find("CameraTarget");

            int count = config != null ? config.InventorySlots : 6;
            slots = new InventorySlot[Mathf.Max(1, count)];
            HoveredSlot = -1;

            // El revólver arranca descargado; las balas iniciales están en el inventario.
            // Tras una muerte el inventario vuelve VACÍO: no se reparten esas balas.
            bool emptyOnRespawn = PlayerRespawn.IsRespawning;
            if (config != null && config.StartAmmoBullets > 0 && !emptyOnRespawn)
            {
                PlaceInto(ItemType.Ammo, config.StartAmmoBullets, commit: true);
            }
        }

        private void Update()
        {
            if (!IsBusy || config == null) return;

            actionTimer += Time.deltaTime;
            if (actionTimer < config.ItemActionDuration) return;

            int slot = busySlot;
            PendingAction action = pending;

            busySlot = -1;
            pending = PendingAction.None;
            actionTimer = 0f;

            if (action == PendingAction.Discard)
            {
                DropSlot(slot);
            }
            else
            {
                CompleteUse(slot);
            }
        }

        public InventorySlot GetSlot(int index)
        {
            if (slots == null || index < 0 || index >= slots.Length) return default;
            return slots[index];
        }

        public int CountOf(ItemType type)
        {
            if (slots == null) return 0;

            int total = 0;
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i].Type == type) total += slots[i].Amount;
            }
            return total;
        }

        /// <summary>Recoge el objeto del mundo. Con el inventario lleno, un objeto clave reemplaza la casilla señalada.</summary>
        public bool TryPickup(WorldItem item)
        {
            if (item == null || slots == null) return false;

            ItemType type = item.Type;
            int amount = Mathf.Max(1, item.Amount);

            if (PlaceInto(type, amount, commit: false) == 0)
            {
                PlaceInto(type, amount, commit: true);
                item.Consume();
                Report($"Recogido: {ItemTypeUtil.DisplayName(type)} x{amount}");
                Changed?.Invoke();
                return true;
            }

            if (!ItemTypeUtil.IsKeyItem(type))
            {
                Report("Inventario lleno");
                return false;
            }

            if (HoveredSlot < 0 || HoveredSlot >= slots.Length)
            {
                Report("Inventario lleno: señala la casilla a reemplazar");
                return false;
            }

            InventorySlot replaced = slots[HoveredSlot];
            if (!replaced.IsEmpty) SpawnDrop(replaced.Type, replaced.Amount);

            slots[HoveredSlot] = new InventorySlot { Type = type, Amount = Mathf.Min(amount, MaxStack(type)) };
            item.Consume();

            Report(replaced.IsEmpty
                ? $"Guardado: {ItemTypeUtil.DisplayName(type)}"
                : $"Reemplazado {ItemTypeUtil.DisplayName(replaced.Type)} por {ItemTypeUtil.DisplayName(type)}");

            Changed?.Invoke();
            return true;
        }

        /// <summary>Usa la casilla. La cura delega en <see cref="PlayerHealth"/> (1.5 s, cancelable por golpe).</summary>
        public bool Use(int index)
        {
            if (slots == null || index < 0 || index >= slots.Length || slots[index].IsEmpty) return false;

            if (IsBusy)
            {
                Report("Ocupado...");
                return false;
            }

            ItemType type = slots[index].Type;

            if (type == ItemType.Heal)
            {
                if (health == null) return false;

                if (health.State == HealthState.Good)
                {
                    Report("No puedes curarte estando Bien");
                    return false;
                }

                return health.TryStartHeal();
            }

            busySlot = index;
            pending = PendingAction.Use;
            actionTimer = 0f;
            Report($"Usando {ItemTypeUtil.DisplayName(type)}...");
            return true;
        }

        /// <summary>Descarta la casilla: el objeto cae al suelo.</summary>
        public bool Discard(int index)
        {
            if (slots == null || index < 0 || index >= slots.Length || slots[index].IsEmpty) return false;

            if (IsBusy)
            {
                Report("Ocupado...");
                return false;
            }

            busySlot = index;
            pending = PendingAction.Discard;
            actionTimer = 0f;
            Report($"Descartando {ItemTypeUtil.DisplayName(slots[index].Type)}...");
            return true;
        }

        /// <summary>Consume una unidad del tipo indicado (la usa la cura al completarse).</summary>
        public bool ConsumeOne(ItemType type)
        {
            if (slots == null) return false;

            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i].Type != type) continue;

                int left = slots[i].Amount - 1;
                slots[i] = left <= 0
                    ? default
                    : new InventorySlot { Type = type, Amount = left };

                Changed?.Invoke();
                return true;
            }

            return false;
        }

        private void CompleteUse(int index)
        {
            if (slots == null || index < 0 || index >= slots.Length || slots[index].IsEmpty) return;

            ItemType type = slots[index].Type;

            switch (type)
            {
                case ItemType.Key:
                    if (TryUnlockNearbyDoor())
                    {
                        RemoveSlot(index);
                        Report("Puerta desbloqueada con la llave");
                    }
                    else
                    {
                        Report("No hay ninguna puerta cerca");
                    }
                    break;

                case ItemType.Ammo:
                    Report("Munición reservada para el arma");
                    break;

                case ItemType.Spike:
                    Report("Punzón: no hay arma que lo use todavía");
                    break;

                default:
                    Report("No tiene uso");
                    break;
            }
        }

        private bool TryUnlockNearbyDoor()
        {
            if (config == null) return false;

            Door[] doors = UnityEngine.Object.FindObjectsByType<Door>(FindObjectsSortMode.None);
            float best = float.MaxValue;
            Door found = null;

            for (int i = 0; i < doors.Length; i++)
            {
                Door door = doors[i];
                if (door == null || !door.IsLocked) continue;

                float distance = Vector3.Distance(transform.position, door.transform.position);
                if (distance > config.KeyUseRange || distance >= best) continue;

                best = distance;
                found = door;
            }

            if (found == null) return false;

            found.Unlock();
            if (logEvents) Debug.Log($"[Inventario] Llave usada en {found.name} ({best:F1} m).");
            return true;
        }

        private void DropSlot(int index)
        {
            if (slots == null || index < 0 || index >= slots.Length || slots[index].IsEmpty) return;

            InventorySlot slot = slots[index];
            SpawnDrop(slot.Type, slot.Amount);
            slots[index] = default;

            Report($"Descartado: {ItemTypeUtil.DisplayName(slot.Type)}");
            Changed?.Invoke();
        }

        private void SpawnDrop(ItemType type, int amount)
        {
            if (worldItemPrefab == null)
            {
                if (logEvents) Debug.LogWarning("[Inventario] Falta el prefab de objeto para descartar.", this);
                return;
            }

            // La dirección sale de la mirada, pero el objeto reposa en el suelo a los pies:
            // usar la altura del CameraTarget lo dejaría flotando a la altura de los ojos.
            Transform forwardSource = dropOrigin != null ? dropOrigin : transform;
            Vector3 forward = forwardSource.forward;
            forward.y = 0f;
            if (forward.sqrMagnitude < 0.0001f) forward = transform.forward;
            forward.Normalize();

            float distance = config != null ? config.ItemDropDistance : 0.7f;
            float height = config != null ? config.ItemDropHeight : 0.2f;

            Vector3 feet = transform.position;
            Vector3 position = feet + forward * distance;
            position.y = GroundHeightAt(position, feet.y) + height;

            WorldItem dropped = Instantiate(worldItemPrefab, position, Quaternion.identity);
            dropped.Configure(type, amount);
        }

        /// <summary>Altura del suelo bajo el punto; si no lo encuentra, la del jugador.</summary>
        private float GroundHeightAt(Vector3 point, float fallback)
        {
            Vector3 origin = point + Vector3.up * GroundProbeHeight;
            if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit, GroundProbeHeight * 2f, ~0, QueryTriggerInteraction.Ignore))
            {
                return hit.point.y;
            }

            return fallback;
        }

        private void RemoveSlot(int index)
        {
            slots[index] = default;
            Changed?.Invoke();
        }

        /// <summary>Reparte la cantidad en pilas existentes y casillas libres. Devuelve lo que no cabe.</summary>
        private int PlaceInto(ItemType type, int amount, bool commit)
        {
            int remaining = amount;
            int max = MaxStack(type);

            if (max > 1)
            {
                for (int i = 0; i < slots.Length && remaining > 0; i++)
                {
                    if (slots[i].Type != type) continue;

                    int space = max - slots[i].Amount;
                    if (space <= 0) continue;

                    int added = Mathf.Min(space, remaining);
                    if (commit) slots[i] = new InventorySlot { Type = type, Amount = slots[i].Amount + added };
                    remaining -= added;
                }
            }

            for (int i = 0; i < slots.Length && remaining > 0; i++)
            {
                if (!slots[i].IsEmpty) continue;

                int added = Mathf.Min(max, remaining);
                if (commit) slots[i] = new InventorySlot { Type = type, Amount = added };
                remaining -= added;
            }

            return remaining;
        }

        private int MaxStack(ItemType type)
        {
            if (type == ItemType.Ammo) return config != null ? Mathf.Max(1, config.AmmoStackSize) : 12;
            return 1;
        }

        private void Report(string message)
        {
            Status?.Invoke(message);
            if (logEvents) Debug.Log($"[Inventario] {message}");
        }
    }
}
