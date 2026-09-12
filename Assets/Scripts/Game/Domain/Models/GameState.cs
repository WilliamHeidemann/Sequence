using System;
using System.Linq;

namespace Game.Domain.Models
{
    public class GameState
    {
        public Hand RedHand { get; }
        public Hand YellowHand { get; }
        public Deck Deck { get; }
        public Board Board { get; }
        public MoveHistory MoveHistory { get; }
        public Score Score { get; }
        public Team ToPlay { get; set; }

        private GameState(Deck deck, Hand redHand, Hand yellowHand, Board board, MoveHistory moveHistory, 
            Score score, Team toStart)
        {
            Deck = deck;
            RedHand = redHand;
            YellowHand = yellowHand;
            Board = board;
            MoveHistory = moveHistory;
            Score = score;
            ToPlay = toStart;
        }

        public static GameState Create()
        {
            Deck deck = new();

            Card[] redCards = Enumerable.Range(0, 7).Select(_ => deck.Draw()).ToArray();
            Hand redHand = new(redCards);

            Card[] yellowCards = Enumerable.Range(0, 7).Select(_ => deck.Draw()).ToArray();
            Hand yellowHand = new(yellowCards);

            Board board = new(Array.Empty<Move>());

            MoveHistory moveHistory = new(Array.Empty<Move>());

            Score score = new();
            
            Random random = new();
            Team toPlay = random.NextDouble() < 0.5 ? Team.Red : Team.Yellow;

            return new GameState(deck, redHand, yellowHand, board, moveHistory, score, toPlay);
        }

        public ClientGameState ToClientGameState(Team team)
        {
            Hand hand = team == Team.Red ? RedHand : YellowHand;
            Card[] cards = hand.GetCards();
            bool isMyTurn = team == ToPlay;
            
            return new ClientGameState
            {
                Moves = MoveHistory.GetMoves(),
                Hand = cards,
                Team = team,
                IsMyTurn = isMyTurn
            };
        }
    }
}