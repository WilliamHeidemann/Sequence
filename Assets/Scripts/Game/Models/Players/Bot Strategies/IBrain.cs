namespace Game.Models.Players.Bot_Strategies
{
    public interface IBrain
    {
        Move DecideMove(GameState gameState);
    }
}