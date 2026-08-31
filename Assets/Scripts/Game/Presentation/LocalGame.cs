using Game.Domain;
using Game.Domain.Models;
using Game.Domain.Players;
using Game.Domain.Players.Bot_Strategies;
using UnityEngine;

namespace Game.Presentation
{
    public class LocalGame : MonoBehaviour
    {
        [SerializeField] private BoardPresenter _boardPresenter;
        [SerializeField] private AnimationOrchestrator _animationOrchestrator;
        
        private LocalPlayer _localPlayer;
        private Bot _bot;
        private LocalPlayCoordinator _playCoordinator;

        private void Start()
        {
            IGameStateProvider gameStateProvider = new LocalGameStateProvider(GameState.Create());
            GameEngine gameEngine = new(gameStateProvider);
            _localPlayer = new LocalPlayer();
            _bot = new Bot(new CenterBrain());
            _playCoordinator = new LocalPlayCoordinator(gameEngine, gameStateProvider, _localPlayer, _bot);

            _boardPresenter.OnPositionClicked += OnPositionClicked;
            _playCoordinator.OnValidMoveRequest += AnimateValidMove;
            _playCoordinator.OnInvalidMoveRequest += AnimateInvalidMove;
        }

        private void AnimateValidMove(Move move)
        {
            _animationOrchestrator.PlayDiscardAndPinAnimation(move)
                .Then(_animationOrchestrator.Play)
        }

        private void AnimateInvalidMove(Move move)
        {
            _boardPresenter.Shake(move.Position);
        }

        private void OnPositionClicked(Position position)
        {
            if (_localPlayer.AttemptPlay(position, out Move move))
            {
                _playCoordinator.Request(move);
            }
        }
    }
}