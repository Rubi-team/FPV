using System;
using System.Threading.Tasks;
using FMOD;
using FMODUnity;
using NUnit.Framework.Constraints;
using Unity.Services.Matchmaker.Models;
using Unity.Services.Vivox;
using Unity.Services.Vivox.AudioTaps;
using UnityEngine;
using Utils;

namespace Vivox
{
    public sealed class VivoxManager : BaseInstance<VivoxManager>
    {
        [Header("Events References")] 
        [SerializeField] private EventReference VivoxEvent1;
        [SerializeField] private EventReference VivoxEvent2;
        [SerializeField] private EventReference VivoxEvent3;
        
        
        [Header("Debug")] [SerializeField] private LogHandler _logHandler;
        public bool echoChannel = false;

        private Channel channel;
        private DSP distortionDSP, echoDSP, reverbDSP, pitchDSP;
        private FMOD.System fmodSystem;
        private AudioSource vivoxAudioSource;
        public TaskCompletionSource<bool> ChannelJoinedTaskCompletionSource { get; private set; }

        private int otherPlayersCount = 0;

        public async void Start()
        {
            _logHandler?.Log("Initializing Vivox...");
            await UnityServiceAuthentification.Instance.Initialise();

            _logHandler?.Log("Logging into Vivox...");
            await LoginToVivoxAsync();
            
            VivoxService.Instance.ParticipantAddedToChannel += AddParticipantEffect;
            VivoxService.Instance.ChannelJoined += JoinChannel;

            _logHandler?.Log("Joining channel...");
            await JoinChannelAsync("test");

            if (echoChannel) 
            {
                var audioModel = new AudioModel
                {
                    Bank = "Master",
                    EventName = "event:/Vivox1"
                };
            
                GetComponent<VivoxToFmodConverter>().Setup(audioModel);
                
            }
            
        }
        
        void AddParticipantEffect(VivoxParticipant participant) // TODO CLEANUP
        {
            if (participant.IsSelf) return;
            
            switch (otherPlayersCount)
            {
                case 0:
                    var player1Tap = participant.CreateVivoxParticipantTap("Player 1 Tap");
                    
                    var converter = player1Tap.AddComponent<VivoxToFmodConverter>();
                    converter.logHandler = _logHandler;
                    
                    var audio1 = new AudioModel
                    {
                        Bank = "Master",
                        EventName = "event:/Vivox1"
                    };
                    
                    converter.Setup(audio1);
                    
                    /*var fmodEmitter1 = player1Tap.AddComponent<StudioEventEmitter>();
                    fmodEmitter1.EventReference= VivoxEvent1;
                    fmodEmitter1.Play();*/
                    otherPlayersCount++;
                    break;
                case 1:
                    var player2Tap = participant.CreateVivoxParticipantTap("Player 2 Tap");
                    
                    var converter2 = player2Tap.AddComponent<VivoxToFmodConverter>();
                    converter2.logHandler = _logHandler;
                    
                    var audio2 = new AudioModel
                    {
                        Bank = "Master",
                        EventName = "event:/Vivox2"
                    };
                    
                    converter2.Setup(audio2);
                    
                    var fmodEmitter2 = player2Tap.AddComponent<StudioEventEmitter>();
                    fmodEmitter2.EventReference = VivoxEvent2;
                    fmodEmitter2.Play();
                    otherPlayersCount++;
                    break;
                case 2:
                    var player3Tap = participant.CreateVivoxParticipantTap("Player 3 Tap");
                    
                    var converter3 = player3Tap.AddComponent<VivoxToFmodConverter>();
                    converter3.logHandler = _logHandler;
                    
                    var audio3 = new AudioModel
                    {
                        Bank = "Master",
                        EventName = "event:/Vivox3"
                    };
                    
                    converter3.Setup(audio3);

                    var fmodEmitter3 = player3Tap.AddComponent<StudioEventEmitter>();
                    fmodEmitter3.EventReference = VivoxEvent3;
                    fmodEmitter3.Play();
                    otherPlayersCount++;
                    break;
            }
        }

        private void JoinChannel(string channelName)
        {
            // loop through all participants and add the effect
            foreach (var participant in VivoxService.Instance.ActiveChannels[channelName])
            {
                if (!participant.IsSelf) 
                    AddParticipantEffect(participant);
            }
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