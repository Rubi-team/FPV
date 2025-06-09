using System;
using FMODUnity;
using Unity.Netcode;
using Unity.Services.Vivox;
using UnityEngine;

namespace FPV.Runtime
{
    public class VOIP : NetworkBehaviour
    {
        public GameObject myVOIPObject;
        public GameObject mateVOIPObject;

        private float loudness;
        private float mateLoudness;

        [SerializeField] private StudioEventEmitter audioSource;

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

            audioSource = GetComponentInChildren<StudioEventEmitter>();
            myVOIP = GetComponentInChildren<MyVOIP>();
        }

        private void Update()
        {
            // If not connected to a vivox channel, return TODO


            if (!isOwner)
            {
                // update the attenuation range of the fmod audio source based on the mate loudness
                audioSource.OverrideMaxDistance = Mathf.Lerp(0, 100, mateLoudness);
                return;
            }

            loudness = myVOIP.GetLoudnessFromMicrophone();
            UpdateVOIPClientRpc(loudness);
        }

        [Rpc(SendTo.NotMe)]
        private void UpdateVOIPClientRpc(float loudness)
        {
            mateLoudness = loudness;
        }
    }
}