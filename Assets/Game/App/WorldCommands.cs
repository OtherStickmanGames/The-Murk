using System;
using System.Collections.Generic;
using System.Text;
using Game.DevTools;
using Game.Gen;
using Game.Sim;

namespace Game.App
{
    /// <summary>Console commands for the voxel world and its generation.</summary>
    public static class WorldCommands
    {
        public static void Register(DebugCommandRegistry commands, GameSimulation simulation, RegionGenerator generator,
            Action<GenerationReport> onRegenerated)
        {
            commands.Register("seed", "Show the seed of the current world.", _ => $"Seed {simulation.WorldSeed}");

            commands.Register("regen", "regen [seed] — generate the region again, with a new random seed if none given.", args =>
            {
                uint seed;
                if (args.Length == 0)
                    seed = SeedSource.NewSeed();
                else if (args.Length != 1 || !uint.TryParse(args[0], out seed))
                    return "Usage: regen [seed], seed is 0..4294967295";

                var report = generator.Generate(simulation.World, seed);
                simulation.WorldSeed = seed;
                onRegenerated(report);
                return report.ToString();
            });

            commands.Register("block", "block x y z — show the block at a position.", args =>
            {
                if (!TryParseInts(args, 3, out var p))
                    return "Usage: block x y z";
                if (!simulation.World.IsInBounds(p[0], p[1], p[2]))
                    return "Outside the world";
                var def = simulation.Blocks.Get(simulation.World.Get(p[0], p[1], p[2]));
                return $"({p[0]}, {p[1]}, {p[2]}): {def.Id} «{def.Label}»";
            });

            commands.Register("column", "column x z — surface height and the blocks of a column, top to bottom.", args =>
            {
                if (!TryParseInts(args, 2, out var p))
                    return "Usage: column x z";
                if (!simulation.World.IsInBounds(p[0], 0, p[1]))
                    return "Outside the world";
                return DescribeColumn(simulation, p[0], p[1]);
            });
        }

        private static string DescribeColumn(GameSimulation simulation, int x, int z)
        {
            var world = simulation.World;
            var surface = -1;
            var runs = new List<string>();
            var y = world.SizeY - 1;
            while (y >= 0)
            {
                var block = world.Get(x, y, z);
                var top = y;
                while (y >= 0 && world.Get(x, y, z) == block)
                    y--;

                var def = simulation.Blocks.Get(block);
                if (surface < 0 && def.IsSolid)
                    surface = top;
                runs.Add(top == y + 1 ? $"{top} {def.Id}" : $"{top}..{y + 1} {def.Id}");
            }

            var text = new StringBuilder();
            text.Append($"Column ({x}, {z}): surface {surface}");
            foreach (var run in runs)
                text.Append('\n').Append(run);
            return text.ToString();
        }

        private static bool TryParseInts(string[] args, int count, out int[] values)
        {
            values = new int[count];
            if (args.Length != count)
                return false;
            for (var i = 0; i < count; i++)
                if (!int.TryParse(args[i], out values[i]))
                    return false;
            return true;
        }
    }
}
