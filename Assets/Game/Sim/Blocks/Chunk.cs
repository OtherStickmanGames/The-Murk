namespace Game.Sim.Blocks
{
    /// <summary>
    /// A 32³ cube of blocks. Layout: index = x | z &lt;&lt; 5 | y &lt;&lt; 10, so one y-layer is contiguous.
    /// </summary>
    public sealed class Chunk
    {
        public const int SizeShift = 5;
        public const int Size = 1 << SizeShift;
        public const int SizeMask = Size - 1;
        public const int LayerShift = SizeShift * 2;
        public const int Volume = Size * Size * Size;

        public Chunk(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        /// <summary>Chunk coordinates (in chunks, not blocks).</summary>
        public int X { get; }
        public int Y { get; }
        public int Z { get; }

        /// <summary>
        /// Raw storage for bulk copies to and from NativeArray (generation, meshing).
        /// Whoever writes it directly must call <see cref="MarkDirty"/>; single blocks go through VoxelWorld.Set.
        /// </summary>
        public ushort[] Blocks { get; } = new ushort[Volume];

        /// <summary>
        /// Grows on every change; the mesher rebuilds a chunk when this differs from its last build.
        /// Not saved: after a load every chunk is meshed from scratch anyway.
        /// </summary>
        public int Version { get; private set; }

        public static int Index(int localX, int localY, int localZ)
        {
            return localX | (localZ << SizeShift) | (localY << LayerShift);
        }

        public ushort Get(int localX, int localY, int localZ) => Blocks[Index(localX, localY, localZ)];

        public void MarkDirty() => Version++;
    }
}
