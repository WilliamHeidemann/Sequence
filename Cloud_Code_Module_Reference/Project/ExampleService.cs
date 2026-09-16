using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Game.Domain.Models;
using Newtonsoft.Json;
using Unity.Services.CloudCode.Apis;
using Unity.Services.CloudCode.Core;
using Unity.Services.CloudCode.Shared;
using Unity.Services.CloudSave.Model;
using JsonException = System.Text.Json.JsonException;

namespace Cloud_Code_Module_Reference;

public class ExampleService(IGameApiClient gameApiClient)
{
    private const string CustomId = "ExampleItemId";

    [CloudCodeFunction]
    public async Task SetExampleData(IExecutionContext context)
    {
        SetItemBody setItemBody = new("OtherKey", 2);

        await gameApiClient.CloudSaveData.SetCustomItemAsync(
            context,
            context.ServiceToken,
            context.ProjectId,
            CustomId,
            setItemBody);
    }

    [CloudCodeFunction]
    public async Task<IEnumerable<string>> GetExampleData(IExecutionContext context)
    {
        var customIdsResponse =
            await gameApiClient.CloudSaveData.GetCustomIDsAsync(context, context.ServiceToken, context.ProjectId);
        var customIds = customIdsResponse.Data.Results.Select(result => result.Id);

        var keysResponse = await gameApiClient.CloudSaveData
            .GetCustomKeysAsync(context, context.ServiceToken, context.ProjectId, CustomId);
        var keys = keysResponse.Data.Results.Select(result => result.Key);

        var valuesResponse = await gameApiClient.CloudSaveData
            .GetCustomItemsAsync(context, context.ServiceToken, context.ProjectId, CustomId);
        var values = valuesResponse.Data.Results.Select(result => result.ToJson());

        return customIds.Concat(keys).Concat(values);
    }

    [CloudCodeFunction]
    public async Task<string> CreateMatch1(IExecutionContext context)
    {
        try
        {
            ApiResponse<SetItemResponse> response = await gameApiClient.CloudSaveData.SetPrivateCustomItemAsync(
                context,
                context.ServiceToken,
                context.ProjectId,
                "match001",
                new SetItemBody("gameStateData", Card.AceOfMoon));

            return response.Data.ToJson();
        }
        catch (Exception e)
        {
            return $"En exception occured: {e.Message}";
        }
    }

    [CloudCodeFunction]
    public async Task<string> StoreGameState(IExecutionContext context)
    {
        try
        {
            GameState gameState = GameState.CreateInitial();
            ApiResponse<SetItemResponse> response = await gameApiClient.CloudSaveData.SetPrivateCustomItemAsync(
                context,
                context.ServiceToken,
                context.ProjectId,
                "match002",
                new SetItemBody("gameState", gameState));

            return response.Data.ToJson();
        }
        catch (Exception e)
        {
            return $"En exception occured: {e.Message}";
        }
    }

    [CloudCodeFunction]
    public async Task<Card> DrawCard(IExecutionContext context)
    {
        ApiResponse<GetItemsResponse> response = await gameApiClient.CloudSaveData.GetPrivateCustomItemsAsync(
            context,
            context.ServiceToken,
            context.ProjectId,
            "match002",
            ["gameState"]);

        var item = response.Data.Results.First();
        string rawJson = JsonConvert.SerializeObject(item.Value);
        GameState gameState = JsonConvert.DeserializeObject<GameState>(rawJson)
                              ?? throw new JsonException("Could not deserialize game state");
        
        Deck deck = new(gameState.Deck);

        return deck.Draw();
    }
}