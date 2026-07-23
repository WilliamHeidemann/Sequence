using System;
using System.Linq;
using Unity.Services.CloudCode.GeneratedBindings;
using UtilityToolkit.CollectionExtensions;

namespace Game.Models.Players.Bot_Strategies
{
    public class CenterBot : IBrain
    {
        public Move DecideMove(GameState gameState)
        {
            // cards in hand -> all positions on the board
            // order by distance to center
            // select first one

            var position = gameState.MyHand.GetCards()
                .Where(card => card.Rank != Rank.Jack)
                .SelectMany(card =>
                {
                    (Position first, Position second) = BoardLayout.Get(card);
                    return new[] { first, second };
                })
                .Where(gameState.Board.Fits).OrderByDescending(position =>
                {
                    int row = (int)position.Row;
                    int column = (int)position.Column;

                    int rowScore = row switch
                    {
                        0 => 0,
                        1 => 1,
                        2 => 2,
                        3 => 2,
                        4 => 1,
                        5 => 0,
                        _ => throw new ArgumentOutOfRangeException()
                    };

                    int columnScore = column switch
                    {
                        0 => 0,
                        1 => 1,
                        2 => 2,
                        3 => 3,
                        4 => 3,
                        5 => 2,
                        6 => 1,
                        7 => 0,
                        _ => throw new ArgumentOutOfRangeException()
                    };

                    return rowScore * columnScore;
                }).FirstOption();

            if (position.IsSome(out Position playedPosition))
            {
                return new Move()
                {
                    Card = BoardLayout.Get(playedPosition),
                    Position = playedPosition,
                    Team = gameState.MyTeam
                };
            }

            throw new Exception("No room for any card");
        }
    }
}