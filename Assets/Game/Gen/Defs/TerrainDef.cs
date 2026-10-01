using Newtonsoft.Json;

namespace Game.Gen
{
    /// <summary>
    /// Surface height = base + plains (±amplitude) + hills (0..amplitude) faded in by the hill mask.
    /// </summary>
    public sealed class TerrainDef
    {
        [JsonRequired] public float BaseHeight { get; set; }
        [JsonRequired] public NoiseDef Plains { get; set; }
        [JsonRequired] public float PlainsAmplitude { get; set; }
        [JsonRequired] public NoiseDef Hills { get; set; }
        [JsonRequired] public float HillsAmplitude { get; set; }
        [JsonRequired] public NoiseDef HillMask { get; set; }
        /// <summary>Mask noise value (-1..1) where hills are half faded in.</summary>
        [JsonRequired] public float HillMaskThreshold { get; set; }
        /// <summary>Half-width of the plains-to-hills transition in mask noise units.</summary>
        [JsonRequired] public float HillMaskBlend { get; set; }
    }
}
