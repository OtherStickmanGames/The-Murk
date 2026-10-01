using Game.Sim.Blocks;
using Unity.Mathematics;

namespace Game.Gen
{
    /// <summary>Defs and a seed turned into the plain values the jobs run on.</summary>
    internal sealed class GenSettings
    {
        // Noise offsets stay within this range: further out, float loses precision in the coordinates.
        private const float MaxOffset = 1024f;

        public TerrainSettings Terrain;
        public FillSettings Fill;
        public OreVein[] Ores;

        public static GenSettings Build(GenDefs defs, BlockRegistry blocks, uint seed)
        {
            var region = defs.Region;
            var biome = defs.Biome;
            var terrain = biome.Terrain;

            // Layers draw offsets in a fixed order, so adding an ore at the end never moves existing terrain.
            var random = Random.CreateFromIndex(seed);

            var settings = new GenSettings
            {
                Terrain = new TerrainSettings
                {
                    SizeX = region.SizeX,
                    MinSurface = region.MinSurface,
                    MaxSurface = region.MaxSurface,
                    WaterLevel = region.WaterLevel,
                    BaseHeight = terrain.BaseHeight,
                    Plains = Layer(terrain.Plains, ref random),
                    PlainsAmplitude = terrain.PlainsAmplitude,
                    Hills = Layer(terrain.Hills, ref random),
                    HillsAmplitude = terrain.HillsAmplitude,
                    HillMask = Layer(terrain.HillMask, ref random),
                    HillMaskThreshold = terrain.HillMaskThreshold,
                    HillMaskBlend = terrain.HillMaskBlend,
                    River = Layer(biome.Rivers.Noise, ref random),
                    RiverWidth = biome.Rivers.Width,
                    RiverBankWidth = biome.Rivers.BankWidth,
                    RiverBedDepth = biome.Rivers.BedDepth,
                },
                Fill = new FillSettings
                {
                    ChunksX = region.SizeX / Chunk.Size,
                    ChunksZ = region.SizeZ / Chunk.Size,
                    BedrockLayers = region.BedrockLayers,
                    WaterLevel = region.WaterLevel,
                    SubsurfaceDepth = biome.SubsurfaceDepth,
                    ShoreHeight = biome.ShoreHeight,
                    Bedrock = blocks.GetNumericId(region.BedrockBlock),
                    Stone = blocks.GetNumericId(biome.StoneBlock),
                    Subsurface = blocks.GetNumericId(biome.SubsurfaceBlock),
                    Surface = blocks.GetNumericId(biome.SurfaceBlock),
                    Shore = blocks.GetNumericId(biome.ShoreBlock),
                    Water = blocks.GetNumericId(biome.WaterBlock),
                },
                Ores = new OreVein[biome.Ores.Count],
            };

            for (var i = 0; i < biome.Ores.Count; i++)
            {
                var ore = biome.Ores[i];
                settings.Ores[i] = new OreVein
                {
                    Block = blocks.GetNumericId(ore.Block),
                    MinDepth = ore.MinDepth,
                    MaxDepth = ore.MaxDepth,
                    Noise = Layer(ore.Noise, ref random),
                    Threshold = ore.Threshold,
                };
            }
            return settings;
        }

        private static NoiseLayer Layer(NoiseDef def, ref Random random)
        {
            return new NoiseLayer
            {
                Offset = random.NextFloat3(-MaxOffset, MaxOffset),
                Frequency = def.Frequency,
                Octaves = def.Octaves,
                Persistence = def.Persistence,
                Lacunarity = def.Lacunarity,
            };
        }
    }
}
