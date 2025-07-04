using HarmonyLib;

namespace HexcellsHelper
{
    [HarmonyPatch(typeof(LevelGenerator), "SetupLevel")]
    public class Patch_LevelGenerator_SetupLevel
    {
        static void Postfix(LevelGenerator __instance)
        {
            var displayModeManager = BepInEx.Bootstrap.Chainloader.ManagerObject.GetComponent<DisplayModeManager>();
            displayModeManager?.UpdateHexNumbers();
        }
    }
}
