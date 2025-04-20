using System;
using System.Threading.Tasks;
using FMOD;
using FMODUnity;
using NUnit.Framework.Constraints;
using Unity.Services.Matchmaker.Models;
using Unity.Services.Vivox;
using Unity.Services.Vivox.AudioTaps;
using UnityEngine;
using UnityEngine.Serialization;
using Debug = UnityEngine.Debug;

namespace FPV
{
    public sealed class VivoxManager : MonoBehaviour
    {
        [Header("Events References")] [SerializeField]
        private EventReference VivoxEvent0;

        public TaskCompletionSource<bool> ChannelJoinedTaskCompletionSource { get; private set; }


        public static VivoxManager Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        public async void VivoxInit()
        {
            Debug.Log("Logging into Vivox...");
            await LoginToVivoxAsync();

            Debug.Log("Joining channel...");
            await JoinChannelAsync(RelayManager.JoinCode);
        }


        private async Task LoginToVivoxAsync()
        {
            // TODO : Handle Vivox initialization and login errors
            try
            {
                await VivoxService.Instance.InitializeAsync();
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to initialize Vivox: {e}");
                throw;
            }

            try
            {
                //TODO add DisplayName si utile
                var options = new LoginOptions { EnableTTS = false };
                await VivoxService.Instance.LoginAsync(options);
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to login to Vivox: {e}");
                throw;
            }
        }

        private async Task JoinChannelAsync(string channelName)
        {
            ChannelJoinedTaskCompletionSource = new TaskCompletionSource<bool>();
            try
            {
                await VivoxService.Instance.LeaveAllChannelsAsync();
                await VivoxService.Instance.JoinGroupChannelAsync(channelName, ChatCapability.AudioOnly);
                Debug.Log($"Joined channel: {channelName}");

                ChannelJoinedTaskCompletionSource.SetResult(true);
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to join channel: {e}");
                ChannelJoinedTaskCompletionSource.SetResult(false);
            }
        }
    }
}