using System.Collections;
using Audio;
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

        private LaserEmitter[] laserEmittersToDeactivate;

        [SerializeField] private GameObject[] objectsToActivate;

        [Header("Settings")] [SerializeField] private LayerMask activationLayers;
        [SerializeField] private bool isActive = true;
        [SerializeField] private float TimeToDeactivate = 5f;

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
            if (!NetworkManager.Singleton.IsHost) return;
            if (!isTargetActive.Value) return;

            // Vérifie si l'objet qui a touché la cible est sur un des layers autorisés
            if ((activationLayers.value & (1 << collision.gameObject.layer)) == 0) return;
            DeactivateTarget();
        }

        public void DeactivateTarget()
        {
            ChangeTargetStateServerRpc(false);
            DeactivateLasersClientRpc();
            ActivateObjectsServerRpc();
            onTargetDeactivated?.Invoke();
<<<<<<< Updated upstream
<<<<<<< Updated upstream

            AudioManager.Instance.PlayOneShot(AudioManager.Instance.target, transform.position);
=======
            
            AudioManager.Instance.PlayOneShot(AudioManager.Instance.target, transform.position, NetworkManager.Singleton.LocalClientId, 5);
>>>>>>> Stashed changes
        }

        [Rpc(SendTo.Server)]
        private void ChangeTargetStateServerRpc(bool newState)
        {
            if (isTargetActive.Value == newState) return;
            isTargetActive.Value = newState;
            // Met à jour l'état de la cible
            isActive = newState;

            // TODO add visual feedback for target state change
            OnTargetStateChanged(!newState, newState);
=======
            
            AudioManager.Instance.PlayOneShot(AudioManager.Instance.target, transform.position, NetworkManager.Singleton.LocalClientId, 5);
>>>>>>> Stashed changes
        }

        private void OnTargetStateChanged(bool previousValue, bool newValue)
        {
            // Met à jour visuellement l'état de la cible
            // TODO add visual feedback for target state change
        }

        [ClientRpc]
        public void DeactivateLasersClientRpc()
        {
            if (lasersToDeactivate != null)
                foreach (var laser in lasersToDeactivate)
                    if (laser != null)
                        laser.DeactivateLaser();
            if (laserEmittersToDeactivate != null)
                foreach (var laserEmitter in laserEmittersToDeactivate)
                    if (laserEmitter != null)
                        laserEmitter.DeactivateLaser();

            // Démarre la coroutine pour réactiver les lasers après un délai
            StartCoroutine(ReactivateLasersAfterDelay(TimeToDeactivate));
        }

        private IEnumerator ReactivateLasersAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            if (lasersToDeactivate != null)
                foreach (var laser in lasersToDeactivate)
                    if (laser != null)
                        laser.ReactivateLaser();
            if (laserEmittersToDeactivate != null)
                foreach (var laserEmitter in laserEmittersToDeactivate)
                    if (laserEmitter != null)
                        laserEmitter.ReactivateLaser();
        }

        [Rpc(SendTo.Server)]
        private void ActivateObjectsServerRpc()
        {
            if (objectsToActivate != null)
                foreach (var obj in objectsToActivate)
                    if (obj != null && NetworkManager.Singleton.IsHost)
                        GetComponent<Door>().TriggerDoorServerRpc();
        }

        private void OnDrawGizmosSelected()
        {
            // Visualisation de la cible dans l'éditeur
            Gizmos.color = isActive ? Color.green : Color.red;
        }
    }
}