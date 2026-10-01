using System.Collections.Generic;
using Newtonsoft.Json;

namespace Game.Gen
{
    /// <summary>A biome from Defs/Biomes.json: terrain shape, layers, rivers and ores.</summary>
    public sealed class BiomeDef
    {
        [JsonRequired] public string Id { get; set; }
        [JsonRequired] public string Label { get; set; }

        [JsonRequired] public string SurfaceBlock { get; set; }
        [JsonRequired] public string SubsurfaceBlock { get; set; }
        /// <summary>Top block of beaches and river or lake beds.</summary>
        [JsonRequired] public string ShoreBlock { get; set; }
        [JsonRequired] public string StoneBlock { get; set; }
        [JsonRequired] public string WaterBlock { get; set; }

        /// <summary>Layers of subsurface block under the top block.</summary>
        [JsonRequired] public int SubsurfaceDepth { get; set; }
        /// <summary>Columns whose surface is at most this many blocks above the water get the shore block.</summary>
        [JsonRequired] public int ShoreHeight { get; set; }

        [JsonRequired] public TerrainDef Terrain { get; set; }
        [JsonRequired] public RiverDef Rivers { get; set; }
        /// <summary>Earlier entries win where veins overlap.</summary>
        [JsonRequired] public List<OreVeinDef> Ores { get; set; }
    }
}
