using HarmonyLib;

namespace HexcellsHelper
{
    [HarmonyPatch(typeof(LevelGenerator), "Start")]
    public class Patch_LevelGenerator_Start
    {
        static void Postfix(LevelGenerator __instance)
        {
            EventManager.TriggerLevelLoaded();
        }
    }

    [HarmonyPatch(typeof(OldLevelGenerator), "Start")]
    public class Patch_OldLevelGenerator_Start
    {
        static void Postfix(OldLevelGenerator __instance)
        {
            EventManager.TriggerLevelLoaded();
        }
    }
}
