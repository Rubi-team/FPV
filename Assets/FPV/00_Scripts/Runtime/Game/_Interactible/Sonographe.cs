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
                Debug.Log($"Hit {hitColliders[i].name}");
                var voip = hitColliders[i].GetComponentInChildren<MyVOIP>();
                if (voip != null) Debug.Log($"Bruit de {voip.name} : {voip.GetLoudnessFromMicrophone()}");
                if (voip != null && voip.GetLoudnessFromMicrophone() > 0.1f)
                {
                    Active();
                    break; // On sort de la boucle dès qu'on trouve un joueur qui fait du bruit
                }
            }
        }

        [ClientRpc]
        public void ActiveClientRpc()
        {
            Debug.LogWarning("Sonographe activated!");
        }

        public void Active()
        {
            ActiveClientRpc();
        }

        private void OnDrawGizmosSelected()
        {
            // Visualisation de la zone de détection dans l'éditeur
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, detectionRadius);
        }
    }
}