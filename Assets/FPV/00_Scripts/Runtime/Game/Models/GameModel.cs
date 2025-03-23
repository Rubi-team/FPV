using Unity.Netcode;
using UnityEngine;

namespace FPV
{
    /// <summary>
    /// Main model of the <see cref="GameApplication"></see>
    /// </summary>
    public class GameModel : Model<GameApplication>
    {
        /*internal bool AllowReconnection => CustomNetworkManager.Configuration.GetBool(ConfigurationManager.k_AllowReconnection);

        [SerializeField]
        MatchDataSynchronizer matchDataSnchronizerPrefab;
        internal MatchDataSynchronizer matchDataSynchronizer;
        internal const uint k_CountdownStartValue = 60;
        internal uint CountdownValue
        {
            get { return matchDataSynchronizer.MatchCountdown.Value; }
            set { matchDataSynchronizer.MatchCountdown.Value = value; }
        }

        internal bool MatchEnded
        {
            get { return matchDataSynchronizer.MatchEnded.Value; }
            set { matchDataSynchronizer.MatchEnded.Value = value; }
        }

        internal bool MatchStarted
        {
            get { return matchDataSynchronizer.MatchStarted.Value; }
            set { matchDataSynchronizer.MatchStarted.Value = value; }
        }

        void Awake()
        {
            if (CustomNetworkManager.Singleton.IsServer)
            {
                matchDataSynchronizer = Instantiate(matchDataSnchronizerPrefab);
                matchDataSynchronizer.GetComponent<NetworkObject>().Spawn();
            }
        }*/
    }
}
