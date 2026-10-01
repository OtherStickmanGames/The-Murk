using Unity.Mathematics;

namespace Game.Gen
{
    /// <summary>Fractal simplex noise from Unity.Mathematics, normalised to about -1..1.</summary>
    public static class NoiseUtil
    {
        public static float Fbm(in NoiseLayer layer, float2 position)
        {
            var p = position * layer.Frequency + layer.Offset.xy;
            float sum = 0f, amplitude = 1f, norm = 0f;
            for (var i = 0; i < layer.Octaves; i++)
            {
                sum += noise.snoise(p) * amplitude;
                norm += amplitude;
                amplitude *= layer.Persistence;
                p *= layer.Lacunarity;
            }
            return sum / norm;
        }

        public static float Fbm(in NoiseLayer layer, float3 position)
        {
            var p = position * layer.Frequency + layer.Offset;
            float sum = 0f, amplitude = 1f, norm = 0f;
            for (var i = 0; i < layer.Octaves; i++)
            {
                sum += noise.snoise(p) * amplitude;
                norm += amplitude;
                amplitude *= layer.Persistence;
                p *= layer.Lacunarity;
            }
            return sum / norm;
        }
    }
}
