using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.Models
{
    public class Board
    {
        private Dictionary<Position, Team> TakenSpaces { get; } = new();

        public bool TryAddPin(Position position, Team team) => TakenSpaces.TryAdd(position, team);

        public bool HasSequence(Team team) =>
            BoardLayout.AllSequencePatterns().Any(pattern => IsSequence(pattern, team));

        public int SequenceCount(Team team) =>
            BoardLayout.AllSequencePatterns().Count(pattern => IsSequence(pattern, team));

        private bool IsSequence(Position[] positions, Team team) =>
            positions.All(position =>
                TakenSpaces.TryGetValue(position, out Team occupyingTeam) && occupyingTeam == team);


        public void Clear() => TakenSpaces.Clear();
    }

    public enum Team
    {
        Red,
        Yellow
    }
}