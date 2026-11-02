using Heartbeat.Core;
using Heartbeat.Narrative;
using UnityEngine;

namespace Heartbeat.Interaction
{
    /// <summary>
    /// A document or confiscated object (M-08, D-73, D-79). E registers it in the arm terminal ("REGISTERED"; Emilia's
    /// count n/6) without pausing. Documents are taken (disappear); confiscated objects are only examined and stay.
    /// Already-registered records can still be read but are not counted twice.
    /// </summary>
    public sealed class DocumentPickup : MonoBehaviour, IInteractable
    {
        [SerializeField] private DocumentDefinition document;
        [Tooltip("Confiscated objects stay in the world after being examined.")]
        [SerializeField] private bool staysInWorld;

        public DocumentDefinition Document => document;
        public string PromptKey => staysInWorld ? "UI.PROMPT.EXAMINE" : "UI.PROMPT.READ";
        public Vector3 InteractionPoint => transform.position;

        public void Setup(DocumentDefinition doc, bool stays)
        {
            document = doc;
            staysInWorld = stays;
        }

        public bool CanInteract(Interactor who) => document != null && isActiveAndEnabled;

        public void Interact(Interactor who)
        {
            var registry = Services.Get<RegistryModel>();
            bool isNew = registry.Register(document.id, document.kind);
            string extra = document.kind == RecordKind.EmiliaDocument ? $" (Emilia {registry.EmiliaCount}/{RegistryModel.EmiliaTotal})" : "";
            HbLog.Info("Registry", isNew ? $"REGISTERED {document.id}{extra}" : $"Already registered {document.id}");
            GameEvents.RaiseItemRegistered(document.id, (int)document.kind, isNew);
            if (isNew && document.kind == RecordKind.EmiliaDocument)
                Subtitles.Say(Subtitles.Lukas, registry.EmiliaCount == 1 ? "SUB.LUKAS.FIRST_EMILIA_DOC" : "SUB.LUKAS.EMILIA_DOC");
            else if (isNew && document.id == "DOC.MOTHER_RH_RECORDS")
                Subtitles.Say(Subtitles.Lukas, "SUB.LUKAS.RH_RECORDS");   // "Six before me." (Textos del Juego, lukas_rh_records)
            if (!staysInWorld) gameObject.SetActive(false);
        }
    }
}
