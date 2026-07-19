using System;

namespace Game.Models
{
    public readonly struct Card : IEquatable<Card>
    {
        public static Card TwoOfClubs => new(Suit.Clubs, Rank.Two);
        public static Card ThreeOfClubs => new(Suit.Clubs, Rank.Three);
        public static Card FourOfClubs => new(Suit.Clubs, Rank.Four);
        public static Card FiveOfClubs => new(Suit.Clubs, Rank.Five);
        public static Card SixOfClubs => new(Suit.Clubs, Rank.Six);
        public static Card SevenOfClubs => new(Suit.Clubs, Rank.Seven);
        public static Card EightOfClubs => new(Suit.Clubs, Rank.Eight);
        public static Card NineOfClubs => new(Suit.Clubs, Rank.Nine);
        public static Card TenOfClubs => new(Suit.Clubs, Rank.Ten);
        public static Card JackOfClubs => new(Suit.Clubs, Rank.Jack);
        public static Card QueenOfClubs => new(Suit.Clubs, Rank.Queen);
        public static Card KingOfClubs => new(Suit.Clubs, Rank.King);
        public static Card AceOfClubs => new(Suit.Clubs, Rank.Ace);

        public static Card TwoOfDiamonds => new(Suit.Diamonds, Rank.Two);
        public static Card ThreeOfDiamonds => new(Suit.Diamonds, Rank.Three);
        public static Card FourOfDiamonds => new(Suit.Diamonds, Rank.Four);
        public static Card FiveOfDiamonds => new(Suit.Diamonds, Rank.Five);
        public static Card SixOfDiamonds => new(Suit.Diamonds, Rank.Six);
        public static Card SevenOfDiamonds => new(Suit.Diamonds, Rank.Seven);
        public static Card EightOfDiamonds => new(Suit.Diamonds, Rank.Eight);
        public static Card NineOfDiamonds => new(Suit.Diamonds, Rank.Nine);
        public static Card TenOfDiamonds => new(Suit.Diamonds, Rank.Ten);
        public static Card JackOfDiamonds => new(Suit.Diamonds, Rank.Jack);
        public static Card QueenOfDiamonds => new(Suit.Diamonds, Rank.Queen);
        public static Card KingOfDiamonds => new(Suit.Diamonds, Rank.King);
        public static Card AceOfDiamonds => new(Suit.Diamonds, Rank.Ace);

        public static Card TwoOfHearts => new(Suit.Hearts, Rank.Two);
        public static Card ThreeOfHearts => new(Suit.Hearts, Rank.Three);
        public static Card FourOfHearts => new(Suit.Hearts, Rank.Four);
        public static Card FiveOfHearts => new(Suit.Hearts, Rank.Five);
        public static Card SixOfHearts => new(Suit.Hearts, Rank.Six);
        public static Card SevenOfHearts => new(Suit.Hearts, Rank.Seven);
        public static Card EightOfHearts => new(Suit.Hearts, Rank.Eight);
        public static Card NineOfHearts => new(Suit.Hearts, Rank.Nine);
        public static Card TenOfHearts => new(Suit.Hearts, Rank.Ten);
        public static Card JackOfHearts => new(Suit.Hearts, Rank.Jack);
        public static Card QueenOfHearts => new(Suit.Hearts, Rank.Queen);
        public static Card KingOfHearts => new(Suit.Hearts, Rank.King);
        public static Card AceOfHearts => new(Suit.Hearts, Rank.Ace);

        public static Card TwoOfSpades => new(Suit.Spades, Rank.Two);
        public static Card ThreeOfSpades => new(Suit.Spades, Rank.Three);
        public static Card FourOfSpades => new(Suit.Spades, Rank.Four);
        public static Card FiveOfSpades => new(Suit.Spades, Rank.Five);
        public static Card SixOfSpades => new(Suit.Spades, Rank.Six);
        public static Card SevenOfSpades => new(Suit.Spades, Rank.Seven);
        public static Card EightOfSpades => new(Suit.Spades, Rank.Eight);
        public static Card NineOfSpades => new(Suit.Spades, Rank.Nine);
        public static Card TenOfSpades => new(Suit.Spades, Rank.Ten);
        public static Card JackOfSpades => new(Suit.Spades, Rank.Jack);
        public static Card QueenOfSpades => new(Suit.Spades, Rank.Queen);
        public static Card KingOfSpades => new(Suit.Spades, Rank.King);
        public static Card AceOfSpades => new(Suit.Spades, Rank.Ace);

        public static Card[] FullDeck => new[]
        {
            TwoOfClubs,
            ThreeOfClubs,
            FourOfClubs,
            FiveOfClubs,
            SixOfClubs,
            SevenOfClubs,
            EightOfClubs,
            NineOfClubs,
            TenOfClubs,
            JackOfClubs,
            QueenOfClubs,
            KingOfClubs,
            AceOfClubs,

            TwoOfDiamonds,
            ThreeOfDiamonds,
            FourOfDiamonds,
            FiveOfDiamonds,
            SixOfDiamonds,
            SevenOfDiamonds,
            EightOfDiamonds,
            NineOfDiamonds,
            TenOfDiamonds,
            JackOfDiamonds,
            QueenOfDiamonds,
            KingOfDiamonds,
            AceOfDiamonds,

            TwoOfHearts,
            ThreeOfHearts,
            FourOfHearts,
            FiveOfHearts,
            SixOfHearts,
            SevenOfHearts,
            EightOfHearts,
            NineOfHearts,
            TenOfHearts,
            JackOfHearts,
            QueenOfHearts,
            KingOfHearts,
            AceOfHearts,

            TwoOfSpades,
            ThreeOfSpades,
            FourOfSpades,
            FiveOfSpades,
            SixOfSpades,
            SevenOfSpades,
            EightOfSpades,
            NineOfSpades,
            TenOfSpades,
            JackOfSpades,
            QueenOfSpades,
            KingOfSpades,
            AceOfSpades,
        };

        private Card(Suit suit, Rank rank)
        {
            Suit = suit;
            Rank = rank;
        }

        public Suit Suit { get; }
        public Rank Rank { get; }

        public Card Equivalent => new(
            Suit switch
            {
                Suit.Clubs => Suit.Spades,
                Suit.Diamonds => Suit.Hearts,
                Suit.Hearts => Suit.Diamonds,
                Suit.Spades => Suit.Clubs,
                _ => throw new ArgumentOutOfRangeException()
            }, Rank);

        public bool IsWild => Rank is Rank.Jack && Suit is Suit.Clubs or Suit.Diamonds;
        public bool IsRemove => Rank is Rank.Jack && Suit is Suit.Spades or Suit.Hearts;

        public bool Equals(Card other)
        {
            return Suit == other.Suit && Rank == other.Rank;
        }

        public override bool Equals(object obj)
        {
            return obj is Card other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine((int)Suit, (int)Rank);
        }

        public override string ToString()
        {
            return $"{Rank} of {Suit}";
        }
    }

    public enum Suit
    {
        Clubs,
        Diamonds,
        Hearts,
        Spades
    }

    public enum Rank
    {
        Ace,
        Two,
        Three,
        Four,
        Five,
        Six,
        Seven,
        Eight,
        Nine,
        Ten,
        Jack,
        Queen,
        King
    }
}