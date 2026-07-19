using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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

        public void Set(Move[] moves)
        {
            Clear();
            foreach (var move in moves)
            {
                if (!TakenSpaces.TryAdd(move.Position, move.Team))
                {
                    Debug.LogError($"Duplicate move at {move.Position}");
                    return;
                }
            }
        }
    }

    public enum Team
    {
        Red,
        Yellow
    }
}