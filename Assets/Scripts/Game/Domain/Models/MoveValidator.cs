using System;

namespace Game.Domain.Models
{
    public static class MoveValidator
    {
        public static bool IsValid(Move move, Board board, Hand hand, Team toPlay)
        {
            if (move.Team != toPlay)
            {
                return false;
            }
            
            bool isOpen = board.Fits(move.Position);

            var requiredCard = hand.FindCard(move.Card, isOpen);

            if (!requiredCard.IsSome(out Card cardInHand))
            {
                return false;
            }

            if (!cardInHand.IsRemover())
            {
                return true;
            }
            
            bool playerOwnsPosition = board.Owner(move.Position)
                .SelectOrDefault(owner => owner == move.Team);

            return !playerOwnsPosition;
        }
        
        public abstract class MoveResult
        {
            public class Success : MoveResult
            {
                public GameState UpdatedState { get; }

                public Success(GameState updatedState)
                {
                    UpdatedState = updatedState;
                }

                public void Deconstruct(out GameState updatedState)
                {
                    updatedState = UpdatedState;
                }
            }

            public class Invalid : MoveResult
            {
            }
        }

        public static MoveResult PlayMove(GameState gameState, Move move)
        {
            Card[] cardsInHand = move.Team switch
            {
                Team.Red => gameState.RedHand,
                Team.Yellow => gameState.YellowHand,
                _ => throw new ArgumentOutOfRangeException()
            };
            
            Hand hand = new(cardsInHand);
            Board board = new(gameState.Moves);

            if (!IsValid(move, board, hand, gameState.ToPlay))
            {
                return new MoveResult.Invalid();
            }
            
            if (move.Card.IsRemover()) board.Remove(move.Position);
            else board.TryAdd(move.Position, move.Team);

            hand.TryRemove(move.Card);

            Deck deck = new(gameState.Deck);
            Card draw = deck.Draw();

            hand.TryAdd(draw);

            MoveHistory moveHistory = new(gameState.Moves);
            moveHistory.Add(move);
            
            (Card[] redHand, Card[] yellowHand) = move.Team switch
                {
                    Team.Red => (hand.GetCards(), gameState.YellowHand),
                    Team.Yellow => (gameState.RedHand, hand.GetCards()),
                    _ => throw new ArgumentOutOfRangeException()
                };
            
            GameState updatedGameState = new(
                redHand, yellowHand, 
                deck.GetCards(), moveHistory.GetMoves(), 
                gameState.Score, move.Team.Opposing());
            
            return new MoveResult.Success(updatedGameState);
        }
    }
}