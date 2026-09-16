using System;
using System.Threading.Tasks;
using Game.Domain.Models;
using Game.Domain.Server;
using Unity.Services.CloudCode.GeneratedBindings;
using UnityEngine;
using UtilityToolkit.Monads;

namespace Game.Cloud
{
    public class CloudGameServer : IGameServer
    {
        private readonly GameLogicServiceBindings _gameLogicService;
        public event Action<Card> OnCardReceived;
        public event Action<ClientGameState> OnOpponentPlayed;

        public CloudGameServer(GameLogicServiceBindings gameLogicService)
        {
            _gameLogicService = gameLogicService;
        }
        
        public async Task Request(Move move)
        {
            var cardResult = await _gameLogicService.Request(move.ToDto(), "match002");
            
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