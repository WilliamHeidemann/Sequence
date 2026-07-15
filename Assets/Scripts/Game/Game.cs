using System;
using Game.Models;
using UnityEngine;

namespace Game
{
    public class Game : MonoBehaviour
    {
        [SerializeField] private BoardPresenter _boardPresenter;
        [SerializeField] private HandPresenter _handPresenter;
        
        private Team _currentTeam = Team.Red;
        private readonly Hand _redHand = new();
        private readonly Hand _yellowHand = new();
        private readonly Deck _deck = new();
        private readonly Board _board = new();

        private Hand CurrentTeamHand => _currentTeam == Team.Red ? _redHand : _yellowHand;

        private void Start()
        {
            _boardPresenter.OnCardClicked += HandleCardClicked;
            _handPresenter.Bind(_redHand);
            Fill(_redHand);
            Fill(_yellowHand);
        }

        private void Fill(Hand hand)
        {
            while(hand.Count < 7)
            {
                hand.TryAdd(_deck.Draw());
            }
        }

        private void HandleCardClicked(Card card)
        {
            bool contains = CurrentTeamHand.Contains(card);
            if (!contains)
            {
                Debug.LogWarning($"Card {card} was not in hand. Hand: {string.Join(", ", CurrentTeamHand.GetCards())}");
                return;
            }

            Position position = BoardLayout.Get(card);

            int previousSequenceCount = _board.SequenceCount(_currentTeam);

            bool canAddPin = _board.TryAddPin(position, _currentTeam);

            if (canAddPin)
            {
                Debug.Log($"Team {_currentTeam} placed a pin on {position} with card {card}");
                
                // if so, put a pin in it, check for sequence, draw a new card and pass the turn
                
                int newSequenceCount = _board.SequenceCount(_currentTeam);

                int difference = newSequenceCount - previousSequenceCount;

                if (difference > 0)
                {
                    Debug.Log("SEQUENCE!");
                }
                
                CurrentTeamHand.TryRemove(card);
                CurrentTeamHand.TryAdd(_deck.Draw());
                
                _boardPresenter.Mark(position, _currentTeam);
                
                _currentTeam = _currentTeam == Team.Red ? Team.Yellow : Team.Red;
                
                _handPresenter.Bind(CurrentTeamHand);
            }
            else
            {
                Debug.LogWarning($"Card {card} was not placed on {position} by {_currentTeam}");
            }
        }
    }
}