using FPV.Runtime.Shared;
using FPV.Shared;
using Unity.Netcode;
using UnityEngine;

namespace FPV
{
    /// <summary>
    /// Main model of the <see cref="GameApplication"></see>
    /// </summary>
    public class GameModel : Model<GameApplication>
    {
        internal bool AllowReconnection => FPV_CONSTANTS.ALLOW_RECONNECTION;

        [SerializeField] private MatchDataSynchronizer matchDataSnchronizerPrefab;
        internal MatchDataSynchronizer matchDataSynchronizer;
        internal const uint k_CountdownStartValue = 60;

        [Header("Server")] public ulong PlayerObject1Id;
        public ulong PlayerObject2Id;

        internal uint CountdownValue
        {
            get => matchDataSynchronizer.MatchCountdown.Value;
            set => matchDataSynchronizer.MatchCountdown.Value = value;
        }

        internal bool MatchEnded
        {
            get => matchDataSynchronizer.MatchEnded.Value;
            set => matchDataSynchronizer.MatchEnded.Value = value;
        }

        internal bool MatchStarted
        {
            get => matchDataSynchronizer.MatchStarted.Value;
            set => matchDataSynchronizer.MatchStarted.Value = value;
        }

        private void Awake()
        {
            if (CustomNetworkManager.Singleton.IsHost)
            {
                matchDataSynchronizer = Instantiate(matchDataSnchronizerPrefab);
                matchDataSynchronizer.GetComponent<NetworkObject>().Spawn();
            }
        }
    }
}