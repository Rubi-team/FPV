using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FPV.Runtime
{
    public class LaserEmitter : MonoBehaviour
    {
        [Header("Laser Settings")] public LineRenderer lineRenderer;
        public float maxDistance = 100f;
        public LayerMask obstacleLayers;
        public LayerMask detectionLayers;
        public float checkInterval = 0.1f;
        public bool debugDraw = true;

        [Header("Activation Settings")]
        [Tooltip("Indique si le laser nécessite deux désactivations pour être complètement désactivé.")]
        [SerializeField]
        private bool requiresTwoDeactivations = false; // Besoin des doubles désactivations

        private int deactivationAttempts = 0; // Compteur des désactivations
        private float lastDeactivationAttemptTime = -Mathf.Infinity; // Dernier essai de désactivation

        [Header("Slow Down Settings")] public float slowDownFactor = 2.0f; // Divides the movement speed by this factor
        public float slowDownDuration = 3.0f; // Duration of the slow down effect

        private float lastCheckTime;
        private RaycastHit[] hits = new RaycastHit[1];

        private Dictionary<GameObject, float> cooldowns = new(); // Keeps track of cooldowns for each player

        public bool IsActive { get; private set; } = true;

        private void Update()
        {
            if (Time.time - lastCheckTime >= checkInterval)
            {
                if (!IsActive)
                {
                    lineRenderer.enabled = false;
                    return;
                }

                lineRenderer.enabled = true;
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
                        HandlePlayerHit(hit.collider.gameObject, player.Model);
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
            if (!cooldowns.ContainsKey(playerObject) || Time.time >= cooldowns[playerObject])
                StartCoroutine(ApplySlowDown(player, playerObject));

            // ALARME RPC TODO
        }

        private IEnumerator ApplySlowDown(PlayerModel player, GameObject playerObject)
        {
            if (player == null || playerObject == null || player.isSlowDownActive)
                yield break;

            // Apply the slow down effect
            var originalSpeed = player.MoveSpeed;
            var originalSprintSpeed = player.SprintSpeed;

            player.MoveSpeed /= slowDownFactor;
            player.SprintSpeed /= slowDownFactor;

            // Set cooldown for this player
            cooldowns[playerObject] = Time.time + slowDownDuration + checkInterval;

            // Wait for the duration
            yield return new WaitForSeconds(slowDownDuration);

            // Reset the speed to its original value
            player.MoveSpeed = originalSpeed;
            player.SprintSpeed = originalSprintSpeed;
            player.isSlowDownActive = false; // Reset the slow down state
        }

        /// <summary>
        /// Méthode pour désactiver le laser.
        /// </summary>
        public void DeactivateLaser()
        {
            if (!requiresTwoDeactivations)
            {
                IsActive = false; // Désactivation immédiate si une seule désactivation est nécessaire.
                return;
            }

            if (Time.time - lastDeactivationAttemptTime > 1f)
                // Si plus de 1 seconde s'est écoulée depuis le dernier essai, réinitialiser le compteur
                deactivationAttempts = 0;

            deactivationAttempts++;
            lastDeactivationAttemptTime = Time.time; // Met à jour le temps du dernier essai de désactivation

            if (deactivationAttempts >= 2)
                IsActive = false; // Désactivation après deux tentatives.
        }

        /// <summary>
        /// Méthode pour réactiver le laser.
        /// </summary>
        public void ReactivateLaser()
        {
            IsActive = true;
            deactivationAttempts = 0; // Réinitialise le compteur de désactivation.
        }
    }
}