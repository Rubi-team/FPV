using System;
using Audio;
using FMODUnity;
using FPV.Runtime;
using Unity.Cinemachine;
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

        private float lastFootstepTime = 0f;

        private void Update()
        {
            if (Controller._input.jump && !jumping)
            {
                // 1. CHANGER LE TYPE D'ACTION
                //JUMP
                //EVENTREF = JumpLand
                RuntimeManager.StudioSystem.setParameterByName("JumpType", 0);
                jumping = true;
                lastFootstepTime = Time.time;

                // 2. APPELER LE SON
                jumpEmitter.Play();
            }

            if (jumping && Model.Grounded && Time.time - lastFootstepTime > 0.2f)
            {
                //LAND
                //EVENTREF = Land
                RuntimeManager.StudioSystem.setParameterByName("JumpType", 1);


                jumping = false;

                // 2. APPELER LE SON
                jumpEmitter.Play();
            }
        }
    }
}