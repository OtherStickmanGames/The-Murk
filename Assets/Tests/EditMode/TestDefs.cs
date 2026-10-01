using System.IO;
using UnityEngine;

namespace Game.Tests
{
    /// <summary>Reads the real Def files from disk, so tests check the content that ships.</summary>
    internal static class TestDefs
    {
        public static string Read(string name)
        {
            return File.ReadAllText(Path.Combine(Application.dataPath, "Game/Data/Resources/Defs", name + ".json"));
        }
    }
}
