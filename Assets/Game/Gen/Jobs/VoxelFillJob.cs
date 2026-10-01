using Game.Sim.Blocks;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace Game.Gen
{
    /// <summary>
    /// One block per index. The output is laid out chunk after chunk in <see cref="VoxelWorld"/> order,
    /// each chunk in <see cref="Chunk"/> layout, so it is copied into chunks slice by slice.
    /// </summary>
    [BurstCompile(CompileSynchronously = true, FloatMode = FloatMode.Strict, FloatPrecision = FloatPrecision.Standard)]
    public struct VoxelFillJob : IJobParallelFor
    {
        public FillSettings Settings;
        [ReadOnly] public NativeArray<int> Heights;
        [ReadOnly] public NativeArray<OreVein> Ores;
        [WriteOnly] public NativeArray<ushort> Blocks;

        public void Execute(int index)
        {
            var s = Settings;
            var chunk = index / Chunk.Volume;
            var local = index - chunk * Chunk.Volume;

            var x = chunk % s.ChunksX * Chunk.Size + (local & Chunk.SizeMask);
            var z = chunk / s.ChunksX % s.ChunksZ * Chunk.Size + ((local >> Chunk.SizeShift) & Chunk.SizeMask);
            var y = chunk / (s.ChunksX * s.ChunksZ) * Chunk.Size + (local >> Chunk.LayerShift);

            var height = Heights[x + z * s.ChunksX * Chunk.Size];
            Blocks[index] = BlockAt(x, y, z, height);
        }

        private ushort BlockAt(int x, int y, int z, int height)
        {
            var s = Settings;
            if (y < s.BedrockLayers)
                return s.Bedrock;
            if (y > height)
                return y <= s.WaterLevel ? s.Water : BlockRegistry.AirId;

            var depth = height - y;
            if (depth == 0)
                return height <= s.WaterLevel + s.ShoreHeight ? s.Shore : s.Surface;
            if (depth <= s.SubsurfaceDepth)
                return s.Subsurface;

            var p = new float3(x, y, z);
            for (var i = 0; i < Ores.Length; i++)
            {
                var ore = Ores[i];
                if (depth >= ore.MinDepth && depth < ore.MaxDepth && NoiseUtil.Fbm(ore.Noise, p) > ore.Threshold)
                    return ore.Block;
            }
            return s.Stone;
        }
    }
}
