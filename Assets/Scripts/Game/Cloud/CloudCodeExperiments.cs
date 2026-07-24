using Unity.Services.Authentication;
using Unity.Services.CloudCode.GeneratedBindings;
using Unity.Services.Core;
using UnityEngine;
using Card = Game.Domain.Models.Card;

namespace Game.Cloud
{
    public class CloudCodeExperiments : MonoBehaviour
    {
        private async void Start()
        {
            await UnityServices.InitializeAsync();
            
            await AuthenticationService.Instance.SignInAnonymouslyAsync();

            ExampleServiceBindings module = new();

            var cardDto = await module.SendCard(Card.AceOfMoon.ToDto());
            
            Card card = cardDto.ToModel();
            
            Debug.Log($"Received card: {card}");
        }
    }
}