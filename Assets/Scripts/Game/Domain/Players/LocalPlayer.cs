using System;
using Game.Domain.Models;
using UtilityToolkit.Monads;

namespace Game.Domain.Players
{
    public class LocalPlayer : IOpponent
    {
        private readonly GameState _gameState;
        public bool IsMyTurn { get; set; }
        public Team MyTeam => _gameState.MyTeam;
        public Hand MyHand => _gameState.MyHand;
        public Hand OpponentHand => _gameState.OpponentHand;
        public Deck Deck => _gameState.Deck;
        public Board Board => _gameState.Board;
        public MoveHistory MoveHistory => _gameState.MoveHistory;
        public GameStateData GetGameStateData() => _gameState.ToData();

        public event Action<Move, GameStateData> OnMovePerformed;
        
        public LocalPlayer(Team team)
        {
            IsMyTurn = true;
            _gameState = new GameState(team);
        }

        public void PassGameState(GameStateData gameStateData)
        {
            MoveHistory.Set(gameStateData.Moves);
            Deck.Set(gameStateData.Deck);
            Board.Set(gameStateData.Moves);
            Card[] opponentHand = MyTeam == Team.Red ? gameStateData.YellowHand : gameStateData.RedHand;
            OpponentHand.Set(opponentHand);

            // if game just loaded:
            // set my hand
            // display all cards (no animation)
            
            IsMyTurn = true;
        }

        public bool AttemptPlay(Position position)
        {
            Card tabbedCard = BoardLayout.Get(position);

            if (!IsMyTurn)
            {
                return false;
            }

            bool isOpenSpace = Board.Fits(position);

            Option<Card> requiredCard = MyHand.FindCard(tabbedCard, isOpenSpace);

            if (!requiredCard.IsSome(out Card cardInHand))
            {
                return false;
            }

            if (cardInHand.IsRemover)
            {
                if (Board.Owner(position).IsSome(out Team owner) && owner == MyTeam)
                {
                    return false;
                }
                
                if (!Board.Remove(position))
                {
                    throw new Exception($"Unexpected behavior: {position} could not be removed from.");
                }
            }
            else if (!Board.TryAddPin(position, MyTeam))
            {
                throw new Exception($"Unexpected behavior: {position} could not be pinned.");
            }

            if (!MyHand.TryRemove(cardInHand))
            {
                throw new Exception($"Unexpected behavior: {cardInHand} was not in the hand.");
            }

            Card drawnCard = Deck.Draw();

            if (!MyHand.TryAdd(drawnCard))
            {
                throw new Exception($"Unexpected behavior: {drawnCard} could not be added.");
            }

            Move move = new()
            {
                Card = cardInHand,
                Position = position,
                Team = MyTeam
            };

            MoveHistory.Add(move);
            
            OnMovePerformed?.Invoke(move, _gameState.ToData());

            return true;
        }
    }
}