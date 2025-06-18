using System.Collections;
using System.Collections.Generic;
using Audio;
using FMODUnity;
using Unity.Netcode;
using UnityEngine;

namespace FPV.Runtime
{
    public class LaserEmitter : MonoBehaviour
    {
        [Header("Laser Settings")] 
        public LineRenderer lineRenderer;
        public float maxDistance = 100f;
        public LayerMask obstacleLayers;
        public LayerMask detectionLayers;
        public float checkInterval = 0.1f;
        public bool debugDraw = true;

        [Header("Activation Settings")]
        [Tooltip("Indique si le laser nécessite deux désactivations pour être complètement désactivé.")]
        [SerializeField]
        private bool requiresTwoDeactivations = false;

        private int deactivationAttempts = 0;
        private float lastDeactivationAttemptTime = -Mathf.Infinity;

        [Header("Slow Down Settings")] 
        public float slowDownFactor = 2.0f;
        public float slowDownDuration = 3.0f;

        private float lastCheckTime;
        private RaycastHit[] hits = new RaycastHit[1];

        [SerializeField] private StudioEventEmitter emitter;

        // ✅ Dictionnaire pour tracker les coroutines actives par joueur
        private Dictionary<GameObject, Coroutine> activeSlowDowns = new();
        private Dictionary<GameObject, float> cooldowns = new();

        public bool IsActive { get; private set; } = true;

        private void Update()
        {
            if (Time.time - lastCheckTime >= checkInterval)
            {
                if (!IsActive)
                {
                    if (lineRenderer != null)
                    {
                        lineRenderer.enabled = false;
                    }
                    {
                        lineRenderer.enabled = false;
                    }
                    return;
                }

                if (lineRenderer != null && !lineRenderer.enabled)
                {
                    // Si le laser est actif, on l'active
                    lineRenderer.enabled = true;
                }
                lastCheckTime = Time.time;
                UpdateLaser();
            }
        }

        private void UpdateLaser()
        {
            var start = transform.position;
            var dir = transform.forward;

            var distance = maxDistance;
            var ray = new Ray(start, dir);

            if (Physics.Raycast(ray, out var hit, maxDistance, obstacleLayers | detectionLayers))
            {
                distance = hit.distance;

                if (((1 << hit.collider.gameObject.layer) & detectionLayers) != 0)
                    if (hit.collider.TryGetComponent<PlayerApplication>(out var player))
                    {
                        if (player.IsDead.Value)
                        {
                            // Si le joueur est mort, on ignore ce hit
                            return;
                        }
                        HandlePlayerHit(hit.collider.gameObject, player.Model);
                    }
                        
            }

            if (lineRenderer != null)
            {
                lineRenderer.SetPosition(0, Vector3.zero);
                lineRenderer.SetPosition(1, Vector3.forward * distance);
            }

            if (debugDraw)
                Debug.DrawRay(start, dir * distance, Color.red, checkInterval);
        }

        private void HandlePlayerHit(GameObject playerObject, PlayerModel player)
        {
            // ✅ Vérifier si une coroutine de slow est déjà active pour ce joueur
            if (activeSlowDowns.ContainsKey(playerObject) && activeSlowDowns[playerObject] != null)
                return;

            // ✅ Vérifier le cooldown
            if (cooldowns.ContainsKey(playerObject) && Time.time < cooldowns[playerObject])
                return;

            // ✅ Démarrer et tracker la coroutine
            var slowCoroutine = StartCoroutine(ApplySlowDown(player, playerObject));
            activeSlowDowns[playerObject] = slowCoroutine;

            // ALARME RPC TODO
        }

        private IEnumerator ApplySlowDown(PlayerModel player, GameObject playerObject)
        {
            if (player == null || playerObject == null)
            {
                // ✅ Nettoyer le dictionnaire si l'objet n'existe plus
                if (playerObject != null)
                    activeSlowDowns.Remove(playerObject);
                yield break;
            }

            // ✅ Double vérification pour éviter les conflits
            if (player.isSlowDownActive)
            {
                activeSlowDowns.Remove(playerObject);
                yield break;
            }

            // Marquer le joueur comme étant sous l'effet du slow
            player.isSlowDownActive = true;
            
            //Sons
            AudioManager.Instance.PlayOneShot(AudioManager.Instance.laserHit, playerObject.transform.position, NetworkManager.Singleton.LocalClientId, 10000);
            AudioManager.Instance.PlayOneShot(AudioManager.Instance.siren, transform.position, NetworkManager.Singleton.LocalClientId, 10000);
            
            player.View.CallAlarmPostProcessPulseRpc(6.6f, 1.8f);

            // Sauvegarder les vitesses originales
            var originalSpeed = player.MoveSpeed;
            var originalSprintSpeed = player.SprintSpeed;

            // Appliquer le ralentissement
            player.MoveSpeed /= slowDownFactor;
            player.SprintSpeed /= slowDownFactor;
            player.View.EnablePostProcessSlow();

            // Définir le cooldown
            cooldowns[playerObject] = Time.time + slowDownDuration + checkInterval;

            // Attendre la durée du ralentissement
            yield return new WaitForSeconds(slowDownDuration);

            // ✅ Restaurer les vitesses si le joueur existe encore
            if (player != null)
            {
                player.MoveSpeed = originalSpeed;
                player.SprintSpeed = originalSprintSpeed;
                player.isSlowDownActive = false;
            }

            // ✅ Nettoyer le dictionnaire des coroutines actives
            if (playerObject != null)
                activeSlowDowns.Remove(playerObject);
        }

        public void DeactivateLaser()
        {
            if (!requiresTwoDeactivations)
            {
                IsActive = false;
                emitter.Stop();
                return;
            }
            
            if (Time.time - lastDeactivationAttemptTime > 1f)
                deactivationAttempts = 0;

            deactivationAttempts++;
            lastDeactivationAttemptTime = Time.time;

            if (deactivationAttempts >= 2)
                IsActive = false;
            
            emitter.Stop();
        }

        public void ReactivateLaser()
        {
            IsActive = true;
            deactivationAttempts = 0;
            
            emitter.Play();
        }

        // ✅ Nettoyer les références quand l'objet est détruit
        private void OnDestroy()
        {
            // Arrêter toutes les coroutines actives
            foreach (var kvp in activeSlowDowns)
            {
                if (kvp.Value != null)
                    StopCoroutine(kvp.Value);
            }
            activeSlowDowns.Clear();
            cooldowns.Clear();
        }
    }
}