using System;
using Game.Domain.Models;
using Game.Domain.Players;

namespace Game.Domain
{
    public interface IPlayCoordinator
    {
        void Request(Move move);
    }

    public class LocalPlayCoordinator : IPlayCoordinator
    {
        private readonly GameEngine _gameEngine;
        private readonly IGameStateProvider _gameStateProvider;
        private readonly IOpponent _player;
        private readonly IOpponent _bot;
        private IOpponent GetOpponent(Team team) => team == Team.Red ? _player : _bot;

        public event Action<Move> OnInvalidMoveRequest;
        public event Action<Move> OnValidMoveRequest;
        
        public LocalPlayCoordinator(GameEngine gameEngine, IGameStateProvider gameStateProvider, IOpponent player,
            IOpponent bot)
        {
            _gameEngine = gameEngine;
            _gameStateProvider = gameStateProvider;
            _player = player;
            _bot = bot;
        }

        public void Request(Move move)
        {
            if (_gameEngine.IsValid(move))
            {
                GameState updatedState = _gameEngine.Play(move);
                _gameStateProvider.Set(updatedState);
                IOpponent opponent = GetOpponent(move.Team.Opposing());
                opponent.PassGameState(updatedState.ToClientGameState(move.Team.Opposing()));
                OnValidMoveRequest?.Invoke(move);
            }
            else
            {
                OnInvalidMoveRequest?.Invoke(move);
            }
        }
    }

    public class OnlinePlayCoordinator : IPlayCoordinator
    {
        public void Request(Move move)
        {
            throw new System.NotImplementedException();
        }
    }
}