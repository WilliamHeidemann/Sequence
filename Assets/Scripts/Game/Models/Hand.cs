using System.Collections.Generic;
using System.Linq;

namespace Game.Models
{
    public class Hand
    {
        private List<Card> Cards { get; set; } = new();

        public bool TryAdd(Card card)
        {
            if (Cards.Count >= 7)
                return false;
            
            Cards.Add(card);
            return true;
        }

        public bool TryRemove(Card card)
        {
            return Cards.Remove(card);
        }

        public IEnumerable<Card> DrawUntilFull(Deck deck)
        {
            while (Cards.Count < 7)
            {
                var card = deck.Draw();
                Cards.Add(card);
                yield return card;
            }
        }
        
        public bool Contains(Card card) => Cards.Contains(card);
        
        public int Count => Cards.Count;

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