using System.Collections.Generic;

namespace Game.Models
{
    public class MoveHistory
    {
        private List<Move> Moves { get; } = new();

        public void Add(Move move)
        {
            Moves.Add(move);
        }

        public void Clear()
        {
            Moves.Clear();
        }

        public Move[] GetMoves()
        {
            return Moves.ToArray();
        }
    }
}