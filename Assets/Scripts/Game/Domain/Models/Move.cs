namespace Game.Domain.Models
{
    public class Move
    {
        public Position Position { get; set; }
        public Card Card { get; set; }
        public Team Team { get; set; }
    }
}