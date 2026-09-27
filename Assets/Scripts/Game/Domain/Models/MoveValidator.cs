using System;
using System.Collections.Generic;
using System.Linq;
using UtilityToolkit.Monads;

namespace Game.Domain.Models
{
    public static class MoveValidator
    {
        public static Option<Move> GetValidMove(ClientGameState clientGameState, Position position)
        {
            if (!clientGameState.IsMyTurn)
            {
                return Option<Move>.None;
            }

            Board board = new(clientGameState.Moves);

            Hand hand = new(clientGameState.Hand);

            Card tabbedCard = BoardLayout.Get(position);

            bool fits = board.Fits(position);

            Option<Card> requiredCard = hand.FindCard(tabbedCard, fits);

            if (!requiredCard.IsSome(out Card cardInHand))
            {
                return Option<Move>.None;
            }

            if (cardInHand.IsRemover())
            {
                bool ownerIsPlayer = board.Owner(position).IsSome(out Team owner) && owner == clientGameState.Team;

                if (ownerIsPlayer)
                {
                    return Option<Move>.None;
                }
            }

            Move move = new(position, cardInHand, clientGameState.Team);

            Team toPlay = clientGameState.IsMyTurn ? clientGameState.Team : clientGameState.Team.Opposing();

            return IsValid(move, board, hand, toPlay) ? Option<Move>.Some(move) : Option<Move>.None;
        }

        public static bool IsValid(Move move, Board board, Hand hand, Team toPlay)
        {
            if (move.Team != toPlay)
            {
                return false;
            }

            bool isOpen = board.Fits(move.Position);

            var requiredCard = hand.FindCard(move.Card, isOpen);

            if (!requiredCard.IsSome(out Card cardInHand))
            {
                return false;
            }

            if (!cardInHand.IsRemover())
            {
                return true;
            }

            bool playerOwnsPosition = board.Owner(move.Position)
                .SelectOrDefault(owner => owner == move.Team);

            return !playerOwnsPosition;
        }

        public abstract class MoveResult
        {
            public class Success : MoveResult
            {
                public GameState UpdatedState { get; }
                public Card DrawnCard { get; }
                public int DeltaScore { get; }

                public Success(GameState updatedState, Card drawnCard, int deltaScore)
                {
                    UpdatedState = updatedState;
                    DrawnCard = drawnCard;
                    DeltaScore = deltaScore;
                }

                public void Deconstruct(out GameState updatedState, out Card drawnCard, out int deltaScore)
                {
                    updatedState = UpdatedState;
                    drawnCard = DrawnCard;
                    deltaScore = DeltaScore;
                }
            }

            public class Invalid : MoveResult
            {
            }

            public class OutOfSync : MoveResult
            {
            }
        }

        public static MoveResult PlayMove(GameState gameState, Move move)
        {
            Card[] cardsInHand = move.Team switch
            {
                Team.Red => gameState.RedHand,
                Team.Yellow => gameState.YellowHand,
                _ => throw new ArgumentOutOfRangeException()
            };

            Hand hand = new(cardsInHand);
            Board board = new(gameState.Moves);

            if (!IsValid(move, board, hand, gameState.ToPlay))
            {
                return new MoveResult.Invalid();
            }

            if (move.Card.IsRemover()) board.Remove(move.Position);
            else board.TryAdd(move.Position, move.Team);

            hand.TryRemove(move.Card);

            Deck deck = new(gameState.Deck);
            Card draw = deck.Draw();

            hand.TryAdd(draw);

            MoveHistory moveHistory = new(gameState.Moves);
            moveHistory.Add(move);

            HashSet<Position> locked = new(gameState.Locked);
            var sequences = Sequences(move, board, locked).ToArray();
            int deltaScore = sequences.Length;
            foreach (var sequence in sequences)
            {
                foreach (Position position in sequence)
                {
                    locked.Add(position);
                }
            }

            (Card[] redHand, Card[] yellowHand) = move.Team switch
            {
                Team.Red => (hand.GetCards(), gameState.YellowHand),
                Team.Yellow => (gameState.RedHand, hand.GetCards()),
                _ => throw new ArgumentOutOfRangeException()
            };

            GameState updatedGameState = new(
                redHand, yellowHand,
                deck.GetCards(), moveHistory.GetMoves(), locked.ToArray(),
                gameState.Score, move.Team.Opposing());

            return new MoveResult.Success(updatedGameState, draw, deltaScore);
        }

        public static IEnumerable<Position[]> Sequences(Move move, Board board, HashSet<Position> locked)
        {
            IEnumerable<Position[]> possibleSequences = SequencePatterns.Around(move.Position);

            IEnumerable<Position[]> sequences = possibleSequences.Where(line =>
                line.All(p => p.Equals(move.Position) || board.Owner(p).IsSome(out Team owner) && owner == move.Team));

            IEnumerable<Position[]> sequencesExcludingLocked =
                sequences.Where(sequence => sequence.Count(locked.Contains) < 2);

            // for every sequence at this point, if they overlap by 2 or more pins, concatenate them.
            // n^2 operation. But there will be very few here so it is ok. 

            IEnumerable<Position[]> concatenatedSequences = sequencesExcludingLocked.CombineAll((seq1, seq2) =>
            {
                int overlap = seq1.Intersect(seq2).Count();
                if (overlap >= 2)
                {
                    return seq1.Concat(seq2).Distinct().ToArray();
                }

                return seq1;
            });

            return concatenatedSequences;
        }
    }

    public static class SequenceExtension
    {
        public static IEnumerable<TResult> CombineAll<T, TResult>(
            this IEnumerable<T> source, Func<T, T, TResult> combinator)
        {
            IEnumerable<T> enumerable = source as T[] ?? source.ToArray();

            foreach (T item1 in enumerable)
            {
                foreach (T item2 in enumerable)
                {
                    yield return combinator(item1, item2);
                }
            }
        }
        
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(
            this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector,
            IEqualityComparer<TKey>? comparer = null)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (keySelector == null) throw new ArgumentNullException(nameof(keySelector));

            return Iterator();

            IEnumerable<TSource> Iterator()
            {
                var knownKeys = new HashSet<TKey>(comparer);
                foreach (TSource element in source)
                {
                    if (knownKeys.Add(keySelector(element)))
                    {
                        yield return element;
                    }
                }
            }
        }
        
        public static Position[] GetSequences(IEnumerable<Position> positions)
        {
            // all lengths
            throw new NotImplementedException();
        }
    }
}