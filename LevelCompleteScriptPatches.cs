#if !ASM_PATCH

using System;
using HarmonyLib;

namespace HexcellsHelper
{
    [HarmonyPatch(typeof(LevelCompleteScriptLevelGen), "Activate")]
    public class Patch_LevelCompleteScriptLevelGen_Activate
    {
        // __0 represents the first parameter of the original method (which is 'mistakes').
        // It's named __0 instead of 'mistakes' to avoid a name conflict with the 'mistakes' property
        // of the LevelGenHistoryEntry class, which caused a false positive warning (Harmony003)
        // from the Harmony analyzer.
        static void Postfix(LevelCompleteScriptLevelGen __instance, int __0)
        {
            LevelGenHistoryManager.AddEntry(new LevelGenHistoryEntry
            {
                timestamp = DateTimeOffset.Now.ToString("o"),
                difficulty = GameObjectUtil.GetOptionsManager().currentOptions.levelGenHardModeActive ? "hard" : "easy",
                seed = GameObjectUtil.GetGameManager().seedNumber,
                mistakes = __0,
                timeTaken = __instance.timer,
            });
        }
    }
}

#endif
