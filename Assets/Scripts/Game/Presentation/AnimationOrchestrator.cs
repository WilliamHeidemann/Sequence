using System;
using System.Collections.Generic;
using Game.Domain;
using Game.Domain.Models;
using Game.Domain.Players;
using UnityEngine;
using UnityEngine.UIElements;
using Position = Game.Domain.Models.Position;

namespace Game.Presentation
{
    public class AnimationOrchestrator : MonoBehaviour
    {
        [SerializeField] private BoardPresenter _boardPresenter;
        [SerializeField] private DrawAnimator _drawAnimator;
        [SerializeField] private CardAligner _cardAligner;
        [SerializeField] private DiscardPile _discardPile;
        [SerializeField] private OpponentHandAnimator _opponentHandAnimator;
        
        public async Awaitable PlaySequenceCelebration()
        {
            Debug.Log("SEQUENCE!");
        }

        private async Awaitable OnLocalPlayerMovePerformed(Move move)
        {
            await PlayDiscardAndPinAnimation(move);
            await PlayDrawAnimation(new[] { drawnCard });
            Opponent.PassGameState(LocalPlayer.GetGameStateData());
        }
        
        private async Awaitable OnOpponentMovePerformed(Move move, ClientGameState clientGameState)
        {
            await AnimateOpponentPlay(move);
            // LocalPlayer.PassGameState(clientGameState);
        }

        private void HandlePositionClicked(Position position)
        {
            int sequenceCountBefore = LocalPlayer.Board.SequenceCount(LocalPlayer.Team);
            
            bool success = LocalPlayer.AttemptPlay(position);
            if (!success) _boardPresenter.Shake(position);
            else
            {
                int sequenceCountAfter = LocalPlayer.Board.SequenceCount(LocalPlayer.Team);

                if (sequenceCountAfter > sequenceCountBefore)
                {
                    PlaySequenceCelebration();
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

        public async Awaitable PlayDiscardAndPinAnimation(Move move)
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
    
    public static class AwaitableExtensions
    {
        public static async void Forget(this Awaitable awaitable)
        {
            try { await awaitable; } 
            catch (Exception exception) { Debug.LogException(exception); }
        }

        public static async Awaitable<TResult> Then<TSource, TResult>(
            this Awaitable<TSource> task,
            Func<TSource, Awaitable<TResult>> continuation)
        {
            TSource result = await task;
            return await continuation(result);
        }
    }
}