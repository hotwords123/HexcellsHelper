#if !ASM_PATCH
using HarmonyLib;
#endif

using System.Reflection;
using UnityEngine;

namespace HexcellsHelper
{
    public static class AccessUtil
    {
        public static FieldInfo GetFieldInfo<T>(string fieldName)
        {
#if ASM_PATCH
            return typeof(T).GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
#else
            return AccessTools.Field(typeof(T), fieldName);
#endif
        }

        public static MethodInfo GetMethodInfo<T>(string methodName)
        {
#if ASM_PATCH
            return typeof(T).GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);
#else
            return AccessTools.Method(typeof(T), methodName);
#endif
        }
    }

    public static class FlowerGuideUtil
    {
        static readonly FieldInfo guideIsOffField =
            AccessUtil.GetFieldInfo<BlueHexFlower>("guideIsOff");
        static readonly MethodInfo toggleHexGuideMethod =
            AccessUtil.GetMethodInfo<BlueHexFlower>("ToggleHexGuide");

        public static bool GetGuideIsOff(BlueHexFlower flower)
        {
            return (bool)guideIsOffField.GetValue(flower);
        }

        public static void ToggleHexGuide(BlueHexFlower flower)
        {
            toggleHexGuideMethod.Invoke(flower, null);
        }
    }

    public static class ColumnGuideUtil
    {
        public static Renderer GetRenderer(ColumnNumber column)
        {
            return column.transform.Find("Line").GetComponent<Renderer>();
        }
    }
}
