using System.Collections;
using FPV.Runtime;
using FPV.Runtime.Shared;
using Unity.Netcode;
using UnityEngine;

namespace FPV
{
    public class Sonographe : NetworkBehaviour
    {
        [Header("Detection Settings")] [SerializeField]
        private float detectionRadius = 5f;

        [SerializeField] private float checkInterval = 0.1f;
        [SerializeField] private LayerMask detectionLayers;

        [Header("Activation Settings")] [SerializeField]
        private float activationDuration = 5f;

        [SerializeField] private float activationCooldown = 10f; // Temps de cooldown avant une nouvelle activation

        [SerializeField] private GameObject[] doorsToActivate;
        [SerializeField] private Laser[] lasersToDeactivate;
        [SerializeField] private LaserEmitter[] laserEmittersToDeactivate;

        private NetworkObject netObject;
        private float lastCheckTime;
        private float lastActivationTime = -Mathf.Infinity; // Dernière activation initialisée à un temps "infini"
        private readonly Collider[] hitColliders = new Collider[10]; // Buffer pour les résultats du SphereCheck

        private void Awake()
        {
            CustomNetworkManager.Singleton.OnServerPrepareGame += Init;
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            if (!IsServer) enabled = false;
        }

        public void Init()
        {
            netObject = GetComponent<NetworkObject>();

            if (!IsHost)
            {
                Debug.LogError("Sonographe must be spawned on the server.");
                Destroy(gameObject);
                return;
            }

            if (!netObject.IsSpawned) netObject.Spawn();
        }

        private void Update()
        {
            // Système de tick pour ne pas vérifier à chaque frame
            if (Time.time - lastCheckTime < checkInterval) return;

            lastCheckTime = Time.time;
            CheckForNoisyPlayers();
        }

        private void CheckForNoisyPlayers()
        {
            // Effectue un SphereCheck pour détecter les colliders à proximité
            var numColliders =
                Physics.OverlapSphereNonAlloc(transform.position, detectionRadius, hitColliders, detectionLayers);

            for (var i = 0; i < numColliders; i++)
            {
                var voip = hitColliders[i].GetComponent<PlayerApplication>();
                if (voip != null && voip.CurrentLoudness > 0.2f)
                {
                    ActivateSonographe();
                    break; // On sort de la boucle dès qu'on trouve un joueur qui fait du bruit
                }
            }
        }

        public void ActivateSonographe()
        {
            // Vérifie si le cooldown est terminé avant d'activer
            if (Time.time - lastActivationTime < activationCooldown) return;

            lastActivationTime = Time.time; // Met à jour le temps de la dernière activation
            ActivateClientRpc();
        }

        /// <summary>
        /// RPC déclenchée sur tous les clients pour des effets visuels ou sonores.
        /// </summary>
        [ClientRpc]
        private void ActivateClientRpc()
        {
            Debug.LogWarning("Sonographe activated!");
            StartCoroutine(ExecuteActivationSequence());
        }

        /// <summary>
        /// Désactive les lasers et active les portes, puis les réactive après un délai.
        /// </summary>
        private IEnumerator ExecuteActivationSequence()
        {
            // Désactive les lasers et active les portes
            if (lasersToDeactivate != null)
                foreach (var laser in lasersToDeactivate)
                    if (laser != null)
                        laser.DeactivateLaser();

            if (laserEmittersToDeactivate != null)
                foreach (var laserEmitter in laserEmittersToDeactivate)
                    if (laserEmitter != null)
                        laserEmitter.DeactivateLaser();

            if (doorsToActivate != null)
                foreach (var door in doorsToActivate)
                    if (door != null)
                        door.GetComponent<Door>().TriggerDoor();

            // Attends un temps donné avant de réactiver les lasers
            yield return new WaitForSeconds(activationDuration);

            if (lasersToDeactivate != null)
                foreach (var laser in lasersToDeactivate)
                    if (laser != null)
                        laser.ReactivateLaser();

            if (laserEmittersToDeactivate != null)
                foreach (var laserEmitter in laserEmittersToDeactivate)
                    if (laserEmitter != null)
                        laserEmitter.ReactivateLaser();
        }

        private void OnDrawGizmosSelected()
        {
            // Visualisation de la zone de détection dans l'éditeur
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, detectionRadius);
        }
    }
}