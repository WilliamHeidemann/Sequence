using System;
using System.Threading.Tasks;
using Game.Domain.Models.Dto;
using Unity.Services.CloudCode.Core;
using Unity.Services.CloudCodePush.Model;

namespace Cloud_Code_Module_Reference;

public class PushMessagesService(IPushClient pushClient)
{
    [CloudCodeFunction]
    public async Task<bool> ChallengeFriend(IExecutionContext context,
        string challengerName, string challengedPlayerId)
    {
        try
        {
            const PushMessageType messageType = PushMessageType.ChallengeRequest;

            SendMessageReply response =
                await pushClient.SendPlayerMessageAsync(context, challengerName, messageType.ToString(),
                    challengedPlayerId);

            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }

    [CloudCodeFunction]
    public async Task<bool> AcceptChallenge(IExecutionContext context, string matchId)
    {
        try
        {
            const PushMessageType messageType = PushMessageType.ChallengeAccepted;

            SendMessageReply response =
                await pushClient.SendPlayerMessageAsync(context, matchId, messageType.ToString(), matchId);

            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }

    [CloudCodeFunction]
    public async Task<bool> NotifyOpponentOfMove(IExecutionContext context, string matchId, string opponentPlayerId)
    {
        try
        {
            const PushMessageType messageType = PushMessageType.NewMove;

            SendMessageReply response =
                await pushClient.SendPlayerMessageAsync(context, matchId, messageType.ToString(), opponentPlayerId);

            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }
}