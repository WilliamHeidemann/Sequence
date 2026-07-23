using System.Threading.Tasks;
using Game.Models.Players;
using Unity.Services.CloudCode.Core;

namespace Cloud_Code_Module_Reference;

public class MultiplayerService
{
    [CloudCodeFunction("GetGameStateData")]
    public GameStateData GetGameStateData()
    {
        return new GameStateData();
    }
}