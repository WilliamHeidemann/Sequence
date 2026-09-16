using System;
using System.Threading.Tasks;
using Game.Domain.Models;
using Game.Domain.Models.Dto;
using Newtonsoft.Json;
using Unity.Services.CloudCode.Apis;
using Unity.Services.CloudCode.Core;
using Unity.Services.CloudCode.Shared;
using Unity.Services.CloudSave.Model;

namespace Cloud_Code_Module_Reference;

public class GameLogicService(IGameApiClient gameApiClient)
{
    [CloudCodeFunction]
    public async Task<string> CreateMatch(IExecutionContext context)
    {
        string matchId = Guid.NewGuid().ToString();

        GameState gameState = GameState.CreateInitial();
        
        ApiResponse<SetItemResponse> response = await gameApiClient.CloudSaveData.SetPrivateCustomItemAsync(
            context,
            context.ServiceToken,
            context.ProjectId,
            matchId,
            new SetItemBody("gameState", gameState));

        return matchId;
    }
    
    [CloudCodeFunction]
    public async Task<CardResultDto> Request(IExecutionContext context, Move move, string matchId) =>
        MoveValidator.PlayMove(await GetGameState(context, matchId), move) switch
        {
            MoveValidator.MoveResult.Success(var updatedGameState) => new CardResultDto
            {
                Card = new Deck(updatedGameState.Deck).Draw(),
                HasCard = true,
            },
            MoveValidator.MoveResult.Invalid => new CardResultDto
            {
                HasCard = false,
            }
        };

    [CloudCodeFunction]
    public async Task<ClientGameState> GetClientGameState(IExecutionContext context, string matchId)
    {
        GameState gameState = await GetGameState(context, matchId);
        return gameState.ToClientGameState(Team.Red);
    }
    
    private async Task<GameState> GetGameState(IExecutionContext context, string matchId)
    {
        ApiResponse<GetItemsResponse> response = await gameApiClient.CloudSaveData.GetPrivateCustomItemsAsync(
            context,
            context.ServiceToken,
            context.ProjectId,
            matchId,
            ["gameState"]);

        Item item = response.Data.Results[0];
        string rawJson = JsonConvert.SerializeObject(item.Value);
        GameState gameState = JsonConvert.DeserializeObject<GameState>(rawJson)
                              ?? throw new JsonException("Could not deserialize game state");

        return gameState;
    }
}