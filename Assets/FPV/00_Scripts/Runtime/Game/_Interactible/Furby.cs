using System.Collections.Generic;
using FPV.Runtime.Shared;
using Unity.Netcode;
using UnityEngine;

namespace FPV.Runtime
{
    public class Furby : NetworkBehaviour, IInteractable
    {
        [Header("Push Settings")] [SerializeField]
        private float explosionRadius = 5f;

        [SerializeField] private float explosionForce = 10f;
        [SerializeField] private LayerMask affectedLayers;

        private NetworkObject netObject;
        private Rigidbody rb;
        private bool hasExploded = false;

        private void Awake()
        {
            CustomNetworkManager.Singleton.OnServerPrepareGame += Init;
            rb = GetComponent<Rigidbody>();
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
                Debug.LogError("Furby must be spawned on the server.");
                Destroy(gameObject);
                return;
            }

            if (!netObject.IsSpawned) netObject.Spawn();
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (!IsServer || hasExploded || rb == null || !rb.isKinematic) return;

            ExplodeServerRpc();
        }

        [ServerRpc]
        private void ExplodeServerRpc()
        {
            if (hasExploded) return;
            hasExploded = true;

            // Explosion effect using SphereCast
            var hits = Physics.OverlapSphere(transform.position, explosionRadius, affectedLayers);
            foreach (var hit in hits)
            {
                var player = hit.GetComponent<PlayerApplication>();
                if (player != null)
                {
                    // Calculate direction and distance for force calculation
                    var direction = (hit.transform.position - transform.position).normalized;
                    var distance = Vector3.Distance(transform.position, hit.transform.position);
                    var force = Mathf.Lerp(explosionForce, 0, distance / explosionRadius);

                    // Use the existing throw mechanics from PlayerController
                    player.Controller.OnPlayerThrowMeRpc(direction, force);
                }
            }

            // Destroy the Furby
            NetworkObject.Despawn(true);
        }

        public void Interact(IInteractable.InteractAction interactAction, Transform interactorTransform)
        {
            Debug.Log($"Furby interacted with action: {interactAction}");
            if (interactAction != IInteractable.InteractAction.Primary) return;
            Debug.Log("Furby picked up");

            var interactorPlayer = interactorTransform.GetComponentInParent<PlayerApplication>();
            if (interactorPlayer == null) return;

            // Transfer ownership to the picking player
            netObject.ChangeOwnership(interactorPlayer.NetworkObject.OwnerClientId);
            GetPickedUpRpc(interactorPlayer.NetworkObjectId);
        }

        [Rpc(SendTo.Owner)]
        private void GetPickedUpRpc(ulong pickerObjectId)
        {
            var pickerTransform = NetworkManager.Singleton.SpawnManager.SpawnedObjects[pickerObjectId].transform;
            transform.position = pickerTransform.position + Vector3.up;
            transform.parent = pickerTransform;
        }

        public Dictionary<IInteractable.InteractAction, string> GetInteractTextDictionary()
        {
            return new Dictionary<IInteractable.InteractAction, string>
            {
                { IInteractable.InteractAction.Primary, "Pick up Furby" }
            };
        }

        public Transform GetTransform()
        {
            return transform;
        }

        public bool CanDoInteractAction(IInteractable.InteractAction interactAction)
        {
            return interactAction == IInteractable.InteractAction.Primary;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, explosionRadius);
        }
    }
}