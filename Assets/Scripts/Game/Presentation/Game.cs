using System.Collections.Generic;
using Game.Domain;
using Game.Domain.Models;
using Game.Domain.Players;
using Game.Domain.Players.Bot_Strategies;
using UnityEngine;
using UnityEngine.UIElements;
using Position = Game.Domain.Models.Position;

namespace Game.Presentation
{
    public class Game : MonoBehaviour
    {
        [SerializeField] private BoardPresenter _boardPresenter;
        [SerializeField] private DrawAnimator _drawAnimator;
        [SerializeField] private CardAligner _cardAligner;
        [SerializeField] private DiscardPile _discardPile;
        [SerializeField] private OpponentHandAnimator _opponentHandAnimator;

        public LocalPlayer LocalPlayer { get; set; }
        public IOpponent Opponent { get; set; }

        private void Start()
        {
            LocalPlayer = new LocalPlayer(Team.Red);
            Opponent = new Bot(Team.Yellow, new CenterBot());
            
            _boardPresenter.OnPositionClicked += HandlePositionClicked;
            
            LocalPlayer.OnMovePerformed += OnLocalPlayerMovePerformed;
            
            Opponent.OnMovePerformed += OnOpponentMovePerformed;
            
            _ = PlayDrawAnimation(LocalPlayer.MyHand.GetCards());
        }

        private async void OnLocalPlayerMovePerformed(Move move, GameStateData gameStateData)
        {
            Card drawnCard = LocalPlayer.MyHand.GetCards()[^1];
            LocalPlayer.IsMyTurn = false;
            await SuccessfulPlayAnimation(move, drawnCard);
            Opponent.PassGameState(LocalPlayer.GetGameStateData());
        }
        
        private async void OnOpponentMovePerformed(Move move, GameStateData gameStateData)
        {
            await AnimateOpponentPlay(move);
            LocalPlayer.PassGameState(gameStateData);
        }

        private void HandlePositionClicked(Position position)
        {
            int sequenceCountBefore = LocalPlayer.Board.SequenceCount(LocalPlayer.MyTeam);
            
            bool success = LocalPlayer.AttemptPlay(position);
            if (!success) _boardPresenter.Shake(position);
            else
            {
                int sequenceCountAfter = LocalPlayer.Board.SequenceCount(LocalPlayer.MyTeam);

                int sequenceCountDelta = sequenceCountAfter - sequenceCountBefore;

                if (sequenceCountDelta > 0)
                {
                    Debug.Log("SEQUENCE!");
                }
            }
        }

        private async Awaitable PlayDrawAnimation(IEnumerable<Card> cards)
        {
            foreach (Card card in cards)
            {
                UIDocument cardUIDocument = _drawAnimator.InstantiateCardFaceDown();
                await _drawAnimator.AnimateDrawing(card, cardUIDocument);
                _cardAligner.AddCard(card, cardUIDocument.transform);
            }
        }

        private async Awaitable SuccessfulPlayAnimation(Move move, Card drawnCard)
        {
            _boardPresenter.Pop(move.Position);

            if (_cardAligner.RemoveCard(move.Card, out Transform cardTransform))
            {
                await _discardPile.Discard(cardTransform);
            }

            if (move.Card.IsRemover())
            {
                await _boardPresenter.RemovePin(move.Position);
            }
            else
            {
                await _boardPresenter.Pin(move.Position, move.Team);
            }

            await PlayDrawAnimation(new[] { drawnCard });
        }

        private async Awaitable AnimateOpponentPlay(Move move)
        {
            await Awaitable.WaitForSecondsAsync(1f); // simulate thinking time.
            await _opponentHandAnimator.AnimatePlay(move.Card);
            if (move.Card.IsRemover())
            {
                await _boardPresenter.RemovePin(move.Position);
            }
            else
            {
                await _boardPresenter.Pin(move.Position, move.Team);
            }
        }
    }
}