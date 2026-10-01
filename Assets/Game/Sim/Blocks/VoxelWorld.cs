using System;

namespace Game.Sim.Blocks
{
    /// <summary>
    /// Block storage of the whole region, split into chunks. Y is up; block (x, y, z) fills [x, x+1) etc.
    /// Chunk index = cx + cz * ChunksX + cy * ChunksX * ChunksZ (generator jobs rely on this order).
    /// </summary>
    public sealed class VoxelWorld
    {
        private readonly Chunk[] _chunks;

        public VoxelWorld(int chunksX, int chunksY, int chunksZ)
        {
            if (chunksX < 1 || chunksY < 1 || chunksZ < 1)
                throw new ArgumentOutOfRangeException(nameof(chunksX), "World must have at least one chunk per axis.");

            ChunksX = chunksX;
            ChunksY = chunksY;
            ChunksZ = chunksZ;
            _chunks = new Chunk[chunksX * chunksY * chunksZ];
            for (var cy = 0; cy < chunksY; cy++)
            for (var cz = 0; cz < chunksZ; cz++)
            for (var cx = 0; cx < chunksX; cx++)
                _chunks[ChunkIndex(cx, cy, cz)] = new Chunk(cx, cy, cz);
        }

        public int ChunksX { get; }
        public int ChunksY { get; }
        public int ChunksZ { get; }
        public int ChunkCount => _chunks.Length;

        public int SizeX => ChunksX * Chunk.Size;
        public int SizeY => ChunksY * Chunk.Size;
        public int SizeZ => ChunksZ * Chunk.Size;

        public Chunk GetChunk(int index) => _chunks[index];

        public Chunk GetChunk(int cx, int cy, int cz)
        {
            if ((uint)cx >= (uint)ChunksX || (uint)cy >= (uint)ChunksY || (uint)cz >= (uint)ChunksZ)
                throw new ArgumentOutOfRangeException(nameof(cx), $"Chunk ({cx}, {cy}, {cz}) is outside the world.");
            return _chunks[ChunkIndex(cx, cy, cz)];
        }

        public bool IsInBounds(int x, int y, int z)
        {
            return (uint)x < (uint)SizeX && (uint)y < (uint)SizeY && (uint)z < (uint)SizeZ;
        }

        public ushort Get(int x, int y, int z)
        {
            CheckBounds(x, y, z);
            return ChunkAt(x, y, z).Get(x & Chunk.SizeMask, y & Chunk.SizeMask, z & Chunk.SizeMask);
        }

        public void Set(int x, int y, int z, ushort block)
        {
            CheckBounds(x, y, z);
            int lx = x & Chunk.SizeMask, ly = y & Chunk.SizeMask, lz = z & Chunk.SizeMask;
            var chunk = ChunkAt(x, y, z);
            var index = Chunk.Index(lx, ly, lz);
            if (chunk.Blocks[index] == block)
                return;

            chunk.Blocks[index] = block;
            chunk.MarkDirty();

            // A block on a chunk face also changes which faces the neighbour must draw.
            MarkNeighbourDirty(chunk.X - 1, chunk.Y, chunk.Z, lx == 0);
            MarkNeighbourDirty(chunk.X + 1, chunk.Y, chunk.Z, lx == Chunk.SizeMask);
            MarkNeighbourDirty(chunk.X, chunk.Y - 1, chunk.Z, ly == 0);
            MarkNeighbourDirty(chunk.X, chunk.Y + 1, chunk.Z, ly == Chunk.SizeMask);
            MarkNeighbourDirty(chunk.X, chunk.Y, chunk.Z - 1, lz == 0);
            MarkNeighbourDirty(chunk.X, chunk.Y, chunk.Z + 1, lz == Chunk.SizeMask);
        }

        private int ChunkIndex(int cx, int cy, int cz) => cx + cz * ChunksX + cy * ChunksX * ChunksZ;

        private Chunk ChunkAt(int x, int y, int z)
        {
            return _chunks[ChunkIndex(x >> Chunk.SizeShift, y >> Chunk.SizeShift, z >> Chunk.SizeShift)];
        }

        private void MarkNeighbourDirty(int cx, int cy, int cz, bool onFace)
        {
            if (onFace && (uint)cx < (uint)ChunksX && (uint)cy < (uint)ChunksY && (uint)cz < (uint)ChunksZ)
                _chunks[ChunkIndex(cx, cy, cz)].MarkDirty();
        }

        private void CheckBounds(int x, int y, int z)
        {
            if (!IsInBounds(x, y, z))
                throw new ArgumentOutOfRangeException(nameof(x), $"Block ({x}, {y}, {z}) is outside the world {SizeX}×{SizeY}×{SizeZ}.");
        }
    }
}
