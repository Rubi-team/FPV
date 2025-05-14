using System;
using SteamAudio;
using Unity.Netcode;
using UnityEngine;

namespace FPV
{
    public class VOIP : NetworkBehaviour
    {
        public GameObject myVOIPObject;
        public GameObject mateVOIPObject;

        public float loudness;

        [SerializeField] private FMODStudioAudioEngineSource audioSource;
        
        private MyVOIP myVOIP;

        private bool isOwner;

        public override void OnNetworkSpawn()
        {
            if (GetComponentInParent<PlayerApplication>().IsOwner)
            {
                isOwner = true;
                myVOIPObject.SetActive(true);
                mateVOIPObject.SetActive(false);
            }
            else
            {
                isOwner = false;
                myVOIPObject.SetActive(false);
                mateVOIPObject.SetActive(true);
            }

            audioSource = GetComponentInChildren<FMODStudioAudioEngineSource>();
            myVOIP = GetComponentInChildren<MyVOIP>();
        }

        private void Update()
        {
            if (!isOwner)
                return;
            loudness = myVOIP.GetLoudnessFromMicrophone();
        }

        [Rpc(SendTo.NotMe)]
        private void UpdateVOIPClientRpc(float loudness)
        {
            
        }
    }
}