using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace HexcellsHelper
{
    [Serializable]
    public class LevelGenHistoryEntry
    {
        public string timestamp;
        public string difficulty;
        public string seed;
        public int mistakes;
        public float timeTaken;
    }

    public static class LevelGenHistoryManager
    {
        readonly static string savePath = Path.Combine(Plugin.SaveDir, "levelgenhistory.jsonl");

        public static void AddEntry(LevelGenHistoryEntry entry)
        {
            using var writer = new StreamWriter(savePath, append: true);
            writer.WriteLine(JsonUtility.ToJson(entry));
        }

        public static List<LevelGenHistoryEntry> LoadHistory()
        {
            var entries = new List<LevelGenHistoryEntry>();
            if (!File.Exists(savePath))
            {
                return entries;
            }

            using var reader = new StreamReader(savePath);
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                if (!string.IsNullOrEmpty(line))
                {
                    var entry = JsonUtility.FromJson<LevelGenHistoryEntry>(line);
                    entries.Add(entry);
                }
            }

            return entries;
        }
    }
}
