using System;

namespace Game.Domain.Models.Dto
{
    [Serializable]
    public class Match
    {
        public string Id { get; set; }
        public ClientGameState ClientGameState { get; set; }
    }
}