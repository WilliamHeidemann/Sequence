using System;

namespace Game.Domain.Models
{
    public static class StateUpdater
    {
        public static bool Update(GameState gameState, Move move)
        {
            Hand hand = move.Team switch
            {
                Team.Red => gameState.RedHand,
                Team.Yellow => gameState.YellowHand,
                _ => throw new ArgumentOutOfRangeException()
            };

            if (!MoveValidator.IsValid(move, gameState.Board, hand, gameState.ToPlay))
            {
                return false;
            }

            if (move.Card.IsRemover()) gameState.Board.Remove(move.Position);
            else gameState.Board.TryAdd(move.Position, move.Team);

            hand.TryRemove(move.Card);

            Card draw = gameState.Deck.Draw();

            hand.TryAdd(draw);

            gameState.MoveHistory.Add(move);

            gameState.ToPlay = move.Team.Opposing();

            return true;
        }
    }
}