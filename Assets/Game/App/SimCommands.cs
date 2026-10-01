using Game.DevTools;
using Game.Sim;

namespace Game.App
{
    /// <summary>Console commands for the simulation clock.</summary>
    public static class SimCommands
    {
        public static void Register(DebugCommandRegistry commands, GameSimulation simulation, SimClock clock)
        {
            commands.Register("speed", "speed <n> — run n simulation ticks per game tick (x1, x2, x3, x10...).", args =>
            {
                if (args.Length != 1 || !int.TryParse(args[0].TrimStart('x', 'X'), out var speed) || speed < 1)
                    return "Usage: speed <n>, n >= 1";
                clock.SetSpeed(speed);
                return $"Speed x{speed}";
            });

            commands.Register("pause", "Toggle simulation pause.", _ =>
            {
                clock.IsPaused = !clock.IsPaused;
                return clock.IsPaused ? "Paused" : "Running";
            });

            commands.Register("tick", "Show the current simulation tick.", _ =>
                $"Tick {simulation.Tick} ({simulation.Tick / (double)SimClock.TicksPerSecond:0.0} s game time), x{clock.Speed}");
        }
    }
}
