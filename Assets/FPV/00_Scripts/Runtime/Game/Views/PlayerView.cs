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

        [Header("Emotes")] [SerializeField] private GameObject emote1;
        [SerializeField] private GameObject emote2;
        [SerializeField] private GameObject emote3;
        [SerializeField] private GameObject emote4;
        [SerializeField] private GameObject emote5;
        [SerializeField] private GameObject emote6;
        [SerializeField] private GameObject emote7;
        [SerializeField] private GameObject emote8;
        [SerializeField] private GameObject emote9;

        [SerializeField] private GameObject trailEffect;


        public void AudioFootsteps()
        {
            if (!App.IsOwner) return;

            if (currentGroundType == null)
            {
                AudioManager.Instance.PlayOneShot(AudioManager.Instance.runConcreteFootStep, Feet.position, NetworkManager.Singleton.LocalClientId, 5);
                return;
            }

            var index = (int)currentGroundType.groundType;
            if (currentGroundType == null) index = 0; // Default to 0 if no ground type is set

            // if controller input move is zero we return 
            if (Controller._input.move == Vector2.zero)
                // If the player is not moving, we don't play any footsteps sound
                return;

            // switch case of index 
            switch (index)
            {
                case 0:
                    if (Controller._input.sprint)
                        AudioManager.Instance.PlayOneShot(AudioManager.Instance.runConcreteFootStep, Feet.position, NetworkManager.Singleton.LocalClientId, 12);
                    else AudioManager.Instance.PlayOneShot(AudioManager.Instance.walkConcreteFootStep, Feet.position, NetworkManager.Singleton.LocalClientId, 5);
                    break;
                case 1:
                    if (Controller._input.sprint)
                        AudioManager.Instance.PlayOneShot(AudioManager.Instance.runWoodFootStep, Feet.position, NetworkManager.Singleton.LocalClientId, 30);
                    else AudioManager.Instance.PlayOneShot(AudioManager.Instance.walkWoodFootStep, Feet.position, NetworkManager.Singleton.LocalClientId, 15);
                    break;
                case 2:
                    if (Controller._input.sprint)
                        AudioManager.Instance.PlayOneShot(AudioManager.Instance.runCarpetFootStep, Feet.position, NetworkManager.Singleton.LocalClientId, 12);
                    else AudioManager.Instance.PlayOneShot(AudioManager.Instance.walkCarpetFootStep, Feet.position, NetworkManager.Singleton.LocalClientId, 5);
                    break;
                case 3:
                    if (Controller._input.sprint)
                        AudioManager.Instance.PlayOneShot(AudioManager.Instance.runMetalFootstep, Feet.position, NetworkManager.Singleton.LocalClientId, 12);
                    else AudioManager.Instance.PlayOneShot(AudioManager.Instance.walkMetalFootstep, Feet.position, NetworkManager.Singleton.LocalClientId, 5);
                    break;
                default:
                    if (Controller._input.sprint)
                        AudioManager.Instance.PlayOneShot(AudioManager.Instance.runConcreteFootStep, Feet.position, NetworkManager.Singleton.LocalClientId, 12);
                    else AudioManager.Instance.PlayOneShot(AudioManager.Instance.walkConcreteFootStep, Feet.position, NetworkManager.Singleton.LocalClientId, 5);
                    break;
            }
        }

        private float LastJumpTime = 0f;

        private void Update()
        {
            if (!IsOwner) return;

            if (Controller._input.jump && !jumping && Time.time - LastJumpTime > 0.2f) PlayJumpLandSound(true);

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
            AudioManager.Instance.PlayOneShot(isJump ? AudioManager.Instance.jump : AudioManager.Instance.land, Feet.position, NetworkManager.Singleton.LocalClientId, 10);
            jumping = false;
            LastJumpTime = Time.time;

            if (isJump) SetAnimatorTriggerRpc("IsJumping");
        }

        public void PlaySoundOnFurbyPickedUp()
        {
            AudioManager.Instance.PlayOneShot(AudioManager.Instance.grabItem, Feet.position, NetworkManager.Singleton.LocalClientId, 8);
            AudioManager.Instance.PlayOneShot(AudioManager.Instance.furbyGrab, Feet.position, NetworkManager.Singleton.LocalClientId, 10);
        }

        public void PlaySoundOnPlayerPickedUp()
        {
            AudioManager.Instance.PlayOneShot(AudioManager.Instance.grabPlayer, Body.position, NetworkManager.Singleton.LocalClientId, 8);
        }

        public void PlaySoundOnFurbyThrown()
        {
            AudioManager.Instance.PlayOneShot(AudioManager.Instance.throwItem, Body.position, NetworkManager.Singleton.LocalClientId, 8);
        }

        public void PlaySoundOnPlayerThrown()
        {
            AudioManager.Instance.PlayOneShot(AudioManager.Instance.throwPlayer, Body.position, NetworkManager.Singleton.LocalClientId, 10);
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

        // Play Emote, instantiate gameobject above App.transform and destroy it after 3 seconds
        [Rpc(SendTo.Everyone)]
        public void PlayEmoteRpc(int emoteIndex)
        {
            GameObject emote = null;
            switch (emoteIndex)
            {
                case 1: emote = emote1; break;
                case 2: emote = emote2; break;
                case 3: emote = emote3; break;
                case 4: emote = emote4; break;
                case 5: emote = emote5; break;
                case 6: emote = emote6; break;
                case 7: emote = emote7; break;
                case 8: emote = emote8; break;
                case 9: emote = emote9; break;
            }

            if (emote != null)
            {
                var instance = Instantiate(emote, AboveHead.position, Quaternion.identity);
                instance.transform.SetParent(AboveHead);
                Destroy(instance, 3f);
            }
        }

        [Rpc(SendTo.Everyone)]
        public void AddTrailEffectRpc()
        {
            if (trailEffect != null)
            {
                var instance = Instantiate(trailEffect, Body.position, Quaternion.identity);
                instance.transform.SetParent(Body);
                Destroy(instance, 3f);
            }
        }
    }
}