using System;
using Game.Domain;
using Game.Domain.Models;
using UnityEngine;

namespace Game.Presentation
{
    [CreateAssetMenu(fileName = "CardSprites", menuName = "CardSprites")]
    public class CardSprites : ScriptableObject
    {
        [SerializeField] private Sprite _moon;
        [SerializeField] private Sprite _sun;
        
        public Sprite Get(Card card)
        {
            return (card.Symbol, card.Rank) switch
            {
                (Symbol.Moon, _) => _moon,
                (Symbol.Sun, _) => _sun,
                _ => throw new ArgumentException($"No sprite found for card: {card.Symbol} {card.Rank}")
            };
        }
    }
}