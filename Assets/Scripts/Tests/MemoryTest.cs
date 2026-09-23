using System;
using Game.Domain.Models;
using NUnit.Framework;

namespace Tests
{
    public class MemoryTest
    {
        [Test]
        public void TestGameStateByteSize()
        {
            long start = GC.GetAllocatedBytesForCurrentThread();
            GameState gameState = GameState.CreateInitial();
            long end = GC.GetAllocatedBytesForCurrentThread();
            long totalBytes = end - start;
            Assert.That(gameState, Is.Not.Null);
            Assert.That(totalBytes, Is.GreaterThan(0));
        }
    }
}