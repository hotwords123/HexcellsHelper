#if !ASM_PATCH

using HarmonyLib;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine;

[HarmonyPatch(typeof(ScrollBar), "Update")]
public static class UpdatePatch
{
    static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var codes = new List<CodeInstruction>(instructions);
        var targetMethod = AccessTools.Method(typeof(UpdatePatch), nameof(CalculateScrollDelta));
        var numberField = AccessTools.Field(typeof(ScrollBar), "numberOfLevelsInList");

        for (int i = 0; i < codes.Count - 4; i++)
        {
            // Original code: 15f / (float)numberOfLevelsInList
            // ldc.r4 15, ldarg.0, ldfld numberOfLevelsInList, conv.r4, div
            if (codes[i].opcode == OpCodes.Ldc_R4 && (float)codes[i].operand == 15f &&
                codes[i + 1].opcode == OpCodes.Ldarg_0 &&
                codes[i + 2].LoadsField(numberField) &&
                codes[i + 3].opcode == OpCodes.Conv_R4 &&
                codes[i + 4].opcode == OpCodes.Div)
            {
                // Replace with: CalculateScrollDelta(numberOfLevelsInList)
                // ldarg.0, ldfld numberOfLevelsInList, call CalculateScrollDelta
                codes[i] = new CodeInstruction(OpCodes.Ldarg_0);
                codes[i + 1] = new CodeInstruction(OpCodes.Ldfld, numberField);
                codes[i + 2] = new CodeInstruction(OpCodes.Call, targetMethod);

                // Clear the next two instructions (replace with nop)
                codes[i + 3] = new CodeInstruction(OpCodes.Nop);
                codes[i + 4] = new CodeInstruction(OpCodes.Nop);

                Debug.Log("ScrollBar Update transpiler applied at index: " + i);
            }
        }

        return codes;
    }

    static float CalculateScrollDelta(int numberOfLevelsInList)
    {
        return 11f / Mathf.Max(11, numberOfLevelsInList - 10);
    }
}

#endif
