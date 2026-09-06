namespace Game.Domain.Models
{
    public static class MoveValidator
    {
        public static bool IsValid(Move move, Board board, Hand hand)
        {
            bool isOpen = board.Fits(move.Position);

            var requiredCard = hand.FindCard(move.Card, isOpen);

            if (!requiredCard.IsSome(out Card cardInHand))
            {
                return false;
            }

            if (cardInHand.IsRemover())
            {
                bool playerOwnsPosition = board.Owner(move.Position)
                    .SelectOrDefault(owner => owner == move.Team);

                if (playerOwnsPosition)
                {
                    return false;
                }
            }

            return true;
        }
        
        public static bool IsValid(Move move, Board board, Hand hand, Team toPlay)
        {
            return move.Team == toPlay && IsValid(move, board, hand);
        }
    }
}