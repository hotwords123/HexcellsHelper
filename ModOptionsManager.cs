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

    public class ModOptionsManager : MonoBehaviour
    {
        public static ModOptionsManager Instance { get; private set; }

        public ModOptions Options { get; private set; }

        private string savePath;

        void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            savePath = Path.Combine(Plugin.SaveDir, "modoptions.json");
            LoadOptions();
        }

        public void LoadOptions()
        {
            if (!File.Exists(savePath))
            {
                Options = new ModOptions();
                return;
            }

            var json = File.ReadAllText(savePath);
            Options = JsonUtility.FromJson<ModOptions>(json);
        }

        public void SaveOptions()
        {
            var json = JsonUtility.ToJson(Options, true);
            File.WriteAllText(savePath, json);
        }
    }
}
