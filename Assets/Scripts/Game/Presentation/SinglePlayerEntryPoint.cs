using System;
using System.Threading.Tasks;
using Game.Cloud;
using Game.Domain;
using Game.Domain.Models;
using Game.Domain.Players;
using Game.Domain.Players.Bot_Strategies;
using Game.Domain.Server;
using Game.Presentation.AnimationSystems;
using Unity.Services.Authentication;
using Unity.Services.CloudCode.GeneratedBindings;
using Unity.Services.Core;
using UnityEngine;

namespace Game.Presentation
{
    public class SinglePlayerEntryPoint : MonoBehaviour
    {
        [SerializeField] private BoardPresenter _boardPresenter;
        [SerializeField] private AnimationOrchestrator _animationOrchestrator;
        
        private LocalPlayer _localPlayer;
        private Bot _bot;
        private PlayCoordinator _playCoordinator;

        private async void Start()
        {
            await UnityServices.InitializeAsync();

            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            
            GameState gameState = GameState.CreateInitial();

            IGameServer playerGameServer = CreateLocalGameServer(gameState);
            // IGameServer playerGameServer = CreateCloudGameServer(gameState);
            
            _localPlayer = new LocalPlayer(gameState.ToClientGameState(gameState.ToPlay));
            
            _playCoordinator = new PlayCoordinator(playerGameServer, _localPlayer);
            _animationOrchestrator.BindAnimations(_playCoordinator);
            _animationOrchestrator.PlayDrawAnimation(_localPlayer.Hand.GetCards());
            _boardPresenter.OnPositionClicked += HandlePositionClicked;
            
            // ExampleServiceBindings exampleService = new();
            // string result1 = await exampleService.CreateMatch();
            // Debug.Log($"Match created: {result1}");
            //
            // string result2 = await exampleService.StoreGameState();
            // Debug.Log($"Game State stored: {result2}");
            //
            // var cardDto = await exampleService.DrawCard();
            // Card card = cardDto.ToModel();
            // Debug.Log($"Card drawn: {card}");
            
            GameLogicServiceBindings gameLogicServiceBindings = new();
            await gameLogicServiceBindings.CreateMatch();
            
            ExampleServiceBindings bindings = new();
            await bindings.CreateCard();
        }

        private LocalGameServer CreateLocalGameServer(GameState gameState)
        {
            LocalGameServer playerGameServer = new(gameState);
            LocalGameServer botGameServer = new(gameState);
            playerGameServer.OtherPlayerServer = botGameServer;
            botGameServer.OtherPlayerServer = playerGameServer;
            _bot = new Bot(botGameServer, new CenterBrain());
            
            return playerGameServer;
        }

        private CloudGameServer CreateCloudGameServer(GameState gameState)
        {
            GameLogicServiceBindings gameLogicService = new();
            CloudGameServer playerGameServer = new CloudGameServer(gameLogicService);
            
            return playerGameServer;
        }

        private async void HandlePositionClicked(Position position)
        {
            try
            {
                await _playCoordinator.PositionClicked(position);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }
    }
}