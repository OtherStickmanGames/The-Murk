using Game.Sim;
using Game.Sim.Blocks;
using NUnit.Framework;

namespace Game.Tests
{
    public class SimClockTests
    {
        [Test]
        public void OneSecondAtNormalSpeed_RunsTenTicks()
        {
            var clock = new SimClock();

            Assert.AreEqual(SimClock.TicksPerSecond, clock.Advance(1.0));
        }

        [Test]
        public void SpeedMultipliesTickCount()
        {
            var clock = new SimClock();
            clock.SetSpeed(3);

            Assert.AreEqual(3 * SimClock.TicksPerSecond, clock.Advance(1.0));
        }

        [Test]
        public void ShortFramesAccumulateIntoTicks()
        {
            var clock = new SimClock();

            var total = 0;
            for (var i = 0; i < 30; i++)
                total += clock.Advance(1.0 / 30);

            Assert.AreEqual(SimClock.TicksPerSecond, total);
        }

        [Test]
        public void PausedClock_RunsNoTicks()
        {
            var clock = new SimClock { IsPaused = true };

            Assert.AreEqual(0, clock.Advance(1.0));
        }

        [Test]
        public void LongHitch_IsCappedAndBacklogDropped()
        {
            var clock = new SimClock();

            Assert.AreEqual(SimClock.MaxTicksPerAdvance, clock.Advance(60.0));
            Assert.AreEqual(1, clock.Advance(0.1));
        }

        [Test]
        public void SpeedBelowOne_Throws()
        {
            var clock = new SimClock();

            Assert.Throws<System.ArgumentOutOfRangeException>(() => clock.SetSpeed(0));
        }

        [Test]
        public void SimulationStep_IncrementsTick()
        {
            var sim = new GameSimulation(BlockRegistry.FromJson("[]"), new VoxelWorld(1, 1, 1), 1);
            sim.Step();
            sim.Step();

            Assert.AreEqual(2, sim.Tick);
        }
    }
}
