#if !ASM_PATCH

using HarmonyLib;

namespace HexcellsHelper
{
    [HarmonyPatch(typeof(LevelGenerator), "Start")]
    public class Patch_LevelGenerator_Start
    {
        static void Postfix(LevelGenerator __instance)
        {
            if (GameObjectUtil.GetOptionsManager().currentOptions.levelGenHardModeActive)
            {
                EventManager.TriggerLevelLoaded();
            }
        }
    }

    [HarmonyPatch(typeof(OldLevelGenerator), "Start")]
    public class Patch_OldLevelGenerator_Start
    {
        static void Postfix(OldLevelGenerator __instance)
        {
            if (!GameObjectUtil.GetOptionsManager().currentOptions.levelGenHardModeActive)
            {
                EventManager.TriggerLevelLoaded();
            }
        }
    }
}

#endif
