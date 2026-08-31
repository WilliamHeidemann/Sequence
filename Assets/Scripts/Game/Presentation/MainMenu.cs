using System;
using Unity.Services.Friends.Models;
using UnityEngine;
using UnityEngine.UIElements;

namespace Game.Presentation
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private UIDocument _mainMenuDocument;
        [SerializeField] private VisualTreeAsset _friendTemplate;
        [SerializeField] private VisualTreeAsset _requestTemplate;

        private Label _playerName;
        private TextField _changeNameInputField;
        private Button _savePlayerNameButton;
        private ScrollView _friendsList;
        private ScrollView _friendRequests;
        private Button _sendFriendRequestButton;
        private TextField _friendRequestInputField;
        private VisualElement _challengeRequestOverlay;

        public event Action<string> OnSetPlayerName;
        public event Action<string> OnSentFriendRequest;
        public event Action<string> OnSentGameRequest;

        private void OnEnable()
        {
            VisualElement root = _mainMenuDocument.rootVisualElement;
            _playerName = root.Q<Label>("CurrentPlayerName");
            _changeNameInputField = root.Q<TextField>("PlayerNameInput");
            _savePlayerNameButton = root.Q<Button>("SavePlayerNameButton");
            _friendsList = root.Q<ScrollView>("FriendsList");
            _friendRequests = root.Q<ScrollView>("IncomingRequestsList");
            _sendFriendRequestButton = root.Q<Button>("SendFriendRequestButton");
            _friendRequestInputField = root.Q<TextField>("FriendRequestInput");
            _challengeRequestOverlay = root.Q<VisualElement>("ChallengeModalOverlay");

            _savePlayerNameButton.clicked += OnSavePlayerNameClicked;
            _sendFriendRequestButton.clicked += OnSendFriendRequestClicked;
        }

        private void OnSendFriendRequestClicked()
        {
            string friendName = _friendRequestInputField.text;
            _friendRequestInputField.value = string.Empty;
            OnSentFriendRequest?.Invoke(friendName);
        }

        private void OnSavePlayerNameClicked()
        {
            var newName = _changeNameInputField.text;
            _changeNameInputField.value = string.Empty;
            SetPlayerName(newName);
            OnSetPlayerName?.Invoke(newName);
        }

        public void SetPlayerName(string newName)
        {
            _playerName.text = newName;
        }

        public void ShowFriend(Member friend)
        {
            TemplateContainer template = _friendTemplate.Instantiate();
            template.Q<Label>("FriendName").text = friend.Profile.Name;
            var button = template.Q<Button>("Play");
            button.clicked += () =>
            {
                button.text = "Play?";
                button.style.backgroundColor = Color.grey;
                OnSentGameRequest?.Invoke(friend.Id);
            };
            _friendsList.Add(template);
        }

        public void ShowFriendRequest(Member potentialFriend)
        {
            TemplateContainer template = _requestTemplate.Instantiate();
            string potentialFriendName = potentialFriend.Profile.Name;
            template.Q<Label>("PotentialFriendName").text = potentialFriendName;
            template.Q<Button>("Accept").clicked += () => OnSentFriendRequest?.Invoke(potentialFriendName);
            _friendRequests.Add(template);
        }

        public void HideFriendRequest()
        {
        }

        public void OpenGameRequestModal(string challengerName)
        {
            _challengeRequestOverlay.style.display = DisplayStyle.Flex;
            
            _challengeRequestOverlay.Q<Label>("ChallengeModalHeader").text =
                $"{challengerName} wants to play with you!";
            
            _challengeRequestOverlay.Q<Button>("AcceptChallengeButton").clicked +=
                () =>
                {
                    Debug.Log("Challenge Accepted!");
                    _challengeRequestOverlay.style.display = DisplayStyle.None;
                };
            
            _challengeRequestOverlay.Q<Button>("DeclineChallengeButton").clicked +=
                () =>
                {
                    Debug.Log("Challenge Declined!");
                    _challengeRequestOverlay.style.display = DisplayStyle.None;
                };
        }
    }
}