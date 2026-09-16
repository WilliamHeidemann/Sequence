using Game.Domain.Models;
using Game.Domain.Models.Dto;
using Unity.Services.CloudCode.Apis;
using Unity.Services.CloudCode.Core;

namespace Cloud_Code_Module_Reference;

public class GameLogicService(IGameApiClient gameApiClient)
{
    [CloudCodeFunction]
    public CardResultDto Request(Move move)
    {
        GameState gameState = GameState.CreateInitial();
        
        Deck deck = new(gameState.Deck);
        
        Card card = deck.Draw();
        
        return new CardResultDto
        {
            Card = card,
            HasCard = true,
        };
    }
}