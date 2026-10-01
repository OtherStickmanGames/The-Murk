using System.Collections.Generic;
using System.Linq;
using Game.Gen;
using Game.Sim.Blocks;
using NUnit.Framework;

namespace Game.Tests
{
    public class RegionGeneratorTests
    {
        private const uint Seed = 12345;

        private BlockRegistry _blocks;
        private GenDefs _defs;
        private RegionGenerator _generator;
        private VoxelWorld _world;

        [OneTimeSetUp]
        public void GenerateOnce()
        {
            _blocks = BlockRegistry.FromJson(TestDefs.Read("Blocks"));
            _defs = GenDefs.Parse(TestDefs.Read("Region"), TestDefs.Read("Biomes"));
            _generator = new RegionGenerator(_defs, _blocks);
            _world = Generate(Seed);
        }

        [Test]
        public void WorldMatchesRegionSize()
        {
            Assert.AreEqual(160, _world.SizeX);
            Assert.AreEqual(64, _world.SizeY);
            Assert.AreEqual(160, _world.SizeZ);
        }

        [Test]
        public void SameSeed_GivesIdenticalWorld()
        {
            var again = Generate(Seed);

            for (var i = 0; i < _world.ChunkCount; i++)
                Assert.IsTrue(_world.GetChunk(i).Blocks.SequenceEqual(again.GetChunk(i).Blocks), $"Chunk {i} differs");
        }

        [Test]
        public void DifferentSeed_GivesDifferentWorld()
        {
            var other = Generate(Seed + 1);

            var differs = Enumerable.Range(0, _world.ChunkCount)
                .Any(i => !_world.GetChunk(i).Blocks.SequenceEqual(other.GetChunk(i).Blocks));
            Assert.IsTrue(differs);
        }

        [Test]
        public void Surface_IsWithinRegionLimits()
        {
            for (var z = 0; z < _world.SizeZ; z++)
            for (var x = 0; x < _world.SizeX; x++)
            {
                var surface = SurfaceHeight(_world, x, z);
                if (surface < _defs.Region.MinSurface || surface > _defs.Region.MaxSurface)
                    Assert.Fail($"Column ({x}, {z}) surface {surface}");
            }
        }

        [Test]
        public void BottomLayers_AreBedrock_AndNothingElseIs()
        {
            var bedrock = _blocks.GetNumericId(_defs.Region.BedrockBlock);
            Assert.IsTrue(_blocks.Get(bedrock).IsIndestructible);

            for (var y = 0; y < _world.SizeY; y++)
            for (var z = 0; z < _world.SizeZ; z++)
            for (var x = 0; x < _world.SizeX; x++)
            {
                var isBedrock = _world.Get(x, y, z) == bedrock;
                if (isBedrock != y < _defs.Region.BedrockLayers)
                    Assert.Fail($"({x}, {y}, {z}) bedrock: {isBedrock}");
            }
        }

        [Test]
        public void Ores_LieOnlyWithinTheirDepthBands_AndEachIsPresent()
        {
            var bands = _defs.Biome.Ores.ToDictionary(o => _blocks.GetNumericId(o.Block));
            var counts = bands.Keys.ToDictionary(id => id, _ => 0);

            for (var z = 0; z < _world.SizeZ; z++)
            for (var x = 0; x < _world.SizeX; x++)
            {
                var surface = SurfaceHeight(_world, x, z);
                for (var y = 0; y <= surface; y++)
                {
                    var block = _world.Get(x, y, z);
                    if (!bands.TryGetValue(block, out var ore))
                        continue;

                    var depth = surface - y;
                    if (depth < ore.MinDepth || depth >= ore.MaxDepth)
                        Assert.Fail($"{ore.Block} at ({x}, {y}, {z}), depth {depth}");
                    counts[block]++;
                }
            }

            foreach (var pair in counts)
                Assert.Greater(pair.Value, 0, $"No {_blocks.Get(pair.Key).Id} generated");
        }

        [Test]
        public void Water_FillsOnlyOpenSpaceUpToWaterLevel_AndExists()
        {
            var water = _blocks.GetNumericId(_defs.Biome.WaterBlock);
            var total = 0;
            foreach (var seed in new uint[] { Seed, 1, 2, 3 })
            {
                var world = seed == Seed ? _world : Generate(seed);
                for (var z = 0; z < world.SizeZ; z++)
                for (var x = 0; x < world.SizeX; x++)
                {
                    var surface = SurfaceHeight(world, x, z);
                    for (var y = 0; y < world.SizeY; y++)
                    {
                        var expected = y > surface && y <= _defs.Region.WaterLevel;
                        if (expected != (world.Get(x, y, z) == water))
                            Assert.Fail($"Seed {seed} ({x}, {y}, {z}): water expected {expected}");
                        if (expected)
                            total++;
                    }
                }
            }

            Assert.Greater(total, 0, "No rivers or lakes in four seeds");
        }

        private VoxelWorld Generate(uint seed)
        {
            var world = _generator.CreateWorld();
            var report = _generator.Generate(world, seed);
            UnityEngine.Debug.Log("[Gen] " + report);
            return world;
        }

        private int SurfaceHeight(VoxelWorld world, int x, int z)
        {
            for (var y = world.SizeY - 1; y >= 0; y--)
                if (_blocks.Get(world.Get(x, y, z)).IsSolid)
                    return y;
            return -1;
        }
    }
}
