using System;
using Unity.Netcode;
using UnityEngine;

namespace FPV
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
        }

        protected void OnApplicationFocus(bool hasFocus)
        {
            //lock cursor when the game is focused
            Cursor.lockState = hasFocus ? CursorLockMode.Locked : CursorLockMode.None;
        }
    }
}