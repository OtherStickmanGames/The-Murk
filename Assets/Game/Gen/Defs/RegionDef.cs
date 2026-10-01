using Newtonsoft.Json;

namespace Game.Gen
{
    /// <summary>Region geometry from Defs/Region.json; sizes must be multiples of the chunk size.</summary>
    public sealed class RegionDef
    {
        [JsonRequired] public int SizeX { get; set; }
        [JsonRequired] public int SizeY { get; set; }
        [JsonRequired] public int SizeZ { get; set; }

        /// <summary>Bottom layers of indestructible block.</summary>
        [JsonRequired] public int BedrockLayers { get; set; }
        [JsonRequired] public string BedrockBlock { get; set; }

        /// <summary>Top solid block of every column lies within [MinSurface, MaxSurface].</summary>
        [JsonRequired] public int MinSurface { get; set; }
        [JsonRequired] public int MaxSurface { get; set; }

        /// <summary>Highest water block; everything open below it is flooded.</summary>
        [JsonRequired] public int WaterLevel { get; set; }

        [JsonRequired] public string Biome { get; set; }
    }
}
