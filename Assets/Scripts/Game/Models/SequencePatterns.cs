using System.Collections.Generic;
using System.Linq;

namespace Game.Models
{
    public static class SequencePatterns
    {
        public static IEnumerable<Position[]> All()
        {
            return RowWiseSequencePatterns()
                .Concat(ColumnWiseSequencePatterns())
                .Concat(DiagonalFallingRightSequencePatterns())
                .Concat(DiagonalFallingLeftSequencePatterns());
        }

        private static IEnumerable<Position[]> RowWiseSequencePatterns()
        {
            Row[] rows = BoardLayout.AllRows();
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
            Column[] columns = BoardLayout.AllColumns();
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
    }
}