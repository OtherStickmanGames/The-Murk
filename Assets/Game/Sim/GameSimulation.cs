namespace Game.Sim
{
    /// <summary>
    /// Root of the simulation state. Advanced only in whole fixed ticks; knows nothing about Unity.
    /// </summary>
    public sealed class GameSimulation
    {
        public long Tick { get; private set; }

        public void Step()
        {
            Tick++;
        }
    }
}
