using System.Linq;
using Game.Models;
using UnityEngine;
using UtilityToolkit.Runtime;

namespace Game
{
    public class Bot : ICommunicationProtocol
    {
        private readonly ICommunicationProtocol _opponent;
        private readonly Team _team;
        private readonly GameState _gameState = new(Team.Yellow);

        public Bot(ICommunicationProtocol opponent, Team team)
        {
            _opponent = opponent;
            _team = team;
        }

        public void PassGameState(GameStateData gameStateData)
        {
            SetGameState(gameStateData);
            Move move = DecideMove();
            Play(move);
        }


        private void SetGameState(GameStateData gameStateData)
        {
            _gameState.MoveHistory.Set(gameStateData.Moves);
            _gameState.Deck.Set(gameStateData.Deck);

            if (_team == Team.Red)
            {
                _gameState.MyHand.Set(gameStateData.RedHand);
                _gameState.OpponentHand.Set(gameStateData.YellowHand);
            }
            else
            {
                _gameState.MyHand.Set(gameStateData.YellowHand);
                _gameState.OpponentHand.Set(gameStateData.RedHand);
            }

            _gameState.Board.Set(gameStateData.Moves);
        }

        private Move DecideMove()
        {
            Card card = _gameState.MyHand.GetCards().Where(card => card.Rank != Rank.Jack).RandomElement();
            Position position = BoardLayout.Get(card);

            if (!_gameState.Board.Fits(position))
            {
                card = card.Equivalent;
                position = BoardLayout.Get(card);
            }

            Move move = new()
            {
                Card = card,
                Position = position,
                Team = _team
            };

            return move;
        }

        private void Play(Move move)
        {
            _gameState.Board.TryAddPin(move.Position, _team);

            _gameState.MyHand.TryRemove(move.Card);

            _gameState.MoveHistory.Add(move);

            while (!_gameState.MyHand.IsFull)
            {
                _gameState.MyHand.TryAdd(_gameState.Deck.Draw());
            }

            _opponent.PassGameState(_gameState.ToData());
        }
    }
}