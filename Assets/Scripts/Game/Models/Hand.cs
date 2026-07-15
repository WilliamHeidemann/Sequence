using System.Collections.Generic;

namespace Game.Models
{
    public class Hand
    {
        private List<Card> Cards { get; } = new();

        public bool TryAdd(Card card)
        {
            if (Cards.Count > 7)
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

        public Card[] GetCards() => Cards.ToArray();

        public override string ToString()
        {
            return string.Join(", ", GetCards());
        }
    }
}