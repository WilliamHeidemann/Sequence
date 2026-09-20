using System;
using System.Threading.Tasks;
using Game.Domain.Models;
using Game.Domain.Server;
using UtilityToolkit.CollectionExtensions;
using UtilityToolkit.Monads;

namespace Game.Domain
{
    public class PlayCoordinator
    {
        private readonly IGameServer _gameServer;
        private ClientGameState _clientGameState;

        public event Action<Position> OnInvalidMoveRequest;
        public event Action<Move> OnValidMoveRequest;
        public event Action<Card> OnDrawCard;
        public event Action<Move> OnOpponentPlayed;

        public PlayCoordinator(IGameServer gameServer, ClientGameState startingState)
        {
            _gameServer = gameServer;
            _clientGameState = startingState;
            
            gameServer.OnCardReceived += card => OnDrawCard?.Invoke(card);
            gameServer.OnOpponentPlayed += Receive;
        }

        public async Task PositionClicked(Position position)
        {
            Option<Move> attempt = MoveValidator.GetValidMove(_clientGameState, position);

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

        public void RaiseDrawHandEvent()
        {
            foreach (Card card in _clientGameState.Hand)
            {
                OnDrawCard?.Invoke(card);
            }
        }
        
        private void Receive(ClientGameState clientGameState)
        {
            _clientGameState = clientGameState;
            
            clientGameState.Moves.LastOption().Try(lastMove =>
            {
                OnOpponentPlayed?.Invoke(lastMove);
            });
        }
    }
}