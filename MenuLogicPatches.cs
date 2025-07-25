#if !ASM_PATCH

using HarmonyLib;
using UnityEngine;

namespace HexcellsHelper
{
    [HarmonyPatch(typeof(MenuLogic), "Update")]
    public class Patch_MenuLogic_Update
    {
        static bool Prefix(MenuLogic __instance)
        {
            if (LevelGenHistoryScreen.Instance.IsScreenActive && Input.GetKeyDown(KeyCode.Escape))
            {
                LevelGenHistoryScreen.Instance.ExitScreen();
                return false; // Skip original Update logic
            }
            return true;
        }
    }
}

#endif
