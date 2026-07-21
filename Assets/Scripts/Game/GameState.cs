using Game.Models;
using Game.Models.Players;

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

        public GameState(Team team)
        {
            MyTeam = team;
            
            Deck = new Deck();
        
            MyHand = new Hand();
            while (!MyHand.IsFull)
            {
                MyHand.TryAdd(Deck.Draw());
            }
        
            OpponentHand = new Hand();
            while (!OpponentHand.IsFull)
            {
                OpponentHand.TryAdd(Deck.Draw());
            }
        
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