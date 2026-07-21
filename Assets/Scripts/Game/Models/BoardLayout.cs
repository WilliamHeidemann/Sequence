using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.Models
{
    public static class BoardLayout
    {
        private static readonly Dictionary<Card, (Position First, Position Second)> CardToPositions = new()
        {
            // --- Sun ---
            [Card.AceOfSun] = (new Position(Row.Five, Column.Three), new Position(Row.Five, Column.Eight)),
            [Card.TwoOfSun] = (new Position(Row.One, Column.One), new Position(Row.Four, Column.Three)),
            [Card.ThreeOfSun] = (new Position(Row.One, Column.Two), new Position(Row.Four, Column.Four)),
            [Card.FourOfSun] = (new Position(Row.One, Column.Three), new Position(Row.Four, Column.Five)),
            [Card.FiveOfSun] = (new Position(Row.One, Column.Four), new Position(Row.Four, Column.Six)),
            [Card.SixOfSun] = (new Position(Row.One, Column.Five), new Position(Row.Three, Column.Six)),
            [Card.SevenOfSun] = (new Position(Row.One, Column.Six), new Position(Row.Three, Column.Five)),
            [Card.EightOfSun] = (new Position(Row.One, Column.Seven), new Position(Row.Three, Column.Four)),
            [Card.NineOfSun] = (new Position(Row.One, Column.Eight), new Position(Row.Three, Column.Three)),
            [Card.TenOfSun] = (new Position(Row.Two, Column.Eight), new Position(Row.Three, Column.Two)),
            [Card.QueenOfSun] = (new Position(Row.Three, Column.Eight), new Position(Row.Four, Column.Two)),
            [Card.KingOfSun] = (new Position(Row.Four, Column.Eight), new Position(Row.Five, Column.Two)),

            // --- Moon ---
            [Card.AceOfMoon] = (new Position(Row.Two, Column.One), new Position(Row.Two, Column.Two)),
            [Card.TwoOfMoon] = (new Position(Row.Five, Column.Four), new Position(Row.Six, Column.Eight)),
            [Card.ThreeOfMoon] = (new Position(Row.Five, Column.Five), new Position(Row.Six, Column.Seven)),
            [Card.FourOfMoon] = (new Position(Row.Five, Column.Six), new Position(Row.Six, Column.Six)),
            [Card.FiveOfMoon] = (new Position(Row.Five, Column.Seven), new Position(Row.Six, Column.Five)),
            [Card.SixOfMoon] = (new Position(Row.Four, Column.Seven), new Position(Row.Six, Column.Four)),
            [Card.SevenOfMoon] = (new Position(Row.Three, Column.Seven), new Position(Row.Six, Column.Three)),
            [Card.EightOfMoon] = (new Position(Row.Two, Column.Seven), new Position(Row.Six, Column.Two)),
            [Card.NineOfMoon] = (new Position(Row.Two, Column.Six), new Position(Row.Six, Column.One)),
            [Card.TenOfMoon] = (new Position(Row.Two, Column.Five), new Position(Row.Five, Column.One)),
            [Card.QueenOfMoon] = (new Position(Row.Two, Column.Four), new Position(Row.Four, Column.One)),
            [Card.KingOfMoon] = (new Position(Row.Two, Column.Three), new Position(Row.Three, Column.One)),
        };

        private static readonly Dictionary<Position, Card> PositionToCard = Invert(CardToPositions);
        
        public static Card Get(Position position) => PositionToCard[position];

        public static (Position First, Position Second) Get(Card card) => CardToPositions[card];
        
        private static Dictionary<Position, Card> Invert(Dictionary<Card, (Position First, Position Second)> source)
        {
            Dictionary<Position, Card> result = new();
            foreach (var (card, (first, second)) in source)
            {
                result.Add(first, card);
                result.Add(second, card);
            }

            return result;
        }

        public static Row[] AllRows() => (Row[])Enum.GetValues(typeof(Row));
        public static Column[] AllColumns() => (Column[])Enum.GetValues(typeof(Column));
    }

    public enum Row
    {
        One,
        Two,
        Three,
        Four,
        Five,
        Six,
    }

    public enum Column
    {
        One,
        Two,
        Three,
        Four,
        Five,
        Six,
        Seven,
        Eight
    }

    public struct Position : IEquatable<Position>
    {
        public Position(Row row, Column column)
        {
            Row = row;
            Column = column;
        }

        public Row Row { get; }
        public Column Column { get; }

        public bool Equals(Position other)
        {
            return Row == other.Row && Column == other.Column;
        }

        public override bool Equals(object obj)
        {
            return obj is Position other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine((int)Row, (int)Column);
        }

        public override string ToString()
        {
            return $"({Row}, {Column})";
        }
    }
}