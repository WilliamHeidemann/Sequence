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
        public Team ToPlay { get; set; }

        private GameState(Deck deck, Hand redHand, Hand yellowHand, Board board, MoveHistory moveHistory, Team toStart)
        {
            Deck = deck;
            RedHand = redHand;
            YellowHand = yellowHand;
            Board = board;
            MoveHistory = moveHistory;
            ToPlay = toStart;
        }

        public static GameState Create()
        {
            var deck = new Deck();

            Card[] redCards = Enumerable.Range(0, 7).Select(_ => deck.Draw()).ToArray();
            var redHand = new Hand(redCards);

            Card[] yellowCards = Enumerable.Range(0, 7).Select(_ => deck.Draw()).ToArray();
            var yellowHand = new Hand(yellowCards);

            var board = new Board(Array.Empty<Move>());

            var moveHistory = new MoveHistory(Array.Empty<Move>());

            Random random = new();
            var toPlay = random.NextDouble() < 0.5 ? Team.Red : Team.Yellow;

            return new GameState(deck, redHand, yellowHand, board, moveHistory, toPlay);
        }

        public ClientGameState ToClientGameState(Team team)
        {
            Hand hand = team == Team.Red ? RedHand : YellowHand;
            Card[] cards = hand.GetCards();

            return new ClientGameState
            {
                Moves = MoveHistory.GetMoves(),
                Hand = cards,
                Team = team
            };
        }
    }
}