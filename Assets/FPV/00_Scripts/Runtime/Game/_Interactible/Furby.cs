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

        private bool pickedUp = false;
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
            if (!IsHost)
            {
                Destroy(gameObject);
                return;
            }

            if (!NetworkObject.IsSpawned) NetworkObject.Spawn();
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (!IsServer || hasExploded || rb == null || rb.isKinematic || !pickedUp) return;

            ExplodeServerRpc();
        }

        [ServerRpc]
        private void ExplodeServerRpc()
        {
            if (hasExploded) return;
            hasExploded = true;

            // SphereCast pour détecter les objets à proximité
            var hits = Physics.OverlapSphere(transform.position, explosionRadius, affectedLayers);
            foreach (var hit in hits)
            {
                // Vérifie si c'est un joueur
                var player = hit.GetComponent<PlayerApplication>();
                if (player != null)
                {
                    // Calcul de la direction et application du bump (en utilisant une méthode existante dans le contrôleur)
                    var direction = (hit.transform.position - transform.position).normalized;
                    var force = Mathf.Lerp(explosionForce, 0,
                        Vector3.Distance(transform.position, hit.transform.position) / explosionRadius);
                    player.Controller.OnPlayerThrowMeRpc(direction, force, true);
                }

                // Vérifie si c'est une cible
                var target = hit.GetComponent<Target>();
                if (target != null)
                    // Par exemple : Active la cible s’il est actif
                    target.DeactivateTarget();

                // Vérifie si c'est un Sonographe, active si proche
                var sonographe = hit.GetComponent<Sonographe>();
                if (sonographe != null) sonographe.ActiveClientRpc(); // Appelle la méthode qui gère l'activation
            }

            // Détruire ou désactiver le Furby après impact
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
            NetworkObject.ChangeOwnership(interactorPlayer.NetworkObject.OwnerClientId);
            GetPickedUpRpc(interactorPlayer.NetworkObjectId);
        }

        [Rpc(SendTo.Owner)]
        private void GetPickedUpRpc(ulong pickerObjectId)
        {
            rb.isKinematic = true; // Permet au rigidbody de rester immobile
            pickedUp = true;
            var pickerTransform = NetworkManager.Singleton.SpawnManager.SpawnedObjects[pickerObjectId].transform;
            transform.position = pickerTransform.position + Vector3.up * 0.5f; // Adjust position above the picker
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

        public void Throw(Vector3 direction, float force)
        {
            if (rb != null)
            {
                rb.isKinematic = false; // Permet au rigidbody de prendre le contrôle
                transform.parent = null; // Détache le Furby de son porteur
                rb.AddForce(direction.normalized * force + Vector3.up * 0.5f,
                    ForceMode.Impulse); // Ajoute une impulsion
            }
        }
    }
}