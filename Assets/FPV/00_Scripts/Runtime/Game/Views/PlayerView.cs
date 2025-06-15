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


        [SerializeField] internal Transform Feet;
        [SerializeField] internal Transform Body;
        [SerializeField] internal Transform AboveHead;
        [SerializeField] private bool jumping;

        [SerializeField] private Transform headTarget;
        [SerializeField] private Animator _animator;


        public void AudioFootsteps()
        {
            var index = (int)currentGroundType.groundType;
            if (currentGroundType == null) index = 0; // Default to 0 if no ground type is set

            // switch case of index 
            switch (index)
            {
                case 0:
                    AudioManager.Instance.PlayOneShot(AudioManager.concreteFootStep, Feet.position);
                    break;
                case 1:
                    AudioManager.Instance.PlayOneShot(AudioManager.carpetFootStep, Feet.position);
                    break;
                case 2:
                    AudioManager.Instance.PlayOneShot(AudioManager.woodFootStep, Feet.position);
                    break;
                case 3:
                    AudioManager.Instance.PlayOneShot(AudioManager.metalFootstep, Feet.position);
                    break;
                default:
                    AudioManager.Instance.PlayOneShot(AudioManager.footStep, Feet.position);
                    break;
            }
        }

        private float LastJumpTime = 0f;

        private void Update()
        {
            if (!IsOwner) return;

            if (Controller._input.jump && !jumping) PlayJumpLandSound(true);

            if (!Model.Grounded) jumping = true;

            if (jumping && Model.Grounded && Time.time - LastJumpTime > 0.2f) PlayJumpLandSound(false);
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

        public void PlayJumpLandSound(bool isJump)
        {
            AudioManager.Instance.PlayOneShot(isJump ? AudioManager.jump : AudioManager.land, Feet.position);
        }

        public void PlaySoundOnFurbyPickedUp()
        {
            AudioManager.Instance.PlayOneShot(AudioManager.grabItem, Feet.position);
        }

        public void PlaySoundOnPlayerPickedUp()
        {
            AudioManager.Instance.PlayOneShot(AudioManager.grabPlayer, Body.position);
        }

        public void PlaySoundOnFurbyThrown()
        {
            AudioManager.Instance.PlayOneShot(AudioManager.throwItem, Body.position);
        }

        public void PlaySoundOnPlayerThrown()
        {
            AudioManager.Instance.PlayOneShot(AudioManager.throwPlayer, Body.position);
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