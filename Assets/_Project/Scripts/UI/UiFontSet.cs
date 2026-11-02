using Heartbeat.Core;
using Heartbeat.Save;
using TMPro;
using UnityEngine;

namespace Heartbeat.UI
{
    /// <summary>The three fonts of D-126 (OFL, in UI/Fonts). Lives in Resources so code-built UI can reach it.</summary>
    [CreateAssetMenu(menuName = "Heartbeat/UI Font Set", fileName = "UiFontSet")]
    public sealed class UiFontSet : ScriptableObject
    {
        [Tooltip("VT323: terminal and facility screens, readouts.")] public TMP_FontAsset data;
        [Tooltip("IBM Plex Sans: titles, menus, prompts, subtitles.")] public TMP_FontAsset menu;
        [Tooltip("OpenDyslexic: replaces both when the accessibility option is on.")] public TMP_FontAsset dyslexia;
    }
}
