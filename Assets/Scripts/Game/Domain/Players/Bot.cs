using System;
using Game.Domain.Models;
using Game.Domain.Players.Bot_Strategies;

namespace Game.Domain.Players
{
    public class Bot : IOpponent
    {
        private readonly IBrain _brain;
        public event Action<Move> OnMovePerformed;

        public Bot(IBrain brain)
        {
            _brain = brain;
        }

        public void PassGameState(ClientGameState clientGameState)
        {
            Move move = _brain.DecideMove(clientGameState);
            OnMovePerformed?.Invoke(move);
        }
    }
}