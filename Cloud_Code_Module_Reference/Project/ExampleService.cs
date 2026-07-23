using System.Threading.Tasks;
using Unity.Services.CloudCode.Apis;
using Unity.Services.CloudCode.Core;
using Unity.Services.CloudSave.Model;

namespace Cloud_Code_Module_Reference;

public class ExampleService(IGameApiClient gameApiClient)
{
    [CloudCodeFunction("SetExampleData")]
    public async Task SetExampleData(IExecutionContext context)
    {
        const string customId = "ExampleItemId";
        
        SetItemBody setItemBody = new("OtherKey", 2);
        
        await gameApiClient.CloudSaveData.SetCustomItemAsync(context, context.AccessToken, context.ProjectId, customId,
            setItemBody);
    }
}