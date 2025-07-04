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
            var gameManager = FindObjectOfType<GameManagerScript>();
            if (gameManager == null)
            {
                Logger.LogError("GameManagerScript not found! Plugin cannot function properly.");
                return;
            }

            if (gameManager.GetComponent<LevelDumper>() == null)
            {
                gameManager.gameObject.AddComponent<LevelDumper>();
                Logger.LogInfo("LevelDumper component added to GameManager.");
            }

            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
        }
    }
}
