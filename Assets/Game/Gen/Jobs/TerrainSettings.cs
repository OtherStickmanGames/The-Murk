namespace Game.Gen
{
    /// <summary>Everything <see cref="HeightMapJob"/> needs, as plain values.</summary>
    public struct TerrainSettings
    {
        public int SizeX;
        public int MinSurface;
        public int MaxSurface;
        public int WaterLevel;

        public float BaseHeight;
        public NoiseLayer Plains;
        public float PlainsAmplitude;
        public NoiseLayer Hills;
        public float HillsAmplitude;
        public NoiseLayer HillMask;
        public float HillMaskThreshold;
        public float HillMaskBlend;

        public NoiseLayer River;
        public float RiverWidth;
        public float RiverBankWidth;
        public int RiverBedDepth;
    }
}
