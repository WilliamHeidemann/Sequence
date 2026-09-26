using System;
using System.Threading.Tasks;
using Game.Domain.Models.Dto;
using Newtonsoft.Json;
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

            if (context.PlayerId == null)
            {
                throw new NullReferenceException("PlayerId is null");
            }
            
            User user = new()
            {
                Name = challengerName,
                Id = context.PlayerId
            };
            
            string jsonUser = JsonConvert.SerializeObject(user);
            
            SendMessageReply response =
                await pushClient.SendPlayerMessageAsync(context, jsonUser, messageType.ToString(),
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
    public async Task<bool> AcceptChallenge(IExecutionContext context, string matchId, string challengerId)
    {
        try
        {
            const PushMessageType messageType = PushMessageType.ChallengeAccepted;

            if (context.PlayerId == null)
            {
                throw new NullReferenceException("PlayerId is null");
            }
            
            MatchAccepted matchAccepted = new()
            {
                MatchID = matchId,
                OpponentID = context.PlayerId
            };
            
            string jsonMatchAccepted = JsonConvert.SerializeObject(matchAccepted);
            
            SendMessageReply response =
                await pushClient.SendPlayerMessageAsync(context, jsonMatchAccepted, messageType.ToString(), challengerId);

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