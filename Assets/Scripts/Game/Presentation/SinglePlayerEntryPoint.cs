using System;
using System.Threading.Tasks;
using Game.Cloud;
using Game.Domain;
using Game.Domain.Models;
using Game.Domain.Models.Dto;
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

        private Bot _bot;
        private PlayCoordinator _playCoordinator;
        private string _matchId;

        private async void Start()
        {
            await UnityServices.InitializeAsync();

            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            
            GameLogicServiceBindings gameLogicServiceBindings = new();
            var matchDto = await gameLogicServiceBindings.CreateMatch();
            Match match = matchDto.ToModel();

            // IGameServer playerGameServer = CreateLocalGameServer(gameState);
            // IGameServer playerGameServer = CreateCloudGameServer(gameState);
            IGameServer playerGameServer = CreateCloudBotGameServer(match);

            _playCoordinator = new PlayCoordinator(playerGameServer, match.ClientGameState);
            _animationOrchestrator.BindAnimations(_playCoordinator);
            _animationOrchestrator.PlayDrawAnimation(match.ClientGameState.Hand);
            _boardPresenter.OnPositionClicked += HandlePositionClicked;
            
            _matchId = match.Id;
        }

        private CloudGameServer CreateCloudBotGameServer(Match match)
        {
            GameLogicServiceBindings gameLogicServiceBindings = new();
            CloudGameServer playerGameServer = new(gameLogicServiceBindings, match.Id);
            CloudGameServer botGameServer = new(gameLogicServiceBindings, match.Id);
            // The following is only possible when both clients are on the same machine. 
            // This is to use remote gameplay without push messages implemented. 
            playerGameServer.OnCardReceived += async _ =>
                await PassGameState(botGameServer, match.Id, match.ClientGameState.Team.Opposing(), gameLogicServiceBindings);
            botGameServer.OnCardReceived += async _ =>
                await PassGameState(playerGameServer, match.Id, match.ClientGameState.Team, gameLogicServiceBindings);

            _bot = new Bot(botGameServer, new CenterBrain());

            return playerGameServer;
        }

        private static async Task PassGameState(CloudGameServer otherServer, string matchId, Team team,
            GameLogicServiceBindings gameLogicServiceBindings)
        {
            var clientGameState = await gameLogicServiceBindings.GetClientGameState(matchId, team.ToDto());
            otherServer.Receive(clientGameState.ToModel());
        }

        private LocalGameServer CreateLocalGameServer()
        {
            GameState gameState = GameState.CreateInitial();
            LocalGameServer playerGameServer = new(gameState);
            LocalGameServer botGameServer = new(gameState);
            playerGameServer.OtherPlayerServer = botGameServer;
            botGameServer.OtherPlayerServer = playerGameServer;
            _bot = new Bot(botGameServer, new CenterBrain());

            return playerGameServer;
        }

        private CloudGameServer CreateCloudGameServer(string matchId)
        {
            GameLogicServiceBindings gameLogicService = new();
            CloudGameServer playerGameServer = new(gameLogicService, matchId);

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

        private async Task OnDestroy()
        {
            GameLogicServiceBindings gameLogicServiceBindings = new();
            bool success = await gameLogicServiceBindings.DeleteMatch(_matchId);
            Debug.Log(success);
        }
    }
}