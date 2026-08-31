using System;

namespace Game.Domain.Models
{
    public class ClientGameState
    {
        // Last move: last entry of Moves
        public Move[] Moves { get; set; } = Array.Empty<Move>();
        public Card[] Hand { get; set; } = Array.Empty<Card>();
        public Team Team { get; set; }
        public bool IsMyTurn { get; set; }
    }
}