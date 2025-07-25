#if !ASM_PATCH

using System;
using HarmonyLib;

namespace HexcellsHelper
{
    [HarmonyPatch(typeof(LevelCompleteScriptLevelGen), "Activate")]
    public class Patch_LevelCompleteScriptLevelGen_Activate
    {
        static void Postfix(LevelCompleteScriptLevelGen __instance, int mistakes)
        {
            LevelGenHistoryManager.AddEntry(new LevelGenHistoryEntry
            {
                timestamp = DateTimeOffset.Now.ToString("o"),
                difficulty = GameObjectUtil.GetOptionsManager().currentOptions.levelGenHardModeActive ? "hard" : "easy",
                seed = GameObjectUtil.GetGameManager().seedNumber,
                mistakes = mistakes,
                timeTaken = __instance.timer,
            });
        }
    }
}

#endif
