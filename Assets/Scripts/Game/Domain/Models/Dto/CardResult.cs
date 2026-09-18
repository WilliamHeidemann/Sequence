using System;

namespace Game.Domain.Models.Dto
{
    [Serializable]
    public class CardResult
    {
        public bool HasCard { get; set; }
        public Card Card { get; set; }
    }
}