using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Heartbeat.Core
{
    /// <summary>
    /// Loads gameplay scenes behind a black fade. Never shows a loading screen (floor transitions are
    /// staged as cinematics, M-15). Lives on the Bootstrap object; fades use unscaled time.
    /// </summary>
    public sealed class SceneLoader : MonoBehaviour
    {
        [SerializeField, Min(0f)] private float fadeSeconds = 0.4f;

        private CanvasGroup fade;
        private TMPro.TMP_Text status;
        private bool busy;

        public bool IsBusy => busy;
        public string CurrentScene { get; private set; }

        private void Awake()
        {
            BuildFadeOverlay();
        }

        /// <summary>Replaces the current gameplay scene (Bootstrap stays loaded).</summary>
        public void Load(string sceneName, Action onLoaded = null, bool fadeOut = true)
        {
            if (busy)
            {
                HbLog.Warn("Scene", $"Load '{sceneName}' ignored: a load is already running");
                return;
            }
            StartCoroutine(LoadRoutine(sceneName, onLoaded, fadeOut));
        }

        public void SetFade(float alpha) => fade.alpha = alpha;

        /// <summary>Terminal boot line on the black screen, e.g. "SYNCHRONIZING SUBJECT R-H-09…" (D-96). Empty clears it.</summary>
        public void SetStatus(string text) => status.text = text ?? "";

        private IEnumerator LoadRoutine(string sceneName, Action onLoaded, bool fadeOut)
        {
            busy = true;
            yield return Fade(0f, 1f);

            // Keep the handle: reloading the same scene name (Floor -> Floor) makes name lookups ambiguous.
            Scene previous = string.IsNullOrEmpty(CurrentScene) ? default : FindLoaded(CurrentScene);
            var op = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            if (op == null)
            {
                HbLog.Error("Scene", $"Scene '{sceneName}' is not in the build settings");
                busy = false;
                yield break;
            }
            while (!op.isDone) yield return null;

            var loaded = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);
            SceneManager.SetActiveScene(loaded);
            if (previous.IsValid() && previous.isLoaded && previous != loaded)
                yield return SceneManager.UnloadSceneAsync(previous);

            CurrentScene = sceneName;
            UI.UiFonts.Refresh();   // texts of the new scene get the D-126 fonts (or OpenDyslexic)
            HbLog.Info("Scene", $"Loaded {sceneName}");
            onLoaded?.Invoke();

            if (fadeOut) yield return Fade(1f, 0f);
            busy = false;
        }

        private static Scene FindLoaded(string name)
        {
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                var s = SceneManager.GetSceneAt(i);
                if (s.name == name && s.isLoaded) return s;
            }
            return default;
        }

        /// <summary>Registers a scene that was already open when Bootstrap started (editor play-from-scene).</summary>
        public void AdoptOpenScene(string sceneName) => CurrentScene = sceneName;

        public IEnumerator Fade(float from, float to)
        {
            fade.blocksRaycasts = true;
            for (float t = 0f; t < fadeSeconds; t += Time.unscaledDeltaTime)
            {
                fade.alpha = Mathf.Lerp(from, to, t / fadeSeconds);
                yield return null;
            }
            fade.alpha = to;
            fade.blocksRaycasts = to > 0.99f;
        }

        private void BuildFadeOverlay()
        {
            var go = new GameObject("FadeOverlay", typeof(Canvas), typeof(CanvasGroup));
            go.transform.SetParent(transform, false);
            var canvas = go.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 1000;
            fade = go.GetComponent<CanvasGroup>();
            fade.alpha = 0f;
            fade.blocksRaycasts = false;

            var img = new GameObject("Black", typeof(RectTransform), typeof(Image));
            img.transform.SetParent(go.transform, false);
            var rt = (RectTransform)img.transform;
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = rt.offsetMax = Vector2.zero;
            img.GetComponent<Image>().color = Color.black;

            status = new GameObject("Status", typeof(RectTransform)).AddComponent<TMPro.TextMeshProUGUI>();
            status.transform.SetParent(go.transform, false);
            var srt = (RectTransform)status.transform;
            srt.anchorMin = srt.anchorMax = srt.pivot = new Vector2(0.5f, 0.5f);
            srt.sizeDelta = new Vector2(1400, 80);
            status.alignment = TMPro.TextAlignmentOptions.Center;
            status.fontSize = 32;
            status.color = new Color(0.31f, 0.88f, 0.8f);
            status.text = "";
            var scaler = go.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;
        }
    }
}
