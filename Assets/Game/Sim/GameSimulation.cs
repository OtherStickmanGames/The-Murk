using Game.Sim.Blocks;

namespace Game.Sim
{
    /// <summary>
    /// Root of the simulation state. Advanced only in whole fixed ticks; knows nothing about Unity.
    /// </summary>
    public sealed class GameSimulation
    {
        public GameSimulation(BlockRegistry blocks, VoxelWorld world, uint worldSeed)
        {
            Blocks = blocks;
            World = world;
            WorldSeed = worldSeed;
        }

        public long Tick { get; private set; }
        public BlockRegistry Blocks { get; }
        public VoxelWorld World { get; }

        /// <summary>Seed the current world was generated from; changes when the region is regenerated.</summary>
        public uint WorldSeed { get; set; }

        public void Step()
        {
            Tick++;
        }
    }
}
