using System.Collections.Generic;
using System.Linq;

namespace Game.Domain.Models
{
    public class MoveHistory
    {
        private List<Move> Moves { get; set; } = new();

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

        public void Set(Move[] moves)
        {
            Moves = moves.ToList();
        }
    }
}