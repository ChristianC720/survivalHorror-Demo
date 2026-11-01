using UnityEngine;

namespace Heartbeat.Narrative
{
    /// <summary>
    /// A readable record (documents, confiscated objects, bestiary entries). Text lives in Localization tables
    /// (Desarrollo/15-texts.md); this asset only stores keys and metadata.
    /// </summary>
    [CreateAssetMenu(menuName = "Heartbeat/Document", fileName = "DOC_")]
    public sealed class DocumentDefinition : ScriptableObject
    {
        [Tooltip("Stable id saved in the registry, e.g. DOC.EMILIA.03")]
        public string id;
        public RecordKind kind;
        [Tooltip("Floor where it can appear (1-3, 0 = intro / any).")]
        [Range(0, 3)] public int floor;
        [Tooltip("Localization key of the title (table DOC/CONF/BEST).")]
        public string titleKey;
        [Tooltip("Localization key of the body text.")]
        public string bodyKey;
        [Tooltip("Placeholder text shown until localization tables exist.")]
        [TextArea] public string placeholderTitle;
    }
}
