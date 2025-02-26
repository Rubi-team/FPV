using UnityEngine;

namespace FPV
{
    /// <summary>
    /// Main view of the <see cref="MetagameApplication"></see>
    /// </summary>
    public class MetagameView : View<MetagameApplication>
    {
       // internal MainMenuView MainMenu => m_MainMenuView;


        //MainMenuView m_MainMenuView;
        
        //internal LoadingScreenView LoadingScreen => m_LoadingScreenView;

        //LoadingScreenView m_LoadingScreenView;

        void Start()
        {
            if (App.IsServer)
            {
                OnDedicatedServerDestroyViews();
            }
        }

        void OnDedicatedServerDestroyViews()
        {
            Destroy(gameObject);
        }
    }
}