using System;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.CloudCode.GeneratedBindings;
using Unity.Services.Core;
using Unity.Services.Friends;
using Unity.Services.Friends.Exceptions;
using Unity.Services.Friends.Notifications;
using Unity.Services.Friends.Options;
using UnityEngine;
using UtilityToolkit.Editor;

namespace Game.Cloud
{
    public class CloudCodeExperiments : MonoBehaviour
    {
        [SerializeField] private string _friend;

        private async void Start()
        {
            InitializationOptions initializationOptions = new InitializationOptions();
            // initializationOptions.SetProfile("player-1");
            await UnityServices.InitializeAsync();

            await AuthenticationService.Instance.SignInAnonymouslyAsync();

            string playerName = await AuthenticationService.Instance.GetPlayerNameAsync(true);

            Debug.Log($"PlayerName: {playerName == null} {playerName}");

            // PlayerInfo playerInfo = await AuthenticationService.Instance.GetPlayerInfoAsync();

            // var username = playerInfo.Username;

            // Debug.Log($"Username: {username == null} {username}");

            await FriendsService.Instance.InitializeAsync();

            FriendsService.Instance.RelationshipAdded += OnRelationShipAdded;

            ListRelationShips();
            
            // await FriendsService.Instance.

            // ExampleServiceBindings module = new();

            // var content = await module.CreateMatch();

            // Debug.Log(content);

            // var cardDto = await module.SendCard(Card.AceOfMoon.ToDto());
            //
            // Card card = cardDto.ToModel();
            //
            // Debug.Log($"Received card: {card}");
        }

        private void OnRelationShipAdded(IRelationshipAddedEvent relationshipAddedEvent)
        {
            Debug.Log(
                $"Received relationship type: {relationshipAddedEvent.Relationship.Type} " +
                $"from {relationshipAddedEvent.Relationship.Member.Profile.Name} " +
                $"with ID {relationshipAddedEvent.Relationship.Member.Id}");
            _ = AddFriendAsync(relationshipAddedEvent.Relationship.Member.Profile.Name);
        }

        public void AddFriend()
        {
            _ = AddFriendAsync(_friend);
        }

        public async Task AddFriendAsync(string friendName)
        {
            try
            {
                Debug.Log($"Attempting to add {friendName}");
                await FriendsService.Instance.AddFriendByNameAsync(friendName);
                Debug.Log($"Successfully sent friend request to {friendName}");
            }
            catch (FriendsServiceException e)
            {
                // Handle 409 Conflict specifically
                if (e.ErrorCode == FriendsErrorCode.RelationshipAlreadyExists || e.StatusCode == System.Net.HttpStatusCode.Conflict)
                {
                    Debug.LogWarning($"Cannot add {friendName}: Request already pending, already friends, or user is blocked.");
                    // Example: Show dynamic UI feedback like "Request already sent!"
                }
                else
                {
                    Debug.LogError($"Friends Service Error ({e.ErrorCode}): {e.Message}");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Unexpected error: {e}");
            }
        }

        public void ListRelationShips()
        {
            foreach (var r in FriendsService.Instance.Relationships)
            {
                Debug.Log($"relationship: Type | {r.Type} \t Name | {r.Member.Profile.Name}");
            }
        }
    }
}