using Unity.Netcode;
using UnityEngine;

namespace FPV.Runtime
{
    /// <summary>
    /// Holds the logical state of a game and synchronizes it across the network
    /// </summary>
    internal class MatchDataSynchronizer : NetworkBehaviour
    {
        internal NetworkVariable<uint> MatchCountdown = new();
        internal NetworkVariable<bool> MatchEnded = new();
        internal NetworkVariable<bool> MatchStarted = new();

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            if (IsClient)
            {
                MatchEnded.OnValueChanged += OnClientMatchEndedChanged;
                MatchStarted.OnValueChanged += OnClientMatchStartedChanged;
            }

            if (IsHost) //TODO remove when new spawning system
            {
                GameApplication.Instance.Model.PlayerObject1Id =
                    NetworkManager.Singleton.SpawnManager.PlayerObjects[0].NetworkObjectId;
                GameApplication.Instance.Model.PlayerObject1Id =
                    NetworkManager.Singleton.SpawnManager.PlayerObjects[1].NetworkObjectId;

                //Spawn Menace here ? 
            }
        }


        private void OnClientMatchEndedChanged(bool previousValue, bool newValue)
        {
            //you can block inputs here, play animations and so on
            Debug.Log($"New match ended value: {newValue}");
        }

        private void OnClientMatchStartedChanged(bool previousValue, bool newValue)
        {
            //you can enable inputs here, play animations and so on
            Debug.Log($"New match started value: {newValue}");
        }

        [ClientRpc]
        internal void OnClientMatchResultComputedClientRpc(ulong winnerClientId)
        {
            GameApplication.Instance.Broadcast(new MatchResultComputedEvent(winnerClientId));
        }
    }
}