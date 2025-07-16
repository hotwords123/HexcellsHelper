using HarmonyLib;

namespace HexcellsHelper
{
    [HarmonyPatch(typeof(HexBehaviour), "OnMouseOver")]
    public class Patch_HexBehaviour_OnMouseOver
    {
        static bool Prefix(HexBehaviour __instance)
        {
            if (!__instance.enabled)
            {
                // Disable mouse interaction if the HexBehaviour is not enabled
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(HexBehaviour), "DestroyClick")]
    public class Patch_HexBehaviour_DestroyClick
    {
        static void Postfix(HexBehaviour __instance)
        {
            if (!__instance.containsShapeBlock)
            {
                EventManager.OnDestroyClick(__instance);
            }
        }
    }

    [HarmonyPatch(typeof(HexBehaviour), "HighlightClick")]
    public class Patch_HexBehaviour_HighlightClick
    {
        static void Postfix(HexBehaviour __instance)
        {
            if (__instance.containsShapeBlock)
            {
                EventManager.OnHighlightClick(__instance);
            }
        }
    }
}
