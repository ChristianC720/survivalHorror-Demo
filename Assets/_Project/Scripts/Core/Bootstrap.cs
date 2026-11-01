using Heartbeat.InputRouting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;

namespace Heartbeat.Core
{
    /// <summary>
    /// Root of the persistent Bootstrap scene (D-108). Creates and registers the services, then routes to the
    /// first state. When Play is pressed in another scene, <see cref="BootstrapLoader"/> loads Bootstrap
    /// additively and that scene is adopted as the gameplay scene (dev convenience).
    /// </summary>
    [DefaultExecutionOrder(-1000)]
    public sealed class Bootstrap : MonoBehaviour
    {
        [SerializeField] private SceneLoader sceneLoader;

        public static bool IsReady { get; private set; }

        private GameStateMachine states;
        private InputRouter input;

        private void Awake()
        {
            if (Services.TryGet<GameStateMachine>(out _))
            {
                // A second Bootstrap (e.g. scene reloaded): keep the first one.
                Destroy(gameObject);
                return;
            }

            DontDestroyOnLoad(gameObject);
            PauseService.ResetTime();

            if (sceneLoader == null) sceneLoader = gameObject.AddComponent<SceneLoader>();
            input = new InputRouter();
            input.ContextChanged += ApplyCursor;
            EnsureEventSystem();

            states = new GameStateMachine();
            states.StateChanged += OnStateChanged;
            states.SubStateChanged += GameEvents.RaisePlaySubStateChanged;

            Services.Register(states);
            Services.Register(sceneLoader);
            Services.Register(new PauseService(states));
            Services.Register(input);
            Services.Register(new Narrative.RegistryModel());   // persistent knowledge; SaveService (E-08) loads it
            gameObject.AddComponent<PauseInput>();

            IsReady = true;
            HbLog.Info("Core", "Services ready");
        }

        private void Start()
        {
            string launchScene = BootstrapLoader.LaunchScene;
            if (string.IsNullOrEmpty(launchScene) || launchScene == SceneNames.Bootstrap)
            {
                // Normal game launch. Brightness calibration and title come with E-10; go to the menu for now.
                states.TryTransition(GameStateId.MainMenu);
                sceneLoader.Load(SceneNames.MainMenu);
                return;
            }

            // Editor: play pressed inside a gameplay or sandbox scene.
            sceneLoader.AdoptOpenScene(launchScene);
            var target = launchScene == SceneNames.MainMenu ? GameStateId.MainMenu
                : launchScene == SceneNames.Intro ? GameStateId.Intro
                : GameStateId.Playing;
            states.TryTransition(target);
            HbLog.Info("Core", $"Launched from scene '{launchScene}' -> {target}");
        }

        private void OnDestroy()
        {
            if (states == null) return;
            states.StateChanged -= OnStateChanged;
            states.SubStateChanged -= GameEvents.RaisePlaySubStateChanged;
            input?.Dispose();
            IsReady = false;
        }

        private void OnStateChanged(GameStateId from, GameStateId to)
        {
            HbLog.Info("State", $"{from} -> {to}");
            input.SetContext(DefaultContextFor(to));
            GameEvents.RaiseGameStateChanged(from, to);
        }

        /// <summary>The cursor is free only where the mouse drives a UI (menus, arm terminal).</summary>
        private static void ApplyCursor(InputContext ctx)
        {
            bool free = ctx == InputContext.Menu || ctx == InputContext.Inventory || ctx == InputContext.None;
            Cursor.lockState = free ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = free;
        }

        /// <summary>Input context when entering a main state. Gameplay systems refine it inside Playing.</summary>
        private InputContext DefaultContextFor(GameStateId state)
        {
            switch (state)
            {
                case GameStateId.Playing:
                    return states.SubState == PlaySubState.Hacking ? InputContext.Hacking
                        : states.SubState == PlaySubState.Surveillance ? InputContext.Surveillance
                        : InputContext.Exploration;
                case GameStateId.Intro: return InputContext.IntroPlayable;
                case GameStateId.FinalDecision: return InputContext.IntroPlayable;
                case GameStateId.MainMenu:
                case GameStateId.Paused:
                case GameStateId.Title:
                case GameStateId.BrightnessCalibration: return InputContext.Menu;
                case GameStateId.Reconstruction:
                case GameStateId.Victory:
                case GameStateId.EndingBasicRelief:
                case GameStateId.EndingFullRelief:
                case GameStateId.EndingAmbiguous:
                case GameStateId.EndingAssimilation:
                case GameStateId.Credits:
                case GameStateId.FloorTransition: return InputContext.Cinematic;
                default: return InputContext.None;
            }
        }

        private void EnsureEventSystem()
        {
            if (FindAnyObjectByType<EventSystem>() != null) return;
            var es = new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));
            es.transform.SetParent(transform, false);
            var module = es.GetComponent<InputSystemUIInputModule>();
            var asset = input.Actions.asset;
            module.actionsAsset = asset;
            module.point = InputActionReference.Create(asset.FindAction("UI/Point", true));
            module.leftClick = InputActionReference.Create(asset.FindAction("UI/Click", true));
            module.rightClick = InputActionReference.Create(asset.FindAction("UI/RightClick", true));
            module.scrollWheel = InputActionReference.Create(asset.FindAction("UI/ScrollWheel", true));
            module.move = InputActionReference.Create(asset.FindAction("UI/Navigate", true));
            module.submit = InputActionReference.Create(asset.FindAction("UI/Submit", true));
            module.cancel = InputActionReference.Create(asset.FindAction("UI/Cancel", true));
            module.deselectOnBackgroundClick = false;
        }
    }

    /// <summary>Loads Bootstrap first whatever scene Play was pressed in.</summary>
    public static class BootstrapLoader
    {
        public static string LaunchScene { get; private set; }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void LoadBootstrap()
        {
            LaunchScene = SceneManager.GetActiveScene().name;
            if (LaunchScene == SceneNames.Bootstrap) return;
            if (!Application.CanStreamedLevelBeLoaded(SceneNames.Bootstrap))
            {
                Debug.LogWarning("[Heartbeat][Core] Bootstrap scene is not in the build settings");
                return;
            }
            SceneManager.LoadScene(SceneNames.Bootstrap, LoadSceneMode.Additive);
        }
    }
}
