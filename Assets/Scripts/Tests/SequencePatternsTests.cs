using System.Linq;
using Game.Domain;
using NUnit.Framework;

namespace Tests
{
    public class SequencePatternsTests
    {
        [Test]
        public void LengthOfAllPatterns_Is84()
        {
            var patterns = SequencePatterns.All().ToArray();
            Assert.That(patterns.Length, Is.EqualTo(84));
        }
    }
}
