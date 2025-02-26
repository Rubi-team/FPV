using UnityEngine;

namespace FPV
{
    /// <summary>
    /// Main controller of the <see cref="MetagameApplication"></see>
    /// </summary>
    public class MetagameController : Controller<MetagameApplication>
    {
        void Awake()
        {
            AddListener<PlayerSignedIn>(OnPlayerSignedIn);
            AddListener<MatchEnteredEvent>(OnMatchEntered);
        }

        void OnDestroy()
        {
            RemoveListeners();
        }

        internal override void RemoveListeners()
        {
            RemoveListener<PlayerSignedIn>(OnPlayerSignedIn);
            RemoveListener<MatchEnteredEvent>(OnMatchEntered);
        }
        void OnPlayerSignedIn(PlayerSignedIn evt)
        {
            if (evt.Success)
            {
                Debug.Log($"Player signed in with id {evt.PlayerId}");
            }
            else
            {
                Debug.Log("Player did not sign in");
            }
        }

        void OnMatchEntered(MatchEnteredEvent evt)
        {
            DisableViewsAndListeners();
        }

        void DisableViewsAndListeners()
        {
            for (int i = 0; i < App.View.transform.childCount; i++)
            {
                App.View.transform.GetChild(i).gameObject.SetActive(false);
            }
            App.OnReturnToMetagameAfterMatch -= OnReturnToMetagameAfterMatch;
            App.OnReturnToMetagameAfterMatch += OnReturnToMetagameAfterMatch;
            //CustomNetworkManager.Singleton.ReturnToMetagame = App.CallOnReturnToMetagameAfterMatch; TODO ADD THIS FUNCTION
        }

        void OnReturnToMetagameAfterMatch()
        {
            EnableViewsAndListener();
        }

        void EnableViewsAndListener()
        {
            for (int i = 0; i < App.View.transform.childCount; i++)
            {
                App.View.transform.GetChild(i).gameObject.SetActive(true);
            }
            /*App.View.Matchmaker.Hide();
            App.View.LoadingScreen.Hide();
            App.View.MainMenu.DisableControlsUnsupportedInAutoconnectMode();*/
        }
    }
}
