using System;
using System.Linq;
using System.Threading.Tasks;
using Game.Domain.Models;

namespace Game.Domain.Server
{
    public class LocalGameState
    {
        public LocalGameState(GameState value)
        {
            Value = value;
        }

        public GameState Value { get; set; }
    }

    public class LocalGameServer : IGameServer
    {
        private LocalGameState _gameState;
        public IGameServer OtherPlayerServer { get; set; }
        public event Action<Card> OnCardReceived;
        public event Action<ClientGameState> OnOpponentPlayed;

        public LocalGameServer(LocalGameState gameState)
        {
            _gameState = gameState;
        }

        public Task Request(Move move)
        {
            MoveValidator.MoveResult result = MoveValidator.PlayMove(_gameState.Value, move);

            _gameState.Value = result switch
            {
                MoveValidator.MoveResult.Success(var updatedState, var drawnCard) => updatedState,
                MoveValidator.MoveResult.Invalid => _gameState.Value,
                _ => throw new ArgumentOutOfRangeException()
            };

            HandleEvents(result, move);

            return Task.CompletedTask;
        }

        private void HandleEvents(MoveValidator.MoveResult result, Move move)
        {
            if (result is MoveValidator.MoveResult.Success(var updatedState, var drawnCard))
            {
                OnCardReceived?.Invoke(drawnCard);

                OtherPlayerServer.Receive(updatedState.ToClientGameState(move.Team.Opposing()));
            }
        }

        public void Receive(ClientGameState gameState)
        {
            OnOpponentPlayed?.Invoke(gameState);
        }
    }
}