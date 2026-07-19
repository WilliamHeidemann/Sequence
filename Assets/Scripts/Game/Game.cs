using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
        private Option<Card> _selectedCard = Option<Card>.None; // = Option<Card>.Some(Card.JackOfClubs);

        private GameState _gameState = new(Team.Red);

        public ICommunicationProtocol Opponent { get; set; }

        private void Start()
        {
            _boardPresenter.OnCardClicked += HandleCardClicked;
            _ = PlayDrawAnimation(MyHand.GetCards());
            Opponent = new Bot(this, Team.Yellow);
        }

        private async Awaitable HandleCardClicked(Card tabbedCard)
        {
            Position position = BoardLayout.Get(tabbedCard);

            if (!_isMyTurn)
            {
                _boardPresenter.Shake(position);
                Debug.Log("Not my turn.");
                return;
            }

            if (_selectedCard.IsSome(out Card cardInHand) && cardInHand.IsWild)
            {
                await PlayWild(cardInHand, position);
            }
            else
            {
                await PlayCard(tabbedCard, position);
            }
        }

        private async Awaitable PlayWild(Card cardInHand, Position position)
        {
            bool wasPositionFree = Board.TryAddPin(position, MyTeam);

            if (!wasPositionFree)
            {
                _boardPresenter.Shake(position);
                Debug.Log("That position is not free.");
                return;
            }
            
            if (!MyHand.TryRemove(cardInHand))
            {
                Debug.LogError($"Unexpected behavior: {cardInHand} could not be removed from the hand.");
                return;
            }
            
            await SuccessfulPlayAnimation(cardInHand, position);

            var move = new Move()
            {
                Card = cardInHand,
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

        private async Awaitable PlayCard(Card card, Position position)
        {
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

            await SuccessfulPlayAnimation(card, position);

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

            if (_cardAligner.RemoveCard(card, out Transform cardTransform))
            {
                await _discardPile.Discard(cardTransform);
            }
            
            await _boardPresenter.Pin(position, MyTeam);

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

            gameStateData.Moves.LastOption().Try(AnimateOpponentPlay);

            _isMyTurn = true;
        }

        private async void AnimateOpponentPlay(Move move)
        {
            await Awaitable.WaitForSecondsAsync(1f); // simulate thinking time.
            await _opponentHandAnimator.AnimatePlay(move.Card);
            await _boardPresenter.Pin(move.Position, move.Team);
        }
    }
}