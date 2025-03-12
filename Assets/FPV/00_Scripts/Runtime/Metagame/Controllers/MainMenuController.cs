using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FPV
{
    internal class MainMenuController : Controller<MetagameApplication>
    {
        MainMenuView View => App.View.MainMenu;

        void Awake()
        {
            AddListener<MatchLoadingEvent>(OnMatchLoading);
            AddListener<StartSinglePlayerModeEvent>(OnStartSinglePlayerMode);
            AddListener<CreateRelayEvent>(CreateRelay);
            AddListener<JoinRelayEvent>(JoinRelay);
        }

        void OnDestroy()
        {
            RemoveListeners();
        }

        internal override void RemoveListeners()
        {
            RemoveListener<MatchLoadingEvent>(OnMatchLoading);
            RemoveListener<StartSinglePlayerModeEvent>(OnStartSinglePlayerMode);
        }

        void OnMatchLoading(MatchLoadingEvent evt)
        {
            View.Hide();
            //App.View.LoadingScreen.Show();
        }

        void OnStartSinglePlayerMode(StartSinglePlayerModeEvent evt)
        {
            View.Hide();
            SceneManager.LoadScene(1, LoadSceneMode.Additive);
        }
        
        
        //TODO maybe Refact en vrai jpense pas mais a check
        async void CreateRelay(CreateRelayEvent evt)
        {
            await RelayManager.CreateRelayAsync();
            // TODO: Add Loading Screen
            View.Hide();
        }
        
        async void JoinRelay(JoinRelayEvent evt)
        {
            await RelayManager.JoinRelayAsync(evt.RelayId);
            View.Hide();
        }
        
    }
}
