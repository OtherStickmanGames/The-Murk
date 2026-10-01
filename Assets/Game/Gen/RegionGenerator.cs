using System;
using System.Collections.Generic;
using System.Diagnostics;
using Game.Sim.Blocks;
using Unity.Collections;
using Unity.Jobs;

namespace Game.Gen
{
    /// <summary>
    /// Fills a <see cref="VoxelWorld"/> from a seed with Burst jobs. Same seed — same blocks.
    /// </summary>
    public sealed class RegionGenerator
    {
        private const int ColumnsPerBatch = 64;
        private const int BlocksPerBatch = Chunk.Size * Chunk.Size;

        private readonly GenDefs _defs;
        private readonly BlockRegistry _blocks;

        public RegionGenerator(GenDefs defs, BlockRegistry blocks)
        {
            _defs = defs;
            _blocks = blocks;
        }

        public VoxelWorld CreateWorld()
        {
            var r = _defs.Region;
            return new VoxelWorld(r.SizeX / Chunk.Size, r.SizeY / Chunk.Size, r.SizeZ / Chunk.Size);
        }

        public GenerationReport Generate(VoxelWorld world, uint seed)
        {
            var r = _defs.Region;
            if (world.SizeX != r.SizeX || world.SizeY != r.SizeY || world.SizeZ != r.SizeZ)
                throw new ArgumentException(
                    $"World {world.SizeX}×{world.SizeY}×{world.SizeZ} does not match Region.json.", nameof(world));

            var total = Stopwatch.StartNew();
            var settings = GenSettings.Build(_defs, _blocks, seed);

            var heights = new NativeArray<int>(r.SizeX * r.SizeZ, Allocator.TempJob,
                NativeArrayOptions.UninitializedMemory);
            var ores = new NativeArray<OreVein>(settings.Ores, Allocator.TempJob);
            var blocks = new NativeArray<ushort>(world.ChunkCount * Chunk.Volume, Allocator.TempJob,
                NativeArrayOptions.UninitializedMemory);
            double heightMapMs, fillMs, copyMs;
            try
            {
                var phase = Stopwatch.StartNew();
                new HeightMapJob { Settings = settings.Terrain, Heights = heights }
                    .Schedule(heights.Length, ColumnsPerBatch).Complete();
                heightMapMs = phase.Elapsed.TotalMilliseconds;

                phase.Restart();
                new VoxelFillJob { Settings = settings.Fill, Heights = heights, Ores = ores, Blocks = blocks }
                    .Schedule(blocks.Length, BlocksPerBatch).Complete();
                fillMs = phase.Elapsed.TotalMilliseconds;

                phase.Restart();
                for (var i = 0; i < world.ChunkCount; i++)
                {
                    var chunk = world.GetChunk(i);
                    blocks.GetSubArray(i * Chunk.Volume, Chunk.Volume).CopyTo(chunk.Blocks);
                    chunk.MarkDirty();
                }
                copyMs = phase.Elapsed.TotalMilliseconds;
            }
            finally
            {
                heights.Dispose();
                ores.Dispose();
                blocks.Dispose();
            }

            var counts = CountBlocks(world);
            return new GenerationReport(seed, r.SizeX, r.SizeY, r.SizeZ, heightMapMs, fillMs, copyMs,
                total.Elapsed.TotalMilliseconds, counts);
        }

        private IReadOnlyDictionary<string, int> CountBlocks(VoxelWorld world)
        {
            var counts = new int[_blocks.Count];
            for (var i = 0; i < world.ChunkCount; i++)
                foreach (var block in world.GetChunk(i).Blocks)
                    counts[block]++;

            var result = new Dictionary<string, int>();
            for (var id = 0; id < counts.Length; id++)
                result[_blocks.Get((ushort)id).Id] = counts[id];
            return result;
        }
    }
}
