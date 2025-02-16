using System;
using System.Threading.Tasks;
using FMOD;
using FMODUnity;
using Unity.Services.Vivox;
using UnityEngine;
using Utils;
using Logger = Utils.Logger;

namespace Vivox
{
    public sealed class VivoxManager : BaseInstance<VivoxManager>
    {
        [Header("Debug")] [SerializeField] private Logger _logger;
        private Channel channel;
        private DSP distortionDSP, echoDSP, reverbDSP, pitchDSP;
        private FMOD.System fmodSystem;
        private AudioSource vivoxAudioSource;
        public TaskCompletionSource<bool> ChannelJoinedTaskCompletionSource { get; private set; }

        public async void Start()
        {
            await UnityServiceAuthentification.Instance.Initialise();
            await LoginToVivoxAsync();
            await JoinChannelAsync("test");
        }

        public async Task<Task> LoginToVivoxAsync()
        {
            await VivoxService.Instance.InitializeAsync();
            var options = new LoginOptions { DisplayName = "Host", EnableTTS = true };
            await VivoxService.Instance.LoginAsync(options);
            return Task.CompletedTask;
        }

        public async Task<bool> JoinChannelAsync(string channelName)
        {
            ChannelJoinedTaskCompletionSource = new TaskCompletionSource<bool>();
            try
            {
                await VivoxService.Instance.LeaveAllChannelsAsync();
                await VivoxService.Instance.JoinEchoChannelAsync(channelName, ChatCapability.AudioOnly);
                _logger.Log($"Joined channel: {channelName}");
                ChannelJoinedTaskCompletionSource.SetResult(true);
                return await ChannelJoinedTaskCompletionSource.Task;
            }
            catch (Exception e)
            {
                _logger.ErrorLog($"Failed to join channel {channelName}: {e}");
                ChannelJoinedTaskCompletionSource.SetResult(false);
                return false;
            }
        }
    }
}