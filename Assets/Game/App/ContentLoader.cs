using System;
using UnityEngine;

namespace Game.App
{
    /// <summary>Reads Def files from Resources; the simulation only ever gets their text.</summary>
    public static class ContentLoader
    {
        public static string LoadDef(string name)
        {
            var asset = Resources.Load<TextAsset>("Defs/" + name);
            if (asset == null)
                throw new InvalidOperationException($"Def file 'Resources/Defs/{name}.json' not found.");
            return asset.text;
        }
    }
}
