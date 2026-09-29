using System.Collections.Generic;
using System.Linq;
using Game.Domain.Models;

namespace Game.Domain
{
    public static class SequencePatterns
    {
        // The 4 axes we need to check: Horizontal, Vertical, Diagonal-Right, Diagonal-Left
        private static readonly (int dRow, int dCol)[] Directions = 
        {
            (0, 1),   // Horizontal (Right)
            (1, 0),   // Vertical (Down)
            (1, 1),   // Diagonal (Down-Right)
            (1, -1)   // Diagonal (Down-Left)
        };

        /// <summary>
        /// Finds all contiguous lines of a specific team that meet or exceed the target length.
        /// </summary>
        public static IEnumerable<Position[]> FindSequences(Board board, Team team, int targetLength = 4)
        {
            Row[] rows = BoardLayout.AllRows();
            Column[] cols = BoardLayout.AllColumns();

            foreach (Row row in rows)
            {
                foreach (Column col in cols)
                {
                    Position currentPos = new(row, col);

                    // Skip if the current position doesn't belong to the target team
                    if (board.Owner(currentPos).IsSome(out Team owner) && owner != team) 
                        continue;

                    foreach ((int dRow, int dCol) in Directions)
                    {
                        // 1. Check if this is the START of a line. 
                        // If the previous position in this direction is the same team, we are in the middle of a line. Skip.
                        Position? prevPos = GetNeighbor(currentPos, -dRow, -dCol);
                        if (prevPos.HasValue && board.Owner(prevPos.Value).IsSome(out Team prevOwner) && prevOwner == team)
                        {
                            continue;
                        }

                        // 2. We found a starting piece! Traverse forward to find the full continuous length.
                        List<Position> currentLine = new() { currentPos };
                        Position? nextPos = GetNeighbor(currentPos, dRow, dCol);

                        while (nextPos.HasValue && board.Owner(nextPos.Value).IsSome(out Team nextOwner) && nextOwner == team)
                        {
                            currentLine.Add(nextPos.Value);
                            nextPos = GetNeighbor(nextPos.Value, dRow, dCol);
                        }

                        // 3. If the total continuous line meets our requirement, yield it.
                        if (currentLine.Count >= targetLength)
                        {
                            yield return currentLine.ToArray();
                        }
                    }
                }
            }
        }

        private static Position? GetNeighbor(Position pos, int dRow, int dCol)
        {
            int newRow = (int)pos.Row + dRow;
            int newCol = (int)pos.Column + dCol;

            // Ensure the new coordinates are within the board's enum bounds
            if (newRow >= 0 && newRow <= 5 && newCol >= 0 && newCol <= 7)
            {
                return new Position((Row)newRow, (Column)newCol);
            }

            return null;
        }
        
        // public static IEnumerable<Position[]> Around(Position position)
        // {
        //     return All().Where(line => line.Contains(position));
        // }
        //
        // public static IEnumerable<Position[]> All()
        // {
        //     return RowWiseSequencePatterns()
        //         .Concat(ColumnWiseSequencePatterns())
        //         .Concat(DiagonalFallingRightSequencePatterns())
        //         .Concat(DiagonalFallingLeftSequencePatterns());
        // }
        //
        // private static IEnumerable<Position[]> RowWiseSequencePatterns()
        // {
        //     Row[] rows = BoardLayout.AllRows();
        //     foreach (Row row in rows)
        //     {
        //         foreach (Column[] columnSequence in ColumnSequencePatterns())
        //         {
        //             yield return new[]
        //             {
        //                 new Position(row, columnSequence[0]),
        //                 new Position(row, columnSequence[1]),
        //                 new Position(row, columnSequence[2]),
        //                 new Position(row, columnSequence[3]),
        //             };
        //         }
        //     }
        // }
        //
        // private static IEnumerable<Position[]> ColumnWiseSequencePatterns()
        // {
        //     Column[] columns = BoardLayout.AllColumns();
        //     foreach (Column column in columns)
        //     {
        //         foreach (Row[] rowSequence in RowSequencePatterns())
        //         {
        //             yield return new[]
        //             {
        //                 new Position(rowSequence[0], column),
        //                 new Position(rowSequence[1], column),
        //                 new Position(rowSequence[2], column),
        //                 new Position(rowSequence[3], column),
        //             };
        //         }
        //     }
        // }
        //
        // private static IEnumerable<Position[]> DiagonalFallingRightSequencePatterns() =>
        //     from rowSequence in RowSequencePatterns()
        //     from columnSequence in ColumnSequencePatterns()
        //     select new[]
        //     {
        //         new Position(rowSequence[0], columnSequence[0]),
        //         new Position(rowSequence[1], columnSequence[1]),
        //         new Position(rowSequence[2], columnSequence[2]),
        //         new Position(rowSequence[3], columnSequence[3]),
        //     };
        //
        // private static IEnumerable<Position[]> DiagonalFallingLeftSequencePatterns() =>
        //     from rowSequence in RowSequencePatterns()
        //     from columnSequence in ColumnSequencePatterns()
        //     select new[]
        //     {
        //         new Position(rowSequence[0], columnSequence[3]),
        //         new Position(rowSequence[1], columnSequence[2]),
        //         new Position(rowSequence[2], columnSequence[1]),
        //         new Position(rowSequence[3], columnSequence[0]),
        //     };
        //
        // private static IEnumerable<Column[]> ColumnSequencePatterns()
        // {
        //     for (int i = 0; i < 5; i++)
        //     {
        //         yield return new[]
        //         {
        //             (Column)i,
        //             (Column)i + 1,
        //             (Column)i + 2,
        //             (Column)i + 3,
        //         };
        //     }
        // }
        //
        // private static IEnumerable<Row[]> RowSequencePatterns()
        // {
        //     for (int i = 0; i < 3; i++)
        //     {
        //         yield return new[]
        //         {
        //             (Row)i,
        //             (Row)i + 1,
        //             (Row)i + 2,
        //             (Row)i + 3,
        //         };
        //     }
        // }
    }
}