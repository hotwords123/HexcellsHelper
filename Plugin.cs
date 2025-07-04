using BepInEx;
using UnityEngine;

namespace HexcellsHelper
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        private void Awake()
        {
            // Plugin startup logic
            BepInEx.Bootstrap.Chainloader.ManagerObject.AddComponent<LevelDumper>();
            Logger.LogInfo("LevelDumper component added to GameManager.");

            BepInEx.Bootstrap.Chainloader.ManagerObject.AddComponent<UndoManager>();
            Logger.LogInfo("UndoManager component added to GameManager.");

            HarmonyLib.Harmony harmony = new(PluginInfo.PLUGIN_GUID);
            harmony.PatchAll();

            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
        }
    }
}
