using System;
using System.Threading.Tasks;
using Game.Domain.Models;
using Game.Domain.Server;
using Unity.Services.CloudCode.GeneratedBindings;

namespace Game.Cloud
{
    public class CloudGameServer : IGameServer
    {
        private readonly GameLogicServiceBindings _gameLogicService;
        private readonly string _matchId;
        public event Action<Card> OnCardReceived;
        public event Action<ClientGameState> OnOpponentPlayed;

        public CloudGameServer(GameLogicServiceBindings gameLogicService, string matchId)
        {
            _gameLogicService = gameLogicService;
            _matchId = matchId;
        }

        public async Task Request(Move move)
        {
            var cardResult = await _gameLogicService.Request(move.ToDto(), _matchId);

            if (cardResult.HasCard)
            {
                OnCardReceived?.Invoke(cardResult.Card.ToModel());
            }
        }

        public void Receive(ClientGameState gameState)
        {
            OnOpponentPlayed?.Invoke(gameState);
        }
    }
}