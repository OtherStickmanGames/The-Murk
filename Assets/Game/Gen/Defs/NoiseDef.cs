using Newtonsoft.Json;

namespace Game.Gen
{
    /// <summary>Fractal simplex noise: <see cref="Octaves"/> layers, each finer and weaker than the previous.</summary>
    public sealed class NoiseDef
    {
        /// <summary>Cycles per block of the first octave.</summary>
        [JsonRequired] public float Frequency { get; set; }
        public int Octaves { get; set; } = 1;
        /// <summary>Amplitude multiplier per octave.</summary>
        public float Persistence { get; set; } = 0.5f;
        /// <summary>Frequency multiplier per octave.</summary>
        public float Lacunarity { get; set; } = 2f;
    }
}
