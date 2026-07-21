using System.Collections.Generic;
using System.Linq;
using Game.Models.Players;
using UnityEngine;
using UtilityToolkit.Runtime;

namespace Game.Models
{
    public class Board
    {
        private Dictionary<Position, Team> TakenSpaces { get; } = new();

        public bool TryAddPin(Position position, Team team) => TakenSpaces.TryAdd(position, team);

        public bool Fits(Position position) => !TakenSpaces.ContainsKey(position);

        public bool HasSequence(Team team) =>
            SequencePatterns.All().Any(pattern => IsSequence(pattern, team));

        public int SequenceCount(Team team) =>
            SequencePatterns.All().Count(pattern => IsSequence(pattern, team));

        private bool IsSequence(Position[] positions, Team team) =>
            positions.All(position =>
                TakenSpaces.TryGetValue(position, out Team occupyingTeam) && occupyingTeam == team);


        public void Clear() => TakenSpaces.Clear();

        public void Set(Move[] moves)
        {
            Clear();
            foreach (Move move in moves)
            {
                if (move.Card.IsRemover)
                {
                    TakenSpaces.Remove(move.Position);
                }
                else if (!TakenSpaces.TryAdd(move.Position, move.Team))
                {
                    Debug.LogError($"Duplicate move at {BoardLayout.Get(move.Position)}");
                    return;
                }
            }
        }

        public bool Remove(Position position)
        {
            return TakenSpaces.Remove(position);
        }

        public Option<Team> Owner(Position position)
        {
            return TakenSpaces.TryGetValue(position, out Team team) 
                ? Option<Team>.Some(team) 
                : Option<Team>.None;
        }
    }

    public enum Team
    {
        Red,
        Yellow
    }
}