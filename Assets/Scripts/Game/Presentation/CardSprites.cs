using System;
using Game.Domain.Models;
using UnityEngine;

namespace Game.Presentation
{
    [CreateAssetMenu(fileName = "CardSprites", menuName = "CardSprites")]
    public class CardSprites : ScriptableObject
    {
        [SerializeField] private Sprite _moon;
        [SerializeField] private Sprite _sun;
        // [SerializeField] private Sprite _moonKing;
        [SerializeField] private Sprite _moonQueen;
        [SerializeField] private Sprite _sunKing;
        [SerializeField] private Sprite _sunQueen;

        public Sprite Get(Symbol symbol)
        {
            return symbol switch
            {
                Symbol.Sun => _sun,
                Symbol.Moon => _moon,
                _ => throw new ArgumentOutOfRangeException(nameof(symbol), symbol, null)
            };
        }
        
        public Sprite Get(Card card)
        {
            return (card.Symbol, card.Rank) switch
            {
                // (Symbol.Moon, Rank.Queen) => _moonQueen,
                // (Symbol.Sun, Rank.King) => _sunKing,
                // (Symbol.Sun, Rank.Queen) => _sunQueen,
                (Symbol.Moon, _) => _moon,
                (Symbol.Sun, _) => _sun,
                _ => throw new ArgumentException($"No sprite found for card: {card.Symbol} {card.Rank}")
            };
        }
    }
}