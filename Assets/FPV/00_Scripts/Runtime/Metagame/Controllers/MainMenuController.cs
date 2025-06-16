using FPV.Runtime.Shared;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FPV
{
    internal class MainMenuController : Controller<MetagameApplication>
    {
        private MainMenuView View => App.View.MainMenu;

        private void Awake()
        {
            AddListener<MatchLoadingEvent>(OnMatchLoading);
            AddListener<StartSinglePlayerModeEvent>(OnStartSinglePlayerMode);
            AddListener<CreateRelayEvent>(CreateRelay);
            AddListener<JoinRelayEvent>(JoinRelay);
        }

        private void OnDestroy()
        {
            RemoveListeners();
        }

        internal override void RemoveListeners()
        {
            RemoveListener<MatchLoadingEvent>(OnMatchLoading);
            RemoveListener<StartSinglePlayerModeEvent>(OnStartSinglePlayerMode);
        }

        private void OnMatchLoading(MatchLoadingEvent evt)
        {
            View.Hide();
            //App.View.LoadingScreen.Show();
        }

        private async void OnStartSinglePlayerMode(StartSinglePlayerModeEvent evt)
        {
            CustomNetworkManager.Singleton.SinglePlayerMode();
            View.EnableButtonsAndInputField(false);
            await SceneManager.LoadSceneAsync("Game_Main", LoadSceneMode.Additive);
            await SceneManager.LoadSceneAsync("Game_LevelArt", LoadSceneMode.Additive);
            NetworkManager.Singleton.StartHost();
            View.Hide();
        }


        private async void CreateRelay(CreateRelayEvent evt)
        {
            View.Hide();
            // TODO: Add Loading Screen
            await CustomNetworkManager.Singleton.InitializeNetworkLogic(true);
        }

        private async void JoinRelay(JoinRelayEvent evt)
        {
            View.EnableButtonsAndInputField(false);
            View.Hide();
            //TODO: Handle Error and add Loading Screen
            await CustomNetworkManager.Singleton.InitializeNetworkLogic(false, evt.RelayId);
        }
    }
}