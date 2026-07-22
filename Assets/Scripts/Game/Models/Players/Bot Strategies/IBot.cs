namespace Game.Models.Players.Bot_Strategies
{
    public interface IBot
    {
        Move DecideMove(GameState gameState);
    }
}