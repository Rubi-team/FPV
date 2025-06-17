using System.Collections;
using System.Collections.Generic;
using Audio;
using FMODUnity;
using Unity.Netcode;
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
        private bool requiresTwoDeactivations = false;

        private int deactivationAttempts = 0;
        private float lastDeactivationAttemptTime = -Mathf.Infinity;

        [Header("Detection Settings")] public LayerMask obstacleLayers;
        public LayerMask detectionLayers;

        [Header("Performance Settings")] public float checkInterval = 0.1f;
        public float visibilityCheckInterval = 0.5f;
        public bool debugDrawRay = true;

        [Header("Slow Down Settings")] public float slowDownFactor = 2.0f;
        public float slowDownDuration = 3.0f;

        private float lastCheckTime = 0f;
        private float lastVisibilityCheck = 0f;
        private bool isVisible = true;

        private RaycastHit[] raycastHits = new RaycastHit[1];
        private List<LineRenderer> lineRenderers = new();

        [SerializeField] private StudioEventEmitter emitter;

        // ✅ Dictionnaire pour tracker les coroutines actives par joueur
        private Dictionary<GameObject, Coroutine> activeSlowDowns = new();
        private Dictionary<GameObject, float> cooldowns = new();

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
            // ✅ Vérifier si une coroutine de slow est déjà active pour ce joueur
            if (activeSlowDowns.ContainsKey(playerObject) && activeSlowDowns[playerObject] != null)
                return;

            // ✅ Vérifier le cooldown
            if (cooldowns.ContainsKey(playerObject) && Time.time < cooldowns[playerObject])
                return;

            // ✅ Démarrer et tracker la coroutine
            var slowCoroutine = StartCoroutine(ApplySlowDown(player, playerObject));
            activeSlowDowns[playerObject] = slowCoroutine;

            // ALARME SERVER RPC TODO
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
            if (player.isSlowDownActive) yield return null;

            // ✅ Marquer le joueur comme étant sous l'effet du slow AVANT de modifier les vitesses
            player.isSlowDownActive = true;

            //Sons
            AudioManager.Instance.PlayOneShot(AudioManager.Instance.laserHit, playerObject.transform.position,
                NetworkManager.Singleton.LocalClientId, 10000);
            AudioManager.Instance.PlayOneShot(AudioManager.Instance.siren, transform.position,
                NetworkManager.Singleton.LocalClientId, 10000);


            // Sauvegarder les vitesses originales
            var originalSpeed = player.MoveSpeed;
            var originalSprintSpeed = player.SprintSpeed;

            // Appliquer le ralentissement
            player.MoveSpeed /= slowDownFactor;
            player.SprintSpeed /= slowDownFactor;

            // Définir le cooldown pour ce joueur
            cooldowns[playerObject] = Time.time + slowDownDuration + checkInterval;

            // Attendre la durée du ralentissement
            yield return new WaitForSeconds(slowDownDuration);

            // ✅ Vérifier que le joueur existe encore avant de restaurer les vitesses
            if (player != null)
            {
                // Restaurer les vitesses originales
                player.MoveSpeed = originalSpeed;
                player.SprintSpeed = originalSprintSpeed;
                player.isSlowDownActive = false;
            }

            // ✅ Nettoyer le dictionnaire des coroutines actives
            if (playerObject != null)
                activeSlowDowns.Remove(playerObject);
        }

        /// <summary>
        /// Méthode pour désactiver le laser.
        /// </summary>
        public void DeactivateLaser()
        {
            if (!requiresTwoDeactivations)
            {
                isActive = false;
                emitter.Stop();
                return;
            }


            if (Time.time - lastDeactivationAttemptTime > 1f)
                deactivationAttempts = 0;

            deactivationAttempts++;
            lastDeactivationAttemptTime = Time.time;

            if (deactivationAttempts >= 2)
                isActive = false;

            emitter.Stop();
        }

        public void ReactivateLaser()
        {
            isActive = true;
            deactivationAttempts = 0;

            emitter.Play();
        }

        // ✅ Nettoyer les références quand l'objet est détruit
        private void OnDestroy()
        {
            // Arrêter toutes les coroutines actives
            foreach (var kvp in activeSlowDowns)
                if (kvp.Value != null)
                    StopCoroutine(kvp.Value);
            activeSlowDowns.Clear();
            cooldowns.Clear();
        }
    }
}