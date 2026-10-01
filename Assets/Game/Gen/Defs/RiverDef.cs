using Newtonsoft.Json;

namespace Game.Gen
{
    /// <summary>
    /// Rivers follow the zero line of a noise: |noise| &lt; Width is the channel, the next BankWidth is the valley slope.
    /// </summary>
    public sealed class RiverDef
    {
        [JsonRequired] public NoiseDef Noise { get; set; }
        [JsonRequired] public float Width { get; set; }
        [JsonRequired] public float BankWidth { get; set; }
        /// <summary>Blocks below the water level at the middle of the channel.</summary>
        [JsonRequired] public int BedDepth { get; set; }
    }
}
