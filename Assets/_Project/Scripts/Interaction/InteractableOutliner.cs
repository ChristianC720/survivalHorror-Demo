using System.Collections.Generic;
using Heartbeat.Core;
using TMPro;
using UnityEngine;

namespace Heartbeat.Interaction
{
    /// <summary>Teal rim on everything Lukas could use nearby (D-163). Hulls are built once per object and share its mesh.</summary>
    public sealed class InteractableOutliner : MonoBehaviour
    {
        private const float Refresh = 0.12f;

        private readonly Dictionary<MonoBehaviour, List<GameObject>> hulls = new Dictionary<MonoBehaviour, List<GameObject>>();
        private readonly HashSet<MonoBehaviour> shown = new HashSet<MonoBehaviour>();
        private readonly HashSet<MonoBehaviour> near = new HashSet<MonoBehaviour>();
        private readonly List<MonoBehaviour> stale = new List<MonoBehaviour>();
        private readonly Collider[] buffer = new Collider[32];
        private Interactor interactor;
        private Material material;
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
                if (it is MonoBehaviour mb && it.CanInteract(interactor) && Visible(eye, it.InteractionPoint)) near.Add(mb);
            }

            stale.Clear();
            foreach (var mb in shown) if (mb == null || !near.Contains(mb)) stale.Add(mb);
            foreach (var mb in stale) { shown.Remove(mb); Show(mb, false); }
            foreach (var mb in near) if (shown.Add(mb)) Show(mb, true);
        }

        private bool Visible(Vector3 eye, Vector3 point)
        {
            // Doors are Environment themselves: a hit right next to the target still counts as seeing it.
            return !Physics.Linecast(eye, point, out var hit, wallMask, QueryTriggerInteraction.Ignore)
                || Vector3.Distance(hit.point, point) < 0.6f;
        }

        private void Show(MonoBehaviour target, bool on)
        {
            if (target == null) { hulls.Remove(target); return; }
            if (!hulls.TryGetValue(target, out var list))
            {
                if (!on) return;
                hulls[target] = list = Build(target);
            }
            foreach (var hull in list) if (hull != null) hull.SetActive(on);
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
