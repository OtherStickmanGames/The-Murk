using System;

namespace Game.Sim
{
    /// <summary>
    /// Converts real frame time into a whole number of fixed simulation ticks.
    /// Speed-up runs more ticks, never a longer tick, so the simulation stays deterministic.
    /// </summary>
    public sealed class SimClock
    {
        public const int TicksPerSecond = 10;

        // Guards against a death spiral after a long hitch (app resume, breakpoint):
        // the backlog beyond this is dropped instead of being simulated in one frame.
        public const int MaxTicksPerAdvance = 30;

        // Absorbs float error so that e.g. 0.1 s at x1 yields exactly one tick.
        private const double Epsilon = 1e-9;

        private double _pendingTicks;

        public int Speed { get; private set; } = 1;
        public bool IsPaused { get; set; }

        public void SetSpeed(int speed)
        {
            if (speed < 1)
                throw new ArgumentOutOfRangeException(nameof(speed), speed, "Speed must be at least 1.");
            Speed = speed;
        }

        /// <summary>Accumulates elapsed real time and returns how many ticks to run now.</summary>
        public int Advance(double realSeconds)
        {
            if (IsPaused || realSeconds <= 0)
                return 0;

            _pendingTicks += realSeconds * TicksPerSecond * Speed;
            var ticks = (int)Math.Floor(_pendingTicks + Epsilon);

            if (ticks > MaxTicksPerAdvance)
            {
                _pendingTicks = 0;
                return MaxTicksPerAdvance;
            }

            _pendingTicks = Math.Max(0, _pendingTicks - ticks);
            return ticks;
        }
    }
}
