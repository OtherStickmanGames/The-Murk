using System;
using Game.Sim.Blocks;
using NUnit.Framework;

namespace Game.Tests
{
    public class VoxelWorldTests
    {
        [Test]
        public void SizeIsChunksTimesChunkSize()
        {
            var world = new VoxelWorld(5, 2, 5);

            Assert.AreEqual(160, world.SizeX);
            Assert.AreEqual(64, world.SizeY);
            Assert.AreEqual(160, world.SizeZ);
            Assert.AreEqual(50, world.ChunkCount);
        }

        [Test]
        public void SetThenGet_RoundTripsAcrossChunkBorders()
        {
            var world = new VoxelWorld(2, 2, 2);
            var points = new[] { (0, 0, 0), (31, 31, 31), (32, 32, 32), (31, 32, 33), (63, 63, 63), (5, 40, 60) };

            ushort value = 1;
            foreach (var (x, y, z) in points)
                world.Set(x, y, z, value++);

            value = 1;
            foreach (var (x, y, z) in points)
                Assert.AreEqual(value++, world.Get(x, y, z), $"({x}, {y}, {z})");
        }

        [Test]
        public void BlockLandsInItsChunkAtLocalIndex()
        {
            var world = new VoxelWorld(2, 2, 2);

            world.Set(33, 34, 35, 7);

            Assert.AreEqual(7, world.GetChunk(1, 1, 1).Blocks[Chunk.Index(1, 2, 3)]);
        }

        [Test]
        public void OutOfBounds_Throws()
        {
            var world = new VoxelWorld(1, 1, 1);

            Assert.IsFalse(world.IsInBounds(-1, 0, 0));
            Assert.IsFalse(world.IsInBounds(0, 32, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => world.Get(32, 0, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => world.Set(0, -1, 0, 1));
        }

        [Test]
        public void Set_BumpsVersionOnlyOnChange()
        {
            var world = new VoxelWorld(1, 1, 1);
            var chunk = world.GetChunk(0, 0, 0);

            world.Set(5, 5, 5, 3);
            var version = chunk.Version;
            world.Set(5, 5, 5, 3);

            Assert.AreEqual(1, version);
            Assert.AreEqual(version, chunk.Version);
        }

        [Test]
        public void SetOnChunkFace_BumpsNeighbourVersion()
        {
            var world = new VoxelWorld(3, 1, 1);
            var left = world.GetChunk(0, 0, 0);
            var middle = world.GetChunk(1, 0, 0);
            var right = world.GetChunk(2, 0, 0);

            world.Set(32, 10, 10, 1);

            Assert.AreEqual(1, left.Version);
            Assert.AreEqual(1, middle.Version);
            Assert.AreEqual(0, right.Version);
        }

        [Test]
        public void SetInsideChunk_LeavesNeighboursAlone()
        {
            var world = new VoxelWorld(3, 1, 1);

            world.Set(40, 10, 10, 1);

            Assert.AreEqual(0, world.GetChunk(0, 0, 0).Version);
            Assert.AreEqual(1, world.GetChunk(1, 0, 0).Version);
            Assert.AreEqual(0, world.GetChunk(2, 0, 0).Version);
        }
    }
}
