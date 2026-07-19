using System.Collections.Generic;
using System.Linq;

namespace Game.Models
{
    public class Hand
    {
        private List<Card> Cards { get; set; } = new();

        public bool TryAdd(Card card)
        {
            if (IsFull)
                return false;
            
            Cards.Add(card);
            return true;
        }

        public bool TryRemove(Card card)
        {
            return Cards.Remove(card);
        }
        
        public bool Contains(Card card) => Cards.Contains(card);
        
        public int Count => Cards.Count;
        public bool IsFull => Count == 7;

        public Card[] GetCards() => Cards.ToArray();

        public override string ToString()
        {
            return string.Join(", ", GetCards());
        }

        public void Set(Card[] opponentHand)
        {
            Cards = opponentHand.ToList();
        }
    }
}