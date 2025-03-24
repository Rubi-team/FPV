using FPV.Runtime.Shared;
using Unity.Netcode;
using UnityEngine;

namespace FPV.Runtime
{
    internal class Player : NetworkBehaviour
    {
        [Header("Player GameObjects")] [SerializeField]
        private NetworkObject playerPrefab;

        [SerializeField] private GameObject playerCameraPrefab;


        [Rpc(SendTo.Everyone)]
        internal void OnClientPrepareGameClientRpc()
        {
            if (!IsLocalPlayer) return;
            if (MetagameApplication.Instance) MetagameApplication.Instance.Broadcast(new MatchEnteredEvent());
            Debug.Log("[Local client] Preparing game [Showing loading screen]");
            if (!IsServer) //the server already does this before asking clients to do the same
                CustomNetworkManager.Singleton.InstantiateGameApplication();
            OnClientReadyToStart();
        }

        internal void OnClientReadyToStart()
        {
            Debug.Log("[Local client] Notifying server I'm ready");
            SpawnPlayerServerRpc(OwnerClientId);
            OnServerNotifiedOfClientReadinessServerRpc();
        }

        [Rpc(SendTo.Server)]
        private void SpawnPlayerServerRpc(ulong clientId)
        {
            var PlayerObj = NetworkManager.SpawnManager.InstantiateAndSpawn(playerPrefab, clientId, true, true);
            if (PlayerObj.TrySetParent(GetComponent<NetworkObject>()) == false)
                Console.LogError("Network", "Failed to set parent");
        }


        [ServerRpc]
        internal void OnServerNotifiedOfClientReadinessServerRpc()
        {
            Debug.Log("[Server] I'm ready");
            CustomNetworkManager.Singleton.OnServerPlayerIsReady(this);
        }

        [ClientRpc]
        internal void OnClientStartGameClientRpc()
        {
            if (!IsLocalPlayer) return;
            //GameApplication.Instance.Broadcast(new StartMatchEvent(false, true));
        }

        [ServerRpc]
        internal void OnPlayerAskedToWinServerRpc()
        {
            OnServerPlayerAskedToWin();
        }

        internal void OnServerPlayerAskedToWin()
        {
            //GameApplication.Instance.Broadcast(new EndMatchEvent(this));
        }
    }
}