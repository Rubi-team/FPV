using System;
using System.Collections;
using System.Threading.Tasks;
using Unity.Services.Vivox;
using UnityEngine;
using Utils;

namespace Vivox
{
    public sealed class VivoxManager : BaseInstance<VivoxManager>
    {
        
        
        public TaskCompletionSource<bool> ChannelJoinedTaskCompletionSource { get; private set; }
        
        
        
        public async void LoginToVivoxAsync()
        {
            LoginOptions options = new LoginOptions();
            options.DisplayName = Unity.Netcode.NetworkManager.Singleton.LocalClientId == 0 ? "Tom-Tom" : "Nana";
            options.EnableTTS = true;
            await VivoxService.Instance.LoginAsync(options);
        }
        
        
        public async Task<bool> JoinChannelAsync(string channelName)
        {
            ChannelJoinedTaskCompletionSource = new TaskCompletionSource<bool>(); // Initialize it at the start
    
            try
            {
                // Leave any existing channel
                await VivoxService.Instance.LeaveAllChannelsAsync();
        
                await VivoxService.Instance.JoinEchoChannelAsync(channelName, ChatCapability.AudioOnly);
        
                Debug.Log($"Joined channel: {channelName}");
                
                ChannelJoinedTaskCompletionSource.SetResult(true); // Ensure task completes
        
                return await ChannelJoinedTaskCompletionSource.Task;
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to join channel {channelName}: {e}");
                ChannelJoinedTaskCompletionSource.SetResult(false); // Ensure task doesn't hang
                return false;
            }
        }
    }
}