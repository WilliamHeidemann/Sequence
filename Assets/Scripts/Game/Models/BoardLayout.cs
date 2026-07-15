using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.Models
{
    public static class BoardLayout
    {
        public static Card Get(Row row, Column column)
        {
            return (row, column) switch
            {
                // --- Row One ---
                (Row.One, Column.One) => Card.TwoOfSpades,
                (Row.One, Column.Two) => Card.ThreeOfSpades,
                (Row.One, Column.Three) => Card.FourOfSpades,
                (Row.One, Column.Four) => Card.FiveOfSpades,
                (Row.One, Column.Five) => Card.SixOfSpades,
                (Row.One, Column.Six) => Card.SevenOfSpades,
                (Row.One, Column.Seven) => Card.EightOfSpades,
                (Row.One, Column.Eight) => Card.NineOfSpades,

                // --- Row Two ---
                (Row.Two, Column.One) => Card.AceOfDiamonds,
                (Row.Two, Column.Two) => Card.AceOfClubs,
                (Row.Two, Column.Three) => Card.KingOfClubs,
                (Row.Two, Column.Four) => Card.QueenOfClubs,
                (Row.Two, Column.Five) => Card.TenOfClubs,
                (Row.Two, Column.Six) => Card.NineOfClubs,
                (Row.Two, Column.Seven) => Card.EightOfClubs,
                (Row.Two, Column.Eight) => Card.TenOfSpades,

                // --- Row Three ---
                (Row.Three, Column.One) => Card.KingOfDiamonds,
                (Row.Three, Column.Two) => Card.TenOfHearts,
                (Row.Three, Column.Three) => Card.NineOfHearts,
                (Row.Three, Column.Four) => Card.EightOfHearts,
                (Row.Three, Column.Five) => Card.SevenOfHearts,
                (Row.Three, Column.Six) => Card.SixOfHearts,
                (Row.Three, Column.Seven) => Card.SevenOfClubs,
                (Row.Three, Column.Eight) => Card.QueenOfSpades,

                // --- Row Four ---
                (Row.Four, Column.One) => Card.QueenOfDiamonds,
                (Row.Four, Column.Two) => Card.QueenOfHearts,
                (Row.Four, Column.Three) => Card.TwoOfHearts,
                (Row.Four, Column.Four) => Card.ThreeOfHearts,
                (Row.Four, Column.Five) => Card.FourOfHearts,
                (Row.Four, Column.Six) => Card.FiveOfHearts,
                (Row.Four, Column.Seven) => Card.SixOfClubs,
                (Row.Four, Column.Eight) => Card.KingOfSpades,

                // --- Row Five ---
                (Row.Five, Column.One) => Card.TenOfDiamonds,
                (Row.Five, Column.Two) => Card.KingOfHearts,
                (Row.Five, Column.Three) => Card.AceOfHearts,
                (Row.Five, Column.Four) => Card.TwoOfClubs,
                (Row.Five, Column.Five) => Card.ThreeOfClubs,
                (Row.Five, Column.Six) => Card.FourOfClubs,
                (Row.Five, Column.Seven) => Card.FiveOfClubs,
                (Row.Five, Column.Eight) => Card.AceOfSpades,

                // --- Row Six ---
                (Row.Six, Column.One) => Card.NineOfDiamonds,
                (Row.Six, Column.Two) => Card.EightOfDiamonds,
                (Row.Six, Column.Three) => Card.SevenOfDiamonds,
                (Row.Six, Column.Four) => Card.SixOfDiamonds,
                (Row.Six, Column.Five) => Card.FiveOfDiamonds,
                (Row.Six, Column.Six) => Card.FourOfDiamonds,
                (Row.Six, Column.Seven) => Card.ThreeOfDiamonds,
                (Row.Six, Column.Eight) => Card.TwoOfDiamonds,

                _ => throw new System.ArgumentOutOfRangeException(nameof(row), "Invalid board coordinates")
            };
        }

        public static Card Get(Position position) => Get(position.Row, position.Column);

        public static Position Get(Card card)
        {
            return (card.Suit, card.Rank) switch
            {
                // --- Row One ---
                (Suit.Spades, Rank.Two) => new Position(Row.One, Column.One),
                (Suit.Spades, Rank.Three) => new Position(Row.One, Column.Two),
                (Suit.Spades, Rank.Four) => new Position(Row.One, Column.Three),
                (Suit.Spades, Rank.Five) => new Position(Row.One, Column.Four),
                (Suit.Spades, Rank.Six) => new Position(Row.One, Column.Five),
                (Suit.Spades, Rank.Seven) => new Position(Row.One, Column.Six),
                (Suit.Spades, Rank.Eight) => new Position(Row.One, Column.Seven),
                (Suit.Spades, Rank.Nine) => new Position(Row.One, Column.Eight),

                // --- Row Two ---
                (Suit.Diamonds, Rank.Ace) => new Position(Row.Two, Column.One),
                (Suit.Clubs, Rank.Ace) => new Position(Row.Two, Column.Two),
                (Suit.Clubs, Rank.King) => new Position(Row.Two, Column.Three),
                (Suit.Clubs, Rank.Queen) => new Position(Row.Two, Column.Four),
                (Suit.Clubs, Rank.Ten) => new Position(Row.Two, Column.Five),
                (Suit.Clubs, Rank.Nine) => new Position(Row.Two, Column.Six),
                (Suit.Clubs, Rank.Eight) => new Position(Row.Two, Column.Seven),
                (Suit.Spades, Rank.Ten) => new Position(Row.Two, Column.Eight),

                // --- Row Three ---
                (Suit.Diamonds, Rank.King) => new Position(Row.Three, Column.One),
                (Suit.Hearts, Rank.Ten) => new Position(Row.Three, Column.Two),
                (Suit.Hearts, Rank.Nine) => new Position(Row.Three, Column.Three),
                (Suit.Hearts, Rank.Eight) => new Position(Row.Three, Column.Four),
                (Suit.Hearts, Rank.Seven) => new Position(Row.Three, Column.Five),
                (Suit.Hearts, Rank.Six) => new Position(Row.Three, Column.Six),
                (Suit.Clubs, Rank.Seven) => new Position(Row.Three, Column.Seven),
                (Suit.Spades, Rank.Queen) => new Position(Row.Three, Column.Eight),

                // --- Row Four ---
                (Suit.Diamonds, Rank.Queen) => new Position(Row.Four, Column.One),
                (Suit.Hearts, Rank.Queen) => new Position(Row.Four, Column.Two),
                (Suit.Hearts, Rank.Two) => new Position(Row.Four, Column.Three),
                (Suit.Hearts, Rank.Three) => new Position(Row.Four, Column.Four),
                (Suit.Hearts, Rank.Four) => new Position(Row.Four, Column.Five),
                (Suit.Hearts, Rank.Five) => new Position(Row.Four, Column.Six),
                (Suit.Clubs, Rank.Six) => new Position(Row.Four, Column.Seven),
                (Suit.Spades, Rank.King) => new Position(Row.Four, Column.Eight),

                // --- Row Five ---
                (Suit.Diamonds, Rank.Ten) => new Position(Row.Five, Column.One),
                (Suit.Hearts, Rank.King) => new Position(Row.Five, Column.Two),
                (Suit.Hearts, Rank.Ace) => new Position(Row.Five, Column.Three),
                (Suit.Clubs, Rank.Two) => new Position(Row.Five, Column.Four),
                (Suit.Clubs, Rank.Three) => new Position(Row.Five, Column.Five),
                (Suit.Clubs, Rank.Four) => new Position(Row.Five, Column.Six),
                (Suit.Clubs, Rank.Five) => new Position(Row.Five, Column.Seven),
                (Suit.Spades, Rank.Ace) => new Position(Row.Five, Column.Eight),

                // --- Row Six ---
                (Suit.Diamonds, Rank.Nine) => new Position(Row.Six, Column.One),
                (Suit.Diamonds, Rank.Eight) => new Position(Row.Six, Column.Two),
                (Suit.Diamonds, Rank.Seven) => new Position(Row.Six, Column.Three),
                (Suit.Diamonds, Rank.Six) => new Position(Row.Six, Column.Four),
                (Suit.Diamonds, Rank.Five) => new Position(Row.Six, Column.Five),
                (Suit.Diamonds, Rank.Four) => new Position(Row.Six, Column.Six),
                (Suit.Diamonds, Rank.Three) => new Position(Row.Six, Column.Seven),
                (Suit.Diamonds, Rank.Two) => new Position(Row.Six, Column.Eight),

                _ => throw new ArgumentException("This card does not exist on the board layout.", nameof(card))
            };
        }


        public static IEnumerable<Position[]> AllSequencePatterns()
        {
            return RowWiseSequencePatterns()
                .Concat(ColumnWiseSequencePatterns())
                .Concat(DiagonalFallingRightSequencePatterns())
                .Concat(DiagonalFallingLeftSequencePatterns());
        }

        private static IEnumerable<Position[]> RowWiseSequencePatterns()
        {
            Row[] rows = AllRows();
            foreach (Row row in rows)
            {
                foreach (Column[] columnSequence in ColumnSequencePatterns())
                {
                    yield return new[]
                    {
                        new Position(row, columnSequence[0]),
                        new Position(row, columnSequence[1]),
                        new Position(row, columnSequence[2]),
                        new Position(row, columnSequence[3]),
                    };
                }
            }
        }

        private static IEnumerable<Position[]> ColumnWiseSequencePatterns()
        {
            Column[] columns = AllColumns();
            foreach (Column column in columns)
            {
                foreach (Row[] rowSequence in RowSequencePatterns())
                {
                    yield return new[]
                    {
                        new Position(rowSequence[0], column),
                        new Position(rowSequence[1], column),
                        new Position(rowSequence[2], column),
                        new Position(rowSequence[3], column),
                    };
                }
            }
        }

        private static IEnumerable<Position[]> DiagonalFallingRightSequencePatterns() =>
            from rowSequence in RowSequencePatterns()
            from columnSequence in ColumnSequencePatterns()
            select new[]
            {
                new Position(rowSequence[0], columnSequence[0]),
                new Position(rowSequence[1], columnSequence[1]),
                new Position(rowSequence[2], columnSequence[2]),
                new Position(rowSequence[3], columnSequence[3]),
            };

        private static IEnumerable<Position[]> DiagonalFallingLeftSequencePatterns() =>
            from rowSequence in RowSequencePatterns()
            from columnSequence in ColumnSequencePatterns()
            select new[]
            {
                new Position(rowSequence[0], columnSequence[3]),
                new Position(rowSequence[1], columnSequence[2]),
                new Position(rowSequence[2], columnSequence[1]),
                new Position(rowSequence[3], columnSequence[0]),
            };

        private static IEnumerable<Column[]> ColumnSequencePatterns()
        {
            for (int i = 0; i < 5; i++)
            {
                yield return new[]
                {
                    (Column)i,
                    (Column)i + 1,
                    (Column)i + 2,
                    (Column)i + 3,
                };
            }
        }

        private static IEnumerable<Row[]> RowSequencePatterns()
        {
            for (int i = 0; i < 3; i++)
            {
                yield return new[]
                {
                    (Row)i,
                    (Row)i + 1,
                    (Row)i + 2,
                    (Row)i + 3,
                };
            }
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