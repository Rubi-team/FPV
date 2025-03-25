using System.Collections;
using System.Linq;
using FPV.Runtime;
using FPV.runtime.Shared;
using FPV.Runtime.Shared;
using Unity.Netcode;
using UnityEngine;

namespace FPV
{
    /// <summary>
    /// Main controller of the <see cref="GameApplication"></see>
    /// </summary>
    public class GameController : Controller<GameApplication>
    {
        GameModel Model => App.Model;
        Coroutine m_CountdownRoutine;

        void Awake()
        {
            AddListener<StartMatchEvent>(OnServerStartMatch);
            AddListener<EndMatchEvent>(OnServerMatchEnded);
            AddListener<PlayerDisconnected>(OnServerPlayerDisconnected);
        }

        void OnDestroy()
        {
            RemoveListeners();
        }

        internal override void RemoveListeners()
        {
            RemoveListener<StartMatchEvent>(OnServerStartMatch);
            RemoveListener<EndMatchEvent>(OnServerMatchEnded);
            RemoveListener<PlayerDisconnected>(OnServerPlayerDisconnected);
        }

        void OnServerPlayerDisconnected(PlayerDisconnected evt)
        {
            Debug.Log($"[Server] Client with it {evt.ClientId} disconnected!");
            if (Model.AllowReconnection)
            {
                return;
            }
            if (Model.MatchStarted && !Model.MatchEnded)
            {
                NetworkClient firstClientStillConnected = NetworkManager.Singleton.ConnectedClients.Where(cc => cc.Key != evt.ClientId)
                                                                                                   .Select(v => v.Value)
                                                                                                   .FirstOrDefault();
                PlayerApplication winner = firstClientStillConnected == null ? null
                                                                  : firstClientStillConnected.PlayerObject.GetComponent<PlayerApplication>();
                Broadcast(new EndMatchEvent(winner));
            }
        }

        void OnServerStartMatch(StartMatchEvent evt)
        {
            if (evt.IsServer)
            {
                Debug.Log("[Server] Starting match!");
                Model.MatchStarted = true;
                Model.MatchEnded = false;
                OnServerStartCountdown();
            }
            if (evt.IsClient)
            {
                Debug.Log("[Client] Starting match!");
            }
        }

        void OnServerStartCountdown()
        {
            Model.CountdownValue = GameModel.k_CountdownStartValue;
            m_CountdownRoutine = StartCoroutine(OnServerDoCountdown());
        }

        IEnumerator OnServerDoCountdown()
        {
            while (Model.CountdownValue > 0
            && !Model.MatchEnded)
            {
                yield return BetterCoroutines.OneSecond;
                Model.CountdownValue--;
            }

            if (Model.MatchEnded) //somebody won
            {
                yield break;
            }
            OnServerCountdownExpired();
        }

        void OnServerCountdownExpired()
        {
            Broadcast(new EndMatchEvent(null));
        }

        void OnServerMatchEnded(EndMatchEvent evt)
        {
            if (Model.MatchEnded)
            {
                return;
            }
            Model.MatchEnded = true;
            Model.MatchStarted = false;
            if (m_CountdownRoutine != null)
            {
                StopCoroutine(m_CountdownRoutine);
                m_CountdownRoutine = null;
            }

            ulong winnerClientId = ulong.MaxValue;
            if (evt.Winner != null)
            {
                winnerClientId = evt.Winner.OwnerClientId;
            }
            Model.matchDataSynchronizer.OnClientMatchResultComputedClientRpc(winnerClientId);
            if (App.IsDedicatedServer)
            {
                CustomNetworkManager.Singleton.OnServerQuitAfter(5);
            }
        }
    }
}
