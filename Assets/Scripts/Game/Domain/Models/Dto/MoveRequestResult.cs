using System;

namespace Game.Domain.Models.Dto
{
    [Serializable]
    public class MoveRequestResult
    {
        public bool HasCard { get; set; }
        public bool IsOutOfSync { get; set; }
        public Card Card { get; set; }
    }
}