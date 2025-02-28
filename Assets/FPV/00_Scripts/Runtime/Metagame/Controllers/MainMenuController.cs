using UnityEngine;

namespace FPV
{
    internal class MainMenuController : Controller<MetagameApplication>
    {
        MainMenuView View => App.View.MainMenu;

        void Awake()
        {
            AddListener<MatchLoadingEvent>(OnMatchLoading);
            AddListener<StartSinglePlayerModeEvent>(OnStartSinglePlayerMode);
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
            //CustomNetworkManager.Singleton.InitializeNetworkLogic(true, false);
        }
    }
}
