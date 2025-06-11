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


        private async Task Init()
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

                // Demande le nom au serveur
                GetPlayerAuthIDRpc();
            }

            audioSource = GetComponentInChildren<StudioEventEmitter>();
            myVOIP = GetComponentInChildren<MyVOIP>();
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
            var tap = GetComponent<VivoxParticipantTap>();
            tap.ChannelName = RelayManager.JoinCode;
            tap.ParticipantName = participantName;
        }


        [Rpc(SendTo.Owner)]
        private Task<string> GetParticipantNameServerRpc()
        {
            // Get AuthenticationService.Instance.PlayerId;
            return Task.FromResult(AuthenticationService.Instance.PlayerId);
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