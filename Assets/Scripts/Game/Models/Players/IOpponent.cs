using System;

namespace Game.Models.Players
{
    public interface IOpponent
    {
        event Action<Move, GameStateData> OnMovePerformed;
        void PassGameState(GameStateData gameStateData);
    }
}