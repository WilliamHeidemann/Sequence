using System;
using Game.Domain.Models;

namespace Game.Domain.Players
{
    public interface IOpponent
    {
        event Action<Move, GameStateData> OnMovePerformed;
        void PassGameState(GameStateData gameStateData);
    }
}