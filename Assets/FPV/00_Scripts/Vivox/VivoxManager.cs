using System;
using System.Threading.Tasks;
using FMOD;
using Unity.Services.Vivox;
using Unity.Services.Vivox.AudioTaps;
using UnityEngine;
using Utils;

namespace Vivox
{
    public sealed class VivoxManager : BaseInstance<VivoxManager>
    {
        [Header("Debug")] [SerializeField] private LogHandler _logHandler;

        private Channel channel;
        private DSP distortionDSP, echoDSP, reverbDSP, pitchDSP;
        private FMOD.System fmodSystem;
        private AudioSource vivoxAudioSource;
        public TaskCompletionSource<bool> ChannelJoinedTaskCompletionSource { get; private set; }

        public async void Start()
        {
            _logHandler?.Log("Initializing Vivox...");
            await UnityServiceAuthentification.Instance.Initialise();

            _logHandler?.Log("Logging into Vivox...");
            await LoginToVivoxAsync();

            _logHandler?.Log("Joining channel...");
            await JoinChannelAsync("test");
            
            VivoxService.Instance.ParticipantAddedToChannel += AddParticipantEffect;
        }
        
        void AddParticipantEffect(VivoxParticipant participant)
        {
            /*var tap = GetComponent<VivoxParticipantTap>();
            tap.ParticipantName = participant.DisplayName;
            tap.ChannelName = participant.ChannelName;*/
            
            GetComponent<VivoxToFmodConverter>().Setup(new AudioModel
            {
                Bank = "Master",
                EventName = "event:/Vivox"
            });
        }


        private async Task LoginToVivoxAsync()
        {
            try
            {
                await VivoxService.Instance.InitializeAsync();
            }
            catch (Exception e)
            {
                _logHandler?.Error($"Failed to initialize Vivox: {e}");
                throw;
            }
            
            try
            {
                var options = new LoginOptions { DisplayName = "Host", EnableTTS = true };
                await VivoxService.Instance.LoginAsync(options);
            }
            catch (Exception e)
            {
                _logHandler?.Error($"Failed to login to Vivox: {e}");
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
                _logHandler?.Log($"Joined channel: {channelName}");
                ChannelJoinedTaskCompletionSource.SetResult(true);
            }
            catch (Exception e)
            {
                _logHandler?.Error($"Failed to join channel : {e}");
                ChannelJoinedTaskCompletionSource.SetResult(false);
            }
        }
    }
}