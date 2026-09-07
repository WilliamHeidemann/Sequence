using System;
using Game.Domain.Models;

namespace Game.Domain.Server
{
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