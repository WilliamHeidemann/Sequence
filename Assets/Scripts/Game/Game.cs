using Game.Models;
using UnityEngine;
using UnityEngine.UIElements;
using Position = Game.Models.Position;

namespace Game
{
    public class Game : MonoBehaviour
    {
        [SerializeField] private BoardPresenter _boardPresenter;
        [SerializeField] private CardDrawAnimator _cardDrawAnimator;
        [SerializeField] private CardAligner _cardAligner;
        [SerializeField] private DiscardPile _discardPile;
        
        private Team _currentTeam = Team.Red;
        private readonly Hand _redHand = new();
        private readonly Hand _yellowHand = new();
        private readonly Deck _deck = new();
        private readonly Board _board = new();

        private Hand CurrentTeamHand => _currentTeam == Team.Red ? _redHand : _yellowHand;

        private void Start()
        {
            _boardPresenter.OnCardClicked += HandleCardClicked;
            _ = Fill(_redHand);
            // Fill(_yellowHand);
        }

        private async Awaitable Fill(Hand hand)
        {
            while (hand.TryAdd(_deck.Draw(out Card card)))
            {
                UIDocument cardUIDocument = _cardDrawAnimator.InstantiateCardFaceDown();
                await _cardDrawAnimator.Draw(card, cardUIDocument);
                _cardAligner.AddCard(card, cardUIDocument.transform);
            }
        }

        public void HandleCardClicked(Card card)
        {
            Position position = BoardLayout.Get(card);

            bool hasCardInHand = CurrentTeamHand.Contains(card) || CurrentTeamHand.Contains(card.Equivalent);

            if (!hasCardInHand)
            {
                _boardPresenter.Shake(position);
                return;
            }
            
            bool onlyHasEquivalent = !CurrentTeamHand.Contains(card) && CurrentTeamHand.Contains(card.Equivalent);
            
            if (onlyHasEquivalent)
            {
                card = card.Equivalent;
            }

            int sequenceCountBefore = _board.SequenceCount(_currentTeam);

            bool wasPositionFree = _board.TryAddPin(position, _currentTeam);

            if (!wasPositionFree)
            {
                _boardPresenter.Shake(position);
                return;
            }
            
            if (!CurrentTeamHand.TryRemove(card))
            {
                Debug.LogError($"Unexpected behavior: {card} could not be removed from the hand.");
                return;
            }
            
            int sequenceCountAfter = _board.SequenceCount(_currentTeam);

            int sequenceCountDelta = sequenceCountAfter - sequenceCountBefore;

            if (sequenceCountDelta > 0)
            {
                Debug.Log("SEQUENCE!");
            }
            
            _ = SuccessfulPlayAnimation(card, position);
            
            // _currentTeam = _currentTeam == Team.Red ? Team.Yellow : Team.Red;
        }

        private async Awaitable SuccessfulPlayAnimation(Card card, Position position)
        {
            _boardPresenter.Pop(position);

            _boardPresenter.Pin(position, _currentTeam);

            await Awaitable.WaitForSecondsAsync(0.5f);
            
            if (_cardAligner.RemoveCard(card, out Transform cardTransform))
            {
                _discardPile.Discard(cardTransform);
            }
            
            await Awaitable.WaitForSecondsAsync(1f);
            
            _ = Fill(_redHand);
        }
    }
}