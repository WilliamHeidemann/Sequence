using System;
using Unity.Services.Authentication;
using Unity.Services.CloudCode;
using Unity.Services.CloudCode.GeneratedBindings;
using Unity.Services.CloudCode.GeneratedBindings.Game.Models.Players;
using Unity.Services.Core;
using UnityEngine;

namespace Game.Cloud
{
    public class CloudCodeExperiments : MonoBehaviour
    {
        private MyModuleBindings _module;
        private MultiplayerServiceBindings _multiplayerService;

        private async void Start()
        {
            await UnityServices.InitializeAsync();
            
            await AuthenticationService.Instance.SignInAnonymouslyAsync();

            ExampleServiceBindings module = new();
            await module.SetExampleData();
            Debug.Log("Other key set to 2 successfully!");
        }

        // private async void Start()
        // {
        //     await UnityServices.InitializeAsync();
        //
        //     await AuthenticationService.Instance.SignInAnonymouslyAsync();
        //
        //     _module = new(CloudCodeService.Instance);
        //     _multiplayerService = new(CloudCodeService.Instance);
        //     
        //     try
        //     {
        //         await SayHello();
        //     }
        //     catch (CloudCodeException e)
        //     {
        //         Debug.Log(e);
        //     }
        //     catch (Exception e)
        //     {
        //         Debug.Log(e);
        //     }
        // }
        //
        // public async Awaitable SayHello()
        // {
        //     string hello = await _module.SayHello("Will");
        //     Debug.Log(hello);
        // }
        //
        // public async Awaitable<Models.Players.GameStateData> GetGameStateData()
        // {
        //     GameStateData dto = await _multiplayerService.GetGameStateData();
        //     Models.Players.GameStateData gameStateData = DtoConverter.Convert(dto);
        //     return gameStateData;
        // }
    }
}