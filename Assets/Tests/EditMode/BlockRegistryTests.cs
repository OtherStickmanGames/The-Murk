using System;
using System.Collections.Generic;
using Game.Sim.Blocks;
using NUnit.Framework;

namespace Game.Tests
{
    public class BlockRegistryTests
    {
        [Test]
        public void AirIsZero_AndJsonBlocksFollowInFileOrder()
        {
            var registry = BlockRegistry.FromJson(
                "[{\"id\":\"stone\",\"label\":\"Камень\",\"isSolid\":true},{\"id\":\"water\",\"label\":\"Вода\",\"isLiquid\":true}]");

            Assert.AreEqual(3, registry.Count);
            Assert.AreEqual(BlockRegistry.AirDefId, registry.Get(BlockRegistry.AirId).Id);
            Assert.IsFalse(registry.Get(BlockRegistry.AirId).IsSolid);
            Assert.AreEqual(1, registry.GetNumericId("stone"));
            Assert.AreEqual(2, registry.GetNumericId("water"));
            Assert.IsTrue(registry.Get("stone").IsSolid);
            Assert.IsTrue(registry.Get("water").IsLiquid);
            Assert.AreEqual("Камень", registry.Get("stone").Label);
        }

        [Test]
        public void DuplicateId_Throws()
        {
            Assert.Throws<InvalidOperationException>(() => BlockRegistry.FromJson(
                "[{\"id\":\"stone\",\"label\":\"a\"},{\"id\":\"stone\",\"label\":\"b\"}]"));
        }

        [Test]
        public void AirInJson_Throws()
        {
            Assert.Throws<InvalidOperationException>(() => BlockRegistry.FromJson("[{\"id\":\"air\",\"label\":\"a\"}]"));
        }

        [Test]
        public void UnknownField_Throws()
        {
            Assert.Throws<InvalidOperationException>(() => BlockRegistry.FromJson(
                "[{\"id\":\"stone\",\"label\":\"a\",\"isSolidd\":true}]"));
        }

        [Test]
        public void MissingLabel_Throws()
        {
            Assert.Throws<InvalidOperationException>(() => BlockRegistry.FromJson("[{\"id\":\"stone\"}]"));
        }

        [Test]
        public void UnknownBlock_Throws()
        {
            var registry = BlockRegistry.FromJson("[]");

            Assert.Throws<KeyNotFoundException>(() => registry.GetNumericId("stone"));
        }

        [Test]
        public void ShippedBlocks_Parse()
        {
            var registry = BlockRegistry.FromJson(TestDefs.Read("Blocks"));

            Assert.IsTrue(registry.Get("bedrock").IsIndestructible);
            Assert.IsTrue(registry.Get("water").IsLiquid);
            Assert.IsFalse(registry.Get("water").IsSolid);
        }
    }
}
