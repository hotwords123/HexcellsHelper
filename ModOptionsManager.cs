using System;
using System.IO;
using UnityEngine;

namespace HexcellsHelper
{
    [Serializable]
    public class ModOptions
    {
        public bool countRemainingOnly = false;
    }

    public static class ModOptionsManager
    {
        public static ModOptions Options { get; private set; }

        readonly static string savePath = Path.Combine(Plugin.SaveDir, "modoptions.json");

        static ModOptionsManager()
        {
            LoadOptions();
        }

        public static void LoadOptions()
        {
            if (!File.Exists(savePath))
            {
                Options = new ModOptions();
                return;
            }

            var json = File.ReadAllText(savePath);
            Options = JsonUtility.FromJson<ModOptions>(json);
        }

        public static void SaveOptions()
        {
            var json = JsonUtility.ToJson(Options, true);
            File.WriteAllText(savePath, json);
        }
    }
}
