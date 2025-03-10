using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

namespace FPV
{
    public static class RelayManager
    {
        
        public static string JoinCode { get; private set; }
        
        public static async Task<int> CreateRelayAsync()
        {
            try
            {
                // Create a Relay allocation
                Allocation allocation = await RelayService.Instance.CreateAllocationAsync(2);

                // Get the join code
                string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
                JoinCode = joinCode;

                string host = allocation.RelayServer.IpV4; 
                ushort port = (ushort)allocation.RelayServer.Port; 
                byte[] joinAllocationId = allocation.AllocationIdBytes; 
                byte[] connectionData = allocation.ConnectionData;
                byte[] hostConnectionData = allocation.ConnectionData;
                byte[] key = allocation.Key;
                bool isSecure = false;

                foreach (var endpoint in allocation.ServerEndpoints)
                {
                    if (endpoint.ConnectionType == "dtls")
                    {
                        host = endpoint.Host;
                        port = (ushort)endpoint.Port;
                        isSecure = endpoint.Secure;
                    }
                }

                NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(host,
                    port,
                    joinAllocationId,
                    connectionData,
                    hostConnectionData,
                    key,
                    isSecure));

                NetworkManager.Singleton.StartHost();

                /*// Retry logic for Vivox
                int maxRetries = 5;
                int attempt = 0;
                bool joinedSuccessfully = false;

                while (attempt < maxRetries && !joinedSuccessfully)
                {
                    attempt++;

                    //await VivoxManager.Instance.JoinChannelAsync(joinCode);
                    joinedSuccessfully = await VivoxManager.Instance.ChannelJoinedTaskCompletionSource.Task;

                    if (!joinedSuccessfully)
                    {
                        Debug.LogWarning($"Vivox connection attempt {attempt} failed.");
                        if (attempt < maxRetries)
                        {
                            await Task.Delay(1000); // Wait 1 second before retrying
                        }
                    }
                }

                if (!joinedSuccessfully)
                {
                    Debug.LogError("Failed to join Vivox after 3 attempts.");
                    return 1; // Failure
                }*/

                return 0; // Success
            }
            catch (RelayServiceException e)
            {
                Debug.LogError($"Failed to create Relay: {e}");
                return 1; // Failure
            }
        }

        /// <summary>
        /// Attempts to join a relay using the provided join code.
        /// </summary>
        /// <param name="joinCode">The relay room code.</param>
        /// <returns>
        /// <para>0 = Success</para>
        /// <para>1 = Error</para>
        /// </returns>
        public static async Task<int> JoinRelayAsync(string joinCode)
        {
            try
            {
                // Join an existing Relay allocation
                JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

                string host = joinAllocation.RelayServer.IpV4;
                ushort port = (ushort)joinAllocation.RelayServer.Port;
                byte[] joinAllocationId = joinAllocation.AllocationIdBytes;
                byte[] connectionData = joinAllocation.ConnectionData;
                byte[] hostConnectionData = joinAllocation.HostConnectionData;
                byte[] key = joinAllocation.Key;
                bool isSecure = false;

                foreach (var endpoint in joinAllocation.ServerEndpoints)
                {
                    if (endpoint.ConnectionType == "dtls")
                    {
                        host = endpoint.Host;
                        port = (ushort)endpoint.Port;
                        isSecure = endpoint.Secure;
                    }
                }

                NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(host,
                    port, joinAllocationId, connectionData, hostConnectionData, key, isSecure));
                NetworkManager.Singleton.StartClient();
                
                /*// Retry logic for Vivox TODO ADD
                int maxRetries = 5;
                int attempt = 0;
                bool joinedSuccessfully = false;

                while (attempt < maxRetries && !joinedSuccessfully)
                {
                    attempt++;

                    //await VivoxManager.Instance.JoinChannelAsync(joinCode);
                    joinedSuccessfully = await VivoxManager.Instance.ChannelJoinedTaskCompletionSource.Task;

                    if (!joinedSuccessfully)
                    {
                        Debug.LogWarning($"Vivox connection attempt {attempt} failed.");
                        if (attempt < maxRetries)
                        {
                            await Task.Delay(1000); // Wait 1 second before retrying
                        }
                    }
                }

                if (!joinedSuccessfully)
                {
                    Debug.LogError("Failed to join Vivox after 3 attempts.");
                    return 1; // Failure
                }*/

                return 0; // Success
            }
            catch (RelayServiceException e)
            {
                // Handle not found by creating a new Relay
                switch (e.Reason)
                {
                    case RelayExceptionReason.JoinCodeNotFound:
                        //TODO do something 
                        return 1;

                    case RelayExceptionReason.AllocationNotFound:
                        return 1;

                    // Handle other specific error codes as needed
                    default:
                        Debug.LogError($"Failed to join Relay: {e}");
                        return 1;
                }
            }

        }
        
    }
}