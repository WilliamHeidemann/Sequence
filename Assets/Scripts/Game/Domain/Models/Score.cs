using System;

namespace Game.Domain.Models
{
    public class Score
    {
        public int Red;
        public int Yellow;

        public int Get(Team team) =>
            team switch
            {
                Team.Red => Red,
                Team.Yellow => Yellow,
                _ => throw new ArgumentOutOfRangeException(nameof(team), team, null)
            };
    }
}