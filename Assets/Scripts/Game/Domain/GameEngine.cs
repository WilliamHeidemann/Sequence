using System;
using Game.Domain.Models;

namespace Game.Domain
{
    public class GameEngine
    {
        private readonly IGameStateProvider _gameStateProvider;

        public GameEngine(IGameStateProvider gameStateProvider)
        {
            _gameStateProvider = gameStateProvider;
        }

        public bool IsValid(Move move)
        {
            GameState gameState = _gameStateProvider.Get();
            
            bool isOpen = gameState.Board.Fits(move.Position);

            Hand hand = move.Team == Team.Red ? gameState.RedHand : gameState.YellowHand;

            var requiredCard = hand.FindCard(move.Card, isOpen);

            if (!requiredCard.IsSome(out Card cardInHand))
            {
                return false;
            }

            if (cardInHand.IsRemover())
            {
                bool playerOwnsPosition = gameState.Board.Owner(move.Position)
                    .SelectOrDefault(owner => owner == move.Team);
                
                if (playerOwnsPosition)
                {
                    return false;
                }
            }

            return true;
        }

        public GameState Play(Move move)
        {
            GameState gameState = _gameStateProvider.Get();
            
            if (!IsValid(move))
            {
                throw new Exception("Invalid move");
            }
            
            gameState.Board.TryAddPin(move.Position, move.Team);
            
            Hand hand = move.Team == Team.Red ? gameState.RedHand : gameState.YellowHand;
            hand.TryRemove(move.Card);
            hand.TryAdd(gameState.Deck.Draw());
            
            gameState.MoveHistory.Add(move);
            
            gameState.ToPlay = move.Team == Team.Red ? Team.Yellow : Team.Red;

            return gameState;
        }
    }
}