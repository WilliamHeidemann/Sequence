using System;
using UnityEngine;
using UtilityToolkit.Runtime;

namespace Game.Models.Players
{
    public class LocalPlayer : IOpponent
    {
        private readonly GameState _gameState;
        public bool IsMyTurn { get; set; }
        public Team MyTeam => _gameState.MyTeam;
        public Hand MyHand => _gameState.MyHand;
        public Hand OpponentHand => _gameState.OpponentHand;
        public Deck Deck => _gameState.Deck;
        public Board Board => _gameState.Board;
        public MoveHistory MoveHistory => _gameState.MoveHistory;
        public GameStateData GetGameStateData() => _gameState.ToData();

        public event Action<Move, GameStateData> OnMovePerformed;
        
        public LocalPlayer(Team team)
        {
            IsMyTurn = true;
            _gameState = new GameState(team);
        }

        public void PassGameState(GameStateData gameStateData)
        {
            MoveHistory.Set(gameStateData.Moves);
            Deck.Set(gameStateData.Deck);
            Board.Set(gameStateData.Moves);
            Card[] opponentHand = MyTeam == Team.Red ? gameStateData.YellowHand : gameStateData.RedHand;
            OpponentHand.Set(opponentHand);

            // if game just loaded:
            // set my hand
            // display all cards (no animation)
            
            IsMyTurn = true;
        }

        public bool AttemptPlay(Position position)
        {
            Card tabbedCard = BoardLayout.Get(position);

            if (!IsMyTurn)
            {
                return false;
            }

            bool isOpenSpace = Board.Fits(position);

            Option<Card> requiredCard = MyHand.FindCard(tabbedCard, isOpenSpace);

            if (!requiredCard.IsSome(out Card cardInHand))
            {
                return false;
            }

            int sequenceCountBefore = Board.SequenceCount(MyTeam);

            if (!Board.TryAddPin(position, MyTeam))
            {
                Debug.LogError($"Unexpected behavior: {position} could not be pinned.");
            }

            int sequenceCountAfter = Board.SequenceCount(MyTeam);

            int sequenceCountDelta = sequenceCountAfter - sequenceCountBefore;

            if (sequenceCountDelta > 0)
            {
                Debug.Log("SEQUENCE!");
            }

            if (!MyHand.TryRemove(cardInHand))
            {
                throw new ArgumentOutOfRangeException($"Unexpected behavior: {cardInHand} was not in the hand.");
            }

            Card drawnCard = Deck.Draw();

            if (!MyHand.TryAdd(drawnCard))
            {
                throw new ArgumentOutOfRangeException($"Unexpected behavior: {drawnCard} could not be added.");
            }

            Move move = new()
            {
                Card = cardInHand,
                Position = position,
                Team = MyTeam
            };

            MoveHistory.Add(move);
            
            OnMovePerformed?.Invoke(move, _gameState.ToData());

            return true;
        }
    }
}