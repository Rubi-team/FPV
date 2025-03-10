using System;
using System.Collections;
using System.Threading.Tasks;
using Unity.Services.Core;
using UnityEngine;

namespace FPV
{
    ///<summary>
    ///Initializes all the Unity Services managers
    ///</summary>
    internal class UnityServicesInitializer : MonoBehaviour
    {
        public const string k_ServerID = "SERVER";
        public static UnityServicesInitializer Instance { get; private set; }

        public const string k_Environment = "production";
        public void Awake()
        {
            if (Instance && Instance != this)
            {
                return;
            }
            Instance = this;
            //CustomNetworkManager.OnConfigurationLoaded += OnConfigurationLoaded;
        }

        /*void OnConfigurationLoaded()
        {
            CustomNetworkManager.OnConfigurationLoaded -= OnConfigurationLoaded;
            OnConfigurationLoaded(CustomNetworkManager.Configuration);
        }

        async void OnConfigurationLoaded(ConfigurationManager configuration)
        {
            Debug.Log($"Configuration loaded: {configuration}");
            await Initialize(configuration.GetMultiplayerRole() == Unity.Multiplayer.MultiplayerRoleFlags.Server ? k_ServerID
                                                                                                                 : string.Empty);
        }*/

        public async Task Initialize(string externalPlayerID)
        {
            string serviceProfileName = "default"; //note: by using "default" UGS automatically assign a different Profile name to every MPPM virtual player.
#if UNITY_EDITOR && HAS_PARRELSYNC
            if (ParrelSync.ClonesManager.IsClone())
            {
                serviceProfileName = "CloneProfile";
            }
#endif
            if (!string.IsNullOrEmpty(externalPlayerID))
            {
                UnityServices.ExternalUserId = externalPlayerID;
            }

            Debug.Log($"Initializing services with externalPlayerID: {externalPlayerID}");
            bool signedIn = await UnityServiceAuthenticator.TrySignInAsync(k_Environment, serviceProfileName);
            MetagameApplication.Instance.Broadcast(new PlayerSignedIn(signedIn, UnityServiceAuthenticator.PlayerId));
            if (!signedIn)
            {
                return;
                //TODO: handle sign in error
            }
            if (externalPlayerID != k_ServerID)
            {
                InitializeClientOnlyServices();
            }
        }

        void InitializeClientOnlyServices()
        {
            
        }
    }
}
