using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FPV.Runtime
{
    public class Laser : MonoBehaviour
    {
#if UNITY_EDITOR
        [Header("Editor Settings")] public GameObject beamPrefab;
        [Range(1, 10)] public int editorLaserCount = 1;

        public bool useSpacing = false;
        public float beamSpacing = 0.5f;
        public Vector3 spacingAxis = Vector3.forward;
#endif

        [Header("Laser Settings")] public bool isRotating = false;
        public float rotationSpeed = 30f;
        public Transform rotationPivot;

        [Header("Activation Settings")]
        [Tooltip("Indique si le laser nécessite deux désactivations pour être complètement désactivé.")]
        [SerializeField]
        private bool requiresTwoDeactivations = false; // Besoin des doubles désactivations

        private int deactivationAttempts = 0; // Compteur des désactivations
        private float lastDeactivationAttemptTime = -Mathf.Infinity; // Dernier essai de désactivation

        [Header("Detection Settings")] public LayerMask obstacleLayers;
        public LayerMask detectionLayers;

        [Header("Performance Settings")] public float checkInterval = 0.1f;
        public float visibilityCheckInterval = 0.5f;
        public bool debugDrawRay = true;

        [Header("Slow Down Settings")] public float slowDownFactor = 2.0f; // Divides the movement speed by this factor
        public float slowDownDuration = 3.0f; // Duration of the slow down effect

        private float lastCheckTime = 0f;
        private float lastVisibilityCheck = 0f;
        private bool isVisible = true;

        private RaycastHit[] raycastHits = new RaycastHit[1];
        private List<LineRenderer> lineRenderers = new();
        private Dictionary<GameObject, float> cooldowns = new(); // Track cooldown per player

        [Tooltip("Détermine si le laser est activé.")]
        public bool isActive = true;

        private void Start()
        {
            foreach (Transform child in transform)
            {
                var lr = child.GetComponentInChildren<LineRenderer>();
                if (lr != null)
                    lineRenderers.Add(lr);
            }
        }

        private void Update()
        {
            if (!isActive)
            {
                foreach (var lr in lineRenderers)
                    if (lr != null)
                        lr.enabled = false;
                return;
            }

            foreach (var lr in lineRenderers)
                if (lr != null)
                    lr.enabled = true;

            if (isRotating && rotationPivot != null)
                transform.RotateAround(rotationPivot.position, Vector3.up, rotationSpeed * Time.deltaTime);

            CheckVisibility();

            if (!isVisible)
                return;

            if (Time.time - lastCheckTime >= checkInterval)
            {
                lastCheckTime = Time.time;
                foreach (var lr in lineRenderers)
                    UpdateLaserBeam(lr);
            }
        }

        private void CheckVisibility()
        {
            if (Time.time - lastVisibilityCheck < visibilityCheckInterval)
                return;

            lastVisibilityCheck = Time.time;
            isVisible = false;

            foreach (var lr in lineRenderers)
            {
                var renderer = lr.GetComponent<Renderer>();
                if (renderer != null && renderer.isVisible)
                {
                    isVisible = true;
                    break;
                }
            }
        }

        private void UpdateLaserBeam(LineRenderer lr)
        {
            var start = lr.transform.position;
            var direction = lr.transform.forward;
            var maxDistance = 100f;

            var ray = new Ray(start, direction);

            if (Physics.Raycast(ray, out var hit, maxDistance, obstacleLayers | detectionLayers))
            {
                maxDistance = hit.distance;

                if (((1 << hit.collider.gameObject.layer) & detectionLayers) != 0)
                    if (hit.collider.TryGetComponent<PlayerApplication>(out var player))
                        HandlePlayerHit(hit.collider.gameObject, player.Model);
            }

            if (debugDrawRay)
                Debug.DrawRay(start, direction * maxDistance, Color.red, checkInterval);

            if (lr != null)
            {
                lr.SetPosition(0, Vector3.zero);
                lr.SetPosition(1, Vector3.forward * maxDistance);
            }
        }

        private void HandlePlayerHit(GameObject playerObject, PlayerModel player)
        {
            if (!cooldowns.ContainsKey(playerObject) || Time.time >= cooldowns[playerObject])
                StartCoroutine(ApplySlowDown(player, playerObject));

            // ALARME SERVER RPC TODO
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
                isActive = false; // Désactivation immédiate si une seule désactivation est nécessaire
                return;
            }

            if (Time.time - lastDeactivationAttemptTime > 1f)
                // Si plus de 1 seconde s'est écoulée depuis le dernier essai, réinitialiser le compteur
                deactivationAttempts = 0;

            deactivationAttempts++;
            lastDeactivationAttemptTime = Time.time; // Met à jour le temps du dernier essai de désactivation

            if (deactivationAttempts >= 2)
                isActive = false; // Désactivation après deux tentatives
        }

        public void ReactivateLaser()
        {
            isActive = true; // Réactive le laser
            deactivationAttempts = 0; // Réinitialise le compteur de désactivation
        }
    }
}