using System;
using System.Threading.Tasks;
using Game.Domain.Models;
using Game.Domain.Players;
using Game.Domain.Server;
using UtilityToolkit.CollectionExtensions;

namespace Game.Domain
{
    public class PlayCoordinator
    {
        private readonly LocalPlayer _localPlayer;
        private readonly IGameServer _gameServer;

        public event Action<Position> OnInvalidMoveRequest;
        public event Action<Move> OnValidMoveRequest;
        public event Action<Card> OnDrawCard;
        public event Action<Move> OnOpponentPlayed;

        public PlayCoordinator(IGameServer gameServer, LocalPlayer localPlayer)
        {
            _gameServer = gameServer;
            _localPlayer = localPlayer;
            
            gameServer.OnCardReceived += card => OnDrawCard?.Invoke(card);
            gameServer.OnOpponentPlayed += Receive;
        }

        public async Task PositionClicked(Position position)
        {
            var attempt = _localPlayer.GetValidMove(position);

            if (attempt.IsSome(out Move move))
            {
                OnValidMoveRequest?.Invoke(move);
                await _gameServer.Request(move);
            }
            else
            {
                OnInvalidMoveRequest?.Invoke(position);
            }
        }

        private void Receive(ClientGameState clientGameState)
        {
            _localPlayer.PassGameState(clientGameState);
            
            clientGameState.Moves.LastOption().Try(lastMove =>
            {
                OnOpponentPlayed?.Invoke(lastMove);
            });
        }
    }
}