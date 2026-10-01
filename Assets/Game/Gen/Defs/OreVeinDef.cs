using Newtonsoft.Json;

namespace Game.Gen
{
    /// <summary>Ore replaces stone where 3D noise exceeds the threshold, only within its depth band.</summary>
    public sealed class OreVeinDef
    {
        [JsonRequired] public string Block { get; set; }
        /// <summary>Depth below the local surface, inclusive.</summary>
        [JsonRequired] public int MinDepth { get; set; }
        /// <summary>Depth below the local surface, exclusive.</summary>
        [JsonRequired] public int MaxDepth { get; set; }
        [JsonRequired] public NoiseDef Noise { get; set; }
        /// <summary>Noise value (-1..1) above which the block becomes ore; higher means rarer.</summary>
        [JsonRequired] public float Threshold { get; set; }
    }
}
