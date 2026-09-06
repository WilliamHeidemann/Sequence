using System;
using Game.Domain.Models;
using UtilityToolkit.Monads;

namespace Game.Domain.Players
{
    public interface IPlayer
    {
        void PassGameState(ClientGameState clientGameState);
    }
    
    public class LocalPlayer : IPlayer
    {
        public bool IsMyTurn { get; set; }
        public Team Team { get; set; }
        public Hand Hand { get; set; }
        public Board Board { get; set; }
        
        public LocalPlayer(ClientGameState clientGameState)
        {
            PassGameState(clientGameState);
        }

        public void PassGameState(ClientGameState clientGameState)
        {
            IsMyTurn = clientGameState.IsMyTurn;
            Team = clientGameState.Team;
            Hand = new Hand(clientGameState.Hand);
            Board = new Board(clientGameState.Moves);
        }
        
        public Option<Move> GetValidMove(Position position)
        {
            Card tabbedCard = BoardLayout.Get(position);

            bool isOpenSpace = Board.Fits(position);

            Option<Card> requiredCard = Hand.FindCard(tabbedCard, isOpenSpace);

            if (!requiredCard.IsSome(out Card cardInHand))
            {
                return Option<Move>.None;
            }

            if (cardInHand.IsRemover())
            {
                if (Board.Owner(position).IsSome(out Team owner) && owner == Team)
                {
                    return Option<Move>.None;
                }

                if (!Board.Remove(position))
                {
                    throw new Exception($"Unexpected behavior: {position} could not be removed from.");
                }
            }

            Move move = new()
            {
                Card = cardInHand,
                Position = position,
                Team = Team
            };

            return IsValid(move) ? Option<Move>.Some(move) : Option<Move>.None;
        }

        private bool IsValid(Move move)
        {
            return MoveValidator.IsValid(move, Board, Hand);
        }
    }
}