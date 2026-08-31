using System;
using Game.Domain.Models;

namespace Game.Domain.Players
{
    public interface IOpponent
    {
        event Action<Move> OnMovePerformed;
        void PassGameState(ClientGameState clientGameState);
    }
}