using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace Game.Gen
{
    /// <summary>
    /// Surface height of every column (index = x + z * SizeX): plains and hills, then river valleys cut down to the water.
    /// </summary>
    // Strict float mode and synchronous compilation keep one seed bit-identical between runs.
    // Jobs are public: Burst does not register internal job structs as entry points and silently runs them as plain C#.
    [BurstCompile(CompileSynchronously = true, FloatMode = FloatMode.Strict, FloatPrecision = FloatPrecision.Standard)]
    public struct HeightMapJob : IJobParallelFor
    {
        public TerrainSettings Settings;
        [WriteOnly] public NativeArray<int> Heights;

        public void Execute(int index)
        {
            var s = Settings;
            var p = new float2(index % s.SizeX, index / s.SizeX);

            var plains = NoiseUtil.Fbm(s.Plains, p) * s.PlainsAmplitude;
            var hills = (NoiseUtil.Fbm(s.Hills, p) * 0.5f + 0.5f) * s.HillsAmplitude;
            var mask = math.smoothstep(s.HillMaskThreshold - s.HillMaskBlend, s.HillMaskThreshold + s.HillMaskBlend,
                NoiseUtil.Fbm(s.HillMask, p));
            var height = s.BaseHeight + plains + hills * mask;

            var river = math.abs(NoiseUtil.Fbm(s.River, p));
            if (river < s.RiverWidth)
            {
                // Deepest in the middle of the channel, level with the water at its edge.
                var bed = s.WaterLevel - s.RiverBedDepth * (1f - river / s.RiverWidth);
                height = math.min(height, bed);
            }
            else
            {
                // Valley slope: pulled down to just above the water near the channel, untouched at the rim.
                var t = math.smoothstep(s.RiverWidth, s.RiverWidth + s.RiverBankWidth, river);
                height = math.min(height, math.lerp(s.WaterLevel + 1f, height, t));
            }

            Heights[index] = math.clamp((int)math.floor(height + 0.5f), s.MinSurface, s.MaxSurface);
        }
    }
}
