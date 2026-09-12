using Game.Domain.Models;
using Game.Domain.Models.Dto;
using Unity.Services.CloudCode.Core;

namespace Cloud_Code_Module_Reference;

public class GameLogicService
{
    [CloudCodeFunction]
    public CardResultDto Request(Move move)
    {
        GameState gameState = GameState.Create();
        Card card = gameState.Deck.Draw();
        return new CardResultDto
        {
            Card = card,
            HasCard = true,
        };
    }
}