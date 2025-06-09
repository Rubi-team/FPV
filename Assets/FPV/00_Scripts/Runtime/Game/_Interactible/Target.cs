using FPV.Runtime.Shared;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

namespace FPV.Runtime
{
    public class Target : NetworkBehaviour
    {
        [Header("References")] [SerializeField]
        private Laser[] lasersToDeactivate;

        [SerializeField] private GameObject[] objectsToActivate;

        [Header("Settings")] [SerializeField] private LayerMask activationLayers;
        [SerializeField] private bool isActive = true;

        private NetworkObject netObject;
        private NetworkVariable<bool> isTargetActive = new(true);

        public UnityEvent onTargetDeactivated;

        private void Awake()
        {
            CustomNetworkManager.Singleton.OnServerPrepareGame += Init;
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            if (!IsServer) enabled = false;

            isTargetActive.OnValueChanged += OnTargetStateChanged;
        }

        public void Init()
        {
            netObject = GetComponent<NetworkObject>();

            if (!IsHost)
            {
                Debug.LogError("Target must be spawned on the server.");
                Destroy(gameObject);
                return;
            }

            if (!netObject.IsSpawned) netObject.Spawn();
            isTargetActive.Value = isActive;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (!IsServer) return;
            if (!isTargetActive.Value) return;

            // Vérifie si l'objet qui a touché la cible est sur un des layers autorisés
            if (((1 << collision.gameObject.layer) & activationLayers) != 0) DeactivateTarget();
        }

        public void DeactivateTarget()
        {
            isTargetActive.Value = false;
            DeactivateLasersClientRpc();
            ActivateObjectsClientRpc();
            onTargetDeactivated?.Invoke();
        }

        private void OnTargetStateChanged(bool previousValue, bool newValue)
        {
            // Met à jour visuellement l'état de la cible
            // Vous pouvez ajouter ici des effets visuels, sons, etc.
            gameObject.SetActive(newValue);
        }

        [ClientRpc]
        public void DeactivateLasersClientRpc()
        {
            if (lasersToDeactivate != null)
                foreach (var laser in lasersToDeactivate)
                    if (laser != null)
                        laser.gameObject.SetActive(false);
        }

        [ClientRpc]
        private void ActivateObjectsClientRpc()
        {
            if (objectsToActivate != null)
                foreach (var obj in objectsToActivate)
                    if (obj != null)
                        obj.SetActive(true);
        }

        private void OnDrawGizmosSelected()
        {
            // Visualisation de la cible dans l'éditeur
            Gizmos.color = isActive ? Color.green : Color.red;
            Gizmos.DrawWireCube(transform.position, Vector3.one);
        }
    }
}