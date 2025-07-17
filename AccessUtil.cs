using System.Reflection;
using HarmonyLib;
using UnityEngine;

namespace HexcellsHelper
{
    public static class AccessUtil
    {
        public static FieldInfo GetFieldInfo<T>(string fieldName)
        {
            return AccessTools.Field(typeof(T), fieldName);
        }

        public static MethodInfo GetMethodInfo<T>(string methodName)
        {
            return AccessTools.Method(typeof(T), methodName);
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
