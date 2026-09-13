using System.Threading.Tasks;
using Unity.Services.CloudCode.Core;
using Unity.Services.CloudCodePush.Model;

namespace Cloud_Code_Module_Reference;

public class PushMessagesService(IPushClient pushClient)
{
    [CloudCodeFunction("ChallengeFriend")]
    public async Task<string> ChallengeFriend(IExecutionContext context, 
        string message, string messageType, string playerId)
    {
        SendMessageReply response = 
            await pushClient.SendPlayerMessageAsync(context, message, messageType, playerId);

        return "Message sent";
    }
}