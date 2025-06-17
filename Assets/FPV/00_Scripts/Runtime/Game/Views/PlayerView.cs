using System;
using System.Collections;
using Audio;
using FMODUnity;
using FPV.Runtime;
using Unity.Cinemachine;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Rendering;

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
        
        private float stressValue;
        private float distanceToMenace;


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
                        AudioManager.Instance.PlayOneShot(AudioManager.Instance.runConcreteFootStep, Feet.position, NetworkManager.Singleton.LocalClientId, 15);
                    else AudioManager.Instance.PlayOneShot(AudioManager.Instance.walkConcreteFootStep, Feet.position, NetworkManager.Singleton.LocalClientId, 5);
                    break;
                case 1:
                    if (Controller._input.sprint)
                        AudioManager.Instance.PlayOneShot(AudioManager.Instance.runWoodFootStep, Feet.position, NetworkManager.Singleton.LocalClientId, 20);
                    else AudioManager.Instance.PlayOneShot(AudioManager.Instance.walkWoodFootStep, Feet.position, NetworkManager.Singleton.LocalClientId, 10);
                    break;
                case 2:
                    if (Controller._input.sprint)
                        AudioManager.Instance.PlayOneShot(AudioManager.Instance.runCarpetFootStep, Feet.position, NetworkManager.Singleton.LocalClientId, 15);
                    else AudioManager.Instance.PlayOneShot(AudioManager.Instance.walkCarpetFootStep, Feet.position, NetworkManager.Singleton.LocalClientId, 5);
                    break;
                case 3:
                    if (Controller._input.sprint)
                        AudioManager.Instance.PlayOneShot(AudioManager.Instance.runMetalFootstep, Feet.position, NetworkManager.Singleton.LocalClientId, 15);
                    else AudioManager.Instance.PlayOneShot(AudioManager.Instance.walkMetalFootstep, Feet.position, NetworkManager.Singleton.LocalClientId, 5);
                    break;
                default:
                    if (Controller._input.sprint)
                        AudioManager.Instance.PlayOneShot(AudioManager.Instance.runConcreteFootStep, Feet.position, NetworkManager.Singleton.LocalClientId, 15);
                    else AudioManager.Instance.PlayOneShot(AudioManager.Instance.walkConcreteFootStep, Feet.position, NetworkManager.Singleton.LocalClientId, 5);
                    break;
            }
        }

        private float LastJumpTime = 0f;

        private void Update()
        {
            if (!App.IsOwner) return;

            if (Controller._input.jump && !jumping && Time.time - LastJumpTime > 0.2f) PlayJumpLandSound(true);

            if (!Model.Grounded) jumping = true;

            if (jumping && Model.Grounded && Time.time - LastJumpTime > 0.2f) PlayJumpLandSound(false);
        }


        private void FixedUpdate()
        {
            if (!App.IsOwner) return;

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
            
            //Menace & Stress
            if (Menace.Instance)
            {
                //CALCULER STRESS VALUE
                distanceToMenace = Vector3.Distance(App.transform.position, Menace.Instance.transform.position);
                if (distanceToMenace < 25)
                {
                    stressValue = 1 - (distanceToMenace / 25);
                }
                else if (stressValue != 0)
                {
                    stressValue = 0;
                }

                RuntimeManager.StudioSystem.setParameterByName("Stress", stressValue);
            }

            SetAnimatorVariables();
        }

        public void PlayJumpLandSound(bool isJump)
        {
            if (!App.IsOwner) return;
            AudioManager.Instance.PlayOneShot(isJump ? AudioManager.Instance.jump : AudioManager.Instance.land, Feet.position, NetworkManager.Singleton.LocalClientId, 10);
            jumping = false;
            LastJumpTime = Time.time;

            if (isJump) SetAnimatorTrigger("IsJumping");
        }

        public void PlaySoundOnFurbyPickedUp()
        {
            if (!App.IsOwner) return;
            AudioManager.Instance.PlayOneShot(AudioManager.Instance.grabItem, Feet.position, NetworkManager.Singleton.LocalClientId, 8);
            AudioManager.Instance.PlayOneShot(AudioManager.Instance.furbyGrab, Feet.position, NetworkManager.Singleton.LocalClientId, 10);
        }

        public void PlaySoundOnPlayerPickedUp()
        {
            if (!App.IsOwner) return;
            AudioManager.Instance.PlayOneShot(AudioManager.Instance.grabPlayer, Body.position, NetworkManager.Singleton.LocalClientId, 8);
        }

        public void PlaySoundOnFurbyThrown()
        {
            if (!App.IsOwner) return;
            AudioManager.Instance.PlayOneShot(AudioManager.Instance.throwItem, Body.position, NetworkManager.Singleton.LocalClientId, 8);
        }

        public void PlaySoundOnPlayerThrown()
        {
            if (!App.IsOwner) return;
            AudioManager.Instance.PlayOneShot(AudioManager.Instance.throwPlayer, Body.position, NetworkManager.Singleton.LocalClientId, 10);
        }
        
        public void SetAnimatorVariables()
        {
            if (!App.IsOwner) return;
            
            if (_animator != null)
            {
                _animator.SetFloat("Speed", Controller._controller.velocity.magnitude);
                _animator.SetBool("IsSprinting", Controller._input.sprint);
                _animator.SetBool("IsGrounded", Model.Grounded);
                SetAnimatorBoolRpc("IsGrounded", Model.Grounded);
                SetAnimatorBoolRpc("IsSprinting", Controller._input.sprint);
                SetAnimatorFloatRpc("Speed", Controller._controller.velocity.magnitude);
            }
        }
        
        public void SetAnimatorTrigger(string triggerName)
        {
            if (!App.IsOwner) return;
            
            if (_animator != null) _animator.SetTrigger(triggerName);
        }
        
        [Rpc(SendTo.NotMe)]
        public void SetAnimatorBoolRpc(string boolName, bool value)
        {
            if (_animator != null) _animator.SetBool(boolName, value);
        }
        
        [Rpc(SendTo.NotMe)]
        public void SetAnimatorFloatRpc(string floatName, float value)
        {
            if (_animator != null) _animator.SetFloat(floatName, value);
        }
        
        [Rpc(SendTo.NotMe)]
        private void SetAnimatorTriggerRpc(string triggerName)
        {
            if (_animator != null) 
                _animator.SetTrigger(triggerName);
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

        [SerializeField] private GameObject SlowPostProcess;
        
        
        internal void EnablePostProcessSlow()
        {
            if (!App.IsOwner) return;

            if (SlowPostProcess != null)
            {
                SlowPostProcess.SetActive(true);
                StartCoroutine(FadePostProcessWeight(SlowPostProcess, 0f, 1f, 0.2f));
                Invoke(nameof(DisablePostProcessSlow), 5f);
            }
        }

        private void DisablePostProcessSlow()
        {
            if (!App.IsOwner) return;

            if (SlowPostProcess != null)
            {
                StartCoroutine(FadePostProcessWeight(SlowPostProcess, 1f, 0f, 1f));
            }
        }

        private IEnumerator FadePostProcessWeight(GameObject obj, float from, float to, float duration)
        {
            var volume = obj.GetComponent<Volume>();
            if (volume == null) yield break;

            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                volume.weight = Mathf.Lerp(from, to, t);
                yield return null;
            }

            volume.weight = to;

            if (to == 0f)
                obj.SetActive(false);
        }
        
        [Rpc(SendTo.Everyone)]
        public void CallAlarmPostProcessPulseRpc(float duration, float pulseDuration)
        {
            StartCoroutine(AlarmPostProcessPulse(duration, pulseDuration));
        }
        
        
        private IEnumerator AlarmPostProcessPulse(float duration, float pulseDuration)
        {
            var volume = Menace.Instance.AlarmeVolume;
            if (volume == null) yield break;

            float timer = 0f;
            while (timer < duration)
            {
                float halfPulse = pulseDuration / 2f;

                // Fade in
                float t = 0f;
                while (t < halfPulse)
                {
                    t += Time.deltaTime;
                    volume.weight = Mathf.Lerp(0f, 1f, t / halfPulse);
                    yield return null;
                }

                // Fade out
                t = 0f;
                while (t < halfPulse)
                {
                    t += Time.deltaTime;
                    volume.weight = Mathf.Lerp(1f, 0f, t / halfPulse);
                    yield return null;
                }

                timer += pulseDuration;
            }

            volume.weight = 0f; // assure que l'effet est désactivé à la fin
        }

    }
}