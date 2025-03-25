using FPV.Runtime.Shared;
using UnityEngine;
using Unity.Netcode;

namespace FPV
{
    public class PlayerApplication : BaseNetworkApplication<PlayerModel, PlayerView, PlayerController>
    {
        protected override void Awake()
        {
            base.Awake();

            // Optionally, check if this is the local player before performing actions
            if (!IsOwner)
            {
                Debug.Log("Not the owner of this player application.");
                return;
            }

            Debug.Log("PlayerApplication initialized for the local player.");
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            if (!IsOwner)
            {
                Controller.enabled = false;
                View.Hide();
            }
        }

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
            OnServerNotifiedOfClientReadinessServerRpc();
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