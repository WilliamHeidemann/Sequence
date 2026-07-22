using System;
using Game.Models.Players.Bot_Strategies;
using UnityEngine;

namespace Game.Models.Players
{
    public class Bot : IOpponent
    {
        private readonly GameState _gameState;
        private readonly IBot _brain;

        public Bot(Team team, IBot brain)
        {
            _gameState = new GameState(team);
            _brain = brain;
        }

        public event Action<Move, GameStateData> OnMovePerformed;

        public void PassGameState(GameStateData gameStateData)
        {
            SetGameState(gameStateData);
            Move move = _brain.DecideMove(_gameState);
            Play(move);
        }


        private void SetGameState(GameStateData gameStateData)
        {
            _gameState.MoveHistory.Set(gameStateData.Moves);
            _gameState.Deck.Set(gameStateData.Deck);

            if (_gameState.MyTeam == Team.Red)
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

        private void Play(Move move)
        {
            _gameState.Board.TryAddPin(move.Position, _gameState.MyTeam);

            _gameState.MyHand.TryRemove(move.Card);

            _gameState.MoveHistory.Add(move);

            while (!_gameState.MyHand.IsFull)
            {
                if (!_gameState.MyHand.TryAdd(_gameState.Deck.Draw()))
                {
                    Debug.LogError("Unexpected behavior: Could not draw card.");
                    break;
                }
            }

            OnMovePerformed?.Invoke(move, _gameState.ToData());
        }
    }
}