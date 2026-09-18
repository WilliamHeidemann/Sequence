using System;
using System.Net;
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
    public async Task<Match> CreateMatch(IExecutionContext context)
    {
        string matchId = Guid.NewGuid().ToString();

        GameState gameState = GameState.CreateInitial();

        ApiResponse<SetItemResponse> response = await gameApiClient.CloudSaveData.SetPrivateCustomItemAsync(
            context,
            context.ServiceToken,
            context.ProjectId,
            matchId,
            new SetItemBody("gameState", gameState));

        ClientGameState clientGameState = gameState.ToClientGameState(gameState.ToPlay);

        return new Match
        {
            Id = matchId,
            ClientGameState = clientGameState
        };
    }

    [CloudCodeFunction]
    public async Task<CardResult> Request(IExecutionContext context, Move move, string matchId) =>
        MoveValidator.PlayMove(await GetGameState(context, matchId), move) switch
        {
            MoveValidator.MoveResult.Success(var updatedGameState) => new CardResult
            {
                Card = new Deck(updatedGameState.Deck).Draw(),
                HasCard = true,
            },
            MoveValidator.MoveResult.Invalid => new CardResult
            {
                HasCard = false,
            }
        };

    [CloudCodeFunction]
    public async Task<ClientGameState> GetClientGameState(IExecutionContext context, string matchId, Team team)
    {
        GameState gameState = await GetGameState(context, matchId);
        return gameState.ToClientGameState(team);
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

    [CloudCodeFunction]
    public async Task<bool> DeleteMatch(IExecutionContext context, string matchId)
    {
        var result = await gameApiClient.CloudSaveData.DeletePrivateCustomItemsAsync(context,
            context.ServiceToken, context.ProjectId, matchId);

        return result.StatusCode == HttpStatusCode.OK;
    }
}