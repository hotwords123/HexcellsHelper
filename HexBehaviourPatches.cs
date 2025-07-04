using HarmonyLib;

namespace HexcellsHelper
{
    [HarmonyPatch(typeof(HexBehaviour), "DestroyClick")]
    public class Patch_HexBehaviour_DestroyClick
    {
        static void Postfix(HexBehaviour __instance)
        {
            if (__instance.containsShapeBlock) return;

            var undoManager = BepInEx.Bootstrap.Chainloader.ManagerObject.GetComponent<UndoManager>();
            undoManager.AddAction(new ClickUndoAction(__instance));
        }
    }

    [HarmonyPatch(typeof(HexBehaviour), "HighlightClick")]
    public class Patch_HexBehaviour_HighlightClick
    {
        static void Postfix(HexBehaviour __instance)
        {
            if (!__instance.containsShapeBlock) return;

            var undoManager = BepInEx.Bootstrap.Chainloader.ManagerObject.GetComponent<UndoManager>();
            undoManager.AddAction(new ClickUndoAction(__instance));

            var displayModeManager = BepInEx.Bootstrap.Chainloader.ManagerObject.GetComponent<DisplayModeManager>();
            displayModeManager.UpdateHexNumbers();
        }
    }
}
