using Game.Domain.Models;

namespace Game.Domain
{
    public interface IGameStateProvider
    {
        public GameState Get();
        public void Set(GameState gameState);
    }

    public class LocalGameStateProvider : IGameStateProvider
    {
        private GameState _gameState;

        public LocalGameStateProvider(GameState gameState)
        {
            _gameState = gameState;
        }

        public GameState Get()
        {
            return _gameState;
        }

        public void Set(GameState gameState)
        {
            _gameState = gameState;
        }
    }
}