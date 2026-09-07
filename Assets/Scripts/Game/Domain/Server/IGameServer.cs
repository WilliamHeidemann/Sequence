using System;
using Game.Domain.Models;

namespace Game.Domain.Server
{
    public interface IGameServer
    {
        void Request(Move move);
        event Action<Card> OnCardReceived;
        event Action<ClientGameState> OnOpponentPlayed;
    }
}