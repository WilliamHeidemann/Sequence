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
using Unity.Services.CloudCode.GeneratedBindings;
using UnityEngine;
using UtilityToolkit.Monads;

namespace Game.Presentation
{
    public class GameStarter : MonoBehaviour
    {
        [SerializeField] private BoardPresenter _boardPresenter;
        [SerializeField] private AnimationOrchestrator _animationOrchestrator;

        private PlayCoordinator _playCoordinator;
        private Option<string> _matchId;

        public void StartLocalGame()
        {
            _playCoordinator = CreateLocalPlayCoordinator();
            _animationOrchestrator.BindAnimations(_playCoordinator);
            _playCoordinator.RaiseDrawHandEvent();
            _boardPresenter.OnPositionClicked += HandlePositionClicked;
        }

        /// <returns>Match ID</returns>
        public async Task<string> StartOnlineGame(string opponentId)
        {
            _playCoordinator = await CreateCloudPlayCoordinator(opponentId);
            _playCoordinator.OnValidMoveRequest += async move => NotifyOpponent(opponentId);
            _animationOrchestrator.BindAnimations(_playCoordinator);
            _playCoordinator.RaiseDrawHandEvent();
            _boardPresenter.OnPositionClicked += HandlePositionClicked;

            if (_matchId.IsSome(out string matchId))
            {
                return matchId;
            }

            throw new Exception("Match Id has not been set.");
        }

        public void StartGame(string matchId, ClientGameState clientGameState)
        {
            _playCoordinator = CreatePlayCoordinator(matchId, clientGameState);
            _animationOrchestrator.BindAnimations(_playCoordinator);
            _playCoordinator.RaiseDrawHandEvent();
            _boardPresenter.OnPositionClicked += HandlePositionClicked;
        }

        private async Task NotifyOpponent(string opponentId)
        {
            if (!_matchId.IsSome(out string matchId))
            {
                Debug.LogError("MatchId does not exist. Opponent was not notified.");
                return;
            }
            
            PushMessagesServiceBindings pushMessagesServiceBindings = new();
            bool success = await pushMessagesServiceBindings.NotifyOpponentOfMove(matchId, opponentId);
            if (success)
            {
                Debug.Log($"Opponent {opponentId} was successfully notified of move.");
            }
            else
            {
                Debug.LogError($"Opponent {opponentId} was not notified of move.");
            }
        }

        public async Task OnNewMovePushMessageReceived(string matchId)
        {
            if (_playCoordinator != null)
            {
                await _playCoordinator.CheckIfOpponentPlayed(matchId);
            }
        }

        private PlayCoordinator CreateLocalPlayCoordinator()
        {
            GameState gameState = GameState.CreateInitial();

            ClientGameState clientGameState = gameState.ToClientGameState(gameState.ToPlay);

            LocalGameServer playerGameServer = CreateLocalGameServer(gameState);

            return new PlayCoordinator(playerGameServer, clientGameState);
        }

        private async Task<PlayCoordinator> CreateCloudPlayCoordinator(string opponentId)
        {
            ClientGameState clientGameState = await CreateCloudClientGameState(opponentId);

            CloudGameServer playerGameServer = CreateCloudBotGameServer();

            return new PlayCoordinator(playerGameServer, clientGameState);
        }

        private PlayCoordinator CreatePlayCoordinator(string matchId, ClientGameState startingState)
        {
            CloudGameServer playerGameServer = CreateCloudGameServer(matchId);
            return new PlayCoordinator(playerGameServer, startingState);
        }

        private async Task<ClientGameState> CreateCloudClientGameState(string opponentId)
        {
            GameLogicServiceBindings gameLogicServiceBindings = new();
            var matchDto = await gameLogicServiceBindings.CreateMatch(opponentId);
            Match match = matchDto.ToModel();
            _matchId = Option<string>.Some(match.Id);
            return match.ClientGameState;
        }

        private CloudGameServer CreateCloudBotGameServer()
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
            playerGameServer.OnCardReceived +=
                async _ => await PassGameState(botGameServer, matchId, gameLogicServiceBindings);
            botGameServer.OnCardReceived += async _ =>
                await PassGameState(playerGameServer, matchId, gameLogicServiceBindings);

            new Bot(botGameServer, new CenterBrain());

            return playerGameServer;
        }

        private static async Task PassGameState(CloudGameServer otherServer, string matchId,
            GameLogicServiceBindings gameLogicServiceBindings)
        {
            var clientGameState = await gameLogicServiceBindings.GetClientGameState(matchId);
            otherServer.Receive(clientGameState.ToModel());
        }

        private LocalGameServer CreateLocalGameServer(GameState gameState)
        {
            LocalGameState localGameState = new(gameState);
            LocalGameServer playerGameServer = new(localGameState);
            LocalGameServer botGameServer = new(localGameState);
            playerGameServer.OtherPlayerServer = botGameServer;
            botGameServer.OtherPlayerServer = playerGameServer;
            new Bot(botGameServer, new CenterBrain());

            return playerGameServer;
        }

        private CloudGameServer CreateCloudGameServer(string matchId)
        {
            GameLogicServiceBindings gameLogicService = new();
            return new CloudGameServer(gameLogicService, matchId);
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