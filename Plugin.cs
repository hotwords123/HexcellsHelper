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
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
#endif
    {
#if ASM_PATCH
        public static readonly string BaseDir =
            Path.Combine(Application.dataPath, "HexcellsHelper");
#else
        public static readonly string BaseDir =
            Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        void Awake()
        {
            var harmony = new Harmony(PluginInfo.PLUGIN_GUID);
            harmony.PatchAll();

            Init(BepInEx.Bootstrap.Chainloader.ManagerObject);
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

            Debug.Log($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
        }
    }
}
