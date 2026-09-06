using System;
using System.Linq;
using Game.Domain.Models;
using Game.Domain.Players;

namespace Game.Domain
{
    public interface IGameServer
    {
        void Request(Move move);
        event Action<Card> OnCardReceived;
        event Action<ClientGameState> OnOpponentPlayed;
    }

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

    public class RemoteGameServer : IGameServer
    {
        public event Action<Card> OnCardReceived;
        public event Action<ClientGameState> OnOpponentPlayed;

        public void Request(Move move)
        {
            // calls an end point with a unity cloud code module
            
            // receives a card as a response to a successful request
            
            // OnCardReceived?.Invoke(card);
            throw new NotImplementedException();
        }

        public void Receive(ClientGameState gameState)
        {
            throw new NotImplementedException();
        }
    }
}