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
    [Serializable]
    private record Teams(string Red, string Yellow);
    
    [CloudCodeFunction]
    public async Task<Match> CreateMatch(IExecutionContext context, string opponentId)
    {
        string matchId = Guid.NewGuid().ToString();

        if (context.PlayerId == null) throw new NullReferenceException("context.PlayerId is null");
        
        Teams teams = AssignTeams(context.PlayerId, opponentId);
        ApiResponse<SetItemResponse> setTeamsResponse = await SetTeams(context, matchId, teams);
        
        GameState gameState = GameState.CreateInitial();
        ApiResponse<SetItemResponse> setMatchResponse = await SetMatch(context, matchId, gameState);
        
        ClientGameState clientGameState = gameState.ToClientGameState(gameState.ToPlay);

        return new Match
        {
            Id = matchId,
            ClientGameState = clientGameState
        };
    }

    private Teams AssignTeams(string player1, string player2)
    {
        return Random.Shared.NextDouble() < 0.5 
            ? new Teams(player1, player2) 
            : new Teams(player2, player1);
    }

    private async Task<ApiResponse<SetItemResponse>> SetTeams(IExecutionContext context, string matchId, Teams teams)
    {
        SetItemBody setGameData = new("teams", teams);

        return await gameApiClient.CloudSaveData.SetPrivateCustomItemAsync(
            context,
            context.ServiceToken,
            context.ProjectId,
            matchId,
            setGameData);
    }

    private async Task<ApiResponse<SetItemResponse>> SetMatch(IExecutionContext context, string matchId,
        GameState gameState)
    {
        SetItemBody setGameData = new("gameState", gameState);

        return await gameApiClient.CloudSaveData.SetPrivateCustomItemAsync(
            context,
            context.ServiceToken,
            context.ProjectId,
            matchId,
            setGameData);
    }

    [CloudCodeFunction]
    public async Task<MoveRequestResult> Request(IExecutionContext context, Move move, string matchId)
    {
        GameState currentGameState = await GetGameState(context, matchId);

        MoveRequestResult moveRequestResult = MoveValidator.PlayMove(currentGameState, move) switch
        {
            MoveValidator.MoveResult.Success(var nextGameState, var drawnCard) => await SuccessMoveRequestResult(context,
                matchId, nextGameState, drawnCard),
            MoveValidator.MoveResult.Invalid => new MoveRequestResult { HasCard = false, },
            MoveValidator.MoveResult.OutOfSync => new MoveRequestResult { HasCard = false, IsOutOfSync = true },
            _ => throw new ArgumentOutOfRangeException()
        };

        return moveRequestResult;
    }

    private async Task<MoveRequestResult> SuccessMoveRequestResult(IExecutionContext context, string matchId,
        GameState next, Card drawnCard)
    {
        ApiResponse<SetItemResponse> response = await SetMatch(context, matchId, next);

        if (response.StatusCode != HttpStatusCode.OK)
        {
            // maybe start returning an enum instead? Or an algebraic data type so the card can be return?
            return new MoveRequestResult { HasCard = false, IsOutOfSync = true };
        }

        return new MoveRequestResult { Card = drawnCard, HasCard = true };
    }


    [CloudCodeFunction]
    public async Task<ClientGameState> GetClientGameState(IExecutionContext context, string matchId)
    {
        GameState gameState = await GetGameState(context, matchId);
        Team team = await GetMyTeam(context, matchId);
        return gameState.ToClientGameState(team);
    }

    [CloudCodeFunction]
    public async Task<Team> GetMyTeam(IExecutionContext context, string matchId)
    {
        ApiResponse<GetItemsResponse> response = await gameApiClient.CloudSaveData.GetPrivateCustomItemsAsync(
            context,
            context.ServiceToken,
            context.ProjectId,
            matchId,
            ["teams"]);
        
        Item item = response.Data.Results[0];
        string rawJson = JsonConvert.SerializeObject(item.Value);
        Teams teams = JsonConvert.DeserializeObject<Teams>(rawJson)
                              ?? throw new JsonException("Could not deserialize teams.");

        if (context.PlayerId == teams.Red) return Team.Red;
        if (context.PlayerId == teams.Yellow) return Team.Yellow;
        throw new InvalidOperationException("Player is not on any team.");
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
                              ?? throw new JsonException("Could not deserialize game state.");

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