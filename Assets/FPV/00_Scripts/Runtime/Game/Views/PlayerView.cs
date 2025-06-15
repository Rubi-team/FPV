using System;
using Audio;
using FMODUnity;
using FPV.Runtime;
using Unity.Cinemachine;
using Unity.Netcode;
using UnityEngine;

namespace FPV.Runtime
{
    public class PlayerView : NetworkView<PlayerApplication>
    {
        private PlayerModel Model => App.Model;
        private PlayerController Controller => App.Controller;

        internal GroundType currentGroundType;

        [Header("Audio Settings")] [SerializeField]
        internal StudioEventEmitter footEmitter;

        [SerializeField] internal StudioEventEmitter jumpEmitter;
        [SerializeField] internal StudioEventEmitter actionEmitter;
        [SerializeField] internal StudioEventEmitter ambienceEmitter;
        [SerializeField] private bool SPRINT = false;
        [SerializeField] private bool jumping;


        public void AudioFootsteps()
        {
            var index = (int)currentGroundType.groundType;
            if (currentGroundType == null) index = 0; // Default to 0 if no ground type is set
            RuntimeManager.StudioSystem.setParameterByName("Surface", index);
            RuntimeManager.StudioSystem.setParameterByName("WalkState", Controller._input.sprint ? 1 : 0);

            footEmitter.Play();
        }

        private float LastJumpTime = 0f;

        private void Update()
        {
            if (!IsOwner) return;

            if (Controller._input.jump && !jumping) PlayJumpLandSoundRpc(true);

            if (!Model.Grounded) jumping = true;

            if (jumping && Model.Grounded && Time.time - LastJumpTime > 0.2f) PlayJumpLandSoundRpc(false);
        }

        [Rpc(SendTo.Everyone)]
        public void PlayJumpLandSoundRpc(bool isJump)
        {
            if (isJump)
            {
                // 1. CHANGER LE TYPE D'ACTION
                //JUMP
                //EVENTREF = JumpLand
                RuntimeManager.StudioSystem.setParameterByName("JumpType", 0);
                jumping = true;
                LastJumpTime = Time.time;
            }
            else
            {
                //LAND
                //EVENTREF = Land
                RuntimeManager.StudioSystem.setParameterByName("JumpType", 1);
                jumping = false;
            }

            // 2. APPELER LE SON
            jumpEmitter.Play();
        }
    }
}