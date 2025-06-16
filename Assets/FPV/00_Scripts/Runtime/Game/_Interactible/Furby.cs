using System;
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

            // Récupérer le premier point de contact
            var contact = collision.contacts[0];

            // Calculer une direction basée sur le point de collision
            var impactDirection = (contact.point - transform.position).normalized;

            // Appliquer une force supplémentaire vers le haut à la direction d'impact
            impactDirection.y += 1.0f; // Ajoute une force verticale (modifiable pour l'intensité)

            // Normaliser la direction finale (pour éviter un déséquilibre dans les forces)
            impactDirection = impactDirection.normalized;

            // Calculer la position précise de l'explosion (léger décalage dans la surface touchée)
            var adjustedImpactPosition = contact.point + contact.normal * -0.5f;

            // Déclencher l'explosion à la position ajustée
            ExplodeServerRpc(adjustedImpactPosition, impactDirection);
        }

        [ServerRpc]
        private void ExplodeServerRpc(Vector3 explosionPosition, Vector3 direction)
        {
            if (hasExploded) return;
            hasExploded = true;

            // SphereCast pour détecter les objets à proximité
            var hits = Physics.OverlapSphere(explosionPosition, explosionRadius, affectedLayers);

            foreach (var hit in hits)
            {
                // Vérifie si c'est un joueur
                var player = hit.GetComponent<PlayerApplication>();
                if (player != null)
                {
                    // Appliquer la direction calculée avec une composante verticale
                    var force = Mathf.Lerp(explosionForce, 0,
                        Vector3.Distance(explosionPosition, hit.transform.position) / explosionRadius);
                    player.Controller.OnPlayerThrowMeRpc(direction, force, true);
                }

                // Vérifie si c'est une cible
                var target = hit.GetComponent<Target>();
                if (target != null) target.DeactivateTarget();

                // Vérifie si c'est un Sonographe
                var sonographe = hit.GetComponentInParent<Sonographe>();
                if (sonographe != null) sonographe.ActivateSonographe();
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
            ChangeOwnershipServerRpc(interactorPlayer.NetworkObjectId);
            GetPickedUpRpc(interactorPlayer.NetworkObjectId);
        }

        [Rpc(SendTo.Server)]
        private void ChangeOwnershipServerRpc(ulong newOwnerId = 0, bool isThrown = false)
        {
            if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(newOwnerId, out var newOwnerObject))
            {
                NetworkObject.ChangeOwnership(newOwnerObject.OwnerClientId);
                if (!isThrown) GetComponent<NetworkObject>().TrySetParent(newOwnerObject);
                else transform.parent = null; // Si lancé, on ne garde pas de parent

                GraphToFollow = newOwnerObject.GetComponent<PlayerApplication>().Model.Graph;
            }
        }

        private Transform GraphToFollow;

        private void Update()
        {
            // Si on est picked Up, update sa position par rapport au GraphToFollow
            if (pickedUp && GraphToFollow != null && rb.isKinematic)
            {
                // Update position to be above and in front of the player
                transform.position = GraphToFollow.position +
                                     GraphToFollow.forward * 1f + Vector3.up * 1f;
                transform.rotation = Quaternion.LookRotation(GraphToFollow.forward, Vector3.up);
            }
        }

        [Rpc(SendTo.Owner)]
        private void GetPickedUpRpc(ulong pickerObjectId)
        {
            rb.isKinematic = true; // Permet au rigidbody de rester immobile
            pickedUp = true;
            var pickerTransform = NetworkManager.Singleton.SpawnManager.SpawnedObjects[pickerObjectId].transform;
            // Make me above and in front of the player
            transform.position = pickerTransform.GetComponent<PlayerApplication>().Model.Graph.position +
                                 pickerTransform.GetComponent<PlayerApplication>().Model.Graph.forward * 1f +
                                 Vector3.up * 1f;

            // Remove the second material of my mesh renderer
            RemoveSecondMaterialRpc();
        }

        [Rpc(SendTo.Everyone)]
        private void RemoveSecondMaterialRpc()
        {
            var meshRenderer = GetComponentInChildren<MeshRenderer>();
            if (meshRenderer != null && meshRenderer.materials.Length > 1)
            {
                var materials = new Material[1];
                materials[0] = meshRenderer.materials[0];
                meshRenderer.materials = materials;
            }
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
                ChangeOwnershipServerRpc(0, true);
                rb.AddForce(direction.normalized * force + Vector3.up * 0.5f,
                    ForceMode.Impulse); // Ajoute une impulsion

                // Trail
                AddTrailEffectRpc();
            }
        }

        [SerializeField] private GameObject TrailEffect;

        [Rpc(SendTo.Everyone)]
        private void AddTrailEffectRpc()
        {
            if (TrailEffect != null)
            {
                var trail = Instantiate(TrailEffect, transform);
                trail.transform.SetParent(transform);
            }
        }
    }
}