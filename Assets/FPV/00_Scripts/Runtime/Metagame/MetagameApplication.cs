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
        internal bool IsServer => NetworkManager.Singleton.IsServer;

        protected override void Awake()
        {
            base.Awake();
            Instance = this;
        }

        internal void CallOnReturnToMetagameAfterMatch()
        {
            OnReturnToMetagameAfterMatch?.Invoke();
        }
    }
}