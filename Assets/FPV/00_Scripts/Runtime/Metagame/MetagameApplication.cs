using System;
using Unity.Netcode;

namespace FPV
{
    /// <summary>
    /// The application that manages the Metagame
    /// </summary>
    public class MetagameApplication : BaseApplication<MetagameModel, MetagameView, MetagameController>
    {
        internal new static MetagameApplication Instance { get; private set; }

        internal event Action OnReturnToMetagameAfterMatch;
        internal bool IsServer => NetworkManager.Singleton.IsServer && !NetworkManager.Singleton.IsClient;
        
        internal bool IsHost => NetworkManager.Singleton.IsHost;

        protected override void Awake()
        {
            base.Awake();
            Instance = this;
            DontDestroyOnLoad(this);
        }

        internal void CallOnReturnToMetagameAfterMatch()
        {
            OnReturnToMetagameAfterMatch?.Invoke();
        }
    }
}