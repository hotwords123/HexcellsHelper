#if !ASM_PATCH
using BepInEx;
using HarmonyLib;
#endif

using System.IO;
using System.Reflection;
using UnityEngine;

namespace HexcellsHelper
{
#if ASM_PATCH
    public static class Plugin
#else
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
#endif
    {
        public static readonly string SaveDir = Path.Combine(Application.dataPath,
            Application.platform == RuntimePlatform.OSXPlayer ? "../../saves" : "../saves");

#if ASM_PATCH
        public static readonly string BaseDir =
            Path.Combine(Application.dataPath, "HexcellsHelper");
#else
        public static readonly string BaseDir =
            Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        void Awake()
        {
            var harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);
            harmony.PatchAll();

            Init(BepInEx.Bootstrap.Chainloader.ManagerObject);

            Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
        }
#endif

        public static void Init(GameObject managerObject)
        {
            // Plugin startup logic
            EventManager.Init();
            MapManager.Init();

            managerObject.AddComponent<LevelDumper>();
            managerObject.AddComponent<UndoManager>();
            managerObject.AddComponent<DisplayModeManager>();
            managerObject.AddComponent<TrivialSolver>();
            managerObject.AddComponent<HypothesisManager>();
            managerObject.AddComponent<GuideHelper>();
            managerObject.AddComponent<LevelGenHistoryScreen>();
        }
    }
}
