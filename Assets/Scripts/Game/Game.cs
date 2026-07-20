using System;
using System.Collections.Generic;
using System.Linq;
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
        // private Option<Card> _selectedCard = Option<Card>.None; // = Option<Card>.Some(Card.JackOfClubs);

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
            Debug.Log(string.Join(", ", MyHand.GetCards()));
            
            Position position = BoardLayout.Get(tabbedCard);

            if (!_isMyTurn)
            {
                _boardPresenter.Shake(position);
                return;
            }
            
            bool isOpenSpace = Board.Fits(position);
            
            Option<Card> requiredCard = MyHand.FindCard(tabbedCard, isOpenSpace);
            
            if (!requiredCard.IsSome(out Card cardInHand))
            {
                _boardPresenter.Shake(position);
                return;
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
                Debug.LogError($"Unexpected behavior: {cardInHand} could not be removed from the hand.");
                return;
            }
            
            await SuccessfulPlayAnimation(cardInHand, position);

            Move move = new()
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