using Game.Domain;
using Game.Domain.Models;
using Game.Domain.Players;
using Game.Domain.Players.Bot_Strategies;
using Game.Domain.Server;
using Game.Presentation.AnimationSystems;
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

        private void Start()
        {
            GameState gameState = GameState.Create();
            
            LocalGameServer playerGameServer = new(gameState);
            LocalGameServer botGameServer = new(gameState);
            playerGameServer.OtherPlayerServer = botGameServer;
            botGameServer.OtherPlayerServer = playerGameServer;
            
            _localPlayer = new LocalPlayer(gameState.ToClientGameState(gameState.ToPlay));
            
            _playCoordinator = new PlayCoordinator(playerGameServer, _localPlayer);
            _animationOrchestrator.BindAnimations(_playCoordinator);
            _animationOrchestrator.PlayDrawAnimation(_localPlayer.Hand.GetCards());
            _boardPresenter.OnPositionClicked += _playCoordinator.PositionClicked;
            
            _bot = new Bot(botGameServer, new CenterBrain());
        }
    }
}