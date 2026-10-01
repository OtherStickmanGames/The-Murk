using System;

namespace Game.App
{
    /// <summary>Picks a fresh world seed. Only the seed is random; everything generated from it is deterministic.</summary>
    public static class SeedSource
    {
        public static uint NewSeed() => unchecked((uint)Guid.NewGuid().GetHashCode());
    }
}
