using System;
using FPV.Runtime.Shared;
using Unity.Netcode;
using UnityEngine;

namespace FPV.Runtime
{
    /// <summary>
    /// Manages the flow of the Game part of the application
    /// </summary>
    public class GameApplication : BaseApplication<GameModel, GameView, GameController>
    {
        internal new static GameApplication Instance { get; private set; }
        internal bool IsDedicatedServer => NetworkManager.Singleton.IsServer && !NetworkManager.Singleton.IsClient;

        protected override void Awake()
        {
            base.Awake();
            Instance = this;
            LockCursor(true);
        }

        public void Start()
        {
            Broadcast(new ServerPrepareGameEvent());
        }

        protected void OnApplicationFocus(bool hasFocus)
        {
            LockCursor(hasFocus);
        }

        protected void LockCursor(bool lockCursor)
        {
            if (lockCursor)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
    }
}