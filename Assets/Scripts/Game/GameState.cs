using Game.Models;

namespace Game
{
    public class GameState
    {
        public Team MyTeam { get; }
        public Hand MyHand { get; }
        public Hand OpponentHand { get; }
        public Deck Deck { get; }
        public Board Board { get; }
        public MoveHistory MoveHistory { get; }

        public GameState()
        {
            Deck = new Deck();
        
            MyHand = new Hand();
            while (MyHand.TryAdd(Deck.Draw(out Card card))) { }
        
            OpponentHand = new Hand();
            while (OpponentHand.TryAdd(Deck.Draw(out Card card))) { }
        
            Board = new Board();
        
            MoveHistory = new MoveHistory();
        }

        public GameStateData ToData()
        {
            var redHand = MyTeam == Team.Red ? MyHand.GetCards() : OpponentHand.GetCards();
            var yellowHand = MyTeam == Team.Yellow ? MyHand.GetCards() : OpponentHand.GetCards();

            return new GameStateData
            {
                Moves = MoveHistory.GetMoves(),
                Deck = Deck.GetCards(),
                RedHand = redHand,
                YellowHand = yellowHand,
            };
        }
    }
}