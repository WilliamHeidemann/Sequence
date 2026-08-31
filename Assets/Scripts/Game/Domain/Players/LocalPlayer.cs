using System;
using Game.Domain.Models;
using UtilityToolkit.Monads;

namespace Game.Domain.Players
{
    public class LocalPlayer : IOpponent
    {
        public bool IsMyTurn { get; set; }
        public Team Team { get; set; }
        public Hand Hand { get; set; }
        public Board Board { get; set; }

        public event Action<Move> OnMovePerformed;
        public event Action OnOpponentGotSequence;

        public void PassGameState(ClientGameState clientGameState)
        {
            Team = clientGameState.Team;
            Hand = new Hand(clientGameState.Hand);

            int sequenceCountBefore = Board.SequenceCount(Team.Opposing());
            Board = new Board(clientGameState.Moves);
            var sequenceCountAfter = Board.SequenceCount(Team.Opposing());
            if (sequenceCountAfter > sequenceCountBefore)
            {
                OnOpponentGotSequence?.Invoke();
            }
            
            // if game just loaded:
            // set my hand
            // display all cards (no animation)
            
            IsMyTurn = clientGameState.IsMyTurn;
        }

        public bool AttemptPlay(Position position, out Move move)
        {
            move = null;
            
            if (!IsMyTurn)
            {
                return false;
            }
            
            Card tabbedCard = BoardLayout.Get(position);

            bool isOpenSpace = Board.Fits(position);

            Option<Card> requiredCard = Hand.FindCard(tabbedCard, isOpenSpace);

            if (!requiredCard.IsSome(out Card cardInHand))
            {
                return false;
            }

            if (cardInHand.IsRemover())
            {
                if (Board.Owner(position).IsSome(out Team owner) && owner == Team)
                {
                    return false;
                }
                
                if (!Board.Remove(position))
                {
                    throw new Exception($"Unexpected behavior: {position} could not be removed from.");
                }
            }
            else if (!Board.TryAddPin(position, Team))
            {
                throw new Exception($"Unexpected behavior: {position} could not be pinned.");
            }
            
            move = new Move
            {
                Card = cardInHand,
                Position = position,
                Team = Team
            };
            
            IsMyTurn = false;
            OnMovePerformed?.Invoke(move);
            
            return true;
            
            // Method ends here. The player has locally validated the move.
            // Now send the move to the server. The server will re-validate the move. 
            // If the move is validated, the server will respond with a new card drawn. 
            // If a sequence has occured, play the animation locally.
            // The server will also record that a sequence has happened. 
            
        }
    }
}