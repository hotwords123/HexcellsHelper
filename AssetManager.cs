using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace HexcellsHelper
{
    public static class AssetManager
    {
        readonly static Dictionary<string, Texture2D> textureCache = [];

        public static string GetAssetPath(string assetName)
        {
            var assetsDir = Path.Combine(Plugin.BaseDir, "Assets");
            return Path.Combine(assetsDir, assetName);
        }

        public static Texture2D LoadTexture(string assetName)
        {
            if (textureCache.TryGetValue(assetName, out var texture))
            {
                return texture;
            }

            var path = GetAssetPath(assetName);
            texture = LoadTextureFromPath(path);
            textureCache[assetName] = texture;
            return texture;
        }

        public static Texture2D LoadTextureFromPath(string path)
        {
            if (!File.Exists(path))
            {
                Debug.LogError($"Texture file not found: {path}");
                return null;
            }

            var bytes = File.ReadAllBytes(path);
            var texture = new Texture2D(2, 2);
            if (!texture.LoadImage(bytes))
            {
                Debug.LogError($"Failed to load texture from: {path}");
                return null;
            }

            texture.name = Path.GetFileNameWithoutExtension(path);
            return texture;
        }
    }
}
