using System;
using System.Linq;
using System.Threading.Tasks;
using Game.Presentation;
using Unity.Services.Authentication;
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
            
            foreach (Member friend in FriendsService.Instance.Friends.Select(f => f.Member))
            {
                _mainMenu.ShowFriend(friend);
            }
            
            _mainMenu.OnSetPlayerName += async n => await AuthenticationService.Instance.UpdatePlayerNameAsync(n);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    private void Start()
    {
        try
        {
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            throw;
        }
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

    private async Task Initialize()
    {
        await UnityServices.InitializeAsync();
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
        await FriendsService.Instance.InitializeAsync();
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