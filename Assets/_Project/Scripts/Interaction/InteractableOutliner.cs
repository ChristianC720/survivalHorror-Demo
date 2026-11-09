using System.Collections.Generic;
using Heartbeat.Core;
using TMPro;
using UnityEngine;

namespace Heartbeat.Interaction
{
    /// <summary>Marks what Lukas could use nearby: small pickups get the teal rim (D-163; hulls are built once per object and share
    /// its mesh); large objects get a pulsing status lamp on the side he is looking at, like the rest of the facility's hardware.</summary>
    public sealed class InteractableOutliner : MonoBehaviour
    {
        private const float Refresh = 0.12f;

        private readonly Dictionary<MonoBehaviour, List<GameObject>> hulls = new Dictionary<MonoBehaviour, List<GameObject>>();
        private readonly HashSet<MonoBehaviour> shown = new HashSet<MonoBehaviour>();
        private readonly HashSet<MonoBehaviour> near = new HashSet<MonoBehaviour>();
        private readonly List<MonoBehaviour> stale = new List<MonoBehaviour>();
        private readonly Collider[] buffer = new Collider[64];
        private Interactor interactor;
        private const float LampDiameter = 0.12f, LampRise = 0.4f;
        private readonly Dictionary<MonoBehaviour, GameObject> lamps = new Dictionary<MonoBehaviour, GameObject>();
        private Material material, lampMaterial;
        private MaterialPropertyBlock block;
        private int mask, wallMask;
        private float range, timer;

        public void Init(Interactor owner, float outlineRange)
        {
            interactor = owner;
            range = outlineRange;
            mask = LayerMask.GetMask("Interactable");
            wallMask = LayerMask.GetMask("Environment");
            material = new Material(Resources.Load<Shader>("HB_InteractOutline"));
            lampMaterial = new Material(Resources.Load<Shader>("HB_StatusLamp"));
            block = new MaterialPropertyBlock();
        }

        private void Update()
        {
            timer -= Time.deltaTime;
            if (timer > 0f || interactor == null) return;
            timer = Refresh;

            near.Clear();
            Vector3 eye = transform.position + Vector3.up * 1.2f;
            int n = Physics.OverlapSphereNonAlloc(eye, range, buffer, mask, QueryTriggerInteraction.Collide);
            for (int i = 0; i < n; i++)
            {
                var it = buffer[i].GetComponentInParent<IInteractable>();
                if (it is MonoBehaviour mb && it.CanInteract(interactor) && Interactor.HasLineOfSight(eye, it.InteractionPoint, wallMask)) near.Add(mb);
            }

            stale.Clear();
            foreach (var mb in shown) if (mb == null || !near.Contains(mb)) stale.Add(mb);
            foreach (var mb in stale) { shown.Remove(mb); Show(mb, false); }
            foreach (var mb in near) if (shown.Add(mb)) Show(mb, true);
        }

        private void Show(MonoBehaviour target, bool on)
        {
            if (target == null) { hulls.Remove(target); lamps.Remove(target); return; }
            if (!(target is WorldItem || target is RevolverPickup || target is DocumentPickup)) { ShowLamp(target, on); return; }
            if (!hulls.TryGetValue(target, out var list))
            {
                if (!on) return;
                hulls[target] = list = Build(target);
            }
            foreach (var hull in list) if (hull != null) hull.SetActive(on);
        }

        private void ShowLamp(MonoBehaviour target, bool on)
        {
            if (!lamps.TryGetValue(target, out var lamp))
            {
                if (!on) return;
                lamp = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                Destroy(lamp.GetComponent<Collider>());
                lamp.name = "StatusLamp";
                var r = lamp.GetComponent<MeshRenderer>();
                r.sharedMaterial = lampMaterial;
                r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                r.receiveShadows = false;
                lamps[target] = lamp;
            }
            if (on) PlaceLamp(lamp.transform, target);
            lamp.SetActive(on);
        }

        private void PlaceLamp(Transform lamp, MonoBehaviour target)
        {
            Vector3 eye = transform.position + Vector3.up * 1.2f;
            Vector3 point = ((IInteractable)target).InteractionPoint + Vector3.up * LampRise;   // the E prompt sits on the point itself
            Vector3 dir = (point - eye).normalized;
            lamp.position = Physics.Linecast(eye, point + dir * 0.5f, out var hit, mask | wallMask, QueryTriggerInteraction.Ignore)
                ? hit.point + hit.normal * 0.03f
                : point - dir * 0.05f;
            lamp.SetParent(target.transform, true);   // doors and panels move
            var s = target.transform.lossyScale;
            lamp.localScale = new Vector3(LampDiameter / s.x, LampDiameter / s.y, LampDiameter / s.z);
        }

        private List<GameObject> Build(MonoBehaviour target)
        {
            var list = new List<GameObject>();
            var roots = target is Level.DoorLeaf door ? door.Leaves : new[] { target.transform };
            foreach (var root in roots)
            foreach (var r in root.GetComponentsInChildren<MeshRenderer>())
            {
                var filter = r.GetComponent<MeshFilter>();
                if (filter == null || filter.sharedMesh == null || r.GetComponent<TMP_Text>() != null) continue;
                var hull = new GameObject("InteractHull");
                hull.transform.SetParent(r.transform, false);
                hull.AddComponent<MeshFilter>().sharedMesh = filter.sharedMesh;
                var hullRenderer = hull.AddComponent<MeshRenderer>();
                var materials = new Material[filter.sharedMesh.subMeshCount];
                for (int i = 0; i < materials.Length; i++) materials[i] = material;
                hullRenderer.sharedMaterials = materials;
                hullRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                hullRenderer.receiveShadows = false;
                block.SetVector("_CenterOS", filter.sharedMesh.bounds.center);
                hullRenderer.SetPropertyBlock(block);
                list.Add(hull);
            }
            HbLog.Dev("Interact", $"Outline built for {target.name}: {list.Count} meshes");
            return list;
        }
    }
}
