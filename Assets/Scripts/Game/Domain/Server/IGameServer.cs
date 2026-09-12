using System;
using System.Threading.Tasks;
using Game.Domain.Models;

namespace Game.Domain.Server
{
    public interface IGameServer
    {
        Task Request(Move move);
        public void Receive(ClientGameState clientGameState);
        event Action<Card> OnCardReceived;
        event Action<ClientGameState> OnOpponentPlayed;
    }
}