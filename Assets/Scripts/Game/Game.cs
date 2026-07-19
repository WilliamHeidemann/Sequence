using System;
using System.Collections.Generic;
using Game.Models;
using UnityEngine;
using UnityEngine.UIElements;
using UtilityToolkit.Runtime;
using Position = Game.Models.Position;

namespace Game
{
    public class Game : MonoBehaviour, ICommunicationProtocol
    {
        [SerializeField] private BoardPresenter _boardPresenter;
        [SerializeField] private CardDrawAnimator _cardDrawAnimator;
        [SerializeField] private CardAligner _cardAligner;
        [SerializeField] private DiscardPile _discardPile;

        private Team MyTeam => _gameState.MyTeam;
        private Hand MyHand => _gameState.MyHand;
        private Hand OpponentHand => _gameState.OpponentHand;
        private Deck Deck => _gameState.Deck;
        private Board Board => _gameState.Board;
        private MoveHistory MoveHistory => _gameState.MoveHistory;

        private bool _isMyTurn = true;

        private GameState _gameState = new();

        public ICommunicationProtocol Opponent { get; set; }

        private void Start()
        {
            _boardPresenter.OnCardClicked += HandleCardClicked;
            _ = PlayDrawAnimation(MyHand.GetCards());
        }

        private void HandleCardClicked(Card card)
        {
            Position position = BoardLayout.Get(card);

            if (!_isMyTurn)
            {
                _boardPresenter.Shake(position);
                return;
            }

            bool hasCardInHand = MyHand.Contains(card) || MyHand.Contains(card.Equivalent);

            if (!hasCardInHand)
            {
                _boardPresenter.Shake(position);
                return;
            }

            bool onlyHasEquivalent = !MyHand.Contains(card) && MyHand.Contains(card.Equivalent);

            if (onlyHasEquivalent)
            {
                card = card.Equivalent;
            }

            int sequenceCountBefore = Board.SequenceCount(MyTeam);

            bool wasPositionFree = Board.TryAddPin(position, MyTeam);

            if (!wasPositionFree)
            {
                _boardPresenter.Shake(position);
                return;
            }

            if (!MyHand.TryRemove(card))
            {
                Debug.LogError($"Unexpected behavior: {card} could not be removed from the hand.");
                return;
            }

            int sequenceCountAfter = Board.SequenceCount(MyTeam);

            int sequenceCountDelta = sequenceCountAfter - sequenceCountBefore;

            if (sequenceCountDelta > 0)
            {
                Debug.Log("SEQUENCE!");
            }

            _ = SuccessfulPlayAnimation(card, position);

            var move = new Move()
            {
                Card = card,
                Position = position,
                Team = MyTeam
            };

            MoveHistory.Add(move);

            if (Opponent != null)
            {
                Opponent.SendGameState(_gameState.ToData());
                _isMyTurn = false;
            }
        }

        private async Awaitable PlayDrawAnimation(IEnumerable<Card> cards)
        {
            foreach (Card card in cards)
            {
                UIDocument cardUIDocument = _cardDrawAnimator.InstantiateCardFaceDown();
                await _cardDrawAnimator.AnimateDrawing(card, cardUIDocument);
                _cardAligner.AddCard(card, cardUIDocument.transform);
            }
        }

        private async Awaitable SuccessfulPlayAnimation(Card card, Position position)
        {
            _boardPresenter.Pop(position);

            _boardPresenter.Pin(position, MyTeam);

            await Awaitable.WaitForSecondsAsync(0.5f);

            if (_cardAligner.RemoveCard(card, out Transform cardTransform))
            {
                _discardPile.Discard(cardTransform);
            }

            await Awaitable.WaitForSecondsAsync(1f);

            var drawnCards = MyHand.DrawUntilFull(Deck);

            await PlayDrawAnimation(drawnCards);
        }

        public void SendMove(Move move)
        {
            // Received move ^
            MoveHistory.Add(move);
            _boardPresenter.Pin(move.Position, move.Team);
            _isMyTurn = true;
        }

        public void SendGameState(GameStateData gameStateData)
        {
            MoveHistory.Set(gameStateData.Moves);
            Deck.Set(gameStateData.Deck);
            Board.Set(gameStateData.Moves);
            Card[] opponentHand = MyTeam == Team.Red ? gameStateData.YellowHand : gameStateData.RedHand;
            OpponentHand.Set(opponentHand);

            // if game just loaded:
            // set my hand
            // display all cards (no animation)

            gameStateData.Moves.LastOption().Try(lastMove =>
                _boardPresenter.Pin(lastMove.Position, lastMove.Team));

            _isMyTurn = true;
        }
    }
}