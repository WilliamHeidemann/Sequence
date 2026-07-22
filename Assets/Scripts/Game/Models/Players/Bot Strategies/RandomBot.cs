using System.Linq;
using UnityEngine;
using UtilityToolkit.Runtime;

namespace Game.Models.Players.Bot_Strategies
{
    public class RandomBot : IBrain
    {
        public Move DecideMove(GameState gameState)
        {
            Card card = gameState.MyHand.GetCards().Where(card => card.Rank != Rank.Jack).RandomElement();
            (Position first, Position second) = BoardLayout.Get(card);

            if (Random.value < 0.5f) (first, second) = (second, first);

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