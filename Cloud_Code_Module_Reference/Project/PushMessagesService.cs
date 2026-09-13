using System;
using System.Threading.Tasks;
using Game.Domain.Models;
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

    public static MoveResult Apply(GameState gameState, Move move)
    {
        throw new System.NotImplementedException();
    }

    // Usage:
    public abstract record MoveResult
    {
        public record Success(GameState NewState) : MoveResult;
        public record Invalid(string Reason) : MoveResult;
    }
    
    void Test()
    {
        GameState currentGameState = GameState.Create();
        Move intendedMove = new Move();

        // Apply the move and process the result in one switch expression
        currentGameState = Apply(currentGameState, intendedMove) switch
        {
            // MoveResult.Success success => success.NewState,
            MoveResult.Success(var newState) => newState,
            MoveResult.Invalid invalid => currentGameState,
            _ => throw new ArgumentOutOfRangeException()
        };

        static GameState HandleInvalidMove(GameState state, string reason)
        {
            Console.WriteLine($"Cannot make move: {reason}");
            return state; // Keep current state unchanged
        }
    }
}