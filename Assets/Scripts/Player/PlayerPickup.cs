using Heartbeat.Config;
using Heartbeat.Items;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Heartbeat.Player
{
    /// <summary>
    /// Recogida con E: lanza un rayo desde la cámara, acepta el objeto apuntado si está
    /// a menos de <see cref="GameConfig.PickupDistance"/> del jugador y lo guarda en el
    /// inventario. Expone <see cref="CurrentTarget"/> para el aviso "E" en pantalla.
    /// </summary>
    public class PlayerPickup : MonoBehaviour
    {
        [Header("Referencias")]
        [SerializeField] private GameConfig config;
        [SerializeField] private InputActionAsset inputActions;
        [SerializeField] private PlayerInventory inventory;
        [SerializeField] private Camera viewCamera;
        [SerializeField] private Transform playerRoot;
        [Tooltip("Remate de cuerpos: si el cuerpo apuntado está más centrado, la E es suya.")]
        [SerializeField] private PlayerFinisher finisher;

        [Tooltip("Hackeo: la E inicia la secuencia en el panel o levanta el sello de la hoja.")]
        [SerializeField] private PlayerHack hack;

        [Header("Depuración")]
        [SerializeField] private bool logEvents = true;

        private const string ActionMapName = "Player";
        private const float RayLength = 40f;

        private InputAction interactAction;
        private readonly RaycastHit[] hitBuffer = new RaycastHit[16];

        /// <summary>Objeto apuntado y a distancia de recogida (o null).</summary>
        public WorldItem CurrentTarget { get; private set; }

        private void Awake()
        {
            if (inventory == null) inventory = GetComponent<PlayerInventory>();
            if (viewCamera == null) viewCamera = GetComponentInChildren<Camera>();
            if (playerRoot == null) playerRoot = transform;
            if (finisher == null) finisher = GetComponent<PlayerFinisher>();
            if (hack == null) hack = GetComponent<PlayerHack>();
        }

        private void OnEnable()
        {
            if (inputActions == null) return;

            InputActionMap map = inputActions.FindActionMap(ActionMapName, throwIfNotFound: false);
            if (map == null) return;

            interactAction = map.FindAction("Interact", throwIfNotFound: false);
            if (interactAction == null)
            {
                Debug.LogWarning("[Recogida] Falta la acción 'Interact' en el mapa Player.", this);
                return;
            }

            interactAction.Enable();
            interactAction.performed += OnInteractPerformed;
        }

        private void OnDisable()
        {
            if (interactAction != null)
            {
                interactAction.performed -= OnInteractPerformed;
                interactAction.Disable();
            }
            interactAction = null;
        }

        private void Update()
        {
            DetectTarget();
        }

        private void OnInteractPerformed(InputAction.CallbackContext context)
        {
            // Con una secuencia de hackeo en curso, la E la abandona.
            if (hack != null && hack.IsHacking)
            {
                hack.Abort("E");
                return;
            }

            // El panel gana la E si está más centrado que el cuerpo a rematar.
            bool hackFirst = hack != null && hack.HasTarget &&
                             (finisher == null || !finisher.HasTarget || hack.CenterScore <= finisher.CenterScore);

            if (hackFirst && hack.TryStart()) return;

            // El remate con punzón manda sobre la recogida cuando el cuerpo está más centrado.
            if (finisher != null && finisher.TrySpike()) return;
            if (finisher != null && finisher.IsBusy) return;

            if (hack != null && hack.TryStart()) return;

            if (CurrentTarget == null) return;
            if (inventory == null) return;

            if (inventory.TryPickup(CurrentTarget))
            {
                if (logEvents) Debug.Log($"[Recogida] {CurrentTarget.DisplayName} guardado en el inventario.");
                CurrentTarget = null;
            }
        }

        private void DetectTarget()
        {
            CurrentTarget = null;

            if (viewCamera == null || config == null) return;

            Transform cameraTransform = viewCamera.transform;
            Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);

            int count = Physics.RaycastNonAlloc(ray, hitBuffer, RayLength, ~0, QueryTriggerInteraction.Collide);

            // Primero la distancia del primer obstáculo sólido: no se recoge a través de muros.
            // Los triggers (volúmenes de sala, recogibles) no bloquean: la cámara OTS queda por
            // encima del techo del RoomVolume y anularía cualquier recogida a ras de suelo.
            float blockDistance = float.MaxValue;
            for (int i = 0; i < count; i++)
            {
                Collider collider = hitBuffer[i].collider;
                if (collider == null) continue;
                if (collider.transform.root == playerRoot.root) continue;
                if (collider.isTrigger) continue;
                if (collider.GetComponentInParent<WorldItem>() != null) continue;

                blockDistance = Mathf.Min(blockDistance, hitBuffer[i].distance);
            }

            float best = float.MaxValue;
            for (int i = 0; i < count; i++)
            {
                Collider collider = hitBuffer[i].collider;
                if (collider == null) continue;
                if (collider.transform.root == playerRoot.root) continue;
                if (hitBuffer[i].distance >= blockDistance) continue;
                if (hitBuffer[i].distance >= best) continue;

                WorldItem item = collider.GetComponentInParent<WorldItem>();
                if (item == null) continue;

                if (Vector3.Distance(playerRoot.position, item.transform.position) > config.PickupDistance) continue;

                best = hitBuffer[i].distance;
                CurrentTarget = item;
            }

            // Si hay un cuerpo al alcance más centrado que el objeto, la E es del remate.
            if (CurrentTarget != null && finisher != null && finisher.HasPriorityOver(CurrentTarget)) CurrentTarget = null;

            // Y si hay un panel o una hoja cerrada más centrados, la E es del hackeo.
            if (CurrentTarget != null && hack != null && hack.HasPriorityOver(CurrentTarget)) CurrentTarget = null;
        }
    }
}
