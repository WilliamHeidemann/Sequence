using System.Threading.Tasks;
using Game.Models.Players;
using Microsoft.Extensions.Logging;
using Unity.Services.CloudCode.Apis;
using Unity.Services.CloudCode.Core;

namespace Cloud_Code_Module_Reference;

public class MultiplayerService(IGameApiClient gameApiClient, ILogger<MyModule> logger)
{
    [CloudCodeFunction("GetGameStateData")]
    public GameStateData GetGameStateData()
    {
        return new GameStateData();
    }
}