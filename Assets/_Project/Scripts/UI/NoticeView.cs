using Heartbeat.Core;
using Heartbeat.InputRouting;
using Heartbeat.Player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Heartbeat.UI
{
    /// <summary>
    /// Brief top-centre notices, the tutorial's standing hint and the damage flash (D-163). Never a permanent HUD: the hint
    /// exists only in the guided tutorial. Arguments such as "Gameplay/Reload" are shown as that action's current key.
    /// </summary>
    public sealed class NoticeView : MonoBehaviour
    {
        [SerializeField] private float noticeSeconds = 3f;
        [SerializeField] private float flashSeconds = 0.35f;
        [SerializeField] private float flashAlpha = 0.3f;

        private TMP_Text hint, notice;
        private Image flash;
        private float noticeUntil, flashUntil;
        private int lastHealth;
        private InputRouter router;
        private string hintKey;
        private object[] hintArgs;

        private void Awake()
        {
            hint = MakeLine("Hint", -70f, 34f, UiKit.Teal);
            notice = MakeLine("Notice", -120f, 30f, UiKit.Amber);
            var go = new GameObject("DamageFlash", typeof(RectTransform), typeof(Image));
            go.transform.SetParent(transform, false);
            var rt = (RectTransform)go.transform;
            rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one; rt.offsetMin = rt.offsetMax = Vector2.zero;
            flash = go.GetComponent<Image>();
            flash.raycastTarget = false;
            flash.color = Color.clear;
        }

        private TMP_Text MakeLine(string lineName, float y, float size, Color color)
        {
            var t = UiKit.Text(transform, "", size, color, data: false);
            t.gameObject.name = lineName;
            UiFonts.Apply(t, data: false, outline: true);
            t.alignment = TextAlignmentOptions.Center;
            t.raycastTarget = false;
            var rt = (RectTransform)t.transform;
            rt.anchorMin = new Vector2(0.1f, 1f); rt.anchorMax = new Vector2(0.9f, 1f); rt.pivot = new Vector2(0.5f, 1f);
            rt.anchoredPosition = new Vector2(0f, y);
            rt.sizeDelta = new Vector2(0f, 48f);
            return t;
        }

        private void OnEnable()
        {
            GameEvents.NoticeRequested += OnNotice;
            GameEvents.HintChanged += OnHint;
            GameEvents.GameStateChanged += OnState;
            GameEvents.PlayerAttacked += OnAttacked;
            GameEvents.InventoryFull += OnFull;
            GameEvents.StaminaExhausted += OnBreath;
            GameEvents.HealthStateChanged += OnHealth;
            GameEvents.GrabResolved += OnGrab;
            GameEvents.HuntChanged += OnHunt;
            GameEvents.PowerRestored += OnPower;
            GameEvents.BossPhaseChanged += OnBoss;
            GameEvents.OptionsChanged += RenderHint;
            Services.TryGet(out router);
        }

        private void OnDisable()
        {
            GameEvents.NoticeRequested -= OnNotice;
            GameEvents.HintChanged -= OnHint;
            GameEvents.GameStateChanged -= OnState;
            GameEvents.PlayerAttacked -= OnAttacked;
            GameEvents.InventoryFull -= OnFull;
            GameEvents.StaminaExhausted -= OnBreath;
            GameEvents.HealthStateChanged -= OnHealth;
            GameEvents.GrabResolved -= OnGrab;
            GameEvents.HuntChanged -= OnHunt;
            GameEvents.PowerRestored -= OnPower;
            GameEvents.BossPhaseChanged -= OnBoss;
            GameEvents.OptionsChanged -= RenderHint;
        }

        private void OnFull(string _) => OnNotice("UI.NOTICE.FULL", null);
        private void OnBreath() => OnNotice("UI.NOTICE.BREATH", null);
        private void OnGrab(bool countered) => OnNotice(countered ? "UI.NOTICE.SPIKE_USED" : "UI.NOTICE.GRABBED", null);
        private void OnHunt(bool hunting) { if (hunting) OnNotice("UI.NOTICE.HUNTED", null); }
        private void OnPower() => OnNotice("UI.NOTICE.POWER", null);
        private void OnBoss(int phase) { if (phase >= 1 && phase <= 3) OnNotice("UI.NOTICE.BOSS" + phase, null); }
        private void OnAttacked(AttackInfo _) => flashUntil = Time.unscaledTime + flashSeconds;

        private void OnHealth(int state)
        {
            if (state == (int)HealthState.Injured && state > lastHealth) OnNotice("UI.NOTICE.INJURED", null);
            else if (state == (int)HealthState.Critical && state > lastHealth) OnNotice("UI.NOTICE.CRITICAL", null);
            else if (state < lastHealth) OnNotice("UI.NOTICE.HEALED", null);
            lastHealth = state;
        }

        private void OnState(GameStateId _, GameStateId to)
        {
            if (to != GameStateId.Playing) noticeUntil = 0f;
        }

        private void OnNotice(string key, object[] args)
        {
            notice.text = Loc.Get(key, Keys(args));
            noticeUntil = Time.unscaledTime + noticeSeconds;
            HbLog.Info("Notice", key);
        }

        private void OnHint(string key, object[] args)
        {
            hintKey = key;
            hintArgs = args;
            RenderHint();
        }

        private void RenderHint() => hint.text = hintKey == null ? "" : Loc.Get(hintKey, Keys(hintArgs));

        private object[] Keys(object[] args)
        {
            if (args == null) return new object[0];
            var result = new object[args.Length];
            for (int i = 0; i < args.Length; i++)
            {
                var action = args[i] is string s && s.Contains("/") && router != null ? router.Actions.asset.FindAction(s) : null;
                result[i] = action != null ? router.KeyFor(action) : args[i];
            }
            return result;
        }

        private void Update()
        {
            notice.alpha = Mathf.Clamp01((noticeUntil - Time.unscaledTime) / 0.4f);
            float flashLeft = (flashUntil - Time.unscaledTime) / flashSeconds;
            flash.color = new Color(UiKit.Red.r, 0f, 0f, flashLeft > 0f ? flashLeft * flashAlpha : 0f);
        }
    }
}
