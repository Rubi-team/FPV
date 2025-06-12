using System.Collections;
using FPV.Runtime;
using FPV.Runtime.Shared;
using Unity.Netcode;
using UnityEngine;

namespace FPV
{
    public class Sonographe : NetworkBehaviour
    {
        [Header("Detection Settings")] 
        [SerializeField] private float detectionRadius = 5f;
        [SerializeField] private float checkInterval = 0.1f;
        [SerializeField] private LayerMask detectionLayers;

        [Header("Activation Settings")]
        [SerializeField] private float activationDuration = 5f;
        [SerializeField] private GameObject[] doorsToActivate;
        [SerializeField] private Laser[] lasersToDeactivate;

        private NetworkObject netObject;
        private float lastCheckTime;
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
            if (!IsServer) return;

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
                var voip = hitColliders[i].GetComponentInChildren<MyVOIP>();
                if (voip != null && voip.GetLoudnessFromMicrophone() > 0.1f)
                {
                    ActivateSonographe();
                    break; // On sort de la boucle dès qu'on trouve un joueur qui fait du bruit
                }
            }
        }

        private void ActivateSonographe()
        {
            ActivateClientRpc();
            StartCoroutine(ExecuteActivationSequence());
        }

        /// <summary>
        /// RPC déclenchée sur tous les clients pour des effets visuels ou sonores.
        /// </summary>
        [ClientRpc]
        private void ActivateClientRpc()
        {
            Debug.LogWarning("Sonographe activated!");
            // Ajoute ici des effets visuels ou sonores pour l'activation.
        }

        /// <summary>
        /// Désactive les lasers et active les portes, puis les réactive après un délai.
        /// </summary>
        private IEnumerator ExecuteActivationSequence()
        {
            // Désactive les lasers et active les portes
            if (lasersToDeactivate != null)
            {
                foreach (var laser in lasersToDeactivate)
                {
                    if (laser != null)
                        laser.isActive = false;
                }
            }

            if (doorsToActivate != null)
            {
                foreach (var door in doorsToActivate)
                {
                    if (door != null)
                        door.GetComponent<Door>().TriggerDoorAnimation();
                }
            }

            // Attends un temps donné avant de réactiver les lasers
            yield return new WaitForSeconds(activationDuration);

            if (lasersToDeactivate != null)
            {
                foreach (var laser in lasersToDeactivate)
                {
                    if (laser != null)
                        laser.isActive = true;
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            // Visualisation de la zone de détection dans l'éditeur
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, detectionRadius);
        }
    }
}