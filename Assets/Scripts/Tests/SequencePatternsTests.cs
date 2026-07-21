using System.Collections;
using System.Linq;
using Game.Models;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class SequencePatternsTests
{
    [Test]
    public void LengthOfAllPatterns_Is84()
    {
        var patterns = SequencePatterns.All().ToArray();
        Assert.That(patterns.Length, Is.EqualTo(84));
    }
}
