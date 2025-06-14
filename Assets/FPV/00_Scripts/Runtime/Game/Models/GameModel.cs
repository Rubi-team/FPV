using System;
using FPV.Runtime.Shared;
using FPV.Shared;
using Unity.Netcode;
using UnityEngine;

namespace FPV.Runtime
{
    /// <summary>
    /// Main model of the <see cref="GameApplication"></see>
    /// </summary>
    public class GameModel : Model<GameApplication>
    {
        internal bool AllowReconnection => FPV_CONSTANTS.ALLOW_RECONNECTION;

        [SerializeField] private MatchDataSynchronizer matchDataSnchronizerPrefab;
        internal MatchDataSynchronizer matchDataSynchronizer;
        [SerializeField] private Menace m_MenacePrefab;
        internal Menace Menace;
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
                var menaceSpawn = FindFirstObjectByType<MenaceStart>().transform.position;
                matchDataSynchronizer = Instantiate(matchDataSnchronizerPrefab);
                matchDataSynchronizer.GetComponent<NetworkObject>().Spawn();
                Menace = Instantiate(m_MenacePrefab, menaceSpawn, Quaternion.identity);
                Menace.GetComponent<NetworkObject>().Spawn();
            }
        }
    }
}