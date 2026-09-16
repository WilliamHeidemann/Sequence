using System;
using System.Linq;
using System.Threading.Tasks;
using Game.Presentation;
using Unity.Services.Authentication;
using Unity.Services.CloudCode;
using Unity.Services.CloudCode.GeneratedBindings;
using Unity.Services.CloudCode.Subscriptions;
using Unity.Services.Core;
using Unity.Services.Friends;
using Unity.Services.Friends.Models;
using Unity.Services.Friends.Notifications;
using UnityEngine;

public class CloudHandler : MonoBehaviour
{
    [SerializeField] private MainMenu _mainMenu;

    private async void Awake()
    {
        try
        {
            await Initialize();
            
            FriendsService.Instance.RelationshipAdded += OnRelationShipAdded;
            FriendsService.Instance.RelationshipDeleted += OnRelationShipDeleted;
            
            _mainMenu.SetPlayerName(AuthenticationService.Instance.PlayerName);
            FriendsService.Instance.Friends.Select(r => r.Member).ToList()
                .ForEach(_mainMenu.ShowFriend);
            FriendsService.Instance.IncomingFriendRequests.Select(r => r.Member).ToList()
                .ForEach(_mainMenu.ShowFriendRequest);
            
            _mainMenu.OnSetPlayerName += async n => await AuthenticationService.Instance.UpdatePlayerNameAsync(n);
            _mainMenu.OnSentFriendRequest += async n => await FriendsService.Instance.AddFriendByNameAsync(n);
            _mainMenu.OnSentGameRequest += async n => await SendGameRequest(AuthenticationService.Instance.PlayerName, n);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }
    
    private async Task Initialize()
    {
        await UnityServices.InitializeAsync();
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
        await FriendsService.Instance.InitializeAsync();
        await CloudCodeService.Instance.SubscribeToPlayerMessagesAsync(CreateSubscriptionEventCallbacks());
    }

    private SubscriptionEventCallbacks CreateSubscriptionEventCallbacks()
    {
        SubscriptionEventCallbacks callbacks = new();
        
        // The subscribed callback should handle all push message types.
        // 1) GameRequest(playerId)
        // 2) OpponentPlayedMessage(matchId)
        callbacks.MessageReceived += evt => _mainMenu.OpenGameRequestModal(evt.Message);
        
        callbacks.Error += Debug.LogError;
        
        return callbacks;
    }

    // Send a push message to the friend that opens their game request overlay.
    // The friend accepting the game will start the match.
    private async Task SendGameRequest(string challengerName, string receiverPlayerId)
    {
        PushMessagesServiceBindings pushMessagesServiceModule = new();
        string response = await pushMessagesServiceModule
            .ChallengeFriend(challengerName, "GameRequest", receiverPlayerId);
        Debug.Log(response);
    }

    private void OnRelationShipDeleted(IRelationshipDeletedEvent relationshipDeletedEvent)
    {
        switch (relationshipDeletedEvent.Relationship.Type)
        {
            case RelationshipType.Friend:
                throw new NotImplementedException();
                break;
            case RelationshipType.FriendRequest:
                _mainMenu.HideFriendRequest();
                break;
            case RelationshipType.Block:
                throw new NotImplementedException();
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void OnRelationShipAdded(IRelationshipAddedEvent relationshipAddedEvent)
    {
        switch (relationshipAddedEvent.Relationship.Type)
        {
            case RelationshipType.Friend:
                _mainMenu.ShowFriend(relationshipAddedEvent.Relationship.Member);
                break;
            case RelationshipType.FriendRequest:
                _mainMenu.ShowFriendRequest(relationshipAddedEvent.Relationship.Member);
                break;
            case RelationshipType.Block:
                throw new NotImplementedException();
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}