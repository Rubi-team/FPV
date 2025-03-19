using UnityEngine;

namespace FPV
{
    /// <summary>
    /// Main controller of the <see cref="MetagameApplication"></see>
    /// </summary>
    public class MetagameController : Controller<MetagameApplication>
    {
        private void Awake()
        {
            AddListener<PlayerSignedIn>(OnPlayerSignedIn);
            AddListener<MatchEnteredEvent>(OnMatchEntered);
            AddListener<ApplicationQuitEvent>(ApplicationQuit);
        }

        private void OnDestroy()
        {
            RemoveListeners();
        }

        internal override void RemoveListeners()
        {
            RemoveListener<PlayerSignedIn>(OnPlayerSignedIn);
            RemoveListener<MatchEnteredEvent>(OnMatchEntered);
        }

        private void OnPlayerSignedIn(PlayerSignedIn evt)
        {
            if (evt.Success)
                Debug.Log($"Player signed in with id {evt.PlayerId}");
            else
                Debug.Log("Player did not sign in");
        }

        private void OnMatchEntered(MatchEnteredEvent evt)
        {
            DisableViewsAndListeners();
        }

        private void DisableViewsAndListeners()
        {
            for (var i = 0; i < App.View.transform.childCount; i++)
                App.View.transform.GetChild(i).gameObject.SetActive(false);
            App.OnReturnToMetagameAfterMatch -= OnReturnToMetagameAfterMatch;
            App.OnReturnToMetagameAfterMatch += OnReturnToMetagameAfterMatch;
            //CustomNetworkManager.Singleton.ReturnToMetagame = App.CallOnReturnToMetagameAfterMatch; TODO ADD THIS FUNCTION
        }

        private void OnReturnToMetagameAfterMatch()
        {
            EnableViewsAndListener();
        }

        private void EnableViewsAndListener()
        {
            for (var i = 0; i < App.View.transform.childCount; i++)
                App.View.transform.GetChild(i).gameObject.SetActive(true);
            /*App.View.Matchmaker.Hide();
            App.View.LoadingScreen.Hide();
            App.View.MainMenu.DisableControlsUnsupportedInAutoconnectMode();*/
        }

        private void ApplicationQuit(ApplicationQuitEvent evt)
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            UnityEngine.Application.Quit();
#endif
        }
    }
}