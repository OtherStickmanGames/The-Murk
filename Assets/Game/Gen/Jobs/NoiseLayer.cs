using Unity.Mathematics;

namespace Game.Gen
{
    /// <summary>Burst-friendly copy of <see cref="NoiseDef"/> plus the seed-derived offset.</summary>
    public struct NoiseLayer
    {
        public float3 Offset;
        public float Frequency;
        public int Octaves;
        public float Persistence;
        public float Lacunarity;
    }
}
