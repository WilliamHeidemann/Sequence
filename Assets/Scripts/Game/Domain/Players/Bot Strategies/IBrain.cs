using Game.Domain.Models;

namespace Game.Domain.Players.Bot_Strategies
{
    public interface IBrain
    {
        Move DecideMove(GameState gameState);
    }
}