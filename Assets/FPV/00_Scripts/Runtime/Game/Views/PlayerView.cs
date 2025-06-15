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
        [SerializeField] private bool jumping;

        [SerializeField] private Transform headTarget;
        [SerializeField] private Animator _animator;


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


        private void FixedUpdate()
        {
            if (!IsOwner) return;

            // Créer un layerMask qui ignore le layer Player
            var playerLayer = LayerMask.NameToLayer("Player");
            var layerMask = ~(1 << playerLayer);

            // Commencer le raycast 1 unité plus loin et appliquer le layerMask
            var startPosition = MainCamera.Instance.transform.position + MainCamera.Instance.transform.forward * 1f;
            var ray = new Ray(startPosition, MainCamera.Instance.transform.forward);

            // Déclarer les variables nécessaires
            RaycastHit hit;
            var distance = 100f; // Ou la distance que vous voulez

            // Faire le raycast avec les bonnes variables
            if (Physics.Raycast(ray, out hit, distance, layerMask)) headTarget.position = hit.point;

            SetAnimatorVariablesRpc();
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
                //jumpEmitter.SetParameter("JumpType", 0);
                jumping = true;
                LastJumpTime = Time.time;
                SetAnimatorTriggerRpc("IsJumping");
            }
            else
            {
                //LAND
                //EVENTREF = Land
                RuntimeManager.StudioSystem.setParameterByName("JumpType", 1);
                //jumpEmitter.SetParameter("JumpType", 1);
                jumping = false;
                SetAnimatorTriggerRpc("IsLanding");
            }

            // 2. APPELER LE SON
            jumpEmitter.Play();
        }

        [Rpc(SendTo.Everyone)]
        public void PlayActionSoundRpc()
        {
            if (actionEmitter != null)
            {
            }
        }

        [Rpc(SendTo.Everyone)]
        public void SetAnimatorVariablesRpc()
        {
            if (_animator != null)
            {
                _animator.SetFloat("Speed", Controller._controller.velocity.magnitude);
                _animator.SetBool("IsSprinting", Controller._input.sprint);
                _animator.SetBool("IsGrounded", Model.Grounded);
            }
        }

        [Rpc(SendTo.Everyone)]
        public void SetAnimatorTriggerRpc(string triggerName)
        {
            if (_animator != null) _animator.SetTrigger(triggerName);
        }
    }
}