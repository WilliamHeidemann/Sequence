using System;
using System.Threading.Tasks;
using Unity.Services.CloudCode.Core;
using Unity.Services.CloudCodePush.Model;

namespace Cloud_Code_Module_Reference;

public class PushMessagesService(IPushClient pushClient)
{
    [CloudCodeFunction("ChallengeFriend")]
    public async Task<string> ChallengeFriend(IExecutionContext context,
        string challengerName, string challengedPlayerId)
    {
        const string messageType = "GameRequest";
        
        SendMessageReply response =
            await pushClient.SendPlayerMessageAsync(context, challengerName, messageType, challengedPlayerId);

        return "Message sent";
    }

    public async Task<bool> NotifyOpponentOfMove(IExecutionContext context, string matchId, string opponentPlayerId)
    {
        try
        {
            const string messageType = "MoveNotification";
            
            SendMessageReply response =
                await pushClient.SendPlayerMessageAsync(context, message, messageType, opponentPlayerId);
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }
}