using Game.Domain.Models;
using Game.Domain.Players.Bot_Strategies;
using Game.Domain.Server;

namespace Game.Domain.Players
{
    public class Bot
    {
        private readonly IBrain _brain;
        private readonly LocalGameServer _localGameServer;

        public Bot(LocalGameServer localGameServer, IBrain brain)
        {
            _brain = brain;
            _localGameServer = localGameServer;
            
            localGameServer.OnOpponentPlayed += PassGameState;
        }

        public void PassGameState(ClientGameState clientGameState)
        {
            Move move = _brain.DecideMove(clientGameState);
            _localGameServer.Request(move);
        }
    }
}