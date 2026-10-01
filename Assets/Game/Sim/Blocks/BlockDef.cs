using Newtonsoft.Json;

namespace Game.Sim.Blocks
{
    /// <summary>A terrain block type from Defs/Blocks.json.</summary>
    public sealed class BlockDef
    {
        [JsonRequired] public string Id { get; set; }
        [JsonRequired] public string Label { get; set; }
        public bool IsSolid { get; set; }
        public bool IsLiquid { get; set; }
        public bool IsIndestructible { get; set; }

        /// <summary>Index in the registry; this is what chunks store. Assigned at load, not in JSON.</summary>
        [JsonIgnore] public ushort NumericId { get; internal set; }
    }
}
