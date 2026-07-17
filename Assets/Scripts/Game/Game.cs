using Game.Models;
using UnityEngine;

namespace Game
{
    public class Game : MonoBehaviour
    {
        [SerializeField] private BoardPresenter _boardPresenter;
        [SerializeField] private CardDrawAnimator _cardDrawAnimator;

        private Team _currentTeam = Team.Red;
        private readonly Hand _redHand = new();
        private readonly Hand _yellowHand = new();
        private readonly Deck _deck = new();
        private readonly Board _board = new();

        private Hand CurrentTeamHand => _currentTeam == Team.Red ? _redHand : _yellowHand;

        private void Start()
        {
            _boardPresenter.OnCardClicked += HandleCardClicked;
            Fill(_redHand);
            // Fill(_yellowHand);
        }

        private async Awaitable Fill(Hand hand)
        {
            while (hand.TryAdd(_deck.Draw(out Card card)))
            {
                await _cardDrawAnimator.Draw(card);
            }
        }

        public void HandleCardClicked(Card card)
        {
            bool contains = CurrentTeamHand.Contains(card);

            Position position = BoardLayout.Get(card);

            if (!contains)
            {
                Debug.LogWarning($"Team {_currentTeam} does not have card {card} in hand.");
                _boardPresenter.Shake(position);
                return;
            }

            int previousSequenceCount = _board.SequenceCount(_currentTeam);

            bool canAddPin = _board.TryAddPin(position, _currentTeam);

            if (canAddPin)
            {
                int newSequenceCount = _board.SequenceCount(_currentTeam);

                int difference = newSequenceCount - previousSequenceCount;

                if (difference > 0)
                {
                    Debug.Log("SEQUENCE!");
                }

                CurrentTeamHand.TryRemove(card);

                CurrentTeamHand.TryAdd(_deck.Draw());

                _boardPresenter.Pin(position, _currentTeam);

                _redHand.TryRemove(card);
                
                // animate card being discarded
                // currently there is no link between the cards logically in the hand and the visual cards in the scene.
                
                _currentTeam = _currentTeam == Team.Red ? Team.Yellow : Team.Red;
            }
            else
            {
                Debug.LogWarning($"Card {card} is already pinned.");
            }
        }
    }
}