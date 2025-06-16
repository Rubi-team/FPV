using System;
using System.Threading.Tasks;
using FMODUnity;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Vivox;
using Unity.Services.Vivox.AudioTaps;
using UnityEngine;

namespace FPV.Runtime
{
    public class VOIP : NetworkBehaviour
    {
        public GameObject myVOIPObject;
        public GameObject mateVOIPObject;

        private float loudness;
        private float mateLoudness;

        private string participantName;

        [SerializeField] private StudioEventEmitter audioSource;

        private MyVOIP myVOIP;

        private bool isOwner;

        public override void OnNetworkSpawn()
        {
            Init();
        }


        private void Init()
        {
            tap = GetComponentInChildren<VivoxParticipantTap>(true);
            audioSource = GetComponentInChildren<StudioEventEmitter>(true);
            myVOIP = GetComponentInChildren<MyVOIP>(true);

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

                // Demande le nom au serveur
                //GetPlayerAuthIDRpc();
            }
        }

        [Rpc(SendTo.Owner)]
        private void GetPlayerAuthIDRpc()
        {
            SendParticipantNameClientRpc(AuthenticationService.Instance.PlayerId);
        }

        [Rpc(SendTo.NotMe)]
        private void SendParticipantNameClientRpc(string participantName)
        {
            this.participantName = participantName;

            //TrySetupTap();
        }


        private void TrySetupTap()
        {
            tap.ChannelName = RelayManager.JoinCode;
            tap.ParticipantName = null;
            tap.ParticipantName = participantName;

            tap.AutoAcquireChannel = true;
        }

        private VivoxParticipantTap tap;

        private void Update()
        {
            // If not connected to a vivox channel, return TODO

            //if (!tap.IsRunning) TrySetupTap();


            if (!isOwner)
            {
                // update the attenuation range of the fmod audio source based on the mate loudness
                audioSource.OverrideMaxDistance = Mathf.Lerp(10, 200, mateLoudness);
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