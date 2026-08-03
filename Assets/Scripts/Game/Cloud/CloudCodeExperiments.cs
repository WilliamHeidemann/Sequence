using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Friends;
using Unity.Services.Friends.Exceptions;
using Unity.Services.Friends.Notifications;
using UnityEngine;
using UnityEngine.UIElements;

namespace Game.Cloud
{
    public class CloudCodeExperiments : MonoBehaviour
    {
        [SerializeField] private string _friend;
        [SerializeField] private UIDocument _mainMenuDocument;

        private VisualElement _challengeModalOverlay;
        private Label _challengerNameLabel;
        private Button _acceptChallengeButton;
        private Button _declineChallengeButton;

        private string _pendingChallengerName;

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
            InitializeChallengeModal();

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

        private void OnDestroy()
        {
            if (UnityServices.State == ServicesInitializationState.Initialized)
            {
                FriendsService.Instance.RelationshipAdded -= OnRelationShipAdded;
            }

            if (_acceptChallengeButton != null)
            {
                _acceptChallengeButton.clicked -= OnAcceptChallengeClicked;
            }

            if (_declineChallengeButton != null)
            {
                _declineChallengeButton.clicked -= OnDeclineChallengeClicked;
            }
        }

        private void InitializeChallengeModal()
        {
            UIDocument mainMenuDocument = _mainMenuDocument != null ? _mainMenuDocument : GetComponent<UIDocument>();
            if (mainMenuDocument == null)
            {
                return;
            }

            VisualElement root = mainMenuDocument.rootVisualElement;
            _challengeModalOverlay = root.Q<VisualElement>("ChallengeModalOverlay");
            _challengerNameLabel = root.Q<Label>("ChallengeChallengerNameLabel");
            _acceptChallengeButton = root.Q<Button>("AcceptChallengeButton");
            _declineChallengeButton = root.Q<Button>("DeclineChallengeButton");

            if (_challengeModalOverlay == null || _challengerNameLabel == null || _acceptChallengeButton == null || _declineChallengeButton == null)
            {
                Debug.LogWarning("Challenge modal UI is missing required elements.");
                return;
            }

            _acceptChallengeButton.clicked += OnAcceptChallengeClicked;
            _declineChallengeButton.clicked += OnDeclineChallengeClicked;
            HideChallengeModal();
        }

        private void OnRelationShipAdded(IRelationshipAddedEvent relationshipAddedEvent)
        {
            OnPlayerChallenged(relationshipAddedEvent.Relationship.Member.Profile.Name);
            Debug.Log(
                $"Received relationship type: {relationshipAddedEvent.Relationship.Type} " +
                $"from {relationshipAddedEvent.Relationship.Member.Profile.Name} " +
                $"with ID {relationshipAddedEvent.Relationship.Member.Id}");
            _ = AddFriendAsync(relationshipAddedEvent.Relationship.Member.Profile.Name);
        }

        public void OnPlayerChallenged(string challengerName)
        {
            _pendingChallengerName = challengerName;

            if (_challengeModalOverlay == null || _challengerNameLabel == null)
            {
                return;
            }

            _challengerNameLabel.text = $"{challengerName} challenged you to a match.";
            _challengeModalOverlay.style.display = DisplayStyle.Flex;
        }

        private void OnAcceptChallengeClicked()
        {
            if (string.IsNullOrWhiteSpace(_pendingChallengerName))
            {
                Debug.LogWarning("Accepted challenge with no challenger name.");
                return;
            }

            Debug.Log($"Accepted challenge from {_pendingChallengerName}");
            HideChallengeModal();
        }

        private void OnDeclineChallengeClicked()
        {
            if (string.IsNullOrWhiteSpace(_pendingChallengerName))
            {
                Debug.LogWarning("Declined challenge with no challenger name.");
                return;
            }

            Debug.Log($"Declined challenge from {_pendingChallengerName}");
            HideChallengeModal();
        }

        private void HideChallengeModal()
        {
            _pendingChallengerName = null;
            if (_challengeModalOverlay != null)
            {
                _challengeModalOverlay.style.display = DisplayStyle.None;
            }
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