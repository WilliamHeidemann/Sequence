using System;

namespace Game.Models.Players
{
    public class GameStateData
    {
        public Move[] Moves { get; set; } = Array.Empty<Move>();
        public Card[] Deck { get; set; } = Array.Empty<Card>();
        public Card[] RedHand { get; set; } = Array.Empty<Card>();
        public Card[] YellowHand { get; set; } = Array.Empty<Card>();

        // Verify state: moves + cards in hands + cards in deck should equal 52
        // Last move: last index of Moves
    }
}