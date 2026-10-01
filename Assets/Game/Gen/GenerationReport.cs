using System.Collections.Generic;
using System.Linq;

namespace Game.Gen
{
    /// <summary>What a generation run took and produced; printed to the log.</summary>
    public sealed class GenerationReport
    {
        public GenerationReport(uint seed, int sizeX, int sizeY, int sizeZ, double heightMapMs, double fillMs,
            double copyMs, double totalMs, IReadOnlyDictionary<string, int> blockCounts)
        {
            Seed = seed;
            SizeX = sizeX;
            SizeY = sizeY;
            SizeZ = sizeZ;
            HeightMapMs = heightMapMs;
            FillMs = fillMs;
            CopyMs = copyMs;
            TotalMs = totalMs;
            BlockCounts = blockCounts;
        }

        public uint Seed { get; }
        public int SizeX { get; }
        public int SizeY { get; }
        public int SizeZ { get; }
        public double HeightMapMs { get; }
        public double FillMs { get; }
        public double CopyMs { get; }
        /// <summary>Also covers settings, allocation and block counting on top of the three phases.</summary>
        public double TotalMs { get; }
        /// <summary>Count of every block type by id, air included.</summary>
        public IReadOnlyDictionary<string, int> BlockCounts { get; }

        public override string ToString()
        {
            var blocks = string.Join(", ", BlockCounts.Where(p => p.Value > 0).Select(p => $"{p.Key} {p.Value}"));
            return $"Region {SizeX}×{SizeY}×{SizeZ}, seed {Seed}: {TotalMs:0.0} ms " +
                   $"(heights {HeightMapMs:0.0}, fill {FillMs:0.0}, copy {CopyMs:0.0}). Blocks: {blocks}";
        }
    }
}
