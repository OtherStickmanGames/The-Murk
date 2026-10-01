namespace Game.Gen
{
    /// <summary>Everything <see cref="VoxelFillJob"/> needs besides heights and ores, as plain values.</summary>
    public struct FillSettings
    {
        public int ChunksX;
        public int ChunksZ;
        public int BedrockLayers;
        public int WaterLevel;
        public int SubsurfaceDepth;
        public int ShoreHeight;

        public ushort Bedrock;
        public ushort Stone;
        public ushort Subsurface;
        public ushort Surface;
        public ushort Shore;
        public ushort Water;
    }
}
