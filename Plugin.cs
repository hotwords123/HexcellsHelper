using System.IO;
using System.Reflection;
using BepInEx;
using UnityEngine;

namespace HexcellsHelper
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        public static readonly string BaseDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        void Awake()
        {
            // Plugin startup logic
            EventManager.Init();
            MapManager.Init();

            var managerObject = BepInEx.Bootstrap.Chainloader.ManagerObject;
            managerObject.AddComponent<LevelDumper>();
            managerObject.AddComponent<UndoManager>();
            managerObject.AddComponent<DisplayModeManager>();
            managerObject.AddComponent<TrivialSolver>();
            managerObject.AddComponent<HypothesisManager>();

            HarmonyLib.Harmony harmony = new(PluginInfo.PLUGIN_GUID);
            harmony.PatchAll();

            Debug.Log($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
        }
    }
}
