using UnityEngine;
using UnityEngine.SceneManagement;

namespace HexcellsHelper {

    public static class EventManager
    {

        public delegate void LevelLoadedEventHandler();
        public static event LevelLoadedEventHandler LevelLoaded;

        public delegate void HexBehaviourEventHandler(HexBehaviour instance);
        public static event HexBehaviourEventHandler DestroyClicked;
        public static event HexBehaviourEventHandler HighlightClicked;

        public static void Init()
        {
            SceneManager.sceneLoaded += (_, _) => OnLevelLoaded();
        }

        public static void OnLevelLoaded()
        {
            var hexGrid = GameObjectUtil.GetHexGrid();
            var hexGridOverlay = GameObjectUtil.GetHexGridOverlay();
            if (hexGrid == null || hexGridOverlay == null)
            {
                return;
            }
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
