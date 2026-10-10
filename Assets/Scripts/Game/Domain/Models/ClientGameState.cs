using System;

namespace Game.Domain.Models
{
    public record ClientGameState(
        Move[] Moves,
        Card[] Hand,
        Position[] Locked,
        Team Team,
        Score Score,
        bool IsMyTurn
    );
}