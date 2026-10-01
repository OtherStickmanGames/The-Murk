using System;
using System.Collections.Generic;

namespace Game.Sim.Blocks
{
    /// <summary>
    /// All block types. Numeric id 0 is always air; JSON blocks get 1..N in file order.
    /// Numeric ids shift when Blocks.json changes, so saves must map them to string ids.
    /// </summary>
    public sealed class BlockRegistry
    {
        public const ushort AirId = 0;
        public const string AirDefId = "air";

        private readonly List<BlockDef> _byNumericId = new List<BlockDef>();
        private readonly Dictionary<string, BlockDef> _byId = new Dictionary<string, BlockDef>(StringComparer.Ordinal);

        private BlockRegistry(IEnumerable<BlockDef> defs)
        {
            // Air is the absence of a block, not content, so it lives in code.
            Add(new BlockDef { Id = AirDefId, Label = AirDefId });
            foreach (var def in defs)
            {
                if (def.Id == AirDefId)
                    throw new InvalidOperationException($"Block id '{AirDefId}' is reserved.");
                Add(def);
            }
        }

        public int Count => _byNumericId.Count;

        public static BlockRegistry FromJson(string json)
        {
            return new BlockRegistry(DefJson.Deserialize<List<BlockDef>>(json, "Blocks.json"));
        }

        public BlockDef Get(ushort numericId) => _byNumericId[numericId];

        public BlockDef Get(string id)
        {
            if (!_byId.TryGetValue(id, out var def))
                throw new KeyNotFoundException($"Unknown block '{id}'.");
            return def;
        }

        public ushort GetNumericId(string id) => Get(id).NumericId;

        private void Add(BlockDef def)
        {
            if (_byNumericId.Count > ushort.MaxValue)
                throw new InvalidOperationException("Too many block types.");
            if (_byId.ContainsKey(def.Id))
                throw new InvalidOperationException($"Duplicate block id '{def.Id}'.");

            def.NumericId = (ushort)_byNumericId.Count;
            _byNumericId.Add(def);
            _byId.Add(def.Id, def);
        }
    }
}
