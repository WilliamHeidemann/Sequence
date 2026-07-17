using System;
using Game.Models;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "CardSprites", menuName = "CardSprites")]
    public class CardSprites : ScriptableObject
    {
        [Header("Twos")] [SerializeField] private Sprite _twoOfClubs;
        [SerializeField] private Sprite _twoOfDiamonds;
        [SerializeField] private Sprite _twoOfHearts;
        [SerializeField] private Sprite _twoOfSpades;

        [Header("Threes")] [SerializeField] private Sprite _threeOfClubs;
        [SerializeField] private Sprite _threeOfDiamonds;
        [SerializeField] private Sprite _threeOfHearts;
        [SerializeField] private Sprite _threeOfSpades;

        [Header("Fours")] [SerializeField] private Sprite _fourOfClubs;
        [SerializeField] private Sprite _fourOfDiamonds;
        [SerializeField] private Sprite _fourOfHearts;
        [SerializeField] private Sprite _fourOfSpades;

        [Header("Fives")] [SerializeField] private Sprite _fiveOfClubs;
        [SerializeField] private Sprite _fiveOfDiamonds;
        [SerializeField] private Sprite _fiveOfHearts;
        [SerializeField] private Sprite _fiveOfSpades;

        [Header("Sixes")] [SerializeField] private Sprite _sixOfClubs;
        [SerializeField] private Sprite _sixOfDiamonds;
        [SerializeField] private Sprite _sixOfHearts;
        [SerializeField] private Sprite _sixOfSpades;

        [Header("Sevens")] [SerializeField] private Sprite _sevenOfClubs;
        [SerializeField] private Sprite _sevenOfDiamonds;
        [SerializeField] private Sprite _sevenOfHearts;
        [SerializeField] private Sprite _sevenOfSpades;

        [Header("Eights")] [SerializeField] private Sprite _eightOfClubs;
        [SerializeField] private Sprite _eightOfDiamonds;
        [SerializeField] private Sprite _eightOfHearts;
        [SerializeField] private Sprite _eightOfSpades;

        [Header("Nines")] [SerializeField] private Sprite _nineOfClubs;
        [SerializeField] private Sprite _nineOfDiamonds;
        [SerializeField] private Sprite _nineOfHearts;
        [SerializeField] private Sprite _nineOfSpades;

        [Header("Tens")] [SerializeField] private Sprite _tenOfClubs;
        [SerializeField] private Sprite _tenOfDiamonds;
        [SerializeField] private Sprite _tenOfHearts;
        [SerializeField] private Sprite _tenOfSpades;

        [Header("Jacks")] [SerializeField] private Sprite _jackOfClubs;
        [SerializeField] private Sprite _jackOfDiamonds;
        [SerializeField] private Sprite _jackOfHearts;
        [SerializeField] private Sprite _jackOfSpades;

        [Header("Queens")] [SerializeField] private Sprite _queenOfClubs;
        [SerializeField] private Sprite _queenOfDiamonds;
        [SerializeField] private Sprite _queenOfHearts;
        [SerializeField] private Sprite _queenOfSpades;

        [Header("Kings")] [SerializeField] private Sprite _kingOfClubs;
        [SerializeField] private Sprite _kingOfDiamonds;
        [SerializeField] private Sprite _kingOfHearts;
        [SerializeField] private Sprite _kingOfSpades;

        [Header("Aces")] [SerializeField] private Sprite _aceOfClubs;
        [SerializeField] private Sprite _aceOfDiamonds;
        [SerializeField] private Sprite _aceOfHearts;
        [SerializeField] private Sprite _aceOfSpades;

        public Sprite Get(Card card)
        {
            return (card.Suit, card.Rank) switch
            {
                // Hearts
                (Suit.Hearts, Rank.Ace) => _aceOfHearts,
                (Suit.Hearts, Rank.Two) => _twoOfHearts,
                (Suit.Hearts, Rank.Three) => _threeOfHearts,
                (Suit.Hearts, Rank.Four) => _fourOfHearts,
                (Suit.Hearts, Rank.Five) => _fiveOfHearts,
                (Suit.Hearts, Rank.Six) => _sixOfHearts,
                (Suit.Hearts, Rank.Seven) => _sevenOfHearts,
                (Suit.Hearts, Rank.Eight) => _eightOfHearts,
                (Suit.Hearts, Rank.Nine) => _nineOfHearts,
                (Suit.Hearts, Rank.Ten) => _tenOfHearts,
                (Suit.Hearts, Rank.Jack) => _jackOfHearts,
                (Suit.Hearts, Rank.Queen) => _queenOfHearts,
                (Suit.Hearts, Rank.King) => _kingOfHearts,

                // Diamonds
                (Suit.Diamonds, Rank.Ace) => _aceOfDiamonds,
                (Suit.Diamonds, Rank.Two) => _twoOfDiamonds,
                (Suit.Diamonds, Rank.Three) => _threeOfDiamonds,
                (Suit.Diamonds, Rank.Four) => _fourOfDiamonds,
                (Suit.Diamonds, Rank.Five) => _fiveOfDiamonds,
                (Suit.Diamonds, Rank.Six) => _sixOfDiamonds,
                (Suit.Diamonds, Rank.Seven) => _sevenOfDiamonds,
                (Suit.Diamonds, Rank.Eight) => _eightOfDiamonds,
                (Suit.Diamonds, Rank.Nine) => _nineOfDiamonds,
                (Suit.Diamonds, Rank.Ten) => _tenOfDiamonds,
                (Suit.Diamonds, Rank.Jack) => _jackOfDiamonds,
                (Suit.Diamonds, Rank.Queen) => _queenOfDiamonds,
                (Suit.Diamonds, Rank.King) => _kingOfDiamonds,

                // Clubs
                (Suit.Clubs, Rank.Ace) => _aceOfClubs,
                (Suit.Clubs, Rank.Two) => _twoOfClubs,
                (Suit.Clubs, Rank.Three) => _threeOfClubs,
                (Suit.Clubs, Rank.Four) => _fourOfClubs,
                (Suit.Clubs, Rank.Five) => _fiveOfClubs,
                (Suit.Clubs, Rank.Six) => _sixOfClubs,
                (Suit.Clubs, Rank.Seven) => _sevenOfClubs,
                (Suit.Clubs, Rank.Eight) => _eightOfClubs,
                (Suit.Clubs, Rank.Nine) => _nineOfClubs,
                (Suit.Clubs, Rank.Ten) => _tenOfClubs,
                (Suit.Clubs, Rank.Jack) => _jackOfClubs,
                (Suit.Clubs, Rank.Queen) => _queenOfClubs,
                (Suit.Clubs, Rank.King) => _kingOfClubs,

                // Spades
                (Suit.Spades, Rank.Ace) => _aceOfSpades,
                (Suit.Spades, Rank.Two) => _twoOfSpades,
                (Suit.Spades, Rank.Three) => _threeOfSpades,
                (Suit.Spades, Rank.Four) => _fourOfSpades,
                (Suit.Spades, Rank.Five) => _fiveOfSpades,
                (Suit.Spades, Rank.Six) => _sixOfSpades,
                (Suit.Spades, Rank.Seven) => _sevenOfSpades,
                (Suit.Spades, Rank.Eight) => _eightOfSpades,
                (Suit.Spades, Rank.Nine) => _nineOfSpades,
                (Suit.Spades, Rank.Ten) => _tenOfSpades,
                (Suit.Spades, Rank.Jack) => _jackOfSpades,
                (Suit.Spades, Rank.Queen) => _queenOfSpades,
                (Suit.Spades, Rank.King) => _kingOfSpades,

                // Fallback
                _ => throw new ArgumentException($"No sprite found for card: {card.Suit} {card.Rank}")
            };
        }

        public void Set(Card card, Sprite sprite)
        {
            switch (card.Suit, card.Rank)
            {
                // Hearts
                case (Suit.Hearts, Rank.Ace): _aceOfHearts = sprite; break;
                case (Suit.Hearts, Rank.Two): _twoOfHearts = sprite; break;
                case (Suit.Hearts, Rank.Three): _threeOfHearts = sprite; break;
                case (Suit.Hearts, Rank.Four): _fourOfHearts = sprite; break;
                case (Suit.Hearts, Rank.Five): _fiveOfHearts = sprite; break;
                case (Suit.Hearts, Rank.Six): _sixOfHearts = sprite; break;
                case (Suit.Hearts, Rank.Seven): _sevenOfHearts = sprite; break;
                case (Suit.Hearts, Rank.Eight): _eightOfHearts = sprite; break;
                case (Suit.Hearts, Rank.Nine): _nineOfHearts = sprite; break;
                case (Suit.Hearts, Rank.Ten): _tenOfHearts = sprite; break;
                case (Suit.Hearts, Rank.Jack): _jackOfHearts = sprite; break;
                case (Suit.Hearts, Rank.Queen): _queenOfHearts = sprite; break;
                case (Suit.Hearts, Rank.King): _kingOfHearts = sprite; break;

                // Diamonds
                case (Suit.Diamonds, Rank.Ace): _aceOfDiamonds = sprite; break;
                case (Suit.Diamonds, Rank.Two): _twoOfDiamonds = sprite; break;
                case (Suit.Diamonds, Rank.Three): _threeOfDiamonds = sprite; break;
                case (Suit.Diamonds, Rank.Four): _fourOfDiamonds = sprite; break;
                case (Suit.Diamonds, Rank.Five): _fiveOfDiamonds = sprite; break;
                case (Suit.Diamonds, Rank.Six): _sixOfDiamonds = sprite; break;
                case (Suit.Diamonds, Rank.Seven): _sevenOfDiamonds = sprite; break;
                case (Suit.Diamonds, Rank.Eight): _eightOfDiamonds = sprite; break;
                case (Suit.Diamonds, Rank.Nine): _nineOfDiamonds = sprite; break;
                case (Suit.Diamonds, Rank.Ten): _tenOfDiamonds = sprite; break;
                case (Suit.Diamonds, Rank.Jack): _jackOfDiamonds = sprite; break;
                case (Suit.Diamonds, Rank.Queen): _queenOfDiamonds = sprite; break;
                case (Suit.Diamonds, Rank.King): _kingOfDiamonds = sprite; break;

                // Clubs
                case (Suit.Clubs, Rank.Ace): _aceOfClubs = sprite; break;
                case (Suit.Clubs, Rank.Two): _twoOfClubs = sprite; break;
                case (Suit.Clubs, Rank.Three): _threeOfClubs = sprite; break;
                case (Suit.Clubs, Rank.Four): _fourOfClubs = sprite; break;
                case (Suit.Clubs, Rank.Five): _fiveOfClubs = sprite; break;
                case (Suit.Clubs, Rank.Six): _sixOfClubs = sprite; break;
                case (Suit.Clubs, Rank.Seven): _sevenOfClubs = sprite; break;
                case (Suit.Clubs, Rank.Eight): _eightOfClubs = sprite; break;
                case (Suit.Clubs, Rank.Nine): _nineOfClubs = sprite; break;
                case (Suit.Clubs, Rank.Ten): _tenOfClubs = sprite; break;
                case (Suit.Clubs, Rank.Jack): _jackOfClubs = sprite; break;
                case (Suit.Clubs, Rank.Queen): _queenOfClubs = sprite; break;
                case (Suit.Clubs, Rank.King): _kingOfClubs = sprite; break;

                // Spades
                case (Suit.Spades, Rank.Ace): _aceOfSpades = sprite; break;
                case (Suit.Spades, Rank.Two): _twoOfSpades = sprite; break;
                case (Suit.Spades, Rank.Three): _threeOfSpades = sprite; break;
                case (Suit.Spades, Rank.Four): _fourOfSpades = sprite; break;
                case (Suit.Spades, Rank.Five): _fiveOfSpades = sprite; break;
                case (Suit.Spades, Rank.Six): _sixOfSpades = sprite; break;
                case (Suit.Spades, Rank.Seven): _sevenOfSpades = sprite; break;
                case (Suit.Spades, Rank.Eight): _eightOfSpades = sprite; break;
                case (Suit.Spades, Rank.Nine): _sixOfSpades = sprite; break;
                case (Suit.Spades, Rank.Ten): _tenOfSpades = sprite; break;
                case (Suit.Spades, Rank.Jack): _jackOfSpades = sprite; break;
                case (Suit.Spades, Rank.Queen): _queenOfSpades = sprite; break;
                case (Suit.Spades, Rank.King): _kingOfSpades = sprite; break;
            }
        }
    }
}