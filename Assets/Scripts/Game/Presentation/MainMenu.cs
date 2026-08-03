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
        private VisualElement _friendsList;
        private VisualElement _friendRequests;
        private Button _sendFriendRequestButton;
        private TextField _friendRequestInputField;
        private VisualElement _challengeRequestOverlay;

        public event Action<string> OnSetPlayerName;
        public event Action<string> OnSentFriendRequest;
        
        private void OnEnable()
        {
            VisualElement root = _mainMenuDocument.rootVisualElement;
            _playerName = root.Q<Label>("CurrentPlayerName");
            _changeNameInputField = root.Q<TextField>("PlayerNameInput");
            _savePlayerNameButton = root.Q<Button>("SavePlayerNameButton");
            _friendsList = root.Q<VisualElement>("FriendsList");
            _friendRequests = root.Q<VisualElement>("RequestsPanel");
            _sendFriendRequestButton = root.Q<Button>("SendFriendRequestButton");
            _friendRequestInputField = root.Q<TextField>("FriendRequestInput");
            _challengeRequestOverlay = root.Q<VisualElement>("ChallengeModalOverlay");

            _savePlayerNameButton.clicked += OnSavePlayerNameClicked;
            _sendFriendRequestButton.clicked += OnSendFriendRequestClicked;
            
            ShowFriend(null);
        }

        private void OnSendFriendRequestClicked()
        {
            string friendName = _friendRequestInputField.text;
            _friendRequestInputField.value = string.Empty;
            OnSentFriendRequest?.Invoke(friendName);
        }

        public void OnSavePlayerNameClicked()
        {
            var newName = _changeNameInputField.text;
            _changeNameInputField.value = string.Empty;
            _playerName.text = newName;
            OnSetPlayerName?.Invoke(newName);
        }
        
        public void ShowChallengePopup()
        {
            _challengeRequestOverlay.style.display = DisplayStyle.Flex;
        }

        public void HideChallengePopup()
        {
            _challengeRequestOverlay.style.display = DisplayStyle.None;
        }

        public void ShowFriend(Member friend)
        {
            TemplateContainer instance = _friendTemplate.Instantiate();
            _friendsList.Add(instance);
        }

        public void ShowFriendRequest(Member potentialFriend)
        {
        }

        public void HideFriendRequest()
        {
            
        }
        
        public void ChallengeFriend()
        {
            
        }

        public void AcceptChallenge()
        {
            
        }

        public void DeclineChallenge()
        {
            
        }
        
        public void AcceptFriend()
        {
            
        }

        public void DeclineFriend()
        {
            
        }

        public void SendFriendRequest()
        {
            
        }
    }
}