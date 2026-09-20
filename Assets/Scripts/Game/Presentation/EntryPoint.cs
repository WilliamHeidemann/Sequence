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
using UtilityToolkit.Monads;

namespace Game.Presentation
{
    public class EntryPoint : MonoBehaviour
    {
        [SerializeField] private BoardPresenter _boardPresenter;
        [SerializeField] private AnimationOrchestrator _animationOrchestrator;
        [SerializeField] private Mode _mode;

        public enum Mode
        {
            Local,
            Online
        }

        private Bot _bot;
        private PlayCoordinator _playCoordinator;
        private Option<string> _matchId;

        private async void Start()
        {
            await UnityServices.InitializeAsync();

            await AuthenticationService.Instance.SignInAnonymouslyAsync();

            // IGameServer playerGameServer = CreateCloudGameServer(gameState);

            _playCoordinator = _mode switch
            {
                Mode.Local => CreateLocalPlayCoordinator(),
                Mode.Online => await CreateCloudPlayCoordinator(),
                _ => throw new ArgumentOutOfRangeException()
            };

            _animationOrchestrator.BindAnimations(_playCoordinator);
            _playCoordinator.RaiseDrawHandEvent();
            _boardPresenter.OnPositionClicked += HandlePositionClicked;
        }

        private PlayCoordinator CreateLocalPlayCoordinator()
        {
            GameState gameState = GameState.CreateInitial();
            
            ClientGameState clientGameState = gameState.ToClientGameState(gameState.ToPlay);
            
            LocalGameServer playerGameServer = CreateLocalGameServer(gameState);

            return new PlayCoordinator(playerGameServer, clientGameState);
        }

        private async Task<PlayCoordinator> CreateCloudPlayCoordinator()
        {
            ClientGameState clientGameState = await CreateCloudClientGameState();

            CloudGameServer playerGameServer = CreateCloudBotGameServer(clientGameState);

            return new PlayCoordinator(playerGameServer, clientGameState);
        }

        private async Task<ClientGameState> CreateCloudClientGameState()
        {
            GameLogicServiceBindings gameLogicServiceBindings = new();
            var matchDto = await gameLogicServiceBindings.CreateMatch();
            Match match = matchDto.ToModel();
            _matchId = Option<string>.Some(match.Id);
            return match.ClientGameState;
        }

        private CloudGameServer CreateCloudBotGameServer(ClientGameState clientGameState)
        {
            if (!_matchId.IsSome(out string matchId))
            {
                throw new Exception("MatchId does not exist");
            }
            
            GameLogicServiceBindings gameLogicServiceBindings = new();
            CloudGameServer playerGameServer = new(gameLogicServiceBindings, matchId);
            CloudGameServer botGameServer = new(gameLogicServiceBindings, matchId);
            // The following is only possible when both clients are on the same machine. 
            // This is to use remote gameplay without push messages implemented. 
            playerGameServer.OnCardReceived += async _ =>
                await PassGameState(botGameServer, matchId, clientGameState.Team.Opposing(),
                    gameLogicServiceBindings);
            botGameServer.OnCardReceived += async _ =>
                await PassGameState(playerGameServer, matchId, clientGameState.Team, gameLogicServiceBindings);

            _bot = new Bot(botGameServer, new CenterBrain());

            return playerGameServer;
        }

        private static async Task PassGameState(CloudGameServer otherServer, string matchId, Team team,
            GameLogicServiceBindings gameLogicServiceBindings)
        {
            var clientGameState = await gameLogicServiceBindings.GetClientGameState(matchId, team.ToDto());
            otherServer.Receive(clientGameState.ToModel());
        }

        private LocalGameServer CreateLocalGameServer(GameState gameState)
        {
            LocalGameState localGameState = new(gameState);
            LocalGameServer playerGameServer = new(localGameState);
            LocalGameServer botGameServer = new(localGameState);
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
            if (_matchId.IsSome(out string matchId))
            {
                GameLogicServiceBindings gameLogicServiceBindings = new();
                bool success = await gameLogicServiceBindings.DeleteMatch(matchId);
                Debug.Log(success);    
            }
        }
    }
}