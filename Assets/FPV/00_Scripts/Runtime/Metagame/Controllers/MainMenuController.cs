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
            View.EnableButtonsAndInputField(false);
            await SceneManager.LoadSceneAsync(1, LoadSceneMode.Additive);
            NetworkManager.Singleton.StartHost();
            View.Hide();
        }


        //TODO maybe Refact en vrai jpense pas mais a check
        private async void CreateRelay(CreateRelayEvent evt)
        {
            await SceneManager.LoadSceneAsync(1, LoadSceneMode.Additive);
            await RelayManager.CreateRelayAsync();
            // TODO: Add Loading Screen
            View.Hide();
        }

        private async void JoinRelay(JoinRelayEvent evt)
        {
            View.EnableButtonsAndInputField(false);
            //TODO: Handle Error 
            await RelayManager.JoinRelayAsync(evt.RelayId);
            View.Hide();
        }
    }
}