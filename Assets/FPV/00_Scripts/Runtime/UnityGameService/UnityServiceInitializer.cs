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

#if UNITY_EDITOR || DEBUG
        public const string k_Environment = "development";
#else
        public const string k_Environment = "production";
#endif

        public void Awake()
        {
            if (Instance && Instance != this) return;
            Instance = this;
        }
        

        public async Task Initialize(string externalPlayerID)
        {
            var serviceProfileName =
                "default"; //note: by using "default" UGS automatically assign a different Profile name to every MPPM virtual player.
#if UNITY_EDITOR && HAS_PARRELSYNC
            if (ParrelSync.ClonesManager.IsClone())
            {
                serviceProfileName = "CloneProfile";
            }
#endif
            if (!string.IsNullOrEmpty(externalPlayerID)) UnityServices.ExternalUserId = externalPlayerID;

            Debug.Log($"Initializing services with externalPlayerID: {externalPlayerID}");
            var signedIn = await UnityServiceAuthenticator.TrySignInAsync(k_Environment, serviceProfileName);
            MetagameApplication.Instance.Broadcast(new PlayerSignedIn(signedIn, UnityServiceAuthenticator.PlayerId));
            if (!signedIn) return;
            //TODO: handle sign in error
            if (externalPlayerID != k_ServerID) InitializeClientOnlyServices();
        }

        private void InitializeClientOnlyServices()
        {
            
        }
    }
}