using System.Collections;
using System.Linq;
using FPV.Runtime;
using FPV.runtime.Shared;
using FPV.Runtime.Shared;
using Unity.Netcode;
using UnityEngine;

namespace FPV.Runtime
{
    /// <summary>
    /// Main controller of the <see cref="GameApplication"></see>
    /// </summary>
    public class GameController : Controller<GameApplication>
    {
        private GameModel Model => App.Model;
        private Coroutine m_CountdownRoutine;

        private void Awake()
        {
            AddListener<StartMatchEvent>(OnServerStartMatch);
            AddListener<EndMatchEvent>(OnServerMatchEnded);
            AddListener<PlayerDisconnected>(OnServerPlayerDisconnected);
        }

        private void OnDestroy()
        {
            RemoveListeners();
        }

        internal override void RemoveListeners()
        {
            RemoveListener<StartMatchEvent>(OnServerStartMatch);
            RemoveListener<EndMatchEvent>(OnServerMatchEnded);
            RemoveListener<PlayerDisconnected>(OnServerPlayerDisconnected);
        }


        private void OnServerPlayerDisconnected(PlayerDisconnected evt)
        {
            Debug.Log($"[Server] Client with it {evt.ClientId} disconnected!");
            if (Model.AllowReconnection) return;
            if (Model.MatchStarted && !Model.MatchEnded)
            {
                var firstClientStillConnected = NetworkManager.Singleton.ConnectedClients
                    .Where(cc => cc.Key != evt.ClientId)
                    .Select(v => v.Value)
                    .FirstOrDefault();
                var winner = firstClientStillConnected == null
                    ? null
                    : firstClientStillConnected.PlayerObject.GetComponent<PlayerApplication>();
                Broadcast(new EndMatchEvent(winner));
            }
        }

        private void OnServerStartMatch(StartMatchEvent evt)
        {
            if (evt.IsServer)
            {
                Debug.Log("[Server] Starting match!");
                Model.MatchStarted = true;
                Model.MatchEnded = false;
                OnServerStartCountdown();
            }

            if (evt.IsClient) Debug.Log("[Client] Starting match!");
        }

        private void OnServerStartCountdown()
        {
            Model.CountdownValue = GameModel.k_CountdownStartValue;
            m_CountdownRoutine = StartCoroutine(OnServerDoCountdown());
        }

        private IEnumerator OnServerDoCountdown()
        {
            while (Model.CountdownValue > 0
                   && !Model.MatchEnded)
            {
                yield return BetterCoroutines.OneSecond;
                Model.CountdownValue--;
            }

            if (Model.MatchEnded) //somebody won
                yield break;
            OnServerCountdownExpired();
        }

        private void OnServerCountdownExpired()
        {
            Broadcast(new EndMatchEvent(null));
        }

        private void OnServerMatchEnded(EndMatchEvent evt)
        {
            if (Model.MatchEnded) return;
            Model.MatchEnded = true;
            Model.MatchStarted = false;
            if (m_CountdownRoutine != null)
            {
                StopCoroutine(m_CountdownRoutine);
                m_CountdownRoutine = null;
            }

            var winnerClientId = ulong.MaxValue;
            if (evt.Winner != null) winnerClientId = evt.Winner.OwnerClientId;
            Model.matchDataSynchronizer.OnClientMatchResultComputedClientRpc(winnerClientId);
            if (App.IsDedicatedServer) CustomNetworkManager.Singleton.OnServerQuitAfter(5);
        }
    }
}