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
        [SerializeField] private OpponentHandAnimator _opponentHandAnimator;

        private Team MyTeam => _gameState.MyTeam;
        private Hand MyHand => _gameState.MyHand;
        private Hand OpponentHand => _gameState.OpponentHand;
        private Deck Deck => _gameState.Deck;
        private Board Board => _gameState.Board;
        private MoveHistory MoveHistory => _gameState.MoveHistory;

        private bool _isMyTurn = true;

        private GameState _gameState = new(Team.Red);

        public ICommunicationProtocol Opponent { get; set; }

        private void Start()
        {
            _boardPresenter.OnCardClicked += HandleCardClicked;
            _ = PlayDrawAnimation(MyHand.GetCards());
            Opponent = new Bot(this, Team.Yellow);
        }

        private void HandleCardClicked(Card card)
        {
            Position position = BoardLayout.Get(card);

            if (!_isMyTurn)
            {
                _boardPresenter.Shake(position);
                Debug.Log("Not my turn.");
                return;
            }

            bool hasCardInHand = MyHand.Contains(card) || MyHand.Contains(card.Equivalent);

            if (!hasCardInHand)
            {
                Debug.Log("I don't have that card.");
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
                Debug.Log("That position is not free.");
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
                _isMyTurn = false;
                Opponent.PassGameState(_gameState.ToData());
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

            Card draw = Deck.Draw();

            MyHand.TryAdd(draw);

            await PlayDrawAnimation(new[] { draw });
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

            gameStateData.Moves.LastOption().Try(lastMove =>
            {
                _opponentHandAnimator.AnimatePlay(lastMove.Card);
                _boardPresenter.Pin(lastMove.Position, lastMove.Team);
            });

            _isMyTurn = true;
        }
    }
}