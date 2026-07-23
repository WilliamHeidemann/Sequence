using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.Services.CloudCode.Apis;
using Unity.Services.CloudCode.Core;
using Unity.Services.CloudCode.Shared;
using Unity.Services.CloudSave.Model;

namespace Cloud_Code_Module_Reference;

public class ExampleService(IGameApiClient gameApiClient)
{
    private const string CustomId = "ExampleItemId";

    [CloudCodeFunction("SetExampleData")]
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

    [CloudCodeFunction("GetExampleData")]
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
}