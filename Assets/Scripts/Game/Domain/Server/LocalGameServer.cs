using System;
using System.Linq;
using Game.Domain.Models;

namespace Game.Domain.Server
{
    public class LocalGameServer : IGameServer
    {
        private readonly GameState _gameState;
        public LocalGameServer OtherPlayerServer { get; set; }
        public event Action<Card> OnCardReceived;
        public event Action<ClientGameState> OnOpponentPlayed;

        public LocalGameServer(GameState gameState)
        {
            _gameState = gameState;
        }

        public void Request(Move move)
        {
            if (StateUpdater.Update(_gameState, move))
            {
                Hand hand = move.Team switch
                {
                    Team.Red => _gameState.RedHand,
                    Team.Yellow => _gameState.YellowHand,
                    _ => throw new ArgumentOutOfRangeException()
                };
                OnCardReceived?.Invoke(hand.GetCards().Last());

                OtherPlayerServer.Receive(_gameState.ToClientGameState(move.Team.Opposing()));
            }
        }

        private void Receive(ClientGameState gameState)
        {
            OnOpponentPlayed?.Invoke(gameState);
        }
    }
}