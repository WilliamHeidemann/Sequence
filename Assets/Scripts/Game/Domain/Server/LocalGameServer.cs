using System;
using System.Linq;
using System.Threading.Tasks;
using Game.Domain.Models;

namespace Game.Domain.Server
{
    public class LocalGameServer : IGameServer
    {
        private GameState _gameState;
        public IGameServer OtherPlayerServer { get; set; }
        public event Action<Card> OnCardReceived;
        public event Action<ClientGameState> OnOpponentPlayed;

        public LocalGameServer(GameState gameState)
        {
            _gameState = gameState;
        }

        public Task Request(Move move)
        {
            MoveValidator.MoveResult moveResult = MoveValidator.PlayMove(_gameState, move);
            
            _gameState = moveResult switch
            {
                MoveValidator.MoveResult.Success(var updatedState) => OnMoveSuccess(updatedState, move),
                MoveValidator.MoveResult.Invalid => _gameState,
                _ => throw new ArgumentOutOfRangeException(nameof(moveResult))
            };

            OtherPlayerServer.Receive(_gameState.ToClientGameState(move.Team.Opposing()));

            return Task.CompletedTask;
        }

        private GameState OnMoveSuccess(GameState updatedState, Move move)
        {
            Card[] hand = move.Team switch
            {
                Team.Red => _gameState.RedHand,
                Team.Yellow => _gameState.YellowHand,
                _ => throw new ArgumentOutOfRangeException()
            };

            OnCardReceived?.Invoke(hand.Last());
            return updatedState;
        }

        public void Receive(ClientGameState gameState)
        {
            OnOpponentPlayed?.Invoke(gameState);
        }
    }
}