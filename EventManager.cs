using UnityEngine;
using UnityEngine.SceneManagement;

namespace HexcellsHelper {

    public static class EventManager
    {
        static bool isLevelLoaded = false;
        public static bool IsLevelLoaded => isLevelLoaded;

        public delegate void LevelEventHandler();
        public static event LevelEventHandler LevelLoaded;
        public static event LevelEventHandler LevelUnloaded;

        public delegate void HexBehaviourEventHandler(HexBehaviour instance);
        public static event HexBehaviourEventHandler DestroyClicked;
        public static event HexBehaviourEventHandler HighlightClicked;

        public static void Init()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneUnloaded += OnSceneUnloaded;
        }

        public static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            // Delay LevelLoaded event until the map is fully generated
            if (scene.name == "Level Generator")
            {
                return;
            }
            // Skip non-level scenes (main menu, etc.)
            if (GameObjectUtil.GetHexGrid() == null || GameObjectUtil.GetHexGridOverlay() == null)
            {
                return;
            }
            TriggerLevelLoaded();
        }

        public static void OnSceneUnloaded(Scene scene)
        {
            if (isLevelLoaded)
            {
                isLevelLoaded = false;
                LevelUnloaded?.Invoke();
            }
        }

        public static void TriggerLevelLoaded()
        {
            if (isLevelLoaded)
            {
                Debug.LogWarning("[EventManager] Level already loaded, skipping initialization.");
                return;
            }
            isLevelLoaded = true;
            LevelLoaded?.Invoke();
        }

        public static void OnDestroyClick(HexBehaviour instance)
        {
            DestroyClicked?.Invoke(instance);
        }

        public static void OnHighlightClick(HexBehaviour instance)
        {
            HighlightClicked?.Invoke(instance);
        }
    }
}
