namespace Game.Gen
{
    /// <summary>Burst-friendly copy of <see cref="OreVeinDef"/> with the block resolved to its numeric id.</summary>
    public struct OreVein
    {
        public ushort Block;
        public int MinDepth;
        public int MaxDepth;
        public NoiseLayer Noise;
        public float Threshold;
    }
}
