using System;
using System.Linq;
using Game.Domain.Models;
using UtilityToolkit.CollectionExtensions;

namespace Game.Domain.Players.Bot_Strategies
{
    public class RandomBot : IBrain
    {
        private readonly Random _random = new();

        public Move DecideMove(GameState gameState)
        {
            Card card = gameState.MyHand.GetCards().Where(card => card.Rank != Rank.Jack).RandomElement();
            (Position first, Position second) = BoardLayout.Get(card);

            if (_random.NextDouble() < 0.5f) (first, second) = (second, first);

            Position position = gameState.Board.Fits(first) ? first : second;

            Move move = new()
            {
                Card = card,
                Position = position,
                Team = gameState.MyTeam
            };

            return move;
        }
    }
}