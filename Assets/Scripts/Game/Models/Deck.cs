using System.Collections.Generic;
using UtilityToolkit.CollectionExtensions;

namespace Game.Models
{
    public class Deck
    {
        private Stack<Card> Cards { get; set; } = new();

        public Deck()
        {
            Reshuffle();
        }
        
        public Card Draw()
        {
            if (Cards.Count == 0) Reshuffle();
            
            return Cards.Pop();
        }

        public void Reshuffle()
        { 
            Cards.Clear();

            foreach (Card card in Card.FullDeck.Shuffle())
            {
                Cards.Push(card);
            }
        }
        
        public Card[] GetCards()
        {
            return Cards.ToArray();
        }

        public void Set(Card[] deck)
        {
            Cards = deck.ToStack();
        }
    }
}