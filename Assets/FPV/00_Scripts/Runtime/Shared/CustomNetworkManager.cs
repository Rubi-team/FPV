using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FPV.runtime.Shared;
using FPV.Shared;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Matchmaker.Models;
using Unity.Services.Relay;
using UnityEditor;
using UnityEditor.Networking.PlayerConnection;
using UnityEngine;
using UnityEngine.SceneManagement;
using NetworkSceneManager = Unity.Netcode.NetworkSceneManager;
#if UNITY_SERVER || ENABLE_UCS_SERVER
using Unity.Services.Authentication.Server;
#endif

namespace FPV.Runtime.Shared
{
    /// <summary>
    ///     A custom network manager that implements additional setup logic and rules
    /// </summary>
    [RequireComponent(typeof(NetworkManager))]
    public class CustomNetworkManager : MonoBehaviour
    {
        public static CustomNetworkManager Singleton { get; private set; }

        internal bool AutoConnectOnStartup => FPV_CONSTANTS.AUTO_CONNECT;

        internal bool IsClient => m_NetworkManager.IsClient;
        internal bool IsHost => m_NetworkManager.IsHost;

        internal Action ReturnToMetagame;
        internal int ExpectedPlayers { get; private set; } = 2;

        private bool m_PreparedGame = true;

        [SerializeField] private GameApplication m_GameAppPrefab;
        private GameApplication m_GameApp;

        internal HashSet<PlayerApplication> ReadyPlayers { get; private set; }
        private NetworkManager m_NetworkManager;

        private void Awake()
        {
            if (Singleton == null) Singleton = this;

            m_NetworkManager = GetComponent<NetworkManager>();

            m_NetworkManager.OnClientConnectedCallback += OnClientConnected;
            m_NetworkManager.OnClientDisconnectCallback += OnClientDisconnected;
            m_NetworkManager.OnServerStarted += OnServerStarted;
        }


        /// <summary>
        ///     Initializes the application's network-related behaviour according to the circumstances
        /// </summary>
        /// <param name="startedByUser">Is Creating the relay?</param>
        /// <param name="singlePlayerMode">Start in SinglePlayer?</param>
        public async Task InitializeNetworkLogic(bool createRelay, string relayCode = null)
        {
            if (IsClient || IsHost)
                m_NetworkManager.Shutdown(true);

            ExpectedPlayers = FPV_CONSTANTS.MAX_PLAYERS;

            if (createRelay)
            {
                await RelayManager.CreateRelayAsync();

                await SceneManager.LoadSceneAsync("Game", LoadSceneMode.Additive);
                await SceneManager.LoadSceneAsync("LevelArt", LoadSceneMode.Additive);

                StartHost();
            }
            else
            {
                await RelayManager.JoinRelayAsync(relayCode);
                StartClient();
            }
        }


        internal async void AutoConnect()
        {
            await InitializeNetworkLogic(true);
        }


        private void OnServerStarted()
        {
            ReadyPlayers = new HashSet<PlayerApplication>();
            m_PreparedGame = false;
        }

        internal void StartHost()
        {
            if (m_NetworkManager.IsHost || m_NetworkManager.IsClient)
            {
                Debug.LogWarning("Already started as host or client");
                return;
            }

            m_NetworkManager.StartHost();
        }

        internal void StartClient()
        {
            if (m_NetworkManager.IsHost || m_NetworkManager.IsClient)
            {
                Debug.LogWarning("Already started as host or client");
                return;
            }

            m_NetworkManager.StartClient();
        }

        internal void OnServerQuitAfter(float seconds)
        {
            Debug.Log($"[Server] quitting game in {seconds} seconds!");
            StartCoroutine(BetterCoroutines.WaitAndDo(new WaitForSeconds(seconds), OnServerQuit));
        }

        private void OnServerQuit()
        {
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
            UnityEngine.Application.Quit();
#endif
        }

        private void OnClientDisconnected(ulong ClientId)
        {
            Debug.Log($"Client {ClientId} disconnected");
            if (IsHost)
            {
                ReadyPlayers.RemoveWhere(p =>
                    p.NetworkObject == m_NetworkManager.ConnectedClients[ClientId].PlayerObject);
                if (GameApplication.Instance) //the game already started
                    GameApplication.Instance.Broadcast(new PlayerDisconnected(ClientId));
            }
        }

        private void OnClientConnected(ulong ClientId)
        {
            if (IsClient)
            {
                Debug.Log($"Local client {ClientId} connected, waiting for other players...");
                if (MetagameApplication.Instance) MetagameApplication.Instance.Broadcast(new MatchLoadingEvent());
            }
            else
            {
                Debug.Log($"Remote client {ClientId} connected");
            }

            if (m_PreparedGame || !IsHost) //game should be prepared only once per server session
                return;
            if (m_NetworkManager.ConnectedClients.Count == ExpectedPlayers) OnHostPrepareGame();
        }

        internal void OnServerPlayerIsReady(PlayerApplication playerApplication)
        {
            ReadyPlayers.Add(playerApplication);
            if (ReadyPlayers.Count == ExpectedPlayers) OnServerGameReadyToStart();
        }

        private void OnHostPrepareGame()
        {
            Debug.Log("[Server] Preparing game");
            m_PreparedGame = true;
            InstantiateGameApplication();
            foreach (var connectionToClient in m_NetworkManager.ConnectedClients.Values)
                connectionToClient.PlayerObject.GetComponent<PlayerApplication>().OnClientPrepareGameClientRpc();
        }

        internal void InstantiateGameApplication()
        {
            m_GameApp = Instantiate(m_GameAppPrefab);
        }

        internal void OnServerGameReadyToStart()
        {
            m_GameApp.Broadcast(new StartMatchEvent(true, false));
            foreach (var player in ReadyPlayers) player.OnClientStartGameClientRpc();
            ReadyPlayers.Clear();
        }

        internal void SinglePlayerMode()
        {
            ExpectedPlayers = 1;
            ChangeTransport();
        }

        internal void ChangeTransport()
        {
            var uTP = gameObject.AddComponent<UnityTransport>();
            m_NetworkManager.NetworkConfig.NetworkTransport = uTP;
        }

        /// <summary>
        ///     Performs cleanup operation after a game
        /// </summary>
        internal void OnClientDoPostMatchCleanupAndReturnToMetagame()
        {
            if (IsClient) m_NetworkManager.Shutdown();
            Destroy(GameApplication.Instance.gameObject);
            ReturnToMetagame?.Invoke();
        }
    }
}