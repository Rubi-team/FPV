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
            RuntimeManager.StudioSystem.setParameterByName("Surface", index);
            if (Controller._input.sprint)
                RuntimeManager.StudioSystem.setParameterByName("WalkState", 1);
            else
                RuntimeManager.StudioSystem.setParameterByName("WalkState", 0);

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
                Debug.Log("Jump with parameter " +
                          jumpEmitter.EventInstance.getParameterByName("JumpType", out var value) + $" {value}");
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

                Debug.Log("Land with parameter " +
                          jumpEmitter.EventInstance.getParameterByName("JumpType", out var value) + $" {value}");

                jumping = false;

                // 2. APPELER LE SON
                jumpEmitter.Play();
            }
        }
    }
}