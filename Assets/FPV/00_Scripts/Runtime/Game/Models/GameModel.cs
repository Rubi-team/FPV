using System;
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
        public const uint k_CountdownStartValue = 60;

        [Header("Server")] internal ulong PlayerObject0Id;
        internal ulong PlayerObject1Id;

        internal PlayerApplication Player0;
        internal PlayerApplication Player1;


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