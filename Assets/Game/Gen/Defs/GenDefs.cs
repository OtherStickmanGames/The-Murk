using System;
using System.Collections.Generic;
using System.Linq;
using Game.Sim;
using Game.Sim.Blocks;

namespace Game.Gen
{
    /// <summary>Validated generation content: the region and the biome it uses.</summary>
    public sealed class GenDefs
    {
        private GenDefs(RegionDef region, BiomeDef biome)
        {
            Region = region;
            Biome = biome;
        }

        public RegionDef Region { get; }
        public BiomeDef Biome { get; }

        public static GenDefs Parse(string regionJson, string biomesJson)
        {
            var region = DefJson.Deserialize<RegionDef>(regionJson, "Region.json");
            var biomes = DefJson.Deserialize<List<BiomeDef>>(biomesJson, "Biomes.json");

            var biome = biomes.FirstOrDefault(b => b.Id == region.Biome)
                        ?? throw new InvalidOperationException($"Region.json: unknown biome '{region.Biome}'.");

            Validate(region);
            Validate(biome);
            return new GenDefs(region, biome);
        }

        private static void Validate(RegionDef r)
        {
            Require(r.SizeX > 0 && r.SizeY > 0 && r.SizeZ > 0
                    && r.SizeX % Chunk.Size == 0 && r.SizeY % Chunk.Size == 0 && r.SizeZ % Chunk.Size == 0,
                $"Region.json: size {r.SizeX}×{r.SizeY}×{r.SizeZ} must be positive multiples of {Chunk.Size}.");
            Require(r.BedrockLayers > 0 && r.BedrockLayers < r.MinSurface,
                "Region.json: bedrockLayers must be in [1, minSurface).");
            Require(r.MinSurface <= r.MaxSurface && r.MaxSurface < r.SizeY,
                "Region.json: need minSurface <= maxSurface < sizeY.");
            Require(r.WaterLevel >= r.BedrockLayers && r.WaterLevel < r.SizeY,
                "Region.json: waterLevel must be in [bedrockLayers, sizeY).");
        }

        private static void Validate(BiomeDef b)
        {
            var source = $"Biomes.json '{b.Id}'";
            Require(b.SubsurfaceDepth >= 0, $"{source}: subsurfaceDepth must be >= 0.");
            Require(b.Terrain.HillMaskBlend > 0, $"{source}: terrain.hillMaskBlend must be > 0.");
            Require(b.Rivers.Width >= 0 && b.Rivers.BankWidth > 0, $"{source}: rivers need width >= 0 and bankWidth > 0.");

            Validate(b.Terrain.Plains, $"{source} terrain.plains");
            Validate(b.Terrain.Hills, $"{source} terrain.hills");
            Validate(b.Terrain.HillMask, $"{source} terrain.hillMask");
            Validate(b.Rivers.Noise, $"{source} rivers.noise");

            foreach (var ore in b.Ores)
            {
                Require(ore.MinDepth >= 0 && ore.MinDepth < ore.MaxDepth,
                    $"{source} ore '{ore.Block}': need 0 <= minDepth < maxDepth.");
                Validate(ore.Noise, $"{source} ore '{ore.Block}'");
            }
        }

        private static void Validate(NoiseDef n, string source)
        {
            Require(n.Frequency > 0 && n.Octaves >= 1 && n.Lacunarity > 0,
                $"{source}: noise needs frequency > 0, octaves >= 1, lacunarity > 0.");
        }

        private static void Require(bool condition, string message)
        {
            if (!condition)
                throw new InvalidOperationException(message);
        }
    }
}
