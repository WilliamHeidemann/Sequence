using System;
using System.Linq;
using UnityEngine;
using UtilityToolkit.Runtime;
using Random = UnityEngine.Random;

namespace Game.Models.Players
{
    public class Bot : IOpponent
    {
        private readonly Team _team;
        private readonly GameState _gameState;

        public Bot(Team team)
        {
            _team = team;
            _gameState = new GameState(team);
        }

        public event Action<Move, GameStateData> OnMovePerformed;

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
            (Position first, Position second) = BoardLayout.Get(card);

            if (Random.value < 0.5f) (first, second) = (second, first);

            Position position = _gameState.Board.Fits(first) ? first : second;

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